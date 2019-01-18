namespace ClampMVC
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class AfterPipeline : AsyncNamedPipelineBase<Func<WebworkContext, CancellationToken, Task>, Action<WebworkContext>>
    {
        private static readonly Task completeTask;

        static AfterPipeline()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(new object());
            completeTask = tcs.Task;
        }

        public AfterPipeline()
        {
        }

        public AfterPipeline(int capacity)
            : base(capacity)
        {
        }

        public static implicit operator Func<WebworkContext, CancellationToken, Task>(AfterPipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator AfterPipeline(Func<WebworkContext, CancellationToken, Task> func)
        {
            var pipeline = new AfterPipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipeline, Func<WebworkContext, CancellationToken, Task> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipeline, Action<WebworkContext> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipelineToAddTo, AfterPipeline pipelineToAdd)
        {
            foreach (var pipelineItem in pipelineToAdd.PipelineItems)
            {
                pipelineToAddTo.AddItemToEndOfPipeline(pipelineItem);
            }

            return pipelineToAddTo;
        }

        public Task Invoke(WebworkContext context, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            var enumerator = this.PipelineDelegates.GetEnumerator();

            if (enumerator.MoveNext())
            {
                ExecuteTasksInternal(context, cancellationToken, enumerator, tcs);
            }
            else
            {
                tcs.SetResult(null);
            }

            return tcs.Task;
        }

        private static void ExecuteTasksInternal(WebworkContext context, CancellationToken cancellationToken, IEnumerator<Func<WebworkContext, CancellationToken, Task>> enumerator, TaskCompletionSource<object> tcs)
        {
            while (true)
            {
                var current = enumerator.Current.Invoke(context, cancellationToken);

                if (current.Status == TaskStatus.Created)
                {
                    current.Start();
                }

                if (current.IsCompleted || current.IsFaulted)
                {
                    // Observe the exception, even though we ignore it, otherwise
                    // we will blow up later
                    var exception = current.Exception;

                    if (enumerator.MoveNext())
                    {
                        continue;
                    }

                    if (current.IsFaulted)
                    {
                        tcs.SetException(current.Exception);
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }

                    break;
                }

                current.ContinueWith(ExecuteTasksContinuation(context, cancellationToken, enumerator, tcs), TaskContinuationOptions.ExecuteSynchronously);
                break;
            }
        }

        private static Action<Task> ExecuteTasksContinuation(WebworkContext context, CancellationToken cancellationToken, IEnumerator<Func<WebworkContext, CancellationToken, Task>> enumerator, TaskCompletionSource<object> tcs)
        {
            return current =>
            {
                // Observe the exception, even though we ignore it, otherwise
                // we will blow up later
                var exception = current.Exception;

                if (enumerator.MoveNext())
                {
                    ExecuteTasksInternal(context, cancellationToken, enumerator, tcs);
                }
                else
                {
                    tcs.SetResult(null);
                }
            };
        }

        /// <summary>
        /// Wraps a sync delegate into it's async form
        /// </summary>
        /// <param name="pipelineItem">Sync pipeline item instance</param>
        /// <returns>Async pipeline item instance</returns>
        protected override PipelineItem<Func<WebworkContext, CancellationToken, Task>> Wrap(PipelineItem<Action<WebworkContext>> pipelineItem)
        {
            var syncDelegate = pipelineItem.Delegate;
            Func<WebworkContext, CancellationToken, Task> asyncDelegate = (ctx, ct) =>
            {
                try
                {
                    syncDelegate.Invoke(ctx);
                    return completeTask;
                }
                catch (Exception e)
                {
                    var tcs = new TaskCompletionSource<object>();
                    tcs.SetException(e);
                    return tcs.Task;
                }
            };
            return new PipelineItem<Func<WebworkContext, CancellationToken, Task>>(pipelineItem.Name, asyncDelegate);
        }
    }
}
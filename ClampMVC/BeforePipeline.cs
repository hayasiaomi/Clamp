﻿namespace ClampMVC
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class BeforePipeline : AsyncNamedPipelineBase<Func<ClampWebContext, CancellationToken, Task<Response>>, Func<ClampWebContext, Response>>
    {
        public BeforePipeline()
        {
        }

        public BeforePipeline(int capacity)
            : base(capacity)
        {
        }

        public static implicit operator Func<ClampWebContext, CancellationToken, Task<Response>>(BeforePipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator BeforePipeline(Func<ClampWebContext, CancellationToken, Task<Response>> func)
        {
            var pipeline = new BeforePipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static BeforePipeline operator +(BeforePipeline pipeline, Func<ClampWebContext, CancellationToken, Task<Response>> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static BeforePipeline operator +(BeforePipeline pipeline, Func<ClampWebContext, Response> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        public static BeforePipeline operator +(BeforePipeline pipelineToAddTo, BeforePipeline pipelineToAdd)
        {
            foreach (var pipelineItem in pipelineToAdd.PipelineItems)
            {
                pipelineToAddTo.AddItemToEndOfPipeline(pipelineItem);
            }

            return pipelineToAddTo;
        }

        public Task<Response> Invoke(ClampWebContext context, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Response>();

            var enumerator = this.PipelineDelegates.GetEnumerator();

            if (enumerator.MoveNext())
            {
                ExecuteTasksWithSingleResultInternal(context, cancellationToken, enumerator, tcs);
            }
            else
            {
                tcs.SetResult(null);
            }

            return tcs.Task;
        }

        private static void ExecuteTasksWithSingleResultInternal(ClampWebContext context, CancellationToken cancellationToken, IEnumerator<Func<ClampWebContext, CancellationToken, Task<Response>>> enumerator, TaskCompletionSource<Response> tcs)
        {
            // Endless loop to try and optimise the "main" use case of
            // our tasks just being delegates wrapped in a task.
            //
            // If they finish executing before returning them we will
            // just loop around this while loop running them one by one,
            // as soon as we have to return, or a task is actually async,
            // then we will bale out and set a continuation.
            while (true)
            {
                var current = enumerator.Current.Invoke(context, cancellationToken);

                if (current.Status == TaskStatus.Created)
                {
                    current.Start();
                }

                if (current.IsCompleted || current.IsFaulted)
                {
                    var resultTask = current;
                    if (!current.IsFaulted)
                    {
                        // Task has already completed, so don't bother with continuations
                        if (ContinueExecution(current.IsFaulted, current.Result, current.Exception))
                        {
                            if (enumerator.MoveNext())
                            {
                                continue;
                            }

                            resultTask = null;
                        }
                    }

                    ExecuteTasksSingleResultFinished(resultTask, tcs);

                    break;
                }

                // Task hasn't finished - set a continuation and bail out of the loop
                current.ContinueWith(ExecuteTasksWithSingleResultContinuation(context, cancellationToken, enumerator, tcs), TaskContinuationOptions.ExecuteSynchronously);
                break;
            }
        }

        private static Action<Task<Response>> ExecuteTasksWithSingleResultContinuation(ClampWebContext context, CancellationToken cancellationToken, IEnumerator<Func<ClampWebContext, CancellationToken, Task<Response>>> enumerator, TaskCompletionSource<Response> tcs)
        {
            return t =>
            {
                if (ContinueExecution(t.IsFaulted, t.IsFaulted ? null : t.Result, t.Exception))
                {
                    if (enumerator.MoveNext())
                    {
                        ExecuteTasksWithSingleResultInternal(context, cancellationToken, enumerator, tcs);
                    }
                    else
                    {
                        ExecuteTasksSingleResultFinished(null, tcs);
                    }
                }
                else
                {
                    ExecuteTasksSingleResultFinished(t, tcs);
                }
            };
        }

        private static void ExecuteTasksSingleResultFinished(Task<Response> task, TaskCompletionSource<Response> tcs)
        {
            if (task == null)
            {
                tcs.SetResult(default(Response));
                return;
            }

            if (task.IsFaulted)
            {
                tcs.SetException(task.Exception);
            }
            else
            {
                tcs.SetResult(task.Result);
            }
        }

        private static bool ContinueExecution(bool isFaulted, Response result, AggregateException exception)
        {
            return !isFaulted && result == null;
        }

        /// <summary>
        /// Wraps a sync delegate into it's async form
        /// </summary>
        /// <param name="pipelineItem">Sync pipeline item instance</param>
        /// <returns>Async pipeline item instance</returns>
        protected override PipelineItem<Func<ClampWebContext, CancellationToken, Task<Response>>> Wrap(PipelineItem<Func<ClampWebContext, Response>> pipelineItem)
        {
            var syncDelegate = pipelineItem.Delegate;
            Func<ClampWebContext, CancellationToken, Task<Response>> asyncDelegate = (ctx, ct) =>
            {
                var tcs = new TaskCompletionSource<Response>();
                try
                {
                    var result = syncDelegate.Invoke(ctx);
                    tcs.SetResult(result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                return tcs.Task;
            };
            return new PipelineItem<Func<ClampWebContext, CancellationToken, Task<Response>>>(pipelineItem.Name, asyncDelegate);
        }
    }
}
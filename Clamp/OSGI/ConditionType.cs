using Clamp.OSGI.Data.Description;
using Clamp.OSGI.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    public abstract class ConditionType
    {
        internal event EventHandler Changed;
        string id;

        /// <summary>
        /// Evaluates the condition.
        /// </summary>
        /// <param name="conditionNode">
        /// Condition node information.
        /// </param>
        /// <returns>
        /// 'true' if the condition is satisfied.
        /// </returns>
        public abstract bool Evaluate(NodeElement conditionNode);

        /// <summary>
        /// Notifies that the condition has changed, and that it has to be re-evaluated.
        /// </summary>
        /// This method must be called when there is a change in the state that determines
        /// the result of the evaluation. When this method is called, all node conditions
        /// depending on it are reevaluated and the corresponding events for adding or
        /// removing extension nodes are fired.
        /// <remarks>
        /// </remarks>
        public void NotifyChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        internal string Id
        {
            get { return id; }
            set { id = value; }
        }
    }

    internal class BaseCondition
    {
        BaseCondition parent;

        internal BaseCondition(BaseCondition parent)
        {
            this.parent = parent;
        }

        public virtual bool Evaluate(TreeClampBundle ctx)
        {
            return parent == null || parent.Evaluate(ctx);
        }

        internal virtual void GetConditionTypes(ArrayList listToFill)
        {
        }
    }

    internal class NullCondition : BaseCondition
    {
        public NullCondition() : base(null)
        {
        }

        public override bool Evaluate(TreeClampBundle ctx)
        {
            return false;
        }
    }

    class OrCondition : BaseCondition
    {
        BaseCondition[] conditions;

        public OrCondition(BaseCondition[] conditions, BaseCondition parent) : base(parent)
        {
            this.conditions = conditions;
        }

        public override bool Evaluate(TreeClampBundle ctx)
        {
            if (!base.Evaluate(ctx))
                return false;
            foreach (BaseCondition cond in conditions)
                if (cond.Evaluate(ctx))
                    return true;
            return false;
        }

        internal override void GetConditionTypes(ArrayList listToFill)
        {
            foreach (BaseCondition cond in conditions)
                cond.GetConditionTypes(listToFill);
        }
    }

    class AndCondition : BaseCondition
    {
        BaseCondition[] conditions;

        public AndCondition(BaseCondition[] conditions, BaseCondition parent) : base(parent)
        {
            this.conditions = conditions;
        }

        public override bool Evaluate(TreeClampBundle ctx)
        {
            if (!base.Evaluate(ctx))
                return false;
            foreach (BaseCondition cond in conditions)
                if (!cond.Evaluate(ctx))
                    return false;
            return true;
        }

        internal override void GetConditionTypes(ArrayList listToFill)
        {
            foreach (BaseCondition cond in conditions)
                cond.GetConditionTypes(listToFill);
        }
    }

    class NotCondition : BaseCondition
    {
        BaseCondition baseCond;

        public NotCondition(BaseCondition baseCond, BaseCondition parent) : base(parent)
        {
            this.baseCond = baseCond;
        }

        public override bool Evaluate(TreeClampBundle ctx)
        {
            return !base.Evaluate(ctx);
        }

        internal override void GetConditionTypes(System.Collections.ArrayList listToFill)
        {
            baseCond.GetConditionTypes(listToFill);
        }
    }


    internal sealed class Condition : BaseCondition
    {
        ExtensionNodeDescription node;
        string typeId;
        ClampBundle addinEngine;
        string addin;

        internal const string SourceBundleFragmentAttribute = "__sourceBundle";

        internal Condition(ClampBundle addinEngine, ExtensionNodeDescription element, BaseCondition parent) : base(parent)
        {
            this.addinEngine = addinEngine;
            typeId = element.GetAttribute("id");
            addin = element.GetAttribute(SourceBundleFragmentAttribute);
            node = element;
        }

        public override bool Evaluate(TreeClampBundle ctx)
        {
            if (!base.Evaluate(ctx))
                return false;

            if (!string.IsNullOrEmpty(addin))
            {
                // Make sure the add-in that implements the condition is loaded
                addinEngine.LoadBundle(addin, true);
                addin = null; // Don't try again
            }

            ConditionType type = ctx.GetCondition(typeId);
            if (type == null)
            {
                addinEngine.ReportError("Condition '" + typeId + "' not found in current extension context.", null, null, false);
                return false;
            }

            try
            {
                return type.Evaluate(node);
            }
            catch (Exception ex)
            {
                addinEngine.ReportError("Error while evaluating condition '" + typeId + "'", null, ex, false);
                return false;
            }
        }

        internal override void GetConditionTypes(ArrayList listToFill)
        {
            listToFill.Add(typeId);
        }
    }
}

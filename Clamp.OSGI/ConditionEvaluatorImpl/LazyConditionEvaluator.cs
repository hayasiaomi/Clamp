using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.ConditionEvaluatorImpl
{
    sealed class LazyConditionEvaluator : IConditionEvaluator
    {
        Bundle addIn;
        string name;
        string className;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public LazyConditionEvaluator(Bundle addIn, AddInProperties properties)
        {
            if (addIn == null)
                throw new ArgumentNullException("addIn");
            this.addIn = addIn;
            this.name = properties["name"];
            this.className = properties["class"];
        }

        public bool IsValid(object parameter, AddInCondition condition)
        {
            IConditionEvaluator evaluator = (IConditionEvaluator)addIn.CreateObject(className);
            if (evaluator == null)
                return false;
            addIn.AddInTree.ConditionEvaluators[name] = evaluator;
            return evaluator.IsValid(parameter, condition);
        }

        public override string ToString()
        {
            return String.Format("[LazyLoadConditionEvaluator: className = {0}, name = {1}]",
                                 className,
                                 name);
        }
    }
}

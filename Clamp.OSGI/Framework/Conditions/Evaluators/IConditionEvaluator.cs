using Clamp.OSGI.Framework.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Conditions.Evaluators
{
    public interface IConditionEvaluator
    {
        bool IsValid(object parameter, AddInCondition condition);
    }
}

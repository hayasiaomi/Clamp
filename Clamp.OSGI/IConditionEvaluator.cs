using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    public interface IConditionEvaluator
    {
        bool IsValid(object parameter, AddInCondition condition);
    }
}

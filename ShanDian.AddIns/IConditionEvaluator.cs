using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns
{
    public interface IConditionEvaluator
    {
        bool IsValid(object parameter, AddInCondition condition);
    }
}

using Clamp.OSGI.Framework.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Framework.Conditions
{
    public class NotCondition : ICondition
    {
        ICondition condition;

        public string Name
        {
            get
            {
                return "Not " + condition.Name;
            }
        }

        ConditionFailedAction action = ConditionFailedAction.Exclude;
        public ConditionFailedAction Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }

        public NotCondition(ICondition condition)
        {
            this.condition = condition;
        }

        public bool IsValid(object parameter)
        {
            return !condition.IsValid(parameter);
        }

        public static ICondition Read(XmlReader reader, IBundle addIn)
        {
            return new NotCondition(AddInCondition.ReadConditionList(reader, "Not", addIn)[0]);
        }
    }
}

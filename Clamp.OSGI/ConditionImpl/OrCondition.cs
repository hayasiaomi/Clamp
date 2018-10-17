using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.ConditionImpl
{
    public class OrCondition : ICondition
    {
        ICondition[] conditions;

        public string Name
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < conditions.Length; ++i)
                {
                    sb.Append(conditions[i].Name);
                    if (i + 1 < conditions.Length)
                    {
                        sb.Append(" Or ");
                    }
                }
                return sb.ToString();
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

        public OrCondition(ICondition[] conditions)
        {
            this.conditions = conditions;
        }

        public bool IsValid(object parameter)
        {
            foreach (ICondition condition in conditions)
            {
                if (condition.IsValid(parameter))
                {
                    return true;
                }
            }
            return false;
        }

        public static ICondition Read(XmlReader reader, Bundle addIn)
        {
            return new OrCondition(AddInCondition.ReadConditionList(reader, "Or", addIn));
        }
    }
}

using Clamp.SDK.Framework.Model;
using System.Collections.Generic;

namespace Clamp.SDK.Framework.Services
{
    public interface IMarkService
    {
         void AddMark(string name, string value);

         void AddMark(string name, string value, string groupName);

        object GetValueByName(string name);

         object GetValueByName(string name, string groupName);

         List<Mark> GetMarks(string name);

         List<object> GetValues(string name);

         List<object> GetValuesByGroupName(string groupName);

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AIM.Helper
{
    public class TaskList
    {
        private object lockinstance = new object();
        // value 0:初始化   1:下载成功  2:下载失败
        public Dictionary<string, int> ttlist = new Dictionary<string, int>();
        public event Action<Dictionary<string, int>> WorkResult;
        public void AddTask(string name)
        {
            lock (lockinstance)
            {
                if (ttlist.ContainsKey(name))
                {
                    return;
                }
                ttlist.Add(name, 0);
            }
        }
        public void SetTaskResult(string name, bool success)
        {
            lock (lockinstance)
            {
                if (!ttlist.TryGetValue(name, out var resu))
                {
                    return;
                }

                ttlist[name] = success ? 1 : 2;
                foreach (var item in ttlist)
                {
                    if (item.Value == 0)
                        return;
                }
            }
            WorkResult?.Invoke(ttlist);
        }
    }
}

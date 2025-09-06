using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tools.Async
{
    public sealed class TaskGroup
    {
        private readonly Action OnComplete;
        private readonly List<Task> Tasks;

        public TaskGroup(Action callback)
        {
            OnComplete = callback;
            Tasks = new List<Task>();
        }

        public void AddTask(Action action)
        {
            if (action == null)
            {
                return;
            }

            var task = Task.Run(action);
            Tasks.Add(task);
        }

        public void ExecuteAll()
        {
            Task.WaitAll(Tasks.ToArray());
            OnComplete.Invoke();
        }

        public void Clear()
        {
            Tasks.Clear();
        }
    }
}
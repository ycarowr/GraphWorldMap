using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tools.Async
{
    public class TaskGroup
    {
        private readonly List<Task> Tasks;
        protected Action OnComplete;
        protected Action OnStart;

        public TaskGroup(Action onStart = null, Action onComplete = null)
        {
            OnStart = onStart;
            OnComplete = onComplete;
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

        protected async void ExecuteAll()
        {
            OnStart?.Invoke();
            await Task.Run(() =>
            {
                Task.WaitAll(Tasks.ToArray());
                OnComplete?.Invoke();
            });
        }

        public void Clear()
        {
            Tasks.Clear();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Tools.Async
{
    public class TaskGroup
    {
        private readonly CancellationTokenSource CancelTokenSource;
        private readonly List<Task> Tasks;
        private readonly int Timeout; // seconds
        private Task CurrentTask;
        protected Action OnComplete;
        protected Action OnStart;

        public TaskGroup(Action onStart = null, Action onComplete = null, int timeout = 10000)
        {
            OnStart = onStart;
            OnComplete = onComplete;
            Tasks = new List<Task>();
            CancelTokenSource = new CancellationTokenSource();
            Timeout = timeout;
        }

        public void AddTask(Action action)
        {
            if (action == null)
            {
                return;
            }

            var task = Task.Run(action, CancelTokenSource.Token);
            Tasks.Add(task);
        }

        protected async void ExecuteAll()
        {
            try
            {
                OnStart?.Invoke();
                SetupTimeout();
                try
                {
                    await Task.WhenAll(Tasks).ContinueWith(task => { OnComplete?.Invoke(); }, CancelTokenSource.Token);
                }
                catch (Exception e)
                {
                    OnComplete?.Invoke();
                    Debug.Log($"Task Exited exception: {e}");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        private void SetupTimeout()
        {
            new Thread(() =>
            {
                Thread.Sleep(Timeout * 1000);
                Cancel();
            }).Start();
        }

        public void Clear()
        {
            Tasks.Clear();
        }

        public void Cancel()
        {
            CancelTokenSource.Cancel();
        }
    }
}
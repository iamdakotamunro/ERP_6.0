using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CMS.WCF.ExceptionService.Task
{
    public class TaskWork
    {
        private static readonly Timer timerTask;
        private static readonly object lockObj = new object();
        private static readonly IDictionary<string, TaskObject> dictTask = new Dictionary<string, TaskObject>();

        static TaskWork()
        {
            timerTask = new Timer(3000);
            timerTask.Elapsed += timerTask_Elapsed;
        }

        public static void Add(string taskName, TaskObject obj)
        {
            lock (lockObj)
            {
                if (!dictTask.ContainsKey(taskName))
                {
                    dictTask.Add(taskName, obj);
                }
            }
        }

        public static void Stop()
        {
            lock (lockObj)
            {
                dictTask.Clear();
            }
            timerTask.Dispose();
        }

        public static void Start()
        {
            timerTask.Start();
        }

        private static void timerTask_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var keyObject in dictTask)
            {
                KeyValuePair<string, TaskObject> obj = keyObject;
                var threadStart = new ThreadStart(delegate
                    {
                        KeyValuePair<string, TaskObject> objInfo = obj;
                        var act = new Action<KeyValuePair<string, TaskObject>>(Execute);
                        act.Invoke(objInfo);
                    });
                Thread thread = new Thread(threadStart);
                thread.Start();
                Thread.Sleep(new Random().Next(10, 1000 * 2));
            }
        }

        private static void Execute(KeyValuePair<string, TaskObject> keyObject)
        {
            string taskName = keyObject.Key;
            TaskObject taskObj = keyObject.Value;
            var allowExecute = false;
            if (!taskObj.IsExecuted)
            {
                var time = (DateTime.Now - taskObj.LastExecuteTime).TotalSeconds;
                if (time>taskObj.ExecuteInterval)
                {
                    allowExecute = true;
                }
            }
            //可以运行
            if (allowExecute)
            {
                lock (lockObj)
                {
                    taskObj.SetState(true);
                }
                var ia = taskObj.Action.BeginInvoke(callback =>
                {
                    while (!callback.IsCompleted)
                    {
                        callback.AsyncWaitHandle.WaitOne(100);
                    }
                }, null);
                taskObj.Action.EndInvoke(ia);
                lock (lockObj)
                {
                    dictTask[taskName].LastExecuteTime = DateTime.Now;
                    taskObj.SetState(false);
                }
            }
        }
    }
}

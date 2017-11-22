using System;

namespace CMS.WCF.ExceptionService.Task
{
    /// <summary>
    /// 任务对象
    /// </summary>
    public class TaskObject
    {
        public Action Action { get; private set; }
        public long ExecuteInterval { get; private set; }
        public DateTime LastExecuteTime { get; set; }

        private bool isExecuted;
        public bool IsExecuted
        {
            get { return isExecuted; }
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="isexecuted"></param>
        public void SetState(bool isexecuted)
        {
            isExecuted = isexecuted;
        }

        public TaskObject(Action action, long interval)
        {
            Action = action;
            ExecuteInterval = interval;
            LastExecuteTime = DateTime.Now;
        }
    }
}

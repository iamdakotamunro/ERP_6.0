using ERP.Environment;
using System;

namespace ERP.UI.Web.Common
{
    [Serializable]
    public class AvoidResubmitHelper
    {
        private readonly Int32 _waitsSecond;
        private DateTime _submitTime;

        protected Guid SubmitID;

        public AvoidResubmitHelper()
        {
            _waitsSecond = 60;
            if (SubmitID == Guid.Empty)
                SubmitID = Guid.NewGuid();
        }

        public AvoidResubmitHelper(Guid guPass)
        {
            _waitsSecond = 60;
            SubmitID = guPass;
        }

        public AvoidResubmitHelper(Guid guPass, Int32 second)
        {
            _waitsSecond = second;
            SubmitID = guPass;
        }

        /// <summary>
        /// 开始提交时执行
        /// </summary>
        public void Submit()
        {
            _submitTime = DateTime.Now;
        }

        /// <summary>
        /// 提交失败时，回滚提交
        /// </summary>
        public void Rollback()
        {
            TimeSpan ts = DateTime.Now - _submitTime;
            if (ts.TotalSeconds >= _waitsSecond)
            {
                return;
            }
            _submitTime = DateTime.Now.AddMilliseconds(GlobalConfig.PageAutoRefreshDelayTime - _waitsSecond * 1000);
        }

        /// <summary>is the submit allowed or not 
        /// </summary>
        public bool Enabled
        {
            get
            {
                TimeSpan ts = DateTime.Now - _submitTime;
                if (ts.TotalSeconds >= _waitsSecond)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 防止重复提交的上下文
        /// </summary>
        public class Context
        {
            public Context()
            {
                IsSucceed = true;
            }

            /// <summary>
            /// 是否执行成功
            /// </summary>
            public bool IsSucceed { get; private set; }

            /// <summary>
            /// 标记为失败
            /// </summary>
            public void SetFail()
            {
                IsSucceed = false;
            }
        }
    }
}
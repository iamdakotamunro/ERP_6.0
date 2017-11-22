using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>
    /// 操作返回信息模型类
    /// </summary>
    [Serializable]
    [DataContract]
    public class ResultModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// 信息
        /// </summary>
        [DataMember]
        public string Message { get; private set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [DataMember]
        public Exception ExceptionInfo { get; private set; }

        public ResultModel()
        {
            IsSuccess = true;
            Message = string.Empty;
        }

        public ResultModel(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public ResultModel(bool isSuccess, string message, Exception exp)
        {
            IsSuccess = isSuccess;
            Message = message;
            ExceptionInfo = exp;
        }
    }
}
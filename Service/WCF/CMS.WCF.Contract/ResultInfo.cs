using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ERP.Service.Contract
{
    /// <summary>
    /// 返回信息
    /// </summary>
    [DataContract]
    public class ResultInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        protected ResultInfo(bool isSuccess = true, string message = "", FaultException exception = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            FaultException = exception;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ResultInfo Success()
        {
            return new ResultInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ResultInfo Failure(string message, FaultException exception = null)
        {
            return new ResultInfo(false, message, exception);
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// 消息日志
        /// </summary>
        [DataMember]
        public string Message { get; private set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [DataMember]
        public FaultException FaultException { get; private set; }
    }

    /// <summary>
    /// 带有返回值的返回信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class ResultInfo<T> : ResultInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        private ResultInfo(T data, bool isSuccess = true, string message = "", FaultException exception = null)
            : base(isSuccess, message, exception)
        {
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ResultInfo<T> Success<T>(T data)
        {
            return new ResultInfo<T>(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ResultInfo<T> Failure<T>(string message, FaultException exception = null)
        {
            return new ResultInfo<T>(default(T), false, message, exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultInfo<T> Failure(string message)
        {
            return new ResultInfo<T>(default(T), false, message);
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        [DataMember]
        public T Data { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class PageResult<T> : ResultInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="total"></param>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        private PageResult(List<T> data,long total, bool isSuccess = true, string message = "", FaultException exception = null)
            : base(isSuccess, message, exception)
        {
            Data = data;
            Total = total;
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        [DataMember]
        public List<T> Data { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long Total { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static PageResult<T> Success<T>(List<T> data,long total)
        {
            return new PageResult<T>(data,total);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PageResult<T> Failure<T>(string message, FaultException exception = null)
        {
            return new PageResult<T>(null,0, false, message, exception);
        }
    }
}

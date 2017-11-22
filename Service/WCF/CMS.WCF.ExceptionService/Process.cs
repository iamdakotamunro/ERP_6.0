using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CMS.WCF.ExceptionService.Dao;
using Framework.Common;
using Keede.Ecsoft.Model;
using CommandInfo = CMS.WCF.ExceptionService.Model.CommandInfo;

namespace CMS.WCF.ExceptionService
{
    public class Process
    {
        private static readonly IDictionary<Guid, string> _fromSourceDict;

        static Process()
        {
            _fromSourceDict = new Dictionary<Guid, string>();
            var list = ExceptionDao.GetFromSources();
            foreach (var info in list)
            {
                if (!_fromSourceDict.ContainsKey(info.Id))
                {
                    _fromSourceDict.Add(info.Id, info.EndPointName);
                }
            }
        }

        public static void ProcessDeleteSuccess()
        {
            var threadStart = new ThreadStart(() => ExceptionDao.DeleteSuccessExceptionByBeforeDay(7));
            var thread = new Thread(threadStart);
            thread.Start();
        }

        public static void ProcessNoSend(int quantity)
        {
            var items = ExceptionDao.GetNoSendCommand(quantity);
            foreach (var commandInfo in items)
            {
                CommandInfo info = commandInfo;
#if debug
                Execute(info);
#else
                var threadStart = new ThreadStart(() => Execute(info));
                var thread = new Thread(threadStart);
                thread.Start();
#endif
            }
        }

        public static void ProcessFailure(int quantity, int failCount)
        {
            var items = ExceptionDao.GetFailureExceptionByFailCount(quantity, failCount);
            foreach (var commandInfo in items)
            {
                CommandInfo info = commandInfo;
#if debug
                Execute(info);
#else
                var threadStart = new ThreadStart(() => Execute(info));
                var thread = new Thread(threadStart);
                thread.Start();
#endif
            }
        }

        #region -- 标记执行返回结果
        /// <summary>
        /// 标记执行返回结果
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="result"></param>
        private static void ProcessSendResult(Guid id, object result)
        {
            if (result != null)
            {
                var exception = result as Exception;
                if (exception != null)
                {
                    var exp = exception;
                    string message = exp.Message;
                    if (exp.InnerException != null)
                    {
                        message += " => " + exp.InnerException.Message;
                    }
                    ExceptionDao.UpdateExceptionCount(id, message);
                }
                else
                {
                    var returnInfo = result as WCFReturnInfo;
                    if (returnInfo != null)
                    {
                        var info = returnInfo;
                        if (!info.IsSuccess)
                        {
                            ExceptionDao.UpdateExceptionCount(id, info.ErrorMessage);
                        }
                        else
                        {
                            ExceptionDao.DeleteExceptionBySuccess(id);
                        }
                    }
                }
            }
        }
        #endregion

        #region -- 二进制类型转换
        /// <summary>
        /// 二进制类型转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static object[] DesObjectArray(byte[] bytes)
        {
            var parms = Serialization.DeserializationObject(bytes);
            return parms as object[];
        }
        #endregion

        #region -- 运行同步数据方法
        /// <summary>
        /// 运行KD的异步数据
        /// </summary>
        /// <param name="commandInfo"> </param>
        private static void Execute(CommandInfo commandInfo)
        {
            var prmObjs = DesObjectArray(commandInfo.CommandParameter);
            if (prmObjs != null)
            {
                string fromSourceName = string.Empty;
                var count = prmObjs.Count();
                if (count > 0)
                {
                    var fromSourceId = (Guid)prmObjs[count - 2];
                    if (_fromSourceDict.ContainsKey(fromSourceId))
                        fromSourceName = _fromSourceDict[fromSourceId].ToLower().Replace("endpoint", string.Empty);
                }

                //处理参数信息
                var arrayList = prmObjs.ToList();
                arrayList.RemoveAt(prmObjs.Count() - 1);
                arrayList.RemoveAt(prmObjs.Count() - 2);

                //默认第一个参数必须是CommandID
                if (arrayList[0] is Guid)
                {
                    if ((Guid)arrayList[0] != commandInfo.CommandID)
                    {
                        arrayList.Insert(0, commandInfo.CommandID);
                    }
                }
                else
                {
                    arrayList.Insert(0, commandInfo.CommandID);
                }

                var prms = arrayList.ToArray();

                object result;
                try
                {
                    if (fromSourceName == string.Empty)
                    {
                        fromSourceName = "eyesee";
                    }

                    //#region -- 临时处理订单同步
                    //if (commandInfo.CommandMethod == "UpdateManyOrderState")
                    //{
                    //    var ids = prms[1] as List<Guid>;
                    //    var state = prms[2] is OrderState ? (OrderState) prms[2] : OrderState.Consignmented;
                    //    if (ids != null)
                    //    {
                    //        foreach (var id in ids)
                    //        {
                    //            result = Execute(fromSourceName, "UpdateOrderState", Guid.NewGuid(), id, state);
                    //            //Thread.Sleep(10);
                    //        }
                    //    }
                    //}
                    //#endregion

                    result = Execute(fromSourceName, commandInfo.CommandMethod, prms);
                }
                catch (Exception expInfo)
                {
                    result = expInfo;
                }
                ProcessSendResult(commandInfo.CommandID, result);
            }
        }

        /// <summary>
        /// 执行方法，同步数据
        /// </summary>
        /// <param name="toEndPoint"></param>
        /// <param name="methodName"></param>
        /// <param name="prmObjs"></param>
        /// <returns></returns>
        private static object Execute(string toEndPoint, string methodName, params object[] prmObjs)
        {
            switch (toEndPoint)
            {
                case "eyesee":
                    using (var client = ServiceFactory.WCF.EyeseeClient)
                        return client.DynamicExecuteServiceByResult(methodName, prmObjs);
                case "keede":
                    using (var client = ServiceFactory.WCF.NewKeedeClient)
                        return client.DynamicExecuteServiceByResult(methodName, prmObjs);
                case "HRS":
                    using (var client = ServiceFactory.WCF.KeedeMisClient)
                        return client.DynamicExecuteServiceByResult(methodName, prmObjs);
                case "baishop":
                    using (var client = ServiceFactory.WCF.BaishopClient)
                        return client.DynamicExecuteServiceByResult(methodName, prmObjs);
                default:
                    using (var client = ServiceFactory.WCF.NewKeedeClient)
                        return client.DynamicExecuteServiceByResult(methodName, prmObjs);
            }
        }
        #endregion
    }
}

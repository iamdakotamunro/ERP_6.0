using System;
using ERP.Environment;
using System.ServiceModel;
using ERP.Service.Contract;
using Framework.WCF.Model;
using Config.Keede.Library;

namespace ERP.Service.Implement
{
    [ServiceBehavior]
    public partial class Service : IService
    {
        static Service()
        {
            //设置推送数据的连接字符串名称
            PUSH.Core.Instance.Init.SetDataBaseConnectionString("System.Data.SqlClient", ConfManager.GetAppsetting("db_ERP_WriteConnection"));
        }

        internal TResult Get<TResult>(Func<TResult> func)
        {
            try
            {
                return func();
            }
            catch (Exception exp)
            {
                SAL.LogCenter.LogService.LogError(exp.Message, "基础服务", exp);
                return default(TResult);
            }
        }

        internal WCFReturnInfo Execute<TResult>(Guid pushDataId, Func<TResult> func)
        {
            if (pushDataId == Guid.Empty)
            {
                return new WCFReturnInfo(false, false, pushDataId, null, "方法：" + func.Method.Name + "，推送数据ID不能为空!");
            }

            lock (this)
            {
                //如果commandid存在，且已执行成功，则直接返回true，表示该命令已经被执行；否则操作
                if (PUSH.Core.Instance.ExistExecuted(pushDataId))
                {
                    return new WCFReturnInfo(true, true, pushDataId, null, "服务验证方法已经执行!");
                }
            }

            string message = string.Empty;

            try
            {
                TResult result = func();
                lock (this)
                {
                    PUSH.Core.Instance.AddExecuted(pushDataId);
                }
                var returnInfo = new WCFReturnInfo(true, PUSH.Core.Instance.ExistExecuted(pushDataId), pushDataId,
                                                   result,
                                                   message);
                return returnInfo;
            }
            catch (Exception exp)
            {
                message = exp.Message;
                if (exp.InnerException != null)
                {
                    message += @"\r\n" + exp.InnerException.Message;
                }
                SAL.LogCenter.LogService.LogError(string.Format("执行失败：{0}, pushDataId={1}", exp.Message, pushDataId), "基础服务", exp);
                return new WCFReturnInfo(false, false, pushDataId, null, message);
            }
        }

        internal WCFReturnInfo Execute(Guid pushDataId, Action act)
        {
            if (pushDataId == Guid.Empty)
            {
                return new WCFReturnInfo(false, false, pushDataId, null, "方法：" + act.Method.Name + "，推送数据ID不能为空!");
            }

            //如果commandid存在，且已执行成功，则直接返回true，表示该命令已经被执行；否则操作
            if (PUSH.Core.Instance.ExistExecuted(pushDataId))
            {
                return new WCFReturnInfo(true, true, pushDataId, null, "服务验证方法已经执行!");
            }
            string message = string.Empty;

            //using (var tran = new System.Transactions.TransactionScope())
            //{
            try
            {
                act();
                PUSH.Core.Instance.AddExecuted(pushDataId);
                var returnInfo = new WCFReturnInfo(true, PUSH.Core.Instance.ExistExecuted(pushDataId), pushDataId,
                                                   null,
                                                   message);
                //tran.Complete();
                return returnInfo;
            }
            catch (Exception exp)
            {
                message = exp.Message;
                if (exp.InnerException != null)
                {
                    message += @"\r\n" + exp.InnerException.Message;
                }
                SAL.LogCenter.LogService.LogError(string.Format("执行失败：{0}, pushDataId={1}", exp.Message, pushDataId), "基础服务", exp);
                return new WCFReturnInfo(false, false, pushDataId, null, message);
            }
            //}
        }

        internal WCFReturnInfo Execute(Action act)
        {
            string message = string.Empty;
            try
            {
                act();
                return new WCFReturnInfo(true, true, Guid.Empty, null, message);
            }
            catch (Exception exp)
            {
                message = exp.Message;
                if (exp.InnerException != null)
                {
                    message += @"\r\n" + exp.InnerException.Message;
                }
                SAL.LogCenter.LogService.LogError(exp.Message, "基础服务", exp);
                return new WCFReturnInfo(false, false, Guid.Empty, null, message);
            }
        }

        internal WCFReturnInfo Execute<TResult>(Func<TResult> func)
        {
            string message = string.Empty;
            try
            {
                var result = func();
                return new WCFReturnInfo(true, true, Guid.Empty, result, message);
            }
            catch (Exception exp)
            {
                message = exp.Message;
                if (exp.InnerException != null)
                {
                    message += @"\r\n" + exp.InnerException.Message;
                }

                SAL.LogCenter.LogService.LogError(exp.Message, "基础服务", exp);
                return new WCFReturnInfo(false, false, Guid.Empty, null, message);
            }
        }
    }
}

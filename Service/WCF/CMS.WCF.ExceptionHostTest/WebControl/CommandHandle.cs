using System;
using System.Collections;
using System.Collections.Generic;
using Keede.Ecsoft.Enum;
using Keede.Ecsoft.Model;
using Keede.SynService;
using LonmeShop.CommandBLL;
using MIS.WCF.Contract;


public class CommandHandle
{
    readonly Command com = new Command();

    public void Handle()
    {
        IList<CommandInfo> info = com.GetNoSuccess();
        var cInfo = new CommandInfo();
        for (int i = 0; i < info.Count; i++)
        {
            //string转换成Enum枚举类型
            try
            {
                //var type = (EyeseeMethodName)Enum.Parse(typeof(EyeseeMethodName), info[i].CommandMethod, true);
                var prm = Function.DeserializationObject(info[i].CommandParameter);
                cInfo = info[i];

                var prms = prm as object[];//as如果是object[]强制转换,如是不是返回NULL
                if (prms != null)
                {
                    HandleExceptionExchange(ref cInfo, prms);//交互命令 
                }
                else
                {
                    HandleExceptionExchange(ref cInfo, new[] { prm });//交互命令
                }
            }
            catch (Exception ex)
            {
                cInfo.Exception = ex.Message;
                com.UpdateExceptionMsg(cInfo); //改变发送次数
            }
        }
    }

    /// <summary>
    /// 交互命令
    /// </summary>
    private void HandleExceptionExchange(ref CommandInfo commdInfo, params object[] prms)
    {
        try
        {
            var arrayList = new ArrayList(prms);
            int count = prms.Length;
            var fromSourceId = (Guid)prms[count - 2];

          MIS.WCF.Contract.  ServiceMethodName misType=0;
            EyeseeMethodName eyeType=0;
            if (fromSourceId.ToString().ToUpper() == "65070EB0-F9ED-4028-AEA2-6E2BA200F85A")
            {
                misType = (ServiceMethodName)Enum.Parse(typeof(ServiceMethodName), commdInfo.CommandMethod, true);
            }
            else
            {
                eyeType = (EyeseeMethodName)Enum.Parse(typeof(EyeseeMethodName), commdInfo.CommandMethod, true);
            }

            arrayList.RemoveAt(count - 1);
            arrayList.RemoveAt(count - 2);

            if (typeof(Guid) == prms[0].GetType()) //判断是不是最后一个参数
            {
                if ((Guid)prms[0] != commdInfo.CommandID) //如果最后已经插入GUID
                {
                    arrayList.Add(commdInfo.CommandID);
                }
            }
            else
            {
                arrayList.Add(commdInfo.CommandID);
            }

            prms = arrayList.ToArray();
            if (commdInfo.CommandMethod == "UpdateOrderState")
            {
                var psm = prms[1] as object[];
                if (psm != null && psm.Length > 1)
                {
                    var pps = new[] { prms[0], psm[0], psm[1] };
                    prms = pps;
                }
            }

            object returnInfo;
            if (fromSourceId.ToString().ToUpper() == "65070EB0-F9ED-4028-AEA2-6E2BA200F85A")
            {
                using (var syn = new MISSynchronous(fromSourceId))
                {
                    returnInfo = syn.SyncFunc(misType, prms); //发送请求并返回结果
                }
            }
            else
            {
                using (var syn = new Synchronous(fromSourceId))
                {
                    returnInfo = syn.SyncFunc(eyeType, prms); //发送请求并返回结果
                }
            }
            var wInfo = returnInfo as WCFReturnInfo;

            //1.如果处理失败,
            if (wInfo == null || !wInfo.IsSuccess)
            {
                commdInfo.Exception = wInfo == null ? "请求服务超时!" : wInfo.ErrorMessage.Length >= 1000 ? wInfo.ErrorMessage.Substring(0, 999) : wInfo.ErrorMessage;
                com.UpdateExceptionCount(commdInfo); //改变发送次数
            }
            else //2.如果处理成功,删除这条数据.. 
            {
                com.DeleteCommand(commdInfo.CommandID);
            }
        }
        catch (System.ServiceModel.FaultException<WcfException> fe)
        {
            commdInfo.Exception = fe.Detail.Message.Length >= 1000
                                      ? fe.Detail.Message.Substring(0, 999)
                                      : fe.Detail.Message;
            com.UpdateExceptionCount(commdInfo); //改变发送次数
        }
        catch (Exception ex)
        {
            commdInfo.Exception = ex.Message.Length >= 1000
                                      ? ex.Message.Substring(0, 999)
                                      : ex.Message;
            com.UpdateExceptionCount(commdInfo); //改变发送次数
        }
    }

}


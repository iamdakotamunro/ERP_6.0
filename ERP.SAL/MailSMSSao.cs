using System;
using System.Collections.Generic;
using NotificationCenter.ReceiveMessage.API.Client;
using NotificationCenter.DTO.ReceiveMessage;
using System.Configuration;
using ERP.SAL.LogCenter;
using Newtonsoft.Json;
using Config.Keede.Library;

namespace ERP.SAL
{
    public class MailSMSSao
    {
        /// <summary>发送短信 eg:SendShortMessage('13761746053','短信内容')
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="mobile">电话号码</param>
        /// <param name="msg">消息</param>
        public static string SendShortMessage(Guid saleFilialeId, Guid salePlatformId, string mobile, string msg)
        {
            var send = new SimpleNotificationPushClient(ConfManager.GetAppsetting("NotificationPushApiUrl"), ConfManager.GetAppsetting("NotificationPushSignKey"),new JsonSerialize());
            var result=send.Push(new SimpleMessageTaskDTO
            {
                ExpectNotifyTime = DateTime.Now,
                MessageContent = new MessageContentDTO
                {
                    Content = msg,
                    MessageType = 1,
                    Signature = "会员提现"
                },
                SaleFilialeID = saleFilialeId,
                SalePlatformID = salePlatformId,
                TaskID = Guid.NewGuid(),
                IsGroupSend = false,
                Targets = new List<NotificationTargetDTO>
                {
                    new NotificationTargetDTO
                    {
                        IsVoiceType = false,
                        JsonParameter = null,
                        TargetNo = mobile,
                        TargetType = 1
                    }
                }
            });
            return result != null ? result.IsSuccess?"" : result.FailMessage:"发送短信失败！";
        }
    }

    public class JsonSerialize : NotificationCenter.ReceiveMessage.API.Client.IJsonSerialize
    {
        public T Deserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                LogService.LogError(json, "发送短信", ex);
            }
            return JsonConvert.DeserializeObject<T>("");
        }

        public string Serialize(object value)
        {
            try
            {
                return JsonConvert.SerializeObject((value));
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.Message, "发送短信", ex);
            }
            return JsonConvert.SerializeObject((""));
        }
    }
}

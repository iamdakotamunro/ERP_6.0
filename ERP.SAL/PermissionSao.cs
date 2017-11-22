using System;

namespace ERP.SAL
{
    public class PermissionSao
    {
        public static Model.LoginResultInfo VerifyLoginToken(string token)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var info = client.Instance.VerifyLoginToken(token);
                if (info == null)
                {
                    return new Model.LoginResultInfo(new MIS.Model.View.LoginResultInfo("登录口令验证通信失败"));
                }
                return new Model.LoginResultInfo(info);
            }
        }

        public static bool VerifyIsAllowVisitPage(Guid personnelId, Guid systemId, string pageUrl)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                return client.Instance.VerifyPageIsVisitPermission(personnelId, systemId, pageUrl);
            }
        }

        public static bool VerifyIsAllowPageOperation(Guid personnelId, Guid systemId, string pageUrl, string pointCode)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                //return true;
                return client.Instance.VerifyPageOperationIsVisitPermission(personnelId, systemId, pageUrl, pointCode);
            }
        }
    }
}

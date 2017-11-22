namespace ERP.SAL
{
    public class AccountSao
    {
        /// <summary>
        /// 获取所有销售平台集合
        /// </summary>
        /// <returns></returns>
        public static Model.LoginResultInfo Login(string userName, string password)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var info = client.Instance.Login(userName, password);
                if (info == null)
                {
                    return new Model.LoginResultInfo(new MIS.Model.View.LoginResultInfo("登录验证通信失败"));
                }
                return new Model.LoginResultInfo(info);
            }
        }
    }
}

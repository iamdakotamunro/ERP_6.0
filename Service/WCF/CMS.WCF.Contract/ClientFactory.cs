using Framework.WCF;

namespace ERP.Service.Contract
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientFactory
    {
        /// <summary>
        /// 终结点名称：Group.ERP
        /// </summary>
        public static string EndPointName
        {
            get { return "Group.ERP"; }
        }

        //static void LogException(Exception exp, string method, params object[] parameters)
        //{
        //    var msg = new StringBuilder("参数：" + Serialization.JsonSerialize(parameters) + "\r\n" + exp.Message + "\r\n" + exp.StackTrace);
        //    if (exp.InnerException != null)
        //    {
        //        msg.Append("\r\n" + exp.InnerException.Message + "\r\n" + exp.InnerException.StackTrace);
        //    }

        //    SAL.LogCenter.LogService.LogError(msg.ToString(), "客户端异常", exp);
        //}

        internal static ServiceClient<IService> CreateClient()
        {
            return new ServiceClient<IService>(EndPointName);
        }
    }
}

using ERP.DAL.Interface.IOrder;
using Framework.Common;
using KeedeGroup.ThirdParty.Logistics;
using System;
using System.Text;
using ERP.Environment;

namespace SF
{
    public class SFClass
    {
        private static readonly IGoodsOrder _goodsOrderWrite = new ERP.DAL.Implement.Order.GoodsOrder(ERP.Environment.GlobalConfig.DB.FromType.Write);

        /// <summary>
        /// 更新快递编号
        /// </summary>
        public static void RunSFClassTask(int num)
        {
            StringBuilder errorMsg = new StringBuilder();
            var baseOrderInfoList = _goodsOrderWrite.GetBaseOrderInfoForExpress(num);
            foreach (var baseOrderInfo in baseOrderInfoList)
            {
                baseOrderInfo.D_Address = FilterSpecialStr(baseOrderInfo.D_Address);
                baseOrderInfo.D_Contact = FilterSpecialStr(baseOrderInfo.D_Contact);
                Config.InitConnectionName(GlobalConfig.DB.GetConnectionString(GlobalConfig.DB.FromType.Write));
                bool isTrue;
                var resultMailNo = Client.CheckExpressIdReturnMailNo(baseOrderInfo, out isTrue);
                if (isTrue)
                {
                    bool result = _goodsOrderWrite.UpdateOrderExpressNo(baseOrderInfo.OrderId, resultMailNo);
                    if (!result)
                    {
                        errorMsg.Append("订单：" + baseOrderInfo.OrderId + "更新快递编号失败(快递编号：" + resultMailNo + ")！");
                    }
                }
            }
            if (!string.IsNullOrEmpty(errorMsg.ToString()))
            {
                LogHelper.Log("RunSFClassTask.Error", "更新快递编号失败", errorMsg.ToString());
            }
        }

        /// <summary>过滤特殊字符串
        /// </summary>
        /// <returns></returns>
        public static String FilterSpecialStr(String theString)
        {
            theString = theString.Replace("&middot;", "");
            theString = theString.Replace("&nbsp;", "");
            theString = theString.Replace("&mdash;", "");
            theString = theString.Replace("&ldquo;", "");
            theString = theString.Replace("&rdquo;", "");
            theString = theString.Replace("&gt;", "");
            theString = theString.Replace("&lt;", "");
            theString = theString.Replace("&quot;", "");
            theString = theString.Replace("&apos;", "");
            theString = theString.Replace("&amp;", "");
            theString = theString.Replace(">", "");
            theString = theString.Replace("<", "");
            theString = theString.Replace("》", "");
            theString = theString.Replace("《", "");
            theString = theString.Replace("&", "");
            theString = theString.Replace("'", "");
            theString = theString.Replace("‘", "");
            return theString;
        }
    }
}

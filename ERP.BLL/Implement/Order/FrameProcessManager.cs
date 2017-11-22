using System;
using System.Linq;
using System.Text;
using ERP.Model;
using ERP.SAL;
using ERP.DAL.Interface.IOrder;
namespace ERP.BLL.Implement.Order
{
    public class FrameProcessManager
    {
        private const string RIGHT = "R";
        private const string RIGHT2 = "右眼";
        private const string LEFT = "L";
        private const string LEFT2 = "左眼";

        private static readonly WMSSao _wmsSao = new WMSSao();
        private static readonly IGoodsOrder _goodsOrderWrite = new DAL.Implement.Order.GoodsOrder(Environment.GlobalConfig.DB.FromType.Write);

        public FrameProcessCertificateInfo GetCertificateInfo(string processNo)
        {
            var info = new FrameProcessCertificateInfo();
            var processInfo = _wmsSao.GetProcessOrder(processNo);
            if (processInfo != null)
            {
                var orderInfo = _goodsOrderWrite.GetGoodsOrder(processInfo.OrderNos.First());
                info.Optician = processInfo.Processor;
                info.OperationTime = processInfo.ProcessDate;
                info.Consignee = processInfo.Name;
                info.OrderId = (orderInfo != null && orderInfo.OrderId != default(Guid)) ? orderInfo.OrderId : Guid.Empty;
                info.ProcessNo = processInfo.ProcessNo;
                info.SaleFilialeID = processInfo.HostingFilialeId;
                var skuList = processInfo.SkuList;

                #region[右眼SKU处理]
                var rightBuilder = new StringBuilder();
                var rightInfo = skuList.FirstOrDefault(ent => ent.Contains(RIGHT) || ent.Contains(RIGHT2));
                if (rightInfo != null)
                {
                    string[] spfArray = rightInfo.Split(' ');
                    rightBuilder.Append("右眼:").Append(spfArray[1].Substring(spfArray[1].IndexOf(':') + 1).Replace("无", "-"));
                    if (spfArray.Length >= 3)
                    {
                        rightBuilder.Append(" ");
                        rightBuilder.Append(spfArray[2].Replace("无", "-"));
                    }
                    if (spfArray.Length >= 4)
                    {
                        rightBuilder.Append(" ");
                        rightBuilder.Append(spfArray[3].Replace("无", "-"));
                    }
                    if (spfArray.Length >= 5)
                    {
                        if (string.IsNullOrEmpty(info.PD))
                        {
                            info.PD = spfArray[4].Trim();
                        }
                    }
                }
                info.RightEyeInfo = rightBuilder.ToString();
                #endregion

                #region [左眼SKU处理]
                var leftBuilder = new StringBuilder();
                var leftInfo = skuList.FirstOrDefault(ent => ent.Contains(LEFT) || ent.Contains(LEFT2));
                if (leftInfo != null)
                {
                    var lspfArray = leftInfo.Split(' ');
                    leftBuilder.Append("左眼:").Append(lspfArray[1].Substring(lspfArray[1].IndexOf(':') + 1).Replace("无", "-"));
                    if (lspfArray.Length >= 3)
                    {
                        leftBuilder.Append(" ");
                        leftBuilder.Append(lspfArray[2].Replace("无", "-"));
                    }
                    if (lspfArray.Length >= 4)
                    {
                        leftBuilder.Append(" ");
                        leftBuilder.Append(lspfArray[3].Replace("无", "-"));
                    }
                }
                info.LeftEyeInfo = leftBuilder.ToString();
                #endregion
            }
            return info;
        }
    }
}

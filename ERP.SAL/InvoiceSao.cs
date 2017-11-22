using System;
using ERP.Enum;

namespace ERP.SAL
{
    public class InvoiceSao
    {
        /// <summary>获取发票信息
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public static B2C.Model.ERPExtensionModel.InvoiceInfo GetInvoiceInfo(Guid saleFilialeId, Guid orderId)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                return client.Instance.GetInvoiceInfo(orderId);
            }

        }

        /// <summary>改变发票状态
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="invoiceState"></param>
        /// <param name="flag">true 更新订单表的发票状态和发票表的发票状态，false 只更新发票表的发票状态</param>
        /// <param name="clew"></param>
        public static void SetInvoiceState(Guid saleFilialeId, Guid invoiceId, InvoiceState invoiceState, Boolean flag,String clew)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                client.Instance.SetInvoiceState(Guid.NewGuid(), invoiceId, (Int32)invoiceState, flag, clew);
            }
        }

    }
}

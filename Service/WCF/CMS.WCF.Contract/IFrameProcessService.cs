using System.Collections.Generic;
using System.ServiceModel;
using ERP.Model;
using ERP.Model.Goods;

namespace ERP.Service.Contract
{
    public partial interface IService
    {
        /// <summary>打印合格证获取加工单信息
        /// </summary>
        /// <param name="processNo">加工单号</param>
        /// <returns></returns>
        [OperationContract]
        FrameProcessCertificateInfo GetFrameProcessCertificateInfo(string processNo);

        /// <summary> 获取出库商品条码信息(框架相关)
        /// </summary>
        /// <param name="tracodeNo"></param>
        /// <returns></returns>
        [OperationContract]
        IList<GoodsBarcodeInfo> GetOutStockGoodsBarcode(string tracodeNo);
    }
}

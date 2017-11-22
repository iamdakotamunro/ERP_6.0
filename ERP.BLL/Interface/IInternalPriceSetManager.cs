using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.BLL.Interface
{
    /// <summary>
    /// 内部采购价设置 业务层代码
    /// </summary>
    public interface IInternalPriceSetManager
    {
        /// <summary>
        /// 获取内部采购价
        /// </summary>
        /// <param name="hostingFilialeId"></param>
        /// <param name="goodsIdGoodsTypeDict"></param>
        /// <returns></returns>
        Dictionary<Guid, decimal> GetInternalPurchasePriceByHostingFilialeIdGoodsIds(Guid hostingFilialeId, IDictionary<Guid, int> goodsIdGoodsTypeDict);
    }
}

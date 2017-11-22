using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.Interface
{
    /// <summary>
    /// 促销接口
    /// </summary>
    public interface IPromotionSao
    {
        IDictionary<string, string> GetGoodsSalePromotionDict(Guid goodsId, DateTime startTime,
            DateTime endTime);

        Dictionary<Guid, decimal> GetGoodsPriceDict(Guid orderId);

    }
}

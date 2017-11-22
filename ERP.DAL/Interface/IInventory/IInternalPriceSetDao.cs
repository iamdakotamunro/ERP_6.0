using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IInternalPriceSetDao
    {
        /// <summary>查询
        /// </summary>
        /// <param name="goodstype"></param>
        /// <returns></returns>
        IList<InternalPriceSetInfo> GetInternalPriceSetInfoList(int goodstype);

        bool UpdateInternalPriceSetInfo(int goodsType, Guid hostingFilialeId, string reserveProfitRatio);

        InternalPriceSetInfo GetInternalPriceSetInfoList(int goodstype, Guid hostingFilialeId);

        Dictionary<Int32, decimal> GetGoodsTypeInternalPriceSets(Guid hostingFilialeId,
            IEnumerable<Int32> goodsTypes);
    }
}

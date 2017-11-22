using System;
using System.Collections.Generic;
using ERP.Model.Goods;

namespace ERP.SAL.Interface
{
    public partial interface IGoodsCenterSao
    {
        #region [申请商品卖库存]

        /// <summary>申请商品卖库存
        /// </summary>
        /// <param name="goodSaleStockInfo"></param>
        /// <param name="personId"> </param>
        /// <param name="errorMessage"></param>
        /// <param name="operatorName"> </param>
        /// <returns></returns>
        bool ApplyGoodsSaleStock(GoodsSaleStockInfo goodSaleStockInfo, string operatorName,
            Guid personId, out string errorMessage);
        #endregion

        #region [审核商品卖库存]

        /// <summary>审核商品卖库存
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="saleStockState"></param>
        /// <param name="operatorName"> </param>
        /// <param name="auditor"></param>
        /// <param name="auditReason"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool AuditGoodsSaleStock(Guid goodsId, int saleStockState, string operatorName, Guid auditor,
            string auditReason, out string errorMessage);
        #endregion

        #region [根据商品ID获取卖库存信息]

        /// <summary>根据商品ID获取卖库存信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        GoodsSaleStockInfo GetGoodsSaleStockInfoByGoodsId(Guid goodsId);
        #endregion


        #region [搜索卖库存商品(分页)]

        /// <summary> 搜索卖库存商品(分页)
        /// </summary>
        /// <param name="goodsNameOrCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IList<GoodsSaleStockGridModel> GetGoodsSaleStockListToPage(string goodsNameOrCode,
            int pageIndex, int pageSize, out int totalCount);

        #endregion
    }
}

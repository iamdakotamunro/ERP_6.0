using ERP.Model.FinanceModule;
using System;
using System.Collections.Generic;

namespace ERP.DAL.Interface.FinanceModule
{
    /// <summary>
    /// 销售订单商品结算价 数据访问层接口
    /// </summary>
    public interface ISaleOrderGoodsSettlementDal
    {
        /// <summary>
        /// 获取未保存结算价的B2C订单列表
        /// </summary>
        /// <param name="statisticDate"></param>
        /// <param name="maxRowCount"></param>
        /// <returns></returns>
        IList<SampleSaleOrderInfo> GetUnsavedB2COrderList(DateTime statisticDate, int maxRowCount);

        /// <summary>
        /// 获取未保存结算价的销售出库给门店列表
        /// </summary>
        /// <param name="statisticDate"></param>
        /// <param name="maxRowCount"></param>
        /// <returns></returns>
        IList<SampleSaleOrderInfo> GetUnsavedSaleStockOutForShopList(DateTime statisticDate, int maxRowCount);

        /// <summary>
        /// 获取未保存结算价的销售出库给销售公司列表
        /// </summary>
        /// <param name="statisticDate"></param>
        /// <param name="maxRowCount"></param>
        /// <returns></returns>
        IList<SampleSaleOrderInfo> GetUnsavedSaleStockOutForSaleFilialeList(DateTime statisticDate, int maxRowCount);

        /// <summary>
        /// 获取未保存结算价的销售出库给销售公司列表
        /// </summary>
        /// <param name="statisticDate"></param>
        /// <param name="maxRowCount"></param>
        /// <returns></returns>
        IList<SampleSaleOrderInfo> GetUnsavedSaleStockOutForHostingFilialeList(DateTime statisticDate, int maxRowCount);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="info"></param>
        void Save(SaleOrderGoodsSettlementInfo info);
    }
}

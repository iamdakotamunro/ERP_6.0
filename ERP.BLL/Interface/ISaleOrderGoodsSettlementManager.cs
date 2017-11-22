using ERP.Enum;
using ERP.Model.FinanceModule;
using System;
using System.Collections.Generic;

namespace ERP.BLL.Interface
{
    /// <summary>
    /// 销售订单商品结算价 业务层
    /// </summary>
    public interface ISaleOrderGoodsSettlementManager
    {
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="statisticDate">统计日期</param>
        /// <param name="maxRowCount">一次取的数据量</param>
        void Generate(DateTime statisticDate, int maxRowCount);

        /// <summary>
        /// 创建销售订单商品结算价
        /// </summary>
        /// <param name="orderInfo">关联的订单</param>
        /// <returns></returns>
        IList<SaleOrderGoodsSettlementInfo> Create(SampleSaleOrderInfo orderInfo);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="item"></param>
        void Save(SaleOrderGoodsSettlementInfo item);
    }
}

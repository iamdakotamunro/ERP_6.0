using ERP.Model.Report;
using System;
using System.Collections.Generic;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICompanyGrossProfitRecordDetail
    {
        /// <summary>
        /// 是否存在公司毛利记录数据明细
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        bool Exists(DateTime dayTime);

        /// <summary>
        /// 批量插入公司毛利记录明细
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddDataDetail(IList<CompanyGrossProfitRecordDetailInfo> list);

        /// <summary>
        /// 根据订单时间修改数据状态
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        bool UpdateState(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 更新每天产生的交易佣金
        /// </summary>
        /// <returns></returns>
        /// zal 2016-09-20
        bool UpdateCatCommission();

        /// <summary>
        /// 根据条件合计公司毛利明细表数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        IList<CompanyGrossProfitRecordInfo> SumCompanyGrossProfitDetailInfos(DateTime startTime, DateTime endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes);

        /// <summary>
        /// 查询公司毛利中超过一个自然月或一个自然月以上未完成的数据（例如：当前是7月份，订单时间是5月份的订单在7月1号之前没有完成的数据）
        /// </summary>
        /// <param name="dayTime">完成时间</param>
        /// <returns></returns>
        IList<CompanyGrossProfitRecordInfo> GetCompanyGrossProfitDetailInfosForMoreMonth(DateTime dayTime);

        /// <summary>
        /// 汇总同一公司不同平台的数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        /// zal 2017-07-17
        IList<CompanyGrossProfitRecordInfo> SumCompanyGrossProfitBySaleFilialeId(DateTime startTime, DateTime endTime,Guid saleFilialeId, string salePlatformIds, string orderTypes);

        /// <summary>
        /// 汇总同一公司同一订单类型不同平台的数据  说明：“门店采购订单”和“帮门店发货订单”按公司和订单类型合计，网络发货订单不进行合计，即将门店数据汇总
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        /// zal 2017-07-17
        IList<CompanyGrossProfitRecordInfo> SumCompanyGrossProfitBySaleFilialeIdAndOrderType(DateTime startTime,DateTime endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes);
    }
}

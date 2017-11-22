using ERP.Model.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICompanyGrossProfitRecord
    {
        /// <summary>
        /// 是否存在公司毛利记录数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        bool Exists(DateTime dayTime);

        /// <summary>
        /// 删除特定时间内的临时数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        bool DeleteData(int year, int month);

        /// <summary>
        /// 批量插入公司毛利记录
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddData(IList<CompanyGrossProfitRecordInfo> list);

        /// <summary>
        /// 修改公司毛利记录(如果已存在则修改，不存在添加)
        /// </summary>
        /// <param name="companyGrossProfitRecordInfo"></param>
        /// <returns></returns>
        bool UpdateCompanyGrossProfitRecordInfo(CompanyGrossProfitRecordInfo companyGrossProfitRecordInfo);

        /// <summary>
        /// 查询历史月份公司毛利信息
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        IList<CompanyGrossProfitRecordInfo> SelectCompanyGrossProfitInfos(DateTime startTime, DateTime? endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes);

        /// <summary>
        /// 汇总同一公司不同平台的数据
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        IList<CompanyGrossProfitRecordInfo> SumCompanyGrossProfitFromMonthBySaleFilialeId(DateTime startTime, DateTime? endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes);

        /// <summary>
        /// 汇总同一公司同一订单类型不同平台的数据  说明：“门店采购订单”和“帮门店发货订单”按公司和订单类型合计，网络发货订单不进行合计，即将门店数据汇总
        /// </summary>
        /// <param name="startTime">记录年月</param>
        /// <param name="endTime"></param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformIds">销售平台</param>
        /// <param name="orderTypes">订单类型(0:网络发货订单;1:门店采购订单;2:帮门店发货订单;)</param>
        /// <returns></returns>
        IList<CompanyGrossProfitRecordInfo> SumCompanyGrossProfitFromMonthBySaleFilialeIdAndOrderType(DateTime startTime, DateTime? endTime, Guid saleFilialeId, string salePlatformIds, string orderTypes);
    }
}

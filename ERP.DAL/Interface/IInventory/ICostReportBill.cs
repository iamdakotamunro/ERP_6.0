using ERP.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICostReportBill
    {
        #region .对本表的维护.
        #region select data

        /// <summary>
        /// 返回lmshop_CostReportBill表的所有数据 
        /// </summary>
        /// <returns></returns>        
        List<CostReportBillInfo> GetAlllmshop_CostReportBill();

        /// <summary>
        /// 根据lmshop_CostReportBill表的ReportId字段返回数据  
        /// </summary>
        /// <param name="reportId">ReportId</param>
        /// <returns></returns>        
        List<CostReportBillInfo> Getlmshop_CostReportBillByReportId(Guid reportId);

        /// <summary>
        /// 根据lmshop_CostReportBill表的BillId字段返回数据 
        /// </summary>
        /// <param name="billId">BillId</param>
        /// <returns></returns>       
        CostReportBillInfo Getlmshop_CostReportBillByBillId(Guid billId);

        /// <summary>
        /// 根据条件返回lmshop_CostReportBill表的数据 
        /// </summary>
        /// <param name="reportPersonnelId">申报人</param>
        /// <param name="invoiceTitleFilialeId">发票抬头</param>
        /// <param name="reportDateStart">申报时间</param>
        /// <param name="reportDateEnd">申报时间</param>
        /// <param name="reportStatus">费用申报状态</param>
        /// <param name="reportNo">申报编号</param>
        /// <param name="billNo">票据号码</param>
        /// <param name="billState">票据状态</param>
        /// <param name="operatingTimeStart">操作时间</param>
        /// <param name="operatingTimeEnd">操作时间</param>
        /// <param name="invoiceType">票据类型(1:普通发票;2:增值税专用发票;3:收据;)</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sumTable"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        DataTable Getlmshop_CostReportBillByPage(Guid reportPersonnelId, Guid invoiceTitleFilialeId,
            DateTime? reportDateStart, DateTime? reportDateEnd, string reportStatus, string reportNo, string billNo,
            int? billState, DateTime? operatingTimeStart, DateTime? operatingTimeEnd, int invoiceType, int pageIndex, int pageSize,
            out DataTable sumTable, out int total);

        /// <summary>
        /// 根据billIdList统计票据单总数、含税金额
        /// </summary>
        /// <param name="billIdList"></param>
        /// <returns></returns>
        /// zal 2016-08-26
        ArrayList GetSumBill(string[] billIdList);
        #endregion
        #region delete data

        /// <summary>
        /// 根据lmshop_CostReportBill表的InvoiceId字段删除数据 
        /// </summary>
        /// <param name="billId">BillId</param>
        /// <returns></returns>        
        bool Deletelmshop_CostReportBillByBillId(Guid billId);

        /// <summary>
        /// 根据lmshop_CostReportBill表的receiptId字段删除数据 
        /// </summary>
        /// <param name="reportId">ReportId</param>
        /// <returns></returns>        
        bool Deletelmshop_CostReportBillByReportId(Guid reportId);
        #endregion
        #region update data
        /// <summary>
        /// 根据lmshop_CostReportBill表的BillId字段更新数据 
        /// </summary> 
        /// <param name="lmshopCostReportBillInfo">lmshopCostReportBillInfo</param>
        /// <returns></returns>       
        bool Updatelmshop_CostReportBillByBillId(CostReportBillInfo lmshopCostReportBillInfo);

        /// <summary>
        /// 根据lmshop_CostReportBill表的BillId字段修改数据
        /// </summary>
        /// <param name="remark">流程记录</param>
        /// <param name="billId">主键</param>
        /// <param name="billState">票据状态(0:未提交;1:已提交;2:已接收;3:已认证;4:已核销)</param>
        /// <returns></returns>
        bool Updatelmshop_CostReportBillByBillId(string remark, Guid billId, int billState);

        /// <summary>
        /// 根据lmshop_CostReportBill表的BillId字段修改备注信息
        /// </summary>
        /// <param name="remark">流程记录</param>
        /// <param name="billId">主键</param>
        /// <returns></returns>
        bool Updatelmshop_CostReportBillRemarkByBillId(string remark, Guid billId);

        /// <summary>
        /// 根据lmshop_CostReportBill表的ReportId字段修改数据
        /// </summary>
        /// <param name="remark">流程记录</param>
        /// <param name="reportId">外键</param>
        /// <param name="billState">票据状态(0:未提交;1:已提交;2:已接收;3:已认证;4:已核销)</param>
        /// <returns></returns>
        bool Updatelmshop_CostReportBillByReportId(string remark, Guid reportId, int billState);

        /// <summary>
        /// 根据ReportId修改未付款的票据的“受理通过”状态
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="isPass"></param>
        /// <returns></returns>
        /// zal 2016-11-25
        bool Updatelmshop_CostReportBillForPassByReportId(Guid reportId, bool isPass);
        #endregion
        #region insert data

        /// <summary>
        /// 向lmshop_CostReportBill表插入一条数据
        /// </summary>
        /// <param name="lmshopCostReportBill">lmshopCostReportBill</param>       
        /// <returns></returns>        
        bool Addlmshop_CostReportBill(CostReportBillInfo lmshopCostReportBill);

        /// <summary>
        /// 向lmshop_CostReportBill表批量插入数据
        /// </summary>
        /// <param name="lmshopCostReportBillList"></param>
        /// <returns></returns>
        bool AddBatchlmshop_CostReportBill(List<CostReportBillInfo> lmshopCostReportBillList);

        #endregion

        #endregion
    }
}

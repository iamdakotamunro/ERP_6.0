using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 发票操作接口
    /// </summary>
    public interface IInvoice
    {
        /// <summary>
        /// 添加发票
        /// </summary>
        /// <param name="invoice">发票类</param>
        /// <param name="dictOrderIdOrderNo">订单数组</param>
        bool Insert(InvoiceInfo invoice, Dictionary<Guid, string> dictOrderIdOrderNo);

        /// <summary>
        /// 添加发票
        /// </summary>
        /// <param name="invoice">发票类</param>
        void Insert(InvoiceInfo invoice);

        /// <summary>
        /// 获取指定的发票索取记录
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        InvoiceInfo GetInvoice(Guid invoiceId);

        /// <summary>
        /// 返回所有发票列表
        /// </summary>
        /// <returns></returns>
        IList<InvoiceInfo> GetInvoiceList();

        /// <summary>
        /// Func : 根据订单号，获取指定的发票索取记录
        /// Code : dyy
        /// Date : 2009 Nov 26th
        /// </summary>
        /// <param name="goodsID"></param>
        /// <returns></returns>
        InvoiceInfo GetInvoiceByGoodsOrder(Guid goodsID);

        /// <summary>
        /// 获取发票某个时间段的统计信息
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="keyword"></param>
        /// <param name="invocestate"></param>
        /// <param name="yesorno">是否显示订单重复记录</param>
        /// <returns></returns>
        IList<InvoiceInfo> GetInvoiceStatistcsInfoList(DateTime starttime, DateTime endtime, string keyword, int invocestate, YesOrNo yesorno);

        /// <summary>
        /// 根据条件查找发票
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="isOrderComplete"></param>
        /// <param name="orderNo"> </param>
        /// <param name="invoiceName"> </param>
        /// <param name="invoiceNo"> </param>
        /// <param name="address"> </param>
        /// <param name="invoiceContent"> </param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="isNeedManual">是否需要手动打印发票</param>
        /// <param name="warehouseIds"> </param>
        /// <param name="invoiceType"></param>
        /// <param name="saleFilialeid"></param>
        /// <param name="cancelPersonel"></param>
        /// <returns></returns>
        IList<InvoiceInfo> GetInvoiceList(DateTime startTime, DateTime endTime, bool isOrderComplete, string orderNo, string invoiceName, string invoiceNo, string address, string invoiceContent, InvoiceState invoiceState, bool isNeedManual, IEnumerable<Guid> warehouseIds,int invoiceType, Guid saleFilialeid, string cancelPersonel);


        /// <summary>
        /// 根据条件查找发票(分页)
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="isOrderComplete"></param>
        /// <param name="orderNo"> </param>
        /// <param name="invoiceName"> </param>
        /// <param name="invoiceNo"> </param>
        /// <param name="address"> </param>
        /// <param name="invoiceContent"> </param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="isNeedManual">是否需要手动打印发票</param>
        /// <param name="warehouseId"> </param>
        /// <param name="permissionFilialeId"> </param>
        /// <param name="permissionBranchId"> </param>
        /// <param name="permissionPositionId"> </param>
        /// <param name="invoiceType"></param>
        /// <param name="saleFilialeid"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="cancelPersonel"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        IList<InvoiceInfo> GetInvoiceListByPage(DateTime startTime, DateTime endTime, bool isOrderComplete, string orderNo, string invoiceName, string invoiceNo, string address, string invoiceContent, InvoiceState invoiceState, bool isNeedManual, Guid warehouseId, Guid permissionFilialeId, Guid permissionBranchId, Guid permissionPositionId, int invoiceType, Guid saleFilialeid, string cancelPersonel, int pageIndex, int pageSize, out int recordCount);

        /// <summary>
        /// 返回指定会员的发票列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        IList<InvoiceInfo> GetInvoiceList(Guid memberId);

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        bool SetInvoiceState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel);

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceIdList">发票编号列表</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        /// zal 2016-12-27
        bool BatchSetInvoiceState(List<Guid> invoiceIdList, InvoiceState invoiceState, string cancelPersonel);

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        bool UpdateSetInvoiceState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel);

        /// <summary>
        /// 提交作废申请人 add by FanGuan 2012-06-25
        /// </summary>
        /// <param name="invoiceId">发票ID</param>
        /// <param name="cancelPersonel">作废申请人</param>
        void SetCancelPersonel(Guid invoiceId, string cancelPersonel);

        /// <summary>
        /// 作废该订单的发票
        /// </summary>
        /// <param name="orderId">订单id</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        void WasteState(Guid orderId, InvoiceState invoiceState, string cancelPersonel);
        /// <summary>
        /// 查找开出发票的总金额 add by dinghq 2011-04-12
        /// </summary>
        /// <param name="start">指定起始时间</param>
        /// <param name="end">指定结束时间</param>
        /// <param name="state">指定状态</param>
        /// <returns></returns>
        decimal GetInvioceTotal(DateTime start, DateTime end, InvoiceState state);
        /// <summary>
        /// 获取发票所在的公司
        /// </summary>
        /// <param name="invoiceID">发票ID</param>
        /// <returns></returns>
        string GetOrderFilieIdByInvoiceID(Guid invoiceID);

        /// <summary>
        /// 新增领取的发票卷
        /// </summary>
        /// <param name="roll"></param>
        /// <returns></returns>
        bool InsertRoll(InvoiceRoll roll);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roll"></param>
        /// <returns></returns>
        bool UpdateRoll(InvoiceRoll roll);

        /// <summary>
        /// 新增领取的发票卷
        /// </summary>
        /// <param name="rollDetail"></param>
        /// <returns></returns>
        bool InsertRollDetail(InvoiceRollDetail rollDetail);

        /// <summary>
        /// 新增领取的发票卷
        /// </summary>
        /// <returns></returns>
        IList<InvoiceRoll> GetRollList();

        /// <summary>
        /// 新增领取的发票卷信息
        /// </summary>
        /// <returns></returns>
        IList<InvoiceRollDetail> GetRollDetailList(Guid rollId);

        /// <summary>
        /// 删除分卷信息
        /// </summary>
        /// <param name="rollId"></param>
        /// <returns></returns>
        bool DeleteRollDetail(Guid rollId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rollId"></param>
        /// <returns></returns>
        int SumRollDeatilState(Guid rollId);

        /// <summary>
        /// 分发发票卷
        /// </summary>
        /// <param name="rollId"></param>
        /// <param name="startNo"></param>
        /// <param name="endNo"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool DistributeInvoiceRoll(Guid rollId, long startNo, long endNo, string remark);

        /// <summary>
        /// 指定状态获取发票卷信息
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        IList<InvoiceRollDetail> GetRollDetailListByState(InvoiceRollState state);

        /// <summary>
        /// 发票报送到
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="filialeId"></param>
        void InvoiceCommit(DateTime startDate, DateTime endDate, Guid filialeId);

        /// <summary>
        /// 遗失上报
        /// </summary>
        void LostSubmit(Guid rollId, long startNo, long endNo, InvoiceRollState state);

        /// <summary>
        /// 删除指定的发票卷
        /// </summary>
        void DeleteRollDetail(Guid rollId, long startNo, long endNo, InvoiceRollState state);

        /// <summary>
        /// 发票汇总搜索
        /// </summary>
        /// <returns></returns>
        IList<InvoiceBriefInfo> Search(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo, int pageSize, int pageIndex, out int recordCount);

        IList<InvoiceNoteStatisticsInfo> InvoiceNoteStatistics(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo);

        /// <summary>发票汇总导出Excel专用
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="noteType"></param>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        IList<InvoiceBriefInfo> OutPutExcelInvoice(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo);

        #region [修改]

        /// <summary>
        /// 修改一条发票记录
        /// </summary>
        /// <param name="invoice">发票信息</param>
        void Update(InvoiceInfo invoice);

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="invoiceState">发票状态</param>
        void SetInvoiceState(Guid invoiceId, InvoiceState invoiceState);

        /// <summary>
        /// 修改发票金额--不作废,只是修改
        /// </summary>
        /// <param name="invoiceId">发票Id</param>
        /// <param name="invoiceSum">金额</param>
        void SetInvoiceSum(Guid invoiceId, float invoiceSum);

        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="invoiceInfo"></param>
        void UpdateInvoice(InvoiceInfo invoiceInfo);
        #endregion

        #region 删除

        /// <summary>
        /// 根据OrderId删除lmShop_Invoice
        /// </summary>
        /// <param name="orderId"></param>
        void DeleteInvoiceByOrderId(Guid orderId);

        #endregion


        /// <summary>
        /// 根据订单号，发票状态，订单是否完成发货，发票是否提交，开始时间，截止时间获取发票集合。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        IList<SimpleInvoiceDetailInfo> GetInvoiceList(string orderNo, byte invoiceState, bool isFinished, bool isCommit,
            DateTime fromTime, DateTime toTime);

        /// <summary>
        /// 根据发票ID获取发票信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        SimpleInvoiceDetailInfo GetInvoiceInfo(Guid invoiceId);


        /// <summary>
        /// 根据发票号码获取发票信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        SimpleInvoiceDetailInfo GetInvoiceInfo(long invoiceNo);

        /// <summary>
        /// 获取发票品名的名称集合。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        List<string> GetInvoiceItem();

        /// <summary>
        /// 根据发票ID更新发票状态，发票号，发票代码。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        bool UpdateInvoiceStateWithInvoiceNo(Guid invoiceId, byte invoiceState, long invoiceno,
            string invoicecode);

        /// <summary>
        /// 根据发票卷详细表开始和截止号码，状态=分发
        /// 获取发票卷表发票卷代码，发票卷所属公司ID，
        /// </summary>
        /// <param name="startNo"></param>
        /// <param name="endNo"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        InvoiceRoll GetInvoiceRollByStartNoandEndNo(long startNo, long endNo);

        /// <summary>
        /// 查询当前发票卷使用到的最大发票号
        /// 发票状态：已开
        /// </summary>
        /// <param name="invoiceStartNo"></param>
        /// <param name="invoiceEndNo"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        long GetInvoiceMaxInvoiceNoByInvoiceNo(long invoiceStartNo, long invoiceEndNo);

        /// <summary>
        /// 根据发票ID获取发票打印所属数据
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        SimpleInvoiceInfo GetInvoicePrintData(Guid invoiceId);

        /// <summary>
        /// 根据订单号取得发票信息
        /// </summary>
        /// <param name="orderId"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        List<SimpleInvoiceInfo> GetInvoiceByOrderId(Guid orderId);

        List<SimpleInvoiceDetailInfo> GetInvoiceByOrderNo(string orderNo);

        /// <summary>
        /// 根据发票ID查询发票信息
        /// </summary>
        /// <param name="invoiceId"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        InvoiceInfo GetInvoiceByInvoiceId(Guid invoiceId);

        /// <summary>
        /// 根据发票ID更新发票状态，发票号，发票代码。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        bool UpdateInvoiceChCodeAndInvoiceChNoByinvoiceId(Guid invoiceId, string invoiceChCode, long invoiceChNo);


        /// <summary>
        /// 添加发票订单关系表
        /// </summary>
        /// <param name="invoiceId">发票ID</param>
        /// <param name="orderId">订单ID</param>
        /// <param name="orderNo">订单号</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        bool InsertOrderInvoice(Guid invoiceId, Guid orderId, string orderNo);

        /// <summary>
        /// 添加发票
        /// </summary>
        /// <param name="invoice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        bool InsertInvoice(InvoiceInfo invoice);

        /// <summary>
        /// 根据发票ID更新发票状态
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        bool UpdateInvoiceStateByinvoiceId(Guid invoiceId, InvoiceState invoiceState);

        /// <summary>
        /// 根据订单ID查询发票ID
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        Guid GetInvoiceIdByOrderNo(string orderNo);

        /// <summary>
        /// 根据发票卷ID和发票起始号修改发票卷详细表状态
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        bool UpdateInvoiceStateByinvoiceId(Guid rollId, long startNo, int state);

        /// <summary>更新发票抬头和发票内容
        /// </summary>
        /// <returns></returns>
        Boolean SetInvoiceNameAndInvoiceContent(Guid invoiceId, string invoiceName, string invoiceContent);

        /// <summary>通过订单ID获取发票号码和发票是否报税    (Key:发票号码，Value:是否报税)
        /// </summary>
        /// <returns></returns>
        KeyValuePair<long, Boolean> GetInvoiceNoAndIsCommitByOrderId(Guid orderId);
    }
}

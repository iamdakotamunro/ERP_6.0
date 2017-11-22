using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// ��Ʊ�����ӿ�
    /// </summary>
    public interface IInvoice
    {
        /// <summary>
        /// ��ӷ�Ʊ
        /// </summary>
        /// <param name="invoice">��Ʊ��</param>
        /// <param name="dictOrderIdOrderNo">��������</param>
        bool Insert(InvoiceInfo invoice, Dictionary<Guid, string> dictOrderIdOrderNo);

        /// <summary>
        /// ��ӷ�Ʊ
        /// </summary>
        /// <param name="invoice">��Ʊ��</param>
        void Insert(InvoiceInfo invoice);

        /// <summary>
        /// ��ȡָ���ķ�Ʊ��ȡ��¼
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        InvoiceInfo GetInvoice(Guid invoiceId);

        /// <summary>
        /// �������з�Ʊ�б�
        /// </summary>
        /// <returns></returns>
        IList<InvoiceInfo> GetInvoiceList();

        /// <summary>
        /// Func : ���ݶ����ţ���ȡָ���ķ�Ʊ��ȡ��¼
        /// Code : dyy
        /// Date : 2009 Nov 26th
        /// </summary>
        /// <param name="goodsID"></param>
        /// <returns></returns>
        InvoiceInfo GetInvoiceByGoodsOrder(Guid goodsID);

        /// <summary>
        /// ��ȡ��Ʊĳ��ʱ��ε�ͳ����Ϣ
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="keyword"></param>
        /// <param name="invocestate"></param>
        /// <param name="yesorno">�Ƿ���ʾ�����ظ���¼</param>
        /// <returns></returns>
        IList<InvoiceInfo> GetInvoiceStatistcsInfoList(DateTime starttime, DateTime endtime, string keyword, int invocestate, YesOrNo yesorno);

        /// <summary>
        /// �����������ҷ�Ʊ
        /// </summary>
        /// <param name="startTime">��ʼʱ��</param>
        /// <param name="endTime">����ʱ��</param>
        /// <param name="isOrderComplete"></param>
        /// <param name="orderNo"> </param>
        /// <param name="invoiceName"> </param>
        /// <param name="invoiceNo"> </param>
        /// <param name="address"> </param>
        /// <param name="invoiceContent"> </param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="isNeedManual">�Ƿ���Ҫ�ֶ���ӡ��Ʊ</param>
        /// <param name="warehouseIds"> </param>
        /// <param name="invoiceType"></param>
        /// <param name="saleFilialeid"></param>
        /// <param name="cancelPersonel"></param>
        /// <returns></returns>
        IList<InvoiceInfo> GetInvoiceList(DateTime startTime, DateTime endTime, bool isOrderComplete, string orderNo, string invoiceName, string invoiceNo, string address, string invoiceContent, InvoiceState invoiceState, bool isNeedManual, IEnumerable<Guid> warehouseIds,int invoiceType, Guid saleFilialeid, string cancelPersonel);


        /// <summary>
        /// �����������ҷ�Ʊ(��ҳ)
        /// </summary>
        /// <param name="startTime">��ʼʱ��</param>
        /// <param name="endTime">����ʱ��</param>
        /// <param name="isOrderComplete"></param>
        /// <param name="orderNo"> </param>
        /// <param name="invoiceName"> </param>
        /// <param name="invoiceNo"> </param>
        /// <param name="address"> </param>
        /// <param name="invoiceContent"> </param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="isNeedManual">�Ƿ���Ҫ�ֶ���ӡ��Ʊ</param>
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
        /// ����ָ����Ա�ķ�Ʊ�б�
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        IList<InvoiceInfo> GetInvoiceList(Guid memberId);

        /// <summary>
        /// ���÷�Ʊ״̬
        /// </summary>
        /// <param name="invoiceId">��Ʊ���</param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="cancelPersonel">����������</param>
        bool SetInvoiceState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel);

        /// <summary>
        /// ���÷�Ʊ״̬
        /// </summary>
        /// <param name="invoiceIdList">��Ʊ����б�</param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="cancelPersonel">����������</param>
        /// zal 2016-12-27
        bool BatchSetInvoiceState(List<Guid> invoiceIdList, InvoiceState invoiceState, string cancelPersonel);

        /// <summary>
        /// ���÷�Ʊ״̬
        /// </summary>
        /// <param name="invoiceId">��Ʊ���</param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="cancelPersonel">����������</param>
        bool UpdateSetInvoiceState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel);

        /// <summary>
        /// �ύ���������� add by FanGuan 2012-06-25
        /// </summary>
        /// <param name="invoiceId">��ƱID</param>
        /// <param name="cancelPersonel">����������</param>
        void SetCancelPersonel(Guid invoiceId, string cancelPersonel);

        /// <summary>
        /// ���ϸö����ķ�Ʊ
        /// </summary>
        /// <param name="orderId">����id</param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="cancelPersonel">����������</param>
        void WasteState(Guid orderId, InvoiceState invoiceState, string cancelPersonel);
        /// <summary>
        /// ���ҿ�����Ʊ���ܽ�� add by dinghq 2011-04-12
        /// </summary>
        /// <param name="start">ָ����ʼʱ��</param>
        /// <param name="end">ָ������ʱ��</param>
        /// <param name="state">ָ��״̬</param>
        /// <returns></returns>
        decimal GetInvioceTotal(DateTime start, DateTime end, InvoiceState state);
        /// <summary>
        /// ��ȡ��Ʊ���ڵĹ�˾
        /// </summary>
        /// <param name="invoiceID">��ƱID</param>
        /// <returns></returns>
        string GetOrderFilieIdByInvoiceID(Guid invoiceID);

        /// <summary>
        /// ������ȡ�ķ�Ʊ��
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
        /// ������ȡ�ķ�Ʊ��
        /// </summary>
        /// <param name="rollDetail"></param>
        /// <returns></returns>
        bool InsertRollDetail(InvoiceRollDetail rollDetail);

        /// <summary>
        /// ������ȡ�ķ�Ʊ��
        /// </summary>
        /// <returns></returns>
        IList<InvoiceRoll> GetRollList();

        /// <summary>
        /// ������ȡ�ķ�Ʊ����Ϣ
        /// </summary>
        /// <returns></returns>
        IList<InvoiceRollDetail> GetRollDetailList(Guid rollId);

        /// <summary>
        /// ɾ���־���Ϣ
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
        /// �ַ���Ʊ��
        /// </summary>
        /// <param name="rollId"></param>
        /// <param name="startNo"></param>
        /// <param name="endNo"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool DistributeInvoiceRoll(Guid rollId, long startNo, long endNo, string remark);

        /// <summary>
        /// ָ��״̬��ȡ��Ʊ����Ϣ
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        IList<InvoiceRollDetail> GetRollDetailListByState(InvoiceRollState state);

        /// <summary>
        /// ��Ʊ���͵�
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="filialeId"></param>
        void InvoiceCommit(DateTime startDate, DateTime endDate, Guid filialeId);

        /// <summary>
        /// ��ʧ�ϱ�
        /// </summary>
        void LostSubmit(Guid rollId, long startNo, long endNo, InvoiceRollState state);

        /// <summary>
        /// ɾ��ָ���ķ�Ʊ��
        /// </summary>
        void DeleteRollDetail(Guid rollId, long startNo, long endNo, InvoiceRollState state);

        /// <summary>
        /// ��Ʊ��������
        /// </summary>
        /// <returns></returns>
        IList<InvoiceBriefInfo> Search(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo, int pageSize, int pageIndex, out int recordCount);

        IList<InvoiceNoteStatisticsInfo> InvoiceNoteStatistics(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo);

        /// <summary>��Ʊ���ܵ���Excelר��
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="noteType"></param>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        IList<InvoiceBriefInfo> OutPutExcelInvoice(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo);

        #region [�޸�]

        /// <summary>
        /// �޸�һ����Ʊ��¼
        /// </summary>
        /// <param name="invoice">��Ʊ��Ϣ</param>
        void Update(InvoiceInfo invoice);

        /// <summary>
        /// ���÷�Ʊ״̬
        /// </summary>
        /// <param name="invoiceId">��Ʊ���</param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        void SetInvoiceState(Guid invoiceId, InvoiceState invoiceState);

        /// <summary>
        /// �޸ķ�Ʊ���--������,ֻ���޸�
        /// </summary>
        /// <param name="invoiceId">��ƱId</param>
        /// <param name="invoiceSum">���</param>
        void SetInvoiceSum(Guid invoiceId, float invoiceSum);

        /// <summary>
        /// �޸ķ�Ʊ��Ϣ
        /// </summary>
        /// <param name="invoiceInfo"></param>
        void UpdateInvoice(InvoiceInfo invoiceInfo);
        #endregion

        #region ɾ��

        /// <summary>
        /// ����OrderIdɾ��lmShop_Invoice
        /// </summary>
        /// <param name="orderId"></param>
        void DeleteInvoiceByOrderId(Guid orderId);

        #endregion


        /// <summary>
        /// ���ݶ����ţ���Ʊ״̬�������Ƿ���ɷ�������Ʊ�Ƿ��ύ����ʼʱ�䣬��ֹʱ���ȡ��Ʊ���ϡ�
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        IList<SimpleInvoiceDetailInfo> GetInvoiceList(string orderNo, byte invoiceState, bool isFinished, bool isCommit,
            DateTime fromTime, DateTime toTime);

        /// <summary>
        /// ���ݷ�ƱID��ȡ��Ʊ��Ϣ
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        SimpleInvoiceDetailInfo GetInvoiceInfo(Guid invoiceId);


        /// <summary>
        /// ���ݷ�Ʊ�����ȡ��Ʊ��Ϣ
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        SimpleInvoiceDetailInfo GetInvoiceInfo(long invoiceNo);

        /// <summary>
        /// ��ȡ��ƱƷ�������Ƽ��ϡ�
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        List<string> GetInvoiceItem();

        /// <summary>
        /// ���ݷ�ƱID���·�Ʊ״̬����Ʊ�ţ���Ʊ���롣
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        bool UpdateInvoiceStateWithInvoiceNo(Guid invoiceId, byte invoiceState, long invoiceno,
            string invoicecode);

        /// <summary>
        /// ���ݷ�Ʊ����ϸ��ʼ�ͽ�ֹ���룬״̬=�ַ�
        /// ��ȡ��Ʊ���Ʊ����룬��Ʊ��������˾ID��
        /// </summary>
        /// <param name="startNo"></param>
        /// <param name="endNo"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        InvoiceRoll GetInvoiceRollByStartNoandEndNo(long startNo, long endNo);

        /// <summary>
        /// ��ѯ��ǰ��Ʊ��ʹ�õ������Ʊ��
        /// ��Ʊ״̬���ѿ�
        /// </summary>
        /// <param name="invoiceStartNo"></param>
        /// <param name="invoiceEndNo"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        long GetInvoiceMaxInvoiceNoByInvoiceNo(long invoiceStartNo, long invoiceEndNo);

        /// <summary>
        /// ���ݷ�ƱID��ȡ��Ʊ��ӡ��������
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        SimpleInvoiceInfo GetInvoicePrintData(Guid invoiceId);

        /// <summary>
        /// ���ݶ�����ȡ�÷�Ʊ��Ϣ
        /// </summary>
        /// <param name="orderId"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        List<SimpleInvoiceInfo> GetInvoiceByOrderId(Guid orderId);

        List<SimpleInvoiceDetailInfo> GetInvoiceByOrderNo(string orderNo);

        /// <summary>
        /// ���ݷ�ƱID��ѯ��Ʊ��Ϣ
        /// </summary>
        /// <param name="invoiceId"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        InvoiceInfo GetInvoiceByInvoiceId(Guid invoiceId);

        /// <summary>
        /// ���ݷ�ƱID���·�Ʊ״̬����Ʊ�ţ���Ʊ���롣
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        bool UpdateInvoiceChCodeAndInvoiceChNoByinvoiceId(Guid invoiceId, string invoiceChCode, long invoiceChNo);


        /// <summary>
        /// ��ӷ�Ʊ������ϵ��
        /// </summary>
        /// <param name="invoiceId">��ƱID</param>
        /// <param name="orderId">����ID</param>
        /// <param name="orderNo">������</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        bool InsertOrderInvoice(Guid invoiceId, Guid orderId, string orderNo);

        /// <summary>
        /// ��ӷ�Ʊ
        /// </summary>
        /// <param name="invoice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        bool InsertInvoice(InvoiceInfo invoice);

        /// <summary>
        /// ���ݷ�ƱID���·�Ʊ״̬
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        bool UpdateInvoiceStateByinvoiceId(Guid invoiceId, InvoiceState invoiceState);

        /// <summary>
        /// ���ݶ���ID��ѯ��ƱID
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        Guid GetInvoiceIdByOrderNo(string orderNo);

        /// <summary>
        /// ���ݷ�Ʊ��ID�ͷ�Ʊ��ʼ���޸ķ�Ʊ����ϸ��״̬
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        bool UpdateInvoiceStateByinvoiceId(Guid rollId, long startNo, int state);

        /// <summary>���·�Ʊ̧ͷ�ͷ�Ʊ����
        /// </summary>
        /// <returns></returns>
        Boolean SetInvoiceNameAndInvoiceContent(Guid invoiceId, string invoiceName, string invoiceContent);

        /// <summary>ͨ������ID��ȡ��Ʊ����ͷ�Ʊ�Ƿ�˰    (Key:��Ʊ���룬Value:�Ƿ�˰)
        /// </summary>
        /// <returns></returns>
        KeyValuePair<long, Boolean> GetInvoiceNoAndIsCommitByOrderId(Guid orderId);
    }
}

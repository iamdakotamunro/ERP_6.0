using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.SAL;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Inventory
{
    public class Invoice : BllInstance<Invoice>
    {
        readonly IInvoice _invoiceDao;

        public Invoice(Environment.GlobalConfig.DB.FromType fromType)
        {
            _invoiceDao = InventoryInstance.GetInvoiceDao(fromType);
        }

        /// <summary>
        /// ��ӷ�Ʊ
        /// </summary>
        /// <param name="invoice">��Ʊ��</param>
        public void Insert(InvoiceInfo invoice)
        {
            if (invoice.SaleFilialeId == Guid.Empty) return;
            _invoiceDao.Insert(invoice);
        }

        /// <summary>
        /// ��ȡָ���ķ�Ʊ��ȡ��¼
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceInfo GetInvoice(Guid invoiceId)
        {
            if (invoiceId == Guid.Empty) return null;
            return _invoiceDao.GetInvoice(invoiceId);
        }

        /// <summary>
        /// Func : ���ݶ����ţ���ȡָ���ķ�Ʊ��ȡ��¼
        /// Code : dyy
        /// Date : 2009 Nov 26th
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public InvoiceInfo GetInvoiceByGoodsOrder(Guid orderId)
        {
            if (orderId == Guid.Empty) return null;
            return _invoiceDao.GetInvoiceByGoodsOrder(orderId);
        }

        /// <summary>
        /// �����������ҷ�Ʊ
        /// </summary>
        /// <param name="startTime">��ʼʱ��</param>
        /// <param name="endTime">����ʱ��</param>
        /// <param name="isOrderComplete"></param>
        /// <param name="orderNo"></param>
        /// <param name="invoiceName"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="address"></param>
        /// <param name="invoiceContent"></param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="isNeedManual">�Ƿ���Ҫ�ֶ���ӡ��Ʊ</param>
        /// <param name="warehouseIds"></param>
        /// <param name="invoiceType"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="cancelPersonnel"></param>
        /// <returns></returns>
        public IList<InvoiceInfo> GetInvoiceList(DateTime startTime, DateTime endTime, bool isOrderComplete, string orderNo, string invoiceName, string invoiceNo, string address, string invoiceContent, InvoiceState invoiceState, bool isNeedManual, IEnumerable<Guid> warehouseIds,int invoiceType,Guid saleFilialeId,string cancelPersonnel)
        {
            return _invoiceDao.GetInvoiceList(startTime, endTime, isOrderComplete, orderNo, invoiceName, invoiceNo, address, invoiceContent, invoiceState, isNeedManual, warehouseIds,invoiceType,saleFilialeId,cancelPersonnel);
        }

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
        public IList<InvoiceInfo> GetInvoiceListByPage(DateTime startTime, DateTime endTime, bool isOrderComplete,
            string orderNo, string invoiceName, string invoiceNo, string address, string invoiceContent,
            InvoiceState invoiceState, bool isNeedManual, Guid warehouseId, Guid permissionFilialeId,
            Guid permissionBranchId, Guid permissionPositionId, int invoiceType, Guid saleFilialeid, string cancelPersonel, int pageIndex,
            int pageSize, out int recordCount)
        {
            return _invoiceDao.GetInvoiceListByPage(startTime, endTime, isOrderComplete, orderNo, invoiceName, invoiceNo, address, invoiceContent, invoiceState, isNeedManual, warehouseId,
                permissionFilialeId, permissionBranchId, permissionPositionId, invoiceType, saleFilialeid, cancelPersonel, pageIndex, pageSize, out recordCount);
        }

        /// <summary>
        /// ���÷�Ʊ״̬
        /// </summary>
        /// <param name="invoiceId">��Ʊ���</param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="cancelPersonel">����������</param>
        public bool SetInvoiceState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel)
        {
            if (invoiceId != Guid.Empty)
                return _invoiceDao.SetInvoiceState(invoiceId, invoiceState, cancelPersonel);
            return false;
        }

        /// <summary>
        /// ���÷�Ʊ״̬
        /// </summary>
        /// <param name="invoiceIdList">��Ʊ����б�</param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="cancelPersonel">����������</param>
        /// zal 2016-12-27
        public bool BatchSetInvoiceState(List<Guid> invoiceIdList, InvoiceState invoiceState, string cancelPersonel)
        {
            if (invoiceIdList.Count > 0)
                return _invoiceDao.BatchSetInvoiceState(invoiceIdList, invoiceState, cancelPersonel);
            return false;
        }

        /// <summary>
        /// ���÷�Ʊ״̬
        /// </summary>
        /// <param name="invoiceId">��Ʊ���</param>
        /// <param name="invoiceState">��Ʊ״̬</param>
        /// <param name="cancelPersonel">����������</param>
        public bool UpdateSetInvoiceState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel)
        {
            if (invoiceId != Guid.Empty)
                return _invoiceDao.UpdateSetInvoiceState(invoiceId, invoiceState, cancelPersonel);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rollInfo"></param>
        /// <returns></returns>
        public bool AddInvoiceRoll(InvoiceRoll rollInfo)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    var invoiceRollDetail = CreateRollDetail(rollInfo);
                    if (invoiceRollDetail.Count > 0)
                    {
                        _invoiceDao.InsertRoll(rollInfo);
                        foreach (var detail in invoiceRollDetail)
                        {
                            _invoiceDao.InsertRollDetail(detail);
                        }
                        tran.Complete();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rollInfo"></param>
        /// <returns></returns>
        public bool UpdateInvoiceRoll(InvoiceRoll rollInfo)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    var state = _invoiceDao.SumRollDeatilState(rollInfo.Id);
                    if (state == 0)
                    {
                        var invoiceRollDetail = CreateRollDetail(rollInfo);
                        if (invoiceRollDetail.Count > 0)
                        {
                            _invoiceDao.UpdateRoll(rollInfo);
                            _invoiceDao.DeleteRollDetail(rollInfo.Id);
                            foreach (var detail in invoiceRollDetail)
                            {
                                _invoiceDao.InsertRollDetail(detail);
                            }
                            tran.Complete();
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rollInfo"></param>
        /// <returns></returns>
        private static IList<InvoiceRollDetail> CreateRollDetail(InvoiceRoll rollInfo)
        {
            IList<InvoiceRollDetail> rollDetailList = new List<InvoiceRollDetail>();
            var rollIntervalQuantity = rollInfo.InvoiceCount / rollInfo.InvoiceRollCount;
            for (int i = 0; i < rollInfo.InvoiceRollCount; i++)
            {
                var detail = new InvoiceRollDetail { RollId = rollInfo.Id, StartNo = rollInfo.InvoiceStartNo + (rollIntervalQuantity * i) };
                if (i == rollInfo.InvoiceRollCount - 1)
                {
                    detail.EndNo = rollInfo.InvoiceEndNo;
                }
                else
                {
                    detail.EndNo = rollInfo.InvoiceStartNo + (rollIntervalQuantity * (i + 1) - 1);
                }
                detail.State = InvoiceRollState.NoInvocation;
                rollDetailList.Add(detail);
            }
            return rollDetailList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<InvoiceRoll> GetInvoiceRollList()
        {
            return _invoiceDao.GetRollList();
        }

        /// <summary>
        /// ��Ʊ���͵�
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="filialeId"></param>
        public void InvoiceCommit(DateTime startDate, DateTime endDate, Guid filialeId)
        {
            startDate = Convert.ToDateTime(startDate.ToString("yyyy-MM-dd") + " " + "00:00:00");
            endDate = Convert.ToDateTime(endDate.ToString("yyyy-MM-dd") + " " + "23:59:59");
            _invoiceDao.InvoiceCommit(startDate, endDate, filialeId);
        }

        /// <summary>
        /// ָ��״̬��ȡ��Ʊ����Ϣ
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<InvoiceRollDetail> GetRollDetailListByState(InvoiceRollState state)
        {
            return _invoiceDao.GetRollDetailListByState(state);
        }

        /// <summary>
        /// ��Ʊ��������
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="noteType"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="pageSize"> </param>
        /// <param name="pageIndex"> </param>
        /// <param name="recordCount"> </param>
        /// <returns></returns>
        public InvoiceStatisticsInfo Search(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo, int pageSize, int pageIndex, out int recordCount)
        {
            var items = _invoiceDao.Search(startTime, endTime, filialeId, noteType, invoiceNo, pageSize, pageIndex, out recordCount);
            var invoiceNoteStatisticsInfoList = _invoiceDao.InvoiceNoteStatistics(startTime, endTime, filialeId, noteType, invoiceNo);
            return new InvoiceStatisticsInfo(invoiceNoteStatisticsInfoList, items);
        }

        /// <summary>��Ʊ���ܵ���Excelר��
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="noteType"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceKindType"></param>
        /// <returns></returns>
        public IList<InvoiceBriefInfo> OutPutExcelInvoice(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo, byte invoiceKindType)
        {
            return _invoiceDao.OutPutExcelInvoice(startTime, endTime, filialeId, noteType, invoiceNo);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.SAL;
using ERP.SAL.Interface;
using Keede.Ecsoft.Model;
using ERP.Model;
using ERP.Enum;
using AllianceShop.Contract.DataTransferObject;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>
    /// 费用审批申报业务类
    /// </summary>
    public class CostReport : BllInstance<CostReport>
    {
        private readonly ICostReport _costReport;
        private readonly ICostReckoning _costReckoningDao;
        private readonly IBankAccounts _bankAccountsDao;
        private readonly IPersonnelSao _personnelManager;
        readonly CodeManager _codeManager = new CodeManager();

        public CostReport(Environment.GlobalConfig.DB.FromType fromType)
        {
            _costReport = InventoryInstance.GetCostReportDao(fromType);
            _costReckoningDao = InventoryInstance.GetCostReckoningDao(fromType);
            _bankAccountsDao = InventoryInstance.GetBankAccountsDao(fromType);
            _personnelManager = new PersonnelSao();
        }

        public CostReport(IBankAccounts bankAccounts, IPersonnelSao personnelSao, ICostReckoning costReckoningDao)
        {
            _bankAccountsDao = bankAccounts;
            _personnelManager = personnelSao;
            _costReckoningDao = costReckoningDao;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="auditingBranchIdList"></param>
        /// <param name="invoiceType"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        public IList<CostReportInfo> GetReportList(DateTime startTime, DateTime endTime, IList<Guid> auditingBranchIdList, IList<int> invoiceType, IList<int> states)
        {
            var typeString = string.Join(",", invoiceType.ToArray());
            string stateString;
            switch (states[0])
            {
                case 0:
                    stateString = " IN(0,3) ";//票据待受理，审核不通过
                    break;
                case 10:
                    stateString = " =10 ";//票据待选
                    typeString = "1,2,3,4";
                    break;
                default:
                    stateString = " NOT IN(0,3) ";
                    break;
            }
            return _costReport.GetReportList(startTime, endTime, string.Empty, typeString, stateString);
        }


        #region 执行完成时的操作(新增资金流、新增帐务记录，与门店费用交互) zal 2015-11-16
        #region 新增资金流
        /// <summary>
        /// 返回资金流Model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="personnelInfo"></param>
        /// <param name="flag">true:有手续费;false:无手续费;</param>
        /// <returns></returns>
        public WasteBookInfo AddWasteBookInfo(CostReportInfo model, PersonnelInfo personnelInfo, bool flag)
        {
            decimal income;
            if (flag && model.Poundage > 0)
            {
                income = model.Poundage;
            }
            else
            {
                income = model.RealityCost;
            }
            string auditingMan = _personnelManager.GetName(personnelInfo.PersonnelId);
            string reportMan = _personnelManager.GetName(model.ReportPersonnelId);
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var wasteBookTradeCode = _codeManager.GetCode(CodeType.RD);
            int wasteBookType;
            string strMan, strDes;
            if (income > 0)
            {
                wasteBookType = (int)WasteBookType.Decrease;
                strMan = "收款";
                strDes = "减少";
            }
            else
            {
                wasteBookType = (int)WasteBookType.Increase;
                strMan = "付款";
                strDes = "增加";
            }

            bool isDeposit = (model.ReportKind == (int)CostReportKind.FeeIncome && !string.IsNullOrEmpty(model.DepositNo));

            var wasteBookDescription = string.Format("[费用申报(申报编号:{0}；费用名称:{1}；申报人:{2}；" + strMan + "单位:{3}；完成打款人:{4}；手续费:{5}；交易流水号:{6}；资金" + strDes + ",{7}" + (isDeposit ? "【押金回收】" : "") + ")]", model.ReportNo, model.ReportName, reportMan, model.PayCompany, auditingMan, model.Poundage.ToString("#0.00"), model.TradeNo, dateTime);

            var bankInfo = _bankAccountsDao.GetBankAccounts(model.PayBankAccountId);
            var wasteBookInfo = new WasteBookInfo(
                                         Guid.NewGuid(),
                                         model.PayBankAccountId,
                                         wasteBookTradeCode,
                                         wasteBookDescription,
                                         -income,//金额
                                         (Int32)AuditingState.Yes,
                                         wasteBookType,
                                         model.AssumeFilialeId)
            {
                LinkTradeCode = model.ReportNo,
                LinkTradeType = (int)WasteBookLinkTradeType.CostReport,
                BankTradeCode = string.Empty,
                State = (int)WasteBookState.Currently,
                IsOut = bankInfo.IsMain
            };
            return wasteBookInfo;
        }
        #endregion

        #region 新增帐务记录
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="personnelInfo"></param>
        /// <param name="flag">true:有手续费;false:无手续费;</param>
        /// <returns></returns>
        public CostReckoningInfo AddCostReckoningInfo(CostReportInfo model, PersonnelInfo personnelInfo, bool flag)
        {
            var description = String.Format("[费用申报单据号:{0}][费用申报完成打款][{1}]", model.ReportNo, model.ReportName);
            if (flag && model.Poundage > 0)
            {
                description += "[手续费：" + model.Poundage + "]";
            }
            description = char.ConvertFromUtf32(10) + description + " [" + personnelInfo.RealName + ": " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]";
            var costReckoningInfo = new CostReckoningInfo
            {
                ReckoningId = Guid.NewGuid(),
                FilialeId = model.ReportFilialeId,
                AssumeFilialeId=model.AssumeFilialeId,
                CompanyId = model.CompanyId,
                TradeCode = _codeManager.GetCode(CodeType.PY),
                DateCreated = DateTime.Now,
                Description = description,
                AccountReceivable = model.RealityCost,
                NonceTotalled = _costReckoningDao.GetTotalled(model.CompanyId),
                ReckoningType = model.RealityCost > 0 ? (int)ReckoningType.Income : (int)ReckoningType.Defray,
                ReckoningState = (int)ReckoningStateType.Currently,
                IsChecked = 0,
                AuditingState = (int)AuditingState.Yes
            };
            costReckoningInfo.NonceTotalled += costReckoningInfo.AccountReceivable;
            return costReckoningInfo;
        }
        #endregion

        #region 向门店插入一条费用信息
        public CostRecordDTO AddCostRecordDto(CostReportInfo model)
        {
            var costRecordDto = new CostRecordDTO
            {
                ID = Guid.NewGuid(),
                Amount = model.RealityCost,
                CostName = model.ReportName,
                CreateTime = DateTime.Now,
                Remark = model.ReportMemo + model.Memo,
                ShopId = model.AssumeShopId
            };
            return costRecordDto;
        }
        #endregion
        #endregion
    }
}

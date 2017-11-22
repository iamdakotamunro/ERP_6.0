using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using AllianceShop.Contract.DataTransferObject;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.Cache;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    ///<summary>完成打款
    ///</summary>
    public partial class CompanyFundPayReceiptFinish : BasePage
    {
        readonly CodeManager _codeBll = new CodeManager();
        readonly PersonnelManager _personnelManager=new PersonnelManager();
        static readonly CompanyBankAccountBindDao _companyBankAccountBindDao = new CompanyBankAccountBindDao(GlobalConfig.DB.FromType.Read);
        //其他公司
        private readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussent _companyCussentDao=new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly IBankAccounts _bankAccounts=new BankAccounts(GlobalConfig.DB.FromType.Read);
        private readonly IWasteBook _wasteBook=new WasteBook(GlobalConfig.DB.FromType.Write);
        private readonly IReckoning _reckoning=new Reckoning(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReceiptNo = string.Empty;
                Type = CompanyFundReceiptType.All;
                if (Request["type"] != null)
                {
                    if (Request["type"] == string.Format("{0}", (int)CompanyFundReceiptType.Payment))
                        Type = CompanyFundReceiptType.Payment;
                    if (Request["type"] == string.Format("{0}", (int)CompanyFundReceiptType.Receive))
                        Type = CompanyFundReceiptType.Receive;
                }
                Status = CompanyFundReceiptState.NoHandle;
            }
        }

        protected void RgCheckInfoNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<CompanyFundReceiptInfo> list = _companyFundReceipt.GetAllFundReceiptInfoList(new Guid(SelectSaleFilialeId), ReceiptPage.CompanyFundPayReceiptFinish,
                !IsPostBack ? CompanyFundReceiptState.Executed : Status, StartTime, EndTime, ReceiptNo, Type);
            if (BankId != Guid.Empty)
            {
                list = list.Where(ent => ent.PayBankAccountsId == BankId).ToList();
            }
            if (SExecuteTime != DateTime.MinValue)
            {
                list = list.Where(c => c.ExecuteDateTime >= SExecuteTime).ToList();
            }
            if (EExecuteTime != DateTime.MaxValue)
            {
                list = list.Where(c => c.ExecuteDateTime < EExecuteTime).ToList();
            }
            if (CompanyId != Guid.Empty.ToString())
            {
                list = list.Where(act => act.CompanyID.ToString() == CompanyId).ToList();
            }
            if (!string.IsNullOrEmpty(WebSite))
            {
                var companyList = RelatedCompany.Instance.ToList().Where(o => o.WebSite.Contains(WebSite)).Select(act => act.CompanyId).ToList();
                list = list.Where(c => companyList.Contains(c.CompanyID)).ToList();
            }
            if (list.Count > 0)
            {
                BankIds = list.Where(ent => ent.PayBankAccountsId != Guid.Empty).Select(ent => ent.PayBankAccountsId).ToList();
            }
            //合计金额
            var sum = RG_CheckInfo.MasterTableView.Columns.FindByUniqueName("RealityBalance");
            if (list.Count > 0)
            {
                var realityBalanceSum = list.Sum(ent => Math.Abs(ent.RealityBalance));
                sum.FooterText = string.Format("合计：{0}", WebControl.NumberSeparator(realityBalanceSum));
            }
            else
            {
                sum.FooterText = string.Empty;
            }
            RG_CheckInfo.DataSource = list;
        }

        // ReSharper disable once FunctionComplexityOverflow
        protected void RgCheckInfoItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                var ddlStatus = e.Item.FindControl("DDL_CheckState") as DropDownList;
                var rdpstart = e.Item.FindControl("RDP_StartTime") as RadDatePicker;
                var rdpend = e.Item.FindControl("RDP_EndTime") as RadDatePicker;
                var tboxNo = e.Item.FindControl("TB_CompanyFundReciptNO") as TextBox;
                var ddlType = e.Item.FindControl("DDL_ReceivePayType") as DropDownList;
                var ddlBank = e.Item.FindControl("DDL_Bank") as DropDownList;
                var rdpSExecuteTime = e.Item.FindControl("RDP_SExecuteTime") as RadDatePicker;
                var rdpEExecuteTime = e.Item.FindControl("RDP_EExecuteTime") as RadDatePicker;
                var companyId = e.Item.FindControl("DdlCompanyList") as DropDownList;
                var webSide = e.Item.FindControl("TbWebSite") as TextBox;
                var filialeId = e.Item.FindControl("DdlSaleFiliale") as DropDownList;
                if (filialeId != null && !string.IsNullOrEmpty(filialeId.SelectedValue))
                    SelectSaleFilialeId = filialeId.SelectedValue;

                #region[申请时间段]
                if (rdpstart != null) if (rdpstart.SelectedDate != null) StartTime = rdpstart.SelectedDate.Value;
                if (rdpend != null)
                    if (rdpend.SelectedDate != null)
                        EndTime = Convert.ToDateTime(rdpend.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                #endregion

                #region[打款时间段]
                if (rdpSExecuteTime != null) if (rdpSExecuteTime.SelectedDate != null) SExecuteTime = rdpSExecuteTime.SelectedDate.Value;
                if (rdpEExecuteTime != null)
                    if (rdpEExecuteTime.SelectedDate != null)
                        EExecuteTime = Convert.ToDateTime(rdpEExecuteTime.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                #endregion

                if (ddlType != null) Type = (CompanyFundReceiptType)int.Parse(ddlType.SelectedValue);
                if (ddlStatus != null) Status = (CompanyFundReceiptState)int.Parse(ddlStatus.SelectedValue);
                if (tboxNo != null) ReceiptNo = tboxNo.Text;
                if (ddlBank != null) BankId = new Guid(ddlBank.SelectedValue);
                if (companyId != null) CompanyId = companyId.SelectedValue;
                if (webSide != null) WebSite = webSide.Text;
                RG_CheckInfo.CurrentPageIndex = 0;
                RG_CheckInfo.Rebind();
            }
            if (e.CommandName == "Finish")
            {
                int num = 0;
                decimal realityBalance = 0;
                foreach (GridDataItem dataItem in RG_CheckInfo.Items)
                {
                    var cbCheck = (CheckBox)dataItem.FindControl("CB_Check");
                    if (!cbCheck.Checked)
                    {
                        continue;
                    }
                    num++;
                    var receiptId = new Guid(dataItem.GetDataKeyValue("ReceiptID").ToString());
                    CompanyFundReceiptInfo info = _companyFundReceipt.GetCompanyFundReceiptInfo(receiptId);
                    realityBalance += info.RealityBalance;
                }
                if (num > 0)
                {
                    var ddlCheckState = (DropDownList)e.Item.FindControl("DDL_CheckState");
                    if (ddlCheckState.SelectedValue == CompanyFundReceiptState.Finish.ToString())
                    {
                        //完成打款
                        RAM.Alert("信息已完成打款");
                    }
                    else
                    {
                        RAM.ResponseScripts.Add("CheckFinsh(" + num + "," + realityBalance + ");");
                    }
                }
                else
                {
                    RAM.Alert("请选择");
                }
            }
        }

        // ReSharper disable once FunctionComplexityOverflow
        protected void BtnFinishClick(object sender, EventArgs e)
        {
            var filialeList = CacheCollection.Filiale.GetShopList();
            IReckoning reckoning=new Reckoning(GlobalConfig.DB.FromType.Write);
            IWasteBook wasteBook = new WasteBook(GlobalConfig.DB.FromType.Write);
            var companySubject=new CompanySubjectDiscountDal(GlobalConfig.DB.FromType.Write);
            foreach (GridDataItem dataItem in RG_CheckInfo.Items)
            {
                var receiptId = new Guid(dataItem.GetDataKeyValue("ReceiptID").ToString());
                var receiptStatus = dataItem.GetDataKeyValue("ReceiptStatus");
                var cbCheck = (CheckBox)dataItem.FindControl("CB_Check");
                if (!cbCheck.Checked || !Convert.ToInt32(CompanyFundReceiptState.Executed).Equals(receiptStatus))
                {
                    continue;
                }

                CompanyFundReceiptInfo info = _companyFundReceipt.GetCompanyFundReceiptInfo(receiptId);
                var flag = info.ReceiptType == (int)CompanyFundReceiptType.Receive; //收款还是付款
                string reckNo = _codeBll.GetCode(CodeType.GT);
                //如果是其他公司则获取任意总公司
                //Guid originalFilialeId = info.FilialeId;
                Guid filialeId = info.FilialeId;
                string tradecode = info.ReceiptNo;
                BankAccountInfo bankInfo = _bankAccounts.GetBankAccounts(info.PayBankAccountsId);
                var compInfo = _companyCussentDao.GetCompanyCussent(info.CompanyID);
                var filialeInfo = filialeList.FirstOrDefault(act => act.ID == info.CompanyID);
                var isShop = filialeInfo != null; //实体门店
                if (compInfo == null && !isShop)
                {
                    RAM.Alert("不是有效的往来单位信息");
                    return;
                }
                var personnelInfo = CurrentSession.Personnel.Get();
                #region 联盟店查询往来单位收付款单据 add by liangcanren at 2015-03-11 16:30
                CompanyFundReceiptDTO entity = null;
                if (isShop)
                {
                    var parent = filialeInfo != null ? filialeInfo.ParentId : Guid.Empty;
                    if (parent == Guid.Empty)
                    {
                        RAM.Alert("店铺信息未找到!");
                        return;
                    }
                    entity = ShopSao.GetCompanyFundReceiptEntityByOriginalTradeCode(parent, info.ReceiptNo);
                    if (entity == null)
                    {
                        RAM.Alert("联盟店往来单位收付款未找到!");
                        return;
                    }
                }
                #endregion
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        var receiveptFilialeId = Guid.Empty;
                        if (compInfo!=null && compInfo.RelevanceFilialeId != Guid.Empty)
                        {
                            receiveptFilialeId = compInfo.RelevanceFilialeId;
                            if (info.ReceiveBankAccountId == Guid.Empty)
                            {
                                RAM.Alert(string.Format("{0}未选择往来银行账号", info.ReceiptNo));
                                return;
                            }
                        }

                        #region  联盟店往来单位收付款完成 add by liangcanren at 2015-03-11 16:30

                        if (isShop)
                        {
                            var rRDesc = string.Format("[{0}款银行:{1}][{2}款人:{3}]" + "[交易流水号：{4}]",
                                flag ? "收" : "付", entity.CompanyBankName, flag ? "付" : "收", entity.ShopName,
                                info.DealFlowNo);
                            var receiveReckoningDesc =
                                WebControl.RetrunUserAndTime(string.Format("[联盟店总管理对{0}{1}款,详细见备注说明]{2}",
                                    entity.ShopName, flag ? "收" : "付", rRDesc));
                            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            var wdes = string.Format("[往来{0}款（{1}款单位：{2}；交易流水号：{3}），资金{4}，{5}）]", flag ? "收" : "付",
                                flag ? "付" : "收",
                                entity.ShopName, info.DealFlowNo, flag ? "增加" : "减少", dateTime);
                            //往来帐
                            var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), info.FilialeId, info.CompanyID, reckNo,
                                receiveReckoningDesc,
                                flag ? -info.RealityBalance : info.RealityBalance,
                                flag ? (int)ReckoningType.Defray : (int)ReckoningType.Income,
                                (int)ReckoningStateType.Currently,
                                (int)CheckType.NotCheck, (int)AuditingState.Yes,
                                info.ReceiptNo, Guid.Empty)
                            {
                                LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                IsOut = info.IsOut
                            };
                            //资金流
                            var wasteBookinfo = new WasteBookInfo(Guid.NewGuid(), info.PayBankAccountsId, info.ReceiptNo,
                                wdes + "[备注说明：" + info.OtherDiscountCaption + "]",
                                flag ? info.RealityBalance : -info.RealityBalance, (Int32)AuditingState.Yes,
                                flag ? (int)WasteBookType.Increase : (int)WasteBookType.Decrease, info.FilialeId)
                            {
                                LinkTradeCode = info.ReceiptNo,
                                LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                BankTradeCode = string.Empty,
                                State = (int)WasteBookState.Currently,
                                IsOut = info.IsOut
                            };
                            string errorMsg;
                            _reckoning.Insert(reckoningInfo,out errorMsg);
                            if (wasteBookinfo.Income!=0)
                            {
                                _wasteBook.Insert(wasteBookinfo);
                            }
                            string remark = WebControl.RetrunUserAndTime("完成");
                            _companyFundReceipt.UpdateFundReceiptRemark(receiptId, remark);
                            _companyFundReceipt.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.Finish);
                        }

                        #endregion

                        else
                        {
                            #region[付款]
                            if (info.ReceiptType == (int)CompanyFundReceiptType.Payment)
                            {
                                var bankAccount = _companyBankAccountBindDao.GetCompanyBankAccountIdBind(info.CompanyID, filialeId) ?? _companyBankAccountBindDao.GetCompanyBankAccountBindInfo(info.CompanyID, filialeId);
                                compInfo.WebSite = bankAccount.WebSite;

                                string originalTradeCode = info.ReceiptNo;
                                int paymentType = 4;
                                //说明
                                string payType = string.Empty;
                                if (!string.IsNullOrEmpty(info.PurchaseOrderNo))
                                {
                                    payType = "按采购单付款,采购单号：" + info.PurchaseOrderNo;
                                    originalTradeCode = info.PurchaseOrderNo;
                                    paymentType = 1;
                                }
                                if (!string.IsNullOrEmpty(info.StockOrderNos))
                                {
                                    payType = "按入库单付款,入库单号：" + info.StockOrderNos;
                                    paymentType = 2;
                                }
                                if (string.IsNullOrEmpty(info.PurchaseOrderNo) && string.IsNullOrEmpty(info.StockOrderNos) &&
                                    info.SettleEndDate != DateTime.Parse("1999-09-09"))
                                {
                                    paymentType = 3;
                                    payType = "按日期付款,日期:" + info.SettleStartDate.ToString("yyyy/MM/dd") + "到" +
                                              info.SettleEndDate.ToString("yyyy/MM/dd");
                                }
                                if (string.IsNullOrEmpty(info.PurchaseOrderNo) && string.IsNullOrEmpty(info.StockOrderNos) &&
                                    info.SettleStartDate == DateTime.Parse("1999-09-09"))
                                {
                                    payType = "预付款";
                                }
                                //完成打款人
                                var finishPersonnel = CurrentSession.Personnel.Get().RealName;
                                //提交人
                                var applicantPersonnel = _personnelManager.GetName(info.ApplicantID);
                                var pRDesc = "[付款银行:" + bankInfo.BankName + "-" + bankInfo.AccountsName + "][收款人:" + compInfo.WebSite + "]";

                                if (!string.IsNullOrEmpty(info.DealFlowNo))
                                {
                                    pRDesc = "[付款银行:" + bankInfo.BankName + "-" + bankInfo.AccountsName + "][收款人:" + compInfo.WebSite + "]" + "[交易流水号：" + info.DealFlowNo + "]";
                                }
                                var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                //资金流备注
                                var wdes = string.Format("[往来付款（收款人:{0}；收款单位：{1}；付款类型：{2}；提交人：{3}；交易流水号：{4}；完成打款人：{5}），资金减少，{6}）]", compInfo.WebSite, compInfo.CompanyName, payType, applicantPersonnel, info.DealFlowNo, finishPersonnel, dateTime);
                                //往来帐备注
                                var rdes = string.Format("[往来付款（付款类型：{0}；申请人:{1}；完成打款人:{2}；付款银行：{3}；收款人：{4}；收款单位：{5}；交易流水号：{6}）,{7}）]", payType, applicantPersonnel, finishPersonnel, bankInfo.BankName + "-" + bankInfo.AccountsName, compInfo.WebSite, compInfo.CompanyName, info.DealFlowNo, dateTime);
                                //往来帐
                                var reckList = new List<ReckoningInfo>();
                                string errorMsg;
                                CompanyFundReceiptInfo receiveReceipt;
                                var result = CheckingData.CheckCompany(paymentType, info, rdes, reckList, info.IncludeStockNos.Split(',').ToList(), info.DebarStockNos.Split(',').ToList(),out receiveReceipt, out errorMsg);
                                if (!result)
                                {
                                    RAM.Alert(string.Format("往来帐对账异常![{0}]", errorMsg));
                                    return;
                                }
                                if (receiveReceipt != null)
                                {
                                    _companyFundReceipt.Insert(receiveReceipt);
                                }
                                foreach (var reckoningInfo in reckList)
                                {
                                    reckoning.Insert(reckoningInfo,out errorMsg);
                                    //_company.UpDatetCussentExtendInfo(info.CompanyID,
                                    //"[付款单号：" + reckoningInfo.TradeCode + payType + "][备注说明：" + info.OtherDiscountCaption +
                                    //"]" + char.ConvertFromUtf32(10));

                                    var addResult = companySubject.Insert(new CompanySubjectDiscountInfo
                                    {
                                        CompanyId = reckoningInfo.ThirdCompanyID,
                                        FilialeId = reckoningInfo.FilialeId ,
                                        ID = Guid.NewGuid(),
                                        Datecreated = DateTime.Now,
                                        Income = 0,
                                        Memo = string.Format("[付款单号：{0}{1}][备注说明：{2}]{3}", reckoningInfo.TradeCode, payType,
                                        info.OtherDiscountCaption, char.ConvertFromUtf32(10)),
                                        PersonnelName = personnelInfo.RealName,
                                        MemoType = (int)MemoType.Subject
                                    });
                                    if (!addResult)
                                    {
                                        RAM.Alert(string.Format("付款单{0}添加备注说明失败!", reckoningInfo.TradeCode));
                                        return;
                                    }
                                }

                                //资金流
                                var wasteBookinfo = new WasteBookInfo(Guid.NewGuid(), bankInfo.BankAccountsId, _codeBll.GetCode(CodeType.RD),
                                    wdes + "[备注说明：" + info.OtherDiscountCaption + "]",
                                    -info.RealityBalance, (Int32)AuditingState.Yes,
                                    (Int32)WasteBookType.Decrease, filialeId)
                                {
                                    LinkTradeCode = tradecode,
                                    LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                    BankTradeCode = string.Empty,
                                    State = (int)WasteBookState.Currently,
                                    IsOut = bankInfo.IsMain
                                };
                                wasteBook.Insert(wasteBookinfo);

                                if (receiveptFilialeId != Guid.Empty)
                                {
                                    var receiveWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), info.ReceiveBankAccountId, _codeBll.GetCode(CodeType.GI),
                                    wdes + "[备注说明：" + info.OtherDiscountCaption + "]",
                                    info.RealityBalance, (Int32)AuditingState.Yes,
                                    (Int32)WasteBookType.Increase, receiveptFilialeId)
                                    {
                                        LinkTradeCode = info.ReceiptNo,
                                        LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                        BankTradeCode = string.Empty,
                                        State = (int)WasteBookState.Currently,
                                        IsOut = bankInfo.IsMain
                                    };
                                    if (receiveWasteBookInfo.Income != 0)
                                        _wasteBook.Insert(receiveWasteBookInfo);
                                }

                                if (info.Poundage != 0) //手续费
                                {
                                    var newinfo = new WasteBookInfo(Guid.NewGuid(), bankInfo.BankAccountsId,
                                        _codeBll.GetCode(CodeType.RD),
                                        WebControl.RetrunUserAndTime("[往来单位付款单据编号:" +
                                                                     info.ReceiptNo + "][手续费]"),
                                        -info.Poundage, (Int32)AuditingState.Yes,
                                        (Int32)WasteBookType.Decrease, filialeId)
                                    {
                                        LinkTradeCode = tradecode,
                                        LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                        BankTradeCode = string.Empty,
                                        State = (int)WasteBookState.Currently,
                                        IsOut = bankInfo.IsMain,
                                    };
                                    wasteBook.Insert(newinfo);
                                }
                                if (info.DiscountMoney != 0 || info.LastRebate != 0)
                                {
                                    if (info.DiscountMoney != 0)
                                    {
                                        var discountDescription =
                                        WebControl.RetrunUserAndTime("【折扣】[" + payType + "][当年折扣,详细见折扣+返利说明]" + pRDesc);
                                        //折扣往来帐
                                        var discountReckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, info.CompanyID,
                                            _codeBll.GetCode(CodeType.PY),
                                            discountDescription,
                                            info.DiscountMoney,
                                            (int)ReckoningType.Income,
                                            (int)ReckoningStateType.Currently,
                                            (int)CheckType.IsChecked,
                                            (int)AuditingState.Yes, originalTradeCode,
                                            Guid.Empty)
                                        {
                                            LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                            IsOut = info.IsOut
                                        };
                                        reckoning.Insert(discountReckoningInfo,out errorMsg);
                                    }
                                    if (info.LastRebate != 0)
                                    {
                                        var lastRebateDescription =
                                        WebControl.RetrunUserAndTime("【返利】[" + payType + "][去年返利,详细见折扣+返利说明]" + pRDesc);
                                        //折扣往来帐
                                        var lastRebateReckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, info.CompanyID,
                                            _codeBll.GetCode(CodeType.PY),
                                            lastRebateDescription,
                                            info.LastRebate,
                                            (int)ReckoningType.Income,
                                            (int)ReckoningStateType.Currently,
                                            (int)CheckType.IsChecked,
                                            (int)AuditingState.Yes, originalTradeCode,
                                            Guid.Empty)
                                        {
                                            LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                            IsOut = info.IsOut
                                        };
                                        reckoning.Insert(lastRebateReckoningInfo,out errorMsg);
                                    }
                                    var addResult =companySubject.Insert(new CompanySubjectDiscountInfo
                                    {
                                        CompanyId = info.CompanyID,
                                        FilialeId = filialeId,
                                        ID = Guid.NewGuid(),
                                        Datecreated = DateTime.Now,
                                        Income = info.DiscountMoney + info.LastRebate,
                                        Memo = string.Format("[付款单号：{0}{1}][折扣+返利说明：{2}]{3}", info.ReceiptNo, payType,
                                        info.DiscountCaption, char.ConvertFromUtf32(10)),
                                        PersonnelName = personnelInfo.RealName,
                                        MemoType = (int)MemoType.Discount
                                    });
                                    if (!addResult)
                                    {
                                        RAM.Alert(string.Format("付款单{0}添加折扣+返利说明失败!", info.ReceiptNo));
                                        return;
                                    }
                                }
                                //string remark = info.IsOut ? WebControl.RetrunUserAndTime("完成转入发票待索取状态") : WebControl.RetrunUserAndTime("完成");
                                string remark = WebControl.RetrunUserAndTime("完成");
                                _companyFundReceipt.UpdateFundReceiptRemark(receiptId, remark);
                                //var receiptState = info.IsOut ? CompanyFundReceiptState.GettingInvoice : CompanyFundReceiptState.Finish;
                                _companyFundReceipt.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.Finish);
                            }

                            #endregion

                            #region[收款]

                            if (info.ReceiptType == (int)CompanyFundReceiptType.Receive)
                            {
                                var rRDesc = "[收款银行:" + bankInfo.AccountsName + "-" + bankInfo.BankName + "][付款人:" +
                                             compInfo.WebSite + "]";
                                if (!string.IsNullOrEmpty(info.DealFlowNo))
                                {
                                    rRDesc = "[收款银行:" + bankInfo.AccountsName + "-" + bankInfo.BankName + "][付款人:" + compInfo.WebSite + "]" +
                                             "[交易流水号：" + info.DealFlowNo + "]";
                                }
                                var receiveReckoningDesc =
                                    WebControl.RetrunUserAndTime("[收款,日期间隔是:" + info.SettleStartDate.ToString("yyyy/MM/dd") +
                                                                 "到" + info.SettleEndDate.ToString("yyyy/MM/dd") +
                                                                 ",详细见备注说明]" + rRDesc);
                                var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                var wdes = string.Format("[往来收款（付款人:{0}；付款单位：{1}；交易流水号：{2}），资金增加，{3}）]", compInfo.WebSite, compInfo.CompanyName,
                                    info.DealFlowNo, dateTime);
                                //往来帐
                                var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, info.CompanyID, reckNo,
                                    receiveReckoningDesc,
                                    -info.RealityBalance, (int)ReckoningType.Defray,
                                    (int)ReckoningStateType.Currently,
                                    (int)CheckType.IsChecked, (int)AuditingState.Yes,
                                    info.ReceiptNo, Guid.Empty)
                                {
                                    LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                    IsOut = info.IsOut
                                };
                                //资金流
                                var wasteBookinfo = new WasteBookInfo(Guid.NewGuid(), bankInfo.BankAccountsId, tradecode,
                                    wdes + "[备注说明：" + info.OtherDiscountCaption + "]",
                                    info.RealityBalance, (Int32)AuditingState.Yes,
                                    (Int32)WasteBookType.Increase, filialeId)
                                {
                                    LinkTradeCode = tradecode,
                                    LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                    BankTradeCode = string.Empty,
                                    State = (int)WasteBookState.Currently,
                                    IsOut = bankInfo.IsMain
                                };
                                string errorMsg;
                                reckoning.Insert(reckoningInfo,out errorMsg);
                                wasteBook.Insert(wasteBookinfo);

                                if (receiveptFilialeId != Guid.Empty)
                                {
                                    var receiveWasteBookInfo = new WasteBookInfo(Guid.NewGuid(), info.ReceiveBankAccountId, _codeBll.GetCode(CodeType.RD),
                                    wdes + "[备注说明：" + info.OtherDiscountCaption + "]",
                                    -info.RealityBalance, (Int32)AuditingState.Yes,
                                    (Int32)WasteBookType.Decrease, receiveptFilialeId)
                                    {
                                        LinkTradeCode = info.ReceiptNo,
                                        LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                                        BankTradeCode = string.Empty,
                                        State = (int)WasteBookState.Currently,
                                        IsOut = bankInfo.IsMain
                                    };
                                    if (receiveWasteBookInfo.Income != 0)
                                        _wasteBook.Insert(receiveWasteBookInfo);
                                }

                                var addSubjectResult = companySubject.Insert(new CompanySubjectDiscountInfo
                                {
                                    CompanyId = reckoningInfo.ThirdCompanyID,
                                    FilialeId = reckoningInfo.FilialeId,
                                    ID = Guid.NewGuid(),
                                    Datecreated = DateTime.Now,
                                    Income = 0,
                                    Memo = string.Format("[收款单号：{0}时间间隔是：{1}到{2}][备注说明：{3}]{4}", reckoningInfo.TradeCode, info.SettleStartDate.ToString("yyyy/MM/dd"),
                                    info.SettleEndDate.ToString("yyyy/MM/dd"),
                                    info.OtherDiscountCaption, char.ConvertFromUtf32(10)),
                                    PersonnelName = personnelInfo.RealName,
                                    MemoType = (int)MemoType.Subject
                                });
                                if (!addSubjectResult)
                                {
                                    RAM.Alert(string.Format("收款单{0}添加备注说明失败!", reckoningInfo.TradeCode));
                                    return;
                                }

                                if (info.DiscountMoney != 0 || info.LastRebate != 0)
                                {
                                    if (info.DiscountMoney != 0)
                                    {
                                        var discountDesc =
                                        WebControl.RetrunUserAndTime("[收款,日期间隔是:" +
                                                                     info.SettleStartDate.ToString("yyyy/MM/dd") + "到" +
                                                                     info.SettleEndDate.ToString("yyyy/MM/dd") +
                                                                     "][当年折扣,详细见折扣+返利说明]");
                                        //折扣往来帐
                                        var discountReckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, info.CompanyID,
                                            _codeBll.GetCode(CodeType.GT),
                                            discountDesc,
                                            -info.DiscountMoney,
                                            (int)ReckoningType.Defray,
                                            (int)ReckoningStateType.Currently,
                                            (int)CheckType.IsChecked,
                                            (int)AuditingState.Yes, info.ReceiptNo,
                                            Guid.Empty)
                                        {
                                            LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                            IsOut = info.IsOut,
                                            IsAllow = true
                                        };
                                        reckoning.Insert(discountReckoningInfo,out errorMsg);
                                    }
                                    if (info.LastRebate != 0)
                                    {
                                        var lastRebateDescription = WebControl.RetrunUserAndTime("[收款,日期间隔是:" +
                                                                     info.SettleStartDate.ToString("yyyy/MM/dd") + "到" +
                                                                     info.SettleEndDate.ToString("yyyy/MM/dd") +
                                                                     "][去年返利,详细见折扣+返利说明]");
                                        //折扣往来帐
                                        var lastRebateReckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, info.CompanyID,
                                            _codeBll.GetCode(CodeType.GT),
                                            lastRebateDescription,
                                            -info.LastRebate,
                                            (int)ReckoningType.Defray,
                                            (int)ReckoningStateType.Currently,
                                            (int)CheckType.IsChecked,
                                            (int)AuditingState.Yes, info.ReceiptNo,
                                            Guid.Empty)
                                        {
                                            LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                                            IsOut = info.IsOut
                                        };
                                        reckoning.Insert(lastRebateReckoningInfo,out errorMsg);
                                    }



                                    var addResult =companySubject.Insert(new CompanySubjectDiscountInfo
                                    {
                                        CompanyId = info.CompanyID,
                                        FilialeId = filialeId ,
                                        ID = Guid.NewGuid(),
                                        Datecreated = DateTime.Now,
                                        Income = info.DiscountMoney + info.LastRebate,
                                        Memo = string.Format("[收款单号：{0}日期间隔是：{1}到{2}][折扣+返利说明：{3}]{4}", info.ReceiptNo, info.SettleStartDate.ToString("yyyy/MM/dd"),
                                        info.SettleEndDate.ToString("yyyy/MM/dd"),
                                        info.DiscountCaption, char.ConvertFromUtf32(10)),
                                        PersonnelName = personnelInfo.RealName,
                                        MemoType = (int)MemoType.Discount
                                    });
                                    if (!addResult)
                                    {
                                        RAM.Alert(string.Format("收款单{0}添加折扣+返利说明失败!", info.ReceiptNo));
                                        return;
                                    }
                                }
                                var remark = WebControl.RetrunUserAndTime("完成");
                                _companyFundReceipt.UpdateFundReceiptRemark(receiptId, remark);
                                _companyFundReceipt.UpdateFundReceiptState(receiptId, CompanyFundReceiptState.Finish);
                            }

                            #endregion
                        }
                        _companyFundReceipt.SetDateTime(receiptId, 3);
                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert("完成打款失败!" + ex.Message);
                        return;
                    }
                    finally
                    {
                        ts.Dispose();
                    }
                }
            }
            RG_CheckInfo.Rebind();
        }

        #region[绑定所有状态]
        protected Dictionary<int, string> BindStatusDataBound()
        {
            var newDic = new Dictionary<int, string>();
            try
            {
                var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptState>();
                foreach (var status in stateDic)
                {
                    if (status.Key == -2 || status.Key == -3)
                    {
                        newDic.Add(status.Key, status.Value);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
            return newDic;
        }
        #endregion

        #region[绑定收付款类型]
        protected Dictionary<int, string> BindTypeDataBound()
        {
            return (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptType>();
        }
        #endregion

        #region 属性

        protected Guid BankId
        {
            set { ViewState["BankId"] = value; }
            get
            {
                if (ViewState["BankId"] == null)
                {
                    return Guid.Empty;
                }
                return new Guid(ViewState["BankId"].ToString());
            }
        }

        protected DateTime StartTime
        {
            set { ViewState["StartTime"] = value; }
            get
            {
                if (ViewState["StartTime"] == null)
                {
                    return DateTime.MinValue;
                }
                return DateTime.Parse(ViewState["StartTime"].ToString());
            }
        }

        protected DateTime EndTime
        {
            set { ViewState["EndTime"] = value; }
            get
            {
                if (ViewState["EndTime"] == null)
                {
                    return DateTime.MaxValue;
                }
                return DateTime.Parse(ViewState["EndTime"].ToString());
            }
        }

        protected DateTime SExecuteTime
        {
            set { ViewState["SExecuteTime"] = value; }
            get
            {
                if (ViewState["SExecuteTime"] == null)
                {
                    return DateTime.MinValue;
                }
                return DateTime.Parse(ViewState["SExecuteTime"].ToString());
            }
        }

        protected DateTime EExecuteTime
        {
            set { ViewState["EExecuteTime"] = value; }
            get
            {
                if (ViewState["EExecuteTime"] == null)
                {
                    return DateTime.MaxValue;
                }
                return DateTime.Parse(ViewState["EExecuteTime"].ToString());
            }
        }

        protected string ReceiptNo
        {
            set { ViewState["ReceiptNo"] = value; }
            get
            {
                return ViewState["ReceiptNo"].ToString();
            }
        }

        protected CompanyFundReceiptState Status
        {
            set { ViewState["Status"] = value; }
            get
            {
                return (CompanyFundReceiptState)ViewState["Status"];
            }
        }

        protected CompanyFundReceiptType Type
        {
            set { ViewState["Type"] = value; }
            get
            {
                return (CompanyFundReceiptType)ViewState["Type"];
            }
        }

        protected List<Guid> BankIds
        {
            set { ViewState["BankIds"] = value; }
            get
            {
                if (ViewState["BankIds"] == null)
                {
                    return new List<Guid>();
                }
                return (List<Guid>)ViewState["BankIds"];
            }
        }

        /// <summary>
        /// 收款人
        /// </summary>
        public string WebSite
        {
            get
            {
                if (ViewState["WebSite"] == null)
                {
                    ViewState["WebSite"] = string.Empty;
                }
                return ViewState["WebSite"].ToString();
            }
            set
            {
                ViewState["WebSite"] = value;
            }
        }

        /// <summary>
        /// 往来单位
        /// </summary>
        public string CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null)
                {
                    ViewState["CompanyId"] = Guid.Empty.ToString();
                }
                return ViewState["CompanyId"].ToString();
            }
            set
            {
                ViewState["CompanyId"] = value;
            }
        }

        #endregion

        #region 显示文字方法
        /// <summary>
        /// 显示往来单位收付款往来单位
        /// modify by liangcanren at 2015-03-16
        /// </summary>
        /// <param name="compId"></param>
        /// <returns></returns>
        protected string GetCompName(string compId)
        {
            var list = RelatedCompany.Instance.ToList();
            if (list == null)
                return "-";
            var info = list.FirstOrDefault(o => o.CompanyId == new Guid(compId));
            if (info == null)
            {
                var shopList = CacheCollection.Filiale.GetShopList();
                if (shopList == null)
                    return "-";
                var shopInfo = shopList.FirstOrDefault(o => o.ID == new Guid(compId));
                return shopInfo == null ? "-" : shopInfo.Name;
            }
            return info.CompanyName;
        }

        protected string GetReceiptStatus(string receiptStatus)
        {
            var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptState>();

            foreach (KeyValuePair<int, string> kvp in stateDic)
            {
                if (receiptStatus == string.Format("{0}", kvp.Key))
                {
                    return kvp.Value;
                }
            }
            return "未知状态";
        }

        protected string GetReceiptTypeName(string type)
        {
            var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptType>();

            foreach (KeyValuePair<int, string> kvp in stateDic)
            {
                if (type == string.Format("{0}", kvp.Key))
                {
                    return kvp.Value;
                }
            }
            return "未知类型";
        }

        protected string GetColor(string type)
        {
            if (type == string.Format("{0}", (int)CompanyFundReceiptType.Receive))
                return "#FF8C69";
            return "black";
        }
        #endregion

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_CheckInfo, e);
        }

        #region[根据往来单位ID获取往来单位信息]

        /// <summary>
        /// 根据往来单位ID获取往来单位信息
        /// Add by liucaijun at 2011-October-10th
        /// </summary>
        /// <param name="companyId">往来单位ID</param>
        /// <param name="filialeId"></param>
        /// <param name="type">0 website 1 AccountNumber 2 BankAccounts</param>
        /// <returns></returns>
        public string GetCompanyCussentByCompanyId(Guid companyId, Guid filialeId, int type)
        {
            var info = _companyBankAccountBindDao.GetCompanyBankAccountIdBind(companyId, filialeId)??_companyBankAccountBindDao.GetCompanyBankAccountBindInfo(companyId, filialeId);
            if (info != null && info.CompanyId != Guid.Empty)
            {
                return type == 0 ? info.WebSite : type == 1 ? info.AccountsNumber : info.BankAccounts;
            }
            var companyCussentInfo = _companyCussentDao.GetCompanyCussent(companyId);
            if (companyCussentInfo != null)
            {
                return type == 0 ? companyCussentInfo.WebSite : type == 1 ? companyCussentInfo.AccountNumber : companyCussentInfo.BankAccounts;
            }
            return string.Empty;
        }
        #endregion

        #region[绑定银行账号]
        protected List<ListItem> BindBankDataBound()
        {
            var listItem = new List<ListItem>();
            if (BankIds.Count > 0)
            {
                foreach (var bankInfo in BankIds.Distinct().Select(id => BankAccountManager.ReadInstance.GetBankAccounts(id)).Where(bankInfo => bankInfo.IsUse))
                {
                    listItem.Add(new ListItem(bankInfo.BankName + "-" + bankInfo.AccountsName, bankInfo.BankAccountsId.ToString()));
                }
            }
            listItem.Insert(0, new ListItem("全部", Guid.Empty.ToString()));
            return listItem;
        }

        /// <summary>
        /// 绑定往来单位
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> BindCompanyDataBound()
        {
            var newDic = new Dictionary<string, string> { { Guid.Empty.ToString(), " " } };
            var companyList = _companyCussentDao.GetCompanyCussentList();
            foreach (var info in companyList)
            {
                newDic.Add(info.CompanyId.ToString(), info.CompanyName);
            }
            return newDic;
        }
        #endregion

        protected void RG_CheckInfo_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.CommandItem)
            {
                var startTime = e.Item.FindControl("RDP_StartTime") as RadDatePicker;
                var endTime = e.Item.FindControl("RDP_EndTime") as RadDatePicker;
                var startTime1 = e.Item.FindControl("RDP_SExecuteTime") as RadDatePicker;
                var endTime1 = e.Item.FindControl("RDP_EExecuteTime") as RadDatePicker;
                if (StartTime != DateTime.MinValue)
                {
                    if (startTime != null) startTime.SelectedDate = StartTime;
                }
                if (EndTime != DateTime.MaxValue)
                {
                    if (endTime != null) endTime.SelectedDate = EndTime;
                }
                if (SExecuteTime != DateTime.MinValue)
                {
                    if (startTime1 != null) startTime1.SelectedDate = SExecuteTime;
                }
                if (EExecuteTime != DateTime.MaxValue)
                {
                    if (endTime1 != null) endTime1.SelectedDate = EExecuteTime;
                }
            }
        }

        #region  添加公司
        /// <summary>
        /// 公司列表
        /// </summary>
        public IList<FilialeInfo> SaleFilialeList
        {
            get
            {
                if (ViewState["SaleFilialeList"] == null)
                {
                    ViewState["SaleFilialeList"] = CacheCollection.Filiale.GetHostingAndSaleFilialeList();
                }
                return (IList<FilialeInfo>)ViewState["SaleFilialeList"];
            }
            set
            {
                ViewState["SaleFilialeList"] = value;
            }
        }

        /// <summary>
        /// 销售公司
        /// </summary>
        protected string SelectSaleFilialeId
        {
            get
            {
                return ViewState["SaleFilialeId"] == null ? Guid.Empty.ToString()
                    : ViewState["SaleFilialeId"].ToString();
            }
            set
            {
                ViewState["SaleFilialeId"] = value;
            }
        }

        protected string GetBankNameById(object obj)
        {
            var id = new Guid(obj.ToString());
            if (id == Guid.Empty) return string.Empty;
            var bankAccountInfo = _bankAccounts.GetBankAccounts(id);
            return bankAccountInfo != null ? bankAccountInfo.BankName : "-";
        }

        /// <summary>
        /// 绑定公司
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> BindFilialeDataBound()
        {
            var newDic = new Dictionary<string, string> { { Guid.Empty.ToString(), string.Empty } };
            foreach (var info in SaleFilialeList)
            {
                newDic.Add(info.ID.ToString(), info.Name);
            }
            return newDic;
        }
        #endregion

        /// <summary>
        /// 显示公司
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        protected string GetFilialeName(string filialeId)
        {
            var info = SaleFilialeList.FirstOrDefault(act => act.ID.ToString() == filialeId);
            if (info != null) return info.Name;
            if (filialeId != Guid.Empty.ToString()) return "ERP";
            return "-";
        }
    }
}

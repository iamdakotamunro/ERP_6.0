using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using MediumReckoning = ERP.BLL.Implement.Inventory.MediumReckoning;

namespace ERP.UI.Web
{
    public partial class CheckHistory : BasePage
    {
        private static readonly CheckDataManager _checkDataManagerWrite = CheckDataManager.WriteInstance;
        private readonly CodeManager _codeManager = new CodeManager();
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        SubmitController _submitController;

        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["Checking"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid(), 5);
                ViewState["Checking"] = _submitController;
            }
            return (SubmitController)ViewState["Checking"];
        }

        // 开始时间
        protected DateTime StartDate
        {
            get
            {
                if (ViewState["StartDate"] == null || Convert.ToDateTime(ViewState["StartDate"]) == DateTime.MinValue)
                {
                    return DateTime.MinValue;
                }
                return Convert.ToDateTime(ViewState["StartDate"]);
            }
            set
            {
                ViewState["StartDate"] = value;
            }
        }
        // 结束时间
        protected DateTime EndDate
        {
            get
            {
                if (ViewState["EndDate"] == null || Convert.ToDateTime(ViewState["EndDate"]) == DateTime.MinValue || Convert.ToDateTime(ViewState["EndDate"]) == DateTime.MaxValue)
                {
                    return Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                }
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndDate"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            }
            set
            {
                ViewState["EndDate"] = value;
            }
        }
        //对账人
        protected String Checker
        {
            get
            {
                if (ViewState["Checker"] == null)
                    return String.Empty;
                return ViewState["Checker"].ToString();
            }
            set
            {
                ViewState["Checker"] = value;
            }
        }

        protected Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set
            {
                ViewState["CompanyId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!Page.IsPostBack)
            {                
                RcbCompanyList.DataSource = _companyCussent.GetCompanyCussentList(CompanyType.Express, State.Enable);
                RcbCompanyList.DataTextField = "CompanyName";
                RcbCompanyList.DataValueField = "CompanyId";
                RcbCompanyList.DataBind();
                RcbCompanyList.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
            }
        }
        protected void LbSearchClick(object sender, EventArgs e)
        {
            rgCheckHistory.Rebind();
        }
        protected void RgCheckHistoryNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            StartDate = RDP_StartDate.SelectedDate ?? DateTime.Now.AddMonths(-3);
            EndDate = RDP_EndDate.SelectedDate ?? DateTime.Now;
            Checker = txtChecker.Text.Trim();
            if (RcbCompanyList.SelectedItem != null && RcbCompanyList.SelectedIndex >= 0)
                CompanyId = new Guid(RcbCompanyList.SelectedItem.Value);
            else
            {
                CompanyId = Guid.Empty;
            }
            int state = DdlState.SelectedValue == null ? -1 : Convert.ToInt32(DdlState.SelectedValue);
            var enumList = EnumAttribute.GetDict<CheckDataState>();
            int[] splieLIst = { (int)CheckDataState.All, (int)CheckDataState.Deleted };
            int[] states = enumList.Where(act => !splieLIst.Contains(act.Key)).Select(act => act.Key).ToArray();
            var list = _checkDataManagerWrite.GetCheckDataList(CompanyId, -1, Checker, StartDate, EndDate, states);
            switch (state)
            {
                case 0:
                    list = list.Where(act => act.CheckDataState <= 7).ToList();
                    break;
                case 1:
                    list = list.Where(act => act.CheckDataState > 7).ToList();
                    break;
            }
            rgCheckHistory.DataSource = list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // ReSharper disable once FunctionComplexityOverflow
        protected void OnFinishClick(object sender, EventArgs e)
        {
            if (!_submitController.Enabled)
            {
                RAM.Alert("请不要重复提交!");
                return;
            }
            if (rgCheckHistory.SelectedItems.Count == 0)
            {
                RAM.Alert("请选择待完成帐!");
                return;
            }
            var checkList = new List<CheckDataRecordInfo>();
            var dictionary = new Dictionary<Guid, int>();
            foreach (GridDataItem item in rgCheckHistory.SelectedItems)
            {
                var checkId = new Guid(item.GetDataKeyValue("CheckId").ToString());
                var info = _checkDataManagerWrite.GetCheckDataInfoById(checkId);
                if (info != null)
                {
                    switch (info.CheckDataState)
                    {
                        case (int)CheckDataState.Finished:
                            checkList.Add(info);
                            dictionary.Add(info.CheckId, info.CheckType);
                            break;
                        default:
                            RAM.Alert("系统提示：选项中存在已完成或处理中对账记录！");
                            return;
                    }
                }
            }
            var companyList = checkList.GroupBy(act => act.CheckCompanyId).Select(act => act.Key).ToList();
            if (companyList.Count > 1)
            {
                RAM.Alert("系统提示：多条记录对账只能在同快递公司之间！");
                return;
            }
            IList<CompanyFundReceiptInfo> receiptList = new List<CompanyFundReceiptInfo>();
            var personnelInfo = CurrentSession.Personnel.Get();

            IList<ExceptionReckoningInfo> newList = new BindingList<ExceptionReckoningInfo>();

            #region [计算当前选择的对账记录需要生成的各公司的收付款记录]

            //计算当前选择的对账记录需要生成的各公司的收付款记录
            foreach (var key in dictionary.Keys)
            {
                var list = _checkDataManagerWrite.GetTotalMoney(key); //实际对账金额 (实收金额累计)
                foreach (var item in list)
                {
                    decimal totalMoney = 0;
                    decimal serverMoney = 0;
                    decimal exceptionMoney;
                    if (dictionary[key] == (int)ReckoningCheckType.Carriage)
                    {
                        totalMoney -= item.SumFinanceConfirmMoney;
                        serverMoney -= item.SumConfirmMoney;
                        exceptionMoney = item.SumConfirmMoney - item.SumFinanceConfirmMoney;
                    }
                    else
                    {
                        totalMoney += item.SumFinanceConfirmMoney;
                        serverMoney += item.SumConfirmMoney;
                        exceptionMoney = item.SumFinanceConfirmMoney - item.SumConfirmMoney;
                    }
                    if (totalMoney == 0) continue;
                    //公司收付款
                    if (item.IsOut)
                    {
                        var tempInfo = newList.FirstOrDefault(ent => ent.FilialeId == item.FilialeId && ent.IsOut);
                        if (tempInfo != null)
                        {
                            /*------公司账务合并------||-------------------------收款----------------------------||-------------------------付款----------------------------*/
                            //zal 2015-11-23
                            if (CKB_Merger.Checked || (totalMoney > 0 && tempInfo.SumFinanceConfirmMoney > 0) || (totalMoney < 0 && tempInfo.SumFinanceConfirmMoney < 0))
                            {
                                tempInfo.FilialeId = item.FilialeId;
                                tempInfo.FilialeName = item.FilialeName;
                                tempInfo.DiffMoney += exceptionMoney;
                                tempInfo.SumConfirmMoney += serverMoney;
                                tempInfo.SumFinanceConfirmMoney += totalMoney;
                                tempInfo.IsOut = true;
                            }
                            else
                            {
                                var tempInfo1 = new ExceptionReckoningInfo
                                {
                                    FilialeId = item.FilialeId,
                                    FilialeName = item.FilialeName,
                                    DiffMoney = exceptionMoney,
                                    SumConfirmMoney = serverMoney,
                                    SumFinanceConfirmMoney = totalMoney,
                                    IsOut = true
                                };
                                newList.Add(tempInfo1);
                            }
                        }
                        else
                        {
                            var tempInfo1 = new ExceptionReckoningInfo
                            {
                                FilialeId = item.FilialeId,
                                FilialeName = item.FilialeName,
                                DiffMoney = exceptionMoney,
                                SumConfirmMoney = serverMoney,
                                SumFinanceConfirmMoney = totalMoney,
                                IsOut = true
                            };
                            newList.Add(tempInfo1);
                        }
                    }
                    else
                    {
                        var tempInfo = newList.FirstOrDefault(ent => ent.FilialeId == _reckoningElseFilialeid && ent.IsOut == false);
                        if (tempInfo != null)
                        {
                            /*------公司账务合并------||-------------------------收款----------------------------||-------------------------付款----------------------------*/
                            //zal 2015-11-23
                            if (CKB_Merger.Checked || (totalMoney > 0 && tempInfo.SumFinanceConfirmMoney > 0) || (totalMoney < 0 && tempInfo.SumFinanceConfirmMoney < 0))
                            {
                                tempInfo.FilialeId = _reckoningElseFilialeid;
                                tempInfo.FilialeName = "ERP";
                                tempInfo.DiffMoney += exceptionMoney;
                                tempInfo.SumConfirmMoney += serverMoney;
                                tempInfo.SumFinanceConfirmMoney += totalMoney;
                                tempInfo.IsOut = false;
                            }
                            else
                            {
                                var tempInfo1 = new ExceptionReckoningInfo
                                {
                                    FilialeId = _reckoningElseFilialeid,
                                    FilialeName = "ERP",
                                    DiffMoney = exceptionMoney,
                                    SumConfirmMoney = serverMoney,
                                    SumFinanceConfirmMoney = totalMoney,
                                    IsOut = false
                                };
                                newList.Add(tempInfo1);
                            }
                        }
                        else
                        {
                            var tempInfo1 = new ExceptionReckoningInfo
                            {
                                FilialeId = _reckoningElseFilialeid,
                                FilialeName = "ERP",
                                DiffMoney = exceptionMoney,
                                SumConfirmMoney = serverMoney,
                                SumFinanceConfirmMoney = totalMoney,
                                IsOut = false
                            };
                            newList.Add(tempInfo1);
                        }
                    }
                }
            }

            #endregion

            #region [生成个公司收付款单据]
            //生成个公司收付款单据
            foreach (var info in newList)
            {
                if (info.IsOut)
                {
                    //生成往来单位收付款
                    var receipt = new CompanyFundReceiptInfo
                    {
                        ReceiptNo = _codeManager.GetCode(info.SumFinanceConfirmMoney > 0 ? CodeType.GT : CodeType.PY),
                        ReceiptType = Convert.ToInt32(info.SumFinanceConfirmMoney > 0 ? CompanyFundReceiptType.Receive : CompanyFundReceiptType.Payment),
                        ApplyDateTime = DateTime.Now,
                        ApplicantID = personnelInfo.PersonnelId,
                        PurchaseOrderNo = string.Empty,
                        CompanyID = companyList[0]
                    };
                    string msg = "{0} 应{1}总额：{2},我方总额：{3}，异常总额：{4}";  //确认金额，系统金额，异常金额
                    msg = string.Format(msg, info.FilialeName, info.SumFinanceConfirmMoney > 0 ? "收" : "付", Math.Abs(info.SumFinanceConfirmMoney).ToString("f2"),
                        Math.Abs(info.SumConfirmMoney).ToString("f2"), Math.Abs(info.DiffMoney).ToString("f2"));
                    receipt.HasInvoice = true;
                    receipt.SettleStartDate = Convert.ToDateTime("1999-09-09");
                    receipt.SettleEndDate = Convert.ToDateTime("1999-09-09");
                    receipt.ExpectBalance = Math.Abs(info.SumFinanceConfirmMoney);
                    receipt.RealityBalance = Math.Abs(info.SumFinanceConfirmMoney);
                    receipt.DiscountMoney = 0;
                    receipt.DiscountCaption = string.Empty;
                    receipt.OtherDiscountCaption = "[快递对账生成的收付款]";
                    receipt.FilialeId = info.FilialeId;
                    receipt.ReceiptStatus = info.SumFinanceConfirmMoney < 0
                        ? Convert.ToInt32(CompanyFundReceiptState.WaitAuditing)
                        : Convert.ToInt32(CompanyFundReceiptState.WaitInvoice);
                    receipt.StockOrderNos = string.Empty;
                    receipt.Remark = WebControl.RetrunUserAndTime(msg);
                    receipt.IsOut = true;
                    receipt.PaymentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM"));
                    receiptList.Add(receipt);
                }
                else
                {
                    var receipt = new CompanyFundReceiptInfo
                    {
                        ReceiptNo = _codeManager.GetCode(info.SumFinanceConfirmMoney > 0 ? CodeType.GT : CodeType.PY),
                        ReceiptType = Convert.ToInt32(info.SumFinanceConfirmMoney > 0 ? CompanyFundReceiptType.Receive : CompanyFundReceiptType.Payment),
                        ApplyDateTime = DateTime.Now,
                        ApplicantID = personnelInfo.PersonnelId,
                        PurchaseOrderNo = string.Empty,
                        CompanyID = companyList[0]
                    };
                    string msg = "{0} 应{1}总额：{2},我方总额：{3}，异常总额：{4}";  //确认金额，系统金额，异常金额
                    msg = string.Format(msg, "其他公司", info.SumFinanceConfirmMoney > 0 ? "收" : "付", Math.Abs(info.SumFinanceConfirmMoney).ToString("f2"), Math.Abs(info.SumConfirmMoney).ToString("f2"), Math.Abs(info.DiffMoney).ToString("f2"));
                    receipt.HasInvoice = false;
                    receipt.SettleStartDate = Convert.ToDateTime("1999-09-09");
                    receipt.SettleEndDate = Convert.ToDateTime("1999-09-09");
                    receipt.ExpectBalance = Math.Abs(info.SumFinanceConfirmMoney);
                    receipt.RealityBalance = Math.Abs(info.SumFinanceConfirmMoney);
                    receipt.DiscountMoney = 0;
                    receipt.DiscountCaption = string.Empty;
                    receipt.OtherDiscountCaption = "[快递对账生成的收付款]";
                    receipt.FilialeId = _reckoningElseFilialeid;
                    receipt.ReceiptStatus = info.SumFinanceConfirmMoney < 0
                        ? Convert.ToInt32(CompanyFundReceiptState.WaitAuditing)
                        : Convert.ToInt32(CompanyFundReceiptState.Audited);
                    receipt.StockOrderNos = string.Empty;
                    receipt.Remark = WebControl.RetrunUserAndTime(msg);
                    receipt.IsOut = false;
                    receipt.PaymentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM"));
                    receiptList.Add(receipt);
                }
            }
            #endregion

            #region [最初处理方式暂注释]
            //foreach (var key in dictionary.Keys)
            //{
            //    //decimal money = checkDataManager.GetTotalMoney(key);
            //    //decimal server = checkDataManager.GetServerMoney(key);
            //    var list = checkDataManager.GetTotalMoney(key); //实际对账金额 (实收金额累计)
            //    IList<ExceptionReckoningInfo> filialeExceptionReckoningInfoList = new BindingList<ExceptionReckoningInfo>();
            //    IList<ExceptionReckoningInfo> elseExceptionReckoningInfoList = new BindingList<ExceptionReckoningInfo>();
            //    foreach (var item in list)
            //    {
            //        if (item.IsOut)
            //            filialeExceptionReckoningInfoList.Add(item);
            //        else
            //            elseExceptionReckoningInfoList.Add(item);
            //    }
            //    //生成公司的往来单位收付款
            //    foreach (var item in filialeExceptionReckoningInfoList)
            //    {
            //        decimal totalMoney = 0;
            //        decimal serverMoney = 0;
            //        decimal exceptionMoney;
            //        if (dictionary[key] == (int)ReckoningCheckType.Carriage)
            //        {
            //            totalMoney -= item.SumFinanceConfirmMoney;
            //            serverMoney -= item.SumConfirmMoney;
            //            exceptionMoney = item.SumConfirmMoney - item.SumFinanceConfirmMoney;
            //        }
            //        else
            //        {
            //            totalMoney += item.SumFinanceConfirmMoney;
            //            serverMoney += item.SumConfirmMoney;
            //            exceptionMoney = item.SumFinanceConfirmMoney - item.SumConfirmMoney;
            //        }
            //        if (totalMoney == 0) continue;
            //        //生成往来单位收付款
            //        var receipt = new CompanyFundReceiptInfo
            //        {
            //            ReceiptNo = codeManager.GetCode(totalMoney > 0 ? CodeType.GT : CodeType.PY),
            //            ReceiptType = Convert.ToInt32(totalMoney > 0 ? CompanyFundReceiptType.Receive : CompanyFundReceiptType.Payment),
            //            ApplyDateTime = DateTime.Now,
            //            ApplicantID = personnelInfo.PersonnelId,
            //            PurchaseOrderNo = string.Empty,
            //            CompanyID = companyList[0]
            //        };
            //        string msg = "{0} 应{1}总额：{2},我方总额：{3}，异常总额：{4}";  //确认金额，系统金额，异常金额
            //        msg = string.Format(msg, item.FilialeName, totalMoney > 0 ? "收" : "付", Math.Abs(totalMoney).ToString("f2"),
            //            Math.Abs(serverMoney).ToString("f2"), Math.Abs(exceptionMoney).ToString("f2"));
            //        receipt.HasInvoice = item.IsOut;
            //        receipt.SettleStartDate = Convert.ToDateTime("1999-09-09");
            //        receipt.SettleEndDate = Convert.ToDateTime("1999-09-09");
            //        receipt.ExpectBalance = Math.Abs(totalMoney);
            //        receipt.RealityBalance = Math.Abs(totalMoney);
            //        receipt.DiscountMoney = 0;
            //        receipt.DiscountCaption = string.Empty;
            //        receipt.OtherDiscountCaption = "[快递对账生成的收付款]";
            //        receipt.FilialeId = item.FilialeId;
            //        receipt.ReceiptStatus = totalMoney < 0
            //            ? Convert.ToInt32(CompanyFundReceiptState.WaitAuditing)
            //            : Convert.ToInt32(CompanyFundReceiptState.WaitInvoice);
            //        receipt.StockOrderNos = string.Empty;
            //        receipt.Remark = WebControl.RetrunUserAndTime(msg);
            //        receipt.IsOut = item.IsOut;
            //        receiptList.Add(receipt);
            //    }

            //    if (elseExceptionReckoningInfoList.Count > 0)
            //    {
            //        decimal totalMoney = 0;
            //        decimal serverMoney = 0;
            //        decimal exceptionMoney;
            //        var sumFinanceConfirmMoney = elseExceptionReckoningInfoList.Sum(ent => ent.SumFinanceConfirmMoney);
            //        var sumConfirmMoney = elseExceptionReckoningInfoList.Sum(ent => ent.SumConfirmMoney);
            //        if (dictionary[key] == (int)ReckoningCheckType.Carriage)
            //        {
            //            totalMoney -= sumFinanceConfirmMoney;
            //            serverMoney -= sumConfirmMoney;
            //            exceptionMoney = sumConfirmMoney - sumFinanceConfirmMoney;
            //        }
            //        else
            //        {
            //            totalMoney += sumFinanceConfirmMoney;
            //            serverMoney += sumConfirmMoney;
            //            exceptionMoney = sumFinanceConfirmMoney - sumConfirmMoney;
            //        }
            //        if (totalMoney == 0) continue;
            //        //生成往来单位收付款
            //        var receipt = new CompanyFundReceiptInfo
            //        {
            //            ReceiptNo = codeManager.GetCode(totalMoney > 0 ? CodeType.GT : CodeType.PY),
            //            ReceiptType = Convert.ToInt32(totalMoney > 0 ? CompanyFundReceiptType.Receive : CompanyFundReceiptType.Payment),
            //            ApplyDateTime = DateTime.Now,
            //            ApplicantID = personnelInfo.PersonnelId,
            //            PurchaseOrderNo = string.Empty,
            //            CompanyID = companyList[0]
            //        };
            //        string msg = "{0} 应{1}总额：{2},我方总额：{3}，异常总额：{4}";  //确认金额，系统金额，异常金额
            //        msg = string.Format(msg, "其他公司", totalMoney > 0 ? "收" : "付", Math.Abs(totalMoney).ToString("f2"), Math.Abs(serverMoney).ToString("f2"), Math.Abs(exceptionMoney).ToString("f2"));
            //        receipt.HasInvoice = false;
            //        receipt.SettleStartDate = Convert.ToDateTime("1999-09-09");
            //        receipt.SettleEndDate = Convert.ToDateTime("1999-09-09");
            //        receipt.ExpectBalance = Math.Abs(totalMoney);
            //        receipt.RealityBalance = Math.Abs(totalMoney);
            //        receipt.DiscountMoney = 0;
            //        receipt.DiscountCaption = string.Empty;
            //        receipt.OtherDiscountCaption = "[快递对账生成的收付款]";
            //        receipt.FilialeId = _reckoningElseFilialeid;
            //        receipt.ReceiptStatus = totalMoney < 0
            //            ? Convert.ToInt32(CompanyFundReceiptState.WaitAuditing)
            //            : Convert.ToInt32(CompanyFundReceiptState.Audited);
            //        receipt.StockOrderNos = string.Empty;
            //        receipt.Remark = WebControl.RetrunUserAndTime(msg);
            //        receipt.IsOut = false;
            //        receiptList.Add(receipt);
            //    }

            //}
            #endregion

            #region  添加往来账，修改往来账
            try
            {
                using (var scop = new TransactionScope())
                {
                    if (receiptList.Count > 0)
                    {
                        ICompanyFundReceipt companyFundReceipt=new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
                        foreach (var item in receiptList)
                        {
                            bool isInsert = companyFundReceipt.Insert(item);
                            var info = companyFundReceipt.GetFundReceiptInfoByReceiptNo(item.ReceiptNo);
                            if (isInsert && info.ReceiptID != Guid.Empty)
                            {
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName,
                                    info.ReceiptID, info.ReceiptNo, item.RealityBalance > 0
                                        ? OperationPoint.CurrentReceivedPayment.FillBill.GetBusinessInfo()
                                        : OperationPoint.CurrentReceivedPayment.FillPayment.GetBusinessInfo(), string.Empty);
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, info.ReceiptID, info.ReceiptNo, OperationPoint.CurrentReceivedPayment.Remarks.GetBusinessInfo(), string.Empty);
                            }
                        }
                    }
                    var mediumReckoning = MediumReckoning.WriteInstance;
                    foreach (var key in dictionary.Keys)
                    {
                        _checkDataManagerWrite.UpdateState(key, (int)CheckDataState.Checked);
                        mediumReckoning.DeleteTempReckoning(key);
                    }
                    scop.Complete();
                }
                _submitController.Submit();
            }
            catch (Exception ex)
            {
                RAM.Alert("往来账处理异常：" + ex.Message);
                return;
            }
            //if (!string.IsNullOrWhiteSpace(receiptNo))
            //{
            //    RAM.ResponseScripts.Add(string.Format("return ShowCheckHistoryForm('{0}')", receiptNo));
            //}
            //else
            //{
            RAM.Alert("往来账收付款生成完毕!");
            // }
            #endregion
            rgCheckHistory.Rebind();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnDeleteClick(object sender, EventArgs e)
        {
            if (rgCheckHistory.SelectedItems.Count == 0)
            {
                RAM.Alert("请选择待删除帐!");
                return;
            }
            var deletes = new Dictionary<Guid, CheckDataRecordInfo>();

            foreach (GridDataItem item in rgCheckHistory.SelectedItems)
            {
                var checkId = new Guid(item.GetDataKeyValue("CheckId").ToString());
                var info = _checkDataManagerWrite.GetCheckDataInfoById(checkId);
                if (info != null && info.CheckDataState < (int)CheckDataState.Confirmed)
                {
                    deletes.Add(checkId, info);
                }
            }
            if (deletes.Count < rgCheckHistory.SelectedItems.Count)
            {
                RAM.Alert("选择项中存在确认后对账记录/记录不存在!");
                return;
            }
            foreach (var delete in deletes.Keys)
            {
                _checkDataManagerWrite.UpdateState(delete, (int)CheckDataState.Deleted);
            }
            rgCheckHistory.Rebind();
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(rgCheckHistory, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="state"></param>
        /// <param name="type">1，对比，2，确认，3，完成</param>
        /// <returns></returns>
        public bool IsVisible(object path, object state, int type)
        {
            if (path == null) return false;
            string filePath = path.ToString();
            if (string.IsNullOrEmpty(filePath)) return false;
            int checkDataState = Convert.ToInt32(state);
            switch (type)
            {
                case 1:
                    return checkDataState >= (int)CheckDataState.Contrasted;
                case 3:
                    return checkDataState >= (int)CheckDataState.Finished;
                default:
                    return true;
            }
        }

        /// <summary>
        /// 获取确认后状态
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetStateString(object obj)
        {
            int checkDataState = Convert.ToInt32(obj);
            if (checkDataState >= (int)CheckDataState.Inhanding)
            {
                var enumList = EnumAttribute.GetDict<CheckDataState>();
                return enumList[checkDataState];
            }
            return "处理中";
        }

        /// <summary>
        /// 提示对账记录是否已经对账
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string IsCheckedString(object obj)
        {
            int checkDataState = Convert.ToInt32(obj);
            if (checkDataState == 8)
                return "(已对账)";
            return " ";
        }

        /// <summary>
        /// 显示实际对账金额(财务确认金额)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        protected string ShowAccountReceivable(object id, object state)
        {
            int checkDataState = Convert.ToInt32(state);
            if (checkDataState < (int)CheckDataState.Finished)
                return "-";
            var list = _checkDataManagerWrite.GetTotalMoney(new Guid(id.ToString()));
            return WebControl.NumberSeparator(list.Sum(ent => ent.SumFinanceConfirmMoney));
        }

        /// <summary>
        /// 是否已经上传，标识财务是否上传了确认文档
        /// </summary>
        /// <param name="filePath"> </param>
        /// <param name="state"></param>
        /// <returns></returns>
        protected string ShowIsUploadMsg(object filePath, object state)
        {
            int checkDataState = Convert.ToInt32(state);
            if (checkDataState == (int)CheckDataState.Contrasted && !string.IsNullOrEmpty(filePath.ToString()))
            {
                return "(已上传)";
            }
            return "";
        }
    }
}

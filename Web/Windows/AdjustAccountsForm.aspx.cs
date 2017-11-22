using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using AllianceShop.Contract.DataTransferObject;
using AllianceShop.Enum;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using OperationLog.Core;
using OperationLog.Core.Attributes;
using Telerik.Web.UI;
using AuditingState = ERP.Enum.AuditingState;
using CodeType = ERP.Enum.CodeType;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class AdjustAccountsForm : WindowsPage
    {
        readonly CodeManager _code = new CodeManager();
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private readonly IWasteBook _wasteBook=new WasteBook(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccounts _bankAccounts=new BankAccounts(GlobalConfig.DB.FromType.Read);
        private readonly IBankAccountDao _bankAccountDao=new BankAccountDao(GlobalConfig.DB.FromType.Read);
        protected string ReqWasteBookType
        {
            get
            {
                if (ViewState["ReqWasteBookType"] == null)
                {
                    if (string.IsNullOrEmpty(Request.QueryString["WasteBookType"]))
                    {
                        ViewState["ReqWasteBookType"] = string.Empty;
                    }
                    else
                    {
                        ViewState["ReqWasteBookType"] = Request.QueryString["WasteBookType"];
                    }
                }
                return ViewState["ReqWasteBookType"].ToString();
            }
        }

        protected Guid ReqWasteBookId
        {
            get
            {
                if (ViewState["ReqWasteBookId"] == null)
                {
                    if (string.IsNullOrEmpty(Request.QueryString["WasteBookId"]))
                    {
                        ViewState["ReqWasteBookId"] = Guid.Empty.ToString();
                    }
                    else
                    {
                        ViewState["ReqWasteBookId"] = Request.QueryString["WasteBookId"];
                    }
                }
                return new Guid(ViewState["ReqWasteBookId"].ToString());
            }
        }

        protected Guid ReqFilialeId
        {
            get
            {
                if (ViewState["ReqFilialeId"] == null)
                {
                    if (string.IsNullOrEmpty(Request.QueryString["FilialeId"]))
                    {
                        ViewState["ReqFilialeId"] = Guid.Empty.ToString();
                    }
                    else
                    {
                        ViewState["ReqFilialeId"] = Request.QueryString["FilialeId"];
                    }
                }
                return new Guid(ViewState["ReqFilialeId"].ToString());
            }
        }

        protected Guid ReqBankAccountsId
        {
            get
            {
                if (ViewState["ReqBankAccountsId"] == null)
                {
                    if (string.IsNullOrEmpty(Request.QueryString["BankAccountsId"]))
                    {
                        ViewState["ReqBankAccountsId"] = Guid.Empty.ToString();
                    }
                    else
                    {
                        ViewState["ReqBankAccountsId"] = Request.QueryString["BankAccountsId"];
                    }
                }
                return new Guid(ViewState["ReqBankAccountsId"].ToString());
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //绑定公司
                var headList = CacheCollection.Filiale.GetHostingAndSaleFilialeList();
                headList.Add(new FilialeInfo
                {
                    ID = _reckoningElseFilialeid,
                    Name = "ERP"
                });
                RCB_FilialeList.DataSource = headList;
                RCB_FilialeList.DataValueField = "ID";
                RCB_FilialeList.DataTextField = "Name";
                RCB_FilialeList.DataBind();
                RCB_FilialeList.Items.Add(new RadComboBoxItem("", Guid.Empty.ToString()));

                //绑定银行
                BindddlBank(ReqFilialeId);

                if (ReqWasteBookId == Guid.Empty)
                {
                    #region [新增]
                    //判断ReqFilialeId是公司还销售平台
                    var isExist = headList.FirstOrDefault(ent => ent.ID == ReqFilialeId);
                    if (isExist != null)
                    {
                        RCB_FilialeList.SelectedValue = ReqFilialeId.ToString();
                    }
                    else
                    {
                        if (ReqFilialeId == Guid.Empty)
                        {
                            RAM.Alert("不是有效的公司信息，请选择账户");
                            RAM.ResponseScripts.Add("CancelWindow();");
                            return;
                        }
                        var parentId = CacheCollection.SalePlatform.Get(ReqFilialeId).FilialeId;
                        RCB_FilialeList.SelectedValue = parentId.ToString();
                    }
                    if (ReqBankAccountsId != Guid.Empty)
                    {
                        ddlBank.SelectedValue = ReqBankAccountsId.ToString();
                    }
                    if (ReqWasteBookType == WasteBookType.Increase.ToString())
                    {
                        Page.Title = "新增-增加资金";
                        TB_Description.Text = "";//TB_Description.Text = " [增加资金]";
                        LB_Adjust.Visible = GetPowerOperationPoint("Increasing");
                        Auditing.Visible = false;// GetPowerOperationPoint("IncreaseAuditing");
                        LB_Delete.Visible = false;// GetPowerOperationPoint("Delete");
                    }
                    else
                    {
                        Page.Title = "新增-减少资金";
                        TB_Description.Text = ""; //TB_Description.Text = " [减少资金]";
                        LB_Adjust.Visible = GetPowerOperationPoint("Subtract");
                        Auditing.Visible = false;// GetPowerOperationPoint("SubtractAuditing");
                        LB_Delete.Visible = false;// GetPowerOperationPoint("Delete");
                    }
                    #endregion
                }
                else
                {
                    #region [修改]

                    WasteBookInfo wasteBookInfo = _wasteBook.GetWasteBook(ReqWasteBookId);
                    if (wasteBookInfo!=null)
                    {
                        if (!string.IsNullOrEmpty(RCB_FilialeList.SelectedValue))
                        {
                            RCB_FilialeList.SelectedValue = wasteBookInfo.SaleFilialeId.ToString();
                        }
                        if (!string.IsNullOrEmpty(ddlBank.SelectedValue))
                        {
                            ddlBank.SelectedValue = wasteBookInfo.BankAccountsId.ToString();
                        }
                        TB_Income.Text = Math.Abs(wasteBookInfo.Income).ToString("0.##");
                        TB_Description.Text = string.Empty;
                        if (wasteBookInfo.WasteBookType == (int)WasteBookType.Increase)
                        {
                            Page.Title = "增加资金";
                            LB_Adjust.Visible = GetPowerOperationPoint("Increasing");
                            Auditing.Visible = GetPowerOperationPoint("IncreaseAuditing");
                            LB_Delete.Visible = GetPowerOperationPoint("Delete");
                        }
                        else
                        {
                            Page.Title = "减少资金";
                            LB_Adjust.Visible = GetPowerOperationPoint("Subtract");
                            Auditing.Visible = GetPowerOperationPoint("SubtractAuditing");
                            LB_Delete.Visible = GetPowerOperationPoint("Delete");
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "AccountsT.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }

        //绑定银行 
        protected void BindddlBank(Guid filialeId)
        {
            ddlBank.Items.Clear();
            var personnel = CurrentSession.Personnel.Get();
            IList<BankAccountInfo> banklist = _bankAccounts.GetBankAccountsList(personnel.FilialeId, personnel.BranchId, personnel.PositionId).ToList();
            if (ReqWasteBookId != Guid.Empty)
            {
                var info = banklist.FirstOrDefault(ent => ent.BankAccountsId == ReqBankAccountsId);
                if (info != null)
                {
                    ddlBank.Items.Insert(0, new ListItem(info.BankName + " - " + info.AccountsName, info.BankAccountsId.ToString()));
                    var bankAccountInfo = _bankAccountDao.GetListByBankAccountId(info.BankAccountsId).FirstOrDefault(ent => ent.BankAccountsId == info.BankAccountsId);
                    var tragetId = Guid.Empty;
                    if (bankAccountInfo != null) tragetId = bankAccountInfo.TargetId;
                    if (tragetId == Guid.Empty && CacheCollection.Filiale.Get(filialeId) != null)
                    {
                        tragetId = _reckoningElseFilialeid;
                    }
                    RCB_FilialeList.SelectedValue = tragetId.ToString();
                }
            }
            else
            {
                for (var i = 0; i < banklist.Count; i++)
                {
                    ddlBank.Items.Insert(i, new ListItem(banklist[i].BankName + " - " + banklist[i].AccountsName, banklist[i].BankAccountsId.ToString()));
                }
            }
        }

        //按钮：确定
        protected void AdjustIncome_Click(object sender, EventArgs e)
        {
            var strFilialeId = RCB_FilialeList.SelectedValue;
            var strBankAccountsId = ddlBank.SelectedValue;
            var filialeId = string.IsNullOrEmpty(strFilialeId) ? Guid.Empty : new Guid(strFilialeId);
            var bankAccountsId = string.IsNullOrEmpty(strBankAccountsId) ? Guid.Empty : new Guid(strBankAccountsId);
            var strIncome = TB_Income.Text;
            string tbDescription = !string.IsNullOrWhiteSpace(TB_Description.Text) ? TB_Description.Text.Trim() : "无";
            if (filialeId == Guid.Empty)
            {
                RAM.Alert("请选择公司");
                return;
            }
            if (bankAccountsId == Guid.Empty)
            {
                RAM.Alert("请选择银行");
                return;
            }
            if (string.IsNullOrEmpty(strIncome))
            {
                RAM.Alert("请输入资金");
                return;
            }
            var personnelInfo = CurrentSession.Personnel.Get();
            if (ReqWasteBookId == Guid.Empty)
            {
                #region [增加]
                string tradeCode;
                int wasteBookType;
                decimal income = Convert.ToDecimal(strIncome);
                EnumPointAttribute enumPointAttribute;
                string description;
                if (ReqWasteBookType == WasteBookType.Decrease.ToString())
                {
                    //减少资金
                    enumPointAttribute = OperationPoint.FundFlow.AbatementFunds.GetBusinessInfo();
                    wasteBookType = (int)WasteBookType.Decrease;
                    tradeCode = _code.GetCode(CodeType.RD);
                    income = -income;
                    description = "减少资金";
                }
                else
                {
                    //增加资金
                    enumPointAttribute = OperationPoint.FundFlow.IncreaseFunds.GetBusinessInfo();
                    wasteBookType = (int)WasteBookType.Increase;
                    tradeCode = _code.GetCode(CodeType.GI);
                    description = "增加资金";
                }

                var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var des = string.Format("[{0}(申请人：{1}；申请备注：{2}）,{3}]", description, personnelInfo.RealName, tbDescription, dateTime);
                var wasteBookInfo = new WasteBookInfo(Guid.NewGuid(), bankAccountsId, tradeCode, des, income,
                                                      (int)AuditingState.No, wasteBookType, filialeId)
                                        {
                                            LinkTradeCode = tradeCode,
                                            LinkTradeType = (int)WasteBookLinkTradeType.Other,
                                            BankTradeCode = string.Empty,
                                            State = (int)WasteBookState.Currently,
                                            IsOut = false
                                        };
                try
                {
                    if (wasteBookInfo.Income!=0)
                    {
                        _wasteBook.Insert(wasteBookInfo);
                        //资金流增加减少资金操作记录添加
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, wasteBookInfo.WasteBookId, wasteBookInfo.TradeCode,
                            enumPointAttribute, string.Empty);

                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch (Exception ex)
                {
                    RAM.Alert("操作异常！\n\n消息提示：" + ex.Message);
                }

                #endregion
            }
            else
            {
                #region [修改]
                var wasteBookInfo = _wasteBook.GetWasteBook(ReqWasteBookId);
                var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var udes = string.Format("[修改资金流（修改人：{0}；修改备注：{1}）,{2}）]", personnelInfo.RealName, tbDescription, dateTime);
                wasteBookInfo.Income = wasteBookInfo.WasteBookType == (int)WasteBookType.Decrease ? -Convert.ToDecimal(strIncome) : Convert.ToDecimal(strIncome);
                wasteBookInfo.SaleFilialeId = filialeId;
                wasteBookInfo.BankAccountsId = bankAccountsId;
                wasteBookInfo.Description = udes;
                try
                {
                    _wasteBook.Update(wasteBookInfo);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                catch (Exception ex)
                {
                    RAM.Alert("操作失败！\n\n消息提示：" + ex.Message);
                }
                #endregion
            }
        }

        //按钮：审核
        protected void Button_Auditing(object sender, EventArgs e)
        {
            try
            {
                //针对联盟店结算资金流审核，公司为其他公司
                //modify by liangcanren  at 2015-03-23
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (ReqWasteBookId != Guid.Empty)
                    {
                        var personnelInfo = CurrentSession.Personnel.Get();
                        string tbDescription = !string.IsNullOrWhiteSpace(TB_Description.Text) ? TB_Description.Text.Trim() : "无";
                        var wasteBookInfo = _wasteBook.GetWasteBook(ReqWasteBookId);
                        string typestr = wasteBookInfo.WasteBookType == (int)WasteBookType.Decrease ? "减少资金审批" : "增加资金审批";
                        var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        var des = string.Format("[{0}（审核人：{1}；审核备注：{2}）,{3}]）", typestr, personnelInfo.RealName, tbDescription, dateTime);
                        _wasteBook.Auditing(wasteBookInfo.TradeCode);
                        _wasteBook.UpdateDescription(wasteBookInfo.WasteBookId, des);

                        #region  add by liangcanren 2015-01-08
                        //判断是否为联盟店
                        Guid parentId = GetShopParentId(wasteBookInfo.SaleFilialeId);
                        if (parentId != Guid.Empty)
                        {
                            var rechargeDto = new RechargeDTO
                            {
                                AccountTotalled = 0,
                                BankAccountID = wasteBookInfo.BankAccountsId,
                                BankAccountName = string.Empty,
                                BankTradeNo = wasteBookInfo.TradeCode,
                                CreateTime = DateTime.Now,
                                Money = wasteBookInfo.Income,
                                No = string.Empty,
                                PurchaseID = Guid.Empty,
                                RechargeID = Guid.NewGuid(),
                                Remark = "[门店结算自动充值]",
                                ShopID = wasteBookInfo.SaleFilialeId,
                                ShopName = CacheCollection.Filiale.Get(wasteBookInfo.SaleFilialeId).Name,
                                State = (int)RechargeState.Paid,
                                Type = (int)RechargeType.Offline
                            };
                            var result = ShopSao.AuditingSettlement(parentId, rechargeDto);
                            //add by luwei 对应bug11365 根据新需求（ERP往来帐中无需生成跟门店的往来帐数据）
                            //往来帐 add by lcj at 2015.9.23
                            //Start
                            //var reckNo = _codeBll.GetCode(CodeType.GT);
                            //var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), parentId, wasteBookInfo.SaleFilialeId, reckNo,
                            //    "[门店结算自动充值]",
                            //    wasteBookInfo.Income,
                            //    (int)ReckoningType.Defray,
                            //    (int)ReckoningStateType.Currently,
                            //    (int)CheckType.NotCheck, (int)AuditingState.Yes,
                            //    wasteBookInfo.TradeCode, Guid.Empty)
                            //{
                            //    LinkTradeType = (int)ReckoningLinkTradeType.Recharge
                            //};
                            //End
                            //End by bug11365
                            if (!result)
                            {
                                RAM.Alert("系统提示：资金流审核失败，门店接口异常！");
                                return;
                            }
                            //add by luwei 对应bug11365 根据新需求（ERP往来帐中无需生成跟门店的往来帐数据）
                            //_reckonBll.Insert(reckoningInfo);//添加ERP本地往来帐 add by lcj at 2015.9.23 对应bug11031
                            //End by bug11365
                        }
                        #endregion
                        //资金流审核增加操作记录
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, wasteBookInfo.WasteBookId, wasteBookInfo.TradeCode,
                            OperationPoint.FundFlow.Auditing.GetBusinessInfo(), string.Empty);
                        ts.Complete();
                    }
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception exp)
            {
                RAM.Alert("审核失败！ \n\n消息提示：'" + exp.Message);
            }
        }

        //按钮：删除
        protected void Button_Delete(object sender, EventArgs e)
        {
            try
            {
                if (ReqWasteBookId != Guid.Empty)
                {
                    var wasteBookInfo = _wasteBook.GetWasteBook(ReqWasteBookId);
                    _wasteBook.DeleteWasteBook(wasteBookInfo.TradeCode);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
            catch
            {
                RAM.Alert("删除失败！");
            }
        }

        /// <summary>
        /// 获取联盟店
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        protected Guid GetShopParentId(Guid shopId)
        {
            var filialeInfo = CacheCollection.Filiale.Get(shopId);
            if (filialeInfo != null && filialeInfo.FilialeTypes.Contains((int)FilialeType.EntityShop))
            {
                return filialeInfo.ParentId;
            }
            return Guid.Empty;
        }
    }
}
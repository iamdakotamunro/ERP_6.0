using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI;
using AllianceShop.Contract.DataTransferObject;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class ShopCompanyFundReceiptForm : Page
    {
        readonly IReckoning _reckon = new Reckoning(GlobalConfig.DB.FromType.Write);
        readonly CodeManager _codeBll = new CodeManager();
        readonly IWasteBook _wastebook = new WasteBook(GlobalConfig.DB.FromType.Write);
        readonly ICompanyFundReceipt _companyFundReceipt=new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        readonly IBankAccounts _bankAccounts=new BankAccounts(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CompanyFundReceiptInfoModel = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
                var flag = CompanyFundReceiptInfoModel == null;
                if (flag)
                {
                    CompanyFundReceiptInfoModel=new CompanyFundReceiptInfo();
                }
                if (!flag)
                {
                    var shopInfo = ShopFilialeList.FirstOrDefault(act => act.ID == CompanyFundReceiptInfoModel.CompanyID);
                    var parent =shopInfo!=null?shopInfo.ParentId:Guid.Empty;
                    if (parent == Guid.Empty)
                    {
                        RAM.Alert("店铺信息未找到!");
                        return;
                    }
                    CompanyFundReceiptDto =
                        ShopSao.GetCompanyFundReceiptEntityByOriginalTradeCode(parent,CompanyFundReceiptInfoModel.ReceiptNo);
                }
                BindShopList(CompanyFundReceiptInfoModel.CompanyID);
                BindShopAccounts(CompanyFundReceiptInfoModel.CompanyID);
                BindKeedeAccounts(CompanyFundReceiptInfoModel.PayBankAccountsId);
                RtbDescription.Text = CompanyFundReceiptInfoModel.OtherDiscountCaption;
                RtbPoudage.Text = CompanyFundReceiptInfoModel.Poundage+"";
                RtbDescription.Enabled = flag;
                RtbPoudage.Enabled = flag;
                RtbRealityBalance.Text = CompanyFundReceiptInfoModel.RealityBalance+"";
                RtbRealityBalance.Enabled = flag;
            }
        }

        #region[收付款单据ID]
        protected Guid RceiptId
        {
            get
            {
                if (Request.QueryString["RceiptId"] != null)
                {
                    if (MethodHelp.CheckGuid(Request.QueryString["RceiptId"]))

                        return new Guid(Request.QueryString["RceiptId"]);
                }
                return Guid.Empty;
            }
        }

        #endregion

        #region  门店公司信息列表

        protected IList<FilialeInfo> ShopFilialeList
        {
            get
            {
                if (ViewState["ShopFilialeList"] != null)
                {
                    return (IList<FilialeInfo>)ViewState["ShopFilialeList"];
                }
                return CacheCollection.Filiale.GetShopList();
            }
            set
            {
                ViewState["ShopFilialeList"] = value;
            }
        }
        #endregion

        #region  门店往来单位收付款

        public CompanyFundReceiptDTO CompanyFundReceiptDto
        {
            get
            {
                if (ViewState["CompanyFundReceiptDto"] != null)
                {
                    return (CompanyFundReceiptDTO)ViewState["CompanyFundReceiptDto"];
                }
                return null;
            }
            set { ViewState["CompanyFundReceiptDto"] = value; }
        }
        #endregion

        #region  往来单位收付款

        public CompanyFundReceiptInfo CompanyFundReceiptInfoModel
        {
            get
            {
                if (ViewState["CompanyFundReceiptInfoModel"] != null)
                {
                    return (CompanyFundReceiptInfo)ViewState["CompanyFundReceiptInfoModel"];
                }
                return null;
            }
            set { ViewState["CompanyFundReceiptInfoModel"] = value; }
        }
        #endregion

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnCanceClick(object sender, EventArgs e)
        {
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAuditClick(object sender, EventArgs e)
        {
            if (CompanyFundReceiptDto==null)
            {
                RAM.Alert("门店没有对应的往来单位收付款单据!");
                return;
            }
            var info = _companyFundReceipt.GetCompanyFundReceiptInfo(RceiptId);
            if (info == null)
            {
                RAM.Alert("对应的往来单位收付款单据不存在!");
                return;
            }
            if (info.ReceiptStatus >= (int)CompanyFundReceiptState.Executed)
            {
                RAM.Alert("单据已处理!");
                return;
            }
            var flag = info.ReceiptType == (int) CompanyFundReceiptType.Receive; //收款还是付款
            string reckNo = _codeBll.GetCode(flag?CodeType.GT:CodeType.PY);
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    if (string.IsNullOrEmpty(info.DealFlowNo))
                    {
                        if (string.IsNullOrEmpty(RtbDealNo.Text))
                        {
                            RAM.Alert("交易流水号不能为空!");
                            return;
                        }
                        _companyFundReceipt.UpdateDealFlowNo(info.ReceiptID, RtbDealNo.Text);
                    }
                    else
                    {
                        if (info.DealFlowNo != RtbDealNo.Text)
                            _companyFundReceipt.UpdateDealFlowNo(info.ReceiptID, RtbDealNo.Text);
                    }
                    string remark = WebControl.RetrunUserAndTime("执行");
                    _companyFundReceipt.UpdateFundReceiptRemark(RceiptId, remark);
                    _companyFundReceipt.UpdateFundReceiptState(RceiptId, CompanyFundReceiptState.Executed);
                    _companyFundReceipt.SetDateTime(RceiptId, 2);


                    var rRDesc = string.Format("[{0}款银行:{1}][{2}款人:{3}]" + "[交易流水号：{4}]",
                        flag ? "收" : "付", CompanyFundReceiptDto.CompanyBankName, flag ? "付" : "收", CompanyFundReceiptDto.ShopName, RtbDealNo.Text);
                    var receiveReckoningDesc = WebControl.RetrunUserAndTime(string.Format("[联盟店总管理对{0}{1}款,详细见差额说明]{2}",
                        CompanyFundReceiptDto.ShopName,flag?"收":"付",rRDesc));
                    var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var wdes = string.Format("[往来{0}款（{1}款单位：{2}；交易流水号：{3}），资金{4}，{5}）]", flag?"收":"付",flag?"付":"收",
                        CompanyFundReceiptDto.ShopName, RtbDealNo.Text, flag ? "增加" : "减少", dateTime);
                    //往来帐
                    var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), info.FilialeId, info.CompanyID, reckNo,
                                                          receiveReckoningDesc,
                                                          flag?-info.RealityBalance:info.RealityBalance, flag?(int)ReckoningType.Defray:(int)ReckoningType.Income,
                                                          (int)ReckoningStateType.Currently,
                                                          (int)CheckType.NotCheck, (int)AuditingState.Yes,
                                                          info.ReceiptNo, Guid.Empty)
                    {
                        LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt,
                        IsOut = info.IsOut
                    };
                    //资金流
                    var wasteBookinfo = new WasteBookInfo(Guid.NewGuid(), info.PayBankAccountsId, info.ReceiptNo,
                                                          wdes,
                                                          flag?info.RealityBalance:-info.RealityBalance, (Int32)AuditingState.Yes,
                                                          flag?(int)WasteBookType.Increase:(int)WasteBookType.Decrease, info.FilialeId)
                    {
                        LinkTradeCode = info.ReceiptNo,
                        LinkTradeType = (int)WasteBookLinkTradeType.CompanyFundReceipt,
                        BankTradeCode = string.Empty,
                        State = (int)WasteBookState.Currently,
                        IsOut = info.IsOut
                    };
                    string errorMsg;
                    _reckon.Insert(reckoningInfo,out errorMsg);
                    if(wasteBookinfo.Income!=0)
                        _wastebook.Insert(wasteBookinfo);
                    remark = WebControl.RetrunUserAndTime("完成");
                    _companyFundReceipt.UpdateFundReceiptRemark(RceiptId, remark);
                    _companyFundReceipt.UpdateFundReceiptState(RceiptId, CompanyFundReceiptState.Finish);

                    _companyFundReceipt.SetDateTime(RceiptId, 3);
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    //平掉门店与联盟总店的帐
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    RAM.Alert("审核失败!" + ex.Message);
                }
            }
        }

        #region  数据绑定

        /// <summary>
        /// 绑定往来店铺
        /// </summary>
        protected void BindShopList(Guid shopId)
        {
            var data = ShopFilialeList.ToDictionary(k=>k.ID,v=>v.Name);
            RcbShopList.DataSource = data;
            RcbShopList.DataTextField = "Value";
            RcbShopList.DataValueField = "Key";
            RcbShopList.DataBind();
            RcbShopList.Items.Add(new RadComboBoxItem("", Guid.Empty.ToString()));
            RcbShopList.SelectedValue = shopId.ToString();
            RcbShopList.Enabled = shopId == Guid.Empty;
        }

        /// <summary>
        /// 获取可得账户
        /// </summary>
        protected void BindKeedeAccounts(Guid bankAccountsId)
        {
            var bankAccountsList = _bankAccounts.GetBankAccountsByShopId()
                .ToDictionary(k => k.BankAccountsId, v => v.BankName);
            RcbKeedeAccounts.DataSource = bankAccountsList;
            RcbKeedeAccounts.DataTextField = "Value";
            RcbKeedeAccounts.DataValueField = "Key";
            RcbKeedeAccounts.DataBind();
            RcbKeedeAccounts.Items.Add(new RadComboBoxItem("", Guid.Empty.ToString()));
            RcbKeedeAccounts.SelectedValue = bankAccountsId.ToString();
            RcbKeedeAccounts.Enabled = bankAccountsId == Guid.Empty;
            if (bankAccountsId!=Guid.Empty)
            {
                RtbAccountsBalance.Text = _bankAccounts.GetBankAccountsNonce(bankAccountsId) + "";
            }
        }

        /// <summary>
        /// 获取店铺账户
        /// </summary>
        protected void BindShopAccounts(Guid shopId)
        {
            var shopInfo=ShopFilialeList.FirstOrDefault(act => act.ID == shopId);
            if(shopInfo==null)return;
            var shopAccountsList = ShopSao.GetBankBalanceList(shopInfo.ParentId,shopId).ToDictionary(k=>k.ID,v=>v.BankName);
            RcbShopAccounts.DataSource = shopAccountsList;
            RcbShopAccounts.DataTextField = "Value";
            RcbShopAccounts.DataValueField = "Key";
            RcbShopAccounts.DataBind();
            RcbShopAccounts.Items.Add(new RadComboBoxItem("", Guid.Empty.ToString()));
            RcbShopAccounts.SelectedValue =CompanyFundReceiptDto!=null?CompanyFundReceiptDto.BankAccountsID+"":Guid.Empty.ToString();
            RcbShopAccounts.Enabled = CompanyFundReceiptDto == null;
        }
        #endregion
        
        /// <summary>
        /// 店铺选择更改，往来余额和往来账户变化
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RcbShopListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (RcbShopList.SelectedItem == null || string.IsNullOrEmpty(RcbShopList.SelectedItem.Value)
                || RcbShopList.SelectedItem.Value==Guid.Empty.ToString())
            {
                return;
            }
            BindShopAccounts(new Guid(RcbShopList.SelectedItem.Value));
        }

        /// <summary>
        /// 可得账户余额
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RcbKeedeAccountsSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (RcbKeedeAccounts.SelectedItem == null || string.IsNullOrEmpty(RcbKeedeAccounts.SelectedItem.Value)
                || RcbKeedeAccounts.SelectedItem.Value == Guid.Empty.ToString())
            {
                return;
            }
            RtbAccountsBalance.Text = _bankAccounts.GetBankAccountsNonce(new Guid(RcbKeedeAccounts.SelectedItem.Value)) + "";
        }
    }
}
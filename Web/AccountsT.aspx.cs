using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary> 资金流
    /// </summary>
    public partial class AccountsT : BasePage
    {
        private readonly IWasteBook _wasteBookDao = new WasteBook(GlobalConfig.DB.FromType.Read);
        private readonly IWasteBookCheck _wasteBookCheck = new WasteBookCheck(GlobalConfig.DB.FromType.Read); 
        private readonly IBankAccounts _bankAccounts=new BankAccounts(GlobalConfig.DB.FromType.Read);
        private readonly IBankAccountDao _bankAccountsDao = new BankAccountDao(GlobalConfig.DB.FromType.Read);
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        #region[公用属性]
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

        protected DateTime EndDate
        {
            get
            {
                if (ViewState["EndDate"] == null || Convert.ToDateTime(ViewState["EndDate"]) == DateTime.MinValue)
                {
                    return DateTime.MinValue;
                }
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndDate"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            }
            set
            {
                ViewState["EndDate"] = value;
            }
        }

        // 单据类型
        //1 收入 0 支出
        private ReceiptType CurrentReceiptType
        {
            get
            {
                if (ViewState["ReceiptType"] == null)
                {
                    ViewState["ReceiptType"] = (int)ReceiptType.All;
                }

                return (ReceiptType)ViewState["ReceiptType"];
            }
            set
            {
                ViewState["ReceiptType"] = (int)value;
            }
        }

        //公司ID
        protected Guid FilialeId
        {
            get
            {
                if (ViewState["FilialeId"] == null) return Guid.Empty;
                return new Guid(ViewState["FilialeId"].ToString());
            }
            set
            {
                ViewState["FilialeId"] = value.ToString();
            }
        }

        private int IsCheck
        {
            get
            {
                if (ViewState["IsCheck"] == null)
                {
                    ViewState["IsCheck"] = -1;
                }

                return (int)ViewState["IsCheck"];
            }
            set
            {
                ViewState["IsCheck"] = value;
            }
        }

        private AuditingState CurrentAuditingState
        {
            get
            {
                if (ViewState["CurrentAuditingState"] == null)
                {
                    ViewState["CurrentAuditingState"] = (int)AuditingState.Yes;
                }

                return (AuditingState)ViewState["CurrentAuditingState"];
            }
            set
            {
                ViewState["CurrentAuditingState"] = (int)value;
            }
        }

        protected Guid BankAccountsId
        {
            get
            {
                if (ViewState["BankAccountsId"] == null) return Guid.Empty;
                return new Guid(ViewState["BankAccountsId"].ToString());
            }
            set
            {
                ViewState["BankAccountsId"] = value.ToString();
            }
        }

        /// <summary>
        /// 搜索资金范围之最小资金
        /// </summary>
        protected double MinIncome
        {
            get
            {
                if (ViewState["MinIncome"] == null) return Double.MinValue;
                return (double)ViewState["MinIncome"];
            }
            set
            {
                ViewState["MinIncome"] = value;
            }
        }

        /// <summary>
        /// 搜索资金范围之最大资金
        /// </summary>
        protected double MaxIncome
        {
            get
            {
                if (ViewState["MaxIncome"] == null) return Double.MaxValue;
                return (double)ViewState["MaxIncome"];
            }
            set
            {
                ViewState["MaxIncome"] = value;
            }
        }

        protected String TradeCode
        {
            get
            {
                if (ViewState["TradeCode"] == null) return String.Empty;
                return ViewState["TradeCode"].ToString();
            }
            set
            {
                ViewState["TradeCode"] = value;
            }
        }

        private IList<BankAccountBalanceInfo> BankAccountBalanceList
        {
            get
            {
                if (ViewState["BankAccountBalanceList"] == null) return new List<BankAccountBalanceInfo>();
                return (IList<BankAccountBalanceInfo>)ViewState["BankAccountBalanceList"];
            }
            set
            {
                ViewState["BankAccountBalanceList"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BankAccountBalanceList = _bankAccounts.GetBalanceList();
                GetBankAccounts();

                var yearlist = new List<int>();
                for (var i = 2007; i <= (DateTime.Now.Year - GlobalConfig.KeepYear); i++)
                {
                    yearlist.Add(i);
                }
                DDL_Years.DataSource = yearlist.OrderByDescending(y => y);
                DDL_Years.DataBind();
                DDL_Years.Items.Add(new ListItem(GlobalConfig.KeepYear + "年内数据", "0"));
                DDL_Years.SelectedValue = "0";
                YearsSelectedIndex();

                span_TotalMoney.InnerText = "总计金额:" + _bankAccounts.GetBankAccountsAllNonceBalance().ToString("N") + "元";
            }
        }

        #region 取得用户操作权限
        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "AccountsT.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);

        }
        #endregion

        #region[绑定树]
        //根目录资金账户
        private void GetBankAccounts()
        {
            RTVBankAccounts.Nodes.Clear();
            var filiales = CacheCollection.Filiale.GetHostingAndSaleFilialeList();
            foreach (var filialeInfo in filiales)
            {
                FilialeInfo info = filialeInfo;
                var tempSumMoney =BankAccountBalanceList.Where(ent => ent.TargetId == info.ID).Sum(ent => ent.NonceBalance);
                RadTreeNode rootNode = CreateNode(filialeInfo.Name + "[" + tempSumMoney.ToString("N") + "]", true, filialeInfo.ID.ToString(), "Filiale");
                RecursivelyBankAccounts(rootNode, filialeInfo.ID);
                rootNode.Selected = true;
                rootNode.PostBack = true;
                RTVBankAccounts.Nodes.Add(rootNode);
                RecursivelySalePlatform(rootNode, filialeInfo.ID);
            }
            var personnelInfo = CurrentSession.Personnel.Get();
            IList<BankAccountInfo> bankAccountsListByNotIsMain = _bankAccounts.GetBankAccountsListByNotIsMain(personnelInfo.FilialeId, personnelInfo.BranchId, personnelInfo.PositionId);
            RadTreeNode rootNodeElse = CreateNode("ERP[" + bankAccountsListByNotIsMain.Sum(ent => ent.NonceBalance).ToString("N") + "]", false, _reckoningElseFilialeid.ToString(), "Filiale");
            foreach (var info in bankAccountsListByNotIsMain)
            {
                var childNode = CreateNode((info.IsUse ? "" : "【停用】") + info.BankName + " - " + info.AccountsName + "[" + info.NonceBalance.ToString("N") + "]", false,
                                info.BankAccountsId.ToString(), "BankAccount");
                rootNodeElse.Nodes.Add(childNode);
            }
            RTVBankAccounts.Nodes.Add(rootNodeElse);
        }

        /// <summary>
        /// 遍历销售平台
        /// </summary>
        /// <param name="node"></param>
        /// <param name="filialeId"></param>
        private void RecursivelySalePlatform(RadTreeNode node, Guid filialeId)
        {
            foreach (var info in CacheCollection.SalePlatform.GetList().Where(f => f.FilialeId == filialeId))
            {
                var bankList = _bankAccountsDao.GetListByTargetId(info.ID);
                if (bankList!=null && bankList.Count>0)
                {
                    RadTreeNode childNode = CreateNode(info.Name, true, info.ID.ToString(), "SalePlatform");
                    if (RecursivelyBankAccounts(childNode, info.ID))
                        node.Nodes.Add(childNode);
                }
            }
        }

        //遍历资金帐户
        private bool RecursivelyBankAccounts(IRadTreeNodeContainer node, Guid targetId)
        {
            var flag = true;
            var personnelInfo = CurrentSession.Personnel.Get();
            if (personnelInfo != null)
            {
                var selectSalePlatformInfo = CacheCollection.SalePlatform.Get(targetId);
                IEnumerable<BankAccountInfo> infos = _bankAccountsDao.GetListByTargetId(targetId);
                if (infos != null && infos.Any())
                {
                    if (selectSalePlatformInfo == null)
                    {

                        IList<BankAccountInfo> bankAccountList =
                            _bankAccounts.GetBankAccountsList(personnelInfo.FilialeId, personnelInfo.BranchId, personnelInfo.PositionId)
                                                    .Where(b => infos.Any(ent => ent.BankAccountsId == b.BankAccountsId) && b.IsUse)
                                                    .OrderByDescending(b => b.IsUse)
                                                    .ToList();

                        var salePlatformList = CacheCollection.SalePlatform.GetListByFilialeId(targetId);
                        var bindingedBankAccountList = new List<BankAccountInfo>();
                        foreach (var salePlatformInfo in salePlatformList.Where(act=>act.IsActive))
                        {
                            bindingedBankAccountList.AddRange(_bankAccountsDao.GetListByTargetId(salePlatformInfo.ID));
                        }
                        var needBankAccountList = (from item in bankAccountList
                                                   where
                                                       bindingedBankAccountList.All(
                                                           ent => ent.BankAccountsId != item.BankAccountsId)
                                                   select new BankAccountInfo
                                                   {
                                                       BankAccountsId = item.BankAccountsId,
                                                       Accounts = item.Accounts,
                                                       AccountsKey = item.AccountsKey,
                                                       AccountsName = item.AccountsName,
                                                       BankIcon = item.BankIcon,
                                                       BankName = item.BankName,
                                                       Description = item.Description,
                                                       IsFinish = item.IsFinish,
                                                       IsUse = item.IsUse,
                                                       OrderIndex = item.OrderIndex,
                                                       PaymentInterfaceId = item.PaymentInterfaceId,
                                                       PaymentType = item.PaymentType,
                                                   }).ToList();
                        flag = needBankAccountList.Count > 0;
                        foreach (var bankAccountsInfo in needBankAccountList)
                        {
                            var childNode =
                                CreateNode(
                                    (bankAccountsInfo.IsUse ? "" : "【停用】") + bankAccountsInfo.BankName + " - " + bankAccountsInfo.AccountsName + "[" +
                                    GetBankAccountsCount(bankAccountsInfo.BankAccountsId).ToString("N") + "]", false,
                                    bankAccountsInfo.BankAccountsId.ToString(), "BankAccount");
                            node.Nodes.Add(childNode);
                        }
                    }
                    else
                    {
                        if (selectSalePlatformInfo.IsActive)
                        {
                            IList<BankAccountInfo> bankAccountsInfoList =
                            _bankAccounts.GetBankAccountsList(personnelInfo.FilialeId, personnelInfo.BranchId, personnelInfo.PositionId)
                                                    .Where(b => infos.Any(ent => ent.BankAccountsId == b.BankAccountsId) && b.IsUse)
                                                    .OrderByDescending(b => b.IsUse)
                                                    .ToList();
                            flag = bankAccountsInfoList.Count > 0;
                            foreach (var bankAccountsInfo in bankAccountsInfoList)
                            {
                                var childNode =
                                    CreateNode(
                                        (bankAccountsInfo.IsUse ? "" : "【停用】") + bankAccountsInfo.BankName + " - " + bankAccountsInfo.AccountsName + "[" +
                                        GetBankAccountsCount(bankAccountsInfo.BankAccountsId).ToString("N") + "]", false,
                                        bankAccountsInfo.BankAccountsId.ToString(), "BankAccount");
                                node.Nodes.Add(childNode);
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                }
            }
            return flag;
        }

        //创建节点
        private static RadTreeNode CreateNode(string text, bool expanded, string id, string category)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded, Category = category };
            return node;
        }
        #endregion

        #region[选择往来单位分类树节点]
        protected void RtvBankAccountsNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                BankAccountsId = new Guid(e.Node.Value);
                var headFilialeList = CacheCollection.Filiale.GetHeadList();
                foreach (var filialeInfo in headFilialeList)
                {
                    if (BankAccountsId != filialeInfo.ID)
                    {
                        FilialeId = Guid.Empty;
                    }
                    else
                    {
                        FilialeId = BankAccountsId;
                        BankAccountsId = Guid.Empty;
                        break;
                    }
                }
                var salePlatformList = CacheCollection.SalePlatform.GetList();
                if (FilialeId == Guid.Empty)
                {
                    foreach (var salePlatformInfo in salePlatformList)
                    {
                        if (BankAccountsId != salePlatformInfo.ID)
                        {
                            FilialeId = Guid.Empty;
                        }
                        else
                        {
                            FilialeId = BankAccountsId;
                            BankAccountsId = Guid.Empty;
                            break;
                        }
                    }
                }
                CurrentReceiptType = ReceiptType.All;
                StartDate = RDP_StartDate.SelectedDate ?? DateTime.MinValue;
                EndDate = RDP_EndDate.SelectedDate ?? DateTime.MinValue;
                RCB_ReceiptType.SelectedValue = "-1";
                txtTradeCode.Text = string.Empty;
                txtMinIncome.Text = string.Empty;
                txtMaxIncome.Text = string.Empty;
                TradeCode = String.Empty;
                RadGridWasteBook.CurrentPageIndex = 0;
                RadGridWasteBook.Rebind();
            }
        }
        #endregion

        protected void RadGridWasteBook_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<WasteBookInfo> wasteBookList = new List<WasteBookInfo>();
            long recordCount = 0;
            if (IsPostBack)
            {
                PersonnelInfo info = CurrentSession.Personnel.Get();
                var startPage = RadGridWasteBook.CurrentPageIndex + 1;
                int pageSize = RadGridWasteBook.PageSize;
                if (FilialeId != Guid.Empty)
                {
                    wasteBookList = _wasteBookDao.GetWasteBookListBySaleFilialeIdToPage(FilialeId, StartDate, EndDate, CurrentReceiptType, CurrentAuditingState, MinIncome, MaxIncome, TradeCode, info.FilialeId, info.BranchId, info.PositionId, GlobalConfig.KeepYear, IsCheck, startPage, pageSize, out recordCount).ToList();
                    if (BankAccountsId != Guid.Empty && wasteBookList.Count==0)
                    {
                        wasteBookList = wasteBookList.Where(act => act.BankAccountsId == BankAccountsId).ToList();
                    }
                }
                else
                {
                    if (BankAccountsId != Guid.Empty)
                    {
                        wasteBookList = _wasteBookDao.GetWasteBookListToPage(BankAccountsId, StartDate, EndDate, CurrentReceiptType,
                                                           CurrentAuditingState, MinIncome, MaxIncome, TradeCode,
                                                           info.FilialeId, info.BranchId, info.PositionId,
                                                           GlobalConfig.KeepYear, IsCheck,
                                                           startPage, pageSize, out recordCount).ToList();
                    }
                }
            }

            RadGridWasteBook.DataSource = wasteBookList;
            RadGridWasteBook.VirtualItemCount = (int)recordCount;
        }

        protected decimal GetBankAccountsCount(Guid bankAccountsId)
        {
            var balanceInfo = BankAccountBalanceList.FirstOrDefault(ent => ent.BankAccountId == bankAccountsId);
            if (balanceInfo != null)
            {
                return balanceInfo.NonceBalance;
            }
            return 0;
        }

        /// <summary>
        /// Excel导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LbxlsClick(object sender, EventArgs e)
        {
            string fileName = RTVBankAccounts.SelectedNode == null
                                  ? "没有选择资金账户"
                                  : new Guid(RTVBankAccounts.SelectedNode.Value) == Guid.Empty
                                        ? "没有选择资金账户"
                                        : RTVBankAccounts.SelectedNode.Text;
            fileName = Server.HtmlEncode(fileName);
            RadGridWasteBook.ExportSettings.ExportOnlyData = true;
            RadGridWasteBook.ExportSettings.IgnorePaging = true;
            RadGridWasteBook.ExportSettings.FileName = fileName;
            RadGridWasteBook.MasterTableView.ExportToExcel();
        }

        #region[搜索]
        // updata by liucaijun at 2010.07.22
        protected void LbSearchClick(object sender, EventArgs e)
        {
            StartDate = RDP_StartDate.SelectedDate == null ? DateTime.MinValue : RDP_StartDate.SelectedDate.Value;
            EndDate = RDP_EndDate.SelectedDate == null ? DateTime.MinValue : RDP_EndDate.SelectedDate.Value;
            CurrentReceiptType = (ReceiptType)Convert.ToInt32(RCB_ReceiptType.SelectedValue);
            MinIncome = String.IsNullOrEmpty(txtMinIncome.Text) ? double.MinValue : Convert.ToDouble(txtMinIncome.Text.Trim());
            MaxIncome = String.IsNullOrEmpty(txtMaxIncome.Text) ? double.MaxValue : Convert.ToDouble(txtMaxIncome.Text.Trim());
            TradeCode = txtTradeCode.Text;
            IsCheck = int.Parse(RCB_IsCheck.SelectedValue);
            var selecteValue = RTVBankAccounts.SelectedValue;
            if (selecteValue == string.Empty)
            {
                RAM.Alert("请选择左边公司或资金账户！");
                return;
            }
            RadGridWasteBook.CurrentPageIndex = 0;
            RadGridWasteBook.Rebind();
        }
        #endregion

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RadGridWasteBook, e);
        }

        #region[未审核单据]
        //updata by liucaijun at 2010.07.22
        protected void LbUnverifyClick(object sender, EventArgs e)
        {
            if (lbUnverify.Text == ">>未审核单据")
            {
                lbUnverify.Text = ">>已审核单据";
                CurrentAuditingState = AuditingState.No;
            }
            else
            {
                lbUnverify.Text = ">>未审核单据";
                CurrentAuditingState = AuditingState.Yes;
            }

            //如果不是叶子节点，则搜索全部分支
            var selectedBankAccountsId = new Guid(RTVBankAccounts.SelectedValue);
            if (BankAccountsId != selectedBankAccountsId)
            {
                BankAccountsId = Guid.Empty;
            }
            RadGridWasteBook.Rebind();
        }
        #endregion

        protected void DdlYearsSelectedIndexChanged(object sender, EventArgs e)
        {
            YearsSelectedIndex();
        }

        private void YearsSelectedIndex()
        {
            if (DDL_Years.SelectedValue == "0")
            {
                RDP_StartDate.MinDate = DateTime.MinValue;
                RDP_StartDate.MaxDate = DateTime.MaxValue;
                RDP_EndDate.MinDate = DateTime.MinValue;
                RDP_EndDate.MaxDate = DateTime.MaxValue;

                RDP_StartDate.SelectedDate = DateTime.Now.AddDays(-30);
                RDP_EndDate.SelectedDate = DateTime.Now.AddDays(1);

                RDP_StartDate.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                RDP_StartDate.MaxDate = DateTime.Now;
                RDP_EndDate.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                RDP_EndDate.MaxDate = DateTime.Now;

                StartDate = RDP_StartDate.SelectedDate.Value;
                EndDate = RDP_EndDate.SelectedDate.Value;
            }
            else
            {
                RDP_StartDate.MinDate = DateTime.MinValue;
                RDP_StartDate.MaxDate = DateTime.MaxValue;
                RDP_EndDate.MinDate = DateTime.MinValue;
                RDP_EndDate.MaxDate = DateTime.MaxValue;
                RDP_StartDate.SelectedDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_EndDate.SelectedDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                RDP_StartDate.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_StartDate.MaxDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                RDP_EndDate.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_EndDate.MaxDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                if (RDP_StartDate.SelectedDate != null)
                    StartDate = DateTime.Parse(DDL_Years.SelectedValue + RDP_StartDate.SelectedDate.Value.ToString("-MM-dd"));
                if (RDP_EndDate.SelectedDate != null)
                    EndDate = DateTime.Parse(DDL_Years.SelectedValue + RDP_EndDate.SelectedDate.Value.ToString("-MM-dd"));
            }
        }

        protected void RadGridWasteBookItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var wasteBookId = new Guid(((HiddenField)e.Item.FindControl("HF_WasteBookId")).Value);
                e.Item.Cells[2].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
                var info = _wasteBookCheck.GetWasteBookCheck(wasteBookId);
                var banlance = decimal.Parse(((Label)e.Item.FindControl("lbNonceBalance")).Text);
                if (info != null)
                {
                    if (info.CheckMoney == banlance)
                    {
                        ((Label)e.Item.FindControl("lbNonceBalance")).Attributes.Remove("style");
                        ((Label)e.Item.FindControl("lbDiff")).Text = "（核对：+0元）";
                        ((Label)e.Item.FindControl("lbNonceBalance")).Attributes.Add("style", "margin-left:30px;float:left;color:#5F8000");
                    }
                    else
                    {
                        if (info.CheckMoney > banlance)
                        {
                            ((Label)e.Item.FindControl("lbDiff")).Text = "（核对：+" + (info.CheckMoney - banlance) + "元）";
                        }
                        else
                        {
                            ((Label)e.Item.FindControl("lbDiff")).Text = "（核对：-" + (banlance - info.CheckMoney) + "元）";
                        }
                        ((Label)e.Item.FindControl("lbNonceBalance")).Attributes.Remove("style");
                        ((Label)e.Item.FindControl("lbNonceBalance")).Attributes.Add("style", "margin-left:30px;float:left;color:#FA0030");
                    }
                }
                else
                {
                    ((Label)e.Item.FindControl("lbNonceBalance")).Attributes.Remove("style");
                    ((Label)e.Item.FindControl("lbNonceBalance")).Attributes.Add("style", "margin-left:30px;float:left;color:#000000");
                }
            }
        }

        #region  获取单据的来源类型 add by liangcanren at 2015-05-26
        public string GetLinkTradeType(object obj)
        {
            var key = Convert.ToInt32(obj);
            var linkTradeList = EnumAttribute.GetDict<WasteBookLinkTradeType>();
            if (linkTradeList != null && linkTradeList.ContainsKey(key))
                return linkTradeList[key];
            return "未设置";
        }
        #endregion
    }
}

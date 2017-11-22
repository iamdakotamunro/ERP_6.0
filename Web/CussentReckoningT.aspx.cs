using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;
using CompanyClass = ERP.DAL.Implement.Inventory.CompanyClass;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>往来账
    /// </summary>
    public partial class CussentReckoningT : BasePage
    {
        private readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyClass _companyClass = new CompanyClass(GlobalConfig.DB.FromType.Read);
        protected IList<CompanyCussentInfo> AllCompanyList;
        protected IList<CompanyBalanceInfo> AllCompanyBalanceList;
        protected IList<CompanyClassInfo> AllCompanyClassList;
        protected IList<CompanyBalanceDetailInfo> AllCompanyBalanceDetailList;

        #region[公用属性]
        #region add by dyy 2009-07-10
        //是否对账
        private int IsChecked
        {
            get
            {
                if (ViewState["IsChecked"] == null) return -1;
                return Convert.ToInt32(ViewState["IsChecked"].ToString());
            }
            set
            {
                ViewState["IsChecked"] = value;
            }
        }
        #endregion

        //往来单位编号
        private Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null) return Guid.Empty;
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set
            {
                ViewState["CompanyId"] = value.ToString();
            }
        }

        //往来单位分类编号
        private Guid CompanyClassId
        {
            get
            {
                if (ViewState["CompanyClassId"] == null) return Guid.Empty;
                return new Guid(ViewState["CompanyClassId"].ToString());
            }
            set
            {
                ViewState["CompanyClassId"] = value.ToString();
            }
        }

        //公司编号
        private Guid FilialeId
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

        //对账类型  (快递运费账和快递代收账)
        private int Type
        {
            get
            {
                if (ViewState["CheckType"] == null)
                {
                    ViewState["CheckType"] = -1;
                }

                return (int)ViewState["CheckType"];
            }
            set
            {
                ViewState["CheckType"] = value;
            }
        }

        //搜索选择公司编号
        private Guid SelectFilialeId
        {
            get
            {
                if (ViewState["SelectFilialeId"] == null) return Guid.Empty;
                return new Guid(ViewState["SelectFilialeId"].ToString());
            }
            set
            {
                ViewState["SelectFilialeId"] = value.ToString();
            }
        }

        // 开始时间
        private DateTime StartDate
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
                ViewState["StartDate"] = value.ToString(CultureInfo.InvariantCulture);
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

        public IList<FilialeInfo> FilialeList
        {
            get
            {
                if (ViewState["FilialeList"] == null)
                {
                    var list = CacheCollection.Filiale.GetList();
                    ViewState["FilialeList"] = list;
                }
                return (IList<FilialeInfo>)ViewState["FilialeList"];
            }
        }
        
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                AllCompanyList = _companyCussent.GetCompanyCussentList();
                AllCompanyBalanceList = _companyCussent.GetCompanyBalanceList();
                AllCompanyClassList = _companyClass.GetCompanyClassList();
                AllCompanyBalanceDetailList = _companyCussent.GetCompanyBalanceDetailList();
                
                Dictionary<Guid,string> dataSource=new Dictionary<Guid, string> { {Guid.Empty, "--所有--" } };
                RCB_Warehouse.DataSource = dataSource.Union(WMSSao.GetAllCanUseWarehouseDics());

                var filialeList= FilialeList.Where(f => f.FilialeTypes.Contains((int)FilialeType.SaleCompany) || f.FilialeTypes.Contains((int)FilialeType.LogisticsCompany) && !f.FilialeTypes.Contains((int)FilialeType.EntityShop));

                RCB_FilialeList.DataSource = filialeList;
                var personnelInfo = CurrentSession.Personnel.Get();
                if (personnelInfo.CurrentFilialeId == Guid.Empty)
                {
                    personnelInfo.CurrentFilialeId = filialeList.First().ID;
                    CurrentSession.Personnel.Set(personnelInfo);
                }
                SelectFilialeId = personnelInfo.CurrentFilialeId;
                GetCompanyCussent();

                RCB_Warehouse.DataBind();
                RCB_FilialeList.DataTextField = "Name";
                RCB_FilialeList.DataValueField = "ID";
                RCB_FilialeList.DataBind();
                RCB_FilialeList.SelectedValue = string.Format("{0}",personnelInfo.CurrentFilialeId);
                

                RDP_StartDate.SelectedDate = DateTime.Now.AddDays(-30);
                RDP_EndDate.SelectedDate = DateTime.Now;

            }

            if (GetPowerOperationPoint("CBIsOut") && !string.IsNullOrWhiteSpace(RCB_FilialeList.SelectedValue) && new Guid(RCB_FilialeList.SelectedValue) != Guid.Empty)
            {
                CB_IsOut.Visible = true;
            }
            else
            {
                CB_IsOut.Visible = false;
            }
        }

        //取得用户操作权限
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "CussentReckoningT.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }

        #region[绑定树节点]

        //绑定根节点和往来单位分类
        private void GetCompanyCussent()
        {
            RadTreeNode rootNode = CreateNode("往来单位", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            RTVCompanyCussent.Nodes.Add(rootNode);
            double total = RecursivelyCompanyClass(Guid.Empty, AllCompanyBalanceList, AllCompanyBalanceDetailList, rootNode);
            rootNode.Text += "[" + total.ToString("N") + "]";
            
            var list = SelectFilialeId != Guid.Empty?FilialeList.Where(ent=>ent.ID!=SelectFilialeId):FilialeList;
            var saleFilialeList = list.Where(ent => ent.FilialeTypes.Contains((int)FilialeType.EntityShop) && ent.ParentId == SelectFilialeId).ToList();
            if (saleFilialeList.Count > 0)
            {
                var filialeNode = CreateNode("内部公司", true, Guid.Empty.ToString());
                var saleNode = CreateNode("销售门店", true, Guid.Empty.ToString());
                filialeNode.Nodes.Add(saleNode);
                foreach (var node in saleFilialeList.Select(item => new RadTreeNode(item.Name, item.ID.ToString()) { ToolTip = "SaleFiliale" }))
                {
                    saleNode.Nodes.Add(node);
                }
                RTVCompanyCussent.Nodes.Add(filialeNode);
            }

            var filiale=FilialeManager.Get(SelectFilialeId);
            if (filiale!=null && filiale.FilialeTypes.Contains((int)FilialeType.SaleCompany))
            {
                var thirdNode = CreateNode("第三方公司", true, Guid.Empty.ToString());
                var automaticaccountNode = CreateNode("冻结自动进账", true, Guid.Empty.ToString());
                thirdNode.Nodes.Add(automaticaccountNode);
                var salePlatformList = CacheCollection.SalePlatform.GetListByFilialeId(SelectFilialeId);
                foreach (
                    var node in
                        salePlatformList.Where(act => act.IsActive && act.AccountCheckingType == 2)
                            .Select(act => new RadTreeNode(act.Name, act.ID.ToString()) { ToolTip = "ThirdSaleFiliale" }))
                {
                    automaticaccountNode.Nodes.Add(node);
                }
                thirdNode.ToolTip = "Company";
                RTVCompanyCussent.Nodes.Add(thirdNode);
            }
            

        }

        //遍历往来单位分类
        private double RecursivelyCompanyClass(Guid companyClassId, IList<CompanyBalanceInfo> allCompanyBalanceList, IList<CompanyBalanceDetailInfo> allCompanyBalanceDetailList, RadTreeNode node)
        {
            double count = 0;
            IList<CompanyClassInfo> childCompanyClassList = AllCompanyClassList.Where(w => w.ParentCompanyClassId == companyClassId).ToList();
            foreach (CompanyClassInfo companyClassInfo in childCompanyClassList)
            {
                RadTreeNode childNode = CreateNode(companyClassInfo.CompanyClassName, false, companyClassInfo.CompanyClassId.ToString());
                childNode.ToolTip = "CompanyClass";
                node.Nodes.Add(childNode);
                double leCount1 = RecursivelyCompanyClass(companyClassInfo.CompanyClassId, allCompanyBalanceList, allCompanyBalanceDetailList, childNode);
                double leCount2 = RepetitionCompanyCussent(companyClassInfo.CompanyClassId, allCompanyBalanceList, allCompanyBalanceDetailList, childNode);
                childNode.Text += "[" + (leCount1 + leCount2).ToString("N") + "]";
                count += (leCount1 + leCount2);
            }
            return count;
        }

        //遍历往来单位
        private double RepetitionCompanyCussent(Guid companyClassId, IList<CompanyBalanceInfo> allCompanyBalanceList, IList<CompanyBalanceDetailInfo> allCompanyBalanceDetailList, RadTreeNode node)
        {
            double accountCount = 0;
            IList<CompanyCussentInfo> companyCussentList = AllCompanyList.Where(w => w.CompanyClassId == companyClassId).ToList();
            foreach (CompanyCussentInfo companyCussentInfo in companyCussentList)
            {
                double leCount;
                if (FilialeId != Guid.Empty)
                {
                    var companyBalanceDetailInfo = allCompanyBalanceDetailList.FirstOrDefault(w => w.CompanyId == companyCussentInfo.CompanyId && w.FilialeId == FilialeId);
                    leCount = companyBalanceDetailInfo == null ? 0 : Convert.ToDouble(companyBalanceDetailInfo.NonceBalance);
                }
                else
                {
                    var companyBalanceInfo = allCompanyBalanceList.FirstOrDefault(w => w.CompanyId == companyCussentInfo.CompanyId);
                    leCount = companyBalanceInfo == null ? 0 : Convert.ToDouble(companyBalanceInfo.NonceBalance);
                }
                accountCount += leCount;
                RadTreeNode childNode = CreateNode(companyCussentInfo.CompanyName, false, companyCussentInfo.CompanyId.ToString());
                childNode.ToolTip = "Company";
                childNode.Text += "[" + leCount.ToString("N") + "]";
                node.Nodes.Add(childNode);

                IList<FilialeInfo> list = FilialeList.Where(ent => ent.FilialeTypes.Contains((int)FilialeType.LogisticsCompany) || ent.FilialeTypes.Contains((int)FilialeType.SaleCompany)).ToList();
                if (companyCussentInfo.RelevanceFilialeId != Guid.Empty)
                {
                    list = list.Where(f => f.ID != companyCussentInfo.RelevanceFilialeId).ToList();
                }
                list.Add(new FilialeInfo { ID = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID")), Name = "ERP" });
                foreach (var filialeInfo in list)
                {
                    var companyBalanceDetailInfo = allCompanyBalanceDetailList.FirstOrDefault(w => w.CompanyId == companyCussentInfo.CompanyId && w.FilialeId == filialeInfo.ID);
                    double sum = companyBalanceDetailInfo == null ? 0 : Convert.ToDouble(companyBalanceDetailInfo.NonceBalance);
                    var fnode = new RadTreeNode(filialeInfo.Name + "[" + sum.ToString("N") + "]", filialeInfo.ID.ToString()) { ToolTip = "Filiale" };
                    childNode.Nodes.Add(fnode);
                }
            }
            return accountCount;
        }

        //创建节点
        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }
        #endregion

        //绑定往来账数据源
        protected void RgReckoningNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (Page.IsPostBack && RTVCompanyCussent.SelectedNode.Level >= 1)
            {
                if (RTVCompanyCussent.SelectedNode.Level == 1 && RTVCompanyCussent.SelectedNode.ToolTip != "Company")
                {
                    RGReckoning.DataSource = new List<ReckoningInfo>();
                    return;
                }
                if (RTVCompanyCussent.SelectedNode.Level == 2 && RTVCompanyCussent.SelectedNode.ToolTip == "CompanyClass")
                {
                    RGReckoning.DataSource = new List<ReckoningInfo>();
                    return;
                }

                //添加按金额搜索
                int minmoney;
                int maxmoney;

                int[] moneys = null;
                if (int.TryParse(TB_MinMoney.Text, out minmoney))
                {
                    if (int.TryParse(TB_MaxMoney.Text, out maxmoney))
                    {
                        if (maxmoney > minmoney)
                            moneys = new[] { minmoney, maxmoney };
                    }
                    else
                        moneys = new[] { minmoney, int.MaxValue };
                }
                else
                {
                    if (int.TryParse(TB_MaxMoney.Text, out maxmoney))
                        moneys = new[] { int.MinValue, maxmoney };
                }
                var pageSize = RGReckoning.PageSize;
                RGReckoning.CurrentPageIndex = PageIndex;
                int recordCount;
                var endDate = DateTime.Now;
                if (RDP_EndDate.SelectedDate != null)
                {
                    endDate = Convert.ToDateTime(Convert.ToDateTime(RDP_EndDate.SelectedDate).AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                }

                bool? isOut = null;
                var rtvNode = RTVCompanyCussent.SelectedNode;
                if (rtvNode.ToolTip.Equals("ThirdSaleFiliale"))
                {
                    isOut = !CB_IsOut.Checked;
                }

                var list = _reckoning.GetValidateDataPage(CompanyClassId, CompanyId, FilialeId, StartDate, endDate, (CheckType)IsChecked, CurrentAuditingState, CurrentReceiptType, txtTradeCode.Text, new Guid(RCB_Warehouse.SelectedValue), GlobalConfig.KeepYear, RGReckoning.CurrentPageIndex * pageSize, pageSize, out recordCount, -1, Type, isOut, moneys);
                foreach (var info in list)
                {
                    info.FilialeName = FilialeManager.GetName(info.FilialeId);
                    if (string.IsNullOrEmpty(info.CompanyName))
                        info.CompanyName = FilialeManager.GetName(info.ThirdCompanyID);
                    if (string.IsNullOrEmpty(info.CompanyName))
                    {
                        var salePlatformInfo = CacheCollection.SalePlatform.Get(info.ThirdCompanyID);
                        info.CompanyName = salePlatformInfo==null?string.Empty: salePlatformInfo.Name;
                    }
                }

                RGReckoning.DataSource = list;
                RGReckoning.VirtualItemCount = recordCount;
            }
            else
                RGReckoning.DataSource = new List<ReckoningInfo>();
        }

        //选择往来单位分类树节点
        protected void RadTreeViewCompanyCussent_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                FilialeId = Guid.Empty;
                CompanyClassId = Guid.Empty;
                CompanyId = Guid.Empty;
                var rtvNode = e.Node;
                //note  搜索树节点处理优化  2015-03-19  陈重文
                switch (rtvNode.ToolTip)
                {
                    case "Filiale":
                        FilialeId = new Guid(rtvNode.Value);
                        CompanyId = new Guid(rtvNode.ParentNode.Value);
                        RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = true;
                        RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = false;
                        break;
                    case "CompanyClass":
                        CompanyClassId = new Guid(rtvNode.Value);
                        break;
                    case "Company":
                    case "SaleFiliale":
                    case "ThirdSaleFiliale":
                        CompanyId = new Guid(rtvNode.Value);
                        if (!string.IsNullOrWhiteSpace(RCB_FilialeList.SelectedValue))
                        {
                            //note 门店往来帐显示总账   2015-03-18  陈重文  
                            var filialeInfo = CacheCollection.Filiale.Get(CompanyId);
                            if (filialeInfo != null)
                            {
                                RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = true;
                                RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = false;
                            }
                            else
                            {
                                RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = false;
                                RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = true;
                            }
                        }
                        else
                        {
                            RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = false;
                            RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = true;
                        }
                        break;
                }
                if (SelectFilialeId != Guid.Empty && FilialeId == Guid.Empty)
                    FilialeId = SelectFilialeId;
                if (CompanyId != Guid.Empty)
                {
                    //取往来单位合同信息 
                    var info = _companyCussent.GetCompanyCussent(CompanyId);
                    LB_CompanyCussentInfo.Text = "合同信息：" + (info == null ? "" : info.Description);
                }

                CurrentReceiptType = ReceiptType.All;
                RCB_ReceiptType.SelectedValue = "-1";
                txtTradeCode.Text = string.Empty;
                rcbIsChecked.SelectedValue = "-1";
                Rcb_ReckoningCheckType.SelectedValue = "-1";
                StartDate = RDP_StartDate.SelectedDate ?? DateTime.MinValue;
                RGReckoning.Rebind();
            }
        }


        protected void RGReckoning_PageIndexChanged(object source, GridPageChangedEventArgs e)
        {
            PageIndex = e.NewPageIndex;
            RGReckoning.Rebind();
        }

        //导出Excel
        protected void imgBtn_ExportExcel_Click(object sender, EventArgs e)
        {
            string fileName = RTVCompanyCussent.SelectedNode == null ? "没有选择往来单位" : new Guid(RTVCompanyCussent.SelectedNode.Value) == Guid.Empty ? "没有选择往来单位" : RTVCompanyCussent.SelectedNode.Text;
            if (fileName.IndexOf('[') > -1)
            {
                fileName = fileName.Replace("[", "【").Replace("]", "】").Replace(",", "，");
            }
            fileName = Server.UrlEncode(fileName);
            RGReckoning.ExportSettings.ExportOnlyData = true;
            RGReckoning.ExportSettings.IgnorePaging = true;
            RGReckoning.ExportSettings.FileName = fileName;
            RGReckoning.MasterTableView.ExportToExcel();
        }

        protected void LbSearchClick(object sender, EventArgs e)
        {
            var start = RDP_StartDate.SelectedDate ?? DateTime.MinValue;
            var end = RDP_EndDate.SelectedDate ?? DateTime.MinValue;
            if (IsSerarchYear(start, end, GlobalConfig.KeepYear))
            {
                StartDate = RDP_StartDate.SelectedDate ?? DateTime.MinValue;
                CurrentReceiptType = (ReceiptType)Convert.ToInt32(RCB_ReceiptType.SelectedValue);
                IsChecked = Convert.ToInt32(rcbIsChecked.SelectedValue);
                Type = Convert.ToInt32(Rcb_ReckoningCheckType.SelectedValue);
                FilialeId = Guid.Empty;
                CompanyClassId = Guid.Empty;
                CompanyId = Guid.Empty;
                //note  搜索树节点处理优化  2015-03-19  陈重文
                var rtvNode = RTVCompanyCussent.SelectedNode;
                switch (rtvNode.ToolTip)
                {
                    case "Filiale":
                        FilialeId = new Guid(rtvNode.Value);
                        CompanyId = new Guid(rtvNode.ParentNode.Value);
                        RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = false;
                        RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = true;
                        break;
                    case "CompanyClass":
                        CompanyClassId = new Guid(rtvNode.Value);
                        break;
                    case "Company":
                    case "SaleFiliale":
                    case "ThirdSaleFiliale":
                        CompanyId = new Guid(rtvNode.Value);
                        if (!string.IsNullOrWhiteSpace(RCB_FilialeList.SelectedValue))
                        {
                            //note 门店往来帐显示总账   2015-03-18  陈重文  
                            var filialeInfo = CacheCollection.Filiale.Get(CompanyId);
                            if (filialeInfo != null)
                            {
                                RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = false;
                                RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = true;
                            }
                            else
                            {
                                RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = true;
                                RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = false;
                            }
                        }
                        else
                        {
                            RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = true;
                            RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = false;
                        }
                        break;
                }
                var personnelInfo = CurrentSession.Personnel.Get();
                if (personnelInfo.CurrentFilialeId!=SelectFilialeId)
                {
                    personnelInfo.CurrentFilialeId = SelectFilialeId;
                    CurrentSession.Personnel.Set(personnelInfo);
                } 
                RGReckoning.Rebind();
            }
            else
            {
                RAM.Alert("温馨提示：不支持当前时间段搜索，请检查配置文件！");
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="sCheck"></param>
        ///<returns></returns>
        public String SetCheckingState(int sCheck)
        {
            String strCheck;
            if (sCheck == 1)
            {
                strCheck = "成功对账";
            }
            else if (sCheck == 2)
            {
                strCheck = "异常对账";
            }
            else
            {
                strCheck = "没有对账";
            }
            return strCheck;
        }

        public string GetCheckType(object checkType)
        {
            var type = Convert.ToInt32(checkType);
            switch (type)
            {
                case (int)ReckoningCheckType.Carriage:
                    return "快递运费账";
                case (int)ReckoningCheckType.Collection:
                    return "快递代收账";
            }
            return "其它";
        }

        ///<summary>
        ///</summary>
        ///<param name="sCheck"></param>
        ///<returns></returns>
        public String GetCss(int sCheck)
        {
            String strCheck;
            if (sCheck == 1)
            {
                strCheck = "chkRed";
            }
            else if (sCheck == 2)
            {
                strCheck = "chkGreen";
            }
            else
            {
                strCheck = "";
            }
            return strCheck;
        }

        protected void RgReckoningItemDataBound(object sender, GridItemEventArgs e)
        {
            try
            {
                if (e.Item.RowIndex <= 1) return;
                var lblCheckLabel = e.Item.Cells[9].FindControl("IsCheckedLabel") as Label;
                if (lblCheckLabel != null)
                    switch (lblCheckLabel.Text.Trim())
                    {
                        case "成功对账":
                            lblCheckLabel.CssClass = "chkRed";
                            break;
                        case "异常对账":
                            lblCheckLabel.CssClass = "chkGreen";
                            break;
                    }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary> 当前页
        /// </summary>
        protected int PageIndex { get; set; }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            PageIndex = RGReckoning.CurrentPageIndex;
            WebControl.RamAjajxRequest(RGReckoning, e);
        }

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

            FilialeId = Guid.Empty;
            CompanyClassId = Guid.Empty;
            CompanyId = Guid.Empty;
            //note  搜索树节点处理优化  2015-03-19  陈重文
            var rtvNode = RTVCompanyCussent.SelectedNode;
            switch (rtvNode.ToolTip)
            {
                case "Filiale":
                    FilialeId = new Guid(rtvNode.Value);
                    CompanyId = new Guid(rtvNode.ParentNode.Value);
                    RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = false;
                    RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = true;
                    break;
                case "CompanyClass":
                    CompanyClassId = new Guid(rtvNode.Value);
                    break;
                case "Company":
                case "SaleFiliale":
                case "ThirdSaleFiliale":
                    CompanyId = new Guid(rtvNode.Value);
                    if (!string.IsNullOrWhiteSpace(RCB_FilialeList.SelectedValue))
                    {
                        //note 门店往来帐显示总账   2015-03-18  陈重文  
                        var filialeInfo = CacheCollection.Filiale.Get(CompanyId);
                        if (filialeInfo != null)
                        {
                            RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = true;
                            RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = false;
                        }
                        else
                        {
                            RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = false;
                            RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = true;
                        }
                    }
                    else
                    {
                        RGReckoning.MasterTableView.Columns.FindByUniqueName("ComCurrBalance").Visible = true;
                        RGReckoning.MasterTableView.Columns.FindByUniqueName("NonceTotalled").Visible = false;
                    }
                    break;
            }
            if (SelectFilialeId != Guid.Empty && FilialeId == Guid.Empty)
                FilialeId = SelectFilialeId;
            RGReckoning.Rebind();
        }

        protected void RadGridReckoningItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;
                var state = int.Parse(dataItem.GetDataKeyValue("State").ToString());
                if (state == (int)ReckoningStateType.Cancellation)
                {
                    e.Item.Style.Add("background-color", "#FF6666");//红色
                }
            }
        }

        #region[选择公司后重新加载树]
        protected void Rcb_FilialeListSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            AllCompanyList = _companyCussent.GetCompanyCussentList();
            AllCompanyBalanceList = _companyCussent.GetCompanyBalanceList();
            AllCompanyClassList = _companyClass.GetCompanyClassList();
            AllCompanyBalanceDetailList = _companyCussent.GetCompanyBalanceDetailList();
            var value = e.Value.Trim();
            SelectFilialeId = string.IsNullOrEmpty(value) ? Guid.Empty : new Guid(value);
            FilialeId = Guid.Empty;
            RTVCompanyCussent.Nodes.Clear();
            GetCompanyCussent();
            PageIndex = 0;
            RGReckoning.PageSize = 20;
            RGReckoning.CurrentPageIndex = 0;
            RGReckoning.VirtualItemCount = 0;
            RGReckoning.DataSource = new List<ReckoningInfo>();
            RGReckoning.DataBind();
        }
        #endregion

        /// <summary>根据配置文件判断当前搜索时间段是否搜索
        /// </summary>
        protected bool IsSerarchYear(DateTime startTime, DateTime endTime, int keepyear)
        {
            var nowYear = DateTime.Now.Year;
            var startYear = startTime.Year;
            var endYear = endTime.Year;
            if (keepyear == 1)
            {
                return startYear == endYear;
            }
            if (keepyear == 2)
            {
                if (startYear != endYear)
                {
                    return (startYear == nowYear || endYear == nowYear) && startYear == endYear - 1;
                }
                return startYear == endYear;
            }
            if (keepyear == 3)
            {
                if (startYear != endYear)
                {
                    var minYear = nowYear - 2;
                    if (startYear >= minYear && endYear <= nowYear)
                    {
                        return true;
                    }
                }
                return startYear == endYear;
            }
            return false;
        }

        #region  获取单据的来源类型 add by liangcanren at 2015-05-26
        public string GetLinkTradeType(object obj)
        {
            var key = Convert.ToInt32(obj);
            var linkTradeList = EnumAttribute.GetDict<ReckoningLinkTradeType>();
            if (linkTradeList != null && linkTradeList.ContainsKey(key))
                return linkTradeList[key];
            return "未设置";
        }
        #endregion

        #region

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentNotolled"></param>
        /// <param name="nonceTotolled"></param>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        public string ShowCurrentNotolled(object currentNotolled, object nonceTotolled, object isChecked)
        {
            if (RTVCompanyCussent.SelectedNode == null)
            {
                return "-";
            }
            string toolTips = RTVCompanyCussent.SelectedNode.ToolTip;
            if (!string.IsNullOrWhiteSpace(toolTips) && toolTips == "Filiale" && Convert.ToInt32(isChecked) == 1)
            {
                var current = Convert.ToDecimal(currentNotolled);
                var nonce = Convert.ToDecimal(nonceTotolled);
                if (current != nonce && current != 0)
                {
                    return string.Format("<font color='red'>{0}</font>", WebControl.RemoveDecimalEndZero(Convert.ToDecimal(currentNotolled)));
                }
                return WebControl.RemoveDecimalEndZero(Convert.ToDecimal(currentNotolled));
            }
            return "0.00";
        }
        #endregion

        public string ShowReckoningForm(int type)
        {
            return string.Format("return ShowReckoningInfoForm('{0}','{1}')", type,
                string.IsNullOrEmpty(RCB_FilialeList.SelectedValue) ? Guid.Empty : new Guid(RCB_FilialeList.SelectedValue));
        }
    }
}

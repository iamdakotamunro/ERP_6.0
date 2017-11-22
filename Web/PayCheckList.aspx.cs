using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.Cache;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using MIS.Enum.Attributes;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>付款审核 
    /// </summary>
    public partial class PayCheckList : BasePage
    {
        private readonly ICompanyClass _companyClass = new CompanyClass(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private readonly IPersonnelSao _personnelManager = new PersonnelSao();
        readonly ICompanyAuditingPower _companyAuditingPower = new DAL.Implement.Inventory.CompanyAuditingPower(GlobalConfig.DB.FromType.Read);
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReceiptNo = string.Empty;
                Status = CompanyFundReceiptState.NoHandle;

                GetTreeCompanyClass();

                LoadFilialeData();
                LoadStatusData();

                GetCompanyList();
                txt_PaymentDate.Text = DateTime.Now.ToString("yyyy-MM");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgCheckInfoNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DdlSaleFiliale.SelectedValue))
                SelectSaleFilialeId = DdlSaleFiliale.SelectedValue;
            Status = (CompanyFundReceiptState)int.Parse(DDL_CheckState.SelectedValue);
            ReceiptNo = TB_CompanyFundReciptNO.Text;
            PayType = int.Parse(DDL_PayType.SelectedValue);
            GridDataBind();
            RG_CheckInfo.CurrentPageIndex = 0;
            RG_CheckInfo.DataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            List<CompanyFundReceiptInfo> list = _companyFundReceipt.GetAllFundReceiptInfoList(new Guid(SelectSaleFilialeId), ReceiptPage.PayCheckList,
                Status, StartTime, EndTime, ReceiptNo, CompanyFundReceiptType.Payment).ToList();

            if (RT_CompanyClass.SelectedNode != null)
            {
                RadTreeNode currentNode = RT_CompanyClass.SelectedNode;
                //门店类型节点与店铺节点选择时数据绑定 
                //modify by liangcanren at 2015-03-16
                var isShopJoinType = currentNode.Value.Length == Guid.Empty.ToString().Length;
                var companyClassId = isShopJoinType ? new Guid(currentNode.Value) : Guid.Empty;
                var shopList = CacheCollection.Filiale.GetShopList();
                if (!isShopJoinType)
                {
                    shopList = shopList.Where(act => string.Format("{0}", act.ShopJoinType) == currentNode.Value).ToList();
                    if (shopList.Count != 0)
                    {
                        list = list.Where(c => shopList.Any(act => act.ID == c.CompanyID)).ToList();
                    }
                }
                else
                {
                    if (shopList.Any(act => act.ID == companyClassId))
                    {
                        list = list.Where(c => c.CompanyID == companyClassId).ToList();
                    }
                    else
                    {
                        if (companyClassId != Guid.Empty)
                        {
                            List<Guid> companylist = _companyCussent.GetCompanyCussentList(companyClassId).Select(cl => cl.CompanyId).ToList();
                            list = list.Where(c => companylist.Contains(c.CompanyID)).ToList();
                        }
                    }
                }
            }

            if (BankId != Guid.Empty)
            {
                list = list.Where(ent => ent.PayBankAccountsId == BankId).ToList();
            }
            if (PayType != -1)
            {
                if (PayType == 0)
                {
                    list = list.Where(c => string.IsNullOrEmpty(c.PurchaseOrderNo) && string.IsNullOrEmpty(c.StockOrderNos)
                        && c.SettleEndDate != DateTime.Parse("1999-09-09")).ToList();
                }
                if (PayType == 1)
                {
                    list = list.Where(c => !string.IsNullOrEmpty(c.StockOrderNos)).ToList();
                }
                if (PayType == 2)
                {
                    list = list.Where(c => !string.IsNullOrEmpty(c.PurchaseOrderNo)).ToList();
                }
                if (PayType == 3)
                {
                    list = list.Where(c => string.IsNullOrEmpty(c.PurchaseOrderNo) && string.IsNullOrEmpty(c.StockOrderNos)
                        && c.SettleStartDate == DateTime.Parse("1999-09-09")).ToList();
                }
                if (PayType == 4)
                {
                    list = list.Where(ent => ent.SettleStartDate == DateTime.Parse("1999-09-09")).ToList();
                }
            }

            if (!string.IsNullOrEmpty(txt_PaymentDate.Text))
            {
                list = list.Where(p => Convert.ToDateTime(p.PaymentDate).ToString("yyyy-MM").Equals(txt_PaymentDate.Text)).ToList();
            }

            if (!string.IsNullOrEmpty(RCB_CompanyList.SelectedValue))
            {
                list = list.Where(p => p.CompanyID.Equals(new Guid(RCB_CompanyList.SelectedValue))).ToList();
            }
            if (!string.IsNullOrEmpty(rcb_Applicant.SelectedValue) && !rcb_Applicant.SelectedValue.Equals(Guid.Empty.ToString()))
            {
                list = list.Where(p => p.ApplicantID.Equals(new Guid(rcb_Applicant.SelectedValue))).ToList();
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

        //绑定所有状态
        protected void LoadStatusData()
        {
            var newDic = new Dictionary<int, string>();
            //审核页面：未通过（0）、作废（8）、待审核（1）、已审核（3、4、5、6、7、9、10）
            var stateDic = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptState>();
            foreach (var status in stateDic)
            {
                if (status.Key == -1 || status.Key == -2 || status.Key == -3)
                {
                    newDic.Add(status.Key, status.Value);
                }
            }

            DDL_CheckState.DataSource = newDic;
            DDL_CheckState.DataTextField = "Value";
            DDL_CheckState.DataValueField = "Key";
            DDL_CheckState.DataBind();
            DDL_CheckState.SelectedValue = ((int)CompanyFundReceiptState.NoHandle).ToString();
        }


        #region[获取往来单位数据信息，包含供应商和物流公司]
        /// <summary>
        /// 获取往来单位数据信息，包含供应商和物流公司
        /// </summary>
        protected void GetCompanyList()
        {
            RCB_CompanyList.DataSource = CompanyCussentList();
            RCB_CompanyList.DataTextField = "CompanyName";
            RCB_CompanyList.DataValueField = "CompanyId";
            RCB_CompanyList.DataBind();
            RCB_CompanyList.Items.Insert(0, new RadComboBoxItem("全部", string.Empty));
        }

        /// <summary>
        ///  获取供应商集合
        /// </summary>
        /// <returns></returns>
        protected IList<CompanyCussentInfo> CompanyCussentList()
        {
            int[] companyTypes = { (int)CompanyType.Suppliers, (int)CompanyType.Express, (int)CompanyType.Vendors };
            return _companyCussent.GetCompanyCussentList(State.Enable).Where(ent => companyTypes.Contains(ent.CompanyType) || ent.RelevanceFilialeId != Guid.Empty).ToList();
        }

        protected void RCB_CompanyList_OnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text;
                var companyList = (IList<CompanyCussentInfo>)CompanyCussentList().Where(p => p.CompanyName.IndexOf(key, StringComparison.Ordinal) > -1).ToList();
                if (e.NumberOfItems >= companyList.Count)
                {
                    e.EndOfItems = true;
                }
                else
                {
                    foreach (CompanyCussentInfo i in companyList)
                    {
                        var item = new RadComboBoxItem { Text = i.CompanyName, Value = i.CompanyId.ToString() };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        #endregion

        #region 属性

        protected DateTime StartTime
        {
            get
            {
                if (string.IsNullOrEmpty(txt_StartTime.Text))
                {
                    return DateTime.MinValue;
                }
                return DateTime.Parse(txt_StartTime.Text);
            }
        }

        protected DateTime EndTime
        {
            get
            {
                if (string.IsNullOrEmpty(txt_EndTime.Text))
                {
                    return DateTime.MaxValue;
                }
                return DateTime.Parse(txt_EndTime.Text).AddDays(1);
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

        protected int PayType
        {
            set { ViewState["PayType"] = value; }
            get
            {
                if (ViewState["PayType"] == null)
                {
                    return -1;
                }
                return int.Parse(ViewState["PayType"].ToString());
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
        /// 公司列表
        /// </summary>
        public IList<FilialeInfo> SaleFilialeList
        {
            get
            {
                if (ViewState["SaleFilialeList"] == null)
                {
                    ViewState["SaleFilialeList"] = CacheCollection.Filiale.GetHeadList();
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
        /// <summary>
        /// 往来单位权限
        /// </summary>
        protected IList<CompanyAuditingPowerInfo> GetCompanyAuditingPower
        {
            get
            {
                if (ViewState["CompanyAuditingPowerInfo"] == null)
                {
                    PersonnelInfo personnelInfo = CurrentSession.Personnel.Get();
                    ViewState["CompanyAuditingPowerInfo"] = _companyAuditingPower.GetCompanyAuditingPower(personnelInfo.FilialeId, personnelInfo.BranchId, personnelInfo.PositionId);
                }
                return (IList<CompanyAuditingPowerInfo>)ViewState["CompanyAuditingPowerInfo"];
            }
            set
            {
                ViewState["CompanyAuditingPowerInfo"] = value;
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

        protected string GetPersonName(string personId)
        {
            return _personnelManager.GetName(new Guid(personId));
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

        protected bool IsShow(string status, string realityBalance, string companyId)
        {
            if (status == string.Format("{0}", (int)CompanyFundReceiptState.WaitAuditing)
                || status == string.Format("{0}", (int)CompanyFundReceiptState.NoAuditing)
                || status == string.Format("{0}", (int)CompanyFundReceiptState.PayBack))
            {
                decimal balance = decimal.Parse(realityBalance);
                var companyAuditingPowerList = GetCompanyAuditingPower.Where(p => p.CompanyID.ToString() == companyId && p.LowerMoney <= balance && p.UpperMoney >= balance);
                if (companyAuditingPowerList.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        #endregion

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_CheckInfo, e);
        }

        #region[绑定银行账号]
        protected void BindBankDataBound(object sender, EventArgs e)
        {
            var ddlBank = sender as DropDownList;
            if (ddlBank != null)
            {
                ddlBank.Items.Clear();
                ddlBank.Items.Insert(0, new ListItem("全部", Guid.Empty.ToString()));
                ddlBank.SelectedValue = Guid.Empty.ToString();
            }
        }
        #endregion

        #region[选择往来单位分类树节点]
        protected void RtCompanyClassNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Node.Value))
                RG_CheckInfo.Rebind();
        }
        #endregion

        #region[绑定往来单位树]
        //获取往来单位分类树
        private void GetTreeCompanyClass()
        {
            RadTreeNode rootNode = CreateNode("往来单位分类", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            RT_CompanyClass.Nodes.Add(rootNode);
            RecursivelyCompanyClass(Guid.Empty, rootNode);
            #region  添加门店往来单位收付款审核权限节点 add by liangcanren at 2015-03-16
            var joinTypeList = EnumArrtibute.GetDict<ShopJoinType>();
            foreach (var joinTypeKey in joinTypeList)
            {
                RadTreeNode typeNode = CreateNode(string.Format("门店类型-{0}", joinTypeKey.Value), false, string.Format("{0}", joinTypeKey.Key));
                typeNode.Category = "ShopJoinType";
                typeNode.Selected = false;
                typeNode.PostBack = true;
                RT_CompanyClass.Nodes.Add(typeNode);
                RecursivelyCompanyShop(typeNode, joinTypeKey.Key);
            }
            #endregion
            RT_CompanyClass.Nodes.Add(rootNode);
        }

        /// <summary>
        /// 添加门店节点 
        /// add by liangcanren at 2015-03-16
        /// </summary>
        /// <param name="node"></param>
        /// <param name="shopJoinType"></param>
        private void RecursivelyCompanyShop(RadTreeNode node, int shopJoinType)
        {
            var shopList = CacheCollection.Filiale.GetShopList();
            if (shopList == null)
                return;
            shopList = shopList.Where(act => act.ShopJoinType == shopJoinType).ToList();
            foreach (var shopInfo in shopList)
            {
                RadTreeNode childNode = CreateNode(shopInfo.Name, false, shopInfo.ID.ToString());
                node.Nodes.Add(childNode);
            }
        }

        private void RecursivelyCompanyClass(Guid companyClassId, RadTreeNode node)
        {
            IList<CompanyClassInfo> childcompanyClassList = _companyClass.GetChildCompanyClassList(companyClassId);
            foreach (CompanyClassInfo childCompanyClass in childcompanyClassList)
            {
                RadTreeNode childNode = CreateNode(childCompanyClass.CompanyClassName, false, childCompanyClass.CompanyClassId.ToString());
                node.Nodes.Add(childNode);
                RecursivelyCompanyClass(childCompanyClass.CompanyClassId, childNode);
            }
        }

        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }
        #endregion

        /// <summary>
        /// 绑定公司
        /// </summary>
        /// <returns></returns>
        protected void LoadFilialeData()
        {
            var newDic = new Dictionary<string, string> { { Guid.Empty.ToString(), string.Empty } };
            foreach (var info in SaleFilialeList)
            {
                newDic.Add(info.ID.ToString(), info.Name);
            }
            newDic.Add(_reckoningElseFilialeid.ToString(), "ERP");

            DdlSaleFiliale.DataSource = newDic;
            DdlSaleFiliale.DataTextField = "Value";
            DdlSaleFiliale.DataValueField = "Key";
            DdlSaleFiliale.DataBind();
        }

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

        //申请人数据绑定
        protected void rcb_Applicant_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var personnelList = _personnelManager.GetList().Where(p => p.RealName.Contains(e.Text)).ToList();
                Int32 totalCount = personnelList.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in personnelList)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.RealName, item.PersonnelId.ToString()));
                    }
                }
            }
        }
    }
}

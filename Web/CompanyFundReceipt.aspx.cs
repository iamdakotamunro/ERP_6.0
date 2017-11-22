using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary> 收付款单据
    /// </summary>
    public partial class CompanyFundReceipt : BasePage
    {
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        protected List<CompanyCussentInfo> CompanyList;
        readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);

        #region -- Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            CompanyList = GetCompanyList();
            if (!IsPostBack)
            {
                ViewState["CompanyFundReceiptInfoList"] = null;
                BindComboBoxReceiptType();
                BindComboBoxVerifyStatus();
                BindCompany(CompanyList);
            }
        }
        #endregion

        #region -- 绑定往来单位收付款数据
        /// <summary>
        /// 绑定往来单位收付款数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RG_ReceiptGridList_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (IsSearch)
            {
                GridBindDataSearch();
            }
            else
            {
                GridBindData();
            }
        }

        private void GridBindData()
        {
            var startPage = RG_ReceiptGridList.CurrentPageIndex + 1;
            int pageSize = RG_ReceiptGridList.PageSize;
            int total;
            var list = _companyFundReceipt.GetDefaultFundReceiptInfoListByPage(CurrentSession.Personnel.Get().PersonnelId, startPage, pageSize, out total);
            RG_ReceiptGridList.VirtualItemCount = total;
            RG_ReceiptGridList.DataSource = list;
        }

        private void GridBindDataSearch()
        {
            var receiptType = (CompanyFundReceiptType)int.Parse(RCB_ReceiptType.SelectedValue);
            var receiptState = (CompanyFundReceiptState)int.Parse(RCB_VerifyStatus.SelectedValue);
            DateTime startDatetime = RC_StartDate.SelectedDate ?? DateTime.MinValue;
            DateTime endDatetime = RC_EndDate.SelectedDate ?? DateTime.Now;
            endDatetime = Convert.ToDateTime(endDatetime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            string receiptNo = RTB_SearchReceiptNO.Text.Trim();
            Guid companyId = (RCB_Company.SelectedItem == null || string.IsNullOrEmpty(RCB_Company.SelectedItem.Value)) ? Guid.Empty : new Guid(RCB_Company.SelectedItem.Value);

            var startPage = RG_ReceiptGridList.CurrentPageIndex + 1;
            int pageSize = RG_ReceiptGridList.PageSize;
            int total;
            var list = _companyFundReceipt.GetAllFundReceiptInfoListByPage(Guid.Empty, ReceiptPage.CompanyFundReceipt, receiptState, startDatetime, endDatetime, receiptNo, receiptType, companyId, startPage, pageSize, out total);
            RG_ReceiptGridList.VirtualItemCount = total;
            RG_ReceiptGridList.DataSource = list;
        }

        #endregion

        #region -- 获取公司数据信息，包含供应商和物流公司
        /// <summary>
        /// 获取公司数据信息，包含供应商和物流公司
        /// </summary>
        protected List<CompanyCussentInfo> GetCompanyList()
        {
            int[] companyTypes = { (int)CompanyType.Suppliers, (int)CompanyType.Express, (int)CompanyType.Vendors };
            return _companyCussent.GetCompanyCussentList(State.Enable).Where(ent => companyTypes.Contains(ent.CompanyType) || ent.RelevanceFilialeId != Guid.Empty).ToList();
        }
        #endregion

        #region -- 获取公司名称
        /// <summary>
        /// 获取公司名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected string GetCompanyName(string id)
        {
            var companyInfo = CompanyList.FirstOrDefault(c => c.CompanyId == new Guid(id));
            if (companyInfo != null)
            {
                return companyInfo.CompanyName;
            }
            var filialeInfo = MISService.GetAllFiliales().FirstOrDefault(act => act.ID == new Guid(id));
            if (filialeInfo != null)
            {
                return filialeInfo.Name;
            }
            return string.Empty;
        }
        protected void BindCompany(List<CompanyCussentInfo> list)
        {
            RCB_Company.Items.Add(new RadComboBoxItem("全部", Guid.Empty.ToString()));
            foreach (var commpany in list)
            {
                RCB_Company.Items.Add(new RadComboBoxItem(commpany.CompanyName, commpany.CompanyId.ToString()));
            }
        }
        #endregion

        #region -- 获取申请者名称
        /// <summary>
        /// 获取申请者名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected string GetApplicantName(string id)
        {
            return new PersonnelManager().GetName(new Guid(id));
        }
        #endregion

        #region -- 获取单据状态
        /// <summary>
        /// 获取单据状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        protected string GetReceiptStatus(string status)
        {
            var rstatus = (CompanyFundReceiptState)Convert.ToInt32(status);
            return EnumAttribute.GetKeyName(rstatus);
        }
        #endregion

        #region -- 获取单据类型，区别是付款单还是收款单
        /// <summary>
        /// 获取单据类型，区别是付款单还是收款单
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string GetReceiptType(string type)
        {
            var rtype = (CompanyFundReceiptType)Convert.ToInt32(type);
            return EnumAttribute.GetKeyName(rtype);
        }
        #endregion

        #region -- 绑定审核状态
        /// <summary>
        /// 绑定审核状态
        /// </summary>
        protected void BindComboBoxVerifyStatus()
        {
            var statusList = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptState>();
            foreach (var status in statusList)
            {
                if (status.Key == -1 || status.Key == -2 || status.Key == -3)
                {
                    RCB_VerifyStatus.Items.Add(new RadComboBoxItem(status.Value, string.Format("{0}", status.Key)));
                }
            }
        }
        #endregion

        #region -- 绑定单据的类型
        /// <summary>
        /// 绑定单据的类型
        /// </summary>
        protected void BindComboBoxReceiptType()
        {
            var typeList = (Dictionary<int, string>)EnumAttribute.GetDict<CompanyFundReceiptType>();
            foreach (var type in typeList)
            {
                RCB_ReceiptType.Items.Add(new RadComboBoxItem(type.Value, string.Format("{0}", type.Key)));
            }
        }
        #endregion

        #region -- AJAX脚本运行
        /// <summary>
        /// AJAX脚本运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RAM_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_ReceiptGridList, e);
        }
        #endregion

        #region -- 点击搜索
        /// <summary>
        /// 点击搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Search_Click(object sender, EventArgs e)
        {
            IsSearch = true;
            RG_ReceiptGridList.CurrentPageIndex = 0;
            GridBindDataSearch();
            RG_ReceiptGridList.DataBind();
        }

        public bool IsSearch
        {
            get
            {
                if (ViewState["IsSearch"] == null)
                {
                    ViewState["IsSearch"] = false;
                }
                return (bool)ViewState["IsSearch"];
            }
            set
            {
                ViewState["IsSearch"] = value;
            }
        }
        #endregion

        #region -- 往来收付款 ItemDataBound 事件
        /// <summary>
        /// 往来收付款 ItemDataBound 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RG_ReceiptGridList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var griditem = e.Item as GridDataItem;
                var info = e.Item.DataItem as CompanyFundReceiptInfo;
                if (info == null) return;
                var imgBtnCancel = griditem.FindControl("ImgBtn_LoseReceipt") as ImageButton;
                if (imgBtnCancel == null) return;
                var btnEdit = griditem.FindControl("btn_Edit") as Button;
                var btnLook = griditem.FindControl("btn_Look") as Button;
                if (btnEdit == null) return;
                if (btnLook == null) return;
                if (info.ReceiptStatus == (int)CompanyFundReceiptState.WaitAuditing || info.ReceiptStatus == (int)CompanyFundReceiptState.NoAuditing || info.ReceiptStatus == (int)CompanyFundReceiptState.NoAuditingPass)
                {
                    if (info.ReceiptType==(int)CompanyFundReceiptType.Receive && (info.ReceiptStatus == (int)CompanyFundReceiptState.NoAuditing || info.ReceiptStatus == (int)CompanyFundReceiptState.NoAuditingPass))
                    {
                        btnEdit.Visible = false;
                    }
                    else
                    {
                        btnEdit.Visible = true;
                    }
                    imgBtnCancel.Visible = true;
                    btnLook.Visible = false;
                }
                else
                {
                    imgBtnCancel.Visible = false;
                    btnEdit.Visible = false;
                    btnLook.Visible = true;

                    if (info.ReceiptType == (int)CompanyFundReceiptType.Receive)
                    {
                        if (info.HasInvoice && info.ReceiptStatus == (int)CompanyFundReceiptState.WaitInvoice)
                        {
                            imgBtnCancel.Visible = true;
                            btnEdit.Visible = true;
                            btnLook.Visible = false;
                        }
                        else if (!info.HasInvoice && info.ReceiptStatus == (int)CompanyFundReceiptState.Audited)
                        {
                            imgBtnCancel.Visible = true;
                            btnEdit.Visible = true;
                            btnLook.Visible = false;
                        }
                    }
                }
                if (MISService.GetAllFiliales().Any(act => act.ID == info.CompanyID && act.FilialeTypes.Contains((int)FilialeType.EntityShop)))
                {
                    imgBtnCancel.Visible = false;
                    btnEdit.Visible = false;
                    btnLook.Visible = true;
                }
            }
        }
        #endregion

        protected void RCB_Company_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
                RCB_Company.Items.Clear();
                BindCompany(CompanyList.Where(c => c.CompanyName.Contains(e.Text)).ToList());
            }
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
            return string.Empty;
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
                    ViewState["SaleFilialeList"] = CacheCollection.Filiale.GetHostingAndSaleFilialeList();
                }
                return (IList<FilialeInfo>)ViewState["SaleFilialeList"];
            }
            set
            {
                ViewState["SaleFilialeList"] = value;
            }
        }
    }
}

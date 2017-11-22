using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.Cache;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.Data.Extensions;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class DemandInvoice : BasePage
    {
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyFundReceiptInvoice _companyFundReceiptInvoice = new CompanyFundReceiptInvoice(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGetCompanyListData();
                LoadSaleFilialeData();
            }
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_ReceiptInvoice.CurrentPageIndex = 0;
            RG_ReceiptInvoice.DataBind();
        }

        #region 数据准备
        #region[获取往来单位数据信息，包含供应商和物流公司]
        /// <summary>
        ///  获取往来单位数据信息，包含供应商和物流公司
        /// </summary>
        /// <returns></returns>
        protected IList<CompanyCussentInfo> CompanyCussentList()
        {
            CompanyType[] companyType = { CompanyType.Suppliers, CompanyType.Express, CompanyType.Vendors };
            var data = (List<CompanyCussentInfo>)_companyCussent.GetCompanyCussentList(companyType, State.Enable);
            return data;
        }

        /// <summary>
        /// 加载往来单位数据信息
        /// </summary>
        protected void LoadGetCompanyListData()
        {
            rcb_CompanyList.DataSource = CompanyCussentList();
            rcb_CompanyList.DataTextField = "CompanyName";
            rcb_CompanyList.DataValueField = "CompanyId";
            rcb_CompanyList.DataBind();
            rcb_CompanyList.Items.Insert(0, new RadComboBoxItem("全部", string.Empty));
        }

        protected void rcb_CompanyList_OnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
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

        //公司
        protected void LoadSaleFilialeData()
        {
            var list = CacheCollection.Filiale.GetHeadList();
            list.Add(new FilialeInfo { ID = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID")), Name = "ERP公司" });
            ddl_SaleFiliale.DataSource = list;
            ddl_SaleFiliale.DataTextField = "Name";
            ddl_SaleFiliale.DataValueField = "ID";
            ddl_SaleFiliale.DataBind();
            ddl_SaleFiliale.Items.Insert(0, new ListItem("全部", ""));
        }
        #endregion

        #region 数据列表相关
        protected void RG_ReceiptInvoice_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            var erpFilialeId = ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID");
            //未付款：待审核，已审核，未通过，已执行； 已付款：完成打款；
            //已审核过的单据(排除ERP公司的单据)
            var list = _companyFundReceipt.GetAllCompanyFundReceiptList().Where(p => (new int?[] { (int)CompanyFundReceiptState.NoAuditing, (int)CompanyFundReceiptState.Audited, (int)CompanyFundReceiptState.Executed, (int)CompanyFundReceiptState.Finish }).Contains(p.ReceiptStatus) && p.ReceiptType.Equals(1) && !p.FilialeId.ToString().Contains(erpFilialeId.ToLower())).OrderBy(p => p.ApplyDateTime).AsQueryable();

            #region 查询条件
            if (!string.IsNullOrEmpty(ddl_SaleFiliale.SelectedValue))
            {
                list = list.Where(p => p.FilialeId.Equals(new Guid(ddl_SaleFiliale.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(rcb_CompanyList.SelectedValue))
            {
                list = list.Where(p => p.CompanyID.Equals(new Guid(rcb_CompanyList.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(txt_ApplyDateTimeStart.Text))
            {
                var startTime = DateTime.Parse(txt_ApplyDateTimeStart.Text);
                list = list.Where(p => p.ApplyDateTime >= startTime);
            }
            if (!string.IsNullOrEmpty(txt_ApplyDateTimeEnd.Text))
            {
                var endtime = DateTime.Parse(txt_ApplyDateTimeEnd.Text);
                list = list.Where(p => p.ApplyDateTime < endtime.AddDays(1));
            }
            if (!string.IsNullOrEmpty(txt_ReceipNo.Text))
            {
                list = list.Where(p => p.ReceiptNo.Equals(txt_ReceipNo.Text));
            }
            if (!string.IsNullOrEmpty(ddl_ReceiptStatus.SelectedValue))
            {
                list = list.Where(p => p.ReceiptStatus.Equals(int.Parse(ddl_ReceiptStatus.SelectedValue)));
            }
            if (!string.IsNullOrEmpty(dll_InvoiceType.SelectedValue))
            {
                list = list.Where(p => p.InvoiceType.Equals(int.Parse(dll_InvoiceType.SelectedValue)));
                if (!String.IsNullOrWhiteSpace(txtBillingUnit.Text))
                {
                   var invoiceId = _companyFundReceiptInvoice.GetAlllmshop_CompanyFundReceiptInvoice()
                        .Where(ent => ent.BillingUnit.Contains(txtBillingUnit.Text)).Select(ent=>ent.ReceiptID);
                    list = list.Where(ent => invoiceId.Contains(ent.ReceiptID));
                }
            }
            #endregion

            RG_ReceiptInvoice.DataSource = list.OrderByDescending(p => p.ApplyDateTime);
        }

        #region 列表显示辅助方法
        /// <summary>
        /// 根据公司id获取公司名称
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        protected string GetFilialeName(string filialeId)
        {
            var list = CacheCollection.Filiale.GetHeadList();
            list.Add(new FilialeInfo { ID = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID")), Name = "ERP公司" });
            var firstOrDefault = list.FirstOrDefault(p => p.ID.Equals(new Guid(filialeId)));
            if (firstOrDefault != null)
            {
                return firstOrDefault.Name;
            }
            else
            {
                return "-";
            }
        }
        /// <summary>
        /// 根据往来单位id获取往来单位
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        protected string GetCompanyName(string companyId)
        {
            var list = RelatedCompany.Instance.ToList();
            if (list == null)
                return "-";
            var info = list.FirstOrDefault(o => o.CompanyId == new Guid(companyId));
            if (info == null)
            {
                var shopList = CacheCollection.Filiale.GetShopList();
                if (shopList == null)
                    return "-";
                var shopInfo = shopList.FirstOrDefault(o => o.ID == new Guid(companyId));
                return shopInfo == null ? "-" : shopInfo.Name;
            }
            return info.CompanyName;
        }
        /// <summary>
        /// 根据往来单位id获取往来单位信息
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="type">1：联系人；2：手机</param>
        /// <returns></returns>
        protected string GetCompanyInfo(Guid companyId, int type)
        {
            var info = _companyCussent.GetCompanyCussent(companyId);
            if (info == null) return "-";
            if (type == 1)
                return info.Linkman;
            return info.Mobile;
        }
        #endregion
        #endregion
    }
}
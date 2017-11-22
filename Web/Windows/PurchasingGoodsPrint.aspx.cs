using System;
using System.Linq;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class PurchasingGoodsPrint : WindowsPage
    {

        private readonly ExcelTemplate _temp = new ExcelTemplate(GlobalConfig.DB.FromType.Read);
        private readonly IPurchasingDetail _purchasingDetail=new PurchasingDetail(GlobalConfig.DB.FromType.Read);
        private readonly IPurchasing _purchasing=new Purchasing(GlobalConfig.DB.FromType.Read);
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);

        protected bool IsVisibale { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var purchasingId = new Guid(Request["PurchasingID"]);
                var warehouseId = new Guid(Request["WarehouseId"]);
                RCB_ExcelTemp.DataSource = _temp.GetExcelTemplateList().Where(act => act.WarehouseId == warehouseId);
                RCB_ExcelTemp.DataBind();
                PurchasingInfo pInfo = _purchasing.GetPurchasingById(purchasingId);
                IsVisibale = pInfo.PurchasingState != (int) PurchasingState.WaitingAudit;
                lab_company.Text = pInfo.CompanyName;
                lab_Time.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                lab_purNo.Text = pInfo.PurchasingNo;
                Cbx_PriceShow.Checked = true;
                lab_ArrivalTime.Text = pInfo.ArrivalTime.ToString("yyyy-MM-dd HH:mm");
                var info = new PersonnelSao().GetList().FirstOrDefault(w => w.PersonnelId == pInfo.PersonResponsible);
                if (info != null)
                {
                    lab_Director.Text = info.RealName;
                }

                //lab_Director.Text = pInfo.Director;

                img_keede.ImageUrl = string.Format("~/Images/cachet/{0}.png", pInfo.PurchasingFilialeId);

                var company = _companyCussent.GetCompanyCussent(pInfo.CompanyID);
                if (company != null)
                {
                    lbl_Linkman.Text = company.Linkman;
                    lbl_Mobile.Text = company.Mobile + "/" + company.Phone;
                }
            }
        }
        protected void Rgd_GoodsPrint_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var purchasingId = new Guid(Request["PurchasingID"]);
            Rgd_GoodsPrint.DataSource = _purchasingDetail.Select(purchasingId).OrderBy(act => act.Specification);
        }

        /// <summary>
        /// 实体模板
        /// </summary>
        protected ExcelTemplateInfo TempInfo
        {
            get
            {
                if (ViewState["TempInfo"] == null)
                {
                    if (new Guid(RCB_ExcelTemp.SelectedValue) == Guid.Empty)
                    {
                        return new ExcelTemplateInfo();
                    }
                    return TempInfo = _temp.GetExcelTemplateInfo(new Guid(RCB_ExcelTemp.SelectedValue));
                }
                return (ExcelTemplateInfo)ViewState["TempInfo"];
            }
            set
            {
                ViewState["TempInfo"] = value;
            }
        }

        /// <summary>
        /// 功     能:模板的选中
        /// 时     间:2010-11-09
        /// 作     者:蒋赛标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Rcb_ExcelTemp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ("00000000-0000-0000-0000-000000000000" == RCB_ExcelTemp.SelectedValue)
            {
                lab_Customer.Text = "";
                lab_Shipper.Text ="";
                lab_address.Text = "";
                lab_ContactPerson.Text = "";
                lab_des.Text = "";
            }
            else
            {
                var tempId = new Guid(RCB_ExcelTemp.SelectedValue);
                ExcelTemplateInfo eInfo = _temp.GetExcelTemplateInfo(tempId);
                lab_Customer.Text = eInfo.Customer;
                lab_Shipper.Text = eInfo.Shipper;
                lab_address.Text = eInfo.ContactAddress;
                lab_ContactPerson.Text = eInfo.ContactPerson;
                lab_des.Text = eInfo.Remarks;
            }
        }
        int _planTotal;
        protected void Rgd_GoodsPrint_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var dataItem = e.Item as GridDataItem;
                int planQuantity = int.Parse(dataItem.GetDataKeyValue("PlanQuantity").ToString());
                _planTotal += planQuantity;
            }
            var footerItem = e.Item as GridFooterItem;
            if (footerItem != null)
            {
                footerItem["PlanQuantity"].Text = string.Format("合计数量: {0}", _planTotal);
            }
        }

        protected void Cbx_PriceShow_CheckedChanged(object sender, EventArgs e)
        {
            Rgd_GoodsPrint.Columns[2].Visible = Cbx_PriceShow.Checked;
            Rgd_GoodsPrint.Rebind();
        }
    }
}

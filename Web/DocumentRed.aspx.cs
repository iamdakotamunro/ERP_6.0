using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Common;
using Telerik.Web.UI;
using System.Transactions;
using ERP.UI.Web.Base;

namespace ERP.UI.Web
{
    /// <summary>
    /// 单据红冲
    /// </summary>
    public partial class DocumentRed : BasePage
    {
        static readonly IDocumentRedDao _documentRedDao = new DocumentRedDao(GlobalConfig.DB.FromType.Write);
        static readonly ICompanyCussent _companyCussentDao = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindWarehouse();
                LoadRedType();
                BindDocumentType();
                BindState();
            }
        }

        #region 数据准备
        /// <summary>
        /// 绑定入库仓储
        /// </summary>
        private void BindWarehouse()
        {
            var wList = CurrentSession.Personnel.WarehouseList;
            ddl_Waerhouse.DataSource = wList;
            ddl_Waerhouse.DataTextField = "WarehouseName";
            ddl_Waerhouse.DataValueField = "WarehouseId";
            ddl_Waerhouse.DataBind();
            ddl_Waerhouse.Items.Insert(0, new ListItem("请选择", Guid.Empty.ToString()));
        }

        /// <summary>
        /// 红冲类型
        /// </summary>
        private void LoadRedType()
        {
            var list = EnumAttribute.GetDict<RedType>();
            ddl_RedType.DataSource = list;
            ddl_RedType.DataTextField = "Value";
            ddl_RedType.DataValueField = "Key";
            ddl_RedType.DataBind();
            ddl_RedType.Items.Insert(0, new ListItem("全部", ""));
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        private void BindDocumentType()
        {
            var list = EnumAttribute.GetDict<DocumentType>();
            ddl_DocumentType.DataSource = list;
            ddl_DocumentType.DataTextField = "Value";
            ddl_DocumentType.DataValueField = "Key";
            ddl_DocumentType.DataBind();
            ddl_DocumentType.Items.Insert(0, new ListItem("全部", "-1"));
        }

        /// <summary>
        /// 状态
        /// </summary>
        private void BindState()
        {
            var list = EnumAttribute.GetDict<DocumentRedState>();
            ddl_State.DataSource = list;
            ddl_State.DataTextField = "Value";
            ddl_State.DataValueField = "Key";
            ddl_State.DataBind();
            ddl_State.Items.Insert(0, new ListItem("全部", ""));
        }
        #endregion

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            Rgd_DocumentRed.DataBind();
        }

        #region 数据列表相关
        protected void RgdDocumentRed_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            long recordCount = 0;
            var startPage = Rgd_DocumentRed.CurrentPageIndex + 1;
            int pageSize = Rgd_DocumentRed.PageSize;
            var warehouseId = new Guid(ddl_Waerhouse.SelectedValue);
            DateTime startTime = string.IsNullOrEmpty(txt_StartTime.Text) ? DateTime.MinValue : DateTime.Parse(txt_StartTime.Text);
            DateTime endTime = string.IsNullOrEmpty(txt_EndTime.Text) ? DateTime.Now : DateTime.Parse(txt_EndTime.Text);
            int redType = 0;
            int documentType = -1;
            int state = 0;

            if (!string.IsNullOrEmpty(ddl_RedType.SelectedValue))
            {
                redType = int.Parse(ddl_RedType.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddl_DocumentType.SelectedValue))
            {
                documentType = int.Parse(ddl_DocumentType.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddl_State.SelectedValue))
            {
                state = int.Parse(ddl_State.SelectedValue);
            }
            var documentRedList = _documentRedDao.GetDocumentRedListToPage(warehouseId, startTime, endTime, redType, documentType, state, txt_NO.Text, startPage, pageSize, out recordCount);
            Rgd_DocumentRed.VirtualItemCount = (int)recordCount;
            Rgd_DocumentRed.DataSource = documentRedList;
        }

        #region 列表显示辅助方法
        //供应商
        protected string GetCompany(Guid thirdCompanyId)
        {
            var company = _companyCussentDao.GetCompanyCussent(thirdCompanyId);
            return company != null ? company.CompanyName : "-";
        }

        //所属仓库
        protected string GetWarehouse(Guid warehouseId)
        {
            var wList = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            return wList != null ? wList.WarehouseName : "-";
        }

        //获取物流配送公司
        protected string GetHostingFilialeId(Guid warehouseId, int storageType, Guid hostingFilialeId)
        {
            var wList = CurrentSession.Personnel.WarehouseList.FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (wList != null)
            {
                var slist = wList.Storages.FirstOrDefault(p => p.StorageType == storageType);
                if (slist != null)
                {
                    var hlist = slist.Filiales.FirstOrDefault(p => p.HostingFilialeId == hostingFilialeId);
                    return hlist == null ? "-" : hlist.HostingFilialeName;
                }
            }
            return "-";
        }
        #endregion

        //作废单据
        protected void imgbtn_Delete_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            var linkTradeId = ((ImageButton)sender).CommandName;
            var documentReds=_documentRedDao.GetDocumentRedInfoByLinkTradeId(new Guid(linkTradeId));
            if (documentReds == null || documentReds.Count==0)
            {
                MessageBox.Show(this, string.Format("获取关联红冲单据列表失败！"));
                return;
            }
            var current = documentReds.FirstOrDefault(act => act.DocumentType != (int) DocumentType.RedDocument);
            if (current == null)
            {
                MessageBox.Show(this, string.Format("对应的红冲单据不存在！"));
                return;
            }
            if (_companyFundReceipt.IsExistsByStockOrderNos(current.LinkTradeCode, ""))
            {
                MessageBox.Show(this, string.Format("对应的单据已生成收付款单据，不能删除！"));
                return;
            }

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (var documentRedInfo in documentReds)
                {
                    _documentRedDao.DelDocumentRedDetailByRedId(documentRedInfo.RedId);
                    _documentRedDao.DelDocumentRedByRedId(documentRedInfo.RedId);
                }
                ts.Complete();
            }
            Rgd_DocumentRed.Rebind();
        }
        #endregion
    }
}
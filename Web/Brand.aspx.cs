using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;
using System.Configuration;
using ERP.Environment;

namespace ERP.UI.Web
{
    public partial class BrandAw : BasePage
    {
        readonly IGoodsCenterSao _goodsBrandSao = new GoodsCenterSao();
        public string ResourceServerInformation = GlobalConfig.ResourceServerInformation;
        protected void Page_Load(object sender, EventArgs e)
        {
            IsHaveInformation = int.Parse(DDL_HaveInformation.SelectedValue);
        }

        protected int IsHaveInformation
        {
            get
            {
                if (ViewState["IsHaveInformation"] == null) ViewState["IsHaveInformation"] = -1;
                return int.Parse(ViewState["IsHaveInformation"].ToString());
            }
            set { ViewState["IsHaveInformation"] = value; }
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(BrandGrid, e);
        }

        protected void Lb_Refresh_Click(object sender, EventArgs e)
        {
            BrandGrid.Rebind();
        }

        protected void BrandGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<GoodsBrandInfo> list = IsHaveInformation == -1 ? _goodsBrandSao.GetAllBrandList().ToList()
                : _goodsBrandSao.GetBrandList(IsHaveInformation == 1).ToList();
            if (!string.IsNullOrEmpty(txt_Brand.Text.Trim()))
            {
                list = list.Where(p => p.Brand.Contains(txt_Brand.Text.Trim())).ToList();
            }
            BrandGrid.DataSource = list.OrderBy(w => w.OrderIndex).ToList();
        }

        protected void BrandGrid_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var dataItem = e.Item as GridDataItem;
            if (dataItem != null)
            {
                var brandId = new Guid(dataItem.GetDataKeyValue("BrandId").ToString());
                try
                {
                    string errorMessage;
                    var result = _goodsBrandSao.DeleteBrand(brandId, out errorMessage);
                    if (!result)
                    {
                        RAM.Alert("操作异常！" + errorMessage);
                    }
                }
                catch
                {
                    RAM.Alert("品牌属性信息删除失败！");
                }
            }
        }

        protected void BrandGrid_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == "SearchNo")
            {
                IsHaveInformation = 0;
                BrandGrid.Rebind();
            }
        }

        protected void txt_OrderIndex_OnTextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var dataItem = ((GridDataItem)textBox.Parent.Parent);
            if (dataItem != null)
            {
                try
                {
                    var brandId = new Guid(dataItem.GetDataKeyValue("BrandId").ToString());
                    var orderIndex = Convert.ToInt32(textBox.Text);
                    _goodsBrandSao.UpdateBrandOrderIndex(brandId, orderIndex);
                }
                catch (Exception ex)
                {
                    RAM.Alert("无效的操作。" + ex.Message);
                }
            }
            BrandGrid.Rebind();
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            IsHaveInformation = int.Parse(DDL_HaveInformation.SelectedValue);
            BrandGrid.Rebind();
        }
    }
}
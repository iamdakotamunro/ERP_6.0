using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class StockStatementForm : WindowsPage
    {
        private readonly IPurchasing _purchasing=new Purchasing(GlobalConfig.DB.FromType.Write);
        protected Guid PurchasingId
        {
            get
            {
                if (ViewState["purchasingId"] == null)
                {
                    ViewState["purchasingId"] = Guid.Empty;
                }
                return new Guid(ViewState["purchasingId"].ToString());
            }
            set { ViewState["purchasingId"] = value.ToString(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PurchasingId = new Guid(Request["PurchasingId"]);
                lab_Company.Text = _purchasing.GetPurchasingById(PurchasingId).CompanyName;
            }
        }

        protected void Rgd_StockStatement_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (Guid.Empty == PurchasingId)
            {
                return;
            }
            Rgd_StockStatement.DataSource = _purchasing.GetGoodsStockStatementByPurchacingID(PurchasingId);
        }

        protected void Bt_StockStatement_Click(object sender, EventArgs e)
        {
            string statement = tbx_AllStatement.Text;
            IList<Guid> glist = new List<Guid>();
            foreach (GridDataItem item in Rgd_StockStatement.Items)
            {
                var goodsid = new Guid(item.GetDataKeyValue("GoodsId").ToString());
                string str = ((TextBox)item.FindControl("tbx_Statement")).Text;
                if (string.IsNullOrEmpty(str))
                {
                    continue;
                }
                glist.Add(goodsid);
                _purchasing.SaveGoodsStatement(goodsid, str + " &#13");
            }
            if (!string.IsNullOrEmpty(statement))
            {
                IList<GoodsStatementInfo> list = _purchasing.GetGoodsStockStatementByPurchacingID(PurchasingId);
                foreach (var detail in list)
                {
                    if (glist.Contains(detail.GoodsId))
                    {
                        continue;
                    }
                    _purchasing.SaveGoodsStatement(detail.GoodsId, statement + " &#13");
                }
            }
            RAM.Alert("保存成功!");
            Rgd_StockStatement.Rebind();
        }
    }
}

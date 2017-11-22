using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AllianceShop.Contract.DataTransferObject;
using ERP.BLL.Implement.Order;
using ERP.BLL.Implement.Organization;
using ERP.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class CompleteOrderTaskDetailForm : Page
    {
        public Guid TaskID
        {
            get
            {
                if (ViewState["TaskID"] == null)
                    ViewState["TaskID"] = Request.QueryString["ID"];
                return new Guid(ViewState["TaskID"].ToString());
            }
        }

        public Dictionary<Guid, IList<ShopDTO>> DictShop
        {
            get
            {
                if (ViewState["DictShop"] == null)
                    ViewState["DictShop"] = new Dictionary<Guid, IList<ShopDTO>>();
                return ViewState["DictShop"] as Dictionary<Guid, IList<ShopDTO>>;
            }
            set { ViewState["DictShop"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void RGTaskDetails_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var list = CompleteOrderTaskManager.GetCompleteOrderTaskDetailsList(TaskID) ?? new List<CompleteOrderTaskDetailsInfo>();
            RGTaskDetails.DataSource = list.Where(w => !w.IsAllComplete).ToList();
        }

        protected void RGTaskDetails_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var info = e.Item.DataItem as CompleteOrderTaskDetailsInfo;
                if (info != null)
                {
                    if (!string.IsNullOrEmpty(info.Description))
                    {
                        var ibtnDescription = e.Item.FindControl("ibtnDescription") as ImageButton;
                        if (ibtnDescription != null)
                            ibtnDescription.Visible = true;
                    }
                    else
                    {
                        var lbTemp = e.Item.FindControl("lbTemp") as Label;
                        if (lbTemp != null)
                            lbTemp.Visible = true;
                    }
                }
            }
        }

        protected void RGTaskDetails_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "CompleteOrder")
            {
                var item = e.Item as GridDataItem;
                if (item != null)
                {
                    var orderId = new Guid(item.GetDataKeyValue("OrderId").ToString());
                    if (CompleteOrderTaskManager.SetCompleteOrderTaskDetail(orderId))
                        RGTaskDetails.Rebind();
                }
            }

        }
        protected void RAM_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RGTaskDetails, e);
        }

        public string GetSaleFilialeName(Guid filialeId)
        {
            return FilialeManager.GetName(filialeId);
        }

        //public string GetSalePlatformName(Guid filialeId, Guid salePlatformId)
        //{
        //    var info = CacheCollection.SalePlatform.Get(salePlatformId);
        //    if (info == null)
        //    {
        //        ShopDTO shopInfo = null;
        //        if (DictShop.ContainsKey(filialeId))
        //        {
        //            shopInfo = DictShop.FirstOrDefault(w => w.Key == filialeId).Value.FirstOrDefault(w => w.ShopID == salePlatformId);
        //        }
        //        else
        //        {
        //            var list = ShopSao.GetAllShop(filialeId);
        //            if (list != null && list.Any())
        //            {
        //                var tempDictShop = DictShop;
        //                tempDictShop.Add(filialeId, list);
        //                DictShop = tempDictShop;
        //                shopInfo = list.FirstOrDefault(w => w.ShopID == salePlatformId);
        //            }
        //        }
        //        return shopInfo == null ? string.Empty : shopInfo.ShopName;
        //    }
        //    return info.Name;
        //}
    }
}
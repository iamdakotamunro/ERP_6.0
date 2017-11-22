using System;
using System.Collections.Generic;
using System.Linq;
using AllianceShop.Contract.DataTransferObject;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using MIS.Enum.Attributes;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class ShopRechargeManager : BasePage
    {

        public IList<FilialeInfo> ShopList
        {
            get
            {
                if (ViewState["ShopList"] == null)
                    return new List<FilialeInfo>();
                return (IList<FilialeInfo>)ViewState["ShopList"];
            }
            set { ViewState["ShopList"] = value; }
        }

        public Guid ParentID
        {
            get
            {
                if (ViewState["ParentID"] == null)
                {
                    var shopInfo=ShopList.FirstOrDefault(act => act.ParentId != Guid.Empty);
                    return shopInfo != null ? shopInfo.ParentId : Guid.Empty;
                }
                return (Guid)ViewState["ParentID"];
            }
            set { ViewState["ParentID"] = value; }
        }

        public Guid? ShopId
        {
            get
            {
                if (ViewState["ShopId"] == null)
                    return null;
                return (Guid)ViewState["ShopId"];
            }
            set { ViewState["ShopId"] = value; }
        }

        /// <summary>
        /// 店铺类型
        /// </summary>
        public int JoinType
        {
            get
            {
                if (ViewState["JoinType"] == null)
                    return -1;
                return (int)ViewState["JoinType"];
            }
            set { ViewState["JoinType"] = value; }
        }

        /// <summary>
        /// 查询关键字
        /// </summary>
        public string SearchKey
        {
            get
            {
                if (ViewState["SearchKey"] == null)
                    return string.Empty;
                return ViewState["SearchKey"].ToString();
            }
            set { ViewState["SearchKey"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var filialeList = MISService.GetAllFiliales();
                ShopList = filialeList.Where(w => w.FilialeTypes.Contains((int)FilialeType.EntityShop) && w.IsActive).ToList();
                BindShopType();
                BindShopList(RcbSaleFiliale,null);
            }
        }

        /// <summary>
        /// 数据绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgExchangedApplyListNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if(!IsPostBack)
            {
                RgRechargeList.DataSource = new List<RechargeDTO>();
            }
            else
            {
                //int pageSize = RgRechargeList.PageSize;
                //int pageIndex = RgRechargeList.CurrentPageIndex + 1;
                var rechargeState = Convert.ToInt32(RcbRechargeState.SelectedValue);
                var now = DateTime.Now;
                var sTime = RdpStartTime.SelectedDate ?? new DateTime(now.Year, now.Month, now.Day).AddDays(-30);
                var eTime = RdpEndTime.SelectedDate == null ? DateTime.Now : Convert.ToDateTime(RdpEndTime.SelectedDate).AddDays(1).AddSeconds(-1);
                var result = ShopSao.SelectRechargeListByParentId(ParentID, ShopId, rechargeState,JoinType, sTime, eTime, null, null);
                if (result == null)
                {
                    RAM.Alert("服务连接失败");
                    return;
                }
                if (result.IsSuccess)
                {
                    RgRechargeList.DataSource = result.Data;
                    RgRechargeList.VirtualItemCount = result.Total;
                }
                else
                    RAM.Alert(result.Message);
            }
            
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RgRechargeList, e);
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            RgRechargeList.Rebind();
        }

        public string ReturnType(int type)
        {
            var str = "";
            switch (type)
            {
                case 10:
                    str = "线上支付";
                    break;
                case 20:
                    str = "线下支付";
                    break;
            }
            return str;
        }
        public string ReturnRName(int rState)
        {
            var str = "";
            switch (rState)
            {
                case 0:
                    str = "待确认";
                    break;
                case 1:
                    str = "已支付";
                    break;
                case 2:
                    str = "拒绝";
                    break;
            }
            return str;
        }

        protected string ShowShopRechargeManager(object eval)
        {
            return string.Format(@"return ShowRefundForm('{0}','{1}')", eval, ParentID);
        }

        protected void RcbSaleFilialeItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            //获取关键字
            var searchKey = e.Text.Trim();
            if (string.IsNullOrEmpty(searchKey))
            {
                return;
            }
            BindShopList(combo,searchKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rcb"></param>
        /// <param name="searchKey"></param>
        protected void BindShopList(RadComboBox rcb,string searchKey)
        {
            var filialeList = ShopList.Where(w => w.Rank == (int)FilialeRank.Partial).OrderBy(f => f.OrderIndex).ToList();
            #region 新需求，门店选择添加所有并允许按店铺类型进行搜索  add by liangcanren  at 2015-04-14  
            if (JoinType!=-1)
            {
                filialeList = filialeList.Where(act => act.ShopJoinType == JoinType).ToList();
            }
            filialeList.Insert(0,new FilialeInfo{ID = Guid.Empty,Name = "全部"});
            #endregion
            if (!string.IsNullOrEmpty(searchKey))
            {
                filialeList = filialeList.Where(act =>act.Name.Contains(searchKey)).ToList();
            }
            var dics= filialeList.ToDictionary(dic => dic.ID, dic => dic.Name);
            rcb.DataSource = dics;
            rcb.DataBind();
            rcb.SelectedIndex=0;
        }

        #region   新需求，充值查询添加店铺类型搜索  add by liangcanren  at 2015-04-14
        /// <summary>
        /// 绑定店铺类型
        /// </summary>
        protected void BindShopType()
        {
            var dics = new Dictionary<int, string>
            {
                {-1,"所有类型"}
            };
            var shopTypeList=EnumArrtibute.GetDict<ShopJoinType>();
            if (shopTypeList!=null)
            {
                foreach (var shopType in shopTypeList)
                {
                    dics.Add(shopType.Key, shopType.Value);
                }
            }
            RcbShopType.DataSource = dics;
            RcbShopType.DataBind();
        }

        /// <summary>
        /// 店铺类型选择
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RcbShopTypeSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (RcbShopType.SelectedItem != null && !string.IsNullOrEmpty(RcbShopType.SelectedItem.Value))
                JoinType = Convert.ToInt32(RcbShopType.SelectedItem.Value);
            else
            {
                JoinType = -1;
            }
            ShopId = Guid.Empty;
            RcbSaleFiliale.Items.Clear();
            BindShopList(RcbSaleFiliale, SearchKey);
        }
        #endregion

        protected void SearchIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (RcbSaleFiliale.SelectedItem != null && !string.IsNullOrEmpty(RcbSaleFiliale.SelectedItem.Value))
            {
                if (RcbSaleFiliale.SelectedItem.Value == Guid.Empty.ToString())
                {
                    ParentID = Guid.Empty;
                    ShopId = null;
                }
                else
                {
                    ShopId = new Guid(RcbSaleFiliale.SelectedItem.Value);
                    var shopInfo = ShopList.FirstOrDefault(act => act.ID == ShopId);
                    ParentID = shopInfo!=null?shopInfo.ParentId:Guid.Empty;
                }
            }
            else
            {
                ShopId = null;
                ParentID = Guid.Empty;
            }
        } 
    }
}
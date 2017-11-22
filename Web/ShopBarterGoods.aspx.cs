using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Organization;
using ERP.BLL.Implement.Shop;
using ERP.Enum.Attribute;
using ERP.Enum.ShopFront;
using ERP.Model.ShopFront;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using MIS.Enum;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class ShopBarterGoods : BasePage
    {
        private static readonly IGoodsCenterSao _goodsCenterSao=new GoodsCenterSao();

        #region  状态保存

        /// <summary>
        /// 门店退换货申请状态列表
        /// </summary>
        protected  IDictionary<int, string> ApplyStateList
        {
            get
            {
                if (ViewState["ApplyStateList"]==null)
                    return EnumAttribute.GetDict<ExchangedState>().Where(act => act.Key != (int)ExchangedState.Returning 
                        && act.Key != (int)ExchangedState.ReturnEnd)
                        .ToDictionary(k => k.Key, v => v.Value); 
                return (Dictionary<int, string>)ViewState["ApplyStateList"];
            }
            set { ViewState["ApplyStateList"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ApplyStateList = EnumAttribute.GetDict<ExchangedState>().Where(act => act.Key != (int)ExchangedState.Returning
                        && act.Key != (int)ExchangedState.ReturnEnd)
                        .ToDictionary(k => k.Key, v => v.Value);
                BindShopList();
                BindExchangedState();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgExchangedApplyListNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if(!IsPostBack)
            {
                RgExchangedApplyList.DataSource = new List<ShopExchangedApplyInfo>();
            }
            else
            {
                Guid shopId = string.IsNullOrEmpty(RcbShopList.SelectedValue)
                              ? Guid.Empty
                              : new Guid(RcbShopList.SelectedValue);
                if (shopId == Guid.Empty)
                {
                    RAM.Alert("请选择具体店铺!");
                    return;
                }
                var goodsIds = new List<Guid>();
                if (!string.IsNullOrEmpty(RtbSearchKey.Text))
                {
                    var dics = _goodsCenterSao.GetGoodsSelectList(RtbSearchKey.Text);
                    if (dics != null && dics.Count > 0)
                    {
                        goodsIds.AddRange(dics.Select(act => new Guid(act.Key)));
                    }
                }
                DateTime startTime = RdpStartTime.SelectedDate ?? DateTime.MinValue;
                DateTime endTime = RdpEndTime.SelectedDate ?? DateTime.Now;

                int state = string.IsNullOrEmpty(RcbApplyState.SelectedValue)
                                  ? -1
                                  : Convert.ToInt32(RcbApplyState.SelectedValue);
                RgExchangedApplyList.DataSource = ShopExchangedApplyBll.ReadInstance.GetShopExchangedApplyList(false, RtbApplyNo.Text, startTime, endTime, goodsIds, shopId, state, RtbSearchKey.Text);
            }
        }

        protected void RgExchangedApplyListItemDataBound(object sender, GridItemEventArgs e)
        {
            
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RgExchangedApplyList, e);
        }

        #region  打开窗口时调用

        /// <summary>
        /// 显示换货货详情
        /// </summary>
        /// <param name="eval"></param>
        /// <param name="isCheck"> </param>
        /// <returns></returns>
        protected string ShowBarterFormJs(object eval,object isCheck)
        {
            return string.Format("return ShowBarterForm('{0}','{1}')", eval,isCheck);
        }

        /// <summary>
        /// 换货发货填写物流
        /// </summary>
        /// <param name="eval"></param>
        /// <returns></returns>
        protected string ShowExpressFormJs(object eval)
        {
            return string.Format("return ShowExpressForm('{0}')", eval);
        }
        #endregion

        /// <summary>
        /// 绑定联盟店铺
        /// </summary>
        protected void BindShopList()
        {
            IDictionary<Guid, string> dics = FilialeManager.GetAllianceFilialeList()
                .Where(act=>act.Rank==(int)FilialeRank.Partial)
                .ToDictionary(k=>k.ID,v=>v.Name);
            RcbShopList.DataSource = dics;
            RcbShopList.DataBind();
            RcbShopList.Items.Insert(0,new RadComboBoxItem("",string.Format("{0}",Guid.Empty)));
        }

        /// <summary>
        /// 绑定退货状态
        /// </summary>
        protected void BindExchangedState()
        {
            RcbApplyState.DataSource = ApplyStateList;
            RcbApplyState.DataBind();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSearchOnClick(object sender, EventArgs e)
        {
            RgExchangedApplyList.Rebind();
        }

        protected void RcbShopListItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            IDictionary<Guid, string> dics = FilialeManager.GetAllianceFilialeList()
                .Where(act => act.Rank == (int)FilialeRank.Partial).ToDictionary(k => k.ID, v => v.Name);
            var combo = (RadComboBox)o;
            combo.Items.Clear();
            var list = !string.IsNullOrEmpty(e.Text) && e.Text.Length >= 1 ? dics.Where(act => act.Value.Contains(e.Text))
                : dics;
            var keyValuePairs = list as KeyValuePair<Guid, string>[] ?? list.ToArray();
            if (e.NumberOfItems >= keyValuePairs.Count())
                e.EndOfItems = true;
            else
            {
                foreach (var item in keyValuePairs)
                {
                    var rcb = new RadComboBoxItem
                    {
                        Text = item.Value,
                        Value = item.Key + "",
                    };
                    combo.Items.Add(rcb);
                }
            }
        }
    }
}
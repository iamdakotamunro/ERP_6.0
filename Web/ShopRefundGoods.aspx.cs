using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Organization;
using ERP.BLL.Implement.Shop;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IShop;
using ERP.Enum.Attribute;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using MIS.Enum;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class ShopRefundGoods : BasePage
    {
        private static readonly IGoodsCenterSao _goodsCenterSao=new GoodsCenterSao();
        private static readonly IShopRefundMessage _shopRefund = new ShopRefundMessageDal(GlobalConfig.DB.FromType.Read);

        #region  状态保存

        /// <summary>
        /// 门店退换货申请状态列表 
        /// </summary>
        protected IDictionary<int, string> ApplyStateList
        {
            get
            {
                if (ViewState["ApplyStateList"] == null)
                    return EnumAttribute.GetDict<ExchangedState>().Where(act => act.Key != (int)ExchangedState.Bartering 
                        && act.Key != (int)ExchangedState.BarterEnd)
                        .ToDictionary(k=>k.Key,v=>v.Value);
                return (Dictionary<int, string>)ViewState["ApplyStateList"];
            }
            set { ViewState["ApplyStateList"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                BindExchangedState();
                BindShopList();
                BtnReturnMsg.Text = MessageText();
            }
        }

        #region  数据绑定
        /// <summary>
        /// 绑定退货状态
        /// </summary>
        protected void BindExchangedState()
        {
            RcbApplyState.DataSource = ApplyStateList.Where(act => act.Key != (int)ExchangedState.Returning && act.Key != (int)ExchangedState.Checked);
            RcbApplyState.DataBind();
        }

        /// <summary>
        /// 绑定联盟店铺
        /// </summary>
        protected void BindShopList()
        {
            IDictionary<Guid, string> dics = FilialeManager.GetAllianceFilialeList()
                .Where(act => act.Rank == (int)FilialeRank.Partial)
                .ToDictionary(k => k.ID, v => v.Name);
            RcbShopList.DataSource = dics;
            RcbShopList.DataBind();
            RcbShopList.Items.Insert(0,new RadComboBoxItem("",string.Format("{0}",Guid.Empty)));
        }
        #endregion

        protected void RgRefundApplyListNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if(!IsPostBack)
            {
                //默认打开显示所有门店的待审核单据
                //modify by lcj at 2015.8.26
                RgRefundApplyList.DataSource = ShopExchangedApplyBll.ReadInstance.GetShopExchangedApplyList(true, string.Empty, DateTime.MinValue,
                    DateTime.Now, new List<Guid>(), Guid.Empty, (int)ExchangedState.CheckPending, RtbSearchKey.Text);
            }
            else
            {
                Guid shopId = string.IsNullOrEmpty(RcbShopList.SelectedValue)
                                 ? Guid.Empty
                                 : new Guid(RcbShopList.SelectedValue);
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
                if (shopId == Guid.Empty)
                {
                    state = (int) ExchangedState.CheckPending;
                }
                RgRefundApplyList.DataSource = ShopExchangedApplyBll.ReadInstance.GetShopExchangedApplyList(true, RtbApplyNo.Text, startTime, endTime, goodsIds, shopId, state, RtbSearchKey.Text);
            }
        }

        protected void RgRefundApplyListItemDataBound(object sender, GridItemEventArgs e)
        {
            
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RgRefundApplyList, e);
        }

        /// <summary>
        /// 获取未审核的退货留言数量
        /// </summary>
        /// <returns></returns>
        protected string MessageText()
        {
            try
            {
                var count = _shopRefund.GetMessageCount(Guid.Empty, (int)ReturnMsgState.CheckPending);
                BtnReturnMsg.Enabled = count > 0;
                return string.Format("退货留言({0})", count);
            }
            catch (Exception ex)
            {
                RAM.Alert(ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// 如果没有退货留言给出提示，有弹出页面窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnReturnMsgOnClick(object sender, EventArgs e)
        {
            try
            {
                var count = _shopRefund.GetMessageCount(Guid.Empty, (int)ReturnMsgState.CheckPending);
                BtnReturnMsg.Text = MessageText();
                if(count>0)
                {
                    RAM.ResponseScripts.Add("return ShowMsgForm();");
                }
            }
            catch (Exception ex)
            {
                RAM.Alert(ex.Message);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSearchOnClick(object sender, EventArgs e)
        {
            Guid shopId = string.IsNullOrEmpty(RcbShopList.SelectedValue)
                                 ? Guid.Empty
                                 : new Guid(RcbShopList.SelectedValue);
            if (shopId == Guid.Empty)
            {
                RAM.Alert("请选择具体店铺!");
                return;
            }
            BtnReturnMsg.Text = MessageText();
            RgRefundApplyList.Rebind();
        }

        #region  窗口打开时调用

        /// <summary>
        /// 显示退货详情
        /// </summary>
        /// <param name="eval"></param>
        /// <param name="isCheck"> </param>
        /// <returns></returns>
        protected string ShowRefundDetailJs(object eval,object isCheck)
        {
            return string.Format(@"return ShowRefundForm('{0}','{1}')", eval,isCheck);
        }
        #endregion

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
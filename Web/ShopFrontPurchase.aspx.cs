using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Organization;
using ERP.BLL.Implement.Shop;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IShop;
using ERP.Enum.Attribute;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model.ShopFront;
using MIS.Enum;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    public partial class ShopFrontPurchase : BasePage
    {
        private readonly IApplyStockDAL _applyStockDal = new ApplyStockDAL(GlobalConfig.DB.FromType.Read);
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Read);

        #region 页面加载
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindApplyState();
            }
        }
        #endregion

        #region 状态保存
        /// <summary>
        /// 店铺列表
        /// </summary>
        protected IDictionary<Guid, string> ShopFilialeList
        {
            get
            {
                if (ViewState["ShopFilialeList"] == null)
                {
                    ViewState["ShopFilialeList"] = GetFilialeList();
                }
                return ViewState["ShopFilialeList"] as IDictionary<Guid, string>;
            }
        }

        /// <summary>
        /// zhangfan added
        /// 门店采购申请状态
        /// </summary>
        private IEnumerable<KeyValuePair<int, string>> ApplyStockStateList
        {
            get
            {
                if (ViewState["ShopApplyStateList"] == null)
                {
                    ViewState["ShopApplyStateList"] = GetApplyStockStateList();
                }
                return ViewState["ShopApplyStateList"] as IDictionary<int, string>;
            }
        }

        /// <summary>
        /// 是否要求选择店铺
        /// </summary>
        public bool SelectedShopId
        {
            get
            {
                return ViewState["SelectedShopId"] != null && Convert.ToBoolean(ViewState["SelectedShopId"]);
            }
            set { ViewState["SelectedShopId"] = value; }
        }
        #endregion

        #region  获取数据

        /// <summary>
        /// 获取门店分公司列表
        /// </summary>
        /// <returns></returns>
        private IDictionary<Guid, string> GetFilialeList()
        {
            var list = CacheCollection.Filiale.GetShopList()
                .Where(ent => ent.Rank == (int)FilialeRank.Partial && ent.IsActive);
            return list.ToDictionary(item => item.ID, item => item.Name);
        }

        /// <summary>
        /// zhangfan added
        /// 获取门店采购申请状态
        /// </summary>
        /// <returns></returns>
        private IDictionary<int, string> GetApplyStockStateList()
        {
            return EnumAttribute.GetDict<ApplyStockState>();
        }

        /// <summary>
        /// zhangfan added
        /// 返回采购状态描述
        /// </summary>
        /// <param name="enumKey"></param>
        /// <returns></returns>
        protected string ReturnApplyState(object enumKey)
        {
            int key;
            if (enumKey != null && int.TryParse(enumKey.ToString(), out key))
                return ApplyStockStateList.FirstOrDefault(ent => ent.Key == key).Value;
            return string.Empty;
        }

        /// <summary>
        /// 得到采购类型
        /// </summary>
        /// <param name="purchaseType"></param>
        /// <returns></returns>
        protected string GetPurchaseTypeName(object purchaseType)
        {
            if (purchaseType == null || purchaseType.ToString() == "0" || string.IsNullOrEmpty(purchaseType.ToString()))
            {
                return "-";
            }
            return EnumAttribute.GetKeyName((PurchaseType)purchaseType);
        }
        #endregion

        #region RadGrid 控件绑定事件
        /// <summary>
        /// 门店采购申请数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgApplyStockListNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Guid filialeID = string.IsNullOrEmpty(RcbShopFrontList.SelectedValue) ? Guid.Empty : new Guid(RcbShopFrontList.SelectedValue);
            int applyStockState = string.IsNullOrEmpty(RcbApplyState.SelectedValue) ? -1 : Convert.ToInt32(RcbApplyState.SelectedValue);
            List<int> states = !IsPostBack
                                   ? new List<int> { (int)ApplyStockState.Applying, (int)ApplyStockState.Delivering }
                                   : applyStockState == -1 ? new List<int>() : new List<int> { applyStockState };
            if (applyStockState == -1)
            {
                if (IsPostBack && (string.IsNullOrEmpty(RcbShopFrontList.SelectedValue) || RcbShopFrontList.SelectedValue == Guid.Empty.ToString()))
                {
                    states =
                            ApplyStockStateList.Where(act => act.Key >= (int)ApplyStockState.Obligation).Select(
                                act => act.Key).ToList();
                }
            }
            var goodsIds = new List<Guid>();
            if (!string.IsNullOrEmpty(RtbGoodsName.Text))
            {
                goodsIds = _goodsCenterSao.GetRealGoodsIdListByGoodsNameOrCode(RtbGoodsName.Text).ToList();
            }
            string tradeCode = RtbSearchKey.Text.Trim();
            DateTime startTime = RdpStartTime.SelectedDate ?? DateTime.MinValue;
            DateTime endTime = RdpEndTime.SelectedDate ?? DateTime.MinValue;
            int purchaseType = int.Parse(RcbPurchaseType.SelectedValue);

            if (endTime <= DateTime.MinValue)
            {
                endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            if (startTime <= DateTime.MinValue || startTime >= endTime)
            {
                startTime = endTime.AddMonths(-6);
            }
            endTime = endTime.AddDays(1).AddMinutes(-1);
            var dataList = _applyStockDal.GetApplyStockList(filialeID, purchaseType, states, startTime, endTime,
                                                            tradeCode, goodsIds);
            RgApplyStockList.DataSource = dataList;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSearchClick(object sender, EventArgs e)
        {
            RgApplyStockList.Rebind();
        }

        /// <summary>
        /// item绑定事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgApplyStockListItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                if (e.Item.RowIndex > 1)
                {
                    var applyinfo = (ApplyStockInfo)e.Item.DataItem;
                    if (applyinfo.StockState != (int)ApplyStockState.Finish
                        && applyinfo.StockState != (int)ApplyStockState.Finished)
                    {
                        var applygoodsList = ApplyStockBLL.ReadInstance.FindDetailList(applyinfo.ApplyId);
                        if (applygoodsList == null || applygoodsList.Count == 0) return;
                        var splits = applyinfo.SemiStockCode.Split(',');
                        var flag = splits.Length > 0 && splits.Any(act => act.Length > 0 && !act.Contains("作废"));
                        int shortageCount = applygoodsList.Count(
                            applyStockDetailInfo => (applyStockDetailInfo.GoodsStock - (flag ? 0 : applyStockDetailInfo.Quantity)) < 0);
                        if (shortageCount == applygoodsList.Count)   //采购单中所有商品都不能满足
                        {
                            e.Item.Style.Add("background-color", "#FF6666");//红色
                        }
                        else if (shortageCount < applygoodsList.Count && shortageCount > 0)   //采购单中部分商品不能满足
                        {
                            e.Item.Style.Add("background-color", "#FFFFCC");//黄色
                        }
                        else if (shortageCount == 0)    //采购单中所有商品都满足
                        {
                            e.Item.Style.Add("background-color", "#CCFF99");//绿色
                        }
                    }
                    else
                    {
                        e.Item.Style.Add("background-color", "white");//白色
                    }
                }
            }
        }
        #endregion

        #region RadComboBox 下拉列表控件数据绑定与Changed事件
        /// <summary>
        /// 绑定申请店铺
        /// </summary>
        protected void BindApplyShop(int isJoin)
        {
            Dictionary<Guid, string> shopFilialeList = CacheCollection.Filiale.GetShopList()
                    .Where(act => act.Rank == (int)FilialeRank.Partial && act.ShopJoinType == isJoin && act.IsActive).ToDictionary(k => k.ID, v => v.Name);
            SelectedShopId = shopFilialeList.Count > 10;
            RcbShopFrontList.DataSource = shopFilialeList;
            RcbShopFrontList.DataBind();
            RcbShopFrontList.Items.Insert(0, new RadComboBoxItem("", string.Format("{0}", Guid.Empty)));
        }

        /// <summary>
        /// 绑定采购状态
        /// </summary>
        protected void BindApplyState()
        {
            RcbApplyState.DataSource = ApplyStockStateList.Where(act => act.Key >= (int)ApplyStockState.Obligation).ToDictionary(act => act.Key, a => a.Value);
            RcbApplyState.DataBind();
            RcbApplyState.Items.Insert(0, new RadComboBoxItem("全部", string.Empty));
            if (!IsPostBack)
            {
                RcbApplyState.SelectedValue = string.Format("{0}", (int)ApplyStockState.Applying);
            }
        }

        /// <summary>
        /// 店铺类型选择
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RcbShopTypeIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var obj = o as RadComboBox;
            if (obj != null)
            {
                RcbShopFrontList.Text = "";
                if (!string.IsNullOrEmpty(obj.SelectedValue) && obj.SelectedValue != "0")
                {
                    BindApplyShop(Convert.ToInt32(obj.SelectedValue));
                }
                else
                {
                    RcbApplyState.DataSource = new Dictionary<Guid, string>();
                    RcbApplyState.DataBind();

                    RcbShopFrontList.DataSource = new Dictionary<Guid, string>();
                    RcbShopFrontList.DataBind();
                }
            }
        }
        #endregion

        #region   JS调用

        /// <summary>
        /// 调用js，打开申请出库窗口
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="filialeId"> </param>
        /// <returns></returns>
        protected string ReturnApplyStockJs(object applyId, object filialeId)
        {
            return string.Format(@"return ShowApplyForm('{0}')", applyId);
        }

        /// <summary>
        /// 调用js，打开确认窗口
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        protected string ShopConfirmFormJs(object applyId)
        {
            return string.Format(@"return ShopConfirmForm('{0}')", applyId);
        }

        /// <summary>
        /// 打开采购单详细
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="isAudit"> </param>
        protected string ShowApplyStockJs(object applyId, object isAudit)
        {
            return string.Format(@"return ShowApplyDetailForm('{0}','{1}')", applyId, isAudit);
        }

        /// <summary>
        /// 打开采购单详细
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="applyNo"> </param>
        protected string ShowLogisticalJs(object applyId, object applyNo)
        {
            return string.Format(@"return ShowLogisticalForm('{0}','{1}')", applyId, applyNo);
        }
        #endregion

        #region[页面刷新请求]
        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RgApplyStockList, e);
        }
        #endregion

        /// <summary>
        /// 采购申请单作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApplyStockCacel(object sender, EventArgs e)
        {
            var lk = (LinkButton)sender;
            if (lk != null)
            {
                var applyId = new Guid(lk.CommandArgument);
                string errorMsg;
                //将采购申请状态设置为交易关闭
                var result = ApplyStockBLL.WriteInstance.UpdateStockStateErp(applyId, (int)ApplyStockState.Close, out errorMsg);
                if (!result)
                {
                    RAM.Alert(errorMsg);
                }
                RgApplyStockList.Rebind();
            }
        }

        /// <summary>
        /// 待确认的采购申请审核
        /// (确认采购单是否符合发货标准)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApplyStockCommit(object sender, EventArgs e)
        {
            var lk = (LinkButton)sender;
            if (lk != null)
            {
                var applyId = new Guid(lk.CommandArgument);
                string errorMsg;
                var result = ApplyStockBLL.WriteInstance.UpdateStockStateErp(applyId, (int)ApplyStockState.Delivering, out errorMsg);
                if (!result)
                {
                    RAM.Alert(errorMsg);
                }
                //是否添加操作记录，待定
                RgApplyStockList.Rebind();
            }
        }

        /// <summary>
        /// 店铺的模糊搜索
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RcbShopFrontListItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            IDictionary<Guid, string> dics = ShopFilialeList;
            if (!string.IsNullOrEmpty(RcbShopType.SelectedValue) && RcbShopType.SelectedValue == "0")
            {
                dics = FilialeManager.GetAllianceFilialeList()
                    .Where(act => act.Rank == (int)FilialeRank.Partial).ToDictionary(k => k.ID, v => v.Name);
            }
            var combo = (RadComboBox)o;
            combo.Items.Clear();
            var list = !string.IsNullOrEmpty(e.Text) && e.Text.Length >= 1
                ? dics.Where(act => act.Value.Contains(e.Text)) : dics;
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
                        Value = string.Format("{0}", item.Key),
                    };
                    combo.Items.Add(rcb);
                }
            }
        }


        /// <summary>
        /// 判断公司是否为门店
        /// modify by liangcanren at 2015-03-26
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        protected bool IsAlliance(object shopId, object tradeCode)
        {
            var splits = tradeCode.ToString().Split(',');
            return CacheCollection.Filiale.IsShop(new Guid(shopId.ToString())) && (splits.Length == 0 || splits.All(act => string.IsNullOrEmpty(act) || act.Contains("(作废)")));
        }

        /// <summary>
        /// 是否允许作废
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <returns></returns>
        public bool IsAllowCancel(string tradeCode)
        {
            if (string.IsNullOrEmpty(tradeCode))
            {
                return true;
            }
            var tradecodes = tradeCode.Split(',');
            //var flag = _storageManager.IsEixstsTradeCode(tradeCode);
            //if (!flag)
            //    return false;
            return tradecodes.All(tradecode => tradecode.Length <= 0 || tradecode.Contains("(作废)"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI;
using ERP.BLL.Implement.Shop;
using ERP.DAL.Implement.Order;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IOrder;
using ERP.DAL.Interface.IShop;
using ERP.Enum;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.Model.ShopFront;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class ShowExchangedDetailForm : Page
    {
        //private readonly ShopExchangedApplyDetailBll _applyDetailBll=new ShopExchangedApplyDetailBll();
        //private readonly ShopExchangedApplyBll _applyBll = new ShopExchangedApplyBll();
        //private readonly CheckRefund _checkRefund=new CheckRefund();
        private readonly IShopExchangedApplyDetail _shopApplyDetail=new ShopExchangedApplyDetailDal(GlobalConfig.DB.FromType.Read);
        private readonly IShopExchangedApply _shopApply = new ShopExchangedApplyDal(GlobalConfig.DB.FromType.Read);
        private readonly ICheckRefund _refundDal = new CheckRefund(GlobalConfig.DB.FromType.Read);
        /// <summary>
        /// 换货列表
        /// </summary>
        protected IList<ShopExchangedApplyDetailInfo> BarterDetailList
        {
            get
            {
                if (ViewState["BarterDetailList"] == null)
                    return null;
                return (List<ShopExchangedApplyDetailInfo>)ViewState["BarterDetailList"];
            }
            set { ViewState["BarterDetailList"] = value; }
        }

        /// <summary>
        /// 退货列表
        /// </summary>
        protected List<ShopApplyDetailInfo> RefundDetailList
        {
            get
            {
                if (ViewState["RefundDetailList"] == null)
                    return null;
                return (List<ShopApplyDetailInfo>)ViewState["RefundDetailList"];
            }
            set { ViewState["RefundDetailList"] = value; }
        }

        /// <summary>
        /// 商品检查列表
        /// </summary>
        protected List<CheckRefundDetailInfo> CheckRefundDetailList
        {
            get
            {
                if (ViewState["CheckRefundDetailList"] == null)
                    return null;
                return (List<CheckRefundDetailInfo>)ViewState["CheckRefundDetailList"];
            }
            set { ViewState["RefundDetailList"] = value; }
        }

        /// <summary>
        /// 商品检查列表
        /// </summary>
        protected CheckRefundInfo RefundInfo
        {
            get
            {
                if (ViewState["RefundInfo"] == null)
                    return null;
                return (CheckRefundInfo)ViewState["RefundInfo"];
            }
            set { ViewState["RefundInfo"] = value; }
        }

        /// <summary>
        /// 退换货申请Id
        /// </summary>
        protected Guid ApplyId
        {
            get { 
                return string.IsNullOrEmpty(Request.QueryString["ApplyId"]) ? Guid.Empty 
                : new Guid(Request.QueryString["ApplyId"]); 
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["Type"]))
                {
                    int type = Convert.ToInt32(Request.QueryString["Type"]);
                    var applyInfo = _shopApply.GetShopExchangedApplyInfo(ApplyId);
                    if(applyInfo==null)return;
                    //var isDisplay = applyInfo.ExchangedState == (int)ExchangedState.Checking;
                    var isCheck = !string.IsNullOrEmpty(Request["IsCheck"]) && Request["IsCheck"] == "1";
                    //显示检查（待审核，且非退货详情查询）
                    ShowCheckRefund.Visible = false;
                    //显示审核（待审核，且非退货详情查询）
                    ShowAudit.Visible = applyInfo.ExchangedState == (int) ExchangedState.CheckPending;
                    LbApplyNoText.Text = string.Format("{0}货单号", type == 0 ? "换" : "退");
                    LbApplyTimeText.Text = string.Format("{0}货时间", type == 0 ? "换" : "退");
                    LbApplyNoValue.Text = applyInfo.ApplyNo;
                    LbApplyTimeValue.Text = applyInfo.ApplyTime.ToString("yyyy-MM-dd HH:mm:ss");
                    RgBarterGoodsList.Visible = type == 0;
                    RgRefundGoodsList.Visible = type == 1;

                    //不添加检查
                    //RgBarterGoodsList.Columns.FindByUniqueName("SumQuantity").Visible = isDisplay && isCheck;
                    //RgBarterGoodsList.Columns.FindByUniqueName("ReturnCount").Visible = isDisplay && isCheck;
                    if (isCheck)  //显示检查div和数据列
                    {
                        //获取
                        var refundList = _refundDal.GetShopCheckRefundList(applyInfo.ShopID, applyInfo.ApplyID,
                                                                             applyInfo.ApplyNo);
                        if (refundList != null && refundList.Count > 0)
                        {
                            var checkRefundInfo = refundList.FirstOrDefault(act => act.CheckState == (int)CheckState.Refuse);
                            if(checkRefundInfo!=null)
                            {
                                RefundInfo = checkRefundInfo;
                                CheckRefundDetailList = _refundDal.GetCheckRefundDetailList(checkRefundInfo.RefundId).ToList();
                            }
                        }
                    }
                    if(type==0) //换货
                    {
                        
                        var dataList = _shopApplyDetail.GetShopExchangedApplyDetailList(ApplyId).OrderBy(act => act.GoodsName).ThenBy(act => act.Specification);
                        var showBarterList = new List<ShopExchangedApplyDetailInfo>();
                        //var dics=new Dictionary<Guid, int>();
                        var dics = new List<DicChangeGoods>();
                        foreach (var shopExchangedApplyDetailInfo in dataList)
                        {
                            var detailInfo =showBarterList.FirstOrDefault
                                (act => act.GoodsID == shopExchangedApplyDetailInfo.GoodsID 
                                    && act.BarterGoodsID==shopExchangedApplyDetailInfo.BarterGoodsID);
                            if(detailInfo!=null)
                            {
                                //if (dics[shopExchangedApplyDetailInfo.GoodsID]%3==0)//如果一个商品换两个不同的商品则会报错插入重复键 modify by lcj at 2015.9.25
                                var info = dics.FirstOrDefault(d => d.GoodsId == shopExchangedApplyDetailInfo.GoodsID);
                                if (info.Quantity % 3 == 0)
                                {
                                    detailInfo.Specification += "<br/>";
                                    detailInfo.BarterSpecification += "<br/>";
                                }
                                //dics[shopExchangedApplyDetailInfo.GoodsID] =
                                //    dics[shopExchangedApplyDetailInfo.GoodsID] + 1;//如果一个商品换两个不同的商品则会报错插入重复键 modify by lcj at 2015.9.25
                                info.Quantity += 1;
                                detailInfo.Specification += string.Format("&nbsp;<span class='gray'>{0}x{1}{2}</span>",
                                                                          shopExchangedApplyDetailInfo.Specification,
                                                                          shopExchangedApplyDetailInfo.Quantity,
                                                                          shopExchangedApplyDetailInfo.Units);
                                detailInfo.BarterSpecification += string.Format("&nbsp;<span class='gray'>{0}x{1}{2}</span>",
                                                                          shopExchangedApplyDetailInfo.BarterSpecification,
                                                                          shopExchangedApplyDetailInfo.Quantity,
                                                                          shopExchangedApplyDetailInfo.Units);
                            }
                            else
                            {
                                //dics.Add(shopExchangedApplyDetailInfo.GoodsID,1);//如果一个商品换两个不同的商品则会报错插入重复键 modify by lcj at 2015.9.25
                                dics.Add(new DicChangeGoods { GoodsId = shopExchangedApplyDetailInfo.GoodsID, ChangeGoodsId = shopExchangedApplyDetailInfo.BarterGoodsID, Quantity = 1 });
                                showBarterList.Add(new ShopExchangedApplyDetailInfo
                                                       {
                                                           GoodsID = shopExchangedApplyDetailInfo.GoodsID,
                                                           GoodsName = shopExchangedApplyDetailInfo.GoodsName,
                                                           Specification = string.Format("<span class='gray'>{0}x{1}{2}</span>",
                                                                          shopExchangedApplyDetailInfo.Specification,
                                                                          shopExchangedApplyDetailInfo.Quantity,
                                                                          shopExchangedApplyDetailInfo.Units),
                                                           BarterGoodsID = shopExchangedApplyDetailInfo.BarterGoodsID,
                                                           BarterGoodsName = shopExchangedApplyDetailInfo.BarterGoodsName,
                                                           BarterSpecification = string.Format("<span class='gray'>{0}x{1}{2}</span>",
                                                                          shopExchangedApplyDetailInfo.BarterSpecification,
                                                                          shopExchangedApplyDetailInfo.Quantity,
                                                                          shopExchangedApplyDetailInfo.Units),
                                                       });
                            }
                        }
                        BarterDetailList = showBarterList;
                    }
                    else  //退货
                    {
                        RefundDetailList = _shopApplyDetail.GetShopApplyDetailList(ApplyId).ToList();
                    }
                }
            }
        }

        /// <summary>
        /// 退货列表
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgRefundGoodsListNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var showRefundList = new List<ShopApplyDetailInfo>();
            foreach (var shopApplyDetailInfo in RefundDetailList)
            {
                var detailInfo = showRefundList.FirstOrDefault
                    (act => act.GoodsID == shopApplyDetailInfo.GoodsID);
                if (detailInfo != null)
                {
                    detailInfo.Quantity += shopApplyDetailInfo.Quantity;
                }
                else
                {
                    showRefundList.Add(new ShopExchangedApplyDetailInfo
                    {
                        GoodsID = shopApplyDetailInfo.GoodsID,
                        GoodsName = shopApplyDetailInfo.GoodsName,
                        Specification = string.Format("{0}", shopApplyDetailInfo.Specification),
                        Quantity = shopApplyDetailInfo.Quantity
                    });
                }
            }
            RgRefundGoodsList.DataSource = showRefundList;
        }

        /// <summary>
        /// 换货商品
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgBarterGoodsListNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RgBarterGoodsList.DataSource = BarterDetailList;
        }

        /// <summary>
        /// 审核确认，重启检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnCheckedOnClick(object sender, ImageClickEventArgs e)
        {
            if (!RbPass.Checked && !RbNoPass.Checked)
            {
                RAM.Alert("请选择处理状态！");
                return;
            }
            if (CkReStart.Checked || RbPass.Checked || RbNoPass.Checked)
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Required))
                {
                    string remark = CkReStart.Checked?"重启检查":RbPass.Checked?"审核通过":"审核不通过";
                    int state = CkReStart.Checked ? (int)ExchangedState.CheckPending 
                        : RbPass.Checked 
                            ? (int)ExchangedState.Pass 
                            : (int)ExchangedState.NoPass;
                    int checkState = CkReStart.Checked ? (int)CheckState.Checking
                        : RbPass.Checked
                            ? (int)CheckState.Pass
                            : (int)CheckState.Refuse;
                    string msg;
                    int row = ShopExchangedApplyBll.WriteInstance.SetShopExchangedState(ApplyId, state, remark, out msg);
                    bool flag = row > 0;
                    if(RefundInfo!=null && flag)
                    {
                        int count = _refundDal.UpdateCheckRefund(RefundInfo.RefundId, checkState, WebControl.RetrunUserAndTime(remark), RefundInfo.SaleFilialeId);
                        flag = count > 0;
                    }
                    if(flag)
                    {
                        tran.Complete(); 
                    }
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgRefundGoodsListDetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = e.DetailTableView.ParentItem;
            if (dataItem != null)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                var realGoodsId = new Guid(dataItem.GetDataKeyValue("RealGoodsID").ToString());
                var dataList = realGoodsId == Guid.Empty && realGoodsId != goodsId
                                   ? RefundDetailList.Where(act => act.GoodsID == goodsId).ToList()
                                   : new List<ShopApplyDetailInfo>();
                e.DetailTableView.DataSource = dataList;
            }
        }

        protected void BtnBackOnClick(object sender, ImageClickEventArgs e)
        {
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
    }

    public class DicChangeGoods
    {
        public Guid GoodsId { get; set; }

        public Guid ChangeGoodsId { get; set; }

        public int Quantity { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using ERP.BLL.Implement.Organization;
using ERP.BLL.Implement.Shop;
using ERP.DAL.Implement.Shop;
using ERP.DAL.Interface.IShop;
using ERP.Enum.ShopFront;
using ERP.Environment;
using ERP.UI.Web.Common;
using MIS.Enum;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class ShopRefundMsgForm : Page
    {
        private static readonly IShopRefundMessage _shopRefund=new ShopRefundMessageDal(GlobalConfig.DB.FromType.Write);
        private readonly ShopRefundMessageBll _shopRefundMessageBll=new ShopRefundMessageBll(_shopRefund);
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                BindShopList();
            }
        }

        /// <summary>
        /// 退货留言查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSearchOnClick(object sender, EventArgs e)
        {
            RgRefundMsgList.Rebind();
        }

        /// <summary>
        /// 退货留言审核
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgRefundMsgListOnItemCommand(object source, GridCommandEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if(item!=null)
            {
                var msgId = new Guid(item.GetDataKeyValue("MsgID").ToString());
                var shopId = new Guid(item.GetDataKeyValue("ShopID").ToString());
                if (e.CommandName == "Approved")
                {
                    try
                    {
                        string msg;
                        var result = _shopRefundMessageBll.SetIsAllowReturnGoods(shopId, msgId, true, WebControl.RetrunUserAndTime("审核通过"), out msg);
                        RAM.Alert(result ? "退货留言处理成功!" : string.Format("退货留言审核失败-{0}!", msg));
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert(ex.Message);
                    }
                    
                }
                else if (e.CommandName == "Delete")
                {
                    try
                    {
                        var count = _shopRefund.SetMessageState(msgId, (int)ReturnMsgState.NoPass,
                        WebControl.RetrunUserAndTime("审核不通过"));
                        if (count > 0)
                        {
                            RAM.Alert("退货留言处理成功!");
                        }
                        else if (count == 0)
                        {
                            RAM.Alert("未找到该退货留言!");
                        }
                        else
                        {
                            RAM.Alert("退货留言审核失败!");
                        }
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert(ex.Message);
                    }
                }
                RgRefundMsgList.Rebind();
            }
        }

        /// <summary>
        /// 绑定退货留言列表
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgRefundMsgListNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DateTime startTime = RdpStartTime.SelectedDate ?? DateTime.MinValue;
            DateTime endTime = RdpEndTime.SelectedDate ?? DateTime.Now;
            Guid shopId = string.IsNullOrEmpty(RcbShopList.SelectedValue) ? Guid.Empty : new Guid(RcbShopList.SelectedValue);
            if(endTime<=DateTime.MinValue || startTime>endTime)
            {
                endTime = DateTime.Now;
            }
            if(startTime<=DateTime.MinValue || startTime==endTime)
            {
                startTime = endTime.AddMonths(-6);
            }
            RgRefundMsgList.DataSource = _shopRefund.GetShopRefundMesageList(startTime, endTime, (int)ReturnMsgState.CheckPending, shopId, true);
        }

        /// <summary>
        /// 绑定联盟店铺
        /// </summary>
        protected void BindShopList()
        {
            IDictionary<Guid, string> dics =new Dictionary<Guid, string>{{Guid.Empty,"--全部--"}};
            var list=FilialeManager.GetAllianceFilialeList()
                .Where(act => act.Rank == (int)FilialeRank.Partial)
                .ToDictionary(k => k.ID, v => v.Name);
            foreach (var item in list)
            {
                dics.Add(item.Key,item.Value);
            }
            RcbShopList.DataSource = dics;
            RcbShopList.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RefundMsgOnAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RgRefundMsgList,e);  
        }
    }
}
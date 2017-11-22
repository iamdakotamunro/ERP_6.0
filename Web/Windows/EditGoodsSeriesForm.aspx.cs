using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using OperationLog.Core;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class EditGoodsSeriesForm : WindowsPage
    {
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                SeriedId = new Guid(Request.QueryString["SeriedId"]);
        }

        protected Guid SeriedId
        {
            get
            {
                if (ViewState["SerieId"] == null)
                {
                    ViewState["SerieId"] = new Guid(Request.QueryString["SeriedId"]);
                }
                return new Guid(ViewState["SeriedId"].ToString());
            }
            set { ViewState["SeriedId"] = value.ToString(); }
        }

        protected void BindGoodsInfo()
        {
            IList<GoodsInfo> list = new List<GoodsInfo>();
            if (RT_Search.Text.Trim() != "")
            {
                var dic = _goodsCenterSao.GetGoodsSelectList(RT_Search.Text);
                if (dic != null)
                {
                    foreach (var keyValuePair in dic)
                    {
                        var goodsInfo = new GoodsInfo { GoodsId = keyValuePair.Key.ToGuid(), GoodsName = keyValuePair.Value };
                        list.Add(goodsInfo);
                    }
                }
            }
            rlbGoodsClass.DataSource = list;
            rlbGoodsClass.DataBind();
        }

        protected void RadAjaxPanel1_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            if (e.Argument == "load")
            {
                BindGoodsInfo();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindRlbExpandGoodsClass();
        }
        private void BindRlbExpandGoodsClass()
        {
            //根据商品系列查找商品
            Dictionary<Guid, string> dicGoodsIdAndName = _goodsCenterSao.GetGoodsIdAndGoodsNameBySeriesId(SeriedId);
            IList<GoodsInfo> list = new List<GoodsInfo>();
            if (dicGoodsIdAndName != null)
            {
                foreach (var keyValuePair in dicGoodsIdAndName)
                {
                    var goodsInfo = new GoodsInfo { GoodsId = keyValuePair.Key, GoodsName = keyValuePair.Value };
                    list.Add(goodsInfo);
                }
            }
            rlbExpandGoodsClass.DataSource = list;
            rlbExpandGoodsClass.DataBind();
        }

        //更新
        protected void ButtonUpdateGoods(object sender, EventArgs e)
        {
            try
            {
                var goodsIds = new List<Guid>();
                foreach (RadListBoxItem item in rlbExpandGoodsClass.Items)
                {
                    goodsIds.Add(new Guid(item.Value));
                }
                if (goodsIds.Count > 0)
                {
                    var dics = _goodsCenterSao.GetGoodsIdAndGoodsNameBySeriesId(SeriedId);
                    var updateSeriesId=new Dictionary<Guid, Guid>();
                    if (dics != null && dics.Count > 0)
                    {
                        foreach (var goodsId in goodsIds)
                        {
                            if(dics.ContainsKey(goodsId))continue;
                            if (!updateSeriesId.ContainsKey(goodsId))
                                updateSeriesId.Add(goodsId, SeriedId);
                        }
                        foreach (var dic in dics)
                        {
                            if (!goodsIds.Contains(dic.Key))
                            {
                                if (!updateSeriesId.ContainsKey(dic.Key))
                                    updateSeriesId.Add(dic.Key, Guid.Empty);
                            }
                        }
                    }
                    if (updateSeriesId.Count > 0)
                    {
                        var manager = new SalesGoodsRankingManager(_goodsCenterSao,new SalesGoodsRanking(GlobalConfig.DB.FromType.Write));
                        var updateResult = manager.UpdateGoodsSaleSeriesId(updateSeriesId);
                        if (!updateResult)
                        {
                            RAM.Alert("商品销量排行系列更新失败!");
                            return;
                        }
                    }
                        
                    string msg;
                    var result = _goodsCenterSao.SetGoodsSeries(SeriedId, goodsIds, out msg);
                    //更新商品销量排行中系列
                    if (result)
                    {
                        //记录工作日志
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, SeriedId, "",
                                                 OperationPoint.GoodsSeriesManager.Binding.GetBusinessInfo(), string.Empty);
                        BindRlbExpandGoodsClass();
                        RAM.Alert("操作成功");
                    }
                    else
                    {
                        RAM.Alert("更新无效!" + msg);
                    }
                }
            }
            catch
            {
                RAM.Alert("更新失败!");
            }
        }
    }
}
using System;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>添加商品登记
    /// </summary>
    public partial class AddCheckStockGoods : WindowsPage
    {
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddl_AddWarehouse.DataSource = CurrentSession.Personnel.WarehouseList;
                ddl_AddWarehouse.DataTextField = "WarehouseName";
                ddl_AddWarehouse.DataValueField = "WarehouseId";
                ddl_AddWarehouse.DataBind();
            }
        }

        //搜索商品
        protected void RcbGoodsItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text) && e.Text.Length >= 2)
            {
                var list = _goodsCenterSao.GetGoodsSelectList(e.Text);
                var totalCount = list.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in list)
                    {
                        var rcb = new RadComboBoxItem
                        {
                            Text = item.Value,
                            Value = item.Key,
                        };
                        combo.Items.Add(rcb);
                    }
                }
            }
        }

        protected void BtnAddClick(object sender, EventArgs e)
        {
            if (!CanSubmit())
            {
                RAM.Alert("请不要重复提交!");
                return;
            }
            ExecuteSubmit((ctx) =>
            {
                if (MethodHelp.CheckGuid(RCB_GoodsName.SelectedValue))
                {
                    var goodsBaseInfo = _goodsCenterSao.GetGoodsBaseInfoById(new Guid(RCB_GoodsName.SelectedValue));
                    if (goodsBaseInfo == null)
                    {
                        RAM.Alert("获取商品信息失败!");
                        ctx.SetFail();
                        return;
                    }
                    var waitCheckGoodsInfo = new WaitCheckGoodsInfo
                    {
                        GoodsId = goodsBaseInfo.GoodsId,
                        GoodsName = goodsBaseInfo.GoodsName,
                        WarehouseId = new Guid(ddl_AddWarehouse.SelectedValue),
                        State = (int)WaitCheckGoodsState.WaitCheck
                    };
                    try
                    {
                        if (BLL.Implement.Inventory.WaitCheckStockGoods.WriteInstance.AdOrUpWaitCheckGoodsInfo(waitCheckGoodsInfo) == 0)
                        {
                            Lbl_errMsg.Text = "添加或修改失败";
                            ctx.SetFail();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ctx.SetFail();
                        new LogUtility().WriteException(ex, "添加待盘点商品");
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");

                }
            });
        }
    }
}

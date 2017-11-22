using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class SetPrice : WindowsPage
    {
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        readonly IGoodsPriceChange _goodsPriceChange=new GoodsPriceChangeDal(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["goodsId"] == null || !MethodHelp.CheckGuid(Request.QueryString["goodsId"]))
                {
                    return;
                }
                const string PAGE_NAME = "MyGoodInfoList.aspx";
                if (!WebControl.GetPowerOperationPoint(PAGE_NAME, "SetPrice"))
                {
                    RAM.Alert("该功能管理不在您的职务范围之内！");
                    return;
                }
                MyGoodsId = new Guid(Request.QueryString["goodsId"]);

                MyDealGoodsSalePriceInfo = _goodsCenterSao.GetDealGoodsSalePriceInfoByGoodsId(MyGoodsId);
                Lbl_GoodsName.Text = MyDealGoodsSalePriceInfo.GoodsName;
                if (MyDealGoodsSalePriceInfo.MarketPrice > 0)
                    Lbl_MarketPrice.Text = MyDealGoodsSalePriceInfo.MarketPrice.ToString("#0.##");
                if (MyDealGoodsSalePriceInfo.ReferencePrice > 0)
                    TB_ReferencePrice.Text = MyDealGoodsSalePriceInfo.ReferencePrice.ToString("#0.##");
                if (MyDealGoodsSalePriceInfo.JoinPrice > 0)
                    TB_JoinPirce.Text = MyDealGoodsSalePriceInfo.JoinPrice.ToString("#0.##");
                TB_ImplicitCost.Text = MyDealGoodsSalePriceInfo.ImplicitCost.ToString("#0.##");
                //TB_ImplicitCost.Text = MyDealGoodsSalePriceInfo.ImplicitCost == 0 ? "0.05" : MyDealGoodsSalePriceInfo.ImplicitCost.ToString("#0.##");
                TB_YearDiscount.Text = MyDealGoodsSalePriceInfo.YearDiscount == 0 ? "1" : MyDealGoodsSalePriceInfo.YearDiscount.ToString("#0.##");


                if (MyDealGoodsSalePriceInfo.GroupGoodsPriceList != null && MyDealGoodsSalePriceInfo.GroupGoodsPriceList.Count > 0)
                {
                    repPriceList.DataSource = MyDealGoodsSalePriceInfo.GroupGoodsPriceList.Select(w => w.GoodsGroupInfo).ToList();
                    repPriceList.DataBind();
                }
                else
                {
                    Btn_UpdateAll.Visible = false;
                }
            }
        }

        #region[公用属性]
        protected Guid MyGoodsId
        {
            set { ViewState["goodsid"] = value; }
            get
            {
                if (ViewState["goodsid"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["goodsid"].ToString());
            }
        }

        protected Guid KeedeWebSiteId
        {
            set { ViewState["KeedeWebSiteId"] = value; }
            get
            {
                if (ViewState["KeedeWebSiteId"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["KeedeWebSiteId"].ToString());
            }
        }

        protected DealGoodsSalePriceInfo MyDealGoodsSalePriceInfo
        {
            set { ViewState["MyDealGoodsSalePriceInfo"] = value; }
            get
            {
                if (ViewState["MyDealGoodsSalePriceInfo"] == null)
                    return new DealGoodsSalePriceInfo();
                return (DealGoodsSalePriceInfo)ViewState["MyDealGoodsSalePriceInfo"];
            }
        }

        protected List<SalePlatformInfo> SalePlatformList
        {
            get
            {
                if (ViewState["SalePlatformList"] == null)
                {
                    ViewState["SalePlatformList"] = CacheCollection.SalePlatform.GetList();
                }
                return (List<SalePlatformInfo>)ViewState["SalePlatformList"];
            }
        }

        #endregion

        #region[获取折扣]
        protected string GetDiscount(string discount)
        {
            return (double.Parse(discount) * 100).ToString("f2") + "%";
        }
        #endregion

        //[生成会员价]
        private void CreatePrice(RadGrid radgrid, TextBox txtprice)
        {
            try
            {
                decimal price;
                var result = decimal.TryParse(txtprice.Text, out price);
                if (!result)
                {
                    RAM.Alert("价格不合法");
                    return;
                }
            }
            catch
            {
                RAM.Alert("价格不合法");
                return;
            }
            foreach (GridDataItem dataItem in radgrid.Items)
            {
                var discount = (Label)dataItem.FindControl("Lbl_Discount");
                decimal dcount = Convert.ToDecimal(discount.Text.Replace("%", "")) * (decimal)0.01;
                //保留2位小数，按四舍五入
                ((TextBox)dataItem.FindControl("TB_Price")).Text = Math.Round((dcount * decimal.Parse(txtprice.Text)), 1, MidpointRounding.AwayFromZero) + "0";
            }
        }

        //[更新全部]
        protected void Btn_UpdateAll_Click(object sender, EventArgs e)
        {
            string error = string.Empty;
            //if (Convert.ToDecimal(TB_ImplicitCost.Text.Trim()) < decimal.Parse("0.05"))
            //{
            //    error += "隐性成本不能小于0.05 \n";
            //}
            if (Convert.ToDecimal(TB_YearDiscount.Text.Trim()) < 1)
            {
                error += "年终折扣超出范围！不能<1";
            }
            else if (Convert.ToDecimal(TB_YearDiscount.Text.Trim()) > 2)
            {
                error += "年终折扣超出范围！不能>2";
            }
            if (!string.IsNullOrEmpty(error))
            {
                RAM.Alert(error);
                return;
            }

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    var strbAlertMessage = new StringBuilder();
                    string errorMessage;
                    var result = UpdateSellPrice(out errorMessage); //参考价
                    strbAlertMessage.AppendLine(result ? "参考价更新成功！" + errorMessage : "参考价操作无效！" + errorMessage);

                    result = UpdateJoinPirce(out errorMessage); //加盟价
                    strbAlertMessage.AppendLine(result ? "加盟价更新成功！" + errorMessage : "加盟价操作无效！" + errorMessage);

                    result = UpdateImplicitCost(out errorMessage);//隐性成本
                    strbAlertMessage.AppendLine(result ? "隐性成本更新成功！" + errorMessage : "隐性成本操作无效！" + errorMessage);

                    result = UpdateYearDiscount(out errorMessage);//年终扣率
                    strbAlertMessage.AppendLine(result ? "年终扣率更新成功！" + errorMessage : "年终扣率操作无效！" + errorMessage);

                    foreach (RepeaterItem repItem in repPriceList.Items)
                    {
                        var hfGroupId = (HiddenField)repItem.FindControl("hfGroupId");
                        var lbGroupName = (Label)repItem.FindControl("lbGroupName");
                        var tbPrice = (TextBox)repItem.FindControl("tbPrice");
                        var rgPrice = (RadGrid)repItem.FindControl("RG_Price");
                        var rgThirdPrice = (RadGrid)repItem.FindControl("RG_ThirdPrice");
                        var groupId = new Guid(hfGroupId.Value);
                        result = UpdatePrice(rgPrice, rgThirdPrice, groupId, decimal.Parse(tbPrice.Text), out errorMessage);//更新会员等级价
                        strbAlertMessage.AppendLine(result ? "[" + lbGroupName.Text + "]操作成功！" + errorMessage : "[" + lbGroupName.Text + "]操作无效！" + errorMessage);
                    }
                    ts.Complete();
                    RAM.Alert(strbAlertMessage.ToString());
                }
                catch (Exception ex)
                {
                    RAM.Alert("操作异常！" + ex);
                }
            }
        }

        //[更新参考价]
        protected void Bt_SavePrice_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TB_ReferencePrice.Text.Trim()))
            {
                try
                {
                    string errorMessage;
                    var result = UpdateSellPrice(out errorMessage);
                    RAM.Alert(result ? "参考价更新成功！" + errorMessage : "参考价操作无效！" + errorMessage);
                }
                catch
                {
                    RAM.Alert("参考价更新失败");
                }
            }
        }
        //参考价
        protected bool UpdateSellPrice(out string errorMessage)
        {
            decimal price = decimal.Parse(TB_ReferencePrice.Text.Trim());
            return _goodsCenterSao.UpdateReferencePrice(MyGoodsId, price, out errorMessage);
        }

        // [更新加盟价]
        protected void Bt_JoinPirce_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TB_JoinPirce.Text))
            {
                try
                {
                    string errorMessage;
                    var result = UpdateJoinPirce(out errorMessage);
                    RAM.Alert(result ? "加盟价更新成功！" + errorMessage : "加盟价操作无效！" + errorMessage);
                }
                catch
                {
                    RAM.Alert("更新失败");
                }
            }
        }
        //更新加盟价
        protected bool UpdateJoinPirce(out string errorMsg)
        {
            decimal joinPriceNew = decimal.Parse(TB_JoinPirce.Text.Trim());
            bool result = _goodsCenterSao.UpdateJoinPrice(MyGoodsId, joinPriceNew, out errorMsg);
            if (result)
            {
                if (!MyDealGoodsSalePriceInfo.JoinPrice.Equals(joinPriceNew))
                {
                    var goodsPriceChange = new Model.GoodsPriceChange
                    {
                        Id = Guid.NewGuid(),
                        Name = CurrentSession.Personnel.Get().RealName,
                        Datetime = DateTime.Now,
                        GoodsId = MyGoodsId,
                        GoodsName = MyDealGoodsSalePriceInfo.GoodsName,
                        GoodsCode = MyDealGoodsSalePriceInfo.GoodsCode,
                        SaleFilialeId = Guid.Empty,
                        SaleFilialeName = string.Empty,
                        SalePlatformId = Guid.Empty,
                        SalePlatformName = string.Empty,
                        OldPrice = MyDealGoodsSalePriceInfo.JoinPrice,
                        NewPrice = joinPriceNew,
                        Quota = MyDealGoodsSalePriceInfo.JoinPrice - joinPriceNew,
                        Type = 1
                    };
                    _goodsPriceChange.AddGoodsPriceChange(goodsPriceChange);
                }
            }
            return result;
        }

        // [更新隐性成本]
        protected void Bt_ImplicitCost_Click(object sender, EventArgs e)
        {
            //if (Convert.ToDecimal(TB_ImplicitCost.Text.Trim()) < decimal.Parse("0.05"))
            //{
            //    RAM.Alert("隐性成本不能小于0.05");
            //    return;
            //}
            if (!string.IsNullOrEmpty(TB_ImplicitCost.Text.Trim()))
            {
                try
                {
                    string errorMessage;
                    var result = UpdateImplicitCost(out errorMessage);
                    RAM.Alert(result ? "隐性成本更新成功！" + errorMessage : "隐性成本操作无效！" + errorMessage);
                }
                catch
                {
                    RAM.Alert("更新失败");
                }
            }
        }
        //更新隐性成本
        protected bool UpdateImplicitCost(out string errorMessage)
        {
            var implicitCost = decimal.Parse(TB_ImplicitCost.Text.Trim());
            return _goodsCenterSao.UpdateImplicitCost(MyGoodsId, implicitCost, out errorMessage);
        }

        // [更新年终扣率]
        protected void Bt_YearDiscount_Click(object sender, EventArgs e)
        {
            if (Convert.ToDecimal(TB_YearDiscount.Text.Trim()) < 1)
            {
                RAM.Alert("年终折扣超出范围！");
                return;
            }
            if (Convert.ToDecimal(TB_YearDiscount.Text.Trim()) > 2)
            {
                RAM.Alert("年终折扣超出范围！");
                return;
            }
            if (!string.IsNullOrEmpty(TB_YearDiscount.Text.Trim()))
            {
                try
                {
                    string errorMessage;
                    var result = UpdateYearDiscount(out errorMessage);
                    RAM.Alert(result ? "年终扣率更新成功！" + errorMessage : "年终扣率操作无效！" + errorMessage);
                }
                catch
                {
                    RAM.Alert("更新失败");
                }
            }
        }
        //更新年终扣率
        protected bool UpdateYearDiscount(out string errorMessage)
        {
            var yearDiscount = decimal.Parse(TB_YearDiscount.Text.Trim());
            return _goodsCenterSao.UpdateYearDiscount(MyGoodsId, yearDiscount, out errorMessage);
        }

        protected void RepPriceList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var hfGroupId = (HiddenField)e.Item.FindControl("hfGroupId");
            if (hfGroupId != null)
            {
                var groupId = new Guid(hfGroupId.Value);
                var goodsSalePriceInfo = MyDealGoodsSalePriceInfo.GroupGoodsPriceList.FirstOrDefault(w => w.GroupId == groupId);
                if (goodsSalePriceInfo != null)
                {
                    var tbPrice = (TextBox)e.Item.FindControl("tbPrice");
                    tbPrice.Text = string.Format("{0}", goodsSalePriceInfo.Price);

                    var rgPrice = (RadGrid)e.Item.FindControl("RG_Price");
                    var rgThirdPrice = (RadGrid)e.Item.FindControl("RG_ThirdPrice");
                    if (goodsSalePriceInfo.GoodsRolePriceList != null)
                    {
                        rgPrice.DataSource = goodsSalePriceInfo.GoodsRolePriceList.OrderByDescending(w => w.Discount).ToList();
                        rgPrice.DataBind();
                    }
                    if (goodsSalePriceInfo.ThirdPriceList != null)
                    {
                        var list = (from info in goodsSalePriceInfo.ThirdPriceList.Where(w => w.GroupId == groupId).ToList()
                                    let salePlatformInfo = SalePlatformList.FirstOrDefault(w => w.ID == info.SalePlatformId)
                                    where salePlatformInfo != null
                                    select new ThirdPriceInfo
                                    {
                                        GroupId = groupId,
                                        SalePlatformId = salePlatformInfo.ID,
                                        SalePlatformName = salePlatformInfo.Name,
                                        Price = info.Price,
                                        IsDefault = info.IsDefault
                                    }).ToList();
                        if (list.Any())
                        {
                            rgThirdPrice.Visible = true;
                            rgThirdPrice.DataSource = list;
                            rgThirdPrice.DataBind();
                        }
                    }
                }
            }
        }

        protected void RepPriceList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var hfGroupId = (HiddenField)e.Item.FindControl("hfGroupId");
            if (e.CommandName == "CreateEyesMemberPrice")
            {
                //生成会员价
                var tbPrice = (TextBox)e.Item.FindControl("tbPrice");
                var rgPrice = (RadGrid)e.Item.FindControl("RG_Price");
                CreatePrice(rgPrice, tbPrice);
            }
            else if (e.CommandName == "UpdatePrice")
            {
                //更新
                var tbPrice = (TextBox)e.Item.FindControl("tbPrice");
                var rgPrice = (RadGrid)e.Item.FindControl("RG_Price");
                var rgThirdPrice = (RadGrid)e.Item.FindControl("RG_ThirdPrice");
                if (rgPrice != null && hfGroupId != null)
                {
                    string errorMessage;
                    var result = UpdatePrice(rgPrice, rgThirdPrice, new Guid(hfGroupId.Value), decimal.Parse(tbPrice.Text), out errorMessage);
                    RAM.Alert(result ? "操作成功！" + errorMessage : "操作无效！" + errorMessage);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rgPrice"></param>
        /// <param name="rgThirdPrice"></param>
        /// <param name="groupId"></param>
        /// <param name="price"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        /// zal 2015-07-17 添加商品改价记录
        private bool UpdatePrice(RadGrid rgPrice, RadGrid rgThirdPrice, Guid groupId, decimal price, out string errorMessage)
        {
            GoodsSalePriceInfo goodsSalePriceInfo = MyDealGoodsSalePriceInfo.GroupGoodsPriceList.FirstOrDefault(w => w.GroupId == groupId);
            if (goodsSalePriceInfo != null)
            {
                var goodsSalePriceListUpdate = new List<GoodsSalePriceInfo>();
                var goodsSalePriceInfoUpdate = new GoodsSalePriceInfo
                {
                    GoodsGroupInfo = goodsSalePriceInfo.GoodsGroupInfo,
                    GoodsId = goodsSalePriceInfo.GoodsId,
                    GroupId = goodsSalePriceInfo.GroupId,
                    GoodsRolePriceList = new List<GoodsRolePriceInfo>(),
                    ThirdPriceList = new List<ThirdGoodsSalePriceInfo>()
                };

                var salePlatformInfoList = MISService.GetAllSalePlatform().ToList();
                var goodsPriceChangeList = new List<Model.GoodsPriceChange>();
                if (!goodsSalePriceInfo.Price.Equals(price))
                {
                    var officialSalePlatformIDs = _goodsCenterSao.GetGroupDetailList().Where(p => p.GroupID == goodsSalePriceInfo.GroupId).Select(p => p.OfficialSalePlatformIDs).FirstOrDefault();
                    if (officialSalePlatformIDs != null)
                    {
                        officialSalePlatformIDs = officialSalePlatformIDs.Where(o => o != new Guid("63710AD6-A235-458F-BFCF-1BFEE63CAB35")).ToList();
                        goodsPriceChangeList.AddRange(from item in officialSalePlatformIDs
                            let salePlatformInfo = salePlatformInfoList.Where(p => p.ID == new Guid(item.ToString()))
                            let saleFilialeId = salePlatformInfo.Select(p => p.FilialeId).FirstOrDefault()
                            select new Model.GoodsPriceChange
                            {
                                Id = Guid.NewGuid(), Name = CurrentSession.Personnel.Get().RealName,
                                Datetime = DateTime.Now, 
                                GoodsId = MyDealGoodsSalePriceInfo.GoodsId, 
                                GoodsName = MyDealGoodsSalePriceInfo.GoodsName, 
                                GoodsCode = MyDealGoodsSalePriceInfo.GoodsCode, 
                                SaleFilialeId = saleFilialeId, 
                                SaleFilialeName = CacheCollection.Filiale.GetHeadList().Where(p => p.ID == saleFilialeId).Select(p => p.Name).FirstOrDefault(), 
                                SalePlatformId = item, 
                                SalePlatformName = salePlatformInfo.Select(p => p.Name).FirstOrDefault(), 
                                OldPrice = goodsSalePriceInfo.Price, NewPrice = price, 
                                Quota = goodsSalePriceInfo.Price - price, 
                                Type = 0
                            });
                    }
                }
                goodsSalePriceInfoUpdate.Price = price;

                try
                {
                    foreach (GridDataItem dataItem in rgPrice.Items)
                    {
                        var tprice = (TextBox)dataItem.FindControl("TB_Price");
                        var roleId = (Guid)dataItem.GetDataKeyValue("RoleId");
                        var info = new GoodsRolePriceInfo
                        {
                            GroupId = groupId,
                            GoodsId = MyGoodsId,
                            RoleId = roleId,
                            Price = decimal.Parse(tprice.Text.Trim())
                        };
                        goodsSalePriceInfoUpdate.GoodsRolePriceList.Add(info);
                    }
                    foreach (GridDataItem dataItem in rgThirdPrice.Items)
                    {
                        var salePlatformId = (Guid)dataItem.GetDataKeyValue("SalePlatformId");
                        var tThirdPrice = (TextBox)dataItem.FindControl("TB_ThirdPrice");
                        var thirdPrice = goodsSalePriceInfo.ThirdPriceList.Where(p => p.SalePlatformId == salePlatformId).Select(p => p.Price).FirstOrDefault();

                        if (thirdPrice.Equals(0))
                        {
                            thirdPrice = goodsSalePriceInfo.Price;
                        }

                        var info = new ThirdGoodsSalePriceInfo
                        {
                            GoodsId = MyGoodsId,
                            GroupId = groupId,
                            SalePlatformId = salePlatformId,
                            Price = decimal.Parse(tThirdPrice.Text.Trim()),
                        };

                        if (!thirdPrice.Equals(decimal.Parse(tThirdPrice.Text.Trim())))
                        {
                            var salePlatformInfo = salePlatformInfoList.Where(p => p.ID == salePlatformId).ToList();
                            var saleFilialeId = salePlatformInfo.Select(p => p.FilialeId).FirstOrDefault();
                            var goodsPriceChange = new Model.GoodsPriceChange
                            {
                                Id = Guid.NewGuid(),
                                Name = CurrentSession.Personnel.Get().RealName,
                                Datetime = DateTime.Now,
                                GoodsId = MyDealGoodsSalePriceInfo.GoodsId,
                                GoodsName = MyDealGoodsSalePriceInfo.GoodsName,
                                GoodsCode = MyDealGoodsSalePriceInfo.GoodsCode,
                                SaleFilialeId = saleFilialeId,
                                SaleFilialeName = CacheCollection.Filiale.GetHeadList().Where(p => p.ID == saleFilialeId).Select(p => p.Name).FirstOrDefault(),
                                SalePlatformId = salePlatformId,
                                SalePlatformName = salePlatformInfo.Select(p => p.Name).FirstOrDefault(),
                                OldPrice = thirdPrice,
                                NewPrice = decimal.Parse(tThirdPrice.Text.Trim()),
                                Quota = thirdPrice - decimal.Parse(tThirdPrice.Text.Trim()),
                                Type = 0
                            };
                            goodsPriceChangeList.Add(goodsPriceChange);
                        }
                        goodsSalePriceInfoUpdate.ThirdPriceList.Add(info);
                    }
                    var result = _goodsCenterSao.SetGoodsPrice(MyGoodsId, goodsSalePriceListUpdate, out errorMessage);
                    if (result)
                    {
                        var resultChange =_goodsPriceChange.AddBulkGoodsPriceChange(goodsPriceChangeList);
                        if (resultChange)
                        {
                            RAM.ResponseScripts.Add("location.href=location.href;");
                        }
                    }
                    return result;
                }
                catch (Exception e)
                {
                    RAM.Alert(e.Message);
                }
            }
            errorMessage = null;
            return false;
        }

        protected void RgThirdPriceItemDataBound(object sender, GridItemEventArgs e)
        {
            var tbPrice = (TextBox)e.Item.FindControl("TB_ThirdPrice");
            //tbPrice.Font.Bo;
            var item = e.Item as GridDataItem;
            var isDefault = false;
            if (item != null)
            {
                isDefault = Convert.ToBoolean(item.GetDataKeyValue("IsDefault"));
            }
            if (tbPrice != null && isDefault)
            {
                tbPrice.ForeColor = Color.Gray;
            }
        }
    }

    public class ThirdPriceInfo
    {
        public Guid GroupId { get; set; }
        public Guid SalePlatformId { get; set; }
        public string SalePlatformName { get; set; }
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Goods;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using ERP.DAL.Interface.IStorage;
using ERP.DAL.Implement.Storage;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 最后修改人：刘彩军
    /// 修改时间：2011-August-05th
    /// 修改内容：修改GoodsInfo为GoodsBaseInfo
    /// </summary>
    public partial class AddComparePro : WindowsPage
    {
        protected string ErrMsg;
        protected string SuccMsg;
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly ComparePrice _comparePrice = new ComparePrice(GlobalConfig.DB.FromType.Write);
        static readonly GoodsPriceDao _goodsPriceDao = new GoodsPriceDao(GlobalConfig.DB.FromType.Write);
        readonly IGoodsStockRecord _goodsStockSettleRecordBll = new GoodsStockRecordDao();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["goodsId"] == null || !MethodHelp.CheckGuid(Request.QueryString["goodsId"]))
                {
                    return;
                }
                GoodsId = new Guid(Request.QueryString["goodsId"]);
                GoodsInfo goodsBaseInfo = _goodsCenterSao.GetGoodsBaseInfoById(GoodsId);
                if (goodsBaseInfo != null)
                {
                    lbInfo.Text = "匹配的商品是：" + goodsBaseInfo.GoodsName;
                }
            }
        }

        #region [Properties属性]
        /// <summary>
        /// 要匹配的产品ID
        /// </summary>
        protected Guid GoodsId
        {
            get
            {
                if (ViewState["GoodsId"] == null)
                {
                    ViewState["GoodsId"] = Guid.Empty;
                }
                return new Guid(ViewState["GoodsId"].ToString());
            }
            set { ViewState["GoodsId"] = value; }
        }

        protected IList<FetchDataInfo> FetchDataList
        {
            get
            {
                if (ViewState["FetchDataList"] == null)
                {
                    var list = new List<FetchDataInfo>();
                    ViewState["FetchDataList"] = list;
                }
                return (IList<FetchDataInfo>)ViewState["FetchDataList"];
            }
            set { ViewState["FetchDataList"] = value; }
        }

        protected string GoodsName
        {
            get
            {
                if (ViewState["GoodsName"] == null)
                {
                    ViewState["GoodsName"] = Guid.Empty;
                }
                return ViewState["GoodsName"].ToString();
            }
            set { ViewState["GoodsName"] = value; }
        }
        #endregion

        protected void RgFetchData_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            FetchDataList = _comparePrice.GetFetchDataListByGoodsId(GoodsId);
            rgFetchData.DataSource = FetchDataList;
        }

        protected void RgFetchData_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var gridEditableItem = (GridEditableItem)e.Item;
            var cbIsChecked = (CheckBox)gridEditableItem.FindControl("cbIsChecked");
            var txtGoodsName = (TextBox)gridEditableItem.FindControl("tbGoodsName");
            var tbPrice = (TextBox)gridEditableItem.FindControl("tbPrice");
            var tbGoodsUrl = (TextBox)gridEditableItem.FindControl("tbGoodsUrl");

            var id = int.Parse(gridEditableItem.GetDataKeyValue("Id").ToString());
            var siteId = int.Parse(gridEditableItem.GetDataKeyValue("SiteId").ToString());
            bool isChecked = cbIsChecked.Checked;
            string goodsName = txtGoodsName.Text.Trim();
            string price = tbPrice.Text.Trim();
            string url = tbGoodsUrl.Text.Trim();

            FetchDataInfo oldFetchDataInfo = FetchDataList.FirstOrDefault(w => w.SiteId == siteId && w.Id == id);

            if (string.IsNullOrEmpty(goodsName) && string.IsNullOrEmpty(price) && string.IsNullOrEmpty(url))
            {
                return;
            }
            if (oldFetchDataInfo != null)
            {
                if (isChecked)
                {
                    //检查是否存在多个已绑定的
                    var list = FetchDataList.Where(w => w.IsChecked && w.SiteId == siteId && w.Id != id).ToList();
                    if (list.Count > 0)
                    {
                        foreach (var fetchDataInfo in list)
                        {
                            _comparePrice.SetChecked(fetchDataInfo.Id, false);
                        }
                    }
                }
                if (FetchDataList.Count(w => w.SiteId == siteId && w.Id != id && w.GoodsUrl == url) > 0)
                {
                    RAM.Alert(oldFetchDataInfo.SiteName + "存在相同的商品URL，操作无效");
                    return;
                }
                if (oldFetchDataInfo.Id > 0)
                {
                    //更改
                    try
                    {
                        var newFetchDataInfo = new FetchDataInfo
                            {
                                Id = id,
                                SiteId = siteId,
                                GoodsId = GoodsId,
                                GoodsName = goodsName,
                                GoodsPrice = decimal.Parse(price),
                                GoodsUrl = url,
                                LastUpdateTime = DateTime.Now,
                                IsChecked = isChecked
                            };
                        _comparePrice.ModifyProduct(newFetchDataInfo);
                    }
                    catch (Exception ex)
                    {
                        ErrMsg += ex.Message;
                    }
                }
                else
                {
                    //添加
                    try
                    {
                        var newFetchDataInfo = new FetchDataInfo
                            {
                                SiteId = siteId,
                                GoodsId = GoodsId,
                                GoodsName = goodsName,
                                GoodsPrice = decimal.Parse(price),
                                GoodsUrl = url,
                                LastUpdateTime = DateTime.Now,
                                IsChecked = isChecked
                            };
                        _comparePrice.AddProduct(newFetchDataInfo);
                    }
                    catch (Exception ex)
                    {
                        ErrMsg += ex.Message;
                    }
                }
            }
            rgFetchData.DataBind();
        }

        protected void RgFetchDataSearch_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (IsPostBack)
            {
                if (!string.IsNullOrEmpty(GoodsName))
                {
                    long recordCount;
                    var searchFetchDataList = _goodsPriceDao.GetFetchDataList((rgFetchDataSearch.CurrentPageIndex + 1), rgFetchDataSearch.PageSize, GoodsName, out recordCount);
                    rgFetchDataSearch.VirtualItemCount = (int)recordCount;
                    rgFetchDataSearch.DataSource = searchFetchDataList;
                }
                else
                {
                    rgFetchDataSearch.VirtualItemCount = 0;
                    rgFetchDataSearch.DataSource = new List<FetchDataInfo>();
                }
            }
        }
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            divSearch.Visible = true;
            GoodsName = tbGoodsName.Text;
            rgFetchDataSearch.CurrentPageIndex = 0;
            rgFetchDataSearch.Rebind();
        }

        //商品绑定
        protected void BtnGoodsBind_Click(object sender, EventArgs e)
        {
            if (rgFetchDataSearch.SelectedItems.Count == 0)
            {
                RAM.Alert("请选择");
            }
            else
            {
                string strSiteIds = ",";
                foreach (GridDataItem dataItem in rgFetchDataSearch.SelectedItems)
                {
                    var siteId = dataItem["SiteId"].Text;
                    if (!strSiteIds.Contains("," + siteId + ","))
                    {
                        strSiteIds += siteId + ",";
                    }
                    else
                    {
                        RAM.Alert("商品绑定中网站名称出现重复，操作无效");
                        return;
                    }
                }
                foreach (GridDataItem dataItem in rgFetchDataSearch.SelectedItems)
                {
                    var id = int.Parse(dataItem["Id"].Text);
                    _comparePrice.UpdateBinding(id, GoodsId);
                }
                RAM.Alert("操作成功");
                rgFetchData.Rebind();
                rgFetchDataSearch.Rebind();
                divSearch.Visible = false;
            }
        }

        protected void btnCloseRefresh_Click(object sender, EventArgs e)
        {
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
    }
}

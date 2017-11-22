using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Enum;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Telerik.Web.UI;
using ERP.Environment;

namespace ERP.UI.Web.Windows
{
    public partial class ShowAttrWordsGoodsForm : WindowsPage
    {
        readonly IGoodsCenterSao _goodsAttributeGroupSao=new GoodsCenterSao();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ReqGroupId == 0 || ReqWordId == 0 || ReqMatchType == -1 || ReqIsMChoice == -1)
                {
                    RAM.Alert("操作异常，请重新操作！");
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
                else
                {
                    if (ReqMatchType == 0)
                    {
                        //文字匹配
                        if (ReqIsMChoice == 1)
                        {
                            //多选
                            GridAttrWordsGoods.Columns[2].Display = false;
                            GridAttrWordsGoods.Columns[3].Display = true;
                        }
                        else
                        {
                            GridAttrWordsGoods.Columns[2].Display = true;
                            GridAttrWordsGoods.Columns[3].Display = false;
                        }
                        GridAttrWordsGoods.Columns[4].Display = false;
                    }
                    else if (ReqMatchType == 1)
                    {
                        //数值匹配
                        GridAttrWordsGoods.Columns[2].Display = false;
                        GridAttrWordsGoods.Columns[3].Display = false;
                        GridAttrWordsGoods.Columns[4].Display = true;
                    }
                }
            }
        }

        private int ReqWordId
        {
            get
            {
                if (ViewState["WordId"] == null)
                {
                    if (Request.QueryString["WordId"] != null)
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["WordId"]))
                        {
                            ViewState["WordId"] = Request.QueryString["WordId"];
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                return Convert.ToInt32(ViewState["WordId"]);
            }
        }

        private int ReqGroupId
        {
            get
            {
                if (ViewState["GroupId"] == null)
                {
                    if (Request.QueryString["GroupId"] != null)
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["GroupId"]))
                        {
                            ViewState["GroupId"] = Request.QueryString["GroupId"];
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                return Convert.ToInt32(ViewState["GroupId"]);
            }
        }

        /// <summary>
        /// 0文字匹配,1数值匹配,-1异常
        /// </summary>
        protected int ReqMatchType
        {
            get
            {
                if (ViewState["MatchType"] == null)
                {
                    if (Request.QueryString["MatchType"] != null)
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["MatchType"]))
                        {
                            ViewState["MatchType"] = Request.QueryString["MatchType"];
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                return Convert.ToInt32(ViewState["MatchType"]);
            }
        }

        /// <summary>
        /// 0false,1true,-1异常
        /// </summary>
        protected int ReqIsMChoice
        {
            get
            {
                if (ViewState["IsMChoice"] == null)
                {
                    if (Request.QueryString["IsMChoice"] != null)
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["IsMChoice"]))
                        {
                            ViewState["IsMChoice"] = Request.QueryString["IsMChoice"];
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                return Convert.ToInt32(ViewState["IsMChoice"]);
            }
        }

        protected IList<AttrWordsGoodsInfo> AttrWordsGoodsInfoList
        {
            get
            {
                if (ViewState["AttrWordsGoodsInfoList"] == null)
                {
                    ViewState["AttrWordsGoodsInfoList"] = new List<AttrWordsGoodsInfo>();
                }
                return (IList<AttrWordsGoodsInfo>)ViewState["AttrWordsGoodsInfoList"];
            }
            set { ViewState["AttrWordsGoodsInfoList"] = value; }
        }

        protected void GridAttrWordsGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            int totalCount;
            var pageIndex = GridAttrWordsGoods.CurrentPageIndex + 1;
            var pageSize = GridAttrWordsGoods.PageSize;
            string errorMessage;
            var attrWordsGoodsList = _goodsAttributeGroupSao.GetAttrWordsGoodsListByPage(ReqWordId, pageIndex, pageSize, out totalCount, out errorMessage);
            AttrWordsGoodsInfoList = attrWordsGoodsList.ToList();
            GridAttrWordsGoods.DataSource = attrWordsGoodsList;
            GridAttrWordsGoods.VirtualItemCount = totalCount;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                RAM.Alert(errorMessage);
            }
        }

        protected void GridAttrWordsGoods_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                var attrWordsGoodsInfo = AttrWordsGoodsInfoList.FirstOrDefault(w => w.GoodsId == goodsId);
                if (ReqMatchType == 0 && attrWordsGoodsInfo != null)
                {
                    //文字匹配
                    if (ReqIsMChoice == 1)
                    {
                        //多选
                        var rcbMoreWordName = (RadComboBox)dataItem.FindControl("RCB_MoreWordName");
                        if (rcbMoreWordName != null)
                        {
                            BindRadComboBox(rcbMoreWordName, attrWordsGoodsInfo.WordIdAndWordName, "Key", "Value", YesOrNo.No);
                            foreach (var titem in rcbMoreWordName.Controls)
                            {
                                if (titem is RadComboBoxItem)
                                {
                                    var nitem = titem as RadComboBoxItem;
                                    foreach (var control in nitem.Controls)
                                    {
                                        if (control is CheckBox)
                                        {
                                            var cb = control as CheckBox;
                                            if (attrWordsGoodsInfo.Value.Contains(cb.ToolTip))
                                            {
                                                cb.Checked = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var rcbWordName = (RadComboBox)dataItem.FindControl("RCB_WordName");
                        if (rcbWordName != null)
                        {
                            rcbWordName.DataSource = attrWordsGoodsInfo.WordIdAndWordName;
                            rcbWordName.DataTextField = "Key";
                            rcbWordName.DataTextField = "Value";
                            rcbWordName.SelectedValue = string.Format("{0}",ReqWordId);
                        }
                    }
                }
            }
        }

        protected void GridAttrWordsGoods_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == "Submit")
            {
                var dataItem = (GridDataItem)e.Item;
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                string value = string.Empty;
                if (ReqMatchType == 0)
                {
                    //文字匹配
                    if (ReqIsMChoice == 1)
                    {
                        var rcbMoreWordName = (RadComboBox)dataItem.FindControl("RCB_MoreWordName");
                        foreach (var titem in rcbMoreWordName.Controls)
                        {
                            if (titem is RadComboBoxItem)
                            {
                                var nitem = titem as RadComboBoxItem;
                                foreach (var control in nitem.Controls)
                                {
                                    if (control is CheckBox)
                                    {
                                        var cb = control as CheckBox;
                                        if (cb.Checked)
                                        {
                                            if (string.IsNullOrEmpty(value))
                                                value += cb.ToolTip;
                                            else
                                                value += "," + cb.ToolTip;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var rcbWordName = (RadComboBox)dataItem.FindControl("RCB_WordName");
                        value = rcbWordName.SelectedValue;
                    }
                }
                else if (ReqMatchType == 1)
                {
                    //数值匹配
                    var tbValue = (TextBox)dataItem.FindControl("tbValue");
                    value = tbValue.Text;
                }
                string errorMessage;
                if (_goodsAttributeGroupSao.ChangeGoodsAttribute(goodsId, ReqGroupId, value, out errorMessage))
                    GridAttrWordsGoods.Rebind();
                else
                    RAM.Alert("操作无效！" + errorMessage);
            }
        }
        #region [绑定一个RadComboBox控件]

        /// <summary> 绑定一个RadComboBox控件
        /// </summary>
        /// <param name="rcb">控件</param>
        /// <param name="dataSource">数据源</param>
        /// <param name="dataValueField">值</param>
        /// <param name="dataTextField">显示文字</param>
        /// <param name="yesOrNo">是否有第一项,YES,有第一项,NO,没有</param>
        internal static void BindRadComboBox<T>(RadComboBox rcb, T dataSource, string dataValueField, string dataTextField, YesOrNo yesOrNo)
        {
            rcb.DataSource = dataSource;
            rcb.DataValueField = dataValueField;
            rcb.DataTextField = dataTextField;
            rcb.ToolTip = dataValueField;
            rcb.DataBind();
            if (yesOrNo == YesOrNo.Yes)
                rcb.Items.Insert(0, new RadComboBoxItem("请选择", Guid.Empty.ToString()));
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Goods;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class AddPurchasingGoodsForm : WindowsPage
    {
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private GoodsClassManager _goodsClassManager;
        private readonly IPurchasing _purchasing=new Purchasing(GlobalConfig.DB.FromType.Read);
        private readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Read);
        private readonly IPurchaseSet _purchaseSet=new PurchaseSet(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {

            _submitController = CreateSubmitInstance();
            if (!Page.IsPostBack)
            {
                _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
                RCB_GoodsClass.DataSource = _goodsClassManager.GetGoodsClassListWithRecursion(); 
                RCB_GoodsClass.DataBind();
                RCB_GoodsClass.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
            }
        }
        /// <summary>
        /// 存储商品列表
        /// </summary>
        private IList<PurchasingDetailInfo> PurchasingDetailList
        {
            get
            {
                if (ViewState["PurchasingDetailInfo"] == null)
                {
                    return new List<PurchasingDetailInfo>();
                }
                return (List<PurchasingDetailInfo>)ViewState["PurchasingDetailInfo"];
            }
            set
            {
                ViewState["PurchasingDetailInfo"] = value;
            }
        }

        SubmitController _submitController;
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["Checking"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid(), 180);
                ViewState["Checking"] = _submitController;
            }
            return (SubmitController)ViewState["Checking"];
        }
        //end add 

        #region 初始化
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            RCB_Goods.ItemsRequested += RcbGoodsItemsRequested;
        }
        #endregion

        #region 绑定要采购明细商品
        protected void Rgd_PurchasingDetail_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Rgd_PurchasingDetail.DataSource = PurchasingDetailList;
        }
        #endregion

        //选择商品框的商品绑定
        protected void RgSelectGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGSelectGoods.DataSource = GoodsTable;
        }

        //创建价格表时设定列宽
        protected void RgSelectGoodsColumnCreated(object sender, GridColumnCreatedEventArgs e)
        {
            string uniqueName = e.Column.UniqueName;
            switch (uniqueName)
            {
                case "GoodsId":
                    e.Column.Visible = false;
                    break;
                case "商品编号":
                    e.Column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    e.Column.HeaderStyle.Width = 100;
                    e.Column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    break;
                case "商品名称":
                    e.Column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    e.Column.HeaderStyle.Width = 255;
                    e.Column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    break;
                case "CheckBoxColumn":
                    e.Column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    e.Column.HeaderStyle.Width = 120;
                    e.Column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    break;
            }
        }

        //价格表单元格绑定
        protected void RgSelectGoodsItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;

                if (dataItem != null)
                {
                    var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                    IList<FieldInfo> fieldInfoList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsId).ToList();

                    string hiddenValue = string.Empty;
                    foreach (GridTableCell cell in e.Item.Cells)
                    {
                        int cellIndex = e.Item.Cells.GetCellIndex(cell);

                        if (cellIndex >= 6)
                        {
                            foreach (FieldInfo fieldInfo in fieldInfoList)
                            {
                                if (fieldInfo.FieldId.CompareTo(cell.Text) == 0)
                                {
                                    if (fieldInfo.ParentFieldId == Guid.Empty)
                                    {
                                        var rads = new RadComboBox();
                                        cell.Controls.Add(rads);
                                        rads.ID = "RCB_Field" + cellIndex;
                                        rads.Height = Unit.Pixel(300);
                                        rads.Width = Unit.Pixel(100);
                                        rads.ItemTemplate = LoadTemplate("~/UserControl/ChildFieldIControl.ascx");
                                        rads.DataSource = fieldInfo.ChildFields;
                                        rads.DataTextField = "FieldValue";
                                        rads.DataValueField = "FieldId";
                                        rads.DataBind();
                                        rads.HighlightTemplatedItems = true;
                                    }
                                    else
                                    {
                                        var rads = new RadComboBox();
                                        cell.Controls.Add(rads);
                                        rads.Height = Unit.Pixel(300);
                                        rads.Width = Unit.Pixel(100);
                                        rads.ItemTemplate = LoadTemplate("~/UserControl/ChildFieldIControl.ascx");
                                        rads.Items.Add(new RadComboBoxItem(fieldInfo.FieldValue, fieldInfo.FieldId.ToString()));
                                    }
                                }
                            }
                        }
                        ((HiddenField)e.Item.FindControl("ItemFieldValue")).Value = hiddenValue;
                    }
                }
            }
        }

        /// <summary>
        /// 商品选择属性 RGSelectGoods
        /// </summary>
        protected DataTable GoodsTable
        {
            get
            {
                if (Session["GoodsTable"] == null)
                {
                    var dataTable = new DataTable();
                    dataTable.Columns.Add("GoodsId", typeof(Guid));
                    dataTable.Columns.Add("商品编号", typeof(string));
                    dataTable.Columns.Add("商品名称", typeof(string));
                    dataTable.PrimaryKey = new[] { dataTable.Columns[0] };
                    return dataTable;
                }
                return (DataTable)Session["GoodsTable"];
            }
            set
            {
                Session["GoodsTable"] = value;
            }
        }

        #region  [搜索商品]

        //选择分类
        protected void GoodsClass_SelectedIndexChanged(object source, EventArgs e)
        {
            HFSonGoods.Value = "";
            Guid goodsClass = Guid.Empty;
            if (!string.IsNullOrEmpty(RCB_GoodsClass.SelectedValue))
            {
                goodsClass = new Guid(RCB_GoodsClass.SelectedValue);
            }

            DataTable dataTable = GoodsTable;
            dataTable.Clear();
            if (dataTable.Columns.Count > 4)
            {
                for (int cols = dataTable.Columns.Count; cols > 4; cols--)
                    dataTable.Columns.RemoveAt(cols - 1);
            }

            //获取分类属性，根据分类属性生成表列
            IList<FieldInfo> fieldList = _goodsCenterSao.GetFieldListByGoodsClassId(goodsClass).ToList();
            foreach (FieldInfo fieldInfo in fieldList)
            {
                dataTable.Columns.Add(fieldInfo.FieldName, typeof(Guid));
            }
            //根据分类包含产品生成表行
            IList<GoodsInfo> goodsInfoList = _goodsCenterSao.GetGoodsInfoListSimpleByClassId(goodsClass, string.Empty);
            foreach (GoodsInfo goodsInfo in goodsInfoList)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow[0] = goodsInfo.GoodsId;
                dataRow[1] = goodsInfo.GoodsCode;
                dataRow[2] = goodsInfo.GoodsName;

                fieldList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsInfo.GoodsId).ToList();
                foreach (FieldInfo fieldInfo in fieldList)
                {
                    dataRow[fieldInfo.FieldName] = fieldInfo.FieldId;
                }
                dataTable.Rows.Add(dataRow);
            }
            GoodsTable = dataTable;

            RGSelectGoods.Rebind();
        }

        private void RcbGoodsItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text) && e.Text.Length >= 2)
            {
                //string[] key = { e.Text };
                //IList<GoodsInfo> goodsBaseList = goodsManager.GetGoodsListBySearch(key);
                //var goodsBaseListExceptBackage = goodsBaseList.Where(p => p.IsCompGoods != 2).ToList();
                var dic = _goodsCenterSao.GetGoodsSelectList(e.Text);
                Int32 totalCount = dic.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    //RadComboBoxItem item;
                    //foreach (GoodsInfo goodsBaseInfo in goodsBaseListExceptBackage)
                    //{
                    //    item = new RadComboBoxItem();
                    //    item.Text = goodsBaseInfo.GoodsName;
                    //    item.Value = goodsBaseInfo.GoodsId.ToString();
                    //    combo.Items.Add(item);
                    //}
                    foreach (var keyValuePair in dic)
                    {
                        var item = new RadComboBoxItem { Text = keyValuePair.Value, Value = keyValuePair.Key };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        protected void Goods_SelectedIndexChanged(object obj, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(RCB_Goods.SelectedValue))
            {
                HFSonGoods.Value = "";
                var goodsId = new Guid(RCB_Goods.SelectedValue);
                GoodsInfo goodsBaseInfo = _goodsCenterSao.GetGoodsBaseInfoById(goodsId);
                if (goodsBaseInfo != null && !string.IsNullOrEmpty(goodsBaseInfo.GoodsName))
                {
                    DataTable dataTable = GoodsTable;

                    //获取产品属性，根据分类属性生成表列
                    IList<FieldInfo> fieldList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsId).ToList();

                    foreach (FieldInfo fieldInfo in fieldList)
                    {
                        if (!dataTable.Columns.Contains(fieldInfo.FieldName))
                            dataTable.Columns.Add(fieldInfo.FieldName, typeof(Guid));
                    }
                    if (!dataTable.Rows.Contains(goodsBaseInfo.GoodsId))
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow[0] = goodsBaseInfo.GoodsId;
                        dataRow[1] = goodsBaseInfo.GoodsCode;
                        dataRow[2] = goodsBaseInfo.GoodsName;
                        foreach (FieldInfo fieldInfo in fieldList)
                        {
                            dataRow[fieldInfo.FieldName] = fieldInfo.FieldId;
                        }
                        dataTable.Rows.InsertAt(dataRow, dataTable.Rows.Count);
                        GoodsTable = dataTable;
                    }
                }
            }
            RGSelectGoods.Rebind();
        }
        #endregion

        #region [添加产品]
        protected void SelectGoods_Click(object sender, EventArgs e)
        {
            //var purch = new PurchasingManager();
            var purchId = new Guid(Request["PurchasingID"]);
            PurchasingInfo pInfo = _purchasing.GetPurchasingById(purchId);

            IList<GridDataItem> dataItems = RGSelectGoods.Items.Cast<GridDataItem>().Where(dataItem => ((CheckBox) dataItem.FindControl("CheckGoods")).Checked).ToList();
            if (dataItems.Count == 0)
            {
                RAM.Alert("请选择待添加商品!");
            }
            var goodsIdList = new List<Guid>();
            foreach (GridDataItem dataItem in dataItems)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                if (goodsIdList.Count(w => w == goodsId) == 0)
                    goodsIdList.Add(goodsId);
            }
            IList<PurchaseSetInfo> purchaseSetList = _purchaseSet.GetPurchaseSetInfoList(goodsIdList, pInfo.WarehouseID,pInfo.PurchasingFilialeId);
            var goodsList = _goodsCenterSao.GetGoodsListByGoodsIds(goodsIdList);
            Dictionary<string, List<string>> dicFiled = CreateFiledGoods(HFSonGoods.Value);
            IList<PurchasingDetailInfo> detailList = PurchasingDetailList;
            foreach (GridDataItem dataItem in dataItems)
            {
                string dataItemClientId = dataItem.OriginalClientID;
                var selectedGoodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                GoodsInfo goodsInfo = goodsList.FirstOrDefault(w => w.GoodsId == selectedGoodsId) ?? new GoodsInfo();
                PurchaseSetInfo purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == selectedGoodsId);
                if (purchaseSetInfo == null || purchaseSetInfo.GoodsId == Guid.Empty)
                {
                    RAM.Alert(goodsInfo.GoodsName + "未添加商品采购设置");
                    return;
                }

                var dicSelectedField = new Dictionary<Guid, List<Guid>>();
                if (dataItem.Cells.Count >= 6)
                {
                    for (int i = 6; i < dataItem.Cells.Count; i++)
                    {
                        var txt = dataItem.Cells[i].Text.Trim().Replace("&nbsp;", "");
                        if (!string.IsNullOrEmpty(txt))
                        {
                            var fieldId = new Guid(txt);
                            string key = dataItemClientId + "_RCB_Field" + i;
                            foreach (var keyValuePair in dicFiled)
                            {
                                if (key == keyValuePair.Key)
                                {
                                    var list = keyValuePair.Value.Select(str => new Guid(str)).ToList();
                                    dicSelectedField.Add(fieldId, list);
                                    break;
                                }
                            }
                        }
                    }
                }
                IList<ChildGoodsInfo> childGoodsList = new List<ChildGoodsInfo>();
                if (dicSelectedField.Count > 0)
                {
                    childGoodsList = _goodsCenterSao.GetRealGoodsListByGoodsIdAndFields(selectedGoodsId, dicSelectedField);
                }
                else
                {
                    var childGoodsInfo = new ChildGoodsInfo
                    {
                        GoodsId = goodsInfo.GoodsId,
                        RealGoodsId = goodsInfo.GoodsId
                    };
                    childGoodsList.Add(childGoodsInfo);
                }

                foreach (var childGoodsInfo in childGoodsList)
                {
                    decimal price = purchaseSetInfo.PurchasePrice <= 0 ? -1 : purchaseSetInfo.PurchasePrice;
                    var detailInfo = new PurchasingDetailInfo(Guid.NewGuid(), childGoodsInfo.RealGoodsId, goodsInfo.GoodsName, goodsInfo.Units, goodsInfo.GoodsCode, childGoodsInfo.Specification, Guid.Empty, price, 1, 0, (int)YesOrNo.No, "", Guid.NewGuid(), Cbx_GoodsType.Checked ? (int)PurchasingGoodsType.Gift : (int)PurchasingGoodsType.NoGift);
                    // 获取商品的60、30、11天销量
                    var purchasingDetailInfo = _purchasingDetail.GetChildGoodsSale(childGoodsInfo.RealGoodsId, pInfo.WarehouseID, DateTime.Now, pInfo.PurchasingFilialeId);
                    if (purchasingDetailInfo != null)
                    {
                        detailInfo.SixtyDaySales = purchasingDetailInfo.SixtyDaySales;
                        detailInfo.ThirtyDaySales = purchasingDetailInfo.ThirtyDaySales;
                        detailInfo.ElevenDaySales = purchasingDetailInfo.ElevenDaySales / 11;//日均销量(11天)
                    }
                    else
                    {
                        detailInfo.SixtyDaySales = 0;
                        detailInfo.ThirtyDaySales = 0;
                        detailInfo.ElevenDaySales = 0;
                    }
                    //保留输入记录的采购数量
                    foreach (GridDataItem item in Rgd_PurchasingDetail.Items)
                    {
                        var goodsId = (Guid)item.GetDataKeyValue("GoodsID");
                        if (goodsId == childGoodsInfo.RealGoodsId)
                        {
                            detailInfo.PlanQuantity = double.Parse(((TextBox)item.FindControl("tbx_quantity")).Text);
                            break;
                        }
                    }
                    if (PurchasingDetailList.Count >= 1)
                    {
                        ChildGoodsInfo info = childGoodsInfo;
                        if (PurchasingDetailList.Where(p => p.GoodsID == info.RealGoodsId).Count(p => p.PurchasingGoodsType == (Cbx_GoodsType.Checked ? (int)PurchasingGoodsType.Gift : (int)PurchasingGoodsType.NoGift)) == 0)
                        {
                            detailList.Add(detailInfo);
                        }
                        else
                        {
                            detailList.First(d => d.GoodsID == childGoodsInfo.RealGoodsId).PlanQuantity = detailInfo.PlanQuantity;
                        }
                    }
                    else
                    {
                        detailList.Add(detailInfo);
                    }
                }
            }
            PurchasingDetailList = detailList;
            Rgd_PurchasingDetail.DataSource = PurchasingDetailList;
            Rgd_PurchasingDetail.Rebind();
        }
        #endregion

        #region[保存]
        /// <summary>
        /// 保存 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button_InsterStock(object sender, EventArgs e)
        {
            if (!CanSubmit())
            {
                RAM.Alert("请不要重复提交!");
                return;
            }
            ExecuteSubmit((ctx) =>
            {
                var purchId = new Guid(Request["PurchasingID"]);
                PurchasingInfo pInfo = _purchasing.GetPurchasingById(purchId);
                var purchasingDetailManager = new PurchasingDetailManager(_purchasingDetail, null);
                IList<PurchasingDetailInfo> plist = (from GridDataItem dataItem in Rgd_PurchasingDetail.Items
                                                     let goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString())
                                                     let planQuantity = ((TextBox)dataItem.FindControl("tbx_quantity")).Text
                                                     let goodsName = dataItem.GetDataKeyValue("GoodsName").ToString()
                                                     let goodsCode = dataItem.GetDataKeyValue("GoodsCode").ToString()
                                                     let specification = dataItem.GetDataKeyValue("Specification").ToString()
                                                     let price = Convert.ToDecimal(dataItem.GetDataKeyValue("Price").ToString())
                                                     let units = dataItem.GetDataKeyValue("Units").ToString()
                                                     let purchasingGoodsType = Convert.ToInt32(dataItem.GetDataKeyValue("PurchasingGoodsType").ToString())
                                                     let info = _purchaseSet.GetPurchaseSetInfo(goodsId, pInfo.PurchasingFilialeId, pInfo.WarehouseID)
                                                     select new PurchasingDetailInfo(purchId, goodsId, goodsName, units, goodsCode, specification, pInfo.CompanyID, purchasingGoodsType == (int)PurchasingGoodsType.Gift ? 0 : price, Convert.ToDouble(planQuantity), 0, 0, "", Guid.NewGuid(), purchasingGoodsType)
                                                     {
                                                         SixtyDaySales = Convert.ToInt32(dataItem["SixtyDaySales"].Text),
                                                         ThirtyDaySales = Convert.ToInt32(dataItem["ThirtyDaySales"].Text),
                                                         ElevenDaySales = Convert.ToInt32(dataItem["ElevenDaySales"].Text),
                                                         CPrice = purchasingGoodsType == (int)PurchasingGoodsType.Gift ? 0
                                                         : (info != null ? info.PurchasePrice : price)
                                                     }).ToList();
                purchasingDetailManager.Save(plist);
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            });
        }
        protected void ButtonDelete(object sender, EventArgs e)
        {
            if (PurchasingDetailList.Count >= 1)
            {
                if (Rgd_PurchasingDetail.SelectedItems.Count == 0)
                {
                    RAM.Alert("没有选择要删除的商品!");
                    return;
                }

                foreach (GridDataItem dataItem in Rgd_PurchasingDetail.SelectedItems)
                {
                    var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                    PurchasingDetailInfo pinfo = PurchasingDetailList.Where(p => p.GoodsID == goodsId).ToList()[0];
                    PurchasingDetailList.Remove(pinfo);
                    PurchasingDetailList = PurchasingDetailList;
                }
                Rgd_PurchasingDetail.Rebind();
            }
        }

        #endregion

        private Dictionary<string, List<string>> CreateFiledGoods(string str)
        {
            var dic = new Dictionary<string, List<string>>();
            if (!string.IsNullOrEmpty(str))
            {
                string[] goodsArr = str.Split('@');
                foreach (string combGoods in goodsArr)
                {
                    string regStr = Regex.Match(combGoods, @"([^_]*_){4}([^_]*)", RegexOptions.IgnoreCase).Value;
                    if (dic.Count > 0)
                    {
                        bool isExist = false;
                        foreach (var keyValuePair in dic)
                        {
                            if (keyValuePair.Key == regStr)
                            {
                                keyValuePair.Value.Add(combGoods.Split('|')[1]);
                                isExist = true;
                            }
                        }
                        if (isExist == false)
                        {
                            var fieldList = new List<string> { combGoods.Split('|')[1] };
                            dic.Add(regStr, fieldList);
                        }
                    }
                    else
                    {
                        var fieldList = new List<string> { combGoods.Split('|')[1] };
                        dic.Add(regStr, fieldList);
                    }
                }
            }
            return dic;
        }
        
        protected string GetDescription(string goodsId)
        {
            string html = string.Empty;
            //CompanyGoodsPriceInfo cgp = companyGoodsPrice.GetCompanyGoodsPriceInfoByGoodsId(new Guid(goodsId));
            //string description = cgp == null ? string.Empty : cgp.Description;
            //if (!string.IsNullOrEmpty(description))
            //{
            //    string divId = DivDescriptionId;
            //    html = "<img id='imageRemark' src='../App_Themes/Default/images/Memo.gif' alt='' onmousemove='ShowImg(\"" + divId + "\");' onmouseout='HiddleImg(\"" + divId + "\");' />";
            //    html += "<div style='position: absolute;'>";
            //    html += "<div id='" + divId + "' style='z-index: 1000; left: -200px; top: 20px; position: relative;display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px;font-weight: bold; height: auto; overflow: visible; word-break: break-all;' runat='server'>";
            //    html += description + "</div></div>";
            //}
            return html;
        }
        //private string DivDescriptionId
        //{
        //    get
        //    {
        //        if (ViewState["DescriptionId"] == null)
        //            ViewState["DescriptionId"] = "0";
        //        int id = Convert.ToInt32(ViewState["DescriptionId"].ToString()) + 1;
        //        ViewState["DescriptionId"] = id.ToString();
        //        return "DivDescriptionId" + id;
        //    }
        //}
    }
}

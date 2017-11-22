using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using Telerik.Web.UI;
using ERP.SAL;

namespace ERP.UI.Web
{
    /// <summary>框架赠品绑定    ADD  2014-08-12   陈重文
    /// </summary>
    public partial class GoodsBindGiftManager : BasePage
    {
        //是否搜索绑定该赠品的商品  
        private static bool _isSearchGoods = true;
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _isSearchGoods = true;
                LoadGoodsKindType();
                LoadGoodsBrand();
                LoadGiftGoods();
            }
        }

        #region [加载商品类型、品牌]

        /// <summary>加载商品类型
        /// </summary>
        private void LoadGoodsKindType()
        {
            var goodsKindTypeDic = (Dictionary<int, string>)EnumAttribute.GetDict<GoodsKindType>();
            foreach (var item in goodsKindTypeDic.Where(ent => ent.Key != 0))
            {
                RCB_GoodsKingType.Items.Add(new RadComboBoxItem(item.Value, string.Format("{0}", item.Key)));
            }
            RCB_GoodsKingType.Items.Insert(0, new RadComboBoxItem("请选择类型", "0"));
        }

        /// <summary>加载商品品牌
        /// </summary>
        private void LoadGoodsBrand()
        {
            var brandList = _goodsCenterSao.GetAllBrandList();
            RCB_GoodsBrand.DataSource = brandList;
            RCB_GoodsBrand.DataTextField = "Brand";
            RCB_GoodsBrand.DataValueField = "BrandId";
            RCB_GoodsBrand.DataBind();
            RCB_GoodsBrand.Items.Insert(0, new RadComboBoxItem("请选择品牌", Guid.Empty.ToString()));
        }

        #endregion

        #region [加载赠品商品及库存]

        /// <summary>加载赠品商品
        /// </summary>
        private void LoadGiftGoods()
        {
            var goodsGiftList = _goodsCenterSao.GetAllGiftList();
            var result = WMSSao.BindGift(goodsGiftList.Select(ent => ent.Key));
            foreach (var item in goodsGiftList)
            {
                var goodsStock = result.ContainsKey(item.Key) ? result[item.Key] : 0;
                GoodsGiftList.Items.Add(new RadListBoxItem(item.Value + "  [" + goodsStock + "]", item.Key.ToString()));
            }
        }

        #endregion

        #region [搜索]

        /// <summary>搜索事件
        /// </summary>
        protected void SerachClick(object sender, EventArgs e)
        {
            var goodsKingdType = Convert.ToInt32(RCB_GoodsKingType.SelectedValue);
            var goodsBrandId = new Guid(RCB_GoodsBrand.SelectedValue);
            SearchGoodsNameOrCode = TBox_SearchGoodsNameOrCode.Text.Trim();
            if (goodsBrandId == Guid.Empty && string.IsNullOrWhiteSpace(SearchGoodsNameOrCode) && goodsKingdType == 0)
            {
                RAM.Alert("温馨提示：至少满足一个搜索条件！");
                return;
            }
            _isSearchGoods = false;
            GoodsGiftList.Items.Clear();
            int? trueGoodsKindType = null;
            Guid? trueBrandId = null;
            if (goodsKingdType != 0)
            {
                trueGoodsKindType = goodsKingdType;
            }
            if (goodsBrandId != Guid.Empty)
            {
                trueBrandId = goodsBrandId;
            }
            RCB_GoodsGift.Items.Clear();
            FiltrateGoodsList.Items.Clear();
            ConfirmGoodsList.Items.Clear();
            GoodsGiftList.Items.Clear();
            var goodsList = _goodsCenterSao.GetGoodsItemList(trueGoodsKindType, trueBrandId, SearchGoodsNameOrCode);
            if (goodsList != null && goodsList.Count > 0)
            {
                foreach (var g in goodsList)
                {
                    FiltrateGoodsList.Items.Add(new RadListBoxItem(g.GoodsName, g.GoodsId.ToString()));
                }
            }
            else
            {
                RAM.Alert("温馨提示：未找到符合条件的商品！");
            }
        }

        #endregion

        #region [商品筛选确定事件]

        /// <summary>添加到商品确定框  
        /// </summary>
        protected void AddToRight(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FiltrateGoodsList.SelectedValue))
            {
                ConfirmGoodsList.Items.Add(new RadListBoxItem(FiltrateGoodsList.SelectedItem.Text,
                                                              FiltrateGoodsList.SelectedItem.Value));
                ShowGoodsGiftList(new Guid(FiltrateGoodsList.SelectedValue));
                FiltrateGoodsList.Items.Remove(FiltrateGoodsList.SelectedItem);
            }
            else
            {
                RAM.Alert("温馨提示：请先选择数据！");
            }
        }

        /// <summary>移除到商品筛选框
        /// </summary>
        protected void RemoveToLeft(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ConfirmGoodsList.SelectedValue))
            {
                FiltrateGoodsList.Items.Add(ConfirmGoodsList.SelectedItem);
                ConfirmGoodsList.Items.Remove(ConfirmGoodsList.SelectedItem);
            }
            else
            {
                RAM.Alert("温馨提示：请先选择数据！");
            }
        }

        /// <summary>全部添加到商品确定框
        /// </summary>
        protected void AllAddToRight(object sender, EventArgs e)
        {
            if (FiltrateGoodsList.Items.Count == 0)
            {
                RAM.Alert("温馨提示：没有数据！");
            }
            else
            {
                foreach (var items in FiltrateGoodsList.Items.ToList())
                {
                    ConfirmGoodsList.Items.Add(new RadListBoxItem(items.Text, items.Value));
                }
                FiltrateGoodsList.Items.Clear();
            }
        }

        /// <summary>全部移除到商品筛选框
        /// </summary>
        protected void AllRemoveToLeft(object sender, EventArgs e)
        {
            if (ConfirmGoodsList.Items.Count == 0)
            {
                RAM.Alert("温馨提示：没有数据！");
            }
            else
            {
                foreach (var items in ConfirmGoodsList.Items.ToList())
                {
                    FiltrateGoodsList.Items.Add(new RadListBoxItem(items.Text, items.Value));
                }
                ConfirmGoodsList.Items.Clear();
            }
        }

        #endregion

        #region [删除]

        protected void DeleteClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(GoodsGiftList.SelectedValue))
            {
                GoodsGiftList.Items.Remove(GoodsGiftList.SelectedItem);
            }
            else
            {
                RAM.Alert("温馨提示：请先选择数据！");
            }
        }

        #endregion

        #region [保存]

        /// <summary>单个保存
        /// </summary>
        protected void SaveClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ConfirmGoodsList.SelectedValue))
            {
                RAM.Alert("温馨提示：请选择需要绑定的商品！");
                return;
            }
            IList<Guid> goodsId = new List<Guid> { new Guid(ConfirmGoodsList.SelectedValue) };
            SetGoodsGfit(goodsId);
        }

        /// <summary>批量保存
        /// </summary>
        protected void AllSaveClick(object sender, EventArgs e)
        {
            if (ConfirmGoodsList.Items.Count == 0)
            {
                RAM.Alert("温馨提示：没有商品需要绑定！");
                return;
            }
            IList<Guid> goodsIdList = new List<Guid>();
            foreach (RadListBoxItem item in ConfirmGoodsList.Items)
            {
                goodsIdList.Add(new Guid(item.Value));
            }
            SetGoodsGfit(goodsIdList);
        }

        /// <summary>设置商品赠品
        /// </summary>
        /// <param name="goodsIdList">需要绑定赠品的商品Id集合</param>
        private void SetGoodsGfit(IEnumerable<Guid> goodsIdList)
        {
            try
            {
                _isSearchGoods = false;
                //商品需要绑定赠品的Id集合
                List<Guid> giftGoodsIdList = GoodsGiftList.Items.Select(ent => new Guid(ent.Value)).ToList();
                //商品绑定赠品字典集合
                var dic = goodsIdList.ToDictionary(goodsId => goodsId, ent => giftGoodsIdList);
                var result = _goodsCenterSao.SetGoodsGift(dic);
                RAM.Alert(result ? "保存成功！" : "温馨提示：保存失败！");
            }
            catch (Exception ex)
            {
                RAM.Alert("更新失败！\n" + ex.Message);
            }
        }

        #endregion

        #region [赠品RCB_GoodsGift搜索选择事件]

        /// <summary>赠品RCB_GoodsGift搜索事件
        /// </summary>
        protected void RCB_GoodsGiftOnItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = o as RadComboBox;
            if (!string.IsNullOrWhiteSpace(e.Text))
            {
                var goodsList = _goodsCenterSao.GetGoodsItemList(null, null, e.Text.Trim());
                if (goodsList != null && goodsList.Count > 0)
                {
                    foreach (var goodsBaseInfo in goodsList)
                    {
                        var item = new RadComboBoxItem
                                       {
                                           Text = goodsBaseInfo.GoodsName,
                                           Value = goodsBaseInfo.GoodsId.ToString()
                                       };
                        if (combo != null) combo.Items.Add(item);
                    }
                }
            }
        }

        /// <summary>RCB_GoodsGift下拉选择事件
        /// </summary>
        protected void RCB_GoodsGiftOnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var rlbGoodsGift = new RadListBoxItem(e.Text, e.Value);
            if (!string.IsNullOrEmpty(e.Value) && RCB_GoodsGift.FindItemByValue(e.Value) == null)
            {
                var item = GoodsGiftList.Items.FirstOrDefault(ent => ent.Value == rlbGoodsGift.Value);
                if (item == null)
                {
                    var goodsId = new List<Guid> { new Guid(e.Value) };
                    var result = WMSSao.BindGift(goodsId);
                    var quantity = result.ContainsKey(new Guid(rlbGoodsGift.Value)) ? result[new Guid(rlbGoodsGift.Value)] : 0;
                    GoodsGiftList.Items.Add(new RadListBoxItem(rlbGoodsGift.Text + "  [" + quantity + "]", rlbGoodsGift.Value));
                }
                RCB_GoodsGift.Text = string.Empty;
            }
        }

        #endregion

        #region [赠品GoodsGiftList选择事件 ]

        /// <summary>赠品GoodsGiftList选择事件 
        /// </summary>
        protected void GoodsGiftListOnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_isSearchGoods)
                {
                    var giftGoodsId = new Guid(GoodsGiftList.SelectedValue);
                    if (giftGoodsId != Guid.Empty)
                    {
                        var result = _goodsCenterSao.GetGoodsListByGiftID(giftGoodsId);
                        if (result != null && result.Count > 0)
                        {
                            ConfirmGoodsList.Items.Clear();
                            foreach (var item in result)
                            {
                                ConfirmGoodsList.Items.Add(new RadListBoxItem(item.Value, item.Key.ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                RAM.Alert("温馨提示：操作失败！");
            }
        }

        #endregion

        #region [商品确定框选择事件]

        protected void ConfirmGoodsListOnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_isSearchGoods)
                {
                    var goodsId = new Guid(ConfirmGoodsList.SelectedValue);
                    ShowGoodsGiftList(goodsId);
                }
            }
            catch (Exception)
            {
                RAM.Alert("温馨提示：操作失败！");
            }
        }

        protected void ShowGoodsGiftList(Guid goodsId)
        {
            try
            {
                if (goodsId != Guid.Empty)
                {
                    var result = _goodsCenterSao.GetGoodsGiftList(goodsId);
                    GoodsGiftList.Items.Clear();
                    if (result != null && result.Count > 0)
                    {
                        var goodsStockDic = WMSSao.BindGift(result.Select(ent => ent.Key));
                        foreach (var item in result)
                        {
                            var quantity = goodsStockDic.ContainsKey(item.Key) ? goodsStockDic[item.Key] : 0;
                            GoodsGiftList.Items.Add(new RadListBoxItem(item.Value + "  [" + quantity + "]", item.Key.ToString()));
                        }
                    }
                }
            }
            catch (Exception)
            {
                RAM.Alert("温馨提示：操作失败！");
            }
        }

        #endregion

        #region [导出EXCEL]

        /// <summary>导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ExportExcelClick(object sender, EventArgs e)
        {
            var goodsKingdType = Convert.ToInt32(RCB_GoodsKingType.SelectedValue);
            var goodsBrandId = new Guid(RCB_GoodsBrand.SelectedValue);
            SearchGoodsNameOrCode = TBox_SearchGoodsNameOrCode.Text.Trim();
            if (goodsBrandId == Guid.Empty && string.IsNullOrWhiteSpace(SearchGoodsNameOrCode) && goodsKingdType == 0)
            {
                RAM.Alert("温馨提示：至少满足一个搜索条件！");
                return;
            }
            _isSearchGoods = false;
            int? trueGoodsKindType = null;
            Guid? trueBrandId = null;
            if (goodsKingdType != 0)
            {
                trueGoodsKindType = goodsKingdType;
            }
            if (goodsBrandId != Guid.Empty)
            {
                trueBrandId = goodsBrandId;
            }
            var goodsList = _goodsCenterSao.GetGoodsListAndGiftList(trueGoodsKindType, trueBrandId, SearchGoodsNameOrCode);
            if (goodsList != null && goodsList.Count > 0)
            {
                OutPutExcel(goodsList);
            }
        }

        /// <summary>导出EXCEL
        /// </summary>
        public void OutPutExcel(Dictionary<GoodsBaseModel, List<GoodsBaseModel>> dic)
        {
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = new HSSFSheet[1]; // 增加sheet。

            #region Excel样式

            //标题样式styletitle
            HSSFFont fonttitle = workbook.CreateFont();
            fonttitle.FontHeightInPoints = 12;
            fonttitle.Color = HSSFColor.RED.index;
            HSSFCellStyle styletitle = workbook.CreateCellStyle();
            styletitle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderRight = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderTop = HSSFCellStyle.BORDER_THIN;
            styletitle.SetFont(fonttitle);
            //内容字体styleContent
            HSSFFont fontcontent = workbook.CreateFont();
            fontcontent.FontHeightInPoints = 9;
            fontcontent.Color = HSSFColor.BLACK.index;
            HSSFCellStyle styleContent = workbook.CreateCellStyle();
            styleContent.BorderBottom = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderLeft = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderRight = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderTop = HSSFCellStyle.BORDER_THIN;
            styleContent.SetFont(fontcontent);
            //总计 styletotal
            HSSFFont fonttotal = workbook.CreateFont();
            fonttotal.FontHeightInPoints = 12;
            fonttotal.Color = HSSFColor.RED.index;
            fonttotal.Boldweight = 2;
            HSSFCellStyle styletotal = workbook.CreateCellStyle();
            styletotal.SetFont(fonttotal);

            #endregion


            #region [模板及sheet名字]


            sheet[0] = workbook.CreateSheet("框架赠品绑定" + DateTime.Now.ToString("yyyy-MM-dd")); //添加sheet名
            sheet[0].DefaultColumnWidth = 30;
            sheet[0].DefaultRowHeight = 20;

            HSSFRow rowtitle = sheet[0].CreateRow(0);
            HSSFCell celltitie = rowtitle.CreateCell(0);
            celltitie.SetCellValue("框架赠品绑定" + DateTime.Now.ToString("yyyy-MM-dd"));
            HSSFCellStyle style = workbook.CreateCellStyle();
            style.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HSSFFont font = workbook.CreateFont();
            font.FontHeightInPoints = 20;
            font.Color = HSSFColor.BLACK.index;
            font.Boldweight = 2;
            style.SetFont(font);
            celltitie.CellStyle = style;
            sheet[0].AddMergedRegion(new Region(0, 0, 0, 7));

            #endregion


            #region [列名]

            HSSFRow rowtitles = sheet[0].CreateRow(1);
            HSSFCell ct1 = rowtitles.CreateCell(0);
            HSSFCell ct2 = rowtitles.CreateCell(1);
            ct1.SetCellValue("商品名名称");
            ct2.SetCellValue("赠品商品名称");
            ct1.CellStyle = styletitle;
            ct2.CellStyle = styletitle;
            #endregion


            int row = 2;
            foreach (var item in dic.Keys)
            {
                HSSFRow rowt = sheet[0].CreateRow(row);
                HSSFCell c1 = rowt.CreateCell(0);
                HSSFCell c2 = rowt.CreateCell(1);
                c1.SetCellValue(item.GoodsName); //商品名
                var giftList = dic.FirstOrDefault(ent => ent.Key.GoodsId == item.GoodsId).Value;
                StringBuilder giftGoodsName = new StringBuilder();
                foreach (var goodsBaseModel in giftList)
                {
                    giftGoodsName.Append(" [ ").Append(goodsBaseModel.GoodsName).Append(" ] ");
                }
                c2.SetCellValue(giftGoodsName.ToString()); //赠品商品
                c1.CellStyle = styleContent;
                c2.CellStyle = styleContent;
                c1.CellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
                row++;

            }
            sheet[0].DisplayGridlines = false;


            workbook.Write(ms);
            Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            Response.AddHeader("Content-Disposition",
                               "attachment; filename=" +
                               HttpUtility.UrlEncode("框架赠品绑定" + DateTime.Now.ToString("yyyyMMdd") + ".xls",
                                                     Encoding.UTF8));
            Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();

            GC.Collect();


        }
        #endregion

        #region [ViewState]

        /// <summary>订单编号
        /// </summary>
        public string SearchGoodsNameOrCode
        {
            set { ViewState["SearchGoodsNameOrCode"] = value; }
            get
            {
                return ViewState["SearchGoodsNameOrCode"] == null
                           ? string.Empty
                           : ViewState["SearchGoodsNameOrCode"].ToString();
            }
        }

        #endregion
    }
}

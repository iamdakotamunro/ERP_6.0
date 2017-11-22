using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using OperationLog.Core;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>商品采购设置
    /// </summary>
    public partial class GoodsPurchaseSet : BasePage
    {
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyPurchaseGoupDao _companyPurchaseGoupDao = new CompanyPurchaseGoupDao(GlobalConfig.DB.FromType.Read);
        protected bool IsDirect;

        protected bool IsLoad
        {
            get { return ViewState["IsLoad"] == null || Convert.ToBoolean(ViewState["IsLoad"]); }
            set { ViewState["IsLoad"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //绑定责任人
                RCB_Persion.DataSource = PersonnelList;
                RCB_Persion.DataBind();
                //绑定供应商
                RCB_Company.DataSource = CompanyCussentList;
                RCB_Company.DataBind();
                BindAuditType();
                IsLoad = true;
                Session["IsLoad"] = "1";
                RCB_PurchaseGroup.Items.Add(new RadComboBoxItem("默认", Guid.Empty.ToString()));
            }
            else
            {
                IsLoad = Session["IsLoad"] == null || Session["IsLoad"].ToString() != "0";
            }
        }

        #region [自定义]
        /// <summary>
        /// 获取(采购部)所有员工的信息
        /// </summary>
        /// <returns></returns>
        protected IList<PersonnelInfo> PersonnelList
        {
            get
            {
                if (ViewState["PersonnelList"] == null)
                {
                    // var filialeId = new Guid("06B30857-82F5-45F5-8768-79BD4211806C");
                    //var branchId = new Guid("E50EDCB7-2124-4DB4-AB62-5E8D8CCD8C2B");//采购部
                    var systemBranchId = new Guid("D9D6002C-196C-4375-B41A-E7040FE12B09"); //系统部门ID
                    var systemPostionList = MISService.GetAllSystemPositionList().ToList();
                    var positonIds = systemPostionList.Where(
                        act => act.ParentSystemBranchID == systemBranchId || act.SystemBranchID == systemBranchId)
                        .Select(act => act.SystemBranchPositionID);
                    IList<PersonnelInfo> list = new PersonnelSao().GetList().Where(ent => positonIds.Contains(ent.SystemBrandPositionId) && ent.IsActive).ToList();
                    ViewState["PersonnelList"] = list;
                }
                return (IList<PersonnelInfo>)ViewState["PersonnelList"];
            }
        }

        /// <summary>
        ///  获取供应商集合
        /// </summary>
        /// <returns></returns>
        protected IList<CompanyCussentInfo> CompanyCussentList
        {
            get
            {
                if (ViewState["CompanyCussentList"] == null)
                {
                    ViewState["CompanyCussentList"] = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers, State.Enable).ToList();
                }
                return (IList<CompanyCussentInfo>)ViewState["CompanyCussentList"];
            }
        }

        /// <summary>
        /// 获取所有仓库
        /// </summary>
        /// <returns></returns>
        protected Dictionary<Guid, String> WarehouseList
        {
            get
            {
                if (ViewState["WarehouseList"] == null)
                {
                    ViewState["WarehouseList"] = WMSSao.GetAllCanUseWarehouseDics();
                }
                return (Dictionary<Guid, String>)ViewState["WarehouseList"];
            }
        }

        #endregion

        #region[商品类型]
        ///<summary>
        /// 审核状态绑定
        ///</summary>
        public void BindAuditType()
        {
            var saleStockTypes = (Dictionary<int, string>)EnumAttribute.GetDict<PurchaseSetLogStatue>();

            foreach (KeyValuePair<int, string> kvp in saleStockTypes)
            {
                var item = new RadComboBoxItem(EnumAttribute.GetKeyName((PurchaseSetLogStatue)kvp.Key), string.Format("{0}", kvp.Key));
                RCB_AuditStatue.Items.Add(item);
            }
            RCB_AuditStatue.Items.Insert(0, new RadComboBoxItem("全部", "-1"));
        }
        #endregion

        protected void RCB_Company_OnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text;
                IList<CompanyCussentInfo> companyList = CompanyCussentList.Where(p => p.CompanyName.IndexOf(key, StringComparison.Ordinal) > -1).ToList();
                if (e.NumberOfItems >= companyList.Count)
                {
                    e.EndOfItems = true;
                }
                else
                {
                    foreach (CompanyCussentInfo i in companyList)
                    {
                        var item = new RadComboBoxItem { Text = i.CompanyName, Value = i.CompanyId.ToString() };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        protected void RCB_Company_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var strCompanyId = RCB_Company.SelectedValue;
            RCB_PurchaseGroup.Items.Clear();
            if (!string.IsNullOrEmpty(strCompanyId))
            {
                var purchaseGoupList = _companyPurchaseGoupDao.GetCompanyPurchaseGoupList(new Guid(strCompanyId));
                if (purchaseGoupList.Count == 0)
                {
                    RCB_PurchaseGroup.Items.Add(new RadComboBoxItem("默认", Guid.Empty.ToString()));
                }
                else
                {
                    foreach (var info in purchaseGoupList)
                    {
                        RCB_PurchaseGroup.Items.Add(new RadComboBoxItem(info.PurchaseGroupName, info.PurchaseGroupId.ToString()));
                    }
                }
            }
            else
            {
                RCB_PurchaseGroup.Items.Add(new RadComboBoxItem("默认", Guid.Empty.ToString()));
            }
        }

        protected void Ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_PurchaseSet, e);
        }

        protected void RgPurchaseSet_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            string goodsName = tbGoodsName.Text;
            string strCompanyId = RCB_Company.SelectedValue;
            string strPersonId = RCB_Persion.SelectedValue;
            int statue = int.Parse(RCB_AuditStatue.SelectedValue);
            Guid companyId = Guid.Empty;
            if (!string.IsNullOrEmpty(strCompanyId) && strCompanyId != Guid.Empty.ToString())
            {
                companyId = new Guid(strCompanyId);
            }
            Guid personId = Guid.Empty;
            if (!string.IsNullOrEmpty(strPersonId))
            {
                personId = new Guid(strPersonId);
            }
            int filingForm = int.Parse(RCB_FilingForm.SelectedValue);
            int stockUpDay = int.Parse(RCB_StockUpDay.SelectedValue);
            long recordCount;
            var startPage = RG_PurchaseSet.CurrentPageIndex + 1;
            int pageSize = RG_PurchaseSet.PageSize;

            var goodsIdList = new List<Guid>();

            if (!string.IsNullOrWhiteSpace(goodsName))
            {
                var goodinfo = _goodsCenterSao.GetGoodsItemListByGoodsNameOrCode(goodsName);
                if (goodinfo != null)
                {
                    goodsIdList.AddRange(goodinfo.Select(info => info.GoodsId));
                }
            }

            var list = _purchaseSet.GetPurchaseSetListToPage(IsLoad, goodsIdList, goodsName, companyId, personId, filingForm, stockUpDay, statue, startPage, pageSize, out recordCount);
            RG_PurchaseSet.DataSource = list;
            RG_PurchaseSet.VirtualItemCount = (int)recordCount;
        }

        /// <summary>
        /// 获取供应商名称
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        protected string GetCompanyName(Guid companyId)
        {
            string companyName = "-";
            CompanyCussentInfo info = _companyCussent.GetCompanyCussent(companyId);
            if (info != null)
            {
                companyName = info.CompanyName;
            }
            return companyName;
        }
        /// <summary>
        /// 获取责任人名称
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        protected string GetPersonName(Guid personId)
        {
            string personName = "-";
            PersonnelInfo info = PersonnelList.FirstOrDefault(w => w.PersonnelId == personId);
            if (info != null)
            {
                personName = info.RealName;
            }
            return personName;
        }
        /// <summary>
        /// 获取仓库名称
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        protected string GetWarehouseName(Guid warehouseId)
        {
            return WarehouseList.ContainsKey(warehouseId)
                ? WarehouseList[warehouseId]
                : "";
        }

        /// <summary>
        /// 报备形式
        /// </summary>
        /// <returns></returns>
        protected string GetFilingForm(int filingForm)
        {
            return filingForm == 1 ? "常规" : "触发报备";
        }

        /// <summary>
        /// 备货日
        /// </summary>
        /// <param name="stockUpDay"></param>
        /// <returns></returns>
        protected string GetStockUpDay(int stockUpDay)
        {
            return stockUpDay == 1 ? "周一" : "周三";
        }

        /// <summary>
        /// 备货量
        /// </summary>
        /// <param name="filingForm"></param>
        /// <param name="filingTrigger"></param>
        /// <returns></returns>
        protected string GetStockUpQuantity(int filingForm, int filingTrigger)
        {
            return filingForm == 1 ? "-" : string.Format("{0}", filingTrigger);
        }

        protected void IbtnSearch_Click(object sender, ImageClickEventArgs e)
        {
            IsLoad = false;
            Session["IsLoad"] = "0";
            RG_PurchaseSet.CurrentPageIndex = 0;
            RG_PurchaseSet.Rebind();
        }

        protected void BtnUpdatePurchaseGroup_OnClick(object sender, EventArgs e)
        {
            var strCompanyId = RCB_Company.SelectedValue;
            var strPurchaseGroup = RCB_PurchaseGroup.SelectedValue;
            if (string.IsNullOrEmpty(strCompanyId) || strCompanyId == Guid.Empty.ToString())
            {
                RAM.Alert("请选择供应商");
                return;
            }
            if (RG_PurchaseSet.SelectedItems.Count == 0)
            {
                RAM.Alert("请勾选商品");
                return;
            }
            var goodsIds = (from GridDataItem dataItem in RG_PurchaseSet.SelectedItems
                            let goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString())
                            let companyId = new Guid(dataItem.GetDataKeyValue("CompanyId").ToString())
                            where companyId == new Guid(strCompanyId)
                            select goodsId).ToList();
            if (goodsIds.Count == 0)
            {
                RAM.Alert("请选择相同供应商的商品");
                return;
            }

            string errorMessage;
            var result = _purchaseSet.UpdatePurchaseSetToPurchaseGroupId(goodsIds, new Guid(strCompanyId), new Guid(strPurchaseGroup), out errorMessage);
            if (result > 0)
                RG_PurchaseSet.Rebind();
        }

        protected void RG_PurchaseSet_GridExporting(object source, GridExportingArgs e)
        {
            e.ExportOutput = e.ExportOutput.Replace("\"0", "=\"0");
        }

        protected void LbxlsClick(object sender, EventArgs e)
        {
            int count = RG_PurchaseSet.SelectedItems.Count;
            if (count == 0)
            {
                RAM.Alert("请选择要导出的商品!");
                return;
            }

            var compGoodsIdList = new List<Guid>();
            foreach (GridDataItem dataItem in RG_PurchaseSet.SelectedItems)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                if (compGoodsIdList.Count(w => w == goodsId) == 0)
                    compGoodsIdList.Add(goodsId);
            }
            IList<PurchaseSetInfo> purchaseSetList = _purchaseSet.GetPurchaseSetList(compGoodsIdList, Guid.Empty);
            OutPutExcel(purchaseSetList);
        }

        /// <summary>
        /// 功能添加:导出供应商Excel
        /// 时    间:2012-2-6
        /// </summary>
        /// <param name="slist"></param>
        public void OutPutExcel(IList<PurchaseSetInfo> slist)
        {
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = new HSSFSheet[1];// 增加sheet。

            #region Excel样式
            //标题样式styletitle
            HSSFFont fonttitle = workbook.CreateFont();
            fonttitle.FontHeightInPoints = 12;
            fonttitle.Color = HSSFColor.RED.index;
            fonttitle.Boldweight = 1;
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
            fonttotal.Color = HSSFColor.BLACK.index;
            fonttotal.Boldweight = 2;
            HSSFCellStyle styletotal = workbook.CreateCellStyle();
            styletotal.SetFont(fonttotal);
            #endregion

            #region 将值插入sheet

            sheet[0] = workbook.CreateSheet("商品采购列表");//添加sheet名
            sheet[0].DefaultColumnWidth = 50;
            sheet[0].DefaultRowHeight = 20;

            HSSFRow rowtitle = sheet[0].CreateRow(0);
            HSSFCell celltitie = rowtitle.CreateCell(0);
            const string SHEET_TITLE = "商品采购列表";
            celltitie.SetCellValue(SHEET_TITLE);
            HSSFCellStyle style = workbook.CreateCellStyle();
            style.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HSSFFont font = workbook.CreateFont();
            font.FontHeightInPoints = 20;
            font.Color = HSSFColor.BLACK.index;
            font.Boldweight = 2;
            style.SetFont(font);
            celltitie.CellStyle = style;
            sheet[0].AddMergedRegion(new Region(0, 0, 0, 2));

            #region //列名
            HSSFRow rowTitle = sheet[0].CreateRow(3);
            HSSFCell cell1 = rowTitle.CreateCell(0);
            HSSFCell cell2 = rowTitle.CreateCell(1);
            HSSFCell cell3 = rowTitle.CreateCell(2);
            HSSFCell cell4 = rowTitle.CreateCell(3);
            HSSFCell cell5 = rowTitle.CreateCell(4);
            HSSFCell cell6 = rowTitle.CreateCell(5);
            HSSFCell cell7 = rowTitle.CreateCell(6);
            HSSFCell cell8 = rowTitle.CreateCell(7);
            HSSFCell cell9 = rowTitle.CreateCell(8);
            cell1.SetCellValue("商品名称");
            cell2.SetCellValue("供应商");
            cell3.SetCellValue("分组");
            cell4.SetCellValue("仓库");
            cell5.SetCellValue("采购价");
            cell6.SetCellValue("报备形式");
            cell7.SetCellValue("备货日");
            cell8.SetCellValue("备货量");
            cell9.SetCellValue("责任人");
            cell1.CellStyle = styletitle;
            cell2.CellStyle = styletitle;
            cell3.CellStyle = styletitle;
            cell4.CellStyle = styletitle;
            cell5.CellStyle = styletitle;
            cell6.CellStyle = styletitle;
            cell7.CellStyle = styletitle;
            cell8.CellStyle = styletitle;
            cell9.CellStyle = styletitle;
            #endregion

            #region //内容
            int row = 4;

            if (slist.Count > 0)
            {
                foreach (PurchaseSetInfo purchaseSetInfo in slist)
                {
                    HSSFRow rowt = sheet[0].CreateRow(row);
                    HSSFCell c1 = rowt.CreateCell(0);
                    HSSFCell c2 = rowt.CreateCell(1);
                    HSSFCell c3 = rowt.CreateCell(2);
                    HSSFCell c4 = rowt.CreateCell(3);
                    HSSFCell c5 = rowt.CreateCell(4);
                    HSSFCell c6 = rowt.CreateCell(5);
                    HSSFCell c7 = rowt.CreateCell(6);
                    HSSFCell c8 = rowt.CreateCell(7);
                    HSSFCell c9 = rowt.CreateCell(8);
                    string purchaseGroupName = "默认";
                    var list = _companyPurchaseGoupDao.GetCompanyPurchaseGoupList(purchaseSetInfo.CompanyId);
                    if (list.Count > 0)
                    {
                        var item = list.FirstOrDefault(ent => ent.PurchaseGroupId == purchaseSetInfo.PurchaseGroupId);
                        if (item != null)
                        {
                            purchaseGroupName = item.PurchaseGroupName;
                        }
                    }
                    c1.SetCellValue(purchaseSetInfo.GoodsName);//商品名
                    c2.SetCellValue(GetCompanyName(purchaseSetInfo.CompanyId));//供应商
                    c3.SetCellValue(purchaseGroupName);//分组
                    c4.SetCellValue(GetWarehouseName(purchaseSetInfo.WarehouseId));//仓库
                    c5.SetCellValue(purchaseSetInfo.PurchasePrice.ToString(CultureInfo.InvariantCulture));//采购价
                    c6.SetCellValue(GetFilingForm(purchaseSetInfo.FilingForm));//报备形式
                    c7.SetCellValue(GetStockUpDay(purchaseSetInfo.StockUpDay));//备货日
                    c8.SetCellValue(GetStockUpQuantity(purchaseSetInfo.FilingForm, purchaseSetInfo.FilingTrigger));//备货量
                    c9.SetCellValue(GetPersonName(purchaseSetInfo.PersonResponsible));//责任人
                    c1.CellStyle = styleContent;
                    c2.CellStyle = styleContent;
                    c3.CellStyle = styleContent;
                    c4.CellStyle = styleContent;
                    c5.CellStyle = styleContent;
                    c6.CellStyle = styleContent;
                    c7.CellStyle = styleContent;
                    c8.CellStyle = styleContent;
                    c9.CellStyle = styleContent;
                    c1.CellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
                    row++;
                }
            }
            else
            {
                HSSFRow rtotal = sheet[0].CreateRow(row);
                HSSFCell t0 = rtotal.CreateCell(0);
                t0.SetCellValue("无数据显示");
                t0.CellStyle = styletotal;
            }
            sheet[0].DisplayGridlines = false;
            #endregion

            #region 输出
            workbook.Write(ms);
            Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("商品采购列表" + DateTime.Now.ToString("yyyyMMdd") + ".xls", Encoding.UTF8));
            Response.BinaryWrite(ms.ToArray());

            ms.Close();
            ms.Dispose();
            GC.Collect();
            #endregion

            #endregion
        }

        /// <summary>启用商品采购设置
        /// </summary>
        protected void OnClick_EnabledPurchaseSet(object sender, EventArgs e)
        {
            var items = RG_PurchaseSet.SelectedItems;
            if (items.Count == 0)
            {
                RAM.Alert("系统提示：请先选择你需要【启用】的商品采购设置！");
                return;
            }
            foreach (GridDataItem dataItem in items)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                var warehouseId = new Guid(dataItem["WarehouseId"].Text);
                var state = Convert.ToInt32(dataItem.GetDataKeyValue("IsDelete").ToString());
                if (state == (int)State.Disable)
                {
                    _purchaseSet.NewDeletePurchaseSet(goodsId, warehouseId, State.Enable);
                    //报备管理启用操作记录添加
                    var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(goodsId);
                    if (goodsInfo != null)
                    {
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsId, goodsInfo.GoodsCode,
                            OperationPoint.ReportManage.Enabled.GetBusinessInfo(), string.Empty);
                    }
                }
            }
            RG_PurchaseSet.Rebind();
        }

        /// <summary>禁用商品采购设置
        /// </summary>
        protected void OnClick_DisablePurchaseSet(object sender, EventArgs e)
        {
            var items = RG_PurchaseSet.SelectedItems;
            if (items.Count == 0)
            {
                RAM.Alert("系统提示：请先选择你需要【禁用】的商品采购设置！");
                return;
            }
            foreach (GridDataItem dataItem in RG_PurchaseSet.SelectedItems)
            {
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsId").ToString());
                var warehouseId = new Guid(dataItem["WarehouseId"].Text);
                var state = Convert.ToInt32(dataItem.GetDataKeyValue("IsDelete").ToString());
                if (state == (int)State.Enable)
                {
                    _purchaseSet.NewDeletePurchaseSet(goodsId, warehouseId, State.Disable);
                    //报备管理禁用操作记录添加
                    var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(goodsId);
                    if (goodsInfo != null)
                    {
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsId, goodsInfo.GoodsCode,
                            OperationPoint.ReportManage.Disable.GetBusinessInfo(), string.Empty);
                    }
                }
            }
            RG_PurchaseSet.Rebind();
        }

        protected void RgPurchaseSetOnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var setinfo = (PurchaseSetInfo)e.Item.DataItem;
                if (setinfo != null && IsLoad && setinfo.PromotionId != Guid.Empty)    //采购单中所有商品都满足
                {
                    e.Item.Style.Add("background-color", "#CCFF99");//绿色
                }
            }
        }
    }
}
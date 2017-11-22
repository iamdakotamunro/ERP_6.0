using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
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
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>赠品借记单
    /// </summary>
    public partial class DebitNoteManage : BasePage
    {
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly IDebitNote _debitNoteDao = new DebitNote(GlobalConfig.DB.FromType.Write);
        private readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Write);
        private readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyPurchaseGoupDao _companyPurchaseGoupDao = new CompanyPurchaseGoupDao(GlobalConfig.DB.FromType.Read);
        private readonly WarehouseManager _warehouseBll = new WarehouseManager();

        SubmitController _submitController;
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["Checking"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid(), 10);
                ViewState["Checking"] = _submitController;
            }
            return (SubmitController)ViewState["Checking"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                //绑定借记单状态
                RCB_State.DataSource = GetDebitNoteStateList();
                RCB_State.DataBind();
                //绑定仓库
                RCB_Warehouse.DataSource = WarehouseDic;
                RCB_Warehouse.DataBind();
                //绑定责任人
                RCB_Persion.DataSource = PersonnelList;
                RCB_Persion.DataBind();
                //绑定供应商
                RCB_Company.DataSource = CompanyCussentList;
                RCB_Company.DataBind();
            }
        }

        /// <summary>获取借记单状态
        /// </summary>
        /// <returns></returns>
        protected Dictionary<int, string> GetDebitNoteStateList()
        {
            var debitNoteState = (Dictionary<int, string>)EnumAttribute.GetDict<DebitNoteState>();
            return debitNoteState;
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
                    ViewState["CompanyCussentList"] = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers).Where(ent => ent.State == 1).ToList();
                }
                return (IList<CompanyCussentInfo>)ViewState["CompanyCussentList"];
            }
        }

        /// <summary>
        /// 获取所有仓库
        /// </summary>
        /// <returns></returns>
        protected Dictionary<Guid, string> WarehouseDic
        {
            get
            {
                if (ViewState["WarehouseList"] == null)
                {
                    var list = WarehouseManager.GetWarehouseDic();
                    ViewState["WarehouseList"] = list;
                }
                return (Dictionary<Guid, string>)ViewState["WarehouseList"];
            }
        }
        #endregion

        /// <summary>获取供应商名称
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        protected string GetCompanyName(Guid companyId)
        {
            string companyName = "-";
            CompanyCussentInfo info = CompanyCussentList.FirstOrDefault(w => w.CompanyId == companyId);
            if (info != null)
            {
                companyName = info.CompanyName;
            }
            return companyName;
        }

        /// <summary>获取分组名称
        /// </summary>
        /// <param name="purchaseGroupId"></param>
        /// <returns></returns>
        protected string GetPurchaseGroupIdName(object purchaseGroupId)
        {
            if (purchaseGroupId != null)
            {
                if ((Guid)purchaseGroupId == Guid.Empty)
                {
                    return "默认";
                }
                var info = _companyPurchaseGoupDao.GetCompanyPurchaseGoupInfo((Guid)purchaseGroupId);
                if (info != null)
                {
                    return info.PurchaseGroupName;
                }
                return string.Empty;
            }
            return string.Empty;
        }


        /// <summary>获取责任人名称
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

        /// <summary>获取仓库名称
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        protected string GetWarehouseName(Guid warehouseId)
        {
            if (WarehouseDic.ContainsKey(warehouseId))
            {
                return WarehouseDic[warehouseId];
            }
            return "-";
        }

        /// <summary>获取借记单状态名称
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected string GetStateName(int state)
        {
            string stateName = "-";
            Dictionary<int, string> dicList = GetDebitNoteStateList();
            if (dicList.Count > 0)
            {
                foreach (KeyValuePair<int, string> keyValue in dicList)
                {
                    if (keyValue.Key == state)
                    {
                        stateName = keyValue.Value;
                    }
                }
            }
            return stateName;
        }

        protected void RCB_Company_OnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text;
                IList<CompanyCussentInfo> companyList = CompanyCussentList.Where(p => p.CompanyName.Contains(key.Trim())).ToList();
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

        protected void Ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_DebitNote, e);
        }

        protected void RgDebitNote_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            DateTime startDate = string.IsNullOrEmpty(txt_StartDate.Text) ? DateTime.MinValue : Convert.ToDateTime(txt_StartDate.Text);
            DateTime endDate = string.IsNullOrEmpty(txt_EndDate.Text) ? DateTime.MinValue : Convert.ToDateTime(txt_EndDate.Text);
            int state = int.Parse(RCB_State.SelectedValue);
            string strWarehouseId = RCB_Warehouse.SelectedValue;
            string strCompanyId = RCB_Company.SelectedValue;
            string strPersonId = RCB_Persion.SelectedValue;
            var warehouseId = string.IsNullOrEmpty(strWarehouseId) ? Guid.Empty : new Guid(strWarehouseId);
            var companyId = string.IsNullOrEmpty(strCompanyId) ? Guid.Empty : new Guid(strCompanyId);
            var personId = string.IsNullOrEmpty(strPersonId) ? Guid.Empty : new Guid(strPersonId);

            long recordCount;
            var startPage = RG_DebitNote.CurrentPageIndex + 1;
            int pageSize = RG_DebitNote.PageSize;
            IList<DebitNoteInfo> list = _debitNoteDao.GetDebitNoteList(startDate, endDate, state, warehouseId, companyId, personId, txt_ActivityTimeStart.Text, txt_ActivityTimeEnd.Text, txt_TitleOrMemo.Text.Trim(), startPage, pageSize, out recordCount);
            RG_DebitNote.DataSource = list;
            RG_DebitNote.VirtualItemCount = (int)recordCount;
        }

        // 搜索
        protected void IbtnSearch_Click(object sender, ImageClickEventArgs e)
        {
            RG_DebitNote.CurrentPageIndex = 0;
            RG_DebitNote.Rebind();
        }

        // 采购中
        protected void IbnPurchasing_Click(object sender, EventArgs eventArgs)
        {
            bool isHave = false;
            var codeManager = new CodeManager();
            foreach (GridDataItem dataItem in RG_DebitNote.Items)
            {
                var purchasingId = new Guid(dataItem.GetDataKeyValue("PurchasingId").ToString());
                var personResponsible = new Guid(dataItem.GetDataKeyValue("PersonResponsible").ToString());
                var cbCheck = (CheckBox)dataItem.FindControl("CB_Check");
                if (cbCheck.Checked)
                {
                    using (var ts = new TransactionScope(TransactionScopeOption.Required))
                    {
                        isHave = true;
                        _debitNoteDao.UpdateDebitNoteStateByPurchasingId(purchasingId, (int)DebitNoteState.Purchasing);
                        //生成采购单
                        DebitNoteInfo debitNoteInfo = _debitNoteDao.GetDebitNoteInfo(purchasingId) ?? new DebitNoteInfo();
                        IList<DebitNoteDetailInfo> debitNoteDetailList = _debitNoteDao.GetDebitNoteDetailList(purchasingId);
                        CompanyCussentInfo companyCussentInfo = CompanyCussentList.FirstOrDefault(w => w.CompanyId == debitNoteInfo.CompanyId);
                        //var warehouseInfo = WarehouseManager.Get(debitNoteInfo.WarehouseId);
                        PersonnelInfo personnelInfo = PersonnelList.FirstOrDefault(w => w.PersonnelId == debitNoteInfo.PersonResponsible) ?? new PersonnelInfo(null);
                        PurchasingInfo oldPurchasingInfo = _purchasing.GetPurchasingById(purchasingId);
                        var realName = CurrentSession.Personnel.Get().RealName;
                        var filialeId = string.IsNullOrWhiteSpace(debitNoteInfo.PurchasingNo) || debitNoteInfo.PurchasingNo=="-"?FilialeManager.GetHeadList().First(ent => ent.Name.Contains("可得")).ID
                            : _purchasing.GetPurchasingList(debitNoteInfo.PurchasingNo).FilialeID;
                        var pInfo = new PurchasingInfo
                        {
                            PurchasingID = Guid.NewGuid(),
                            PurchasingNo = codeManager.GetCode(CodeType.PH),
                            CompanyID = debitNoteInfo.CompanyId,
                            CompanyName = companyCussentInfo == null ? "" : companyCussentInfo.CompanyName,
                            FilialeID = filialeId,
                            WarehouseID = debitNoteInfo.WarehouseId,
                            PurchasingState = (int)PurchasingState.Purchasing,
                            PurchasingType = (int)PurchasingType.Custom,
                            StartTime = DateTime.Now,
                            EndTime = DateTime.MaxValue,
                            //Description = "[采购类别:{0}赠品借记单][对应采购单号" + debitNoteInfo.PurchasingNo + "]" + CurrentSession.Personnel.Get().RealName,
                            Description = string.Format("[采购类别:{0},赠品借记单对应采购单号{1};采购人:{2}]", EnumAttribute.GetKeyName(PurchasingType.Custom), debitNoteInfo.PurchasingNo, realName),
                            PmId = personnelInfo.PersonnelId,
                            PmName = personnelInfo.RealName,
                            ArrivalTime = oldPurchasingInfo.ArrivalTime,
                            PersonResponsible = personResponsible,
                            PurchasingFilialeId = filialeId
                        };
                        IList<PurchasingDetailInfo> purchasingDetailList = new List<PurchasingDetailInfo>();
                        if (debitNoteDetailList.Count > 0)
                        {
                            List<Guid> goodsIdOrRealGoodsIdList = debitNoteDetailList.Select(w => w.GoodsId).Distinct().ToList();
                            Dictionary<Guid, GoodsInfo> dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(goodsIdOrRealGoodsIdList);
                            if (dicGoods != null && dicGoods.Count > 0)
                            {
                                foreach (var debitNoteDetailInfo in debitNoteDetailList)
                                {
                                    bool hasKey = dicGoods.ContainsKey(debitNoteDetailInfo.GoodsId);
                                    if (hasKey)
                                    {
                                        GoodsInfo goodsBaseInfo = dicGoods.FirstOrDefault(w => w.Key == debitNoteDetailInfo.GoodsId).Value;
                                        // 获取商品的60、30、11天销量
                                        var purchasingDetailInfo = _purchasingDetail.GetChildGoodsSale(debitNoteDetailInfo.GoodsId, debitNoteInfo.WarehouseId, DateTime.Now, pInfo.PurchasingFilialeId) ?? new PurchasingDetailInfo();
                                        var durchasingDetailInfo = new PurchasingDetailInfo
                                        {
                                            PurchasingID = pInfo.PurchasingID,
                                            PurchasingGoodsID = Guid.NewGuid(),
                                            GoodsID = debitNoteDetailInfo.GoodsId,
                                            GoodsName = debitNoteDetailInfo.GoodsName,
                                            GoodsCode = goodsBaseInfo.GoodsCode,
                                            Specification = debitNoteDetailInfo.Specification,
                                            CompanyID = pInfo.CompanyID,
                                            Price = debitNoteDetailInfo.Price,
                                            PlanQuantity = debitNoteDetailInfo.GivingCount,
                                            RealityQuantity = 0,
                                            State = 0,
                                            Description = "",
                                            Units = goodsBaseInfo.Units,
                                            PurchasingGoodsType = (int)PurchasingGoodsType.Gift,
                                            SixtyDaySales = purchasingDetailInfo.SixtyDaySales,
                                            ThirtyDaySales = purchasingDetailInfo.ThirtyDaySales,
                                            ElevenDaySales = purchasingDetailInfo.ElevenDaySales == 0 ? 0 : purchasingDetailInfo.ElevenDaySales, // 11 //日均销量(11天)
                                            CPrice = debitNoteDetailInfo.Price
                                        };
                                        purchasingDetailList.Add(durchasingDetailInfo);
                                    }
                                }
                            }
                        }
                        if (purchasingDetailList.Count > 0)
                        {
                            _debitNoteDao.UpdateDebitNoteNewPurchasingIdByPurchasingId(purchasingId, pInfo.PurchasingID);
                            _purchasing.PurchasingInsert(pInfo);
                            _purchasing.PurchasingUpdateIsOut(pInfo.PurchasingID);
                            //报备管理生成采购单操作记录添加
                            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, pInfo.PurchasingID, pInfo.PurchasingNo,
                                 OperationPoint.ReportManage.DebitToAddPurchasingList.GetBusinessInfo(), string.Empty);
                            var purchasingDetailManager = new PurchasingDetailManager(_purchasingDetail, _purchasing);
                            purchasingDetailManager.Save(purchasingDetailList);
                        }
                        ts.Complete();
                    }
                }
            }
            if (isHave == false)
            {
                RAM.Alert("未勾选借记单!");
            }
            else
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        // 注销
        protected void IbnLogout_Click(object sender, EventArgs eventArgs)
        {
            bool isHave = false;
            foreach (GridDataItem dataItem in RG_DebitNote.Items)
            {
                var purchasingId = new Guid(dataItem.GetDataKeyValue("PurchasingId").ToString());
                var cbCheck = (CheckBox)dataItem.FindControl("CB_Check");
                if (cbCheck.Checked)
                {
                    isHave = true;
                    _debitNoteDao.UpdateDebitNoteStateByPurchasingId(purchasingId, (int)DebitNoteState.Logout);
                }
            }
            if (isHave == false)
            {
                RAM.Alert("未勾选借记单!");
            }
            else
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        // 删除
        protected void IbDelete_Click(object sender, EventArgs eventArgs)
        {
            bool isHave = false;
            foreach (GridDataItem dataItem in RG_DebitNote.Items)
            {
                var purchasingId = new Guid(dataItem.GetDataKeyValue("PurchasingId").ToString());
                var cbCheck = (CheckBox)dataItem.FindControl("CB_Check");
                if (cbCheck.Checked)
                {
                    isHave = true;
                    _debitNoteDao.DeleteDebitNote(purchasingId);
                }
            }
            if (isHave == false)
            {
                RAM.Alert("未勾选借记单!");
            }
            else
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        protected void LbxlsClick(object sender, EventArgs e)
        {
            IList<DebitNoteInfo> pList = new List<DebitNoteInfo>();
            foreach (GridDataItem dataItem in RG_DebitNote.Items)
            {
                var cbCheck = (CheckBox)dataItem.FindControl("CB_Check");
                if (cbCheck.Checked)
                {
                    var purchasingId = new Guid(dataItem.GetDataKeyValue("PurchasingId").ToString());
                    DebitNoteInfo priceInfo = _debitNoteDao.GetDebitNoteInfo(purchasingId);

                    if (priceInfo != null)
                    {
                        pList.Add(priceInfo);
                    }
                }
            }
            if (pList.Count == 0)
            {
                RAM.Alert("请选择要导出的借记单!");
            }
            else
            {
                OutPutExcel(pList);
            }
        }

        /// <summary>导出Excel
        /// </summary>
        /// <param name="slist"></param>
        public void OutPutExcel(IList<DebitNoteInfo> slist)
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

            sheet[0] = workbook.CreateSheet("赠品借记单列表");//添加sheet名
            sheet[0].DefaultColumnWidth = 50;
            sheet[0].DefaultRowHeight = 20;

            HSSFRow rowtitle = sheet[0].CreateRow(0);
            HSSFCell celltitie = rowtitle.CreateCell(0);
            const string SHEET_TITLE = "赠品借记单列表";
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
            cell1.SetCellValue("对应采购单号");
            cell2.SetCellValue("供应商");
            cell3.SetCellValue("赠品总价");
            cell4.SetCellValue("活动开始");
            cell5.SetCellValue("活动结束");
            cell6.SetCellValue("借记单状态");
            cell7.SetCellValue("仓库");
            cell8.SetCellValue("责任人");
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
                foreach (DebitNoteInfo debitNoteInfo in slist)
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
                    c1.SetCellValue(debitNoteInfo.PurchasingNo);//对应采购单号
                    c2.SetCellValue(GetCompanyName(debitNoteInfo.CompanyId));//供应商
                    c3.SetCellValue(debitNoteInfo.PresentAmount.ToString(CultureInfo.InvariantCulture));//赠品总价
                    c4.SetCellValue(debitNoteInfo.ActivityTimeStart.ToString());//活动开始
                    c5.SetCellValue(debitNoteInfo.ActivityTimeEnd.ToString());//活动结束
                    c6.SetCellValue(EnumAttribute.GetKeyName((DebitNoteState)debitNoteInfo.State));//借记单状态
                    c7.SetCellValue(GetWarehouseName(debitNoteInfo.WarehouseId));//仓库
                    c8.SetCellValue(GetPersonName(debitNoteInfo.PersonResponsible));//责任人
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
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("赠品借记单列表" + DateTime.Now.ToString("yyyyMMdd") + ".xls", Encoding.UTF8));
            Response.BinaryWrite(ms.ToArray());

            ms.Close();
            ms.Dispose();
            GC.Collect();
            #endregion

            #endregion
        }

        #region [控制按钮是否显示]

        //查看按钮是否显示
        protected bool Ibn_PartIsVisible(object purchasingNo)
        {
            if (purchasingNo == null)
            {
                return true;
            }
            var pNo = purchasingNo.ToString();
            if (string.IsNullOrWhiteSpace(pNo))
            {
                return true;
            }
            //手动新建的赠品借记单无需查看
            return pNo != "-";
        }

        //完成按钮是否显示
        protected bool Ibn_OKIsVisible(object purchasingNo, int state)
        {
            if (purchasingNo == null)
            {
                return false;
            }
            var pNo = purchasingNo.ToString();
            if (string.IsNullOrWhiteSpace(pNo))
            {
                return false;
            }
            //手动新建的赠品借记单
            return pNo == "-" && state == (int)DebitNoteState.Purchasing;
        }

        //核销按钮是否显示
        protected bool Ibn_ChargeOffIsVisible(object purchasingNo, int state)
        {
            if (purchasingNo == null)
            {
                return false;
            }
            var pNo = purchasingNo.ToString();
            if (string.IsNullOrWhiteSpace(pNo))
            {
                return false;
            }
            //手动新建的赠品借记单
            return pNo == "-" && state == (int)DebitNoteState.WaitChargeOff;
        }

        //查看按钮是否显示（手动新建的赠品借记单）
        protected bool Ibn_LookIsVisible(object purchasingNo, int state)
        {
            if (purchasingNo == null)
            {
                return false;
            }
            var pNo = purchasingNo.ToString();
            if (string.IsNullOrWhiteSpace(pNo))
            {
                return false;
            }
            //手动新建的赠品借记单
            return pNo == "-";
        }

        #endregion

        /// <summary>手动增加的赠品借记单完成操作
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RG_DebitNote_OnItemCommand(object source, GridCommandEventArgs e)
        {
            if (!(e.Item is GridDataItem)) return;
            var dataItem = (GridDataItem)e.Item;
            var purchasingId = new Guid(dataItem.GetDataKeyValue("PurchasingId").ToString());
            var personnel = CurrentSession.Personnel.Get();
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //2015-03-12  note 完成和核销备注增加换行<br/>  陈重文 
            if ("Complete" == e.CommandName)
            {
                _debitNoteDao.UpdateDebitNoteStateByPurchasingId(purchasingId, (int)DebitNoteState.WaitChargeOff);
                var description = string.Format("[完成单据,操作人:{0};{1}]<br/><br/>", personnel.RealName, dateTime);
                _debitNoteDao.AddDebitNoteMemo(purchasingId, description);
            }
            if ("ChargeOff" == e.CommandName)
            {
                _debitNoteDao.UpdateDebitNoteStateByPurchasingId(purchasingId, (int)DebitNoteState.AllComplete);
                var description = string.Format("[核销单据,操作人:{0};{1}]<br/><br/>", personnel.RealName, dateTime);
                _debitNoteDao.AddDebitNoteMemo(purchasingId, description);
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
    }
}
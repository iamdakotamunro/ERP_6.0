using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    ///<summary>
    /// 新版本商品资料列表页面
    ///</summary>
    public partial class Information : BasePage
    {
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);
        private readonly IQualificationManager _qualificationManager = new QualificationManager(GlobalConfig.DB.FromType.Write);

        public IList<Guid> SelectedIds
        {
            get
            {
                if (ViewState["SelectedIds"] == null)
                    return new List<Guid>();
                return (IList<Guid>)ViewState["SelectedIds"];
            }
            set { ViewState["SelectedIds"] = value; }
        }

        public IList<Guid> UnSelectedIds
        {
            get
            {
                if (ViewState["UnSelectedIds"] == null)
                    return new List<Guid>();
                return (IList<Guid>)ViewState["UnSelectedIds"];
            }
            set { ViewState["UnSelectedIds"] = value; }
        } 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RCB_SearchType.SelectedValue = "3";
                CurrentSearchType = 3;
                var list = CacheCollection.Filiale.GetHeadList();

                list.Insert(0, new FilialeInfo { ID = Guid.Empty, Name = "全部" });
                DDL_Filiale.DataSource = list;
                DDL_Filiale.DataTextField = "Name";
                DDL_Filiale.DataValueField = "ID";
                DDL_Filiale.DataBind();
                InformationsGrid.Rebind();
                DDL_HaveInformation.Items.Clear();
                var dict = EnumAttribute.GetDict<SupplierCompleteType>();
                if (dict.Count>0)
                {
                    foreach (var item in dict.OrderBy(act=>act.Key))
                    {
                        DDL_HaveInformation.Items.Add(new RadComboBoxItem(item.Value, string.Format("{0}",item.Key)));
                    }
                }

                DDL_Period.Items.Clear();
                var dicts = EnumAttribute.GetDict<SupplierTimeLimitType>();
                if (dict.Count>0)
                {
                    foreach (var item in dicts.OrderBy(act=>act.Key))
                    {
                        DDL_Period.Items.Add(new RadComboBoxItem(item.Value, string.Format("{0}", item.Key)));
                    }
                }
                if (Request.QueryString["IsWarning"] != null)
                {
                    DDL_Period.SelectedValue = string.Format("{0}", (int)SupplierTimeLimitType.Expire);
                }
                divFiliale.Visible = CurrentSearchType == 1;
            }
        }

        public IList<Guid> IdentifyIds
        {
            get
            {
                if (ViewState["IdentifyIds"] == null)
                    return new List<Guid>();
                return (IList<Guid>)ViewState["IdentifyIds"];
            }
            set { ViewState["IdentifyIds"] = value; }
        }

        public int CurrentSearchType
        {
            get
            {
                if (ViewState["CurrentSearchType"] == null)
                    return 3;
                return (int)ViewState["CurrentSearchType"];
            }
            set { ViewState["CurrentSearchType"] = value; }
        }

        public IList<CompanyCussentInfo> SupplierInfoList
        {
            get
            {
                if (ViewState["SupplierInfoList"] == null)
                {
                    ViewState["SupplierInfoList"] = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers, State.Enable);
                }
                return (IList<CompanyCussentInfo>)ViewState["SupplierInfoList"];
            }
        }

        public bool IsAll
        {
            get
            {
                return ViewState["IsAll"] != null && (bool)ViewState["IsAll"];
            }
            set
            {
                ViewState["IsAll"] = value;
            }
        }

        #region[设置商品资料列表数据源]

        protected void InformationsGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var pageIndex = InformationsGrid.CurrentPageIndex + 1;
            int pageSize = InformationsGrid.PageSize;
            if (CurrentSearchType == 1) //供应商
            {
                var supplierInfos = GetSupplierGoodsInfos();
                int totalCount = supplierInfos.Count();
                var supplierInfoListPage =
                    supplierInfos.ToList().Skip((pageIndex - 1) * pageSize).Take(InformationsGrid.PageSize);
                InformationsGrid.DataSource = supplierInfoListPage;
                InformationsGrid.VirtualItemCount = totalCount;
            }
            else
            {
                int? state = null;
                //是否完整
                if (!string.IsNullOrEmpty(DDL_HaveInformation.SelectedValue) && DDL_HaveInformation.SelectedValue != "0")
                {
                    state = Convert.ToInt32(DDL_HaveInformation.SelectedValue);
                }
                //是否过期
                int? dataState =null;
                if (!IsPostBack && Request.QueryString["IsWarning"] != null)
                {
                    dataState = (int) SupplierTimeLimitType.Expire;
                }
                if(!string.IsNullOrEmpty(DDL_Period.SelectedValue) && DDL_Period.SelectedValue != "0")
                {
                    dataState = Convert.ToInt32(DDL_Period.SelectedValue);
                }
                long totalCount;
                var goodsList = _goodsCenterSao.SelectGoodsInformationInfosByPage(RCB_SelectKey.Text, TB_CertificateNumber.Text, state, dataState, pageIndex, pageSize,
                out totalCount);
                InformationsGrid.DataSource = goodsList;
                InformationsGrid.VirtualItemCount = (int)totalCount;
            }
        }
        #endregion

        //返回筛选后的SupplierGoodsInfoList
        public List<SupplierGoodsInfo> GetSupplierGoodsInfos()
        {
            int complete = string.IsNullOrEmpty(DDL_HaveInformation.SelectedValue)
                ? 0
                : Convert.ToInt32(DDL_HaveInformation.SelectedValue);
            int expire = string.IsNullOrEmpty(DDL_Period.SelectedValue) ? 0 : Convert.ToInt32(DDL_Period.SelectedValue);
            string searchKey = RCB_SelectKey.Text;
            var supplierInfoList = _companyCussent.GetSupplierGoodsInfos(CompanyType.Suppliers, State.Enable, searchKey,
                complete, expire);
            if (!string.IsNullOrEmpty(TB_CertificateNumber.Text) || (!string.IsNullOrEmpty(DDL_Filiale.SelectedValue) 
                && new Guid(DDL_Filiale.SelectedValue)!=Guid.Empty))
            {
                var tempList = new List<SupplierGoodsInfo>();
                foreach (var item in supplierInfoList)
                {
                    var supplierInfomationList = _qualificationManager.GetSupplierQualificationBySupplierId(item.ID);
                    if (supplierInfomationList.Count == 0) continue;
                    if (!string.IsNullOrEmpty(TB_CertificateNumber.Text))
                    {
                        if (supplierInfomationList.Any(act => act.Number.Contains(TB_CertificateNumber.Text)))
                        {
                            tempList.Add(item);
                            continue;
                        }
                    }
                    if (!string.IsNullOrEmpty(DDL_Filiale.SelectedValue) &&
                        new Guid(DDL_Filiale.SelectedValue) != Guid.Empty)
                    {
                        if (supplierInfomationList.Any(act => act.FilialeID == new Guid(DDL_Filiale.SelectedValue)))
                            tempList.Add(item);
                    }
                }
                return tempList;
            }
            return supplierInfoList.ToList();
        }
        public bool HaveExpire(IList<SupplierInformationInfo> infoList)
        {
            return infoList.Any(item => item.OverdueDate < DateTime.Now.AddMonths(1));
        }

        public bool HaveExpired(IList<SupplierInformationInfo> infoList)
        {
            return infoList.Any(item => item.OverdueDate < DateTime.Now);
        }

        public bool HaveNumber(IList<SupplierInformationInfo> infoList, string certificateNumber)
        {
            return infoList.Any(item => item.Number.Contains(certificateNumber));
        }

        public bool HaveFiliale(IList<SupplierInformationInfo> infoList, Guid filialeId)
        {
            return infoList.Any(item => item.FilialeID == filialeId);
        }

        #region[Ajax页面返回]
        protected void RamGoodsAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(InformationsGrid, e);
        }
        #endregion

        protected void SearchTypeIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            //如果搜索类型又被重新选择了，要清空下拉选项
            RCB_SelectKey.Items.Clear();
            RCB_SelectKey.Text = string.Empty;
            CurrentSearchType = e.Value.ToInt();
            InformationsGrid.CurrentPageIndex = 0;
            SelectedIds = new List<Guid>();
            UnSelectedIds = new List<Guid>();
            IsAll = false;
            InformationsGrid.Rebind();
            divFiliale.Visible = CurrentSearchType == 1;
        }

        protected void DDLHaveInformation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DDL_HaveInformation.SelectedValue == "0")
            {
                DDL_Period.Enabled = false;
                DDL_Period.Items.Clear();
                DDL_Period.Items.Add(new RadComboBoxItem("全部", "0"));
            }
            else if (DDL_HaveInformation.SelectedValue == "1")
            {
                DDL_Period.Enabled = true;
                DDL_Period.Items.Clear();
                DDL_Period.Items.Add(new RadComboBoxItem("正常", "1"));
                DDL_Period.Items.Add(new RadComboBoxItem("快过期", "2"));
            }
            else if (DDL_HaveInformation.SelectedValue == "2")
            {
                DDL_Period.Enabled = true;
                DDL_Period.Items.Clear();
                DDL_Period.Items.Add(new RadComboBoxItem("全部", "0"));
                DDL_Period.Items.Add(new RadComboBoxItem("已过期", "3"));
            }
            
        }

        //搜索往来单位
        protected void SearchItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            //获取关键字
            var searchKey = e.Text.Trim();
            if (string.IsNullOrEmpty(searchKey))
            {
                return;
            }
            IDictionary<string, string> dataDict;

            //开始查询关键字
            if (CurrentSearchType == 1)
            {
                //搜索供应商
                dataDict = SupplierInfoList.Where(w => w.CompanyName.Contains(e.Text)).ToDictionary(ent => ent.CompanyId.ToString(), ent => ent.CompanyName);
            }
            else
            {
                //搜索商品名称/编号
                dataDict = _goodsCenterSao.GetGoodsSelectList(e.Text);
            }

            //绑定数据
            if (dataDict == null)
            {
                dataDict = new Dictionary<string, string>();
            }
            combo.DataSource = dataDict;
            combo.DataBind();
        }

        protected string GetInformationOperate(string extensionName, string path)
        {
            if(string.IsNullOrEmpty(extensionName) || string.IsNullOrEmpty(path))
            {
                return "";
            }
            var filePath = GlobalConfig.ResourceServerInformation + path.Replace("~/", "");
            var types = new[]
            {
                ".bmp",".gif",".jpg",".png",".jpeg","bmp","gif","jpg","png","jpeg"//更多文件类型自己添加到此数组即可
            };
            var ispic = types.Any(type => type == extensionName.ToLower());
            if (!ispic)
            {
                return "<a  href='" + filePath + "' target='_blank'>下载</a>";
                //if (File.Exists(filePath))
                //{
                //    return "<a href='" + GetDownPath(path) + "'>下载</a>";
                //}
                //return "<a href='javascript:void(0);' onclick='javascript:alert(\"文件不存在\");'>下载</a>";
            }
            return "<a href='javascript:void(0);' onclick='ShowPicWindow(\"" + filePath + "\")'>打开</a>";
        }

        #region[获取下载商品资料地址]
        /// <summary>
        /// 获取下载商品资料地址
        /// </summary>
        /// <param name="path">商品资料路径</param>
        /// <returns></returns>
        public string GetDownPath(string path)
        {
            string returnPath = "";
            if (!string.IsNullOrEmpty(path))
            {
                returnPath = "./Windows/DownloadPage.aspx?tag=goodsInformation&fullname=" + (GlobalConfig.ResourceServerInformation + path.Replace("~/", ""));
            }
            return returnPath;
        }
        #endregion

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            SelectedIds=new List<Guid>();
            UnSelectedIds=new List<Guid>();
            IsAll = false;
            InformationsGrid.Rebind();
        }

        /// <summary> Excel导出
        /// </summary>
        protected void Ib_ExportData_Click(object sender, EventArgs e)
        {
            if (!IsAll && SelectedIds.Count == 0)
            {
                RAM.Alert("请选择导出数据!");
                return;
            }
            var supplierGoodsInfoList=new List<SupplierGoodsInfo>();
            switch (RCB_SearchType.SelectedValue)
            {
                case "1":
                    supplierGoodsInfoList = GetSupplierGoodsInfos();
                    break;
                case "3":
                    string searchKey = RCB_SelectKey.Text;
                    var total = InformationsGrid.VirtualItemCount;
                    int pageindex = 1;
                    const int PAGE_SIZE = 30;
                    var count = Math.Ceiling((double)total/PAGE_SIZE);
                    int? state = null;
                    if (DDL_HaveInformation.SelectedValue!="0")
                    {
                        state = Convert.ToInt32(DDL_HaveInformation.SelectedValue);
                    }
                    int? dataState = null;
                    if (DDL_Period.SelectedValue != "0")
                    {
                        dataState = Convert.ToInt32(DDL_Period.SelectedValue);
                    }
                    if (count>0)
                    {
                        while (pageindex<=count)
                        {
                            long totalCount;
                            var goodsList = _goodsCenterSao.SelectGoodsInformationInfosByPage(searchKey, TB_CertificateNumber.Text,
                            state, dataState, pageindex, PAGE_SIZE, out totalCount);
                            supplierGoodsInfoList.AddRange(goodsList);
                            pageindex++;
                        }
                    }
                    break;
            }
            if (supplierGoodsInfoList.Count > 0)
            {
                var dataList =IsAll?supplierGoodsInfoList.Where(act=>!UnSelectedIds.Contains(act.ID)).ToList()
                    :supplierGoodsInfoList.Where(act => SelectedIds.Contains(act.ID)).ToList();
                OutPutExcel(dataList);
            }
            else
            {
                RAM.Alert("数据为空!");
            }
        }

        public void OutPutExcel(List<SupplierGoodsInfo> supplierList)
        {
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = new HSSFSheet[1];// 增加sheet。

            #region Excel样式

            //标题样式styletitle
            HSSFFont fonttitle = workbook.CreateFont();
            fonttitle.FontHeightInPoints = 12;
            fonttitle.Color = HSSFColor.BLACK.index;
            fonttitle.Boldweight = 3;
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

            #region 报备模板 以及sheet名字
            string name = RCB_SearchType.SelectedValue == "1" ? "供应商" : "商品";
            sheet[0] = workbook.CreateSheet(string.Format("{0}资质情况",name));//添加sheet名
            sheet[0].DefaultColumnWidth = 30;
            sheet[0].DefaultRowHeight = 20;

            HSSFRow rowtitle = sheet[0].CreateRow(0);
            HSSFCell celltitie = rowtitle.CreateCell(0);
            celltitie.SetCellValue(string.Format("{0}资质情况列表",name));
            HSSFCellStyle style = workbook.CreateCellStyle();
            style.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HSSFFont font = workbook.CreateFont();
            font.FontHeightInPoints = 20;
            font.Color = HSSFColor.BLACK.index;
            font.Boldweight = 2;
            style.SetFont(font);
            celltitie.CellStyle = style;
            sheet[0].AddMergedRegion(new Region(0, 0, 0, 2));
            #endregion

            #region //列名
            HSSFRow row2 = sheet[0].CreateRow(1);
            HSSFCell cell1 = row2.CreateCell(0);
            HSSFCell cell2 = row2.CreateCell(1);
            HSSFCell cell3 = row2.CreateCell(2);
            cell1.SetCellValue("资质类型");
            cell2.SetCellValue(string.Format("{0}名称",name));
            cell3.SetCellValue("资质情况");
            cell1.CellStyle = styletitle;
            cell2.CellStyle = styletitle;
            cell3.CellStyle = styletitle;

            #endregion

            #region 数据填充
            int row = 2;
            //int index = 0;
            foreach (var supplierInfo in supplierList)
            {
                HSSFRow rowt = sheet[0].CreateRow(row);
                HSSFCell c1 = rowt.CreateCell(0);
                HSSFCell c2 = rowt.CreateCell(1);
                HSSFCell c3 = rowt.CreateCell(2);

                c1.SetCellValue(name);//资质类型
                c2.SetCellValue(supplierInfo.Name);//供应商名称
                c3.SetCellValue(supplierInfo.Complete == (int) SupplierCompleteType.Complete ? "完整" : "不完整");
                c1.CellStyle = styleContent;
                c2.CellStyle = styleContent;
                c3.CellStyle = styleContent;
                row++;
            }
            sheet[0].DisplayGridlines = true;
            #endregion

            #region 输出
            workbook.Write(ms);
            Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(string.Format("{0}资质情况", name) + DateTime.Now.ToString("yyyyMMdd") + ".xls", Encoding.UTF8));
            Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();

            GC.Collect();
            #endregion
        }

        /// <summary>
        /// 是否全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CkAllOnCheckedChanged(object sender, EventArgs e)
        {
            IsAll = !IsAll;
            SelectedIds = new List<Guid>();
            UnSelectedIds = new List<Guid>();
            InformationsGrid.Rebind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CkSelectedChanged(object sender, EventArgs e)
        {
            var ck = sender as CheckBox;
            if (ck!=null)
            {
                var item = ck.Parent.Parent as GridDataItem;
                if (item!=null)
                {
                    var selectedIds = SelectedIds;
                    var unSelectedIds = UnSelectedIds;
                    var id=new Guid(item.GetDataKeyValue("ID").ToString());
                    if (ck.Checked)
                    {
                        if (unSelectedIds.Contains(id))
                            unSelectedIds.Remove(id);
                        if (!selectedIds.Contains(id))
                            selectedIds.Add(id);
                    }
                    else 
                    {
                        if (selectedIds.Contains(id))
                            selectedIds.Remove(id);
                        if (!unSelectedIds.Contains(id))
                            unSelectedIds.Add(id);
                    }
                    SelectedIds = selectedIds;
                    UnSelectedIds = unSelectedIds;
                }
            }
        }
    }
}

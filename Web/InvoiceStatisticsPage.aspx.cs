using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Environment;
using ERP.Model.Invoice;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using OperationLog.Core;
using Telerik.Web.UI;
using Region = NPOI.HSSF.Util.Region;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>发票汇总
    /// </summary>
    public partial class InvoiceStatisticsPage : BasePage
    {
        private readonly SynchronousData sync = SynchronousData.Instance;

        /// <summary>开始时间
        /// </summary>
        protected DateTime StartTime
        {
            get
            {
                var selectedStartTime = RDP_StartTime.SelectedDate;
                if (selectedStartTime != null)
                {
                    return Convert.ToDateTime(selectedStartTime.Value.ToString("yyyy-MM-dd") + " 00:00:00");
                }
                return DateTime.Now.AddMonths(-1);
            }
        }

        /// <summary>完成时间
        /// </summary>
        protected DateTime EndTime
        {
            get
            {
                var selectedEndTime = RDP_EndTime.SelectedDate;
                if (selectedEndTime != null)
                {
                    return Convert.ToDateTime(selectedEndTime.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                }
                return DateTime.Now;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RCB_FilialeList.DataSource = FilialeList.Where(f => f.Rank == (int)FilialeRank.Head && f.FilialeTypes.Contains((int)FilialeType.SaleCompany));
                RCB_FilialeList.DataTextField = "Name";
                RCB_FilialeList.DataValueField = "ID";
                RCB_FilialeList.DataBind();
                RCB_FilialeList.SelectedIndex = 1;
                RDP_StartTime.SelectedDate = StartTime;
                RDP_EndTime.SelectedDate = EndTime;

                var kindTypes = Enum.Attribute.EnumAttribute.GetDict<InvoiceKindType>();
                RcbKindType.DataSource = kindTypes;
                RcbKindType.DataBind();
                RcbKindType.SelectedValue = string.Format("{0}", (Byte)InvoiceKindType.Electron);

                BindNoteType((Byte)InvoiceKindType.Electron);
            }
        }

        private void BindNoteType(byte kindType)
        {
            var flag = kindType == (Byte)InvoiceKindType.Electron;
            var dataSource = flag ? new Dictionary<int, string> { { -1, "全部" }, { 0, "蓝票" }, { 1, "红票" } } :
                Enum.Attribute.EnumAttribute.GetDict<InvoiceNoteType>();
            RCB_NoteType.DataSource = dataSource;
            RCB_NoteType.DataBind();

            table_ele.Visible = flag;
            table_count.Visible = !flag;

            rad_invoce.MasterTableView.Columns.FindByUniqueName("IsCommit").Display = !flag;
            rad_invoce.MasterTableView.Columns.FindByUniqueName("RateMoney").Display = flag;
            rad_invoce.MasterTableView.Columns.FindByUniqueName("ActualMoney").Display = flag;
            rad_invoce.MasterTableView.Columns.FindByUniqueName("TotalMoney").HeaderText = flag ? "金额" : "价税金额";
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsElectron
        {
            get
            {
                return !string.IsNullOrEmpty(RcbKindType.SelectedValue) && Convert.ToInt32(RcbKindType.SelectedValue) == (int)InvoiceKindType.Electron;
            }
        }

        protected IList<FilialeInfo> FilialeList
        {
            get
            {
                if (ViewState["FilialeList"] == null)
                {
                    var list = CacheCollection.Filiale.GetList();
                    ViewState["FilialeList"] = list;
                }
                return (IList<FilialeInfo>)ViewState["FilialeList"];
            }
        }

        protected InvoiceStatisticsInfo InvoiceStatisticsInfo
        {
            get { return ViewState["InvoiceStatisticsInfo"] as InvoiceStatisticsInfo; }
            set { ViewState["InvoiceStatisticsInfo"] = value; }
        }

        protected IList<InvoiceBriefInfo> InvoiceList
        {
            get
            {
                if (ViewState["InvoiceList"] == null)
                {
                    ViewState["InvoiceList"] = new List<InvoiceBriefInfo>();
                }
                return ViewState["InvoiceList"] as IList<InvoiceBriefInfo>;
            }
            set { ViewState["InvoiceList"] = value; }
        }

        protected void Button_InvoiceCommit_Click(object sender, EventArgs e)
        {
            if (RDP_StartTime.SelectedDate == null)
            {
                RAM.Alert("请选择起止日期!");
                return;
            }

            if (RcbKindType.SelectedValue == string.Format("{0}", (Byte)InvoiceKindType.Electron))
            {
                RAM.Alert("此功能只适用于纸质发票!");
                return;
            }

            if (RDP_EndTime.SelectedDate != null)
            {
                if (RDP_EndTime.SelectedDate >= DateTime.Now)
                {
                    RAM.Alert("报送结束日期不能大于等于今天");
                    return;
                }
            }

            if (RDP_EndTime.SelectedDate != null)
            {
                var filialeId = new Guid(RCB_FilialeList.SelectedValue);
                Invoice.WriteInstance.InvoiceCommit((DateTime)RDP_StartTime.SelectedDate, (DateTime)RDP_EndTime.SelectedDate, filialeId);
            }
            RAM.Alert("报送完成！");
            rad_invoce.Rebind();
        }

        /// <summary>点击导出发票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button_InvoiceExport_Click(object sender, EventArgs e)
        {
            if (RDP_StartTime.SelectedDate == null)
            {
                RAM.Alert("请选择起止日期!");
                return;
            }

            if (RcbKindType.SelectedValue == string.Format("{0}", (Byte)InvoiceKindType.Electron))
            {
                RAM.Alert("此功能只适用于纸质发票!");
                return;
            }

            if (RDP_EndTime.SelectedDate != null)
            {
                if (RDP_EndTime.SelectedDate >= DateTime.Now)
                {
                    RAM.Alert("导出结束日期不能大于等于今天");
                    return;
                }
            }
            //获取授权仓库列表
            var warehouseList = CurrentSession.Personnel.WarehouseList;
            if (warehouseList.Count == 0)
            {
                RAM.Alert("没有授权的仓库，无法导出！");
                return;
            }
            //获取发票遗失
            var lostInvoiceRollList = Invoice.ReadInstance.GetRollDetailListByState(InvoiceRollState.Lost);

            string saveInvoiceExportDir = Request.PhysicalApplicationPath + @"\InvoiceExport\";
            const string FPMX_ROW = "<Row  XH=\"{XH}\" HPMC=\"{HPMC}\"  JLDW=\"{JLDW}\"  SL=\"{SL}\" DJ=\"{DJ}\"  JE=\"{JE}\"></Row>";
            var filialeId = new Guid(RCB_FilialeList.SelectedValue);
            string kpzlContent;
            string commonInvoiceModuleContent;
            string saveInvoicefilename;
            if (filialeId == new Guid("444E0C93-1146-4386-BAE2-CB352DA70499"))
            {
                //百秀
                kpzlContent = File.ReadAllText(Request.PhysicalApplicationPath + @"\App_Data\BaiShopKPJL.txt", Encoding.GetEncoding("GB2312"));
                commonInvoiceModuleContent = File.ReadAllText(Request.PhysicalApplicationPath + @"\App_Data\BaiShopCommonInvoiceModule.xml", Encoding.GetEncoding("GB2312"));
                saveInvoicefilename = "SB_310227596494506_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            }
            else if (filialeId == new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698"))
            {
                //可得
                kpzlContent = File.ReadAllText(Request.PhysicalApplicationPath + @"\App_Data\KPJL.txt", Encoding.GetEncoding("GB2312"));
                commonInvoiceModuleContent = File.ReadAllText(Request.PhysicalApplicationPath + @"\App_Data\CommonInvoiceModule.xml", Encoding.GetEncoding("GB2312"));
                saveInvoicefilename = "SB_310227695808144_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            }
            else if (filialeId == new Guid("43609645-97DD-4AE4-989D-F3C867969A99"))
            {
                //镜拓
                kpzlContent = File.ReadAllText(Request.PhysicalApplicationPath + @"\App_Data\JingtuoKPJL.txt", Encoding.GetEncoding("GB2312"));
                commonInvoiceModuleContent = File.ReadAllText(Request.PhysicalApplicationPath + @"\App_Data\JingtuoCommonInvoiceModule.xml", Encoding.GetEncoding("GB2312"));
                saveInvoicefilename = "SB_31022705129415X_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            }
            else
            {
                RAM.Alert("系统提示：没有所选公司导出模版！");
                return;
            }

            var kpzlBuilder = new StringBuilder();
            //开始生成发票

            string msg = "不连号发票有：" + "\n";  //不连号信息提示
            bool isExist = false;

            GetInvoiceList(false);
            var kpInvoiceList = InvoiceList.Where(ent => (int)ent.NoteType > -1).Where(ent => !ent.IsCommit).ToList();

            var invoiceNos = new long[kpInvoiceList.Count];
            for (var i = 0; i < kpInvoiceList.Count; i++)
            {
                invoiceNos[i] = kpInvoiceList[i].InvoiceNo;
            }

            foreach (var invoice in kpInvoiceList)
            {
                var kpzl = kpzlContent.Replace("{FPHM}", invoice.InvoiceNo.ToMendString(8)); //发票号码

                if (!invoiceNos.Contains(invoice.InvoiceNo - 1) && !invoiceNos.Contains(invoice.InvoiceNo + 1))
                {
                    isExist = true;
                    msg += invoice.InvoiceNo + "\n";
                }

                kpzl = kpzl.Replace("{FPDM}", invoice.InvoiceCode);//发票代码

                //(正票或废票)和退票
                if (invoice.NoteType == InvoiceNoteType.Effective)
                {
                    kpzl = kpzl.Replace("{KPLX}", "0"); //0是正票
                }
                if (invoice.NoteType == InvoiceNoteType.Abolish)
                {
                    kpzl = kpzl.Replace("{KPLX}", "1"); //1是废票
                }
                if (invoice.NoteType == InvoiceNoteType.Retreat)
                {
                    kpzl = kpzl.Replace("{KPLX}", ((int)invoice.NoteType).ToString()); //开票类型
                }
                kpzl = kpzl.Replace("{KPRQ}", invoice.PrintDate.ToString("yyyyMMdd HH:mm:ss")); //开票日期
                kpzl = kpzl.Replace("{HYFLDM}", "02"); //行业分类代码
                kpzl = kpzl.Replace("{JE}", invoice.TotalMoney.ToString(CultureInfo.InvariantCulture)); //发票金额
                kpzl = kpzl.Replace("{FKRMC}", invoice.InvoicePayerName); //付款人名称
                kpzl = kpzl.Replace("{FKRSH}", invoice.TaxpayerID); //付款人税号
                kpzl = kpzl.Replace("{GHFQYLX}", ((int)invoice.PurchaserType).ToString()); //购货方企业类型
                kpzl = kpzl.Replace("{SPFDZJDH}", invoice.Address); //受票方地址及电话
                kpzl = kpzl.Replace("{SPFYHJZH}", string.Empty); //受票方银行及帐号

                //如果是退票
                const string YDPDM_CONTENT = @"<YFPDM>{YFPDM}</YFPDM><YFPHM>{YFPHM}</YFPHM><TPLX></TPLX><ZFRQ>{ZFRQ}</ZFRQ><ZFRXM>{ZFRXM}</ZFRXM>";
                if (invoice.NoteType == InvoiceNoteType.Retreat)
                {
                    var ydpdmContent2 = YDPDM_CONTENT.Replace("{YFPDM}", invoice.RetreatInvoiceCode); //原发票代码
                    ydpdmContent2 = ydpdmContent2.Replace("{YFPHM}", invoice.RetreatInvoiceNo.ToMendString(8)); //原发票号码
                    ydpdmContent2 = ydpdmContent2.Replace("{TPLX}", "1"); //退票类型(1-有原票数据  2-无原票数据)
                    ydpdmContent2 = ydpdmContent2.Replace("{ZFRQ}", invoice.PrintDate.ToString("yyyyMMdd HH:mm:ss")); //作废日期
                    ydpdmContent2 = ydpdmContent2.Replace("{ZFRXM}", "朱美萍"); //作废人姓名
                    kpzl = kpzl.Replace("{YFPDM_TPLX_Content}", ydpdmContent2);
                }
                else if (invoice.NoteType == InvoiceNoteType.Abolish)
                {
                    var ydpdmContent2 = YDPDM_CONTENT.Replace("{YFPDM}", string.Empty); //原发票代码
                    ydpdmContent2 = ydpdmContent2.Replace("{YFPHM}", string.Empty); //原发票号码
                    ydpdmContent2 = ydpdmContent2.Replace("{TPLX}", string.Empty); //退票类型(1-有原票数据  2-无原票数据)
                    ydpdmContent2 = ydpdmContent2.Replace("{ZFRQ}", invoice.PrintDate.ToString("yyyyMMdd HH:mm:ss")); //作废日期
                    ydpdmContent2 = ydpdmContent2.Replace("{ZFRXM}", "朱美萍"); //作废人姓名
                    kpzl = kpzl.Replace("{YFPDM_TPLX_Content}", ydpdmContent2);
                }
                else
                {
                    kpzl = kpzl.Replace("{YFPDM_TPLX_Content}", string.Empty);
                }

                //记录发票明细
                kpzl = kpzl.Replace("{FPMX_RowNum}", "1"); //发票明细记录数
                kpzl = kpzl.Replace("{HJJE}", invoice.TotalMoney.ToString(CultureInfo.InvariantCulture)); //合计金额

                //记录行信息
                var fpmxRow = FPMX_ROW.Replace("{XH}", "1"); //发票明细记录数
                fpmxRow = fpmxRow.Replace("{HPMC}", invoice.InvoiceContent); //货品或项目名称
                fpmxRow = fpmxRow.Replace("{JLDW}", "1"); //计量单位
                fpmxRow = fpmxRow.Replace("{SL}", "1"); //数量
                fpmxRow = fpmxRow.Replace("{DJ}", invoice.TotalMoney.ToString(CultureInfo.InvariantCulture)); //单价
                fpmxRow = fpmxRow.Replace("{JE}", invoice.TotalMoney.ToString(CultureInfo.InvariantCulture)); //金额
                fpmxRow = fpmxRow.Replace("{BZ}", string.Empty); //备注

                //把记录行并入开票记录内容里
                kpzl = kpzl.Replace("{FPMX_Row}", fpmxRow); //合计金额

                //合并开票记录
                kpzlBuilder.AppendLine(kpzl);
            }

            //发票基本信息设置
            var invoiceContent = commonInvoiceModuleContent.Replace("{SCSJ}", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));//生成时间
            invoiceContent = invoiceContent.Replace("{JLS}", kpInvoiceList.Count.ToString());//记录数
            invoiceContent = invoiceContent.Replace("{KSSJ}", StartTime.ToString("yyyyMMdd HH:mm:ss"));//开始时间(yyyyMMdd HH:mm:ss)
            invoiceContent = invoiceContent.Replace("{JSSJ}", EndTime.ToString("yyyyMMdd HH:mm:ss"));//结束时间(yyyyMMdd HH:mm:ss)

            string zpsl = kpInvoiceList.Count(ent => ent.NoteType == InvoiceNoteType.Effective).ToString();
            invoiceContent = invoiceContent.Replace("{ZPFS}", zpsl);//正票份数

            string zpje = kpInvoiceList.Where(ent => ent.NoteType == InvoiceNoteType.Effective).Sum(p => p.TotalMoney).ToString(CultureInfo.InvariantCulture);
            invoiceContent = invoiceContent.Replace("{ZPJE}", zpje);//正票金额

            int fpsl = kpInvoiceList.Count(ent => ent.NoteType == InvoiceNoteType.Abolish);
            invoiceContent = invoiceContent.Replace("{FPFS}", fpsl.ToString());//废票份数

            var tpInfos = kpInvoiceList.Where(ent => ent.NoteType == InvoiceNoteType.Retreat);
            var tpfs = tpInfos.Count().ToString();
            var tpje = tpInfos.Sum(ent => ent.TotalMoney).ToString(CultureInfo.InvariantCulture);
            invoiceContent = invoiceContent.Replace("{TPFS}", tpfs);//退票份数
            invoiceContent = invoiceContent.Replace("{TPJE}", tpje.Replace("-", tpje));//退票金额
            invoiceContent = invoiceContent.Replace("{YSFS}", lostInvoiceRollList.Count.ToString());//遗失份数
            invoiceContent = invoiceContent.Replace("{JXFS}", "0");//缴销份数
            invoiceContent = invoiceContent.Replace("{KP_RecNum}", kpInvoiceList.Count.ToString());//开票记录数
            invoiceContent = invoiceContent.Replace("{KPZL_RecNum}", kpInvoiceList.Count.ToString());//开票记录数

            //加入发票遗失
            if (lostInvoiceRollList.Count > 0)
            {
                var lostInvoice = new StringBuilder();
                //lostInvoice.Append();
                foreach (var inv in lostInvoiceRollList)
                {
                    lostInvoice.Append(@"<Row><FPDM>" + inv.InvoiceCode + "</FPDM><FPHS>" + inv.StartNo +
                                       "</FPHS><FPHZ>" + inv.EndNo + "</FPHZ><YSRQ>" +
                                       DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "</YSRQ><DJRQ>" +
                                       DateTime.Now.ToString("yyyyMMdd") + "</DJRQ></Row>");
                }
                invoiceContent = invoiceContent.Replace("{YSFP_Content}", "<YSFP RecNum=\"" + lostInvoiceRollList.Count + "\">{YSFP_Row_Content}</YSFP>".Replace("{YSFP_Row_Content}", lostInvoice.ToString()));
            }
            else
            {
                invoiceContent = invoiceContent.Replace("{YSFP_Content}", string.Empty);
            }

            //把开票记录内入发票内容里
            invoiceContent = invoiceContent.Replace("{KPZL_Content}", kpzlBuilder.ToString());

            //生成XML文档
            if (!Directory.Exists(saveInvoiceExportDir))
            {
                Directory.CreateDirectory(saveInvoiceExportDir);
            }
            // string saveInvoicefilename = "SB_310227695808144_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            File.WriteAllText(saveInvoiceExportDir + saveInvoicefilename, invoiceContent, Encoding.Default);
            if (isExist)
            {
                RAM.Alert("生成发票XML文档成功！" + "\n" + msg);
            }
            else
            {
                RAM.Alert("生成发票XML文档成功！");
            }
            RAM.ResponseScripts.Add("window.open('/WebService/Downfile.ashx?filename=" + saveInvoicefilename + "')");
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            if (RDP_StartTime.SelectedDate == null)
            {
                RAM.Alert("请选择起止日期!");
                return;
            }
            rad_invoce.CurrentPageIndex = 0;
            rad_invoce.Rebind();
        }

        protected void Rad_invoce_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (!IsPostBack)
            {
                rad_invoce.DataSource = new List<InvoiceBriefInfo>();
                return;
            }
            if (RDP_StartTime.SelectedDate != null && RDP_EndTime.SelectedDate != null)
            {
                var filialeId = new Guid(RCB_FilialeList.SelectedValue);
                string invoiceNo = string.Empty;
                if (!string.IsNullOrEmpty(tbx_keyword.Text.Trim()))
                {
                    invoiceNo = tbx_keyword.Text.Trim();
                }

                var pageSize = rad_invoce.PageSize;
                var pageIndex = rad_invoce.CurrentPageIndex + 1;
                int recordCount;

                var kindType = Convert.ToByte(RcbKindType.SelectedValue);
                var flag = kindType == (Byte)InvoiceKindType.Electron;
                string msg;
                InvoiceStatisticsInfo = flag ? GetStatisticsInfo(EndTime, Convert.ToInt32(RCB_NoteType.SelectedValue), filialeId, invoiceNo.ToString(), StartTime, pageIndex, pageSize,out msg, out recordCount)
                    : Invoice.ReadInstance.Search(StartTime, EndTime, filialeId, Convert.ToInt32(RCB_NoteType.SelectedValue), invoiceNo, pageSize, pageIndex, out recordCount);
                rad_invoce.DataSource = InvoiceStatisticsInfo.InvoiceList;
                rad_invoce.VirtualItemCount = recordCount;

                //正票
                var zpInfo = InvoiceStatisticsInfo.NoteStatistics.FirstOrDefault(ent => ent.NoteType == InvoiceNoteType.Effective);
                //废票
                var fpInfo = InvoiceStatisticsInfo.NoteStatistics.FirstOrDefault(ent => ent.NoteType == InvoiceNoteType.Abolish);

                if (flag)
                {
                    lb_lp.Text = zpInfo != null ? string.Format("{0}", zpInfo.TotalQuantity) : "0";
                    lb_lpsum.Text = zpInfo != null ? WebControl.NumberSeparator(zpInfo.TotalMoney.ToString("#.##")) : "0.00";


                    lb_hp.Text = fpInfo != null ? string.Format("{0}", fpInfo.TotalQuantity) : "0";
                    lb_hpsum.Text = fpInfo != null ? WebControl.NumberSeparator(fpInfo.TotalMoney.ToString("#.##")) : "0.00";

                    var sum = zpInfo?.TotalMoney - (fpInfo?.TotalMoney ?? 0);
                    //实际金额
                    lb_tolsum.Text = WebControl.NumberSeparator(sum ?? 0);
                }
                else
                {
                    lab_zp.Text = zpInfo != null ? string.Format("{0}", zpInfo.TotalQuantity) : "0";
                    lab_zpsum.Text = zpInfo != null ? WebControl.NumberSeparator(zpInfo.TotalMoney.ToString("#.##")) : "0.00";


                    lab_fp.Text = fpInfo != null ? string.Format("{0}", fpInfo.TotalQuantity) : "0";
                    Label_fpje.Text = fpInfo != null ? WebControl.NumberSeparator(fpInfo.TotalMoney.ToString("#.##")) : "0.00";

                    //退票
                    var tpInfo = InvoiceStatisticsInfo.NoteStatistics.FirstOrDefault(ent => ent.NoteType == InvoiceNoteType.Retreat);
                    Label_tpzs.Text = tpInfo != null ? string.Format("{0}", tpInfo.TotalQuantity) : "0";
                    Label_tpje.Text = tpInfo != null ? WebControl.NumberSeparator(tpInfo.TotalMoney.ToString("#.##")) : "0.00";

                    //实际金额
                    lab_tol.Text = WebControl.NumberSeparator((zpInfo != null ? zpInfo.TotalMoney : 0) + (tpInfo != null ? tpInfo.TotalMoney : 0));
                }
            }
            else
            {
                rad_invoce.DataSource = new List<InvoiceBriefInfo>();
            }
        }

        private List<InvoiceBriefInfo> GetList(DateTime endDateTime, int invoiceType, Guid saleFilialeId, string invoiceNo, DateTime startDateTime, int pageIndex, int pageSize)
        {
            var result = Common.InvoiceApi.GetInvoiceList(endDateTime, invoiceType, saleFilialeId, invoiceNo, startDateTime, pageIndex, pageSize);
            if (result != null && result.State)
            {
                return GetList(result.Data.DataList);
            }
            return new List<InvoiceBriefInfo>();
        }

        private List<InvoiceBriefInfo> GetList(IEnumerable<B2CInvoiceDTO> invoiceDto)
        {
            var data = new List<InvoiceBriefInfo>();
            if (invoiceDto != null && invoiceDto.Any())
            {
                return invoiceDto.Select(ent => new InvoiceBriefInfo
                {
                    PrintDate = ent.BillingDate,
                    CreateDate = ent.BillingDate,
                    InvoicePayerName = ent.InvoiceTitle,
                    InvoiceCode = ent.InvoiceCode,
                    InvoiceNo =string.IsNullOrEmpty(ent.InvoiceNumber)?0:Convert.ToInt64(ent.InvoiceNumber),
                    RetreatInvoiceCode = ent.OriginalInvoiceCode,
                    RetreatInvoiceNo = string.IsNullOrEmpty(ent.OriginalInvoiceNumber) ?0: Convert.ToInt64(ent.OriginalInvoiceNumber),
                    TotalMoney = (float)ent.NoTaxAmount,
                    NoteType = ent.BillingType == 1? InvoiceNoteType.Effective : InvoiceNoteType.Abolish,
                    OrderNo = ent.OrderNo,
                    State = ent.BillingType != 1? InvoiceState.All: InvoiceState.Success,
                    InvoiceKindType = (int)InvoiceKindType.Electron,
                    RateMoney = ent.Taxes,
                    ActualMoney = ent.Amount,
                    OrderId = ent.OrderId,
                    InvoiceContent = ent.Remark
                }).ToList();
            }
            return data;
        }

        private InvoiceStatisticsInfo GetStatisticsInfo(DateTime endDateTime, int invoiceType, Guid saleFilialeId, string invoiceNo, DateTime startDateTime, int page, int size, out string message, out int recordCount)
        {
            var result = Common.InvoiceApi.GetInvoiceList(endDateTime, invoiceType, saleFilialeId, string.IsNullOrEmpty(invoiceNo)?string.Empty:invoiceNo, startDateTime, page, size);
            IList<InvoiceBriefInfo> items = new List<InvoiceBriefInfo>();
            IList<InvoiceNoteStatisticsInfo> invoiceNoteStatisticsInfoList = new List<InvoiceNoteStatisticsInfo>();
            recordCount = 0;
            message = "";
            if (result != null)
            {
                if (!result.State)
                {
                    message = result.Msg;
                }
                else
                {
                    recordCount = result.Data.TotalCount;
                    items = result.Data!=null && result.Data.DataList!=null? GetList(result.Data.DataList) :new List<InvoiceBriefInfo>();
                    invoiceNoteStatisticsInfoList = new List<InvoiceNoteStatisticsInfo>
                    {
                        new InvoiceNoteStatisticsInfo
                        {
                            NoteType = InvoiceNoteType.Effective,
                            TotalMoney = (float)result.Data.BlueSumAmount,
                            TotalQuantity = result.Data.BlueCount
                        },
                        new InvoiceNoteStatisticsInfo
                        {
                            NoteType = InvoiceNoteType.Abolish,
                            TotalMoney = (float)result.Data.RedSumAmount,
                            TotalQuantity = result.Data.RedCount
                        }
                    };
                }
            }
            return new InvoiceStatisticsInfo(invoiceNoteStatisticsInfoList, items);
        }

        protected void Rad_invoce_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var dataItem = e.Item as GridDataItem;
                var invoiceid = new Guid(dataItem.GetDataKeyValue("InvoiceId").ToString());
                IGoodsOrder order = new GoodsOrder(GlobalConfig.DB.FromType.Read);
                IList<GoodsOrderInfo> infos = order.GetInvoiceGoodsOrderList(invoiceid);
                if (infos == null || infos.Count == 0) throw new NotImplementedException("invoiceid" + invoiceid + "--方法GetInvoiceGoodsOrderList(invoiceid)为空");
                if ("delete" == e.CommandName)
                {
                    //ibll.SetInvoiceState(invoiceid, InvoiceState.Cancel, "该发票存在重复,因此取消该发票![" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]", CurrentSession.Personnel.Get().RealName);
                    Invoice.WriteInstance.SetInvoiceState(invoiceid, InvoiceState.Cancel, CurrentSession.Personnel.Get().RealName);
                    sync.SyncSetInvoiceState(infos[0].SaleFilialeId, invoiceid, InvoiceState.Cancel, true, "该发票存在重复,因此取消该发票![" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]");

                }
                else if ("waste" == e.CommandName)
                {
                    //var success = ibll.SetInvoiceState(invoiceid, InvoiceState.Waste, "该发票财务确认作废.[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]", CurrentSession.Personnel.Get().RealName);
                    var success = Invoice.WriteInstance.SetInvoiceState(invoiceid, InvoiceState.Waste, CurrentSession.Personnel.Get().RealName);
                    if (success)
                    {
                        var personnelInfo = CurrentSession.Personnel.Get();
                        //发票汇总允许作废操作记录添加
                        var invoiceInfo = Invoice.WriteInstance.GetInvoice(invoiceid);
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, invoiceid, invoiceInfo == null ? "" : invoiceInfo.InvoiceNo.ToMendString(8),
                                                   OperationPoint.InvoiceStatistics.AllowScrap.GetBusinessInfo(), string.Empty);

                        sync.SyncSetInvoiceState(infos[0].SaleFilialeId, invoiceid, InvoiceState.Waste, true,
                                                 "该发票财务确认作废.[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]");
                    }
                    else
                    {
                        RAM.Alert("发票拒绝作废，有可能发票已经报送税务！");
                    }
                }
                else if ("return" == e.CommandName)
                {
                    //ibll.SetInvoiceState(invoiceid, InvoiceState.Success, "该发票财务拒绝作废.状态返回已开[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "]", CurrentSession.Personnel.Get().RealName);
                    Invoice.WriteInstance.UpdateSetInvoiceState(invoiceid, InvoiceState.Success, CurrentSession.Personnel.Get().RealName);

                    var invoiceInfo = Invoice.WriteInstance.GetInvoice(invoiceid);
                    var personnelInfo = CurrentSession.Personnel.Get();
                    //发票汇总财务拒绝作废操作记录添加
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, invoiceid, invoiceInfo == null ? "" : invoiceInfo.InvoiceNo.ToMendString(8),
                        OperationPoint.InvoiceStatistics.RefuseScrap.GetBusinessInfo(), string.Empty);

                }
                rad_invoce.Rebind();
            }
            if (e.CommandName == "Search")
            {
                if (RcbKindType.SelectedValue == string.Format("{0}", (Byte)InvoiceKindType.Electron))
                {
                    RAM.Alert("此功能只适用于纸质发票!");
                }
                else
                {
                    AnomalyInvoce();
                    rad_invoce.Rebind();
                }
            }

        }

        #region 导出发票清单
        /// <summary>
        /// 导出发票至XLS文档
        /// </summary>
        public void OutPutExcel()
        {
            var flag = Convert.ToByte(RcbKindType.SelectedValue) == (Byte)InvoiceKindType.Electron;
            GetInvoiceList(flag);
            IList<InvoiceBriefInfo> list = flag ? InvoiceList : InvoiceList.Where(ent => ent.InvoiceNo != 0).ToList();
            if (list.Count == 0)
            {
                RAM.Alert("没有可导出的数据!");
                return;
            }
            //var filialeList = CacheCollection.Filiale.GetList();
            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = new HSSFSheet[list.Count];// 增加sheet。

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
            #endregion

            #region 将值插入sheet

            //是否为电子发票
            
            int t = -1;
            var zp = InvoiceStatisticsInfo.NoteStatistics.FirstOrDefault(ent => ent.NoteType == InvoiceNoteType.Effective);
            int zpcount = zp != null ? zp.TotalQuantity : 0;
            var fpInfo = InvoiceStatisticsInfo.NoteStatistics.FirstOrDefault(ent => ent.NoteType == InvoiceNoteType.Abolish);
            int fpcount = 0;
            if (fpInfo != null)
            {
                fpcount = fpInfo.TotalQuantity;
            }
            //var fp =InvoiceStatisticsInfo.NoteStatistics.FirstOrDefault(ent => ent.NoteType == InvoiceNoteType.Effective);
            float sum = (zp?.TotalMoney ?? 0) - (flag && fpInfo != null ? fpInfo.TotalMoney : 0);

            t++;
            sheet[t] = workbook.CreateSheet(flag?"电子发票清单":"发票清单");//添加sheet名
            sheet[t].DefaultColumnWidth = 35;
            sheet[t].DefaultRowHeight = 20;
            sheet[t].DisplayGridlines = false;
            HSSFRow rowtitle = sheet[t].CreateRow(0);
            HSSFCell celltitie = rowtitle.CreateCell(0);

            celltitie.SetCellValue("发票清单");
            HSSFCellStyle style = workbook.CreateCellStyle();
            style.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HSSFFont font = workbook.CreateFont();
            font.FontHeightInPoints = 20;
            font.Color = HSSFColor.BLACK.index;
            font.Boldweight = 2;
            style.SetFont(font);
            celltitie.CellStyle = style;
            sheet[t].AddMergedRegion(new Region(0, 0, 0, 4));
            //采购联系消息
            HSSFRow row1 = sheet[t].CreateRow(4);
            row1.CreateCell(0).SetCellValue(string.Format("{0}总数", !flag ? "正票" : "蓝票"));
            row1.CreateCell(1).SetCellValue(zpcount);


            HSSFRow row2 = sheet[t].CreateRow(5);
            row2.CreateCell(0).SetCellValue(string.Format("{0}总数", !flag ? "废票" : "红票"));
            row2.CreateCell(1).SetCellValue(fpcount);



            HSSFRow row3 = sheet[t].CreateRow(6);
            row3.CreateCell(0).SetCellValue("实际总金额 ");
            row3.CreateCell(1).SetCellValue(sum.ToString());


            HSSFRow row8 = sheet[t].CreateRow(9);
            HSSFCell cell1 = row8.CreateCell(0);
            HSSFCell cell2 = row8.CreateCell(1);
            HSSFCell cell3 = row8.CreateCell(2);
            HSSFCell cell4 = row8.CreateCell(3);
            HSSFCell cell5 = row8.CreateCell(4);
            HSSFCell cell6 = row8.CreateCell(5);
            HSSFCell cell7 = row8.CreateCell(6);

            cell1.SetCellValue("开票时间");
            cell2.SetCellValue("抬头");
            cell3.SetCellValue("发票代码");
            cell4.SetCellValue("发票号码");
            cell5.SetCellValue("退票原代码");
            cell6.SetCellValue("退票原号码");
            cell7.SetCellValue(flag ? "金额" : "价税金额");

            cell1.CellStyle = styletitle;
            cell2.CellStyle = styletitle;
            cell3.CellStyle = styletitle;
            cell4.CellStyle = styletitle;
            cell5.CellStyle = styletitle;
            cell6.CellStyle = styletitle;
            cell7.CellStyle = styletitle;
            
            if (flag)
            {
                HSSFCell cell8 = row8.CreateCell(7);
                HSSFCell cell9 = row8.CreateCell(8);
                HSSFCell cell10 = row8.CreateCell(9);
                HSSFCell cell11 = row8.CreateCell(10);
                HSSFCell cell12 = row8.CreateCell(11);
                cell8.SetCellValue("税额");
                cell9.SetCellValue("价税合计");
                cell10.SetCellValue("票据");
                cell11.SetCellValue("订单号");
                cell12.SetCellValue("状态");
                cell8.CellStyle = styletitle;
                cell9.CellStyle = styletitle;
                cell10.CellStyle = styletitle;
                cell11.CellStyle = styletitle;
                cell12.CellStyle = styletitle;
            }
            else
            {
                HSSFCell cell8 = row8.CreateCell(7);
                HSSFCell cell9 = row8.CreateCell(8);
                HSSFCell cell10 = row8.CreateCell(9);
                HSSFCell cell11 = row8.CreateCell(10);

                cell8.SetCellValue("票据");
                cell9.SetCellValue("订单号");
                cell10.SetCellValue("状态");
                cell11.SetCellValue("报送");

                cell8.CellStyle = styletitle;
                cell9.CellStyle = styletitle;
                cell10.CellStyle = styletitle;
                cell11.CellStyle = styletitle;
            }
            cell4.CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;

            int row = 10;
            foreach (InvoiceBriefInfo info in list.OrderBy(p => p.State))
            {

                HSSFRow rowt = sheet[t].CreateRow(row);
                HSSFCell c1 = rowt.CreateCell(0);
                HSSFCell c2 = rowt.CreateCell(1);
                HSSFCell c3 = rowt.CreateCell(2);
                HSSFCell c4 = rowt.CreateCell(3);
                HSSFCell c5 = rowt.CreateCell(4);
                HSSFCell c6 = rowt.CreateCell(5);
                HSSFCell c7 = rowt.CreateCell(6);
                c1.SetCellValue(info.PrintDate.ToString("yyyy-MM-dd HH:mm:ss"));
                c2.SetCellValue(info.InvoicePayerName);
                c3.SetCellValue(info.InvoiceCode);
                c4.SetCellValue(info.InvoiceNo);
                c5.SetCellValue(info.RetreatInvoiceCode);
                c6.SetCellValue(info.RetreatInvoiceNo);
                c7.SetCellValue(info.TotalMoney.ToString());

                //var filialeInfo = filialeList.FirstOrDefault(w => w.ID == info.SaleFilialeId);
                //if (filialeInfo != null)
                //{
                //    c8.SetCellValue(filialeInfo.TaxNumber);
                //    c9.SetCellValue(filialeInfo.RegisterAddress);
                //}
                //else
                //{
                //    c8.SetCellValue("");
                //    c9.SetCellValue("");
                //}

                c1.CellStyle = styleContent;
                c2.CellStyle = styleContent;
                c3.CellStyle = styleContent;
                c4.CellStyle = styleContent;
                c5.CellStyle = styleContent;
                c6.CellStyle = styleContent;
                c7.CellStyle = styleContent;

                if (flag)
                {
                    HSSFCell cell8 = rowt.CreateCell(7);
                    HSSFCell cell9 = rowt.CreateCell(8);
                    HSSFCell cell10 = rowt.CreateCell(9);
                    HSSFCell cell11 = rowt.CreateCell(10);
                    HSSFCell cell12 = rowt.CreateCell(11);
                    cell8.SetCellValue(info.RateMoney.ToString(CultureInfo.InvariantCulture));
                    cell9.SetCellValue(info.ActualMoney.ToString(CultureInfo.InvariantCulture));
                    cell10.SetCellValue(info.NoteType==(int)InvoiceNoteType.Effective?"蓝票":"红票");
                    cell11.SetCellValue(info.OrderNo);
                    cell12.SetCellValue("已开发票");
                    cell8.CellStyle = styleContent;
                    cell9.CellStyle = styleContent;
                    cell10.CellStyle = styleContent;
                    cell11.CellStyle = styleContent;
                    cell12.CellStyle = styleContent;
                }
                else
                {
                    HSSFCell cell8 = rowt.CreateCell(7);
                    HSSFCell cell9 = rowt.CreateCell(8);
                    HSSFCell cell10 = rowt.CreateCell(9);
                    HSSFCell cell11 = rowt.CreateCell(10);

                    cell8.SetCellValue(info.NoteTypeName);
                    cell9.SetCellValue(info.OrderNo);
                    cell10.SetCellValue(info.State == InvoiceState.Success ? "已开发票" : info.State == InvoiceState.Waste ? "作废" : "作废申请");
                    cell11.SetCellValue(info.IsCommit?"已报":"未报");

                    cell8.CellStyle = styleContent;
                    cell9.CellStyle = styleContent;
                    cell10.CellStyle = styleContent;
                    cell11.CellStyle = styleContent;
                }
                c4.CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                row++;
            }


            #endregion
            workbook.Write(ms);
            Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("可得网发票清单" + DateTime.Now.ToString("yyyyMMdd") + ".xls", Encoding.UTF8));
            Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();
            GC.Collect();
        }
        #endregion

        protected void img_Export_Click(object sender, ImageClickEventArgs e)
        {
            if (RDP_StartTime.SelectedDate == null && RDP_EndTime.SelectedDate == null)
            {
                RAM.Alert("请选择起止日期!");
                return;
            }
            //获取授权仓库列表
            var warehouseList = CurrentSession.Personnel.WarehouseList;
            if (warehouseList.Count == 0)
            {
                RAM.Alert("没有授权的仓库，无法导出数据！");
                return;
            }
            OutPutExcel();
        }

        protected string GetNoteType(object state, object noteType)
        {
            var flag = Convert.ToByte(RcbKindType.SelectedValue) == (Byte)InvoiceKindType.Electron;
            if ((InvoiceNoteType)noteType == InvoiceNoteType.Retreat)
            {
                return !flag ? "退票" : "未定义";
            }
            if ((InvoiceNoteType)noteType == InvoiceNoteType.Abolish)
            {
                return !flag ? "废票" : "红票";
            }
            if ((InvoiceNoteType)noteType == InvoiceNoteType.Effective)
            {
                return !flag ? "正票" : "蓝票";
            }
            return "-";
        }

        protected Color GetInvoiceMoneyForeColor(object state, object noteType)
        {
            if ((InvoiceNoteType)noteType == InvoiceNoteType.Retreat)
            {
                return Color.Red;
            }
            if ((InvoiceState)state == InvoiceState.Waste)
            {
                return Color.LightGray;
            }
            return Color.Black;
        }

        protected void RCBWarehouse_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var rcb = (RadComboBox)sender;
                //获取授权仓库列表
                var warehouseList = CurrentSession.Personnel.WarehouseList;
                foreach (var info in warehouseList)
                {
                    rcb.Items.Add(new RadComboBoxItem { Text = info.WarehouseName, Value = info.WarehouseId.ToString() });
                }
            }
        }

        /// <summary>查看不连号发票
        /// </summary>
        private void AnomalyInvoce()
        {
            var filialeId = new Guid(RCB_FilialeList.SelectedValue);
            string invoiceNo = string.Empty;
            if (!string.IsNullOrEmpty(tbx_keyword.Text.Trim()))
            {
                invoiceNo = tbx_keyword.Text.Trim();
            }
            var noteType = Convert.ToInt32(RCB_NoteType.SelectedValue);
            IList<InvoiceBriefInfo> list = Invoice.ReadInstance.OutPutExcelInvoice(StartTime, EndTime, filialeId, noteType, invoiceNo, (Byte)InvoiceKindType.Paper);
            var newList = list.OrderBy(w => w.InvoiceNo).ToList();
            if (newList.Count > 1)
            {
                var message = string.Empty;
                var invoiceBriefInfo = new InvoiceBriefInfo();
                for (int i = 0; i < newList.Count; i++)
                {
                    InvoiceBriefInfo info = newList[i];
                    if (i == 0)
                    {
                        invoiceBriefInfo = info;
                    }
                    else
                    {
                        Int64 a = Convert.ToInt64(invoiceBriefInfo.InvoiceNo);
                        Int64 b = Convert.ToInt64(info.InvoiceNo);
                        if ((a + 1) != b)
                        {
                            message += invoiceBriefInfo.InvoiceNo.ToMendString(8) + " - " + info.InvoiceNo.ToMendString(8) + "\n";
                        }
                        invoiceBriefInfo = info;
                    }
                }
                if (!string.IsNullOrEmpty(message))
                {
                    RAM.Alert("发票号码有不连号的信息如下：\n" + message);
                }
            }
        }

        private void GetInvoiceList(bool flag)
        {
            var filialeId = new Guid(RCB_FilialeList.SelectedValue);
            string invoiceNo = string.Empty;
            if (!string.IsNullOrEmpty(tbx_keyword.Text.Trim()))
            {
                invoiceNo = tbx_keyword.Text.Trim();
            }
            var noteType = Convert.ToInt32(RCB_NoteType.SelectedValue);
            InvoiceList = flag ? GetList(EndTime, noteType, filialeId, invoiceNo, StartTime, 1, int.MaxValue)
                : Invoice.ReadInstance.OutPutExcelInvoice(StartTime, EndTime, filialeId, noteType, invoiceNo, Convert.ToByte(RcbKindType.SelectedValue)).ToList();
        }

        protected void RcbKindTypeSelectedChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var obj = (RadComboBox)o;
            var kindType = Convert.ToByte(obj.SelectedValue);
            BindNoteType(kindType);
            rad_invoce.Rebind();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using OperationLog.Core;
using Telerik.Web.UI;

/*新建发票功能
 * 作者：刘彩军
 * 时间:2012-10-10
 */
namespace ERP.UI.Web.Windows
{
    public partial class AddInvoice : WindowsPage
    {
        private readonly IInvoice _invoice = new Invoice(GlobalConfig.DB.FromType.Write);

        #region
        /// <summary>
        /// 订单来源
        /// </summary>
        public IList<FilialeInfo> SaleFilialeList
        {
            get
            {
                if (ViewState["SaleFilialeList"] == null)
                {
                    ViewState["SaleFilialeList"] =
                        CacheCollection.Filiale.GetList()
                                       .Where(act => act.Rank == (int)FilialeRank.Head).ToList();
                }
                return (IList<FilialeInfo>)ViewState["SaleFilialeList"];
            }
            set
            {
                ViewState["SaleFilialeList"] = value;
            }
        }

        protected string SelectSaleFilialeId
        {
            get
            {
                return ViewState["FromSource"] == null ? string.Empty : ViewState["FromSource"].ToString();
            }
            set
            {
                ViewState["FromSource"] = value;
            }
        }

        protected InvoiceInfo InvoiceInfo
        {
            get { return ViewState["InvoiceInfo"] as InvoiceInfo; }
            set { ViewState["InvoiceInfo"] = value; }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //RDP_AddInvoiceTime.SelectedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
                if (Request.QueryString["InvoiceId"] != null)
                {
                    BindInvoice(new Guid(Request.QueryString["InvoiceId"]));
                }
            }
        }

        #region[保存]
        protected void BtSaveClick(object sender, EventArgs e)
        {
            if (RDP_AddInvoiceTime.SelectedDate == null)
            {
                RAM.Alert("请选择发票开篇日期");
                return;
            }
            if (RcbSaleFiliale.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择发票公司");
                return;
            }
            try
            {
                var invoiceInfo = InvoiceInfo ?? new InvoiceInfo();
                if (InvoiceInfo == null)
                {
                    invoiceInfo = new InvoiceInfo
                    {
                        InvoiceId = Guid.NewGuid(),
                        MemberId = Guid.Empty,
                        PostalCode = "201600",
                        AcceptedTime = RDP_AddInvoiceTime.SelectedDate.Value,
                        Address = TB_Address.Text,
                        InvoiceCode = TB_Code.Text,
                        InvoiceNo = long.Parse(TB_No.Text),
                        InvoiceName = TB_Name.Text,
                        InvoiceContent = TB_Content.Text,
                        Receiver = TB_Receiver.Text,
                        RequestTime = RDP_AddInvoiceTime.SelectedDate.Value,
                        InvoiceSum = Convert.ToDouble(TB_Sum.Text),
                        InvoiceState = 2,
                        NoteType = (InvoiceNoteType)Convert.ToInt32(DDR_Type.SelectedValue),
                        PurchaserType = 0,
                        IsCommit = false,
                        InvoiceChNo = 0,
                        SaleFilialeId = new Guid(RcbSaleFiliale.SelectedValue)
                    };
                    if(invoiceInfo.SaleFilialeId!=Guid.Empty)
                        _invoice.Insert(invoiceInfo);
                    //发票管理新建操作记录添加
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, invoiceInfo.InvoiceId, invoiceInfo.InvoiceNo.ToMendString(8),
                        OperationPoint.InvoiceManage.Create.GetBusinessInfo(), string.Empty);
                }
                else
                {
                    invoiceInfo.InvoiceName = TB_Name.Text;
                    invoiceInfo.InvoiceContent = TB_Content.Text;
                    invoiceInfo.Address = TB_Address.Text;
                    invoiceInfo.InvoiceCode = TB_Code.Text;
                    invoiceInfo.InvoiceNo = long.Parse(TB_No.Text);
                    invoiceInfo.NoteType = (InvoiceNoteType)Convert.ToInt32(DDR_Type.SelectedValue);
                    invoiceInfo.InvoiceSum = Convert.ToDouble(TB_Sum.Text);
                    invoiceInfo.Receiver = TB_Receiver.Text;
                    invoiceInfo.RequestTime = RDP_AddInvoiceTime.SelectedDate.Value;
                    invoiceInfo.SaleFilialeId = new Guid(RcbSaleFiliale.SelectedValue);
                    _invoice.UpdateInvoice(invoiceInfo);
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception ex)
            {
                RAM.Alert("保存失败，系统提示：" + ex.Message);
            }
        }
        #endregion

        protected void RcbFromSourceLoad(object sender, EventArgs e)
        {
            var rcb = (RadComboBox)sender;
            foreach (var info in SaleFilialeList)
            {
                rcb.Items.Add(new RadComboBoxItem { Text = info.Name, Value = info.ID.ToString(), Selected = info.ID.ToString() == SelectSaleFilialeId });
            }
        }

        /// <summary>
        /// 绑定发票信息
        /// </summary>
        /// <param name="invoiceId"></param>
        protected void BindInvoice(Guid invoiceId)
        {
            var info = BLL.Implement.Inventory.Invoice.ReadInstance.GetInvoice(invoiceId);
            if (info != null)
            {
                InvoiceInfo = info;
                TB_Name.Text = info.InvoiceName;
                TB_Content.Text = info.InvoiceContent;
                TB_Address.Text = info.Address;
                TB_Code.Text = info.InvoiceCode;
                TB_No.Text = info.InvoiceNo + "";
                DDR_Type.SelectedValue = (int)info.NoteType + "";
                TB_Sum.Text = info.InvoiceSum.ToString(CultureInfo.InvariantCulture);
                TB_Receiver.Text = info.Receiver;
                RDP_AddInvoiceTime.SelectedDate = info.RequestTime;
                SelectSaleFilialeId = info.SaleFilialeId.ToString();
            }
        }
    }
}
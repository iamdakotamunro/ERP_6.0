using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ERP.BLL.Implement.Inventory;
using ERP.Enum;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class BatchAddInvoice : WindowsPage
    {
        //private readonly Invoice invoice = new Invoice();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }


        protected void BtSaveClick(object sender, EventArgs e)
        {
            if (RDP_AddInvoiceStartTime.SelectedDate == null)
            {
                RAM.Alert("请选择开始开票日期");
                return;
            }
            if (RDP_AddInvoiceEndTime.SelectedDate == null)
            {
                RAM.Alert("请选择结束开票日期");
                return;
            }
            if (RcbSaleFiliale.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择发票公司");
                return;
            }
            var startNo = Convert.ToInt64(TB_StartNo.Text);
            var endNo = Convert.ToInt64(TB_EndNo.Text);

            if (startNo > endNo)
            {
                RAM.Alert("请检查发票号码是否按从小到大填写");
                return;
            }
            var startSum = Convert.ToDouble(TB_StartSum.Text);
            var endSum = Convert.ToDouble(TB_EndSum.Text);
            if (startSum > endSum)
            {
                RAM.Alert("请检查发票金额是否按从小到大填写");
                return;
            }
            var startDate = Convert.ToDateTime(RDP_AddInvoiceStartTime.SelectedDate);
            var endDate = Convert.ToDateTime(RDP_AddInvoiceEndTime.SelectedDate);
            if (startDate > endDate)
            {
                RAM.Alert("请检查发票日期是否按从小到大填写");
                return;
            }
            var list=new List<InvoiceInfo>();
            var dateList = new List<DateTime>();
            var sumList = new List<double>();
            var count = endNo - startNo+1;
            for (int i = 0; i < count; i++)
            {
               Thread.Sleep(10);
                var date = GetRandomDateTime(startDate, endDate);
                var sum = Convert.ToDouble(GetRandomInt(Convert.ToInt32(startSum), Convert.ToInt32(endSum)));
                dateList.Add(date);
                sumList.Add(sum);
            }
            var newDateList = dateList.OrderBy(ent => ent).ToList();
            int index=0;
            for (long i = startNo; i <= endNo; i++)
            {
                var realDate = newDateList[index];
                var realSum = sumList[index];
                var invoiceInfo = new InvoiceInfo
                {
                    InvoiceId = Guid.NewGuid(),
                    MemberId = Guid.Empty,
                    PostalCode = "201600",
                    AcceptedTime = realDate,
                    Address = TB_Address.Text,
                    InvoiceCode = TB_Code.Text,
                    InvoiceNo = i,
                    InvoiceName = TB_Name.Text,
                    InvoiceContent = TB_Content.Text,
                    Receiver = TB_Receiver.Text,
                    RequestTime = realDate,
                    InvoiceSum = realSum,
                    InvoiceState = 2,
                    //NoteType = (InvoiceNoteType)Convert.ToInt32(DDR_Type.SelectedValue),
                    NoteType =  (InvoiceNoteType)Convert.ToInt32(DDR_Type.SelectedValue),
                    PurchaserType = 0,
                    IsCommit = false,
                    InvoiceChNo = 0,
                    SaleFilialeId = new Guid(RcbSaleFiliale.SelectedValue)
                };
                list.Add(invoiceInfo);
                index++;
            }
            foreach (var item in list)
            {
                Invoice.WriteInstance.Insert(item);
            }
        }

        /// <summary>获取两个日期之间随机日期
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private DateTime GetRandomDateTime(DateTime startTime, DateTime endTime)
        {
            var random = new Random(DateTime.Now.Millisecond);
            DateTime tempTime;
            var ts = new TimeSpan(endTime.Ticks - startTime.Ticks);
            double totalSecontds = ts.TotalSeconds;
            int temptotalSecontds;
            if (totalSecontds > Int32.MaxValue)
                temptotalSecontds = Int32.MaxValue;
            else if (totalSecontds < Int32.MinValue)
                temptotalSecontds = Int32.MinValue;
            else
                temptotalSecontds = (int)totalSecontds;
            if (temptotalSecontds > 0)
                tempTime = startTime;
            else if (temptotalSecontds < 0)
                tempTime = startTime;
            else
                return startTime;
            int maxValue = temptotalSecontds;
            if (temptotalSecontds <= Int32.MinValue)
                maxValue = Int32.MinValue + 1;
            int i = random.Next(Math.Abs(maxValue));
            return tempTime.AddSeconds(i);
        }
        
        private Int32 GetRandomInt(int minValue, int maxValue)
        {
            var random = new Random(DateTime.Now.Millisecond);
            return random.Next(minValue, maxValue);
        }

        protected void RcbFromSourceLoad(object sender, EventArgs e)
        {
            var rcb = (RadComboBox)sender;
            foreach (var info in SaleFilialeList)
            {
                rcb.Items.Add(new RadComboBoxItem { Text = info.Name, Value = info.ID.ToString(), Selected = info.ID.ToString() == SelectSaleFilialeId });
            }
        }

        /// <summary> 订单来源
        /// </summary>
        public IList<FilialeInfo> SaleFilialeList
        {
            get
            {
                if (ViewState["SaleFilialeList"] == null)
                {
                    ViewState["SaleFilialeList"] = CacheCollection.Filiale.GetList().Where(act => act.Rank == (int)FilialeRank.Head).ToList();
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
    }
}
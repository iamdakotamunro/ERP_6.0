using System;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using OperationLog.Core;

namespace ERP.UI.Web.Windows
{
    ///<summary>
    /// 发票遗失
    ///</summary>
    public partial class InvoiceLost : WindowsPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtSaveClick(object sender, EventArgs e)
        {
            try
            {
                var rollId = WebControl.GetGuidFromQueryString("RollId");
                var invoiceRollDetail = new Keede.Ecsoft.Model.InvoiceRollDetail
                {
                    RollId = rollId,
                    StartNo = long.Parse(TB_StartNo.Text),
                    EndNo = long.Parse(TB_EndNo.Text),
                    Remark = TB_Memo.Text,
                    State = InvoiceRollState.Lost
                };
                IInvoice invoiceDao=new Invoice(GlobalConfig.DB.FromType.Write);
                var invoiceRollInfo = invoiceDao.GetRollList().FirstOrDefault(i => i.Id == rollId);
                if(invoiceRollInfo==null)return;
                if ((invoiceRollDetail.StartNo >= invoiceRollInfo.InvoiceStartNo & invoiceRollDetail.StartNo <= invoiceRollInfo.InvoiceEndNo) &&
                    (invoiceRollDetail.EndNo >= invoiceRollInfo.InvoiceStartNo & invoiceRollDetail.EndNo <= invoiceRollInfo.InvoiceEndNo))
                {
                    invoiceDao.InsertRollDetail(invoiceRollDetail);
                    //发票库管理遗失操作记录添加
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, invoiceRollInfo.Id, invoiceRollInfo.InvoiceCode,
                                               OperationPoint.InvoiceStorageManage.Loss.GetBusinessInfo(), string.Empty);
                    RAM.ResponseScripts.Add("alert('保存成功');window.close(); ");
                }
                else
                {
                    RAM.ResponseScripts.Add("alert('起始号或者结束号不再该发票卷中');");
                }
            }
            catch (Exception exception)
            {
                RAM.Alert("保存失败，系统提示：" + exception.Message);
            }
        }
    }
}

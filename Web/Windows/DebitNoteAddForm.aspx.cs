using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>手动添加赠品借记单 ADD 2015-02-06 陈重文
    /// </summary>
    public partial class DebitNoteAddForm : WindowsPage
    {
        protected SubmitController SubmitController;
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["AddPurchaseSetForm"] == null)
            {
                SubmitController = new SubmitController(Guid.NewGuid());
                ViewState["AddPurchaseSetForm"] = SubmitController;
            }
            return (SubmitController)ViewState["AddPurchaseSetForm"];
        }
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitController = CreateSubmitInstance();
            if (IsPostBack) return;
            var list = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers).Where(ent => ent.State == 1).ToList();
            RCB_Company.DataSource = list;
            RCB_Company.DataBind();
            RCB_Company.Items.Insert(0, new RadComboBoxItem("选择供应商", Guid.Empty.ToString()));
            RCB_Company.SelectedIndex = 0;
            //绑定仓库
            RCB_Warehouse.DataSource = WMSSao.GetWarehouseAuthDic(CurrentSession.Personnel.Get().PersonnelId).WarehouseDics;
            RCB_Warehouse.DataBind();
            //绑定责任人
            //var branchId = new Guid("E50EDCB7-2124-4DB4-AB62-5E8D8CCD8C2B");//采购部
            //IList<PersonnelInfo> personnelList =new PersonnelSao().GetList().Where(ent => ent.BranchId == branchId && ent.IsActive).ToList();//只显示在职

            RCB_Persion.DataSource = PersonnelList;
            RCB_Persion.DataBind();
            txt_ActivityTimeStart.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txt_ActivityTimeEnd.Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
        }

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

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSave_OnClick(object sender, ImageClickEventArgs e)
        {
            if (!SubmitController.Enabled)
            {
                RAM.Alert("程序正在处理");
                return;
            }
            var title = TB_Title.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                RAM.Alert("系统提示：标题不能为空！");
                return;
            }

            var companyId = RCB_Company.SelectedValue;
            if (string.IsNullOrWhiteSpace(companyId) || companyId == Guid.Empty.ToString())
            {
                RAM.Alert("系统提示：请选择供应商！");
                return;
            }
            var persion = RCB_Persion.SelectedValue;
            if (string.IsNullOrWhiteSpace(persion))
            {
                RAM.Alert("系统提示：请选择责任人！");
                return;
            }
            var warehouse = RCB_Warehouse.SelectedValue;
            if (string.IsNullOrWhiteSpace(warehouse))
            {
                RAM.Alert("系统提示：请选择仓库！");
                return;
            }
            var memo = TB_Memo.Text;
            if (string.IsNullOrWhiteSpace(memo))
            {
                RAM.Alert("系统提示：请填写赠品借记单备注信息！");
                return;
            }
            var presentAmount = RTB_PresentAmount.Text.Trim();
            Decimal amount = 0;
            if (!string.IsNullOrWhiteSpace(presentAmount))
            {
                //正则表达式
                if (!Regex.IsMatch(presentAmount, @"^(([1-9]\d{0,9})|0)(\.\d{1,2})?$"))
                {
                    RAM.Alert("系统提示：输入金额格式不正确！");
                    return;
                }
                amount = Convert.ToDecimal(presentAmount);
            }

            var personnel = CurrentSession.Personnel.Get();
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var description = string.Format("[新增赠品借记单][备注:{0};操作人:{1};{2}]<br/><br/>", memo.Trim(), personnel.RealName, dateTime);
            var debitNoteInfo = new DebitNoteInfo
                                            {
                                                PurchasingId = Guid.NewGuid(),
                                                PurchasingNo = "-",
                                                CompanyId = new Guid(companyId),
                                                PresentAmount = amount,
                                                CreateDate = DateTime.Now,
                                                FinishDate = DateTime.MinValue,
                                                State = (int)DebitNoteState.Purchasing,
                                                WarehouseId = new Guid(warehouse),
                                                Memo = description,
                                                PersonResponsible = new Guid(persion),
                                                PurchaseGroupId = Guid.Empty,
                                                Title = title,
                                                ActivityTimeStart = Convert.ToDateTime(txt_ActivityTimeStart.Text),
                                                ActivityTimeEnd = Convert.ToDateTime(txt_ActivityTimeEnd.Text)
                                            };
            IDebitNote debitNote = new DebitNote(GlobalConfig.DB.FromType.Write);
            var result = debitNote.AddPurchaseSetAndDetail(debitNoteInfo, new List<DebitNoteDetailInfo>());
            if (result)
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                SubmitController.Submit();
            }
        }
    }
}
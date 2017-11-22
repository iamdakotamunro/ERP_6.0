using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.Cache;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

/*
 * 创建人：刘彩军
 * 创建时间：2011-June-14th
 * 文件作用:往来单位收付款统计页面
 */
namespace ERP.UI.Web
{
    /// <summary>往来单位收付款统计
    /// </summary>
    public partial class CompanyFundStatistics : BasePage
    {
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Read);
        private readonly IBankAccounts _bankAccounts = new BankAccounts(GlobalConfig.DB.FromType.Read);
        private readonly IPersonnelSao _personnelManager=new PersonnelSao();
        protected List<CompanyCussentInfo> CompanyList;
        private decimal _amount;
        private decimal _payAmount;
        private decimal _cost;
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));


        #region[页面加载]
        protected void Page_Load(object sender, EventArgs e)
        {
            CompanyList = GetCompanyList();
            //收付款统计增加门店（公司）  ADD  2015-03-16  陈重文 
            var filialeList = CacheCollection.Filiale.GetList();
            foreach (var filialeInfo in filialeList)
            {
                var info = new CompanyCussentInfo
                {
                    CompanyId = filialeInfo.ID,
                    CompanyName = filialeInfo.Name
                };
                CompanyList.Add(info);
            }
            if (!IsPostBack)
            {
                BindCompany(string.Empty);
                if (StartTime != DateTime.MinValue && StartTime != DateTime.MaxValue)
                {
                    RDP_StartTime.SelectedDate = StartTime;
                }
                else
                {
                    RDP_StartTime.SelectedDate = DateTime.Now.Date.AddDays(-30);
                    StartTime = DateTime.Now.Date.AddDays(-30);
                }

                if (EndTime != DateTime.MinValue && EndTime != DateTime.MinValue.AddDays(1).AddSeconds(-1))
                {
                    RDP_EndTime.SelectedDate = EndTime;
                }
                else
                {
                    RDP_EndTime.SelectedDate = DateTime.Now.Date;
                    EndTime = DateTime.Now.Date;
                }
                DdlBankAccount.DataSource = BindBankDataBound();
                DdlBankAccount.DataBind();

                #region  公司绑定
                var dics = new Dictionary<Guid, string>
                {
                    {Guid.Empty,string.Empty}
                };
                var headList = CacheCollection.Filiale.GetHeadList();
                foreach (var filialeInfo in headList)
                {
                    dics.Add(filialeInfo.ID, filialeInfo.Name);
                }
                dics.Add(_reckoningElseFilialeid, "ERP");
                DdlFiliale.DataSource = dics;
                DdlFiliale.DataBind();
                #endregion
            }
        }
        #endregion

        #region[往来单位统计列表]
        protected void RgCompanyNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<CompanyFundReceiptInfo> list;
            var bankId = DdlBankAccount.SelectedValue;
            if (!string.IsNullOrEmpty(bankId) && bankId != Guid.Empty.ToString())
            {
                list = _companyFundReceipt.GetCompanyFundStatistics(CompanyId, AuditingId, new Guid(bankId), false, StartTime, EndTime, FilialeId);
            }
            else
            {
                list = _companyFundReceipt.GetCompanyFundStatistics(CompanyId, AuditingId, Guid.Empty, false, StartTime, EndTime, FilialeId);
            }
            _payAmount = list.Sum(act => act.PayRealityBalance);
            _amount = list.Sum(act => act.RealityBalance);
            RGCompany.DataSource = list.OrderByDescending(act => Math.Abs(act.PayRealityBalance));
        }
        #endregion

        #region[列表明细]
        protected void RgCompanyDetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = e.DetailTableView.ParentItem;
            var companyId = new Guid(dataItem.GetDataKeyValue("CompanyID").ToString());
            //Boolean isOut = Boolean.Parse(dataItem.GetDataKeyValue("IsOut").ToString());
            IList<CompanyFundReceiptInfo> list = _companyFundReceipt.GetFundReceiptListByCompanyID(companyId,null);
            if (AuditingId != Guid.Empty)
            {
                list = list.Where(c => c.AuditorID == AuditingId).ToList();
            }
            if (FilialeId!=Guid.Empty)
            {
                list = list.Where(act => act.FilialeId == FilialeId).ToList();
            }
            //list = list.Where(c => c.IsOut == isOut).ToList();
            list = list.Where(c => c.ApplyDateTime >= StartTime && c.ApplyDateTime <= EndTime).ToList();
            if (!string.IsNullOrEmpty(DdlBankAccount.SelectedValue) && DdlBankAccount.SelectedValue != Guid.Empty.ToString())
            {
                list = list.Where(ent => ent.PayBankAccountsId == new Guid(DdlBankAccount.SelectedValue)).ToList();
            }
            e.DetailTableView.DataSource = list.OrderByDescending(ent => ent.ApplyDateTime).ToList();
            _cost = list.Sum(c => c.RealityBalance);
        }
        #endregion

        #region[时间段]
        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set
            {
                ViewState["StartTime"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null || Convert.ToDateTime(ViewState["EndTime"]) == DateTime.MinValue) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).AddSeconds(-1);
            }
            set
            {
                ViewState["EndTime"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }
        #endregion

        #region[搜索往来单位ID]
        public Guid CompanyId
        {
            set
            {
                ViewState["CompanyID"] = value;
            }
            get
            {
                if (ViewState["CompanyID"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["CompanyID"].ToString());
            }
        }
        #endregion

        #region[搜索公司ID]
        public Guid FilialeId
        {
            set
            {
                ViewState["FilialeId"] = value;
            }
            get
            {
                if (ViewState["FilialeId"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["FilialeId"].ToString());
            }
        }
        #endregion

        #region[搜索审批人ID]
        public Guid AuditingId
        {
            set
            {
                ViewState["AuditingID"] = value;
            }
            get
            {
                if (ViewState["AuditingID"] == null)
                    return Guid.Empty;
                return new Guid(ViewState["AuditingID"].ToString());
            }
        }
        #endregion

        #region[绑定往来单位]
        public void BindCompany(string search)
        {
            var list = _companyCussent.GetCompanyCussentList(State.Enable).Where(c => c.CompanyType != (int)CompanyType.MemberGeneralLedger).ToList();
            DDL_Company.DataSource = string.IsNullOrEmpty(search) ? list : list.Where(act => act.CompanyName.Contains(search));
            DDL_Company.DataTextField = "CompanyName";
            DDL_Company.DataValueField = "CompanyId";
            DDL_Company.DataBind();
            if (string.IsNullOrEmpty(search))
            {
                DDL_Company.Items.Insert(0, new RadComboBoxItem("请选择", "-1"));
                DDL_Company.SelectedValue = "-1";
            }
        }
        #endregion

        #region[搜索审批人]
        protected void RcbAuditingItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text;
                IList<PersonnelInfo> personnelList = _personnelManager.GetList().Where(p => p.RealName.IndexOf(key, StringComparison.Ordinal) > -1).ToList();
                Int32 totalCount = personnelList.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (PersonnelInfo personnelInfo in personnelList)
                    {
                        var item = new RadComboBoxItem
                        {
                            Text = personnelInfo.RealName,
                            Value = personnelInfo.PersonnelId.ToString()
                        };
                        combo.Items.Add(item);
                    }
                }
            }
        }
        #endregion

        #region[获取公司名称]
        /// <summary>
        /// 获取公司名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected string GetCompanyName(string id)
        {
            try
            {
                var companyInfo = CompanyList.FirstOrDefault(c => c.CompanyId == new Guid(id));
                if (companyInfo != null)
                {
                    return companyInfo.CompanyName;
                }
                return "该往来单位已被删除";
            }
            catch
            {
                return "该往来单位已被删除";
            }
        }
        #endregion

        #region[获取申请人名称]
        /// <summary>
        /// 获取申请者名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected string GetApplicantName(string id)
        {
            return _personnelManager.GetName(new Guid(id));
        }
        #endregion

        #region[获取单据状态]
        /// <summary>
        /// 获取单据状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        protected string GetReceiptStatus(string status)
        {
            var rstatus = (CompanyFundReceiptState)Convert.ToInt32(status);
            return EnumAttribute.GetKeyName(rstatus);
        }
        #endregion

        #region[获取单据类型]
        /// <summary>
        /// 获取单据类型，区别是付款单还是收款单
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string GetReceiptType(string type)
        {
            var rtype = (CompanyFundReceiptType)Convert.ToInt32(type);
            return EnumAttribute.GetKeyName(rtype);
        }
        #endregion

        #region[获取往来单位信息]
        /// <summary>
        /// 获取往来单位信息
        /// </summary>
        protected List<CompanyCussentInfo> GetCompanyList()
        {
            List<CompanyCussentInfo> data = RelatedCompany.Instance.ToList().Where(c => c.State == (int)State.Enable).ToList();
            return data;
        }
        #endregion

        #region[统计]
        protected void LbSearchClick(object sender, ImageClickEventArgs e)
        {
            CompanyId = new Guid(DDL_Company.SelectedValue == "-1" || string.IsNullOrEmpty(DDL_Company.SelectedValue.Trim()) ? Guid.Empty.ToString() : DDL_Company.SelectedValue);
            FilialeId = new Guid(string.IsNullOrEmpty(DdlFiliale.SelectedValue.Trim()) ? Guid.Empty.ToString() : DdlFiliale.SelectedValue);
            AuditingId = !string.IsNullOrEmpty(RCB_Auditing.Text) ? new Guid(RCB_Auditing.SelectedValue) : Guid.Empty;
            StartTime = RDP_StartTime.SelectedDate ?? DateTime.MinValue;
            EndTime = RDP_EndTime.SelectedDate ?? DateTime.MinValue;
            RGCompany.Rebind();
        }
        #endregion

        #region[获取审批人]
        public string GetAuditingById(object id)
        {
            return _personnelManager.GetName(new Guid(id.ToString()));
        }
        #endregion

        #region[金额汇总]
        /// <summary>
        /// 金额汇总
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgCompanyItemDataBound(object sender, GridItemEventArgs e)
        {
            //if (_cost <= 0)
            //{
            //if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            //{
            //    var lab = e.Item.FindControl("Lab_RealityBalance") as Label;
            //    if (lab != null)
            //        if (!string.IsNullOrEmpty(lab.Text))
            //        {
            //            _amount += Convert.ToDecimal(lab.Text);
            //        }
            //    //
            //    var labPay = e.Item.FindControl("LabPayRealityBalance") as Label;
            //    if (labPay != null)
            //        if (!string.IsNullOrEmpty(labPay.Text))
            //        {
            //            _payAmount += Convert.ToDecimal(labPay.Text);
            //        }
            //}
            //}
            if (e.Item.ItemType == GridItemType.Footer)
            {
                var tbTotalAmount = e.Item.FindControl("TB_TotalAmount") as Label;
                if (tbTotalAmount != null)
                {
                    tbTotalAmount.Text += string.Format("合计：{0} 元", WebControl.NumberSeparator(_amount));// "计算的总数，或者也可以单独计算 ";// 
                }
                var tbPayTotalAmount = e.Item.FindControl("TbTotalAmount") as Label;
                if (tbPayTotalAmount != null)
                {
                    tbPayTotalAmount.Text += string.Format("合计：{0} 元", WebControl.NumberSeparator(_payAmount));// "计算的总数，或者也可以单独计算 ";// 
                }
            }
        }
        #endregion

        #region[明细金额汇总]
        public string GetDetailSum()
        {
            return string.Format("合计{0}元", WebControl.NumberSeparator(_cost));
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void DDL_Company_ItemRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            DDL_Company.Items.Clear();
            BindCompany(e.Text);
        }

        /// <summary> 绑定付款银行列表
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> BindBankDataBound()
        {
            var personnel = CurrentSession.Personnel.Get();
            var newDic = new Dictionary<string, string> { { Guid.Empty.ToString(), string.Empty } };
            var list = _bankAccounts.GetBankAccountsList(personnel.FilialeId, personnel.BranchId, personnel.PositionId);
            foreach (var item in list)
            {
                newDic.Add(item.BankAccountsId.ToString(), item.BankName + "-" + item.AccountsName);
            }
            return newDic;
        }

        #region  获取公司名称
        /// <summary>
        /// 获取公司
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetFilialeName(object obj)
        {
            if (obj == null) return string.Empty;
            if (string.IsNullOrEmpty(obj.ToString())) return string.Empty;
            var infoName = CacheCollection.Filiale.GetName(new Guid(obj.ToString()));
            return string.IsNullOrEmpty(infoName) ? "ERP" : infoName;
        }
        #endregion
    }
}

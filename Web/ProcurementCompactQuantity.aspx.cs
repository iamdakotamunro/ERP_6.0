using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>▄︻┻┳═一 采购合同量   ADD 2014-12-17  陈重文
    /// </summary>
    public partial class ProcurementCompactQuantity : BasePage
    {
        private static readonly IPurchaseSet _purchaseSet=new PurchaseSet(GlobalConfig.DB.FromType.Read);
        private readonly ProcurementCompactQuantityDAL _procurementCompactQuantityDal=new ProcurementCompactQuantityDAL(GlobalConfig.DB.FromType.Write);
        /// <summary>采购责任人Id集合
        /// </summary>
        private static readonly IList<Guid> _personIdList = _purchaseSet.GetPersonList().Select(ent => ent.PersonResponsible).ToList();
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        /// <summary>
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadShowData();
            }
        }

        #region [加载供应商列表和年份]

        private void LoadShowData()
        {
            var personnel = CurrentSession.Personnel.Get();
            if (personnel == null)
            {
                RAM.Alert("系统提示：无法获取当前登录人信息，请尝试重新登录后再操作！");
                return;
            }
            IList<CompanyCussentInfo> list = _personIdList.Contains(personnel.PersonnelId) ? _purchaseSet.GetCompanyIds(personnel.PersonnelId) : _companyCussent.GetCompanyCussentList(CompanyType.Suppliers, State.Enable);
            RCB_Company.DataSource = list;
            RCB_Company.DataTextField = "CompanyName";
            RCB_Company.DataValueField = "CompanyId";
            RCB_Company.DataBind();
            RCB_Company.Items.Insert(0, new RadComboBoxItem("全部供应商", Guid.Empty.ToString()));
            RCB_Company.SelectedIndex = 0;

            //加载年份
            var year = DateTime.Now.Year;
            for (var i = year-1; i <= year + 1; i++)
            {
                RCB_Year.Items.Add(new RadComboBoxItem(string.Format("{0}", i), string.Format("{0}", i)));
            }
            RCB_Year.SelectedValue = string.Format("{0}", year);
            var str = year + "年" + DateTime.Now.Month + "月采购金额";
            RadGridProcurementCompactQuantity.MasterTableView.Columns.FindByUniqueName("TheMonthProcurementMoney").HeaderText = str;
        }

        #endregion

        #region [绑定数据源]

        /// <summary>绑定数据源
        /// </summary>
        protected void RadGridProcurementCompactQuantity_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<ProcurementCompactQuantityInfo> list = new List<ProcurementCompactQuantityInfo>();
            if (IsPostBack)
            {
                var personnel = CurrentSession.Personnel.Get();
                if (personnel == null)
                {
                    RAM.Alert("系统提示：无法获取当前登录人信息，请尝试重新登录后再操作！");
                    return;
                }
                var companyId = new Guid(RCB_Company.SelectedValue);
                var result = _personIdList.Contains(personnel.PersonnelId);
                if (companyId == Guid.Empty && !result)
                {
                    list = _procurementCompactQuantityDal.GetProcurementCompactQuantityList(DateYear);
                }
                else if (companyId != Guid.Empty && !result)
                {
                    list = _procurementCompactQuantityDal.GetProcurementCompactQuantityList(DateYear, companyId);
                }
                else if (companyId == Guid.Empty && result)
                {
                    list = _procurementCompactQuantityDal.GetProcurementCompactQuantityList(personnel.PersonnelId, DateYear);
                }
                else if (companyId != Guid.Empty && result)
                {
                    list = _procurementCompactQuantityDal.GetProcurementCompactQuantityList(DateYear, companyId, personnel.PersonnelId);
                }
            }
            RadGridProcurementCompactQuantity.DataSource = list.OrderBy(ent => ent.CompanyName).ToList();
        }

        #endregion

        #region [搜索]

        /// <summary>搜索
        /// </summary>
        protected void OnClick_Btn_Search(object sender, EventArgs e)
        {
            DateYear = Convert.ToInt32(RCB_Year.SelectedValue);
            RadGridProcurementCompactQuantity.Rebind();
        }

        #endregion

        #region [文本框触发保存]

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnTextChanged_CompactMoney(object sender, EventArgs e)
        {
            try
            {
                var tbCompactMoney = (TextBox)sender;
                var dataItem = (GridDataItem)tbCompactMoney.Parent.Parent;
                var companyId = dataItem.GetDataKeyValue("CompanyId");
                var compactMoney = WebControl.NumberRecovery(tbCompactMoney.Text);

                //正则表达式
                if (Regex.IsMatch(compactMoney, @"^(([1-9]\d{0,9})|0)(\.\d{1,2})?$"))
                {
                    var info = new ProcurementCompactQuantityInfo
                                                              {
                                                                  CompanyId = new Guid(companyId.ToString()),
                                                                  DateYear = DateYear,
                                                                  CompactMoney = Convert.ToDecimal(compactMoney)
                                                              };
                    var result = _procurementCompactQuantityDal.SaveProcurementCompactQuantity(info);
                    if (!result)
                    {
                        RAM.Alert("系统提示：数据保存异常，请尝试刷新后继续操作！");
                    }
                    else
                    {
                        RadGridProcurementCompactQuantity.Rebind();
                    }
                }
                else
                {
                    var companyInfo = _companyCussent.GetCompanyCussent(new Guid(companyId.ToString()));
                    var companyName = string.Empty;
                    if (companyInfo != null)
                    {
                        companyName = companyInfo.CompanyName;
                    }
                    RAM.Alert(string.Format("系统提示：{0}输入金额格式不正确！(不能设置负值)", companyName));
                }
            }
            catch (Exception)
            {
                RAM.Alert("系统提示：保存异常，请尝试刷新后继续操作！");
            }
        }

        #endregion

        #region [ViewState]

        /// <summary>年份
        /// </summary>
        protected int DateYear
        {
            get
            {
                return ViewState["DateYear"] == null ? 0 : Convert.ToInt16(ViewState["DateYear"].ToString());
            }
            set
            {
                ViewState["DateYear"] = value;
            }
        }

        #endregion
    }
}
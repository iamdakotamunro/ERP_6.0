using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class CompanyPurchaseGroupForm : WindowsPage
    {
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyPurchaseGoupDao _companyPurchaseGoupDao = new CompanyPurchaseGoupDao(GlobalConfig.DB.FromType.Write);
        private readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                #region --> 供应商绑定

                RCB_Company.DataSource = CompanyCussentList;
                RCB_Company.DataBind();

                #endregion

            }
        }

        protected IList<CompanyPurchaseGoupInfo> CompanyPurchaseGoupList
        {
            get
            {
                if (ViewState["CompanyPurchaseGoupList"] == null)
                {
                    return new List<CompanyPurchaseGoupInfo>();
                }
                return (List<CompanyPurchaseGoupInfo>)ViewState["CompanyPurchaseGoupList"];
            }
            set { ViewState["CompanyPurchaseGoupList"] = value; }
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
                    ViewState["CompanyCussentList"] =_companyCussent.GetCompanyCussentList(CompanyType.Suppliers, State.Enable).ToList();
                }
                return (IList<CompanyCussentInfo>)ViewState["CompanyCussentList"];
            }
        }

        protected void RcbCompany_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var strCompanyId = RCB_Company.SelectedValue;
            var companyId = string.IsNullOrEmpty(strCompanyId) ? Guid.Empty : new Guid(strCompanyId);
            if (companyId != Guid.Empty)
            {
                var list = _companyPurchaseGoupDao.GetCompanyPurchaseGoupList(companyId);
                if (list.Count == 0)
                {
                    string errorMessage;
                    var info = new CompanyPurchaseGoupInfo
                               {
                                   CompanyId = companyId,
                                   PurchaseGroupId = Guid.Empty,
                                   PurchaseGroupName = "默认",
                                   OrderIndex = 0
                               };
                    var result = _companyPurchaseGoupDao.Insert(info, out errorMessage);
                    if (result == 0)
                    {
                        RAM.Alert("异常！" + errorMessage);
                    }
                    else
                    {
                        list.Add(info);
                    }
                }
                CompanyPurchaseGoupList = list;
            }
            else
            {
                CompanyPurchaseGoupList = new List<CompanyPurchaseGoupInfo>();
            }
            RG_Group.Rebind();
        }

        protected void RgGroup_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RG_Group.DataSource = CompanyPurchaseGoupList.OrderBy(w => w.OrderIndex).ToList();
        }

        protected void RgGroup_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var dataItem = e.Item as GridDataItem;
                var companyId = new Guid(dataItem.GetDataKeyValue("CompanyId").ToString());
                var purchaseGroupId = new Guid(dataItem.GetDataKeyValue("PurchaseGroupId").ToString());

                if (e.CommandName == "Delete")
                {
                    var list = CompanyPurchaseGoupList;
                    var info = list.FirstOrDefault(w => w.CompanyId == companyId && w.PurchaseGroupId == purchaseGroupId);
                    var isTrue = _companyPurchaseGoupDao.IsExist(companyId, purchaseGroupId);
                    string errorMessage = string.Empty;
                    bool isSuccess = true;
                    if (isTrue)
                    {
                        isSuccess = _companyPurchaseGoupDao.Delete(companyId, purchaseGroupId, out errorMessage);
                    }
                    if (!isSuccess && info != null && info.OrderIndex != 0)
                    {
                        RAM.Alert("操作无效！" + errorMessage);
                    }
                    else
                    {
                        if (info != null)
                        {
                            list.Remove(info);
                            CompanyPurchaseGoupList = list;
                            if (info.OrderIndex != 0)
                                _purchaseSet.UpdatePurchaseSetDefault(companyId, purchaseGroupId, out errorMessage);
                            RG_Group.Rebind();
                        }
                    }
                }
            }
        }

        private IList<CompanyPurchaseGoupInfo> GetRgGroupData()
        {
            IList<CompanyPurchaseGoupInfo> list = new List<CompanyPurchaseGoupInfo>();
            foreach (GridDataItem dataItem in RG_Group.Items)
            {
                var companyId = new Guid(dataItem.GetDataKeyValue("CompanyId").ToString());
                var purchaseGroupId = new Guid(dataItem.GetDataKeyValue("PurchaseGroupId").ToString());
                var orderIndex = int.Parse(dataItem.GetDataKeyValue("OrderIndex").ToString());

                var groupName = (TextBox)dataItem.FindControl("tbPurchaseGroupName");
                var name = groupName.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    RAM.Alert("列表中“分组名称”不允许为空");
                    return new List<CompanyPurchaseGoupInfo>();
                }
                if (list.Count(w => w.PurchaseGroupName == name) > 0)
                {
                    RAM.Alert("列表中“分组名称”不允许出现重复");
                    return new List<CompanyPurchaseGoupInfo>();
                }
                list.Add(new CompanyPurchaseGoupInfo
                {
                    CompanyId = companyId,
                    PurchaseGroupId = purchaseGroupId,
                    OrderIndex = orderIndex,
                    PurchaseGroupName = name
                });
            }
            return list;
        }

        protected void BtnAddGroup_OnClick(object sender, EventArgs e)
        {
            var strCompanyId = RCB_Company.SelectedValue;
            var companyId = string.IsNullOrEmpty(strCompanyId) ? Guid.Empty : new Guid(strCompanyId);
            if (companyId == Guid.Empty)
            {
                RAM.Alert("请选择供应商");
            }
            else
            {
                var name = tbPurchaseGroupName.Text.Trim();
                if (name == "分组名称")
                {
                    RAM.Alert("请填写分组名称");
                    return;
                }
                var list = GetRgGroupData();
                if (list.Count > 0)
                {
                    if (CompanyPurchaseGoupList.Count(w => w.PurchaseGroupName == name) > 0)
                    {
                        RAM.Alert("该分组名称已存在");
                        return;
                    }
                    var orderIndex = list.Max(w => w.OrderIndex) + 1;
                    var info = new CompanyPurchaseGoupInfo
                    {
                        CompanyId = companyId,
                        PurchaseGroupId = Guid.NewGuid(),
                        PurchaseGroupName = name,
                        OrderIndex = orderIndex
                    };

                    list.Add(info);
                    tbPurchaseGroupName.Text = "请填写分组名称";
                    CompanyPurchaseGoupList = list;
                    RG_Group.Rebind();
                }
            }
        }

        protected void BtnSave_OnClick(object sender, EventArgs e)
        {
            var list = GetRgGroupData().Where(w => w.OrderIndex != 0).ToList();
            if (list.Count > 0)
            {
                string errorMessage;
                bool isSuccess = _companyPurchaseGoupDao.InsertList(list, out errorMessage);
                if (!isSuccess)
                {
                    RAM.Alert("保存无效！" + errorMessage);
                }
                else
                {
                    RAM.Alert("保存成功！");
                }
            }
        }

        protected void RCB_Company_OnItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)o;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text;
                IList<CompanyCussentInfo> companyList =CompanyCussentList.Where(p => p.CompanyName.IndexOf(key, StringComparison.Ordinal) > -1).ToList();
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
    }
}
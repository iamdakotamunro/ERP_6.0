using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AllianceShop.Common.Extension;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.SAL;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL.Interface;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class CompanyCussentAuthorityAdd : System.Web.UI.Page
    {
        readonly IPersonnelSao _personnelSao = new PersonnelSao();
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly ICompanyCussentRelation _companyCussentRelation = new CompanyCussentRelation(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var companyId = Request.QueryString["companyId"];
                var type = Request.QueryString["type"];
                var accountNo = Request.QueryString["accountNo"];
                if (!String.IsNullOrWhiteSpace(companyId))
                {
                    if (type == "1")
                    {
                        RCB_Personel.Enabled = false;
                        RAM.ResponseScripts.Add(" $('#trType').hide();");
                        BindThirdCompany(new Guid(companyId));
                        LoadSaleFilialeData();
                        BindData(accountNo, companyId);
                        Btn_Serach.Visible = false;
                    }
                    else
                    {
                        BindThirdCompany(new Guid(companyId));
                        GridBindingData();
                        LoadSaleFilialeData();
                    }

                }
            }
        }

        private void BindData(String accountNo, String companyId)
        {
            var list = _companyCussentRelation.GetEditCompanyCussentRelationInfoList(accountNo, new Guid(companyId));
            if (list != null && list.Count > 0)
            {
                //RCB_Personel.SelectedItem.Text = list.First().AccountName;
                RCB_Personel.SelectedValue = list.First().AccountNo;
                RCB_Personel.Text = list.First().AccountName;
                foreach (ListItem item in ckb_RelatedSalesGroupPlatform.Items)
                {
                    foreach (var value in list.Select(ent => ent.SaleFilialeId))
                    {
                        if (item.Value.Equals(value.ToString()))
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
        }

        protected void LoadSaleFilialeData()
        {
            var saleFilialesGroupInfoList = CacheCollection.Filiale.GetSaleFilialeList().ToDictionary(k => k.ID, v => v.Name);
            ckb_RelatedSalesGroupPlatform.DataSource = saleFilialesGroupInfoList;
            ckb_RelatedSalesGroupPlatform.DataTextField = "Value";
            ckb_RelatedSalesGroupPlatform.DataValueField = "Key";
            ckb_RelatedSalesGroupPlatform.DataBind();
        }

        private void BindThirdCompany(Guid companyId)
        {
            var wList = _companyCussent.GetCompanyCussentList(State.Enable);
            var dics = wList.Where(ent => ent.CompanyId == companyId).ToDictionary(k => k.CompanyId, v => v.CompanyName);
            RcbThirdCompany.DataSource = dics;
            RcbThirdCompany.DataBind();
        }

        private void GridBindingData()
        {
            var personnelLists = _personnelSao.GetList();
            foreach (var personnelList in personnelLists)
            {
                FiltrateGoodsList.Items.Add(new RadListBoxItem(personnelList.RealName + "(" + personnelList.AccountNo + ")", personnelList.AccountNo));
            }
        }

        protected void SerachClick(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(RCB_Personel.SelectedValue))
            {
                var personnelLists = _personnelSao.GetList().Where(ent=> !ConfirmGoodsList.Items.Select(p=>p.Value).Contains(ent.AccountNo));
                foreach (var personnelList in personnelLists)
                {
                    FiltrateGoodsList.Items.Add(new RadListBoxItem(personnelList.RealName + "(" + personnelList.AccountNo + ")", personnelList.AccountNo));
                }
            }
            else
            {
                var personnelList = _personnelSao.Get(new Guid(RCB_Personel.SelectedValue));
                if (personnelList != null)
                {
                    FiltrateGoodsList.Items.Clear();
                    FiltrateGoodsList.Items.Add(new RadListBoxItem(personnelList.RealName + "(" + personnelList.AccountNo + ")", personnelList.AccountNo));
                }
                else
                {
                    RAM.Alert("温馨提示：未找到符合条件的登陆账号！");
                }
            }
           
        }


        protected void RcbPersonelItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text) && e.Text.Length >= 0)
            {
                if (e.Text.IsInt())
                {
                    var account = _personnelSao.Get(e.Text);
                    if (account != null && account.PersonnelId != Guid.Empty)
                    {
                        var rcb = new RadComboBoxItem
                        {
                            Text = account.RealName + "(" + account.AccountNo + ")",
                            Value = account.PersonnelId.ToString(),
                        };
                        combo.Items.Add(rcb);
                    }
                }
                else
                {
                    var list = _personnelSao.GetAccountInfoByRealName(e.Text);
                    var totalCount = list.Count;
                    if (e.NumberOfItems >= totalCount)
                        e.EndOfItems = true;
                    else
                    {
                        foreach (var item in list)
                        {
                            var rcb = new RadComboBoxItem
                            {
                                Text = item.RealName + "(" + item.AccountNo + ")",
                                Value = item.PersonnelId.ToString(),
                            };
                            combo.Items.Add(rcb);
                        }
                    }
                }
            }
            else
            {
                GridBindingData();
            }
        }

        #region [账号筛选确定事件]

        /// <summary>添加到商品确定框  
        /// </summary>
        protected void AddToRight(object sender, EventArgs e)
        {
            if (FiltrateGoodsList.Items.Count > 0)
            {
                IList<RadListBoxItem> collection = FiltrateGoodsList.CheckedItems;
                int i = 0;
                foreach (RadListBoxItem item in collection)
                {
                    var extis = ConfirmGoodsList.Items.Select(ent => ent.Value);
                    if (!extis.Contains(item.Value))
                        ConfirmGoodsList.Items.Insert(ConfirmGoodsList.Items.Count, item);
                    if (FiltrateGoodsList.Items.Count > 0)
                        FiltrateGoodsList.Items.Remove(FiltrateGoodsList.Items[i]);
                    i++;
                }

            }
            else
            {
                RAM.Alert("温馨提示：请先选择数据！");
            }
        }

        /// <summary>移除到商品筛选框
        /// </summary>
        protected void RemoveToLeft(object sender, EventArgs e)
        {
            if (ConfirmGoodsList.SelectedItem != null)
            {
                var extis = FiltrateGoodsList.Items.Select(ent => ent.Value);
                if (!extis.Contains(ConfirmGoodsList.SelectedItem.Value))
                    FiltrateGoodsList.Items.Insert(0, ConfirmGoodsList.SelectedItem);
                ConfirmGoodsList.Items.Remove(ConfirmGoodsList.SelectedItem);
            }
            else
            {
                RAM.Alert("温馨提示：请先选择数据！");
            }
        }

        /// <summary>全部添加到商品确定框
        /// </summary>
        protected void AllAddToRight(object sender, EventArgs e)
        {
            if (FiltrateGoodsList.Items.Count == 0)
            {
                RAM.Alert("温馨提示：没有数据！");
            }
            else
            {
                foreach (var items in FiltrateGoodsList.Items.ToList())
                {
                    ConfirmGoodsList.Items.Add(new RadListBoxItem(items.Text, items.Value));
                }
                FiltrateGoodsList.Items.Clear();
            }
        }

        /// <summary>全部移除到商品筛选框
        /// </summary>
        protected void AllRemoveToLeft(object sender, EventArgs e)
        {
            if (ConfirmGoodsList.Items.Count == 0)
            {
                RAM.Alert("温馨提示：没有数据！");
            }
            else
            {
                foreach (var items in ConfirmGoodsList.Items.ToList())
                {
                    FiltrateGoodsList.Items.Add(new RadListBoxItem(items.Text, items.Value));
                }
                ConfirmGoodsList.Items.Clear();
            }
        }

        #endregion


        protected void SaveClick(object sender, EventArgs e)
        {
            var existsIds = new List<Guid>();
            var deleteIds = new List<Guid>();
            var list = new List<CompanyCussentRelationInfo>();
            var personelIdDic = new Dictionary<String, String>();
            var companyId = new Guid(Request.QueryString["companyId"]);
            if (Request.QueryString["type"] != "1")
            {
                if (ConfirmGoodsList.Items.Count == 0)
                {
                    RAM.Alert("温馨提示：请选择需要绑定的登陆账号！");
                    return;
                }

                for (int i = 0; i < ConfirmGoodsList.Items.Count; i++)
                {
                    personelIdDic.Add(ConfirmGoodsList.Items[i].Value, ConfirmGoodsList.Items[i].Text);
                }

                var exists = _companyCussentRelation.GetCompanyCussentRelationInfoList(companyId);

                bool hasValue = false;
                foreach (var personelId in personelIdDic)
                {
                    foreach (ListItem item in ckb_RelatedSalesGroupPlatform.Items)
                    {

                        if (item.Selected)
                        {
                            if (exists.All(ent => ent.AccountNo != personelId.Key || ent.SaleFilialeId != new Guid(item.Value)))
                            {
                                int nStartIndex = personelId.Value.IndexOf('(');
                                string sResult = personelId.Value.Substring(0, nStartIndex);
                                list.Add(new CompanyCussentRelationInfo
                                {
                                    Id = Guid.NewGuid(),
                                    AccountNo = personelId.Key,
                                    AccountName = sResult,
                                    CompanyId = new Guid(RcbThirdCompany.SelectedValue),
                                    CompanyName = RcbThirdCompany.SelectedItem.Text,
                                    SaleFilialeId = new Guid(item.Value),
                                    SaleFilialeName = item.Text
                                });
                            }
                            else
                            {
                                if (!hasValue)
                                    hasValue = true;
                            }
                        }
                    }
                }
                if (list.Count == 0)
                {
                    RAM.Alert(hasValue ? "已存在相关的设置！" : "请选择需绑定的销售公司！");
                    return;
                }
            }
            else
            {
                personelIdDic.Add(RCB_Personel.SelectedValue, RCB_Personel.Text);
                var exists = _companyCussentRelation.GetEditCompanyCussentRelationInfoList(Request.QueryString["accountNo"], companyId);
                foreach (var personelId in personelIdDic)
                {
                    foreach (ListItem item in ckb_RelatedSalesGroupPlatform.Items)
                    {
                        if (item.Selected)
                        {
                            var relationInfo = exists.FirstOrDefault(ent => ent.SaleFilialeId == new Guid(item.Value));
                            if (relationInfo != null)
                            {
                                existsIds.Add(relationInfo.Id);
                                continue;
                            }
                            list.Add(new CompanyCussentRelationInfo
                            {
                                Id = Guid.NewGuid(),
                                AccountNo = personelId.Key,
                                AccountName = personelId.Value,
                                CompanyId = new Guid(RcbThirdCompany.SelectedValue),
                                CompanyName = RcbThirdCompany.SelectedItem.Text,
                                SaleFilialeId = new Guid(item.Value),
                                SaleFilialeName = item.Text
                            });
                        }
                    }
                }
                deleteIds = exists.Where(ent => !existsIds.Contains(ent.Id)).Select(ent => ent.Id).ToList();
                if (deleteIds.Count == 0 && list.Count == 0)
                {
                    RAM.Alert("未做任何修改、保存无效！");
                    return;
                }

                if (list.Count == 0 && exists.All(ent => !existsIds.Contains(ent.Id)))
                {
                    RAM.Alert("无法去除所有销售公司的绑定设置！");
                    return;
                }

            }
            var result = _companyCussentRelation.Save(deleteIds, list);
            if (!result)
            {
                RAM.Alert("保存失败！");
                return;
            }
            MessageBox.AppendScript(this, "alert('保存成功！');CloseAndRebind();");
        }

        protected void CancelClick(object sender, EventArgs e)
        {
            MessageBox.AppendScript(this, "CloseAndRebind();");
        }
    }
}
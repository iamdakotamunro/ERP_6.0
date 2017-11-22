using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.Cache;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using TextBox = System.Web.UI.WebControls.TextBox;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>往来单位资料 
    /// </summary>
    public partial class CompanyCussentControl : WindowsPage
    {
        static readonly CompanyBankAccountBindDao _companyBankAccountBindDao = new CompanyBankAccountBindDao(GlobalConfig.DB.FromType.Write);
        readonly ICompanyCussent _companyCussentManager = new CompanyCussent(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyPurchaseGoupDao _companyPurchaseGoupDao = new CompanyPurchaseGoupDao(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyInvoicePower _companyInvoicePowerDao = new DAL.Implement.Inventory.CompanyInvoicePower(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccountDao _bankAccounts = new BankAccountDao(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyBankAccountBind _companyBankAccountBind = new CompanyBankAccountBindDao(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussentRelation _companyCussentRelation = new CompanyCussentRelation(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var dataSource = CacheCollection.Filiale.GetList().Where(ent=>ent.IsActive && (ent.FilialeTypes.Contains((int)MIS.Enum.FilialeType.LogisticsCompany) || ent.FilialeTypes.Contains((int)MIS.Enum.FilialeType.SaleCompany) || (ent.FilialeTypes.Contains((int)MIS.Enum.FilialeType.EntityShop) && ent.ShopJoinType == (int)MIS.Enum.ShopJoinType.Join))).ToList();
                var companyId = !string.IsNullOrEmpty(Request.QueryString["CompanyId"]) ? new Guid(Request.QueryString["CompanyId"]) : Guid.Empty;
                CompanyCussentModel = companyId != Guid.Empty ? _companyCussentManager.GetCompanyCussent(new Guid(Request.QueryString["CompanyId"])) : null;
                GetCompanyClass();
                BindFiliale(dataSource.Where(ent=> ent.FilialeTypes.Contains((int)MIS.Enum.FilialeType.LogisticsCompany) || ent.FilialeTypes.Contains((int)MIS.Enum.FilialeType.SaleCompany)).ToList(),CompanyCussentModel != null ? CompanyCussentModel.RelevanceFilialeId : Guid.Empty);
                var isNormal = CompanyCussentModel != null && CompanyCussentModel.RelevanceFilialeId != Guid.Empty;
                RepeaterNext.Visible = isNormal;
                RepeartBank.Visible = !isNormal;
                if (companyId != Guid.Empty)
                {
                    companyId = new Guid(Request.QueryString["CompanyId"]);
                    IsEdit = true;
                    if (CompanyCussentModel == null)
                    {
                        RAM.Alert("不是有效的往来单位信息");
                        return;
                    }
                    BindInformation(CompanyCussentModel);
                }
                else
                {
                    companyId = Guid.NewGuid();
                }
                CompanyId = companyId;
                var dataList = dataSource.Select(filialeInfo => new CompanyBankAccountBindInfo
                {
                    FilialeId = filialeInfo.ID,
                    FilialeName = filialeInfo.Name,
                    CompanyId = companyId
                }).ToList();

                if (isNormal)
                {
                    RepeaterNext.DataSource = dataList;
                    RepeaterNext.DataBind();
                }
                else
                {
                    RepeartBank.DataSource = dataList;
                    RepeartBank.DataBind();
                }
                if (!String.IsNullOrWhiteSpace(Request.QueryString["CompanyId"]))
                {
                    if (CompanyCussentModel != null && CompanyCussentModel.SalesScope == (int)Enum.Overseas.SalesScopeType.Overseas)
                        RAM.ResponseScripts.Add(" $('#trType').show();");
                    else
                        RAM.ResponseScripts.Add(" $('#trType').hide();");
                }
                else
                {
                    RAM.ResponseScripts.Add("$('#trType').hide();");
                }
            }
        }

        #region[是否编辑]

        ///<summary>是否编辑
        ///</summary>
        public Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null) return Guid.Empty;
                return (Guid)ViewState["CompanyId"];
            }
            set
            {
                ViewState["CompanyId"] = value;
            }
        }

        ///<summary>是否编辑
        ///</summary>
        public bool IsEdit
        {
            get
            {
                if (ViewState["IsEdit"] == null) return false;
                return (bool)ViewState["IsEdit"];
            }
            set
            {
                ViewState["IsEdit"] = value;
            }
        }

        public CompanyCussentInfo CompanyCussentModel
        {
            get
            {
                if (ViewState["CompanyCussentModel"] == null) return null;
                return (CompanyCussentInfo)ViewState["CompanyCussentModel"];
            }
            set
            {
                ViewState["CompanyCussentModel"] = value;
            }
        }

        public Dictionary<Guid, Guid> ChildAndParent
        {
            get
            {
                if (ViewState["ChildAndParent"] == null) return new Dictionary<Guid, Guid>();
                return (Dictionary<Guid, Guid>)ViewState["ChildAndParent"];
            }
            set
            {
                ViewState["ChildAndParent"] = value;
            }
        } 
        #endregion

        #region[绑定显示内容]

        /// <summary>绑定显示内容
        /// </summary>
        /// <param name="companyCussentInfo">往来单位ID</param>
        public void BindInformation(CompanyCussentInfo companyCussentInfo)
        {
            TB_CompanyName.Text = companyCussentInfo.CompanyName;
            DDL_CompanyClassId.SelectedValue = companyCussentInfo.CompanyClassId.ToString();
            TB_Linkman.Text = companyCussentInfo.Linkman;
            TB_Mobile.Text = companyCussentInfo.Mobile;
            TB_Address.Text = companyCussentInfo.Address;
            TB_PostalCode.Text = companyCussentInfo.PostalCode;
            TB_Phone.Text = companyCussentInfo.Phone;
            TB_Fax.Text = companyCussentInfo.Fax;
            TB_Email.Text = companyCussentInfo.Email;
            CB_IsNeedInvoices.Checked = companyCussentInfo.IsNeedInvoices;
            DDL_CompanyType.SelectedValue = string.Format("{0}", companyCussentInfo.CompanyType);
            RBL_State.SelectedValue = string.Format("{0}", companyCussentInfo.State);
            TB_Description.Text = companyCussentInfo.Description;
            DDL_PaymentDays.SelectedValue = string.Format("{0}", companyCussentInfo.PaymentDays);
            DdlFiliale.SelectedValue = string.Format("{0}", companyCussentInfo.RelevanceFilialeId);
            rbl_SalesScope.SelectedValue = string.Format("{0}", companyCussentInfo.SalesScope);
            if (companyCussentInfo.SalesScope == (int) Enum.Overseas.SalesScopeType.Overseas)
            {
                rbl_DeliverType.SelectedValue = string.Format("{0}", companyCussentInfo.DeliverType);
                rbl_DeliverType.Enabled = companyCussentInfo.CompanyType != (int) CompanyType.Express;
            }
            else
            {
                rbl_DeliverType.SelectedValue = "0";
                rbl_DeliverType.Enabled = true;
            }    
        }

        #endregion

        #region[保存]

        /// <summary> 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnUpdateClick(object sender, ImageClickEventArgs e)
        {
            bool result = IsEdit ? Update() : Insert();
            if (result)
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        #endregion

        #region[添加]

        /// <summary>添加
        /// </summary>
        protected bool Insert()
        {
            string message;
            var dataList = GetBind(out message);
            if (message.Length > 0)
            {
                RAM.Alert(message);
                return false;
            }
            var companyClassId = new Guid(DDL_CompanyClassId.SelectedValue);
            string companyName = TB_CompanyName.Text;
            string linkman = TB_Linkman.Text;
            string address = TB_Address.Text;
            string postalCode = TB_PostalCode.Text;
            string phone = TB_Phone.Text;
            string mobile = TB_Mobile.Text;
            string fax = TB_Fax.Text;
            string email = TB_Email.Text;
            bool isNeedInvoices = CB_IsNeedInvoices.Checked;
            DateTime dateCreated = WebControl.GetNowTime();
            int companyType = Convert.ToInt32(DDL_CompanyType.SelectedValue);
            int state = Convert.ToInt32(RBL_State.SelectedValue);
            string description = TB_Description.Text;
            string subjectinfo = string.Empty;
            Guid filialeId = string.IsNullOrEmpty(DdlFiliale.SelectedValue) ? Guid.Empty : new Guid(DdlFiliale.SelectedValue);
            int salesScope = Convert.ToInt32(rbl_SalesScope.SelectedValue);
            int deliverType = 0;
            if (salesScope == (int)Enum.Overseas.SalesScopeType.Overseas)
                deliverType = Convert.ToInt32(rbl_DeliverType.SelectedValue);
            if (filialeId != Guid.Empty)
            {
                var existCompanyId = _companyCussentManager.GetCompanyIdByRelevanceFilialeId(filialeId);
                if (existCompanyId != Guid.Empty)
                {
                    RAM.Alert("关联公司已绑定其它的往来单位");
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(companyName) && !_companyCussentManager.IsBeing(companyName))
            {
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if ((CompanyType)companyType == CompanyType.MemberGeneralLedger)
                        {
                            if (_companyCussentManager.IsUseMemberGeneralLedger())
                            {
                                RAM.Alert("会员总帐已经存在，不允许再次添加！");
                            }
                            else
                            {
                                var companyCussentInfo = new CompanyCussentInfo(CompanyId, companyClassId, companyName,
                                    linkman, address, postalCode,
                                    phone, mobile, fax, "", email, string.Empty, string.Empty, dateCreated,
                                    companyType, state, description, subjectinfo, filialeId, salesScope, deliverType)
                                {
                                    IsNeedInvoices = isNeedInvoices,
                                    PaymentDays = Convert.ToInt32(DDL_PaymentDays.SelectedValue)
                                };
                                _companyCussentManager.Insert(companyCussentInfo);
                                RelatedCompany.Instance.Remove();

                                List<CompanyInvoicePowerInfo> companyInvoicePowerInfos =
                                    _companyInvoicePowerDao.GetCompanyInvoicePowerByCompanyID(
                                        companyCussentInfo.CompanyClassId)
                                        .Where(b => b.InvoicesType == 1).ToList();
                                CompanyInvoicePowerInfo companyInvoicePowerInfo =
                                    companyInvoicePowerInfos.Count > 0
                                        ? companyInvoicePowerInfos[0]
                                        : new CompanyInvoicePowerInfo();
                                companyInvoicePowerInfo.ParentPowerID = companyInvoicePowerInfo.PowerID;
                                companyInvoicePowerInfo.PowerID = Guid.NewGuid();
                                companyInvoicePowerInfo.CompanyID = companyCussentInfo.CompanyId;
                                _companyInvoicePowerDao.InsertCompanyInvoicePower(companyInvoicePowerInfo);
                            }
                        }
                        else
                        {
                            var companyCussentInfo = new CompanyCussentInfo(CompanyId, companyClassId, companyName,
                                linkman, address, postalCode,
                                phone, mobile, fax, "", email, string.Empty, string.Empty, dateCreated, companyType,
                                state, description, subjectinfo, filialeId, salesScope, deliverType)
                            {
                                IsNeedInvoices = isNeedInvoices,
                                PaymentDays = Convert.ToInt32(DDL_PaymentDays.SelectedValue)
                            };

                            _companyCussentManager.Insert(companyCussentInfo);
                            RelatedCompany.Instance.Remove();
                            List<CompanyInvoicePowerInfo> companyInvoicePowerInfos =
                                _companyInvoicePowerDao.GetCompanyInvoicePowerByCompanyID(companyCussentInfo.CompanyClassId)
                                    .Where(b => b.InvoicesType == 1).ToList();
                            CompanyInvoicePowerInfo companyInvoicePowerInfo =
                                companyInvoicePowerInfos.Count > 0
                                    ? companyInvoicePowerInfos[0]
                                    : new CompanyInvoicePowerInfo();
                            companyInvoicePowerInfo.ParentPowerID = companyInvoicePowerInfo.PowerID;
                            companyInvoicePowerInfo.PowerID = Guid.NewGuid();
                            companyInvoicePowerInfo.CompanyID = companyCussentInfo.CompanyId;
                            _companyInvoicePowerDao.InsertCompanyInvoicePower(companyInvoicePowerInfo);
                            //新增往来单位添加往来单位对应采购默认分组
                            var info = new CompanyPurchaseGoupInfo
                            {
                                CompanyId = companyCussentInfo.CompanyId,
                                PurchaseGroupId = Guid.Empty,
                                OrderIndex = 0,
                                PurchaseGroupName = "默认"
                            };
                            string errorMsg;
                            _companyPurchaseGoupDao.Insert(info, out errorMsg);

                        }

                        if (filialeId != Guid.Empty)
                        {
                            if (dataList.Select(companyBankAccountBindInfo => _companyBankAccountBindDao.InsertCompanyBankAccountBindWithFiliale(companyBankAccountBindInfo)).Any(result => !result))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!_companyBankAccountBindDao.DeleteCompanyBankAccountBind(CompanyId)) return false;
                            if (dataList.Select(companyBankAccountBindInfo => _companyBankAccountBindDao.InsertCompanyBankAccountBind(companyBankAccountBindInfo)).Any(result => !result))
                            {
                                return false;
                            }
                        }

                        tran.Complete();
                    }
                    catch(Exception ex)
                    {
                        RAM.Alert("往来单位信息添加失败！");
                        return false;
                    }
                }
            }
            else
            {
                RAM.Alert("该单位已存在！");
                return false;
            }
            return true;
        }
        #endregion

        #region[更新]

        /// <summary> 更新
        /// </summary>
        protected bool Update()
        {
            string message;
            var dataList = GetBind(out message);
            if (message.Length > 0)
            {
                RAM.Alert(message);
                return false;
            }
            var companyClassId = new Guid(DDL_CompanyClassId.SelectedValue);
            string companyName = TB_CompanyName.Text;
            string linkman = TB_Linkman.Text;
            string address = TB_Address.Text;
            string postalCode = TB_PostalCode.Text;
            string phone = TB_Phone.Text;
            string mobile = TB_Mobile.Text;
            string fax = TB_Fax.Text;
            string email = TB_Email.Text;
            bool isNeedInvoices = CB_IsNeedInvoices.Checked;
            DateTime dateCreated = WebControl.GetNowTime();
            int companyType = Convert.ToInt32(DDL_CompanyType.SelectedValue);
            int state = Convert.ToInt32(RBL_State.SelectedValue);
            Guid filialeId = string.IsNullOrEmpty(DdlFiliale.SelectedValue) ? Guid.Empty : new Guid(DdlFiliale.SelectedValue);
            string description = TB_Description.Text;
            string subjectinfo = string.Empty;
            int salesScope = Convert.ToInt32(rbl_SalesScope.SelectedValue);
            int deliverType = 0;
            if (salesScope == (int)Enum.Overseas.SalesScopeType.Overseas) { }
                deliverType = Convert.ToInt32(rbl_DeliverType.SelectedValue);
            CompanyCussentInfo oldCompanyCussent = _companyCussentManager.GetCompanyCussent(CompanyId);
            if (oldCompanyCussent == null)
            {
                RAM.Alert("不是有效的往来单位信息");
                return false;
            }
            if (filialeId != Guid.Empty && filialeId != oldCompanyCussent.RelevanceFilialeId)
            {
                var existCompanyId = _companyCussentManager.GetCompanyIdByRelevanceFilialeId(filialeId);
                if (existCompanyId != Guid.Empty && existCompanyId != CompanyId)
                {
                    RAM.Alert("关联公司已绑定其它的往来单位");
                    return false;
                }
            }
            if (oldCompanyCussent.SalesScope==(int)Enum.Overseas.SalesScopeType.Overseas && salesScope!= oldCompanyCussent.SalesScope)
            {
                if (_companyCussentRelation.IsExist(CompanyId))
                {
                    RAM.Alert("该往来单位已设置境外登录权限,请先删除对应权限再尝试更新！");
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(companyName) && (!_companyCussentManager.IsBeing(companyName) || oldCompanyCussent.CompanyName == companyName))
            {
                using (var tran = new TransactionScope())
                {
                    if ((CompanyType)companyType == CompanyType.MemberGeneralLedger)
                    {
                        CompanyCussentInfo memberGeneralLedger = _companyCussentManager.GetMemberGeneralLedger();
                        if (memberGeneralLedger.CompanyId != CompanyId)
                        {
                            RAM.Alert("会员总帐已经存在，不允许再次添加！");
                        }
                        else
                        {
                            var companyCussentInfo = new CompanyCussentInfo(CompanyId, companyClassId, companyName,
                                linkman, address, postalCode, phone,
                                mobile, fax, "", email, string.Empty, string.Empty, dateCreated, companyType, state,
                                description, subjectinfo, filialeId, salesScope, deliverType)
                            {
                                IsNeedInvoices = isNeedInvoices,
                                PaymentDays = Convert.ToInt32(DDL_PaymentDays.SelectedValue)
                            };

                            _companyCussentManager.Update(companyCussentInfo);
                            RelatedCompany.Instance.Remove();
                        }
                    }
                    else
                    {
                        var companyCussentInfo = new CompanyCussentInfo(CompanyId, companyClassId, companyName, linkman,
                            address, postalCode, phone,
                            mobile, fax, "", email, string.Empty, string.Empty, dateCreated, companyType, state,
                            description, subjectinfo, filialeId, salesScope, deliverType)
                        {
                            IsNeedInvoices = isNeedInvoices,
                            PaymentDays = Convert.ToInt32(DDL_PaymentDays.SelectedValue)
                        };

                        _companyCussentManager.Update(companyCussentInfo);
                        RelatedCompany.Instance.Remove();
                    }
                    if (filialeId != Guid.Empty)
                    {
                        if (dataList.Select(companyBankAccountBindInfo => _companyBankAccountBindDao.InsertCompanyBankAccountBindWithFiliale(companyBankAccountBindInfo)).Any(result => !result))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!_companyBankAccountBindDao.DeleteCompanyBankAccountBind(CompanyId)) return false;
                        if (dataList.Select(companyBankAccountBindInfo => _companyBankAccountBindDao.InsertCompanyBankAccountBind(companyBankAccountBindInfo)).Any(result => !result))
                        {
                            return false;
                        }
                    }
                    tran.Complete();
                }
            }
            else
            {
                RAM.Alert("该单位已存在！");
                return false;
            }
            return true;
        }

        #endregion

        #region[绑定单位类型]
        /// <summary>
        /// 绑定单位类型
        /// </summary>
        protected void GetCompanyClass()
        {
            IList<CompanyClassInfo> companyClassList = WebControl.RecursionCompanyClass(Guid.Empty, 0);
            companyClassList.Insert(0, new CompanyClassInfo());
            DDL_CompanyClassId.DataTextField = "CompanyClassName";
            DDL_CompanyClassId.DataValueField = "CompanyClassId";
            DDL_CompanyClassId.DataSource = companyClassList;
            DDL_CompanyClassId.DataBind();
        }

        protected void BindFiliale(List<FilialeInfo> filialeList, Guid filialeId)
        {
            var filialeIds = _companyCussentManager.GetRelevanceFilialeIdList();
            var salePlatforms = CacheCollection.SalePlatform.GetList();
            filialeList.AddRange(salePlatforms.Where(ent=>ent.IsActive).Select(ent=>new FilialeInfo {ID = ent.ID,Name = ent.Name,ParentId = ent.FilialeId}));
            List<FilialeInfo> filialeInfos = new List<FilialeInfo> { new FilialeInfo { ID = Guid.Empty, Name = "--请选择--" } };
            ChildAndParent = filialeList.ToDictionary(k => k.ID, v => v.ParentId);
            if (filialeIds != null && filialeIds.Any())
            {
                if (filialeId != Guid.Empty)
                {
                    filialeIds = filialeIds.Where(ent => ent != filialeId);
                }
                filialeInfos.AddRange(filialeList.Where(ent => !filialeIds.Contains(ent.ID)));
            }
            else
            {
                filialeInfos.AddRange(filialeList);
            }
            filialeInfos.AddRange(salePlatforms.Where(ent => !filialeIds.Contains(ent.ID)).Select(ent=>new FilialeInfo {ID = ent.ID,Name = ent.Name}));
            DdlFiliale.DataTextField = "Name";
            DdlFiliale.DataValueField = "ID";
            DdlFiliale.DataSource = filialeInfos;
            DdlFiliale.DataBind();
        }
        #endregion

        /// <summary>
        /// 获取银行绑定的信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IList<CompanyBankAccountBindInfo> GetBind(out string message)
        {
            var list = new List<CompanyBankAccountBindInfo>();
            message = "";
            Guid relevanceFilialeId = string.IsNullOrEmpty(DdlFiliale.SelectedValue)
                    ? Guid.Empty
                    : new Guid(DdlFiliale.SelectedValue);
            if (relevanceFilialeId != Guid.Empty)
            {
                foreach (RepeaterItem item in RepeaterNext.Items)
                {
                    if (message.Length > 0) continue;
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        var companyId = (HiddenField)item.FindControl("HfCompanyId");
                        var filialeId = (HiddenField)item.FindControl("HfFilialeId");
                        if (companyId == null || filialeId == null) continue;

                        var filialeName = (HiddenField)item.FindControl("HfFilialeName");
                        var bank1 = (DropDownList)item.FindControl("DdlBankId");

                        if (!string.IsNullOrEmpty(bank1.SelectedValue) && bank1.SelectedValue != Guid.Empty.ToString())
                        {
                            list.Add(new CompanyBankAccountBindInfo
                            {
                                CompanyId = new Guid(companyId.Value),
                                FilialeId = new Guid(filialeId.Value),
                                AccountsNumber = "",
                                BankAccounts = "",
                                WebSite = "",
                                BankAccountsId = new Guid(bank1.SelectedValue)
                            });
                        }
                    }
                }
            }
            else
            {
                foreach (RepeaterItem item in RepeartBank.Items)
                {
                    if (message.Length > 0) continue;
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        var companyId = (HiddenField)item.FindControl("HfCompanyId");
                        var filialeId = (HiddenField)item.FindControl("HfFilialeId");
                        if (companyId == null || filialeId == null) continue;

                        var filialeName = (HiddenField)item.FindControl("HfFilialeName");
                        var bankAccounts = (TextBox)item.FindControl("TbBankAccounts");
                        var accountsNumber = (TextBox)item.FindControl("TbAccountNo");
                        var webSite = (TextBox)item.FindControl("TbWebSite");

                        if (string.IsNullOrEmpty(bankAccounts.Text) && string.IsNullOrEmpty(accountsNumber.Text) && string.IsNullOrEmpty(webSite.Text)) continue;
                        if ((string.IsNullOrEmpty(bankAccounts.Text) || string.IsNullOrEmpty(accountsNumber.Text) || string.IsNullOrEmpty(webSite.Text)))
                        {
                            if (string.IsNullOrEmpty(webSite.Text))
                            {
                                message = string.Format("公司：{0}-收款人为空", filialeName != null ? filialeName.Value : "");
                                continue;
                            }
                            if (string.IsNullOrEmpty(bankAccounts.Text))
                            {
                                message = string.Format("公司：{0}-开户银行为空", filialeName != null ? filialeName.Value : "");
                                continue;
                            }
                            message = string.Format("公司：{0}-银行帐号为空", filialeName != null ? filialeName.Value : "");
                            continue;
                        }
                        list.Add(new CompanyBankAccountBindInfo
                        {
                            CompanyId = new Guid(companyId.Value),
                            FilialeId = new Guid(filialeId.Value),
                            AccountsNumber = accountsNumber.Text,
                            BankAccounts = bankAccounts.Text,
                            WebSite = webSite.Text
                        });
                    }
                }
            }
            if (list.Count == 0) message = "对方收款帐号信息为空!";
            return list;
        }

        protected void DdlFilialeSelectedChanged(object sender, EventArgs e)
        {
            var obj = (DropDownList)sender;
            var dataSource = CacheCollection.Filiale.GetHostingAndSaleFilialeList();
            var isFiliale = obj != null && obj.SelectedValue != Guid.Empty.ToString();
            if (isFiliale)
            {
                dataSource = dataSource.Where(ent => ent.ID != new Guid(obj.SelectedValue)).ToList();
            }
            var dataList = dataSource.Select(filialeInfo => new CompanyBankAccountBindInfo
            {
                FilialeId = filialeInfo.ID,
                FilialeName = filialeInfo.Name,
                CompanyId = CompanyId
            }).ToList();
            if (obj != null && obj.SelectedValue != Guid.Empty.ToString())
            {
                RepeaterNext.Visible = true;
                RepeartBank.Visible = false;
                RepeaterNext.DataSource = dataList;
                RepeaterNext.DataBind();
            }
            else
            {
                RepeaterNext.Visible = false;
                RepeartBank.Visible = true;
                RepeartBank.DataSource = dataList;
                RepeartBank.DataBind();

            }
        }

        protected void DdlCompanyTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            var salesScope = rbl_SalesScope.SelectedValue;
            if (salesScope==string.Format("{0}",(int)Enum.Overseas.SalesScopeType.Overseas))
            {
                if (DDL_CompanyType.SelectedValue == string.Format("{0}",(int)CompanyType.Express))
                {
                    rbl_DeliverType.SelectedValue = string.Format("{0}", (int)Enum.Overseas.DeliverType.Transfer);
                    rbl_DeliverType.Enabled = false;
                }
                else
                {
                    rbl_DeliverType.SelectedValue = string.Format("{0}", (int)Enum.Overseas.DeliverType.Direct);
                    rbl_DeliverType.Enabled = true;
                }
            }
            else
            {
                rbl_DeliverType.Enabled = true;
            }
        }

        protected void RepeartBankItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var companyId = (HiddenField)e.Item.FindControl("HfCompanyId");
                var filialeId = (HiddenField)e.Item.FindControl("HfFilialeId");
                if (companyId == null || filialeId == null) return;
                var webSite = (TextBox)e.Item.FindControl("TbWebSite");
                var bankAccounts = (TextBox)e.Item.FindControl("TbBankAccounts");
                var accountNo = (TextBox)e.Item.FindControl("TbAccountNo");
                var bindBankAccounts = _companyBankAccountBind.GetCompanyBankAccountBindInfo(new Guid(companyId.Value), new Guid(filialeId.Value));
                if (bindBankAccounts != null)
                {
                    if (webSite != null)
                        webSite.Text = bindBankAccounts.WebSite;
                    if (bankAccounts != null)
                        bankAccounts.Text = bindBankAccounts.BankAccounts;
                    if (accountNo != null)
                        accountNo.Text = bindBankAccounts.AccountsNumber;
                }
            }
        }

        protected void RepeartBankItemDataBoundNext(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var companyId = (HiddenField)e.Item.FindControl("HfCompanyId");
                var filialeId = string.IsNullOrEmpty(DdlFiliale.SelectedValue) ? Guid.Empty : new Guid(DdlFiliale.SelectedValue);
                if (companyId == null) return;
                var bank1 = (DropDownList)e.Item.FindControl("DdlBankId");

                bank1.Items.Clear();
                bank1.Text = "";
                bank1.Items.Add(new ListItem("--请选择--", Guid.Empty.ToString()));
                var bankAccountList = _bankAccounts.GetListByTargetId(filialeId);
                foreach (var bankAcount in bankAccountList)
                {
                    bank1.Items.Add(new ListItem(string.Format("[{0}]{1}", bankAcount.AccountsName, bankAcount.Accounts), string.Format("{0}", bankAcount.BankAccountsId)));
                }
                var bindBankAccounts = _companyBankAccountBind.GetCompanyBankAccountIdBind(new Guid(companyId.Value), filialeId);
                bank1.SelectedValue = string.Format("{0}", bindBankAccounts?.BankAccountsId ?? Guid.Empty);
            }
        }

        protected void RblSalesScopeSelectedIndexChanged(object sender, EventArgs e)
        {
            var salesScope = rbl_SalesScope.SelectedValue;
            if (salesScope == string.Format("{0}", (int)Enum.Overseas.SalesScopeType.Overseas))
            {
                if (DDL_CompanyType.SelectedValue == string.Format("{0}", (int)CompanyType.Express))
                {
                    rbl_DeliverType.SelectedValue = string.Format("{0}", (int)Enum.Overseas.DeliverType.Transfer);
                    rbl_DeliverType.Enabled = false;
                }
                else
                {
                    rbl_DeliverType.Enabled = true;
                }
            }
            else
            {
                rbl_DeliverType.Enabled = true;
            }
        }
    }
}
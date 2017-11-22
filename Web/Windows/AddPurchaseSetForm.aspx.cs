using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.WMS;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class AddPurchaseSetForm : WindowsPage
    {
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);
        private readonly IPurchaseSetLog _purchaseSetLog = new PurchaseSetLog(GlobalConfig.DB.FromType.Write);
        private readonly CompanyPurchaseGoupDao _companyPurchaseGoupDao = new CompanyPurchaseGoupDao(GlobalConfig.DB.FromType.Write);
        private readonly IPurchasePromotion _purchasePromotion = new PurchasePromotion(GlobalConfig.DB.FromType.Write);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);

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

        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                //绑定仓库
                RCB_Warehouse.DataSource = WMSSao.GetAllCanUseWarehouseDics();
                RCB_Warehouse.DataBind();
                var personinfo = CurrentSession.Personnel.Get();
                WarehouseAuths = WMSSao.GetWarehouseAndFilialeAuth(personinfo.PersonnelId);
                BindFilile();
                //绑定责任人
                RCB_Persion.DataSource = PersonnelList;
                RCB_Persion.DataBind();
                //绑定供应商
                RCB_Company.DataSource = CompanyCussentList;
                RCB_Company.DataBind();
                if (GoodsId == Guid.Empty)
                {
                    //新建
                    btnSave.Visible = true;
                    btnEdit.Visible = false;
                    RCB_PurchaseGroup.Items.Add(new RadComboBoxItem("默认", Guid.Empty.ToString()));
                }
                else
                {
                    //编辑
                    btnSave.Visible = false;
                    btnEdit.Visible = true;
                    BtnAddGoods.Enabled = false;
                    BtnRemoveGoods.Enabled = false;
                    var hostingFilialeId = new Guid(Request.QueryString["HostingFilialeId"]);
                    PurchaseSetInfo purchaseSetInfo = _purchaseSet.GetPurchaseSetInfo(GoodsId, hostingFilialeId, WarehouseId);
                    OldPurchaseSetInfo = purchaseSetInfo;

                    if (purchaseSetInfo != null)
                    {
                        //采购分组
                        var purchaseGoupList = _companyPurchaseGoupDao.GetCompanyPurchaseGoupList(purchaseSetInfo.CompanyId);
                        if (purchaseGoupList.Count == 0)
                        {
                            RCB_PurchaseGroup.Items.Add(new RadComboBoxItem("默认", Guid.Empty.ToString()));
                        }
                        else
                        {
                            foreach (var info in purchaseGoupList)
                            {
                                RCB_PurchaseGroup.Items.Add(new RadComboBoxItem(info.PurchaseGroupName, info.PurchaseGroupId.ToString()));
                            }
                        }
                        SetGoodsList.Items.Add(new RadListBoxItem(purchaseSetInfo.GoodsName, string.Format("{0}", purchaseSetInfo.GoodsId)));
                        var purchaseSetLogInfo = _purchaseSetLog.GetPurchaseSetLogList(purchaseSetInfo.GoodsId, purchaseSetInfo.WarehouseId, hostingFilialeId).Where(p => p.Statue == (int)PurchaseSetLogStatue.NotAudit).ToList();
                        if (purchaseSetLogInfo.Count > 0)
                        {
                            foreach (var setLogInfo in purchaseSetLogInfo)
                            {
                                if (setLogInfo.LogType == (int)PurchaseSetLogType.PurchasePrice)
                                {
                                    tbPurchasePrice.Enabled = false;
                                }
                            }
                        }
                        RCB_Goods.Items.Add(new RadComboBoxItem(purchaseSetInfo.GoodsName, purchaseSetInfo.GoodsId.ToString()));
                        RCB_Goods.SelectedValue = purchaseSetInfo.GoodsId.ToString();
                        RCB_Warehouse.SelectedValue = purchaseSetInfo.WarehouseId.ToString();
                        BindFilile();
                        RCB_Filile.SelectedValue = purchaseSetInfo.HostingFilialeId.ToString();
                        RCB_Company.SelectedValue = purchaseSetInfo.CompanyId.ToString();
                        tbPurchasePrice.Text = purchaseSetInfo.PurchasePrice.ToString(CultureInfo.InvariantCulture);
                        HidPurchasePrice.Value = purchaseSetInfo.PurchasePrice.ToString(CultureInfo.InvariantCulture);
                        RCB_Persion.SelectedValue = purchaseSetInfo.PersonResponsible.ToString();
                        tbMemo.Text = purchaseSetInfo.Memo;
                        CB_IsStockUp.Checked = purchaseSetInfo.IsStockUp;
                        RCB_PurchaseGroup.SelectedValue = purchaseSetInfo.PurchaseGroupId.ToString();
                        RCB_FilingForm.SelectedValue = string.Format("{0}", purchaseSetInfo.FilingForm);
                        RCB_StockUpDay.SelectedValue = string.Format("{0}", purchaseSetInfo.StockUpDay);
                        if (purchaseSetInfo.FilingForm == 1 || purchaseSetInfo.FilingForm == 0)//等于0 ，发布前新建字段数据为0，默认显示有问题。故==0。
                        {
                            divOne.Visible = true;
                            divTwo.Visible = false;
                        }
                        else
                        {
                            divOne.Visible = false;
                            divTwo.Visible = true;
                            tbFilingTrigger.Text = string.Format("{0}", purchaseSetInfo.FilingTrigger);
                            tbInsufficient.Text = string.Format("{0}", purchaseSetInfo.Insufficient);
                        }
                        tbFirstWeek.Text = string.Format("{0}", purchaseSetInfo.FirstWeek);
                        tbSecondWeek.Text = string.Format("{0}", purchaseSetInfo.SecondWeek);
                        tbThirdWeek.Text = string.Format("{0}", purchaseSetInfo.ThirdWeek);
                        tbFourthWeek.Text = string.Format("{0}", purchaseSetInfo.FourthWeek);
                        IList<PurchasePromotionInfo> list = _purchasePromotion.GetPurchasePromotionListByGoodsId(purchaseSetInfo.GoodsId, purchaseSetInfo.HostingFilialeId, WarehouseId);
                        PurchasePromotionList = list.OrderByDescending(w => w.StartDate).ToList();
                        RCB_Goods.Enabled = false;
                        RCB_Warehouse.Enabled = false;
                        RCB_Filile.Enabled = false;
                        Rp_PurchasePromotion.DataBind();
                    }
                }
            }
        }

        protected void RCB_InStock_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindFilile();
        }

        #region 分公司和仓库绑定

        private void BindFilile()
        {
            RCB_Filile.Items.Clear();
            RCB_Filile.Text = "";
            List<HostingFilialeAuth> fl = new List<HostingFilialeAuth>();
            var warehouse = RCB_Warehouse.SelectedValue;
            if (!string.IsNullOrEmpty(warehouse) && warehouse != Guid.Empty.ToString())
            {
                var selectWarehouse = WarehouseAuths.FirstOrDefault(act => act.WarehouseId == new Guid(warehouse));
                if (selectWarehouse != null)
                {
                    fl.AddRange(selectWarehouse.FilialeAuths);
                }
            }
            RCB_Filile.DataSource = fl;
            RCB_Filile.DataTextField = "HostingFilialeName";
            RCB_Filile.DataValueField = "HostingFilialeId";
            RCB_Filile.DataBind();
        }

        #endregion

        #region [自定义]

        protected Guid GoodsId
        {
            get
            {
                if (ViewState["GoodsId"] == null)
                {
                    var id = Guid.Empty;
                    if (Request.QueryString["GoodsId"] == null || Request.QueryString["GoodsId"].Trim() == "")
                    {

                    }
                    else
                    {
                        id = new Guid(Request.QueryString["GoodsId"]);
                    }
                    ViewState["GoodsId"] = id;
                }
                return new Guid(ViewState["GoodsId"].ToString());
            }
        }

        protected Guid WarehouseId
        {
            get
            {
                if (ViewState["WarehouseId"] == null)
                {
                    Guid id = Guid.Empty;
                    if (Request.QueryString["WarehouseId"] == null || Request.QueryString["WarehouseId"].Trim() == "")
                    {

                    }
                    else
                    {
                        id = new Guid(Request.QueryString["WarehouseId"]);
                    }
                    ViewState["WarehouseId"] = id;
                }
                return new Guid(ViewState["WarehouseId"].ToString());
            }
        }

        protected PurchaseSetInfo OldPurchaseSetInfo
        {
            get
            {
                if (ViewState["OldPurchaseSetInfo"] == null)
                {
                    return null;
                }
                return (PurchaseSetInfo)ViewState["OldPurchaseSetInfo"];
            }
            set { ViewState["OldPurchaseSetInfo"] = value; }
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
                    ViewState["CompanyCussentList"] = _companyCussent.GetCompanyCussentList(CompanyType.Suppliers).Where(ent => ent.State == 1).ToList();
                }
                return (IList<CompanyCussentInfo>)ViewState["CompanyCussentList"];
            }
        }

        protected List<WarehouseFilialeAuth> WarehouseAuths
        {
            get
            {
                if (ViewState["WarehouseAuths"] == null)
                {
                    return new List<WarehouseFilialeAuth>();
                }
                return (List<WarehouseFilialeAuth>)ViewState["WarehouseAuths"];
            }
            set { ViewState["WarehouseAuths"] = value; }
        }

        /// <summary>
        /// 采购促销明细
        /// </summary>
        public IList<PurchasePromotionInfo> PurchasePromotionList
        {
            get
            {
                if (ViewState["PurchasePromotionList"] == null)
                {
                    ViewState["PurchasePromotionList"] = new List<PurchasePromotionInfo>();
                }
                return (IList<PurchasePromotionInfo>)ViewState["PurchasePromotionList"];
            }
            set { ViewState["PurchasePromotionList"] = value; }
        }

        #endregion

        protected void RCB_Goods_OnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var dic = _goodsCenterSao.GetGoodsSelectList(e.Text);
                Int32 totalCount = dic.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var keyValuePair in dic)
                    {
                        var item = new RadComboBoxItem
                        {
                            Text = keyValuePair.Value,
                            Value = keyValuePair.Key
                        };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        protected void RCB_Company_OnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text;
                IList<CompanyCussentInfo> companyList =
                    CompanyCussentList.Where(p => p.CompanyName.IndexOf(key, StringComparison.Ordinal) > -1).ToList();
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

        protected void RCB_Company_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var strCompanyId = RCB_Company.SelectedValue;
            RCB_PurchaseGroup.Items.Clear();
            if (!string.IsNullOrEmpty(strCompanyId))
            {
                var purchaseGoupList = _companyPurchaseGoupDao.GetCompanyPurchaseGoupList(new Guid(strCompanyId));
                if (purchaseGoupList.Count == 0)
                {
                    RCB_PurchaseGroup.Items.Add(new RadComboBoxItem("默认", Guid.Empty.ToString()));
                }
                else
                {
                    foreach (var info in purchaseGoupList)
                    {
                        RCB_PurchaseGroup.Items.Add(new RadComboBoxItem(info.PurchaseGroupName, info.PurchaseGroupId.ToString()));
                    }
                }
            }
            else
            {
                RCB_PurchaseGroup.Items.Add(new RadComboBoxItem("默认", Guid.Empty.ToString()));
            }
        }

        protected void RCB_FilingForm_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var strFilingForm = RCB_FilingForm.SelectedValue;
            if (strFilingForm == "1")
            {
                divOne.Visible = true;
                divTwo.Visible = false;
            }
            else
            {
                divOne.Visible = false;
                divTwo.Visible = true;
            }
        }

        // 添加一行促销信息
        protected void BtnAddPromotion_Click(object sender, EventArgs eventArgs)
        {
            IList<PurchasePromotionInfo> list = new List<PurchasePromotionInfo>();
            foreach (RepeaterItem dataItem in Rp_PurchasePromotion.Items)
            {
                var info = new PurchasePromotionInfo();
                //现返
                var rbtnPromotionType1 = (RadioButton)dataItem.FindControl("rbtnPromotionType1");
                //非现返
                var rbtnPromotionType2 = (RadioButton)dataItem.FindControl("rbtnPromotionType2");
                var cbPromotionKind = (CheckBox)dataItem.FindControl("cbPromotionKind");
                var tbBuy = (TextBox)dataItem.FindControl("tbBuy");
                var tbGive = (TextBox)dataItem.FindControl("tbGive");
                var cbPromotion = (CheckBox)dataItem.FindControl("cbPromotion");
                var tbPromotionInfo = (TextBox)dataItem.FindControl("tbPromotionInfo");
                var rdpStartDate = (RadDatePicker)dataItem.FindControl("RDP_StartDate");
                var rdpEndDate = (RadDatePicker)dataItem.FindControl("RDP_EndDate");
                var ckIsSingle = (CheckBox)dataItem.FindControl("CkIsSingle");

                var hfGoodsId = (HiddenField)dataItem.FindControl("hfGoodsId");
                var goodsId = new Guid(hfGoodsId.Value);

                info.GoodsId = goodsId;
                //未选择
                info.PromotionType = (int)PurchasePromotionType.None;
                if (rbtnPromotionType1.Checked)
                {
                    //现返
                    info.PromotionType = (int)PurchasePromotionType.Back;
                }
                if (rbtnPromotionType2.Checked)
                {
                    //非现返
                    info.PromotionType = (int)PurchasePromotionType.NoBack;
                }
                if (cbPromotionKind.Checked && cbPromotion.Checked)
                {
                    //买几赠几和促销信息
                    info.PromotionKind = (int)PromotionKind.Both;
                }
                else if (cbPromotionKind.Checked)
                {
                    //买几赠几
                    info.PromotionKind = (int)PromotionKind.BuyGive;
                }
                else if (cbPromotion.Checked)
                {
                    //促销信息
                    info.PromotionKind = (int)PromotionKind.PromotionInfo;
                }
                else
                {
                    //无
                    info.PromotionKind = (int)PromotionKind.None;
                }
                info.IsSingle = ckIsSingle.Checked;
                info.BuyCount = string.IsNullOrEmpty(tbBuy.Text) ? 0 : int.Parse(tbBuy.Text);
                info.GivingCount = string.IsNullOrEmpty(tbGive.Text) ? 0 : int.Parse(tbGive.Text);
                info.PromotionInfo = tbPromotionInfo.Text;
                info.StartDate = rdpStartDate.SelectedDate ?? DateTime.MinValue;
                info.EndDate = rdpEndDate.SelectedDate ?? DateTime.MinValue;

                list.Add(info);
            }

            PurchasePromotionList = list.OrderByDescending(w => w.StartDate).ToList();
            var newInfo = new PurchasePromotionInfo { GoodsId = Guid.NewGuid(), StartDate = DateTime.MinValue, EndDate = DateTime.MinValue, PromotionId = Guid.NewGuid() };
            PurchasePromotionList.Add(newInfo);
            Rp_PurchasePromotion.DataBind();
        }

        protected void RpPurchasePromotion_OnDataBinding(object sender, EventArgs e)
        {
            Rp_PurchasePromotion.DataSource = PurchasePromotionList;
        }

        protected void Rp_PurchasePromotion_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var rdpStartDate = (RadDatePicker)e.Item.FindControl("RDP_StartDate");
            var rdpEndDate = (RadDatePicker)e.Item.FindControl("RDP_EndDate");
            var hfStartDate = (HiddenField)e.Item.FindControl("hfStartDate");
            var hfEndDate = (HiddenField)e.Item.FindControl("hfEndDate");

            var startDate = DateTime.Parse(hfStartDate.Value);
            var endDate = DateTime.Parse(hfEndDate.Value);

            if (startDate != DateTime.MinValue)
            {
                rdpStartDate.SelectedDate = startDate;
            }
            if (endDate != DateTime.MinValue)
            {
                rdpEndDate.SelectedDate = endDate;
            }
        }

        protected void RpPurchasePromotion_OnItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeletePurchasePromotion")
            {
                //var hfGoodsId = (HiddenField)e.Item.FindControl("hfGoodsId");
                var hfPromotionId = (HiddenField)e.Item.FindControl("HfPromotionid");
                var promotionId = hfPromotionId.Value;
                IList<PurchasePromotionInfo> list = PurchasePromotionList.Where(w => w.PromotionId != new Guid(promotionId)).ToList();
                PurchasePromotionList = list;
                Rp_PurchasePromotion.DataBind();
            }
        }

        /// <summary> 获取 填写 商品采购设置
        /// </summary>
        /// <returns></returns>
        private IList<PurchaseSetInfo> GetPurchaseSetInfo()
        {
            var purchaseSetList = new List<PurchaseSetInfo>();
            foreach (RadListBoxItem item in SetGoodsList.Items)
            {
                var purchaseSetInfo = new PurchaseSetInfo();
                if (!string.IsNullOrEmpty(item.Value))
                {
                    purchaseSetInfo.GoodsId = new Guid(item.Value);
                    purchaseSetInfo.GoodsName = item.Text;
                }
                else
                {
                    purchaseSetInfo.GoodsId = Guid.Empty;
                }
                purchaseSetInfo.WarehouseId = (!string.IsNullOrEmpty(RCB_Warehouse.SelectedValue)) ? new Guid(RCB_Warehouse.SelectedValue) : Guid.Empty;
                purchaseSetInfo.HostingFilialeId = (!string.IsNullOrEmpty(RCB_Filile.SelectedValue)) ? new Guid(RCB_Filile.SelectedValue) : Guid.Empty;
                purchaseSetInfo.CompanyId = (!string.IsNullOrEmpty(RCB_Company.SelectedValue)) ? new Guid(RCB_Company.SelectedValue) : Guid.Empty;
                purchaseSetInfo.PurchasePrice = decimal.Parse(tbPurchasePrice.Text);
                var strPerson = RCB_Persion.SelectedValue;
                purchaseSetInfo.PersonResponsible = string.IsNullOrEmpty(strPerson) ? Guid.Empty : new Guid(strPerson);
                purchaseSetInfo.Memo = tbMemo.Text;
                purchaseSetInfo.IsStockUp = CB_IsStockUp.Checked;

                purchaseSetInfo.PurchaseGroupId = new Guid(RCB_PurchaseGroup.SelectedValue);
                purchaseSetInfo.FilingForm = int.Parse(RCB_FilingForm.SelectedValue);
                purchaseSetInfo.StockUpDay = int.Parse(RCB_StockUpDay.SelectedValue);
                if (purchaseSetInfo.FilingForm == 1)
                {
                    //常规(月周期)
                    purchaseSetInfo.FirstWeek = Convert.ToInt32(tbFirstWeek.Text.Trim());
                    purchaseSetInfo.SecondWeek = Convert.ToInt32(tbSecondWeek.Text.Trim());
                    purchaseSetInfo.ThirdWeek = Convert.ToInt32(tbThirdWeek.Text.Trim());
                    purchaseSetInfo.FourthWeek = Convert.ToInt32(tbFourthWeek.Text.Trim());
                    purchaseSetInfo.FilingTrigger = 0;
                    purchaseSetInfo.Insufficient = 0;
                }
                else
                {
                    //触发报备
                    purchaseSetInfo.FirstWeek = 0;
                    purchaseSetInfo.SecondWeek = 0;
                    purchaseSetInfo.ThirdWeek = 0;
                    purchaseSetInfo.FourthWeek = 0;

                    string strFilingTrigger = tbFilingTrigger.Text;
                    string strInsufficient = tbInsufficient.Text;
                    purchaseSetInfo.FilingTrigger = string.IsNullOrEmpty(strFilingTrigger) ? 0 : int.Parse(strFilingTrigger);
                    purchaseSetInfo.Insufficient = string.IsNullOrEmpty(strInsufficient) ? 0 : int.Parse(strInsufficient);
                }
                purchaseSetList.Add(purchaseSetInfo);
            }
            return purchaseSetList;
        }

        /// <summary> 获取促销信息
        /// </summary>
        /// <returns></returns>
        private IList<PurchasePromotionInfo> GetPurchasePromotionList(out string message)
        {
            IList<PurchasePromotionInfo> purchasePromotionList = new List<PurchasePromotionInfo>();
            foreach (RepeaterItem dataItem in Rp_PurchasePromotion.Items)
            {
                var purchasePromotionInfo = new PurchasePromotionInfo();
                //现返
                var rbtnPromotionType1 = (RadioButton)dataItem.FindControl("rbtnPromotionType1");
                //非现返
                var rbtnPromotionType2 = (RadioButton)dataItem.FindControl("rbtnPromotionType2");
                var cbPromotionKind = (CheckBox)dataItem.FindControl("cbPromotionKind");
                var tbBuy = (TextBox)dataItem.FindControl("tbBuy");
                var tbGive = (TextBox)dataItem.FindControl("tbGive");
                var cbPromotion = (CheckBox)dataItem.FindControl("cbPromotion");
                var tbPromotionInfo = (TextBox)dataItem.FindControl("tbPromotionInfo");
                var rdpStartDate = (RadDatePicker)dataItem.FindControl("RDP_StartDate");
                var rdpEndDate = (RadDatePicker)dataItem.FindControl("RDP_EndDate");
                var isSingle = (CheckBox)dataItem.FindControl("CkIsSingle");

                purchasePromotionInfo.GoodsId = GoodsId;

                //未选择
                purchasePromotionInfo.PromotionType = (int)PurchasePromotionType.None;
                if (rbtnPromotionType1.Checked)
                {
                    //现返
                    purchasePromotionInfo.PromotionType = (int)PurchasePromotionType.Back;
                }
                if (rbtnPromotionType2.Checked)
                {
                    //非现返
                    purchasePromotionInfo.PromotionType = (int)PurchasePromotionType.NoBack;
                }

                if (cbPromotionKind.Checked && cbPromotion.Checked)
                {
                    //买几赠几和促销信息
                    purchasePromotionInfo.PromotionKind = (int)PromotionKind.Both;
                }
                else if (cbPromotionKind.Checked)
                {
                    //买几赠几
                    purchasePromotionInfo.PromotionKind = (int)PromotionKind.BuyGive;
                }
                else if (cbPromotion.Checked)
                {
                    //促销信息
                    purchasePromotionInfo.PromotionKind = (int)PromotionKind.PromotionInfo;
                }
                else
                {
                    //无
                    purchasePromotionInfo.PromotionKind = (int)PromotionKind.None;
                }

                if (cbPromotionKind.Checked)
                {
                    purchasePromotionInfo.BuyCount = string.IsNullOrEmpty(tbBuy.Text) ? 0 : int.Parse(tbBuy.Text);
                    purchasePromotionInfo.GivingCount = string.IsNullOrEmpty(tbGive.Text) ? 0 : int.Parse(tbGive.Text);
                }
                else
                {
                    purchasePromotionInfo.BuyCount = 0;
                    purchasePromotionInfo.GivingCount = 0;
                }
                purchasePromotionInfo.IsSingle = isSingle.Checked;
                purchasePromotionInfo.PromotionInfo = cbPromotion.Checked ? tbPromotionInfo.Text : "";
                purchasePromotionInfo.StartDate = rdpStartDate.SelectedDate ?? DateTime.MinValue;
                purchasePromotionInfo.EndDate = rdpEndDate.SelectedDate == null ? DateTime.MinValue : DateTime.Parse(rdpEndDate.SelectedDate.Value.ToString("yyyy-MM-dd 23:59:59"));
                if (!purchasePromotionList.All(act => act.EndDate < purchasePromotionInfo.StartDate
                    || act.StartDate > purchasePromotionInfo.EndDate))
                {
                    message = "促销列表中存在促销日期冲突!";
                    return null;
                }
                purchasePromotionInfo.PromotionId = Guid.NewGuid();
                if (purchasePromotionInfo.PromotionType != (int)PurchasePromotionType.None && purchasePromotionInfo.PromotionKind != (int)PromotionKind.None)
                {
                    purchasePromotionList.Add(purchasePromotionInfo);
                }
            }
            message = string.Empty;
            return purchasePromotionList;
        }

        /// <summary> 检验
        /// </summary>
        /// <param name="purchaseSetInfo"></param>
        /// <param name="purchasePromotionList"></param>
        /// <returns></returns>
        private bool IsCheck(PurchaseSetInfo purchaseSetInfo, IList<PurchasePromotionInfo> purchasePromotionList)
        {
            if (purchaseSetInfo.GoodsId == Guid.Empty)
            {
                RAM.Alert("请选择商品");
                return false;
            }
            if (purchaseSetInfo.WarehouseId == Guid.Empty)
            {
                RAM.Alert("请选择仓库");
                return false;
            }
            if (purchaseSetInfo.HostingFilialeId == Guid.Empty)
            {
                RAM.Alert("请选择物流公司");
                return false;
            }
            if (purchaseSetInfo.CompanyId == Guid.Empty)
            {
                RAM.Alert("请选择供应商");
                return false;
            }
            if (purchaseSetInfo.PersonResponsible == Guid.Empty)
            {
                RAM.Alert("请选择责任人");
                return false;
            }
            if (purchaseSetInfo.FilingForm == 2)
            {
                //触发报备
                if (purchaseSetInfo.FilingTrigger == 0)
                {
                    RAM.Alert("请填写“触发时报备”");
                    return false;
                }
                if (purchaseSetInfo.FilingTrigger == 0)
                {
                    RAM.Alert("请填写“不足”");
                    return false;
                }
            }
            if (btnSave.Visible)
            {
                //添加
                if (_purchaseSet.IsExist(purchaseSetInfo.WarehouseId, purchaseSetInfo.HostingFilialeId, purchaseSetInfo.GoodsId))
                {
                    RAM.Alert("商品：" + purchaseSetInfo.GoodsName + " 同仓库、同物流公司、同商品不允许重复添加");
                    return false;
                }
            }
            if (purchasePromotionList.Count > 0)
            {
                purchasePromotionList = purchasePromotionList.OrderBy(w => w.StartDate).ToList();
                var newPurchasePromotionList = (IList<PurchasePromotionInfo>)purchasePromotionList.DeepCopy();
                foreach (var info in purchasePromotionList)
                {
                    if (info.PromotionKind == (int)PromotionKind.BuyGive || info.PromotionKind == (int)PromotionKind.Both)
                    {
                        if (info.BuyCount == 0)
                        {
                            RAM.Alert("有买几个赠几个的“买”不能为零");
                            return false;
                        }
                    }
                    if (info.PromotionKind == (int)PromotionKind.PromotionInfo || info.PromotionKind == (int)PromotionKind.Both)
                    {
                        if (string.IsNullOrEmpty(info.PromotionInfo))
                        {
                            RAM.Alert("有促销信息的内容不能为空");
                            return false;
                        }
                    }
                    if (info.StartDate == DateTime.MinValue || info.EndDate == DateTime.MinValue)
                    {
                        RAM.Alert("时间起止不能为空，请选择");
                        return false;
                    }
                    if (info.StartDate > info.EndDate)
                    {
                        RAM.Alert("开始时间不能大于结束时间");
                        return false;
                    }
                    if (newPurchasePromotionList.Count(w => w.GoodsId != info.GoodsId && w.StartDate < info.EndDate) > 0)
                    {
                        RAM.Alert("起止时间段不允许有交集");
                        return false;
                    }
                    newPurchasePromotionList.RemoveAt(0);
                }
            }
            return true;
        }

        protected bool CheckPrice()
        {
            const string PATTERN = @"^\d+(\.\d{1,4})?$";
            if (!Regex.IsMatch(tbPurchasePrice.Text, PATTERN))
            {
                RAM.Alert("采购价格式不正确");
                return false;
            }
            return true;
        }

        #region[添加]
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            if (!SubmitController.Enabled)
            {
                RAM.Alert("程序正在处理");
                return;
            }
            if (!CheckPrice()) return;
            if (SetGoodsList.Items.Count == 0)
            {
                RAM.Alert("请添加设置商品!");
                return;
            }
            string message;
            var purchaseSetList = GetPurchaseSetInfo();
            if (purchaseSetList == null || purchaseSetList.Count == 0)
                return;
            IList<PurchasePromotionInfo> purchasePromotionList = GetPurchasePromotionList(out message);
            if (message.Length > 0)
            {
                RAM.Alert(message);
                return;
            }
            var personnelInfo = CurrentSession.Personnel.Get();
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (var purchaseSetInfo in purchaseSetList)
                {
                    if (IsCheck(purchaseSetInfo, purchasePromotionList))
                    {
                        try
                        {
                            string errorMessage;
                            bool isSuccess = _purchaseSet.AddPurchaseSet(purchaseSetInfo, out errorMessage);
                            if (!isSuccess)
                            {
                                RAM.Alert(string.Format("商品{0}操作无效！{1}", purchaseSetInfo.GoodsName, errorMessage));
                                return;
                            }
                            IList<PurchasePromotionInfo> list = _purchasePromotion.GetPurchasePromotionListByGoodsId(purchaseSetInfo.GoodsId, purchaseSetInfo.HostingFilialeId, WarehouseId);
                            if (list != null && list.Count > 0)
                            {
                                foreach (var promotionInfo in list)
                                {
                                    _purchasePromotion.DeletePurchasePromotion(promotionInfo.PromotionId);
                                }
                            }
                            foreach (var purchasePromotionInfo in purchasePromotionList)
                            {
                                purchasePromotionInfo.PromotionId = Guid.NewGuid();
                                purchasePromotionInfo.GoodsId = purchaseSetInfo.GoodsId;
                                purchasePromotionInfo.WarehouseId = purchaseSetInfo.WarehouseId;
                                purchasePromotionInfo.HostingFilialeId = purchaseSetInfo.HostingFilialeId;
                                _purchasePromotion.AddPurchasePromotion(purchasePromotionInfo);
                            }
                            var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(purchaseSetInfo.GoodsId);
                            if (goodsInfo != null)
                            {
                                //报备管理添加操作记录添加
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, purchaseSetInfo.GoodsId, goodsInfo.GoodsCode,
                                    OperationPoint.ReportManage.Add.GetBusinessInfo(), string.Empty);
                            }
                        }
                        catch (Exception ex)
                        {
                            RAM.Alert("操作异常！" + ex);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                ts.Complete();
            }

            SubmitController.Submit();
            RAM.Alert("操作成功");
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
        #endregion

        #region[修改]
        protected void BtnEdit_Click(object sender, EventArgs e)
        {
            if (!SubmitController.Enabled)
            {
                RAM.Alert("程序正在处理");
                return;
            }
            if (!CheckPrice()) return;
            var hostingFilialeId = string.IsNullOrEmpty(RCB_Filile.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_Filile.SelectedValue);
            if (hostingFilialeId == Guid.Empty)
            {
                RAM.Alert("请选择物流公司");
                return;
            }
            PurchaseSetInfo purchaseSetInfoModel = _purchaseSet.GetPurchaseSetInfo(GoodsId, hostingFilialeId, WarehouseId);
            var purchaseSetLogList = _purchaseSetLog.GetPurchaseSetLogList(purchaseSetInfoModel.GoodsId, purchaseSetInfoModel.WarehouseId, hostingFilialeId).Where(p => p.Statue == (int)PurchaseSetLogStatue.NotAudit && p.LogType.Equals((int)PurchaseSetLogType.PurchasePrice)).ToList();
            if (purchaseSetLogList.Any())
            {
                RAM.Alert("“采购价”未审核，不允许重复修改！");
                return;
            }
            var purchaseSetList = GetPurchaseSetInfo();
            string message;
            IList<PurchasePromotionInfo> purchasePromotionList = GetPurchasePromotionList(out message);
            if (message.Length > 0)
            {
                RAM.Alert(message);
                return;
            }

            foreach (var purchaseSetInfo in purchaseSetList)
            {
                if (IsCheck(purchaseSetInfo, purchasePromotionList))
                {
                    try
                    {
                        using (var ts = new TransactionScope(TransactionScopeOption.Required))
                        {
                            purchaseSetInfo.PromotionId = purchasePromotionList.Count == 0 ? Guid.Empty : Guid.NewGuid();

                            //商品采购更改记录添加
                            decimal purchasePrice = decimal.Parse(tbPurchasePrice.Text);
                            decimal oldPurchasePrice = decimal.Parse(HidPurchasePrice.Value);
                            if (purchasePrice > oldPurchasePrice)
                            {
                                purchaseSetInfo.PurchasePrice = oldPurchasePrice;
                            }
                            string errorMessage;
                            var reslult = _purchaseSet.UpdatePurchaseSet(purchaseSetInfo, out errorMessage);
                            if (reslult == 0)
                            {
                                RAM.Alert("操作无效！" + errorMessage);
                                return;
                            }
                            var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(purchaseSetInfo.GoodsId);
                            if (goodsInfo != null)
                            {
                                //报备管理修改操作记录添加
                                var personnelInfo = CurrentSession.Personnel.Get();
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, purchaseSetInfo.GoodsId, goodsInfo.GoodsCode,
                                    OperationPoint.ReportManage.Edit.GetBusinessInfo(), string.Empty);
                            }
                            if (OldPurchaseSetInfo.PromotionId != Guid.Empty)
                            {
                                _purchasePromotion.DeletePurchasePromotion(OldPurchaseSetInfo.PromotionId);
                            }
                            IList<PurchasePromotionInfo> list = _purchasePromotion.GetPurchasePromotionListByGoodsId(purchaseSetInfo.GoodsId, purchaseSetInfo.HostingFilialeId, WarehouseId);
                            if (list != null && list.Count > 0)
                            {
                                foreach (var purchasePromotionInfo in list)
                                {
                                    _purchasePromotion.DeletePurchasePromotion(purchasePromotionInfo.PromotionId);
                                }
                            }
                            if (purchasePromotionList.Count > 0)
                            {
                                foreach (var purchasePromotionInfo in purchasePromotionList)
                                {
                                    purchasePromotionInfo.GoodsId = purchaseSetInfo.GoodsId;
                                    purchasePromotionInfo.WarehouseId = purchaseSetInfo.WarehouseId;
                                    purchasePromotionInfo.HostingFilialeId = purchaseSetInfo.HostingFilialeId;
                                    _purchasePromotion.AddPurchasePromotion(purchasePromotionInfo);
                                }
                            }
                            var purchaseSetLogInfos =_purchaseSetLog.GetPurchaseSetLogList(purchaseSetInfo.GoodsId, purchaseSetInfo.WarehouseId,purchaseSetInfo.HostingFilialeId);

                            #region[修改采购价]
                            if (purchasePrice != oldPurchasePrice)
                            {
                                if (purchaseSetLogInfos.All(ent=>ent.Statue != (int)PurchaseSetLogStatue.NotAudit ||
                                        ent.LogType != (int)PurchaseSetLogType.PurchasePrice))
                                {
                                    var purchaseSetLogInfo = new PurchaseSetLogInfo
                                    {
                                        LogId = Guid.NewGuid(),
                                        GoodsId = purchaseSetInfo.GoodsId,
                                        GoodsName = purchaseSetInfo.GoodsName,
                                        WarehouseId = purchaseSetInfo.WarehouseId,
                                        Applicant = CurrentSession.Personnel.Get().PersonnelId,
                                        Auditor = Guid.Empty,
                                        ChangeDate = DateTime.Now,
                                        ChangeReason = RTB_PurchasePriceReason.Text,
                                        ChangeValue = purchasePrice - oldPurchasePrice,
                                        LogType = (int)PurchaseSetLogType.PurchasePrice,
                                        OldValue = oldPurchasePrice,
                                        NewValue = purchasePrice,
                                        Statue = (int)PurchaseSetLogStatue.NotAudit,
                                        HostingFilialeId = purchaseSetInfo.HostingFilialeId
                                    };
                                    if (purchasePrice < oldPurchasePrice)
                                    {
                                        purchaseSetLogInfo.Statue = (int)PurchaseSetLogStatue.Pass;
                                    }
                                    _purchaseSetLog.AddPurchaseSetLog(purchaseSetLogInfo);
                                }
                            }
                            #endregion
                            
                            ts.Complete();
                        }
                        SubmitController.Submit();
                        RAM.Alert("操作成功");
                        RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert("操作异常！" + ex);
                        return;
                    }
                }
            }
        }
        #endregion

        protected void A_PurchasePrice_Click(object sender, EventArgs e)
        {
            tbPurchasePrice.Text = HidPurchasePrice.Value;
        }

        /// <summary>将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToDataTable<T>(IList<T> list)
        {
            return ToDataTable(list, null);
        }

        /// <summary>将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            var propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);

            var result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    var tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddOnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(RCB_Goods.SelectedValue) && string.Format("{0}", Guid.Empty) != RCB_Goods.SelectedValue)
            {
                var item = SetGoodsList.Items.FirstOrDefault(ent => ent.Value == RCB_Goods.SelectedValue);
                if (item == null)
                {
                    SetGoodsList.Items.Add(new RadListBoxItem(RCB_Goods.Text, RCB_Goods.SelectedValue));
                }
                else
                {
                    RAM.Alert(string.Format("商品[{0}]已在列表中!", RCB_Goods.Text));
                }
            }
            else
            {
                RAM.Alert("请选择商品");
            }
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RemoveOnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SetGoodsList.SelectedValue))
            {
                SetGoodsList.Items.Remove(SetGoodsList.SelectedItem);
            }
            else
            {
                RAM.Alert("温馨提示：请先选择数据！");
            }
        }
    }
}
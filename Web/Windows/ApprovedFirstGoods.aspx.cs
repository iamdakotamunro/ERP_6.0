using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Goods;
using ERP.DAL.Implement.Basis;
using ERP.DAL.Interface.IBasis;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Common;
using MIS.Enum;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class ApprovedFirstGoods : Page
    {
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        readonly IUnits _units = new Units(GlobalConfig.DB.FromType.Read);
        readonly GoodsManager _goodManager = new GoodsManager(_goodsCenterSao, null);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var goodsId = Request.QueryString["GoodsId"];
                var isShow = Request.QueryString["IsShow"];
                if (goodsId != null && isShow != null)
                {
                    LoadApprovedData(goodsId);//加载审核数据
                    if (isShow.Equals("0"))
                    {
                        btn_Pass.Visible = btn_NoPass.Visible = false;
                    }
                }
            }
        }

        #region 数据准备
        /// <summary>
        /// 商品品牌
        /// </summary>
        public void LoadBrandInfoData()
        {
            var goodsBrandInfoList = _goodsCenterSao.GetAllBrandList().ToList();
            ddl_Brand.DataSource = goodsBrandInfoList;
            ddl_Brand.DataTextField = "Brand";
            ddl_Brand.DataValueField = "BrandId";
            ddl_Brand.DataBind();
            ddl_Brand.Items.Insert(0, new ListItem("请选择", ""));
        }

        ///<summary>
        /// 商品类型
        ///</summary>
        public void LoadGoodsKindData()
        {
            var list = EnumAttribute.GetDict<GoodsKindType>();
            ddl_GoodsType.DataSource = list;
            ddl_GoodsType.DataTextField = "Value";
            ddl_GoodsType.DataValueField = "Key";
            ddl_GoodsType.DataBind();
            ddl_GoodsType.Items.Insert(0, new ListItem("请选择", ""));
        }

        ///<summary>
        /// 商品分类
        ///</summary>
        public void LoadGoodsClassData()
        {
            var goodsClassInfoList = _goodsClassManager.GetGoodsClassListWithRecursion();
            ddl_GoodsClass.DataSource = goodsClassInfoList;
            ddl_GoodsClass.DataTextField = "ClassName";
            ddl_GoodsClass.DataValueField = "ClassId";
            ddl_GoodsClass.DataBind();
            ddl_GoodsClass.Items.Insert(0, new ListItem("请选择", ""));
        }

        /// <summary>
        /// 关联销售组平台
        /// </summary>
        public void LoadGoodsGroupInfoData()
        {
            var goodsGroupInfoList = _goodsCenterSao.GetGroupList();
            ckb_RelatedSalesGroupPlatform.DataSource = goodsGroupInfoList;
            ckb_RelatedSalesGroupPlatform.DataTextField = "GroupName";
            ckb_RelatedSalesGroupPlatform.DataValueField = "GroupId";
            ckb_RelatedSalesGroupPlatform.DataBind();
        }

        ///<summary>
        /// 计量单位
        ///</summary>
        public void LoadUnitsInfoData()
        {
            var unitsInfoList = _units.GetUnitsList();
            ddl_Units.DataSource = unitsInfoList;
            ddl_Units.DataTextField = "Units";
            ddl_Units.DataValueField = "UnitsId";
            ddl_Units.DataBind();
            ddl_Units.Items.Insert(0, new ListItem("请选择", ""));
        }

        ///<summary>
        /// 质检分类
        ///</summary>
        public void LoadMedicineQualityTypeData()
        {
            var list = EnumAttribute.GetDict<MedicineQualityType>().ToList();
            list.Insert(0, new KeyValuePair<int, string>(-1, "请选择"));
            ddl_MedicineQualityType.DataSource = list;
            ddl_MedicineQualityType.DataTextField = "Value";
            ddl_MedicineQualityType.DataValueField = "Key";
            ddl_MedicineQualityType.DataBind();
        }

        ///<summary>
        /// 销售品种
        ///</summary>
        public void LoadMedicineSaleKindTypeData()
        {
            var list = EnumAttribute.GetDict<MedicineSaleKindType>().ToList();
            list.Insert(0, new KeyValuePair<int, string>(-1, "请选择"));
            ddl_MedicineSaleKindType.DataSource = list;
            ddl_MedicineSaleKindType.DataTextField = "Value";
            ddl_MedicineSaleKindType.DataValueField = "Key";
            ddl_MedicineSaleKindType.DataBind();
        }

        ///<summary>
        /// 剂型
        ///</summary>
        public void LoadMedicineDosageFormTypeData()
        {
            var list = EnumAttribute.GetDict<MedicineDosageFormType>().ToList();
            list.Insert(0, new KeyValuePair<int, string>(-1, "请选择"));
            ddl_MedicineDosageFormType.DataSource = list;
            ddl_MedicineDosageFormType.DataTextField = "Value";
            ddl_MedicineDosageFormType.DataValueField = "Key";
            ddl_MedicineDosageFormType.DataBind();
        }

        ///<summary>
        /// 储存条件
        ///</summary>
        public void LoadMedicineStorageConditionTypeData()
        {
            var list = EnumAttribute.GetDict<MedicineStorageConditionType>().ToList();
            list.Insert(0, new KeyValuePair<int, string>(-1, "请选择"));
            ddl_MedicineStorageConditionType.DataSource = list;
            ddl_MedicineStorageConditionType.DataTextField = "Value";
            ddl_MedicineStorageConditionType.DataValueField = "Key";
            ddl_MedicineStorageConditionType.DataBind();
        }

        ///<summary>
        /// 门店柜台
        ///</summary>
        public void LoadMedicineStoreCounterTypeData()
        {
            var list = EnumAttribute.GetDict<MedicineStoreCounterType>().ToList();
            list.Insert(0, new KeyValuePair<int, string>(-1, "请选择"));
            ddl_MedicineStoreCounterType.DataSource = list;
            ddl_MedicineStoreCounterType.DataTextField = "Value";
            ddl_MedicineStoreCounterType.DataValueField = "Key";
            ddl_MedicineStoreCounterType.DataBind();
        }

        ///<summary>
        /// 库位管理
        ///</summary>
        public void LoadLibraryManageTypeData()
        {
            var list = EnumAttribute.GetDict<LibraryManageType>().ToList();
            list.Insert(0, new KeyValuePair<int, string>(-1, "请选择"));
            ddl_LibraryManageType.DataSource = list;
            ddl_LibraryManageType.DataTextField = "Value";
            ddl_LibraryManageType.DataValueField = "Key";
            ddl_LibraryManageType.DataBind();
        }
        #endregion

        #region 审核准备
        //加载审核数据
        protected void LoadApprovedData(string goodsId)
        {
            LoadBrandInfoData();//商品品牌
            LoadGoodsKindData();//商品类型
            LoadGoodsClassData();//商品分类
            LoadGoodsGroupInfoData();//关联销售组平台
            LoadUnitsInfoData();//计量单位
            GoodsInfo goodsInfo = _goodsCenterSao.GetGoodsInfoBeforeUpdate(new Guid(goodsId));
            LoadGoodsData(goodsInfo);//初始化页面数据
            Hid_GoodsAuditState.Value = goodsInfo.GoodsAuditState.ToString();
        }

        //初始化页面数据
        protected void LoadGoodsData(GoodsInfo model)
        {
            #region 商品相关模型
            #region 商品扩展信息
            if (model.ExpandInfo != null)
            {
                //商品扩展信息
                txt_PackCount.Text = model.ExpandInfo.PackCount.ToString();
                txt_ReferencePrice.Text = model.ExpandInfo.ReferencePrice.ToString(CultureInfo.InvariantCulture);
                ckb_IsStatisticalPerformance.Checked = model.ExpandInfo.IsStatisticalPerformance;
                txt_Length.Text = model.ExpandInfo.Length.ToString(CultureInfo.InvariantCulture);
                txt_Width.Text = model.ExpandInfo.Width.ToString(CultureInfo.InvariantCulture);
                txt_Height.Text = model.ExpandInfo.Height.ToString(CultureInfo.InvariantCulture);
            }
            #endregion

            switch ((GoodsKindType)model.GoodsType)
            {
                case GoodsKindType.Frames:
                case GoodsKindType.SunFrame:
                case GoodsKindType.SportFrame:
                case GoodsKindType.FunctionFrame:
                    MessageBox.AppendScript(this, "$(\"#A_FrameInfo\").css(\"display\", \"\");$(\"input[id$='Hid_FrameInfo']\").val(\"1\");");
                    #region 框架信息
                    if (model.FrameGoodsInfo != null)
                    {
                        //框架信息
                        txt_TempleLength.Text = model.FrameGoodsInfo.TempleLength.ToString();
                        txt_Besiclometer.Text = model.FrameGoodsInfo.Besiclometer.ToString();
                        txt_FrameWithinWidth.Text = model.FrameGoodsInfo.FrameWithinWidth.ToString();
                        txt_NoseWidth.Text = model.FrameGoodsInfo.NoseWidth.ToString();
                        txt_OpticalVerticalHeight.Text = model.FrameGoodsInfo.OpticalVerticalHeight.ToString();
                        txt_EyeSize.Text = model.FrameGoodsInfo.EyeSize.ToString();
                    }
                    #endregion
                    break;
                case GoodsKindType.Medicinal:
                case GoodsKindType.PrescriptionMedicine:
                    MessageBox.AppendScript(this, "$(\"#A_Medicament\").css(\"display\", \"\");$(\"input[id$='Hid_Medicament']\").val(\"1\");");
                    #region 加载相关分类数据
                    LoadMedicineQualityTypeData();//质检分类
                    LoadMedicineSaleKindTypeData();//销售品种
                    LoadMedicineDosageFormTypeData();//剂型
                    LoadMedicineStorageConditionTypeData();//储存条件
                    LoadMedicineStoreCounterTypeData();//门店柜台
                    LoadLibraryManageTypeData();//库位管理
                    #endregion
                    #region 商品药品信息
                    if (model.GoodsMedicineInfo != null)
                    {
                        //商品药品信息
                        txt_ChemistryName.Text = model.GoodsMedicineInfo.ChemistryName;
                        txt_ChemistryNameFirstLetter.Text = model.GoodsMedicineInfo.ChemistryNameFirstLetter;
                        ddl_MedicineQualityType.SelectedValue = model.GoodsMedicineInfo.MedicineQualityType.ToString();
                        ddl_MedicineSaleKindType.SelectedValue = model.GoodsMedicineInfo.MedicineSaleKindType.ToString();
                        txt_MedicineWholesalePrice.Text = model.GoodsMedicineInfo.MedicineWholesalePrice.ToString(CultureInfo.InvariantCulture);
                        ddl_MedicineTaxRateType.SelectedValue = model.GoodsMedicineInfo.MedicineTaxRateType.ToString();
                        txt_QualityStandardDescription.Text = model.GoodsMedicineInfo.QualityStandardDescription;
                        ddl_MedicineDosageFormType.SelectedValue = model.GoodsMedicineInfo.MedicineDosageFormType.ToString();
                        ddl_MedicineStorageConditionType.SelectedValue = model.GoodsMedicineInfo.MedicineStorageConditionType.ToString();
                        ddl_MedicineCuringKindType.SelectedValue = model.GoodsMedicineInfo.MedicineCuringKindType.ToString();
                        ddl_MedicineCuringCycleType.SelectedValue = model.GoodsMedicineInfo.MedicineCuringCycleType.ToString();
                        ddl_MedicineStoreCounterType.SelectedValue = model.GoodsMedicineInfo.MedicineStoreCounterType.ToString();
                        ddl_LibraryManageType.SelectedValue = model.GoodsMedicineInfo.MedicineLibraryManageType.ToString();
                    }
                    #endregion
                    break;
                case GoodsKindType.HealthProducts:
                case GoodsKindType.FamilyPlanningProuducts:
                case GoodsKindType.MedicalEquipment:
                    break;
            }

            txt_GoodsName.Text = model.GoodsName;
            txt_PurchaseNameFirstLetter.Text = model.PurchaseNameFirstLetter;
            ddl_Brand.SelectedValue = model.BrandId.ToString();
            txt_SupplierGoodsCode.Text = model.SupplierGoodsCode;
            ddl_GoodsType.SelectedValue = model.GoodsType.ToString();
            ddl_GoodsClass.SelectedValue = model.ClassId.ToString();
            var ddlUnitsSelected = ddl_Units.Items.FindByText(model.Units);
            if (ddlUnitsSelected != null)
            {
                ddlUnitsSelected.Selected = true;
            }
            txt_Specification.Text = model.Specification;
            ddl_SaleStockType.SelectedValue = model.SaleStockType.ToString();
            txt_StockStatus.Text = model.StockStatus;
            txt_BarCode.Text = model.BarCode;
            txt_MarketPrice.Text = model.MarketPrice.ToString(CultureInfo.InvariantCulture);
            txt_ShelfLife.Text = model.ShelfLife;
            ckb_IsImportedGoods.Checked = model.IsImportedGoods;
            ckb_IsLuxury.Checked = model.IsLuxury;
            ckb_IsBannedPurchase.Checked = model.IsBannedPurchase;
            ckb_IsBannedSale.Checked = model.IsBannedSale;
            ckb_IsLuxury.Checked = model.IsLuxury;
            ckb_IsOnShelf.Checked = model.IsOnShelf;
            txt_Weight.Text = model.Weight.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(model.ImgPath))
            {
                PreA.HRef = model.ImgPath;
            }
            else
            {
                UploadImgName.Visible = false;
            }
            #endregion

            #region 关联公司采购组
            if (model.DictGoodsPurchase != null && model.DictGoodsPurchase.Count > 0)
            {
                var filialeList = CacheCollection.Filiale.GetList().Where(w => (w.FilialeTypes.Contains((int)FilialeType.SaleCompany)) && w.Rank == (int)FilialeRank.Head && w.IsActive).OrderBy(f => f.OrderIndex);
                var purchasingFilialeInfoList = filialeList.Select(w => new PurchasingFilialeInfo
                {
                    ID = w.ID,
                    Name = w.Name,
                    Code = w.ID.ToString().Split('-')[0]
                }).ToList();

                if (filialeList.Any())
                {
                    foreach (var keyValuePair in model.DictGoodsPurchase)
                    {
                        var info = purchasingFilialeInfoList.FirstOrDefault(w => w.ID == keyValuePair.Key);
                        if (info != null)
                        {
                            foreach (var entityPurchaseType in keyValuePair.Value)
                            {
                                switch (entityPurchaseType)
                                {
                                    case EntityPurchaseType.Direct:
                                        info.IsDirect = true;
                                        break;
                                    case EntityPurchaseType.Join:
                                        info.IsJoin = true;
                                        break;
                                    case EntityPurchaseType.Alliance:
                                        info.IsAlliance = true;
                                        break;
                                }
                            }
                            if (info.IsDirect && info.IsJoin && info.IsAlliance)
                            {
                                info.IsAll = true;
                            }
                        }
                    }
                    RG_RelatedCompanyPurchasingGroup.DataSource = purchasingFilialeInfoList;
                    RG_RelatedCompanyPurchasingGroup.DataBind();
                }
            }
            #endregion

            #region 关联销售组平台
            foreach (ListItem item in ckb_RelatedSalesGroupPlatform.Items)
            {
                foreach (var value in model.SaleGroupList)
                {
                    if (item.Value.Equals(value.GroupId.ToString()))
                    {
                        item.Selected = true;
                    }
                }
            }
            #endregion
        }
        #endregion

        #region 数据列表相关
        //关联公司采购组
        protected void RG_RelatedCompanyPurchasingGroup_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            var filialeList = CacheCollection.Filiale.GetList().Where(w => (w.FilialeTypes.Contains((int)FilialeType.SaleCompany)) && w.Rank == (int)FilialeRank.Head && w.IsActive).OrderBy(f => f.OrderIndex);
            var purchasingFilialeInfoList = filialeList.Select(w => new PurchasingFilialeInfo
            {
                ID = w.ID,
                Name = w.Name,
                Code = w.ID.ToString().Split('-')[0]
            }).ToList();
            RG_RelatedCompanyPurchasingGroup.DataSource = purchasingFilialeInfoList;
        }
        #endregion

        //核准
        protected void btn_Pass_Click(object sender, EventArgs e)
        {
            var goodsId = Request.QueryString["GoodsId"];
            var goodsAuditState = Hid_GoodsAuditState.Value;
            if (goodsId != null && goodsAuditState != null)
            {
                int auditState = int.Parse(goodsAuditState);
                string auditStateMemo = string.Empty;
                switch ((GoodsAuditState)int.Parse(goodsAuditState))
                {
                    case GoodsAuditState.PurchasingWaitAudit:
                        auditState = (int)GoodsAuditState.QualityWaitAudit;
                        auditStateMemo = "【采购经理审核】:审核通过;";
                        break;
                    case GoodsAuditState.QualityWaitAudit:
                        auditState = (int)GoodsAuditState.CaptainWaitAudit;
                        auditStateMemo = "【质管部审核】:审核通过;";
                        break;
                    case GoodsAuditState.CaptainWaitAudit:
                        auditState = (int)GoodsAuditState.Pass;
                        auditStateMemo = "【负责人终审】:审核通过;";
                        break;
                }
                auditStateMemo = WebControl.RetrunUserAndTime("[" + auditStateMemo + "审核说明:" + (string.IsNullOrEmpty(txt_GoodsAuditStateMemo.Text) ? "暂无说明" : txt_GoodsAuditStateMemo.Text) + ";]");
                string failMessage;
                bool result = _goodManager.UpdateGoodsAuditStateAndAuditStateMemo(new Guid(goodsId), auditState, auditStateMemo, out failMessage);
                if (result)
                {
                    MessageBox.AppendScript(this, "alert('保存成功！');CloseAndRebind();");
                }
                else
                {
                    MessageBox.Show(this, "保存失败！" + failMessage);
                }
            }
        }

        //核退
        protected void btn_NoPass_Click(object sender, EventArgs e)
        {
            #region 验证数据
            var errorMsg = CheckData();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(this, errorMsg);
                return;
            }
            #endregion

            var goodsId = Request.QueryString["GoodsId"];
            var goodsAuditState = Hid_GoodsAuditState.Value;
            if (goodsId != null && goodsAuditState != null)
            {
                string auditStateMemo = string.Empty;
                switch ((GoodsAuditState)int.Parse(goodsAuditState))
                {
                    case GoodsAuditState.PurchasingWaitAudit:
                        auditStateMemo = "【采购经理审核】:审核不通过;";
                        break;
                    case GoodsAuditState.QualityWaitAudit:
                        auditStateMemo = "【质管部审核】:审核不通过;";
                        break;
                    case GoodsAuditState.CaptainWaitAudit:
                        auditStateMemo = "【负责人终审】:审核不通过;";
                        break;
                }
                auditStateMemo = WebControl.RetrunUserAndTime("[" + auditStateMemo + "审核说明:" + (string.IsNullOrEmpty(txt_GoodsAuditStateMemo.Text) ? "暂无说明" : txt_GoodsAuditStateMemo.Text) + ";]");
                string failMessage;
                bool result = _goodManager.UpdateGoodsAuditStateAndAuditStateMemo(new Guid(goodsId), (int)GoodsAuditState.NoPass, auditStateMemo, out failMessage);
                if (result)
                {
                    MessageBox.AppendScript(this, "alert('保存成功！');CloseAndRebind();");
                }
                else
                {
                    MessageBox.Show(this, "保存失败！" + failMessage);
                }
            }
        }

        //验证数据
        protected string CheckData()
        {
            var errorMsg = new StringBuilder();
            var goodsAuditStateMemo = txt_GoodsAuditStateMemo.Text;
            if (string.IsNullOrEmpty(goodsAuditStateMemo))
            {
                errorMsg.Append("请填写“审核说明”！").Append("\\n");
            }
            return errorMsg.ToString();
        }
    }
}
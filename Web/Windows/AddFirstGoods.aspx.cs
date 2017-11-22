using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Goods;
using ERP.DAL.Implement.Basis;
using ERP.DAL.Interface.IBasis;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using MIS.Enum;
using Telerik.Web.UI;
using Keede.Ecsoft.Model;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class AddFirstGoods : WindowsPage
    {
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        readonly IUnits _units = new Units(GlobalConfig.DB.FromType.Read);
        readonly GoodsManager _goodManager = new GoodsManager(_goodsCenterSao, null);

        #region 属性
        /// <summary>
        /// 商品资质列表
        /// </summary>
        public IList<GoodsInformationInfo> GoodsInformationInfoList
        {
            get
            {
                if (ViewState["GoodsInformationInfoList"] == null)
                {
                    return new List<GoodsInformationInfo>();
                }
                return (IList<GoodsInformationInfo>)ViewState["GoodsInformationInfoList"];
            }
            set { ViewState["GoodsInformationInfoList"] = value; }
        }

        /// <summary>
        /// 资质类型
        /// </summary>
        public List<KeyValuePair<int, string>> GoodsQualificationTypeList
        {
            get
            {
                if (ViewState["GoodsQualificationTypeList"] == null)
                {
                    return new List<KeyValuePair<int, string>>();
                }
                return (List<KeyValuePair<int, string>>)ViewState["GoodsQualificationTypeList"];
            }
            set { ViewState["GoodsQualificationTypeList"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBrandInfoData();//商品品牌
                LoadGoodsKindData();//商品类型
                LoadGoodsClassData();//商品分类
                LoadGoodsGroupInfoData();//关联销售组平台
                LoadUnitsInfoData();//计量单位
                LoadGoodsQualificationTypeData();//资质类型
            }
        }

        #region 数据准备
        /// <summary>
        /// 商品品牌
        /// </summary>
        public void LoadBrandInfoData()
        {
            var goodsBrandInfoList = _goodsCenterSao.GetAllBrandList().ToList();
            rcb_Brand.DataSource = goodsBrandInfoList;
            rcb_Brand.DataTextField = "Brand";
            rcb_Brand.DataValueField = "BrandId";
            rcb_Brand.DataBind();
            rcb_Brand.Items.Insert(0, new RadComboBoxItem("请选择", ""));
        }

        ///<summary>
        /// 商品类型
        ///</summary>
        public void LoadGoodsKindData()
        {
            var list = EnumAttribute.GetDict<GoodsKindType>();
            rcb_GoodsType.DataSource = list;
            rcb_GoodsType.DataTextField = "Value";
            rcb_GoodsType.DataValueField = "Key";
            rcb_GoodsType.DataBind();
            rcb_GoodsType.Items.Insert(0, new RadComboBoxItem("请选择", ""));
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
        /// 资质类型
        ///</summary>
        public void LoadGoodsQualificationTypeData()
        {
            GoodsQualificationTypeList = EnumAttribute.GetDict<GoodsQualificationType>().ToList();
        }
        #endregion

        #region 数据列表相关
        #region 关联公司采购组
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

        #region 商品资质数据源
        //商品资质数据源
        protected void RepeaterGoodsInformationsDataBind()
        {
            Repeater_GoodsInformations.DataSource = GoodsInformationInfoList.OrderByDescending(ent => ent.UploadDate);
            Repeater_GoodsInformations.DataBind();
            Hid_GoodsQualification.Value = "1";
        }

        //获取商品资质数据
        protected void GetRepeaterGoodsInformationsData()
        {
            GoodsInformationInfoList = new List<GoodsInformationInfo>();
            foreach (RepeaterItem item in Repeater_GoodsInformations.Items)
            {
                var ddlGoodsQualificationType = (DropDownList)item.FindControl("ddl_GoodsQualificationType");
                var txtUnitName = (TextBox)item.FindControl("txt_UnitName");
                var txtCertificateNumber = (TextBox)item.FindControl("txt_CertificateNumber");
                var uploadImgNameInfo = (TextBox)item.FindControl("UploadImgNameInfo");
                var uploadImgInfo = (FileUpload)item.FindControl("UploadImgInfo");
                var txtOverdueDate = (TextBox)item.FindControl("txt_OverdueDate");
                var hidImgPath = (HiddenField)item.FindControl("Hid_ImgPath");

                if (GoodsInformationInfoList.Count(act => (act.Number == txtUnitName.Text || act.Number == txtCertificateNumber.Text) && act.QualificationType == int.Parse(ddlGoodsQualificationType.SelectedValue)) > 0)
                {
                    continue;
                }

                var goodsInformationInfo = new GoodsInformationInfo
                {
                    ID = Guid.NewGuid(),
                    QualificationType = int.Parse(ddlGoodsQualificationType.SelectedValue),
                    UploadDate = DateTime.Now
                };

                if (!string.IsNullOrEmpty(txtUnitName.Text))
                {
                    goodsInformationInfo.Number = txtUnitName.Text;
                }
                if (!string.IsNullOrEmpty(txtCertificateNumber.Text))
                {
                    goodsInformationInfo.Number = txtCertificateNumber.Text;
                }

                if (string.IsNullOrEmpty(hidImgPath.Value))
                {
                    string filePath = UploadImage(uploadImgInfo, uploadImgNameInfo, string.Empty, "../UserDir/GoodsInfomation/Company/{0}");
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        string ext = Path.GetExtension(filePath).ToLower();
                        goodsInformationInfo.Path = filePath;
                        goodsInformationInfo.ExtensionName = ext;
                        hidImgPath.Value = filePath;
                    }
                }
                else
                {
                    string ext = Path.GetExtension(hidImgPath.Value).ToLower();
                    goodsInformationInfo.Path = hidImgPath.Value;
                    goodsInformationInfo.ExtensionName = ext;
                }

                if (!string.IsNullOrEmpty(txtOverdueDate.Text))
                {
                    goodsInformationInfo.OverdueDate = DateTime.Parse(txtOverdueDate.Text);
                }

                GoodsInformationInfoList.Add(goodsInformationInfo);
            }
        }

        #region 商品资质列表操作
        protected void Repeater_GoodsInformations_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                #region 资质类型
                var ddlGoodsQualificationType = (DropDownList)e.Item.FindControl("ddl_GoodsQualificationType");
                ddlGoodsQualificationType.DataSource = GoodsQualificationTypeList;
                ddlGoodsQualificationType.DataTextField = "Value";
                ddlGoodsQualificationType.DataValueField = "Key";
                ddlGoodsQualificationType.DataBind();
                ddlGoodsQualificationType.SelectedValue = DataBinder.Eval(e.Item.DataItem, "QualificationType").ToString();
                #endregion

                #region 显示/隐藏
                var tdUnitName = e.Item.FindControl("td_UnitName");
                var tdCertificateNumber = e.Item.FindControl("td_CertificateNumber");
                var tdUploadImgNameInfo = e.Item.FindControl("td_UploadImgNameInfo");
                var tdOverdueDate = e.Item.FindControl("td_OverdueDate");

                var goodsQualificationType = int.Parse(ddlGoodsQualificationType.SelectedValue);
                if (goodsQualificationType == (int)GoodsQualificationType.ProductionUnit)
                {
                    tdUnitName.Visible = true;
                    tdCertificateNumber.Visible = false;
                    tdUploadImgNameInfo.Visible = false;
                    tdOverdueDate.Visible = false;
                }
                else if (goodsQualificationType == (int)GoodsQualificationType.ProductionPermitNo)
                {
                    tdUnitName.Visible = false;
                    tdCertificateNumber.Visible = true;
                    tdUploadImgNameInfo.Visible = false;
                    tdOverdueDate.Visible = true;
                }
                else
                {
                    tdUnitName.Visible = false;
                    tdCertificateNumber.Visible = true;
                    tdUploadImgNameInfo.Visible = true;
                    tdOverdueDate.Visible = true;
                }
                #endregion

                #region 必填项处理
                var txtUnitName = (TextBox)e.Item.FindControl("txt_UnitName");
                var txtCertificateNumber = (TextBox)e.Item.FindControl("txt_CertificateNumber");
                var uploadImgNameInfo = (TextBox)e.Item.FindControl("UploadImgNameInfo");
                var txtOverdueDate = (TextBox)e.Item.FindControl("txt_OverdueDate");

                ddlGoodsQualificationType.CssClass = "Check";
                if (goodsQualificationType == (int)GoodsQualificationType.ProductionUnit)
                {
                    txtUnitName.CssClass = "Check";
                }
                else if (goodsQualificationType == (int)GoodsQualificationType.ProductionPermitNo)
                {
                    txtCertificateNumber.CssClass = "Check";
                    txtOverdueDate.CssClass = "Check";
                }
                else
                {
                    txtCertificateNumber.CssClass = "Check";
                    uploadImgNameInfo.CssClass = "Check";
                    txtOverdueDate.CssClass = "Check";
                }
                #endregion

                #region 状态
                var number = DataBinder.Eval(e.Item.DataItem, "Number") == null ? string.Empty : DataBinder.Eval(e.Item.DataItem, "Number").ToString();
                var overdueDate = DataBinder.Eval(e.Item.DataItem, "OverdueDate") == null ? DateTime.MinValue.ToString(CultureInfo.InvariantCulture) : DataBinder.Eval(e.Item.DataItem, "OverdueDate").ToString();
                var litState = (Literal)e.Item.FindControl("Lit_State");
                if (string.IsNullOrEmpty(number))
                {
                    litState.Text = "<span style=\"color: red;\">缺失</span>";
                }
                else
                {
                    litState.Text = "<span>正常</span>";
                }

                if (!string.IsNullOrEmpty(overdueDate) && Convert.ToDateTime(overdueDate) != DateTime.MinValue)
                {
                    if (DateTime.Parse(overdueDate) >= DateTime.Now && DateTime.Parse(overdueDate) <= DateTime.Now.AddMonths(1))
                    {
                        litState.Text = "<span style=\"color: red;\">快过期</span>";
                    }
                    else if (DateTime.Parse(overdueDate) < DateTime.Now)
                    {
                        litState.Text = "<span style=\"color: red;\">过期</span>";
                    }
                }
                #endregion
            }
        }

        //删除商品资质
        protected void Imgbtn_Del_Click(object sender, EventArgs e)
        {
            var id = new Guid(((ImageButton)sender).CommandName);
            var removeItem = GoodsInformationInfoList.FirstOrDefault(p => p.ID.Equals(id));
            GoodsInformationInfoList.Remove(removeItem);
            RepeaterGoodsInformationsDataBind();
        }
        #endregion
        #endregion
        #endregion

        #region SelectedIndexChanged事件
        //商品品牌搜索
        protected void rcb_Brand_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var goodsBrandInfoList = _goodsCenterSao.GetAllBrandList().Where(p => p.Brand.Contains(e.Text)).ToList();
                int totalCount = goodsBrandInfoList.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in goodsBrandInfoList)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.Brand, item.BrandId.ToString()));
                    }
                }
            }
        }

        //商品类型
        protected void rcb_GoodsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var goodsType = rcb_GoodsType.SelectedValue;
            if (string.IsNullOrEmpty(goodsType))
            {
                return;
            }
            var goodsKindType = (GoodsKindType)int.Parse(goodsType);
            bool flag = (//商品类型是否由“非处方药”更改为“处方药”或者由“处方药”更改为“非处方药”
                        (Hid_GoodsType.Value.Equals(((int)GoodsKindType.Medicinal).ToString()) && goodsType.Equals(((int)GoodsKindType.PrescriptionMedicine).ToString())) ||
                        (Hid_GoodsType.Value.Equals(((int)GoodsKindType.PrescriptionMedicine).ToString()) && goodsType.Equals(((int)GoodsKindType.Medicinal).ToString()))
                        );
            //清除框架信息
            ClearFrameInfoData();
            if (!flag)
            {
                //清除药品信息
                ClearMedicamentData();
            }
            MessageBox.AppendScript(this, "changeGoodsType('Remove');checkMedicament('Remove')");
            switch (goodsKindType)
            {
                case GoodsKindType.Frames:
                case GoodsKindType.SunFrame:
                case GoodsKindType.SportFrame:
                case GoodsKindType.FunctionFrame:
                    MessageBox.AppendScript(this, "$(\"#A_FrameInfo\").css(\"display\", \"\");$(\"input[id$='Hid_FrameInfo']\").val(\"1\");");
                    break;
                case GoodsKindType.Medicinal:
                case GoodsKindType.PrescriptionMedicine:
                    MessageBox.AppendScript(this, "$(\"#A_Medicament\").css(\"display\", \"\");$(\"input[id$='Hid_Medicament']\").val(\"1\");");
                    MessageBox.AppendScript(this, "changeGoodsType('Add');checkMedicament('Add')");

                    #region 加载相关分类数据
                    if (!flag)
                    {
                        LoadMedicineQualityTypeData();//质检分类
                        LoadMedicineSaleKindTypeData();//销售品种
                        LoadMedicineDosageFormTypeData();//剂型
                        LoadMedicineStorageConditionTypeData();//储存条件
                        LoadMedicineStoreCounterTypeData();//门店柜台
                        LoadLibraryManageTypeData();//库位管理
                    }
                    #endregion
                    break;
                case GoodsKindType.HealthProducts:
                case GoodsKindType.FamilyPlanningProuducts:
                case GoodsKindType.MedicalEquipment:
                    MessageBox.AppendScript(this, "changeGoodsType('Add');$(\"input[id$='Hid_Other']\").val(\"1\");");
                    break;
            }
            Hid_GoodsType.Value = goodsType;
        }

        //商品类型搜索
        protected void rcb_GoodsType_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var goodsKindTypeList = EnumAttribute.GetDict<GoodsKindType>().Where(p => p.Value.Contains(e.Text)).ToList();
                int totalCount = goodsKindTypeList.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in goodsKindTypeList)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.Value, item.Key.ToString()));
                    }
                }
            }
        }

        //商品分类
        protected void ddl_GoodsClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            var goodsClass = ddl_GoodsClass.SelectedValue;
            if (string.IsNullOrEmpty(goodsClass))
            {
                ckb_LightFieldList.Items.Clear();
                ckb_AstigmatismFieldList.Items.Clear();
                ckb_AxialViewFieldList.Items.Clear();
                return;
            }
            var fieldInfoList = _goodsCenterSao.GetFieldListByGoodsClassId(new Guid(goodsClass)).ToList();

            //光度
            var lightParentField = fieldInfoList.FirstOrDefault(ent => ent.FieldName == "光度");
            if (lightParentField != null)
            {
                Hid_LightParentFieldId.Value = lightParentField.FieldId.ToString();
                var data = lightParentField.ChildFields.OrderBy(act => act.OrderIndex);
                ckb_LightFieldList.DataSource = data;
                ckb_LightFieldList.DataTextField = "FieldValue";
                ckb_LightFieldList.DataValueField = "FieldId";
                ckb_LightFieldList.DataBind();
            }
            else
            {
                ckb_LightFieldList.Items.Clear();
                Hid_LightParentFieldId.Value = string.Empty;
            }

            //散光
            var astigmatismParentField = fieldInfoList.FirstOrDefault(ent => ent.FieldName == "散光");
            if (astigmatismParentField != null)
            {
                Hid_AstigmatismParentFieldId.Value = astigmatismParentField.FieldId.ToString();
                var data = astigmatismParentField.ChildFields.OrderBy(act => act.OrderIndex);
                ckb_AstigmatismFieldList.DataSource = data;
                ckb_AstigmatismFieldList.DataTextField = "FieldValue";
                ckb_AstigmatismFieldList.DataValueField = "FieldId";
                ckb_AstigmatismFieldList.DataBind();
            }
            else
            {
                ckb_AstigmatismFieldList.Items.Clear();
                Hid_AstigmatismParentFieldId.Value = string.Empty;
            }

            //轴位
            var axialViewParentField = fieldInfoList.FirstOrDefault(ent => ent.FieldName == "轴位");
            if (axialViewParentField != null)
            {
                Hid_AxialParentFieldId.Value = axialViewParentField.FieldId.ToString();
                var data = axialViewParentField.ChildFields.OrderBy(act => act.OrderIndex);
                ckb_AxialViewFieldList.DataSource = data;
                ckb_AxialViewFieldList.DataTextField = "FieldValue";
                ckb_AxialViewFieldList.DataValueField = "FieldId";
                ckb_AxialViewFieldList.DataBind();
            }
            else
            {
                ckb_AxialViewFieldList.Items.Clear();
                Hid_AxialParentFieldId.Value = string.Empty;
            }

            if (lightParentField == null && astigmatismParentField == null && axialViewParentField == null)
            {
                MessageBox.AppendScript(this, "$(\"#A_SubProductAttributes\").css(\"display\", \"none\");$(\"input[id$='Hid_SubProductAttributes']\").val(\"\");");
            }
            else
            {
                MessageBox.AppendScript(this, "$(\"#A_SubProductAttributes\").css(\"display\", \"\");$(\"input[id$='Hid_SubProductAttributes']\").val(\"1\");");
            }
        }

        //商品资质
        protected void ddl_GoodsQualificationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region 资质类型
            var ddlGoodsQualificationType = ((DropDownList)sender);
            #endregion

            #region 显示/隐藏
            var tdUnitName = ((DropDownList)sender).Parent.FindControl("td_UnitName");
            var tdCertificateNumber = ((DropDownList)sender).Parent.FindControl("td_CertificateNumber");
            var tdUploadImgNameInfo = ((DropDownList)sender).Parent.FindControl("td_UploadImgNameInfo");
            var tdOverdueDate = ((DropDownList)sender).Parent.FindControl("td_OverdueDate");

            var goodsQualificationType = int.Parse(ddlGoodsQualificationType.SelectedValue);
            if (goodsQualificationType == (int)GoodsQualificationType.ProductionUnit)
            {
                tdUnitName.Visible = true;
                tdCertificateNumber.Visible = false;
                tdUploadImgNameInfo.Visible = false;
                tdOverdueDate.Visible = false;
            }
            else if (goodsQualificationType == (int)GoodsQualificationType.ProductionPermitNo)
            {
                tdUnitName.Visible = false;
                tdCertificateNumber.Visible = true;
                tdUploadImgNameInfo.Visible = false;
                tdOverdueDate.Visible = true;
            }
            else
            {
                tdUnitName.Visible = false;
                tdCertificateNumber.Visible = true;
                tdUploadImgNameInfo.Visible = true;
                tdOverdueDate.Visible = true;
            }
            #endregion

            #region 必填项处理
            var txtUnitName = (TextBox)((DropDownList)sender).Parent.FindControl("txt_UnitName");
            var txtCertificateNumber = (TextBox)((DropDownList)sender).Parent.FindControl("txt_CertificateNumber");
            var uploadImgNameInfo = (TextBox)((DropDownList)sender).Parent.FindControl("UploadImgNameInfo");
            var txtOverdueDate = (TextBox)((DropDownList)sender).Parent.FindControl("txt_OverdueDate");

            ddlGoodsQualificationType.CssClass = "Check";
            if (goodsQualificationType == (int)GoodsQualificationType.ProductionUnit)
            {
                txtUnitName.CssClass = "Check";
                txtCertificateNumber.CssClass = "";
                uploadImgNameInfo.CssClass = "";
                txtOverdueDate.CssClass = "";
            }
            else if (goodsQualificationType == (int)GoodsQualificationType.ProductionPermitNo)
            {
                txtUnitName.CssClass = "";
                txtCertificateNumber.CssClass = "Check";
                uploadImgNameInfo.CssClass = "";
                txtOverdueDate.CssClass = "Check";
            }
            else
            {
                txtUnitName.CssClass = "";
                txtCertificateNumber.CssClass = "Check";
                uploadImgNameInfo.CssClass = "Check";
                txtOverdueDate.CssClass = "Check";
            }
            #endregion
        }
        #endregion

        //增加商品资质
        protected void btn_AddInfo_Click(object sender, EventArgs e)
        {
            GetRepeaterGoodsInformationsData();
            var goodsQualificationTypeKeys = EnumAttribute.GetDict<GoodsQualificationType>().Keys;
            GoodsInformationInfoList.Add(new GoodsInformationInfo
            {
                ID = Guid.NewGuid(),
                QualificationType = goodsQualificationTypeKeys.FirstOrDefault(key => GoodsInformationInfoList.All(act => act.QualificationType != key)),
                UploadDate = DateTime.Now
            });
            RepeaterGoodsInformationsDataBind();
        }

        //保存
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            Save(0);
        }

        //保存并添加
        protected void btn_SaveAdd_Click(object sender, EventArgs e)
        {
            Save(1);
        }

        //保存并复制添加
        protected void btn_SaveCopyAdd_Click(object sender, EventArgs e)
        {
            Save(2);
        }

        //保存数据
        private void Save(int type = 0)
        {
            #region 获取商品资质数据
            GetRepeaterGoodsInformationsData();
            #endregion

            #region 验证数据
            var errorMsg = CheckData();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(this, errorMsg);
                return;
            }
            #endregion

            #region 赋值
            GoodsInfo goodsInfo = new GoodsInfo();
            Dictionary<Guid, List<Guid>> fieldDict = new Dictionary<Guid, List<Guid>>();
            EditModel(goodsInfo, fieldDict);
            #endregion

            #region 保存数据
            try
            {
                string failMessage;
                //添加主商品
                var success = _goodManager.Add(CurrentSession.Personnel.Get().PersonnelId, goodsInfo, fieldDict, out failMessage);
                if (success)
                {
                    if (type == 0)
                    {
                        MessageBox.AppendScript(this, "alert('保存成功！');parent.EditGoodsAttribute('" + goodsInfo.GoodsId + "');CancelWindow();");
                    }
                    else if (type == 1)
                    {
                        MessageBox.AppendScript(this, "alert('保存成功！');parent.EditGoodsAttribute('" + goodsInfo.GoodsId + "');location.href=location.href;");
                    }
                    else if (type == 2)
                    {
                        UploadImgName.Text = string.Empty;
                        MessageBox.AppendScript(this, "alert('保存成功！');parent.EditGoodsAttribute('" + goodsInfo.GoodsId + "');");
                    }
                }
                else
                {
                    MessageBox.Show(this, "保存失败：" + failMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "保存失败！" + ex.Message);
            }
            #endregion
        }

        #region 编辑Model
        protected void EditModel(GoodsInfo goodsInfo, Dictionary<Guid, List<Guid>> fieldDict)
        {
            #region 商品相关模型
            var goodsId = Guid.NewGuid();
            //商品扩展信息
            var expendInfo = new GoodsExtendInfo
            {
                GoodsId = goodsId,
                PackCount = int.Parse(txt_PackCount.Text.Trim()),
                ReferencePrice = decimal.Parse(txt_ReferencePrice.Text.Trim()),
                ApprovalTime = Convert.ToDateTime("1900-01-01"),
                IsStatisticalPerformance = ckb_IsStatisticalPerformance.Checked,
                Length = decimal.Parse(txt_Length.Text.Trim()),
                Width = decimal.Parse(txt_Width.Text.Trim()),
                Height = decimal.Parse(txt_Height.Text.Trim()),

                JoinPrice = 0,
                ImplicitCost = 0,
                YearDiscount = 1
            };

            goodsInfo.GoodsAuditState = (int)GoodsAuditState.Pass;
            #region 商品类型相关信息
            //框架信息
            var frameInfo = new FrameGoodsInfo();
            //商品药品信息
            var goodsMedicineInfo = new GoodsMedicineInfo();
            var goodsType = rcb_GoodsType.SelectedValue;
            if (!string.IsNullOrEmpty(goodsType))
            {
                var goodsKindType = (GoodsKindType)int.Parse(goodsType);
                switch (goodsKindType)
                {
                    #region 框架信息
                    case GoodsKindType.Frames:
                    case GoodsKindType.SunFrame:
                    case GoodsKindType.SportFrame:
                    case GoodsKindType.FunctionFrame:
                        //框架信息
                        frameInfo = new FrameGoodsInfo
                        {
                            GoodsId = goodsId,
                            TempleLength = int.Parse(txt_TempleLength.Text.Trim()),
                            Besiclometer = int.Parse(txt_Besiclometer.Text.Trim()),
                            FrameWithinWidth = int.Parse(txt_FrameWithinWidth.Text.Trim()),
                            NoseWidth = int.Parse(txt_NoseWidth.Text.Trim()),
                            OpticalVerticalHeight = int.Parse(txt_OpticalVerticalHeight.Text.Trim()),
                            EyeSize = int.Parse(txt_EyeSize.Text.Trim())
                        };
                        break;
                    #endregion

                    #region 商品药品信息
                    case GoodsKindType.Medicinal:
                    case GoodsKindType.PrescriptionMedicine:
                        goodsInfo.GoodsAuditState = (int)GoodsAuditState.PurchasingWaitAudit;
                        //商品药品信息
                        goodsMedicineInfo = new GoodsMedicineInfo
                        {
                            GoodsId = goodsId,
                            ChemistryName = txt_ChemistryName.Text.Trim(),
                            ChemistryNameFirstLetter = txt_ChemistryNameFirstLetter.Text.Trim(),
                            MedicineQualityType = int.Parse(ddl_MedicineQualityType.SelectedValue),
                            MedicineSaleKindType = int.Parse(ddl_MedicineSaleKindType.SelectedValue),
                            MedicineWholesalePrice = decimal.Parse(txt_MedicineWholesalePrice.Text.Trim()),
                            MedicineTaxRateType = int.Parse(ddl_MedicineTaxRateType.SelectedValue),
                            QualityStandardDescription = txt_QualityStandardDescription.Text,
                            MedicineDosageFormType = int.Parse(ddl_MedicineDosageFormType.SelectedValue),
                            MedicineStorageConditionType = int.Parse(ddl_MedicineStorageConditionType.SelectedValue),
                            MedicineLibraryManageType = int.Parse(ddl_LibraryManageType.SelectedValue),
                            MedicineCuringKindType = int.Parse(ddl_MedicineCuringKindType.SelectedValue),
                            MedicineCuringCycleType = int.Parse(ddl_MedicineCuringCycleType.SelectedValue),
                            MedicineStoreCounterType = int.Parse(ddl_MedicineStoreCounterType.SelectedValue)
                        };
                        break;
                    case GoodsKindType.HealthProducts:
                    case GoodsKindType.FamilyPlanningProuducts:
                    case GoodsKindType.CosmeticSkinCare:
                    case GoodsKindType.ChineseMedicineYinPian:
                    case GoodsKindType.DisinfectionTypeGoods:
                    case GoodsKindType.MedicalEquipment:
                        goodsInfo.GoodsAuditState = (int)GoodsAuditState.PurchasingWaitAudit;
                        break;
                        #endregion
                }
            }
            #endregion

            goodsInfo.GoodsId = goodsId;
            goodsInfo.GoodsName = txt_GoodsName.Text.Trim();
            goodsInfo.PurchaseNameFirstLetter = txt_PurchaseNameFirstLetter.Text.Trim();
            goodsInfo.BrandId = new Guid(rcb_Brand.SelectedValue);
            goodsInfo.SupplierGoodsCode = txt_SupplierGoodsCode.Text.Trim();
            goodsInfo.GoodsType = int.Parse(rcb_GoodsType.SelectedValue);
            goodsInfo.ClassId = new Guid(ddl_GoodsClass.SelectedValue);
            goodsInfo.Units = ddl_Units.SelectedItem.Text.Trim();
            goodsInfo.Specification = txt_Specification.Text.Trim();
            goodsInfo.SaleStockType = int.Parse(ddl_SaleStockType.SelectedValue);
            goodsInfo.StockStatus = txt_StockStatus.Text.Trim();
            goodsInfo.BarCode = txt_BarCode.Text.Trim();
            goodsInfo.MarketPrice = decimal.Parse(txt_MarketPrice.Text.Trim());
            goodsInfo.ShelfLife = txt_ShelfLife.Text.Trim();
            goodsInfo.IsImportedGoods = ckb_IsImportedGoods.Checked;
            goodsInfo.IsLuxury = ckb_IsLuxury.Checked;
            goodsInfo.IsBannedPurchase = ckb_IsBannedPurchase.Checked;
            goodsInfo.IsBannedSale = ckb_IsBannedSale.Checked;
            goodsInfo.IsLuxury = ckb_IsLuxury.Checked;
            goodsInfo.IsOnShelf = ckb_IsOnShelf.Checked;

            string filePath = UploadImage(UploadImg, UploadImgName, string.Empty, "../UserDir/AttrWordsImg/{0}");
            if (!string.IsNullOrEmpty(filePath))
            {
                goodsInfo.ImgPath = filePath;
            }

            goodsInfo.Weight = double.Parse(txt_Weight.Text.Trim());
            goodsInfo.IsStockScarcity = false;
            goodsInfo.GoodsAuditStateMemo = WebControl.RetrunUserAndTime("[【新增】：新增商品;]");

            //其它信息
            goodsInfo.ExpandInfo = expendInfo;
            goodsInfo.FrameGoodsInfo = frameInfo;
            goodsInfo.GoodsMedicineInfo = String.IsNullOrWhiteSpace(txt_ChemistryName.Text.Trim()) ? null : goodsMedicineInfo;
            goodsInfo.DictGoodsPurchase = new Dictionary<Guid, List<EntityPurchaseType>>();
            goodsInfo.SaleGroupList = new List<GoodsGroupInfo>();
            #endregion

            #region 关联公司采购组
            foreach (GridDataItem dataItem in RG_RelatedCompanyPurchasingGroup.Items)
            {
                var id = dataItem.GetDataKeyValue("ID");
                var ckbIsDirect = (CheckBox)dataItem.FindControl("ckb_IsDirect");
                var ckbIsJoin = (CheckBox)dataItem.FindControl("ckb_IsJoin");
                var ckbIsAlliance = (CheckBox)dataItem.FindControl("ckb_IsAlliance");

                var entityPurchaseTypeList = new List<EntityPurchaseType>();
                if (ckbIsDirect.Checked) entityPurchaseTypeList.Add(EntityPurchaseType.Direct);
                if (ckbIsJoin.Checked) entityPurchaseTypeList.Add(EntityPurchaseType.Join);
                if (ckbIsAlliance.Checked) entityPurchaseTypeList.Add(EntityPurchaseType.Alliance);
                if (entityPurchaseTypeList.Any())
                {
                    goodsInfo.DictGoodsPurchase.Add(new Guid(id.ToString()), entityPurchaseTypeList);
                }
            }
            #endregion

            #region 关联销售组平台
            foreach (ListItem item in ckb_RelatedSalesGroupPlatform.Items)
            {
                if (item.Selected)
                {
                    goodsInfo.SaleGroupList.Add(new GoodsGroupInfo { GroupId = new Guid(item.Value) });
                }
            }
            #endregion

            #region 子商品属性
            //光度属性
            if (ckb_LightFieldList.Items.Count > 0)
            {
                var lightFieldList = ckb_LightFieldList.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => Guid.Parse(item.Value)).ToList();
                fieldDict.Add(Guid.Parse(Hid_LightParentFieldId.Value), lightFieldList);
            }

            //散光属性
            if (ckb_AstigmatismFieldList.Items.Count > 0)
            {
                var astigmatismFieldList = ckb_AstigmatismFieldList.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => Guid.Parse(item.Value)).ToList();
                fieldDict.Add(Guid.Parse(Hid_AstigmatismParentFieldId.Value), astigmatismFieldList);
            }

            //轴位属性
            if (ckb_AxialViewFieldList.Items.Count > 0)
            {
                var axialViewFieldList = ckb_AxialViewFieldList.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => Guid.Parse(item.Value)).ToList();
                fieldDict.Add(Guid.Parse(Hid_AxialParentFieldId.Value), axialViewFieldList);
            }
            #endregion

            #region 商品资质
            var list = new List<GoodsQualificationDetailInfo>();
            foreach (var item in GoodsInformationInfoList)
            {
                list.Add(new GoodsQualificationDetailInfo
                {
                    GoodsID = goodsId,
                    GoodsQualificationType = item.QualificationType,
                    NameOrNo = item.Number,
                    Path = string.IsNullOrEmpty(item.Path)?string.Empty:item.Path,
                    ExtensionName = string.IsNullOrEmpty(item.ExtensionName) ? string.Empty : item.ExtensionName,
                    OverdueDate =item.OverdueDate ?? DateTime.MinValue,
                    LastOperationTime = DateTime.Now
                });
            }
            goodsInfo.GoodsQualificationDetailInfos = list;
            #endregion
        }
        #endregion

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        protected string UploadImage(FileUpload uploadImg, TextBox uploadImgName, string imagePath, string saveFolderPath)
        {
            //判断是否已经上传文件
            if (uploadImg.HasFile && !string.IsNullOrEmpty(uploadImgName.Text))
            {
                if (uploadImg.PostedFile != null && uploadImg.PostedFile.ContentLength > 0)
                {
                    #region 如果上传文件已经存在，则删除该文件
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(Server.MapPath(imagePath)))
                    {
                        File.Delete(Server.MapPath(imagePath));
                    }
                    #endregion

                    string ext = Path.GetExtension(uploadImg.PostedFile.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".jepg" || ext == ".bmp" || ext == ".gif" || ext == ".png")
                    {
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                        string folderPath = string.Format(saveFolderPath, DateTime.Now.ToString("yyyyMM") + "/");
                        if (!Directory.Exists(Server.MapPath(folderPath)))
                        {
                            Directory.CreateDirectory(Server.MapPath(folderPath));
                        }
                        string filePath = folderPath + fileName;
                        uploadImg.PostedFile.SaveAs(Server.MapPath(filePath));
                        return filePath;
                    }
                    MessageBox.AppendScript(this, "图片格式错误（.jpg|.jepg|.bmp|.gif|.png）！");
                }
                else
                {
                    MessageBox.AppendScript(this, "请选择一张图片！");
                }
            }
            return string.Empty;
        }

        //清除框架信息
        private void ClearFrameInfoData()
        {
            txt_TempleLength.Text =
            txt_Besiclometer.Text =
            txt_FrameWithinWidth.Text =
            txt_NoseWidth.Text =
            txt_OpticalVerticalHeight.Text =
            txt_EyeSize.Text = "0";
        }

        //清除药品信息
        private void ClearMedicamentData()
        {
            txt_ChemistryName.Text =
            txt_ChemistryNameFirstLetter.Text =
            txt_MedicineWholesalePrice.Text =
            txt_QualityStandardDescription.Text = string.Empty;

            ddl_MedicineQualityType.SelectedIndex =
            ddl_MedicineSaleKindType.SelectedIndex =
            ddl_MedicineTaxRateType.SelectedIndex =
            ddl_MedicineDosageFormType.SelectedIndex =
            ddl_MedicineStorageConditionType.SelectedIndex =
            ddl_LibraryManageType.SelectedIndex =
            ddl_MedicineCuringKindType.SelectedIndex =
            ddl_MedicineCuringCycleType.SelectedIndex =
            ddl_MedicineStoreCounterType.SelectedIndex = 0;
        }

        //验证数据
        protected string CheckData()
        {
            var errorMsg = new StringBuilder();
            #region 基本信息
            var goodsName = txt_GoodsName.Text;
            if (string.IsNullOrEmpty(goodsName))
            {
                errorMsg.Append("请填写“【基本信息】商品名称”！").Append("\\n");
            }
            else
            {
                int textLength = WebControl.GetTextLength(goodsName);
                if (textLength > 60)
                {
                    errorMsg.Append("“【基本信息】商品名称”长度超出范围！").Append("\\n");
                }
            }

            var goodsType = rcb_GoodsType.SelectedValue;
            if (string.IsNullOrEmpty(goodsType))
            {
                errorMsg.Append("请选择“【基本信息】商品类型”！").Append("\\n");
            }
            var goodsClass = ddl_GoodsClass.SelectedValue;
            if (string.IsNullOrEmpty(goodsClass))
            {
                errorMsg.Append("请选择“【基本信息】商品分类”！").Append("\\n");
            }
            var brand = rcb_Brand.SelectedValue;
            if (string.IsNullOrEmpty(brand))
            {
                errorMsg.Append("请选择“【基本信息】商品品牌”！").Append("\\n");
            }
            var marketPrice = txt_MarketPrice.Text;
            if (string.IsNullOrEmpty(marketPrice))
            {
                errorMsg.Append("请填写“【基本信息】市场价”！").Append("\\n");
            }
            var referencePrice = txt_ReferencePrice.Text;
            if (string.IsNullOrEmpty(referencePrice))
            {
                errorMsg.Append("请填写“【基本信息】参考价”！").Append("\\n");
            }
            #endregion

            #region 商品类型相关数据
            if (goodsType != null)
            {
                var goodsKindType = (GoodsKindType)int.Parse(goodsType);
                #region 基本信息
                if (goodsKindType.Equals(GoodsKindType.Medicinal) ||
                    goodsKindType.Equals(GoodsKindType.PrescriptionMedicine) ||
                    goodsKindType.Equals(GoodsKindType.HealthProducts) ||
                    goodsKindType.Equals(GoodsKindType.FamilyPlanningProuducts) ||
                    goodsKindType.Equals(GoodsKindType.MedicalEquipment))
                {
                    var goodsNameFirstLetter = txt_PurchaseNameFirstLetter.Text;
                    if (string.IsNullOrEmpty(goodsNameFirstLetter))
                    {
                        errorMsg.Append("请填写“【基本信息】商品首字母名称”！").Append("\\n");
                    }
                    var specification = txt_Specification.Text;
                    if (string.IsNullOrEmpty(specification))
                    {
                        errorMsg.Append("请填写“【基本信息】SKU”！").Append("\\n");
                    }
                    var shelfLife = txt_ShelfLife.Text;
                    if (string.IsNullOrEmpty(shelfLife))
                    {
                        errorMsg.Append("请填写“【基本信息】保质期”！").Append("\\n");
                    }
                }
                #endregion

                #region 验证药品相关数据
                if (goodsKindType.Equals(GoodsKindType.Medicinal) || goodsKindType.Equals(GoodsKindType.PrescriptionMedicine))
                {
                    var chemistryName = txt_ChemistryName.Text;
                    if (string.IsNullOrEmpty(chemistryName))
                    {
                        errorMsg.Append("请填写“【药品】商品化学名”！").Append("\\n");
                    }
                    var chemistryNameFirstLetter = txt_ChemistryNameFirstLetter.Text;
                    if (string.IsNullOrEmpty(chemistryNameFirstLetter))
                    {
                        errorMsg.Append("请填写“【药品】商品化学名首字母”！").Append("\\n");
                    }
                    var medicineQualityType = ddl_MedicineQualityType.SelectedValue;
                    if (medicineQualityType.Equals("-1"))
                    {
                        errorMsg.Append("请选择“【药品】质检分类”！").Append("\\n");
                    }
                    var medicineSaleKindType = ddl_MedicineSaleKindType.SelectedValue;
                    if (medicineSaleKindType.Equals("-1"))
                    {
                        errorMsg.Append("请选择“【药品】销售品种”！").Append("\\n");
                    }
                    var medicineWholesalePrice = txt_MedicineWholesalePrice.Text;
                    if (string.IsNullOrEmpty(medicineWholesalePrice))
                    {
                        errorMsg.Append("请填写“【药品】批发价”！").Append("\\n");
                    }
                    var medicineTaxRateType = ddl_MedicineTaxRateType.SelectedValue;
                    if (medicineTaxRateType.Equals("-1"))
                    {
                        errorMsg.Append("请选择“【药品】税率”！").Append("\\n");
                    }
                    var medicineDosageFormType = ddl_MedicineDosageFormType.SelectedValue;
                    if (medicineDosageFormType.Equals("-1"))
                    {
                        errorMsg.Append("请选择“【药品】剂型”！").Append("\\n");
                    }
                    var medicineStorageConditionType = ddl_MedicineStorageConditionType.SelectedValue;
                    if (medicineStorageConditionType.Equals("-1"))
                    {
                        errorMsg.Append("请选择“【药品】储存条件”！").Append("\\n");
                    }
                    var libraryManageType = ddl_LibraryManageType.SelectedValue;
                    if (libraryManageType.Equals("-1"))
                    {
                        errorMsg.Append("请选择“【药品】库位管理”！").Append("\\n");
                    }
                    var medicineCuringKindType = ddl_MedicineCuringKindType.SelectedValue;
                    if (medicineCuringKindType.Equals("-1"))
                    {
                        errorMsg.Append("请选择“【药品】养护类别”！").Append("\\n");
                    }
                    var medicineCuringCycleType = ddl_MedicineCuringCycleType.SelectedValue;
                    if (medicineCuringCycleType.Equals("-1"))
                    {
                        errorMsg.Append("请选择“【药品】养护周期”！").Append("\\n");
                    }
                    var medicineStoreCounterType = ddl_MedicineStoreCounterType.SelectedValue;
                    if (medicineStoreCounterType.Equals("-1"))
                    {
                        errorMsg.Append("请选择“【药品】门店柜台”！").Append("\\n");
                    }
                    var qualityStandardDescription = txt_QualityStandardDescription.Text;
                    if (string.IsNullOrEmpty(qualityStandardDescription))
                    {
                        errorMsg.Append("请填写“【药品】质量标准”！").Append("\\n");
                    }
                }
                #endregion
            }
            #endregion

            #region 商品资质
            if (goodsType != null && !string.IsNullOrEmpty(goodsClass))
            {
                var goodsKindType = (GoodsKindType)int.Parse(goodsType);
                if (goodsKindType.Equals(GoodsKindType.Medicinal) ||
                    goodsKindType.Equals(GoodsKindType.HealthProducts) ||
                    goodsKindType.Equals(GoodsKindType.MedicalEquipment) ||
                    goodsKindType.Equals(GoodsKindType.PrescriptionMedicine) ||
                    goodsKindType.Equals(GoodsKindType.ChineseMedicineYinPian)
                )
                {
                    if (GoodsInformationInfoList.Count > 0)
                    {
                        var productionUnitCount = GoodsInformationInfoList.Count(p => p.QualificationType.Equals((int)GoodsQualificationType.ProductionUnit));
                        var medicalDeviceRegistrationNumberCount = GoodsInformationInfoList.Count(p => p.QualificationType.Equals((int)GoodsQualificationType.MedicalDeviceRegistrationNumber));
                        if (productionUnitCount.Equals(0))
                        {
                            errorMsg.Append("请填写“【商品资质】生产厂家”资质！").Append("\\n");
                        }
                        if (medicalDeviceRegistrationNumberCount.Equals(0))
                        {
                            errorMsg.Append("请填写“【商品资质】注册证号/批准文号”资质！").Append("\\n");
                        }
                    }
                    else
                    {
                        errorMsg.Append("请填写“【商品资质】”！").Append("\\n");
                    }
                }
                foreach (var item in GoodsInformationInfoList)
                {
                    if (item.QualificationType == (int)GoodsQualificationType.ProductionUnit)
                    {
                        if (string.IsNullOrEmpty(item.Number))
                        {
                            errorMsg.Append("请填写“【商品资质】单位名称”！").Append("\\n");
                        }
                    }
                    else if (item.QualificationType == (int)GoodsQualificationType.ProductionPermitNo)
                    {
                        if (string.IsNullOrEmpty(item.Number))
                        {
                            errorMsg.Append("请填写“【商品资质】证书号码”！").Append("\\n");
                        }
                        if (item.OverdueDate == null)
                        {
                            errorMsg.Append("请填写“【商品资质】过期日期”！").Append("\\n");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.Number))
                        {
                            errorMsg.Append("请填写“【商品资质】证书号码”！").Append("\\n");
                        }
                        if (string.IsNullOrEmpty(item.Path))
                        {
                            errorMsg.Append("请上传“【商品资质】资质资料”！").Append("\\n");
                        }
                        if (item.OverdueDate == null)
                        {
                            errorMsg.Append("请填写“【商品资质】过期日期”！").Append("\\n");
                        }
                    }
                }
            }
            #endregion

            return errorMsg.ToString();
        }
    }

    public class PurchasingFilialeInfo
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsAll { get; set; }
        public bool IsDirect { get; set; }
        public bool IsJoin { get; set; }
        public bool IsAlliance { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using ERP.Environment;

namespace ERP.UI.Web.Windows
{
    public partial class GoodsInfomation : WindowsPage
    {
        readonly IGoodsCenterSao _goodsManager = new GoodsCenterSao();
        public static readonly string ResourceServerInformation = GlobalConfig.ResourceServerInformation;
        #region  页面加载
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["GoodsId"] == null)
                    return;
                GoodInfo = _goodsManager.GetGoodsBaseInfoById(GoodsId);
                if (GoodInfo == null)
                {
                    RAM.Alert("不是有效的商品信息");
                    RAM.ResponseScripts.Add("CancelWindow();");
                    return;
                }
                lb_goodsName.Text = GoodInfo.GoodsName;
                //获取资质类型列表
                var dict = EnumAttribute.GetDict<GoodsQualificationType>();
                var qualificationTypeList = dict.ToDictionary(item => string.Format("{0}", item.Key), item => item.Value);
                QualificationTypeList = qualificationTypeList;

                var list = _goodsManager.GetInformation(GoodsId);
                if (list.Count > 0)
                {
                    var dataList = new List<GoodsInformationInfo>();
                    foreach (var goodsInfomationInfo in list)
                    {
                        if (dataList.All(act => act.ID != goodsInfomationInfo.ID))
                        {
                            dataList.Add(goodsInfomationInfo);
                        }
                    }
                    InformationList = dataList;
                    RpGoodsInformations.DataBind();
                }
            }
        }
        #endregion

        #region  添加商品资料
        /// <summary>
        /// 添加资料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAddInformationClick(object sender, EventArgs e)
        {
            IList<GoodsInformationInfo> list = new List<GoodsInformationInfo>();
            var keys = EnumAttribute.GetDict<GoodsQualificationType>().Keys;
            foreach (RepeaterItem item in RpGoodsInformations.Items)
            {
                var hfPasth = (HiddenField)item.FindControl("HfPath");
                var upInfor = (RadUpload)item.FindControl("UploadInformation");
                var informationInfo = new GoodsInformationInfo
                {
                    ID = new Guid(((HiddenField)item.FindControl("HfId")).Value),
                    Number = ((TextBox)item.FindControl("TbInformationNumber")).Text,
                    QualificationType =
                        Convert.ToInt32(((DropDownList)item.FindControl("DDL_GoodsQualification")).SelectedValue)
                };
                if (list.Any(act => informationInfo.ID == act.ID)) continue;
                var selectedDate = ((RadDatePicker)item.FindControl("RDPOverdueDate")).SelectedDate;
                if (selectedDate != null)
                {
                    informationInfo.OverdueDate = selectedDate.Value;
                }
                informationInfo.UploadDate = DateTime.Now;
                if (!string.IsNullOrEmpty(hfPasth.Value) && upInfor.UploadedFiles.Count == 0)
                {
                    informationInfo.Path = hfPasth.Value;
                }
                else if (!string.IsNullOrEmpty(hfPasth.Value) && upInfor.UploadedFiles.Count > 0)
                {
                    UploadedFile uploadedFile = upInfor.UploadedFiles[0];
                    informationInfo.Path = uploadedFile.GetName();
                }
                else if (upInfor.UploadedFiles.Count > 0)
                {
                    UploadedFile uploadedFile = upInfor.UploadedFiles[0];
                    informationInfo.Path = uploadedFile.GetName();
                }
                informationInfo.Type = (int)InformationType.Goods;
                if (!string.IsNullOrEmpty(informationInfo.Path))
                {
                    informationInfo.ExtensionName = informationInfo.Path.Substring(informationInfo.Path.LastIndexOf('.') + 1);
                }
                if (((HiddenField)item.FindControl("HfIsNew")).Value == "1")
                {
                    informationInfo.IsNew = 1;
                }
                list.Add(informationInfo);
            }
            InformationList = list.OrderByDescending(w => w.UploadDate).ToList();
            var newInfo = new GoodsInformationInfo
            {
                ID = Guid.NewGuid(),
                UploadDate = DateTime.MinValue,
                OverdueDate = DateTime.MinValue,
                Path = string.Empty,
                IsNew = 1,
                QualificationType = keys.FirstOrDefault(key => InformationList.All(act => act.QualificationType != key))
            };
            InformationList.Add(newInfo);
            BtnSave.Visible = true;
            RpGoodsInformations.DataBind();
        }

        #endregion

        #region  保存资料
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSaveClick(object sender, EventArgs e)
        {
            string msg = Validator();
            if (String.IsNullOrWhiteSpace(msg))
            {
                IList<GoodsInformationInfo> infos = GetInformationList();
                if (infos.Any(act => act.IsNew == 1 && act.OverdueDate != null && act.OverdueDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)))
                {
                    RAM.Alert("新增资质中存在过期日期小于当前日期");
                    return;
                }
                string errorMsg;
                var result = _goodsManager.SetInformationList(GoodsId, infos, out errorMsg);
                if(result)
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                else
                {
                    RAM.Alert("保存失败!请完善以下必填信息：" + errorMsg);
                }
            }
            else
            {
                RAM.Alert(msg);
            }
        }
        #endregion

        public bool IsComplate(IList<SupplierInformationInfo> supplierInformationInfoList)
        {
            var has1 = false;
            var has2 = false;
            var has3 = false;
            foreach (var item in supplierInformationInfoList)
            {
                if (item.Number != string.Empty && item.Path != string.Empty &&
                    item.OverdueDate >= DateTime.Now)
                {
                    if (item.QualificationType == (int)SupplierQualificationType.BusinessLicense)
                    {
                        has1 = true;
                    }
                    if (item.QualificationType == (int)SupplierQualificationType.OrganizationCodeCertificate)
                    {
                        has2 = true;
                    }
                    if (item.QualificationType == (int)SupplierQualificationType.TaxRegistrationCertificate)
                    {
                        has3 = true;
                    }
                }
                if (has1 && has2 && has3)
                {
                    return true;
                }
            }
            return false;
        }

        protected void GoodsQualification_Change(object sender, EventArgs e)
        {
            var lb_RedSign = ((DropDownList)sender).Parent.FindControl("lb_RedSign");
            var lbNumber = ((DropDownList)sender).Parent.FindControl("lb_Number");
            var lbQual = ((DropDownList)sender).Parent.FindControl("lb_Qual");

            var uploadInformation = ((DropDownList)sender).Parent.FindControl("UploadInformation");
            var lbOverdueDate = ((DropDownList)sender).Parent.FindControl("lb_OverdueDate");
            var rdpOverdueDate = ((DropDownList)sender).Parent.FindControl("RDPOverdueDate");

            ((Label)lb_RedSign).Visible = false;

            var ddlGoodsInformation = (DropDownList)sender;
            int ddlGoodsInformationVal = Convert.ToInt32(ddlGoodsInformation.SelectedItem.Value);

            if (ddlGoodsInformationVal == (int)GoodsQualificationType.ProductionUnit
                || ddlGoodsInformationVal == (int)GoodsQualificationType.MedicalDeviceRegistrationNumber
                || ddlGoodsInformationVal == (int)GoodsQualificationType.InspectionReport)
            {
                ((Label)lb_RedSign).Visible = true;

            }

            if (ddlGoodsInformationVal == (int)GoodsQualificationType.ProductionUnit)
            {

                ((Label)lbNumber).Text = "单位名称：";

                lbQual.Visible = false;
                uploadInformation.Visible = false;
                lbOverdueDate.Visible = false;
                rdpOverdueDate.Visible = false;
            }
            else if (ddlGoodsInformationVal == (int)GoodsQualificationType.ProductionPermitNo)
            {
                ((Label)lbNumber).Text = "证书号码：";
                lbQual.Visible = false;
                uploadInformation.Visible = false;
                lbOverdueDate.Visible = true;
                rdpOverdueDate.Visible = true;
            }
            else
            {
                ((Label)lbNumber).Text = "证书号码：";
                (lbQual).Visible = true;
                uploadInformation.Visible = true;
                (lbOverdueDate).Visible = true;
                rdpOverdueDate.Visible = true;
            }
        }

        #region  获取商品资质
        private IList<GoodsInformationInfo> GetInformationList()
        {
            IList<GoodsInformationInfo> list = new List<GoodsInformationInfo>();
            foreach (RepeaterItem item in RpGoodsInformations.Items)
            {
                var hfPasth = (HiddenField)item.FindControl("HfPath");
                var upInfor = (RadUpload)item.FindControl("UploadInformation");
                var number = ((TextBox)item.FindControl("TbInformationNumber")).Text;
                var qualificationType = Convert.ToInt32(((DropDownList)item.FindControl("DDL_GoodsQualification")).SelectedValue);
                var informationInfo = new GoodsInformationInfo
                {
                    ID = new Guid(((HiddenField)item.FindControl("HfId")).Value),
                    Number = number,
                    IdentifyId = GoodInfo.GoodsId,
                    IdentifyName = GoodInfo.GoodsName,
                    QualificationType = qualificationType,
                    FilialeId = Guid.Empty
                };
                if (list.Any(act => act.Number == number && act.QualificationType == qualificationType))
                {
                    continue;
                }
                if (((HiddenField)item.FindControl("HfIsNew")).Value == "1")
                {
                    informationInfo.IsNew = 1;
                }
                if (list.Any(act => informationInfo.ID == act.ID)) continue;
                var selectedDate = ((RadDatePicker)item.FindControl("RDPOverdueDate")).SelectedDate;
                if (selectedDate != null)
                {
                    informationInfo.OverdueDate = selectedDate.Value;
                }
                informationInfo.UploadDate = DateTime.Now;
                if (!string.IsNullOrEmpty(hfPasth.Value) && upInfor.UploadedFiles.Count == 0)
                {
                    informationInfo.Path = hfPasth.Value;
                }
                else if (!string.IsNullOrEmpty(hfPasth.Value) && upInfor.UploadedFiles.Count > 0)
                {
                    informationInfo.Path = GetUploadPath(upInfor, hfPasth);
                }
                else if (upInfor.UploadedFiles.Count > 0)
                {
                    informationInfo.Path = GetUploadPath(upInfor, hfPasth);
                }
                informationInfo.Type = (int)InformationType.CompanyCussent;
                if (!string.IsNullOrEmpty(informationInfo.Path))
                {
                    informationInfo.ExtensionName = informationInfo.Path.Substring(informationInfo.Path.LastIndexOf('.') + 1);
                }
                list.Add(informationInfo);
            }
            return list;
        }
        #endregion

        #region  绑定往来单位资料
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RpGoodsInformationsOnDataBinding(object sender, EventArgs e)
        {
            RpGoodsInformations.DataSource = InformationList.OrderByDescending(ent => ent.UploadDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>   
        /// <param name="e"></param>
        protected void RpGoodsInformationsItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            //绑定资质类型下拉框
            var ddlGoodsQualification = (DropDownList)e.Item.FindControl("DDL_GoodsQualification");
            ddlGoodsQualification.DataSource = QualificationTypeList;
            ddlGoodsQualification.DataTextField = "value";
            ddlGoodsQualification.DataValueField = "key";
            ddlGoodsQualification.DataBind();

            //判断是否是新增资质，不是新增资质，资质下拉框只读
            var hidGoodsQualification = (HiddenField)e.Item.FindControl("HfGoodsQualification");
            var isNew = (HiddenField)e.Item.FindControl("HfIsNew");
            ddlGoodsQualification.Enabled = isNew.Value == "1";
            if (hidGoodsQualification.Value.Trim() != "0")
            {
                ddlGoodsQualification.SelectedValue = hidGoodsQualification.Value.Trim();
            }

            var ddlGoodsInformationVal = Convert.ToInt32(ddlGoodsQualification.SelectedItem.Value);
            if (ddlGoodsInformationVal == (int)GoodsQualificationType.ProductionUnit
                || ddlGoodsInformationVal == (int)GoodsQualificationType.MedicalDeviceRegistrationNumber
                || ddlGoodsInformationVal == (int)GoodsQualificationType.InspectionReport)
            {
                ((Label)e.Item.FindControl("lb_RedSign")).Visible = true;
            }


            //资质名称为生产单位
            if (Convert.ToInt32(ddlGoodsQualification.SelectedItem.Value) == (int)GoodsQualificationType.ProductionUnit)
            {
                var lbNumber = (Label)e.Item.FindControl("lb_Number");
                lbNumber.Text = "单位名称：";
                var lbQual = (Label)e.Item.FindControl("lb_Qual");
                lbQual.Visible = false;
                var upInformation = (RadUpload)e.Item.FindControl("UploadInformation");
                upInformation.Visible = false;
                var lbOverdueDate = (Label)e.Item.FindControl("lb_OverdueDate");
                lbOverdueDate.Visible = false;
                var rdfOverdueDate = (RadDatePicker)e.Item.FindControl("RDPOverdueDate");
                rdfOverdueDate.Visible = false;
                var lbState = (Label)e.Item.FindControl("lb_state");
                var hidNumber = (HiddenField)e.Item.FindControl("HfNumber");
                if (hidNumber.Value.Trim().Length == 0)
                {
                    lbState.Text = "缺失";
                    lbState.ForeColor = Color.Red;
                }
                else
                {
                    lbState.Text = "正常";
                    lbState.ForeColor = Color.Black;
                }
            }
            else
            {
                if (Convert.ToInt32(ddlGoodsQualification.SelectedItem.Value) ==
                     (int)GoodsQualificationType.ProductionPermitNo) //资质名称为生产许可证号
                {
                    var lbQual = (Label)e.Item.FindControl("lb_Qual");
                    lbQual.Visible = false;
                    var upInformation = (RadUpload)e.Item.FindControl("UploadInformation");
                    upInformation.Visible = false;
                }
                var rdfOverdueDate = (RadDatePicker)e.Item.FindControl("RDPOverdueDate");
                rdfOverdueDate.MinDate = DateTime.MinValue;
                rdfOverdueDate.MaxDate = DateTime.MaxValue;
                var hidOverdueDate = (HiddenField)e.Item.FindControl("HfOverdueDate");
                var overdueDate = hidOverdueDate.Value == string.Empty
                    ? DateTime.MinValue
                    : DateTime.Parse(hidOverdueDate.Value);

                var lbState = (Label)e.Item.FindControl("lb_state");
                lbState.Text = "正常";
                lbState.ForeColor = Color.Black;

                if (overdueDate != DateTime.MinValue && overdueDate != (DateTime)SqlDateTime.MinValue)
                {
                    rdfOverdueDate.SelectedDate = overdueDate;
                    if (overdueDate < DateTime.Now)
                    {
                        lbState.Text = "过期";
                        lbState.ForeColor = Color.Red;
                    }
                }
                if (overdueDate != DateTime.MinValue && overdueDate != (DateTime)SqlDateTime.MinValue)
                {
                    rdfOverdueDate.SelectedDate = overdueDate;
                    if (overdueDate >= DateTime.Now && overdueDate <= DateTime.Now.AddMonths(1))
                    {
                        lbState.Text = "快过期";
                        lbState.ForeColor = Color.Red;
                    }
                }
                var hidNumber = (HiddenField)e.Item.FindControl("HfNumber");
                var hidPath = (HiddenField)e.Item.FindControl("HfPath");

                if (Convert.ToInt32(ddlGoodsQualification.SelectedItem.Value) ==
                    (int)GoodsQualificationType.ProductionPermitNo)
                {
                    if (hidNumber.Value == string.Empty)
                    {
                        lbState.Text = "缺失";
                        lbState.ForeColor = Color.Red;
                    }
                }
                else
                {
                    if (hidNumber.Value == string.Empty || hidPath.Value == string.Empty)
                    {
                        lbState.Text = "缺失";
                        lbState.ForeColor = Color.Red;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RpGoodsInformationsOnItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteInformation")
            {
                List<Guid> dList = DeleteList;
                var hfId = (HiddenField)e.Item.FindControl("HfId");
                string id = hfId.Value;
                if (((HiddenField)e.Item.FindControl("HfGoodsQualification")).Value != "0")
                {
                    dList.Add(new Guid(id));
                    DeleteList = dList;
                }
                IList<GoodsInformationInfo> list = InformationList.Where(w => w.ID != new Guid(id)).ToList();
                InformationList = list;
                RpGoodsInformations.DataBind();
            }
        }
        #endregion

        #region[保存商品资料文件]
        /// <summary>
        /// 资料保存
        /// </summary>
        /// <param name="radUpload"></param>
        /// <param name="hfPath"></param>
        /// <returns></returns>
        private string GetUploadPath(RadUpload radUpload, HiddenField hfPath)
        {
            Thread.Sleep(1);
            if (radUpload.UploadedFiles.Count > 0)
            {
                // 每个月创建一个存放图片的目录
                var upLoadedFile = radUpload.UploadedFiles[0];
                var yearMonth = DateTime.Now.ToString("yyyyMM") + "/";
                var time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var subPath = yearMonth;
                string folderPath = String.Format("{0}/GoodsInfomation/Company", Server.MapPath("/UserDir"));
                var filePath = String.Format("{0}/{1}", folderPath, subPath);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var fileName = time + upLoadedFile.GetExtension();

                upLoadedFile.SaveAs(filePath + fileName);
                hfPath.Value = "~/UserDir/GoodsInfomation/Company/" + subPath + fileName;
            }
            return hfPath.Value;
        }
        #endregion

        #region[验证相关属性]

        /// <summary>
        /// 验证相关属性
        /// </summary>
        public String Validator()
        {
            //只针对于药品
            if (GoodInfo.GoodsType == 12 || GoodInfo.GoodsType == 17 || GoodInfo.GoodsType == 18 || GoodInfo.GoodsType == 13 || GoodInfo.GoodsType == 16)
            {
                var val = String.Empty;
                if (RpGoodsInformations.Items.Count == 0)
                {
                    if (Request.QueryString["GoodsId"] != null)
                    {
                        return String.Empty;
                    }
                    RAM.Alert("资质材料为空！");
                    return String.Empty;
                }

                List<string> listErrorMsg = new List<string>();
                String listErrorMsgNotEmpty = String.Empty;
                String msg = String.Empty;

                bool IsHas_ProductionUnit = false;
                bool IsHas_MedicalDeviceRegistrationNumber = false;
                bool IsHas_InspectionReport = false;

                foreach (RepeaterItem item in RpGoodsInformations.Items)
                {
                    var tbInformationNumber = (TextBox)item.FindControl("TbInformationNumber");
                    var upInformation = (RadUpload)item.FindControl("UploadInformation");
                    var rdpOver = (RadDatePicker)item.FindControl("RDPOverdueDate");
                    var hfPath = (HiddenField)item.FindControl("HfPath");
                    var type = (DropDownList)item.FindControl("DDL_GoodsQualification");

                    if (!string.IsNullOrEmpty(type.SelectedValue))
                    {
                        var temp = (GoodsQualificationType)(int.Parse(type.SelectedValue));
                        switch (temp)
                        {
                            case GoodsQualificationType.ProductionUnit:
                                IsHas_ProductionUnit = true;
                                break;
                            case GoodsQualificationType.MedicalDeviceRegistrationNumber:
                                IsHas_MedicalDeviceRegistrationNumber = true;
                                break;
                            case GoodsQualificationType.InspectionReport:
                                IsHas_InspectionReport = true;
                                break;

                            default:
                                break;
                        }
                    }

                    if (type.SelectedValue == string.Format("{0}", (int)GoodsQualificationType.ProductionUnit))
                    {
                        if (tbInformationNumber.Text.Trim() == string.Empty)
                        {
                            listErrorMsgNotEmpty += "单位名称，";
                        }
                    }
                    else
                    {
                        if (tbInformationNumber.Text.Trim() == string.Empty)
                        {
                            listErrorMsgNotEmpty += "证书号码，";
                        }
                    }


                    if (type.SelectedValue != string.Format("{0}", (int)GoodsQualificationType.ProductionUnit)
                    && type.SelectedValue != string.Format("{0}", (int)GoodsQualificationType.ProductionPermitNo))
                    {
                        //if (string.IsNullOrEmpty(hfPath.Value) && upInformation.UploadedFiles.Count == 0)
                        //{
                        //    listErrorMsgNotEmpty += "资质资料的上传，";
                        //}
                    }
                    if (type.SelectedValue != string.Format("{0}", (int)GoodsQualificationType.ProductionUnit))
                    {
                        if (rdpOver.SelectedDate == null)
                        {
                            listErrorMsgNotEmpty += "过期日期，";
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(listErrorMsgNotEmpty.Trim('，')))
                    {
                        msg += type.SelectedItem.Text + "：" + listErrorMsgNotEmpty.Trim('，') + "；";
                    }
                    listErrorMsgNotEmpty = string.Empty;
                }

                if (!IsHas_ProductionUnit)
                {
                    listErrorMsg.Add("生产厂家的证书号码,过期日期；");
                }
                if (!IsHas_MedicalDeviceRegistrationNumber)
                {
                    listErrorMsg.Add("注册证号/批准文号的证书号码,过期日期；");
                }

                if (listErrorMsg.Count > 0)
                {
                    val = string.Join("、", listErrorMsg.ToArray());
                }

                if (!String.IsNullOrWhiteSpace(msg))
                {
                    val += msg.Trim(',');
                }

                if (!String.IsNullOrWhiteSpace(val))
                    return "请完善以下必填信息：" + val;
                else
                    return String.Empty;
            }
            return String.Empty;
        }

        #endregion

        #region  公用属性
        /// <summary>
        /// 商品资料列表
        /// </summary>
        public IList<GoodsInformationInfo> InformationList
        {
            get
            {
                if (ViewState["InformationList"] == null)
                {
                    return new List<GoodsInformationInfo>();
                }
                return (IList<GoodsInformationInfo>)ViewState["InformationList"];
            }
            set { ViewState["InformationList"] = value; }
        }

        /// <summary>
        /// 资质类型
        /// </summary>
        public Dictionary<String, String> QualificationTypeList
        {
            get
            {
                if (ViewState["QualificationTypeList"] == null)
                {
                    return new Dictionary<String, String>();
                }
                return (Dictionary<String, String>)ViewState["QualificationTypeList"];
            }
            set { ViewState["QualificationTypeList"] = value; }
        }

        /// <summary>
        /// 公司列表
        /// </summary>
        public IList<FilialeInfo> FilialeList
        {
            get
            {
                if (ViewState["FilialeList"] == null)
                {
                    return new List<FilialeInfo>();
                }
                return (List<FilialeInfo>)ViewState["FilialeList"];
            }
            set { ViewState["FilialeList"] = value; }
        }

        public List<Guid> DeleteList
        {
            get
            {
                if (ViewState["DeleteList"] == null)
                {
                    return new List<Guid>();
                }
                return (List<Guid>)ViewState["DeleteList"];
            }
            set { ViewState["DeleteList"] = value; }
        }
        /// <summary>
        /// 商品Id
        /// </summary>
        protected Guid GoodsId
        {
            get
            {
                return new Guid(Request.QueryString["GoodsId"]);
            }
        }

        /// <summary>
        /// 往来单位信息
        /// </summary>
        protected GoodsInfo GoodInfo
        {
            get
            {
                if (ViewState["GoodsInfo"] == null)
                {
                    return _goodsManager.GetGoodsBaseInfoById(GoodsId);
                }
                return (GoodsInfo)ViewState["GoodsInfo"];
            }
            set { ViewState["GoodsInfo"] = value; }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class CompanyInfomation : WindowsPage
    {
        static readonly IQualificationManager _qualificationManager = new QualificationManager(GlobalConfig.DB.FromType.Write);

        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);

        readonly int _days = GlobalConfig.Expire;

        public static readonly string ResourceServerInformation = GlobalConfig.ResourceServerInformation;
        #region  页面加载
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["CompanyId"] == null)
                    return;
                CompanyInfo = _companyCussent.GetCompanyCussent(CompanyId);
                if (CompanyInfo == null)
                {
                    RAM.Alert("不是有效的往来单位信息");
                    RAM.ResponseScripts.Add("CancelWindow();");
                    return;
                }
                lb_supplierName.Text = CompanyInfo.CompanyName;
                //获取资质类型列表
                var dict = EnumAttribute.GetDict<SupplierQualificationType>();
                var qualificationTypeList = dict.ToDictionary(item => string.Format("{0}", item.Key), item => item.Value);
                QualificationTypeList = qualificationTypeList;
                //获取分公司列表
                FilialeList = CacheCollection.Filiale.GetHeadList();
                var list = _qualificationManager.GetSupplierQualificationBySupplierId(CompanyId).ToList();
                if (list.Count > 0)
                {
                    var dataList = new List<SupplierInformationInfo>();
                    foreach (var supplierInfomationInfo in list)
                    {
                        if (dataList.All(act => act.ID != supplierInfomationInfo.ID))
                        {
                            dataList.Add(supplierInfomationInfo);
                        }
                    }
                    InformationList = dataList;
                    RpCompanyInformations.DataBind();
                }
            }
        }
        #endregion

        #region  添加往来单位资料
        /// <summary>
        /// 添加资料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAddInformationClick(object sender, EventArgs e)
        {
            IList<SupplierInformationInfo> list = new List<SupplierInformationInfo>();
            var keys = EnumAttribute.GetDict<SupplierQualificationType>().Keys;
            foreach (RepeaterItem item in RpCompanyInformations.Items)
            {
                var hfPasth = (HiddenField)item.FindControl("HfPath");
                var upInfor = (RadUpload)item.FindControl("UploadInformation");
                var informationInfo = new SupplierInformationInfo
                {
                    ID = new Guid(((HiddenField)item.FindControl("HfId")).Value),
                    Number = ((TextBox)item.FindControl("TbInformationNumber")).Text,
                    QualificationType =
                        Convert.ToInt32(((DropDownList)item.FindControl("DDL_SupplierQualification")).SelectedValue)
                };
                if (Convert.ToInt32(((DropDownList)item.FindControl("DDL_SupplierQualification")).SelectedValue) == (int)SupplierQualificationType.GoodsSalesAuthorization)
                {
                    informationInfo.FilialeID = new Guid(((DropDownList)item.FindControl("DDL_Filiale")).SelectedValue);
                }

                if (list.Any(act => informationInfo.ID == act.ID)) continue;
                var selectedDate = ((RadDatePicker)item.FindControl("RDPOverdueDate")).SelectedDate;
                if (selectedDate != null)
                {
                    informationInfo.OverdueDate = selectedDate.Value;
                }
                informationInfo.UploadDate = DateTime.Now;
                if (upInfor.UploadedFiles.Count > 0)
                {
                    UploadedFile uploadedFile = upInfor.UploadedFiles[0];
                    informationInfo.Path = uploadedFile.GetName();
                }
                else
                {
                    if (!string.IsNullOrEmpty(hfPasth.Value))
                    {
                        informationInfo.Path = hfPasth.Value;
                    }
                }
                informationInfo.Type = (int)InformationType.CompanyCussent;
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
            var newInfo = new SupplierInformationInfo
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
            RpCompanyInformations.DataBind();
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
            if (Validator())
            {
                IList<SupplierInformationInfo> infos = GetInformationList();
                if (infos.Count > 0)
                {
                    if (infos.Any(item => string.IsNullOrEmpty(item.Number) || string.IsNullOrEmpty(item.Path)
                    || item.OverdueDate == null || item.OverdueDate == DateTime.MinValue))
                    {
                        RAM.Alert("供应商资质证书号码和资料不能为空、过期日期未选择!");
                        return;
                    }
                    if (infos.Any(item => item.IsNew == 1 && item.OverdueDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)))
                    {
                        RAM.Alert("新增供应商资质过期时间小于当前日期!");
                        return;
                    }
                }

                foreach (var item in infos)
                {
                    if (item.IsNew == 1)
                    {
                        _qualificationManager.Insert(item);
                    }
                    else
                    {
                        _qualificationManager.Update(item);
                    }
                }
                foreach (var infoId in DeleteList)
                {
                    _qualificationManager.Delete(infoId);
                }

                var list = _qualificationManager.GetSupplierQualificationBySupplierId(CompanyId).ToList();
                var qualificationManager = new BLL.Implement.Company.QualificationManager();
                var complete = qualificationManager.IsComplete(list);
                var companyInfo = _companyCussent.GetCompanyCussent(CompanyId);
                var expire = qualificationManager.IsExpire(list, _days);
                if (companyInfo != null && (companyInfo.Complete != complete || expire != companyInfo.Expire))
                {
                    _companyCussent.UpdateQualificationCompleteState(CompanyId, complete, expire);
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }
        #endregion

        protected void SupplierQualification_Change(object sender, EventArgs e)
        {
            var ddlSupplierInformation = (DropDownList)sender;
            if (ddlSupplierInformation == null)
            {
                return;
            }

            int ddlSupplierInformationVal = Convert.ToInt32(ddlSupplierInformation.SelectedItem.Value);

            var ddlFiliale = ddlSupplierInformation.Parent.FindControl("DDL_Filiale");
            ddlFiliale.Visible = ddlSupplierInformationVal == (int)SupplierQualificationType.GoodsSalesAuthorization;


            var lb_RedSign = ddlSupplierInformation.Parent.FindControl("lb_RedSign");

            ((Label)lb_RedSign).Visible = false;
            if (ddlSupplierInformationVal == (int)SupplierQualificationType.BusinessLicense)
            {
                ((Label)lb_RedSign).Visible = true;

            }

        }

        #region  获取往来单位资料
        private IList<SupplierInformationInfo> GetInformationList()
        {
            var list = new List<SupplierInformationInfo>();
            foreach (RepeaterItem item in RpCompanyInformations.Items)
            {
                var hfPasth = (HiddenField)item.FindControl("HfPath");
                var upInfor = (RadUpload)item.FindControl("UploadInformation");
                var number = ((TextBox)item.FindControl("TbInformationNumber")).Text;
                var qualificationType =
                    Convert.ToInt32(((DropDownList)item.FindControl("DDL_SupplierQualification")).SelectedValue);
                var informationInfo = new SupplierInformationInfo
                {
                    ID = new Guid(((HiddenField)item.FindControl("HfId")).Value),
                    Number = number,
                    IdentifyId = CompanyInfo.CompanyId,
                    QualificationType = qualificationType,
                    FilialeID = Guid.Empty
                };
                if (list.Any(act => informationInfo.ID == act.ID)) continue;
                if (list.Any(act => act.Number == number && act.QualificationType == qualificationType))
                {
                    continue;
                }
                if (Convert.ToInt32(((DropDownList)item.FindControl("DDL_SupplierQualification")).SelectedValue) == (int)SupplierQualificationType.GoodsSalesAuthorization)
                {
                    informationInfo.FilialeID = new Guid(((DropDownList)item.FindControl("DDL_Filiale")).SelectedValue);
                }
                informationInfo.IsNew = Convert.ToInt32(((HiddenField)item.FindControl("HfIsNew")).Value);
                var selectedDate = ((RadDatePicker)item.FindControl("RDPOverdueDate")).SelectedDate;
                if (selectedDate != null)
                {
                    informationInfo.OverdueDate = selectedDate.Value;
                }
                informationInfo.UploadDate = DateTime.Now;
                if (upInfor.UploadedFiles.Count > 0)
                {
                    informationInfo.Path = GetUploadPath(upInfor, hfPasth);
                }
                else
                {
                    if (!string.IsNullOrEmpty(hfPasth.Value))
                    {
                        informationInfo.Path = hfPasth.Value;
                    }
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
        protected void RpCompanyInformationsOnDataBinding(object sender, EventArgs e)
        {
            RpCompanyInformations.DataSource = InformationList.OrderByDescending(ent => ent.UploadDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>   
        /// <param name="e"></param>
        protected void RpCompanyInformationsItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            //绑定资质类型下拉框
            var ddlSupplierQualification = (DropDownList)e.Item.FindControl("DDL_SupplierQualification");
            ddlSupplierQualification.DataSource = QualificationTypeList;
            ddlSupplierQualification.DataTextField = "value";
            ddlSupplierQualification.DataValueField = "key";
            ddlSupplierQualification.DataBind();

            //绑定分公司下拉框
            var ddlFiliale = (DropDownList)e.Item.FindControl("DDL_Filiale");
            ddlFiliale.DataSource = FilialeList;
            ddlFiliale.DataTextField = "Name";
            ddlFiliale.DataValueField = "ID";
            ddlFiliale.DataBind();

            //判断是否是新增资质，不是新增资质，资质下拉框只读
            var hidSupplierQualification = (HiddenField)e.Item.FindControl("HfSupplierQualification");
            var isNew = (HiddenField)e.Item.FindControl("HfIsNew");
            ddlSupplierQualification.Enabled = isNew.Value == "1";
            if (hidSupplierQualification.Value.Trim() != "0")
            {
                ddlSupplierQualification.SelectedValue = hidSupplierQualification.Value.Trim();
                if (Convert.ToInt32(hidSupplierQualification.Value.Trim()) ==
                    (int)SupplierQualificationType.GoodsSalesAuthorization)
                {
                    ddlFiliale.SelectedValue = ((HiddenField)e.Item.FindControl("HfFilialeId")).Value;
                    ddlFiliale.Enabled = true;
                }
                else
                {
                    ddlFiliale.Visible = false;
                }
            }
            else
            {
                if (Convert.ToInt32(hidSupplierQualification.Value.Trim()) !=
                    (int)SupplierQualificationType.GoodsSalesAuthorization)
                {
                    ddlFiliale.Visible = false;
                }
            }

            var rdfOverdueDate = (RadDatePicker)e.Item.FindControl("RDPOverdueDate");
            var hidOverdueDate = (HiddenField)e.Item.FindControl("HfOverdueDate");
            var overdueDate = hidOverdueDate.Value == string.Empty ? DateTime.MinValue : DateTime.Parse(hidOverdueDate.Value);

            var lbState = (Label)e.Item.FindControl("lb_state");
            lbState.Text = "正常";
            lbState.ForeColor = Color.Black;

            if (overdueDate != DateTime.MinValue && overdueDate != (DateTime)SqlDateTime.MinValue)
            {
                rdfOverdueDate.SelectedDate = overdueDate;
                var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                if (overdueDate < now)
                {
                    lbState.Text = "过期";
                    lbState.ForeColor = Color.Red;
                }
                else if (overdueDate >= now && now.AddDays(_days) >= overdueDate)
                {
                    lbState.Text = "快过期";
                    lbState.ForeColor = Color.Red;
                }
            }
            var hidNumber = (HiddenField)e.Item.FindControl("HfNumber");
            var hidPath = (HiddenField)e.Item.FindControl("HfPath");
            if (hidNumber.Value == string.Empty || hidPath.Value == string.Empty)
            {
                lbState.Text = "缺失";
                lbState.ForeColor = Color.Red;
            }


            var ddlGoodsInformationVal = Convert.ToInt32(ddlSupplierQualification.SelectedItem.Value);
            if (ddlGoodsInformationVal == (int)SupplierQualificationType.BusinessLicense)
            {
                ((Label)e.Item.FindControl("lb_RedSign")).Visible = true;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RpCompanyInformationsOnItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteInformation")
            {
                List<Guid> dList = DeleteList;
                var hfId = (HiddenField)e.Item.FindControl("HfId");
                string id = hfId.Value;
                if (((HiddenField)e.Item.FindControl("HfSupplierQualification")).Value != "0")
                {
                    dList.Add(new Guid(id));
                    DeleteList = dList;
                }
                IList<SupplierInformationInfo> list = InformationList.Where(w => w.ID != new Guid(id)).ToList();
                InformationList = list;
                RpCompanyInformations.DataBind();
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
        public bool Validator()
        {
            if (RpCompanyInformations.Items.Count == 0)
            {
                if (Request.QueryString["CompanyId"] == null)
                {
                    return false;
                }
                var originalList = _qualificationManager.GetSupplierQualificationBySupplierId(CompanyId).ToList();
                if (originalList.Count == 0)
                {
                    RAM.Alert("资质材料为空！");
                    return false;
                }
            }

            var val = true;

            List<string> listErrorMsgNotEmpty = new List<string>();
            bool IsHas_BusinessLicense = false;

            foreach (RepeaterItem item in RpCompanyInformations.Items)
            {

                var type = (DropDownList)item.FindControl("DDL_SupplierQualification");
                if (!string.IsNullOrEmpty(type.SelectedValue))
                {
                    if ((SupplierQualificationType)(int.Parse(type.SelectedValue)) == SupplierQualificationType.BusinessLicense)
                    {
                        IsHas_BusinessLicense = true;
                    }
                }

                var tbInformationNumber = (TextBox)item.FindControl("TbInformationNumber");
                var upInformation = (RadUpload)item.FindControl("UploadInformation");
                var rdpOver = (RadDatePicker)item.FindControl("RDPOverdueDate");
                var hfPath = (HiddenField)item.FindControl("HfPath");
                if (tbInformationNumber.Text.Trim() == string.Empty)
                {
                    listErrorMsgNotEmpty.Add("请填写证书号码！");
                }
                if (string.IsNullOrEmpty(hfPath.Value) && upInformation.UploadedFiles.Count == 0)
                {
                    listErrorMsgNotEmpty.Add("请上传单位资料！");
                }
                if (rdpOver.SelectedDate == null)
                {
                    listErrorMsgNotEmpty.Add("请选择过期日期！");
                }
            }

            if (!IsHas_BusinessLicense)
            {
                RAM.Alert("请添加商品资质：营业执照！");
                val = false;
            }

            if (listErrorMsgNotEmpty.Count > 0)
            {
                RAM.Alert(string.Join("", listErrorMsgNotEmpty.ToArray()));
                val = false;
            }
            return val;
        }

        #endregion

        #region  公用属性
        /// <summary>
        /// 商品资料列表
        /// </summary>
        public IList<SupplierInformationInfo> InformationList
        {
            get
            {
                if (ViewState["InformationList"] == null)
                {
                    return new List<SupplierInformationInfo>();
                }
                return (IList<SupplierInformationInfo>)ViewState["InformationList"];
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
        /// 往来单位Id
        /// </summary>
        protected Guid CompanyId
        {
            get
            {
                return new Guid(Request.QueryString["CompanyId"]);
            }
        }

        /// <summary>
        /// 往来单位信息
        /// </summary>
        protected CompanyCussentInfo CompanyInfo
        {
            get
            {
                if (ViewState["CompanyInfo"] == null)
                {
                    return _companyCussent.GetCompanyCussent(CompanyId);
                }
                return (CompanyCussentInfo)ViewState["CompanyInfo"];
            }
            set { ViewState["CompanyInfo"] = value; }
        }
        #endregion
    }
}
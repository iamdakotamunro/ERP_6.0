using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using ERP.Enum;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using OperationLog.Core;
using Telerik.Web.UI;
using Image = System.Drawing.Image;
using WebControl = ERP.UI.Web.Common.WebControl;
using ERP.Environment;

namespace ERP.UI.Web.Windows
{
    public partial class EditBrandForm : WindowsPage
    {

        readonly IGoodsCenterSao _goodsBrandSao=new GoodsCenterSao();
        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitController = CreateSubmitInstance();
            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["BrandId"]))
                {
                    //修改
                    LB_Inster.Visible = false;
                    BrandId = new Guid(Request.QueryString["BrandId"]);
                    if(BrandId==Guid.Empty)return;
                    var brandInfo = _goodsBrandSao.GetBrandDetail(BrandId);
                    NonceBrandInfo = brandInfo;
                    if (brandInfo.GoodsInformationList.Any())
                    {
                        OldGoodsInformationInfo = brandInfo.GoodsInformationList.ToList()[0];
                    }
                    InformationList = brandInfo.GoodsInformationList.ToList();
                    Rp_Informations.DataBind();
                }
                else
                {
                    LB_Update.Visible = false;
                    Lab_UpdateSpace.Visible = false;
                    BrandId = Guid.Empty;
                }
            }
        }

        /// <summary>商品资料列表
        /// </summary>
        public IList<GoodsInformationInfo> InformationList
        {
            get
            {
                if (ViewState["InformationList"] == null)
                {
                    ViewState["InformationList"] = new List<GoodsInformationInfo>();
                }
                return (IList<GoodsInformationInfo>)ViewState["InformationList"];
            }
            set { ViewState["InformationList"] = value; }
        }

        protected SubmitController SubmitController;

        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["EditBrandForm"] == null)
            {
                SubmitController = new SubmitController(Guid.NewGuid());
                ViewState["EditBrandForm"] = SubmitController;
            }
            return (SubmitController)ViewState["EditBrandForm"];
        }

        private Guid BrandId
        {
            get { return (Guid)ViewState["BrandId"]; }
            set { ViewState["BrandId"] = value; }
        }

        #region[商品资料ID]

        protected Guid InformationId
        {
            get
            {
                if (ViewState["InformationId"] == null)
                {
                    return Guid.Empty;
                }
                return (Guid)ViewState["InformationId"];
            }
            set { ViewState["InformationId"] = value; }
        }

        #endregion

        #region[要修改的商品资料原始模型]

        protected GoodsInformationInfo OldGoodsInformationInfo
        {
            get
            {
                if (ViewState["InformationInfo"] == null)
                {
                    var informationInfo = new GoodsInformationInfo();
                    return informationInfo;
                }
                return (GoodsInformationInfo)ViewState["InformationInfo"];
            }
            set { ViewState["InformationInfo"] = value; }
        }

        #endregion

        private GoodsBrandInfo NonceBrandInfo
        {
            get
            {
                Guid brandId = BrandId;
                int orderIndex = 0;
                if (BrandId == Guid.Empty)
                {
                    brandId = Guid.NewGuid();
                }
                else
                {
                    var value = HF_OrderIndex.Value;
                    orderIndex = Convert.ToInt32(string.IsNullOrEmpty(Request.QueryString["OrderIndex"]) ? "0" : value);
                }
                string brandName = TB_Brand.Text;
                string brandLogo = GetLogoUploadPath(RU_BrandLogo, HF_BrandLogo);
                string description = Editor_Description.Content;
                var brandInfo = new GoodsBrandInfo
                                    {
                                        BrandId = brandId,
                                        Brand = brandName,
                                        OrderIndex = orderIndex,
                                        BrandLogo = brandLogo,
                                        Description = description
                                    };
                return brandInfo;
            }
            set
            {
                HF_OrderIndex.Value = string.Format("{0}", value.OrderIndex);
                TB_Brand.Text = value.Brand;
                HF_BrandLogo.Value = value.BrandLogo;
                Editor_Description.Content = value.Description;
            }
        }

        #region[验证相关属性]

        /// <summary>验证相关属性
        /// </summary>
        public bool Validator()
        {
            var val = true;
            var namelist = new List<string>();
            foreach (RepeaterItem item in Rp_Informations.Items)
            {
                var name = ((TextBox)item.FindControl("TB_InformationName")).Text;
                if (namelist.Count == 0)
                {
                    namelist.Add(name);
                }
                else
                {
                    if (namelist.Contains(name))
                    {
                        RAM.Alert("存在同名的商品资料！");
                        val = false;
                    }
                }
                var upInfor = (RadUpload)item.FindControl("Upload_GoodsInformation");
                var rdpOver = (RadDatePicker)item.FindControl("RDPOverdueDate");
                if (upInfor.UploadedFiles.Count > 0)
                {
                    if (rdpOver.SelectedDate == null)
                    {
                        RAM.Alert("请选择过期日期");
                        val = false;
                    }
                }
            }
            return val;
        }

        #endregion

        protected void Button_InsterGoods(object sender, EventArgs e)
        {
            if (!SubmitController.Enabled)
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind('navigateToInserted'); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                return;
            }
            try
            {
                if (Validator())
                {
                    //商品资料
                    //GetGoodsInformationList(NonceBrandInfo.BrandId);
                    var goodsBrandInfo = new GoodsBrandInfo
                                             {
                                                 Brand = NonceBrandInfo.Brand,
                                                 BrandId = NonceBrandInfo.BrandId,
                                                 BrandLogo = NonceBrandInfo.BrandLogo,
                                                 Description = NonceBrandInfo.Description,
                                                 GoodsInformationList = InformationList.ToList(),
                                                 OrderIndex = NonceBrandInfo.OrderIndex
                                             };
                    if (goodsBrandInfo.BrandLogo == string.Empty)
                    {
                        RAM.Alert("图片大小不符合要求!");
                        return;
                    }
                    string errorMessage;
                    var result = _goodsBrandSao.AddBrand(goodsBrandInfo, out errorMessage);
                    if (!result)
                    {
                        RAM.Alert("操作异常！" + errorMessage);
                    }
                    else
                    {
                        //品牌添加操作记录添加
                        var personnelInfo = CurrentSession.Personnel.Get();
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, NonceBrandInfo.BrandId, "", OperationPoint.GoodsBrandManager.AddBrand.GetBusinessInfo(), "");
                        //品牌资料添加操作记录添加
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, NonceBrandInfo.BrandId, "", OperationPoint.GoodsBrandManager.AddInformation.GetBusinessInfo(), "");
                        RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind('navigateToInserted'); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                        SubmitController.Submit();
                    }
                }
            }
            catch
            {
                RAM.Alert("品牌信息添加失败！");
            }
        }

        protected void Button_UpdateGoods(object sender, EventArgs e)
        {
            try
            {
                if (Validator())
                {
                    //商品资料
                    //GetGoodsInformationList(NonceBrandInfo.BrandId);
                    var goodsBrandInfo = new GoodsBrandInfo
                                             {
                                                 Brand = NonceBrandInfo.Brand,
                                                 BrandId = NonceBrandInfo.BrandId,
                                                 BrandLogo = NonceBrandInfo.BrandLogo,
                                                 Description = NonceBrandInfo.Description,
                                                 GoodsInformationList = InformationList.ToList(),
                                                 OrderIndex = NonceBrandInfo.OrderIndex
                                             };
                    if (goodsBrandInfo.BrandLogo == string.Empty)
                    {
                        RAM.Alert("图片大小不符合要求!");
                        return;
                    }
                    string errorMessage;
                    var result = _goodsBrandSao.UpdateBrand(goodsBrandInfo, out errorMessage);
                    if (!result)
                    {
                        RAM.Alert("操作异常！" + errorMessage);
                    }
                    else
                    {
                        RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    }
                }
            }
            catch
            {
                RAM.Alert("品牌信息更改失败！");
            }
        }

        #region[组装商品资料信息]

        private void GetGoodsInformationList(Guid brandId)
        {
           // GoodsBrandInfo goodsBrandInfo = BrandManager.GetBrand(brandId);
           // IList<GoodsInformationInfo> list = goodsBrandInfo == null ? new List<GoodsInformationInfo>() : BrandManager.GetBrand(brandId).GoodsInformationList.ToList();

            IList<GoodsInformationInfo> informationList = new List<GoodsInformationInfo>();
            foreach (RepeaterItem item in Rp_Informations.Items)
            {
                // var hfId = (HiddenField)item.FindControl("hfId");
                //var informationInfoId = new Guid(hfId.Value);
                var informationInfo = new GoodsInformationInfo
                                      {
                                          ID = Guid.NewGuid(),
                                          Name = ((TextBox)item.FindControl("TB_InformationName")).Text
                                      };
                var selectedDate = ((RadDatePicker)item.FindControl("RDPOverdueDate")).SelectedDate;
                if (selectedDate != null)
                {
                    informationInfo.OverdueDate = selectedDate.Value;
                }
                informationInfo.UploadDate = DateTime.Now;
                informationInfo.Path = ((HiddenField)item.FindControl("hfPath")).Value;
                informationInfo.IdentifyId = brandId;
                informationInfo.Type = (int)InformationType.Brand;
                if (!string.IsNullOrEmpty(informationInfo.Path))
                {
                    informationInfo.ExtensionName = informationInfo.Path.Substring(informationInfo.Path.LastIndexOf('.') + 1);
                }
                informationList.Add(informationInfo);
            }
            InformationList = informationList;
        }


        private string GetLogoUploadPath(RadUpload radUpload, HiddenField hfPath)
        {
            if (radUpload.UploadedFiles.Count > 0)
            {
                // 每个月创建一个存放图片的目录
                var upLoadedFile = radUpload.UploadedFiles[0];
                var yearMonth = DateTime.Now.ToString("yyyyMM") + "/";
                var time = DateTime.Now.ToString("yyyyMMddHHmmss");
                var subPath = yearMonth;
                var filePath = String.Format("{0}/BrandImages/{1}", Server.MapPath("/UserDir"), subPath);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var fileName = time + upLoadedFile.GetExtension();

                upLoadedFile.SaveAs(filePath + fileName);

                #region 限制品牌上传图片大小为180*180  add by liangcanren  at 2015-03-11 11:30
                var read = new FileStream(filePath + fileName, FileMode.Open);
                Image img = Image.FromStream(read);
                read.Close();
                if (img.Height!=180 && img.Width!=180)
                {
                    File.Delete(filePath + fileName);
                    return string.Empty;
                }
                #endregion

                return "~/UserDir/BrandImages/" + subPath + fileName;
            }
            return hfPath.Value;
        }

        #endregion

        protected void Rp_Informations_OnItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteInformation")
            {
                var hfId = (HiddenField)e.Item.FindControl("hfId");
                string id = hfId.Value;
                IList<GoodsInformationInfo> list = InformationList.Where(w => w.ID != new Guid(id)).ToList();
                InformationList = list;
                Rp_Informations.DataBind();
            }
        }

        protected void Rp_Informations_OnDataBinding(object sender, EventArgs e)
        {
            Rp_Informations.DataSource = InformationList;
        }

        protected void Rp_Informations_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var rdfOverdueDate = (RadDatePicker)e.Item.FindControl("RDPOverdueDate");
            var hidOverdueDate = (HiddenField)e.Item.FindControl("hfOverdueDate");
            string temp = hidOverdueDate.Value;
            var overdueDate = string.IsNullOrEmpty(temp) ? DateTime.MinValue : DateTime.Parse(temp);

            if (overdueDate != DateTime.MinValue && overdueDate != (DateTime)SqlDateTime.MinValue)
            {
                rdfOverdueDate.SelectedDate = overdueDate;
            }
        }

        protected void BtnAddInformation_Click(object sender, EventArgs e)
        {
            IList<GoodsInformationInfo> list = new List<GoodsInformationInfo>();
            foreach (RepeaterItem item in Rp_Informations.Items)
            {
                var informationInfo = new GoodsInformationInfo
                                          {
                                              ID = new Guid(((HiddenField)item.FindControl("hfId")).Value),
                                              Name = ((TextBox)item.FindControl("TB_InformationName")).Text
                                          };
                var selectedDate = ((RadDatePicker)item.FindControl("RDPOverdueDate")).SelectedDate;
                if (selectedDate != null)
                {
                    informationInfo.OverdueDate = selectedDate.Value;
                }
                informationInfo.UploadDate = DateTime.Now;
                informationInfo.Path = ((HiddenField)item.FindControl("hfPath")).Value;
                informationInfo.Type = (int)InformationType.Brand;
                if (!string.IsNullOrEmpty(informationInfo.Path))
                {
                    informationInfo.ExtensionName = informationInfo.Path.Substring(informationInfo.Path.LastIndexOf('.') + 1);
                }
                list.Add(informationInfo);
            }
            InformationList = list.OrderByDescending(w => w.UploadDate).ToList();
            var newInfo = new GoodsInformationInfo
                              {
                                  ID = Guid.NewGuid(),
                                  UploadDate = DateTime.MinValue,
                                  OverdueDate = DateTime.MinValue,
                                  Path = string.Empty
                              };
            InformationList.Add(newInfo);
            Rp_Informations.DataBind();
        }

        protected void Ru_BrandLogo_Upload(object sender, EventArgs e)
        {
            string path = Server.MapPath(((RadUpload)sender).TargetFolder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        protected void Upload_GoodsInformation_Upload(object sender, EventArgs e)
        {
            Thread.Sleep(1);
            var radUpload = sender as RadUpload;
            if (radUpload != null)
            {
                var item = (RepeaterItem)radUpload.Parent;
                var hfPath = (HiddenField)item.FindControl("hfPath");
                if (radUpload.UploadedFiles.Count > 0)
                {
                    // 每个月创建一个存放图片的目录
                    var upLoadedFile = radUpload.UploadedFiles[0];
                    var yearMonth = DateTime.Now.ToString("yyyyMM") + "/";
                    var time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    var subPath = yearMonth;
                    string folderPath = String.Format("{0}/GoodsInfomation/Brand", Server.MapPath("/UserDir"));
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
                    hfPath.Value = "~/UserDir/GoodsInfomation/Brand/" + subPath + fileName;
                }
            }
        }
    }
}
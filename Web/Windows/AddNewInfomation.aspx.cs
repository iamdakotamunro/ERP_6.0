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
using WebControl = ERP.UI.Web.Common.WebControl;
using ERP.Environment;

namespace ERP.UI.Web.Windows
{
    /// <summary>商品资料添加
    /// </summary>
    public partial class AddNewInfomation : WindowsPage
    {
        #region  公用属性
        /// <summary>
        /// 待添加的商品Id
        /// </summary>
        public Guid GoodsId
        {
            get
            {
                return string.IsNullOrEmpty(Request.QueryString["GoodsId"]) ? Guid.Empty : new Guid(Request.QueryString["GoodsId"]);
            }
        }

        protected GoodsInfo MyGoodsInfo
        {
            get
            {
                if (ViewState["MyGoodsInfo"] == null)
                {
                    var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(GoodsId) ?? new GoodsInfo();
                    ViewState["MyGoodsInfo"] = goodsInfo;
                }
                return (GoodsInfo)ViewState["MyGoodsInfo"];
            }
        }

        /// <summary>
        /// 待添加的商品编号
        /// </summary>
        public string GoodsCode
        {
            get
            {
                return string.IsNullOrEmpty(Request.QueryString["GoodsCode"]) ? string.Empty : Request.QueryString["GoodsCode"];
            }
        }

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
        /// 是否添加操作记录
        /// </summary>
        public bool IsAddRecord
        {
            get
            {
                if (ViewState["IsAddRecord"] == null)
                {
                    return false;
                }
                return Convert.ToBoolean(ViewState["IsAddRecord"]);
            }
            set { ViewState["IsAddRecord"] = value; }
        }
        #endregion

        readonly IGoodsCenterSao _goodsCenterSao=new GoodsCenterSao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InformationList = _goodsCenterSao.GetInformation(GoodsId);
                IsAddRecord = InformationList.Count > 0;
                Rp_Informations.DataBind();
            }
        }

        /// <summary>
        /// 添加商品资料项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAddInformation_Click(object sender, EventArgs e)
        {
            IList<GoodsInformationInfo> list = new List<GoodsInformationInfo>();
            foreach (RepeaterItem item in Rp_Informations.Items)
            {
                var hfPasth = (HiddenField)item.FindControl("hfPath");
                var upInfor = (RadUpload)item.FindControl("Upload_GoodsInformation");
                var informationInfo = new GoodsInformationInfo
                                      {
                                          ID =
                                              new Guid(
                                              ((HiddenField)item.FindControl("hfId")).Value),
                                          Name =
                                              ((TextBox)item.FindControl("TB_InformationName"))
                                              .Text
                                      };
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
                list.Add(informationInfo);
            }
            InformationList = list.OrderByDescending(w => w.UploadDate).ToList();
            var newInfo = new GoodsInformationInfo { ID = Guid.NewGuid(), UploadDate = DateTime.MinValue, OverdueDate = DateTime.MinValue, Path = string.Empty };
            InformationList.Add(newInfo);
            BtnSave.Visible = true;
            Rp_Informations.DataBind();
        }

        /// <summary>
        /// 添加/更新商品资料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            if (GoodsId != Guid.Empty)
            {
                if (Validator())
                {
                    IEnumerable<GoodsInformationInfo> infos = GetGoodsInformationList(GoodsId);
                    string errorMessage;
                    var result = _goodsCenterSao.SetInformationList(GoodsId, infos, out errorMessage);
                    if (result)
                    {
                        //商品资料添加操作记录添加
                        if (!IsAddRecord)
                        {
                            var personnelInfo = CurrentSession.Personnel.Get();
                            WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, GoodsId, "", OperationPoint.GoodsInformationManager.Add.GetBusinessInfo(), string.Empty);
                        }
                        RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    }
                    else
                    {
                        RAM.Alert("操作异常！" + errorMessage);
                    }
                }
            }
        }

        private IEnumerable<GoodsInformationInfo> GetGoodsInformationList(Guid goodsId)
        {
            IList<GoodsInformationInfo> list = new List<GoodsInformationInfo>();
            foreach (RepeaterItem item in Rp_Informations.Items)
            {
                var hfPasth = (HiddenField)item.FindControl("hfPath");
                var upInfor = (RadUpload)item.FindControl("Upload_GoodsInformation");
                var informationInfo = new GoodsInformationInfo();
                informationInfo.ID = new Guid(((HiddenField)item.FindControl("hfId")).Value);
                informationInfo.Name = ((TextBox)item.FindControl("TB_InformationName")).Text;
                informationInfo.IdentifyId = goodsId;
                informationInfo.IdentifyName = MyGoodsInfo.GoodsName;
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
                informationInfo.Type = (int)InformationType.Goods;
                if (!string.IsNullOrEmpty(informationInfo.Path))
                {
                    informationInfo.ExtensionName = informationInfo.Path.Substring(informationInfo.Path.LastIndexOf('.') + 1);
                }
                list.Add(informationInfo);
            }
            return list;
        }

        protected void Rp_Informations_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var rdfOverdueDate = (RadDatePicker)e.Item.FindControl("RDPOverdueDate");
            var hidOverdueDate = (HiddenField)e.Item.FindControl("hfOverdueDate");
            var overdueDate = hidOverdueDate.Value == string.Empty ? DateTime.MinValue : DateTime.Parse(hidOverdueDate.Value);
            if (overdueDate != DateTime.MinValue && overdueDate != (DateTime)SqlDateTime.MinValue)
            {
                rdfOverdueDate.SelectedDate = overdueDate;
            }
        }

        /// <summary>
        /// 资料删除
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 绑定数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Rp_Informations_OnDataBinding(object sender, EventArgs e)
        {
            Rp_Informations.DataSource = InformationList;
        }

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
                string folderPath = String.Format("{0}/GoodsInfomation/Goods", Server.MapPath("/UserDir"));
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
                hfPath.Value = "~/UserDir/GoodsInfomation/Goods/" + subPath + fileName;
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
            var val = true;
            var namelist = new List<string>();
            if (!IsAddRecord)
            {
                if (Rp_Informations.Items.Count == 0)
                {
                    RAM.Alert("商品资料为空！");
                    return false;
                }
            }
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
                var hfPath = (HiddenField)item.FindControl("hfPath");
                if (string.IsNullOrEmpty(hfPath.Value) && upInfor.UploadedFiles.Count == 0)
                {
                    RAM.Alert("请上传商品资料");
                    val = false;
                }
                if (rdpOver.SelectedDate != null) continue;
                RAM.Alert("请选择过期日期");
                val = false;
            }
            return val;
        }

        #endregion
    }
}
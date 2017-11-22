using System;
using System.IO;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class ShowConfirmCheckForm : WindowsPage
    {
        private readonly ICheckDataRecord _checkDataRead = new CheckDataRecord(GlobalConfig.DB.FromType.Read);
        private readonly ICheckDataRecord _checkDataWrite = new CheckDataRecord(GlobalConfig.DB.FromType.Write);
        protected Guid CheckId
        {
            get
            {
                if (ViewState["CheckId"] == null) return Guid.Empty;
                return new Guid(ViewState["CheckId"].ToString());
            }
            set { ViewState["CheckId"] = value; }
        }

        SubmitController submitController;
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["Checking"] == null)
            {
                submitController = new SubmitController(Guid.NewGuid(), 180);
                ViewState["Checking"] = submitController;
            }
            return (SubmitController)ViewState["Checking"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            submitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                string checkId = Request.QueryString["CheckId"];
                if (string.IsNullOrEmpty(checkId) || checkId == Guid.Empty.ToString())
                {
                    return;
                }
                CheckId = new Guid(checkId);
                var info = _checkDataRead.GetCheckDataInfoById(CheckId);
                span_Msg.InnerText = "[" + info.CompanyName + "] "; 
                span_Msg2.InnerText = info.CheckType == (int)ReckoningCheckType.Carriage ? "运费对账" : "代收对账";

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LbSubmitClick(object sender, EventArgs e)
        {
            if (!submitController.Enabled)
            {
                RAM.Alert("文件已上传，请不要重复提交");
                return;
            }
            if (RuLoadXLS.UploadedFiles.Count == 0)
            {
                RAM.Alert("请选择你上传的对账Excel/Csv文件");
                return;
            }

            foreach (UploadedFile item in RuLoadXLS.UploadedFiles)
            {
                DateTime dtNow = DateTime.Now;
                if (item.FileName != null)
                {
                    if (!(item.ContentLength > 0))
                    {
                        RAM.Alert("请上传有数据的对账单！");
                        return;
                    }
                    string sFileName = item.GetName();

                    //****保存原始单据EXCEL****/
                    string sCheckingServerPath = Server.MapPath("../UserDir/ZzzConfirmDataFolder/") + dtNow.ToString("yyyyMM");
                    if (!Directory.Exists(sCheckingServerPath))
                        Directory.CreateDirectory(sCheckingServerPath);

                    string sTime = dtNow.ToString("(yyyy-MM-dd(hh-mm-ss))");
                    string filePath = sCheckingServerPath + "/" + sTime + sFileName;
                    try
                    {
                        var info = _checkDataWrite.GetCheckDataInfoById(CheckId);
                        if (info == null)
                        {
                            RAM.Alert("对账记录不存在!");
                            return;
                        }
                        if (!string.IsNullOrEmpty(info.ConfirmFilePath))
                        {
                            string path = sCheckingServerPath + "/" + info.ConfirmFilePath;
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                            }
                        }
                        info.ConfirmFilePath = "ZzzConfirmDataFolder/" + dtNow.ToString("yyyyMM") + "/" + sTime + sFileName;
                        item.SaveAs(filePath);
                        _checkDataWrite.UpdateResult(info);
                        submitController.Submit();
                    }
                    catch (Exception ex)
                    {
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                        RAM.Alert("保存Excel失败：" + ex.Message);
                        return;
                    }
                }
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Text;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

//Func：对账页面(尽量避免人工的操作出错，这个目地。)
//Date：2009-07-05
//Code：dyy
//Modify by liucaijun at 2011-May-25th
//Modify line:330;380;400;421

namespace ERP.UI.Web.Windows
{
    public partial class ShowCheckingForm : WindowsPage
    {
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        //readonly CheckDataManager checkDataManager = new CheckDataManager();
        SubmitController _submitController;
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["Checking"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid(), 180);
                ViewState["Checking"] = _submitController;
            }
            return (SubmitController)ViewState["Checking"];
        }

        #region 属性列表
        //往来单位编号
        protected Guid CompanyId
        {
            get
            {
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set
            {
                ViewState["CompanyId"] = value.ToString();
            }
        }

        protected Guid FilialeId
        {
            get
            {
                return new Guid(ViewState["FilialeId"].ToString());
            }
            set
            {
                ViewState["FilialeId"] = value.ToString();
            }
        }

        //往来单位编号
        protected bool IsExpress
        {
            get
            {
                return ViewState["IsExpress"] != null && Convert.ToBoolean(ViewState["IsExpress"]);
            }
            set
            {
                ViewState["IsExpress"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (Page.IsPostBack) return;
            Session["FirstTime"] = null;
            CompanyId = new Guid(Request.QueryString["CompanyId"]);
            FilialeId = new Guid(Request.QueryString["FilialeId"]);
            string companyName = string.Empty;
            if (CacheCollection.Filiale.GetList().Any(f => f.ID == CompanyId))
            {
                var info = CacheCollection.Filiale.GetList().FirstOrDefault(f => f.ID == CompanyId);
                companyName = info != null ? info.Name : string.Empty;
            }
            else
            {
                var info = _companyCussent.GetCompanyCussent(CompanyId);
                if (info != null)
                {
                    companyName = info.CompanyName;
                    IsExpress = !string.IsNullOrEmpty(info.CompanyName);
                }
            }
            TB_Description.Text = "[对账]" + companyName;
        }

        #region 对账事件
        /// <summary>
        /// 对账处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LbCheckingClick(object sender, EventArgs e)
        {
            if (!_submitController.Enabled)
            {
                ExeJsAlertClosWindow(true, true, "对账成功，请不要重复提交");
                return;
            }

            if (upXLS.UploadedFiles.Count == 0)
            {
                ExeJsAlert("提示：", "请选择你上传的对账Excel文件");
                return;
            }
            if (!IsExpress)
            {
                ExeJsAlert("提示：", "请选择快递公司!");
                return;
            }
            foreach (UploadedFile item in upXLS.UploadedFiles)
            {
                DateTime dtNow = DateTime.Now;
                if (item.FileName != null)
                {
                    if (!(item.ContentLength > 0))
                    {
                        ExeJsAlert("提示：", "请上传有数据的对账单！");
                        return;
                    }
                    string sFileName = item.GetName();

                    var strs = new[] { Server.MapPath("../UserDir/ZzzOriginalFolder"), Server.MapPath("../UserDir/ZzzContrastFolder") 
                        ,Server.MapPath("../UserDir/ZzzConfirmDataFolder"),Server.MapPath("../UserDir/ZzzFinishedFolder")};
                    //****保存原始单据EXCEL****/
                    //创建对账所需目录
                    foreach (string t in strs)
                    {
                        if (!Directory.Exists(t))
                            Directory.CreateDirectory(t);
                    }
                    string path = strs[0] + "/" + dtNow.ToString("yyyyMM");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    else
                    {
                        var list = Directory.GetFiles(path);
                        if (list.Length > 0 && list.Any(act => act.Contains(sFileName)))
                        {
                            ExeJsAlert("保存Excel失败：", "该Excel/Csv已存在对账记录!");
                            return;
                        }
                    }
                    int checkType = RdbtnCarriage.Checked ? (int)ReckoningCheckType.Carriage : (int)ReckoningCheckType.Collection;
                    string sTime = dtNow.ToString("(yyyy-MM-dd(hh-mm-ss))");
                    string filePath = path + "/" + sTime + sFileName;
                    string description = WebControl.RetrunUserAndTime(TB_Description.Text);
                    try
                    {
                        item.SaveAs(filePath);
                        var recordInfo = new CheckDataRecordInfo
                        {
                            OriginalFilePath = "ZzzOriginalFolder/" + dtNow.ToString("yyyyMM") + "/" + sTime + sFileName,
                            CheckCreateDate = DateTime.Now,
                            CheckCompanyId = CompanyId,
                            CheckType = checkType,
                            CheckId = Guid.NewGuid(),
                            CheckUser = CurrentSession.Personnel.Get().EnterpriseNo,
                            CheckPersonnelId = CurrentSession.Personnel.Get().PersonnelId,
                            CheckFilialeId = FilialeId == Guid.Empty ? CurrentSession.Personnel.Get().FilialeId : FilialeId,
                            CheckDescription = description,
                            CheckDataState = (int)CheckDataState.Loaded,
                        };
                        CheckDataManager.WriteInstance.InsertData(recordInfo);
                        _submitController.Submit();
                    }
                    catch (Exception ex)
                    {
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                        ExeJsAlert("保存Excel失败：", ex.Message);
                        return;
                    }
                }
            }
            ExeJsAlertClosWindow(true, true, "提示：上传成功" + "\\n");
            RAM.ResponseScripts.Add("hidemask();");
        }
        #endregion

        #region 方法
        /// <summary>
        /// 复杂的返回window.alert对话框
        /// </summary>
        /// <param name="enableClose">是否关闭当前对话框</param>
        /// <param name="enableRefreshParentWindow">是否刷新父对话框</param>
        /// <param name="sAlert">显示的内容 实现分行</param>
        protected void ExeJsAlertClosWindow(bool enableClose, bool enableRefreshParentWindow, string sAlert)
        {
            String ssAlert = sAlert.Substring(0, sAlert.Length - 2);
            string sssAlert;
            if (!enableClose && !enableRefreshParentWindow)
                sssAlert = String.Format(@"<script type='text/javascript'>window.alert('{0}')</script>", ssAlert);
            else if (!enableClose)
                sssAlert = String.Format(@"<script type='text/javascript'>window.alert('{0}');GetRadWindow().BrowserWindow.refreshGrid();</script>", ssAlert);
            else if (!enableRefreshParentWindow)
                sssAlert = String.Format(@"<script type='text/javascript'>window.alert('{0}');CancelWindow();</script>", ssAlert);
            else
                sssAlert = String.Format(@"<script type='text/javascript'>window.alert('{0}'); GetRadWindow().BrowserWindow.refreshGrid();GetRadWindow().Close();</script>", ssAlert);

            ClientScript.RegisterStartupScript(GetType(), DateTime.Now.ToLongTimeString(), sssAlert, false);
        }

        /// <summary>
        /// 简单返回window.alert对话框
        /// </summary>
        /// <param name="sAlert">显示内容 实现分行</param>
        public void ExeJsAlert(params string[] sAlert)
        {
            String ssAlert;
            if (sAlert.Length == 0)
                ssAlert = "";
            else
            {
                var strAlert = new StringBuilder();
                foreach (string salert in sAlert)
                {
                    strAlert.Append(salert);
                    strAlert.Append("\\n");
                }
                int length = strAlert.Length;
                ssAlert = strAlert.ToString().Substring(0, length - 2);
            }
            ClientScript.RegisterStartupScript(GetType(), DateTime.Now.ToLongTimeString(), String.Format(@"<script type='text/javascript'> window.alert('{0}');</script>", ssAlert), false);

        }
        #endregion
    }
}

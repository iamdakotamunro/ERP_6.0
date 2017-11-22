using System;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;

//================================================
// 功能：新版本买库存商品申请审核页面
// 作者：翟晓飞
// 时间：2013-03-05
//================================================
namespace ERP.UI.Web.Windows
{
    ///<summary>
    /// 新版本买库存商品申请审核页面
    ///</summary>
    public partial class AddWasteBookCheckForm : WindowsPage
    {
        #region[声明类]

        private readonly IWasteBook _wasteBook = new WasteBook(GlobalConfig.DB.FromType.Read);
        private readonly IWasteBookCheck _wasteBookCheck=new WasteBookCheck(GlobalConfig.DB.FromType.Write);

        protected Boolean IsSaveAndAdd;

        #endregion

        #region[提交控件]
        protected SubmitController SubmitController;
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["SubmitController"] == null)
            {
                SubmitController = new SubmitController(Guid.NewGuid());
                ViewState["SubmitController"] = SubmitController;
            }
            return (SubmitController)ViewState["SubmitController"];
        }
        #endregion

        #region[窗体加载]
        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["WasteBookId"]))
                {
                    WasteBookId = new Guid(Request.QueryString["WasteBookId"]);
                    BindCheckField(WasteBookId);
                }
            }
        }

        #endregion

        #region[商品ID]
        protected Guid WasteBookId
        {
            get
            {
                return new Guid(ViewState["WasteBookId"].ToString());
            }
            set
            {
                ViewState["WasteBookId"] = value.ToString();
            }
        }
        #endregion

        #region [加载页面初始值]

        private void BindCheckField(Guid wasteBookId)
        {
            var info = _wasteBook.GetWasteBook(wasteBookId)??new WasteBookInfo();
            var checkInfo = _wasteBookCheck.GetWasteBookCheck(wasteBookId);
            LitNonceBalance.Text = string.Format("{0}",info.NonceBalance);
            if (checkInfo != null)
            {
                TB_CheckMoney.Text = string.Format("{0}",checkInfo.CheckMoney);
                TB_Memo.Text = checkInfo.Memo;
            }
        }

        #endregion

        protected void Btn_Apply_Click(object sender, EventArgs e)
        {
            SaveCheck();
        }

        private void SaveCheck()
        {
            try
            {
                var info = _wasteBook.GetWasteBook(WasteBookId)??new WasteBookInfo();
                var checkInfo = _wasteBookCheck.GetWasteBookCheck(WasteBookId);
                if (info.NonceBalance.CompareTo(decimal.Parse(TB_CheckMoney.Text))!=0)
                {
                    if (TB_Memo.Text.Trim()=="")
                    {
                        RAM.Alert("差额说明不能为空！");
                        return;
                    }
                }
                if (checkInfo != null)
                {
                    checkInfo.CheckMoney = decimal.Parse(TB_CheckMoney.Text);
                    checkInfo.Memo = TB_Memo.Text;
                    checkInfo.DateCreated = DateTime.Now;
                    _wasteBookCheck.Update(checkInfo);
                }
                else
                {
                    checkInfo = new WasteBookCheckInfo
                                    {
                                        WasteBookId = WasteBookId,
                                        CheckMoney = decimal.Parse(TB_CheckMoney.Text),
                                        Memo = TB_Memo.Text,
                                        DateCreated = DateTime.Now
                                    };
                    _wasteBookCheck.Insert(checkInfo);
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception ex)
            {
                RAM.Alert("保存失败，系统提示：" + ex.Message);
            }
        }

        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
    }
}

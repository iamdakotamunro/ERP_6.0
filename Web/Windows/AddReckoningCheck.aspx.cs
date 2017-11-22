using System;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;

namespace ERP.UI.Web.Windows
{
    public partial class AddReckoningCheck : WindowsPage
    {
        private readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Read);

        private readonly IReckoningCheck _reckoningCheck = new ReckoningCheck(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ReckoningId"]))
                {
                    ReckoningId = new Guid(Request.QueryString["ReckoningId"]);

                    var reckoningInfo = _reckoning.GetReckoning(ReckoningId);
                    var reckoningCheckInfo = _reckoningCheck.GetReckoningCheckByReckoningId(ReckoningId);
                    LitNonceTotalled.Text = string.Format("{0}", reckoningInfo.NonceTotalled);
                    if (reckoningCheckInfo != null)
                    {
                        TB_Memo.Text = reckoningCheckInfo.Memo;
                    }
                }
            }
        }

        protected Guid ReckoningId
        {
            get
            {
                return new Guid(ViewState["ReckoningId"].ToString());
            }
            set
            {
                ViewState["ReckoningId"] = value.ToString();
            }
        }

        protected void Btn_Apply_Click(object sender, EventArgs e)
        {
            try
            {
                if(TB_Memo.Text.Trim()=="")
                {
                    RAM.Alert("差额说明不能为空！");
                    return;
                }
                var reckoningCheckInfo = _reckoningCheck.GetReckoningCheckByReckoningId(ReckoningId);
                if (reckoningCheckInfo != null)
                {
                    reckoningCheckInfo.Memo = TB_Memo.Text;
                    reckoningCheckInfo.DateCreated = DateTime.Now;
                    _reckoningCheck.UpdateReckoningCheck(reckoningCheckInfo);
                }
                else
                {
                    var rc = new ReckoningCheckInfo { ReckoningId = ReckoningId, Memo = TB_Memo.Text, DateCreated = DateTime.Now };
                    _reckoningCheck.InsertReckoningCheck(rc);
                }
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception ex)
            {
                RAM.Alert("保存失败，系统提示：" + ex.Message);
            }
        }
    }
}
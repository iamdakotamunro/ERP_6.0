using System;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;

namespace ERP.UI.Web.Windows
{
    public partial class AddActityFilingLog : WindowsPage
    {
        private  static readonly IActivityOperateLog _activityOperateLog = InventoryInstance.GetActivityOperateLogDao(GlobalConfig.DB.FromType.Write);

        protected void Page_Load(object sender, EventArgs e)
        {
            var id = new Guid(Request.QueryString["id"]);
            var list = _activityOperateLog.SelectLogModels(id);
            HF_ActivityFilingID.Value = id.ToString();
            RP_Clew.DataSource = list;
            RP_Clew.DataBind();
        }

        protected void LB_Save_OncLick(object sender, EventArgs e)
        {
            if (!CanSubmit())
            {
                RAM.Alert("请不要重复提交!");
                return;
            }
            ExecuteSubmit((ctx) =>
            {
                if (!string.IsNullOrEmpty(RTB_RemarkInput.Text))
                {
                    var personnelInfo = CurrentSession.Personnel.Get();
                    var activityLogModel = new ActivityOperateLogModel
                    {
                        OperatePersonnelID = personnelInfo.PersonnelId,
                        OperatePersonnelName = personnelInfo.RealName,
                        OperateDate = DateTime.Now,
                        ActivityFilingID = new Guid(HF_ActivityFilingID.Value),
                        Description = "[" + DateTime.Now + personnelInfo.RealName + "]" + RTB_RemarkInput.Text
                    };
                    var result = _activityOperateLog.InsertLog(activityLogModel);
                    if (result)
                    {
                        var list = _activityOperateLog.SelectLogModels(new Guid(HF_ActivityFilingID.Value));
                        RP_Clew.DataSource = list;
                        RP_Clew.DataBind();
                        RTB_RemarkInput.Text = "";
                    }
                    else
                    {
                        ctx.SetFail();
                    }
                }
                else
                {
                    RAM.Alert("日志内容不允许为空！");
                    ctx.SetFail();
                }
            });
        }
    }
}
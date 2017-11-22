using System;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Basis;
using ERP.DAL.Interface.IBasis;
using ERP.Environment;
using ERP.UI.Web.Base;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary> 基本单位 2015-05-08  陈重文  （优化代码，增加注释）
    /// </summary>
    public partial class UnitsAw : BasePage
    {

        private readonly IUnits _unitsWirte=new Units(GlobalConfig.DB.FromType.Write);
        private readonly IWebRudder _webRudder=new WebRudder(GlobalConfig.DB.FromType.Read);

        /// <summary>页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        /// <summary>绑定数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UnitsGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            UnitsGrid.DataSource = _unitsWirte.GetUnitsList();
        }

        /// <summary>新增基本单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UnitsGrid_InsertCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            var unitsId = Guid.NewGuid();
            if (editedItem != null)
            {
                var textBox = editedItem.FindControl("TB_Units") as TextBox;
                if (textBox != null)
                {
                    string unitsName = textBox.Text;
                    try
                    {
                        var unitsInfo = new UnitsInfo(unitsId, unitsName);
                        _unitsWirte.Insert(unitsInfo);
                    }
                    catch
                    {
                        RAM.Alert("数量单位信息数据添加失败！");
                    }
                }
            }
        }

        /// <summary>更新基本单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UnitsGrid_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                var unitsId = new Guid(editedItem.GetDataKeyValue("UnitsId").ToString());
                var textBox = editedItem.FindControl("TB_Units") as TextBox;
                if (textBox != null)
                {
                    string unitsName = textBox.Text;
                    try
                    {
                        var unitsInfo = new UnitsInfo(unitsId, unitsName);
                        _unitsWirte.Update(unitsInfo);
                    }
                    catch
                    {
                        RAM.Alert("数量单位信息数据修改失败！");
                    }
                }
            }
        }

        /// <summary>删除基本单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UnitsGrid_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                var webRudderInfo = _webRudder.GetWebRudder();
                var unitsId = new Guid(editedItem.GetDataKeyValue("UnitsId").ToString());
                if (unitsId != webRudderInfo.DefaultUnitsId)
                {
                    try
                    {
                        _unitsWirte.Delete(unitsId);
                    }
                    catch
                    {
                        RAM.Alert("数量单位信息数据删除失败！");
                    }
                }
                else
                {
                    RAM.Alert("该数量单位为默认使用积分规则，不允许删除！");
                }
            }
        }
    }
}
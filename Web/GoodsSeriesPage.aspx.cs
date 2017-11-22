using System;
using System.Web.UI.WebControls;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    public partial class GoodsSeriesPage : BasePage
    {
        private readonly IGoodsCenterSao _goodsCenterSao=new GoodsCenterSao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected string SeriesName
        {
            get
            {
                if (ViewState["SeriesName"] == null) return string.Empty;
                return ViewState["SeriesName"].ToString();
            }
            set { ViewState["SeriesName"] = value; }
        }

        // 数据绑定
        protected void GridGoodsSeries_OnNeedDataSource(object sender, EventArgs e)
        {
            Grid_GoodsSeries.DataSource = _goodsCenterSao.GetSeriesList(SeriesName);
        }

        protected void GridGoodsSeries_OnItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Search")
            {
                SeriesName = ((TextBox)e.Item.FindControl("TB_Search")).Text.Trim();
                Grid_GoodsSeries.Rebind();
            }
        }

        // 增加
        protected void GridGoodsSeries_OnInsertCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            string seriesName = ((TextBox)editedItem.FindControl("TB_SeriesName")).Text;
            var info = new GoodsSeriesInfo();
            info.SeriesID = Guid.NewGuid();
            info.SeriesName = seriesName;

            string errorMessage;
            var result = _goodsCenterSao.AddSeries(info, out errorMessage);//插入系列
            if (result)
            {
                //记录工作日志
                var personnelInfo = CurrentSession.Personnel.Get();
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, info.SeriesID, "",
                                         OperationPoint.GoodsSeriesManager.Add.GetBusinessInfo(), string.Empty);
            }
            else
            {
                RAM.Alert("操作无效！" + errorMessage);
            }
        }

        // 删除
        protected void GridGoodsSeries_OnDeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            var id = (Guid)editedItem.GetDataKeyValue("SeriesID");
            try
            {
                string errorMessage;
                var result = _goodsCenterSao.DeleteSeries(id, out errorMessage);//删除分类
                if (result)
                {
                    //记录工作日志
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, id, "",
                                             OperationPoint.GoodsSeriesManager.Delete.GetBusinessInfo(), string.Empty);
                }
                else
                {
                    RAM.Alert("操作无效！" + errorMessage);
                }
            }
            catch
            {
                RAM.Alert("删除失败！");
            }
        }

        // 修改
        protected void GridGoodsSeries_OnUpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            var id = (Guid)editedItem.GetDataKeyValue("SeriesID");
            string seriesName = ((TextBox)editedItem.FindControl("TB_SeriesName")).Text;
            var goodsSeries = new GoodsSeriesInfo();
            goodsSeries.SeriesID = id;
            goodsSeries.SeriesName = seriesName;

            string errorMessage;
            var result = _goodsCenterSao.UpdateSeries(goodsSeries, out errorMessage);//更新系列
            if (result)
            {
                //记录工作日志
                var personnelInfo = CurrentSession.Personnel.Get();
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, id, "",
                                         OperationPoint.GoodsSeriesManager.Edit.GetBusinessInfo(), string.Empty);

            }
            else
            {
                RAM.Alert("操作无效！" + errorMessage);
            }
        }
    }
}
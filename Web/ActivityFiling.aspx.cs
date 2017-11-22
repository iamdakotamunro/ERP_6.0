using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using AllianceShop.Common.Extension;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    public partial class ActivityFiling : BasePage
    {
        readonly IActivityFiling _activityFilingWrite = InventoryInstance.GetActivityFilingDao(GlobalConfig.DB.FromType.Write);
        readonly IActivityOperateLog _activityOperateLog = InventoryInstance.GetActivityOperateLogDao(GlobalConfig.DB.FromType.Write);
        private const string PAGE_NAME = "ActivityFiling.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //绑定销售平台
                BandingSaleTerrace();
                if (!WebControl.GetPowerOperationPoint(PAGE_NAME, "Auditing"))
                {
                    ActivityFilingRad.MasterTableView.Columns.FindByUniqueName("Auditing").Visible = false;
                }
                if (!WebControl.GetPowerOperationPoint(PAGE_NAME, "Delete"))
                {
                    ActivityFilingRad.MasterTableView.Columns.FindByUniqueName("Delete").Visible = false;
                }
            }
        }

        #region 绑定销售平台、状态

        public void BandingSaleTerrace()
        {
            //var saleTerraceList = Common.CacheCollection.SalePlatform.GetList();
            var saleTerraceList = CacheCollection.Filiale.GetHeadList();
            RadSaleTerrace.DataSource = saleTerraceList;
            RadSaleTerrace.DataTextField = "Name";
            RadSaleTerrace.DataValueField = "ID";
            RadSaleTerrace.DataBind();
            RadSaleTerrace.Items.Insert(0, new RadComboBoxItem("销售平台列表", Guid.Empty.ToString()));
            //RadSaleTerrace.MaxHeight = 100;

            var stateDic = EnumAttribute.GetDict<ActivityFilingState>();
            foreach (var items in stateDic)
            {
                //if (items.Key!=5)
                //{
                    RadFilingState.Items.Add(new RadComboBoxItem(items.Value, items.Key.ToString(CultureInfo.InvariantCulture)));
                //}
            }
            RadFilingState.Items.Insert(0, new RadComboBoxItem("请选择状态", "0"));
            RadFilingState.Items.Insert(1, new RadComboBoxItem("所有状态", "-1"));
        }

        #endregion

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="title"></param>
        /// <param name="goodsName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="saleTrrace"></param>
        /// <param name="state"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        private IList<ActivityFilingInfo> GetData(string title, string goodsName, DateTime startDate, DateTime endDate, Guid saleTrrace, int state, int pageIndex, int pageSize, out int total)
        {
            //return BLL.Implement.Inventory.ActivityFiling.GetActivityFilings(title, goodsName, startDate, endDate, saleTrrace, state, pageIndex, pageSize, out total);
            var resultList = new List<ActivityFilingInfo>();
            var list = _activityFilingWrite.SelectActivityFilings(title, goodsName, startDate, endDate,
                saleTrrace,
                state, pageIndex, pageSize, out total);
            if (list != null && list.Any())
            {
                resultList.AddRange(list);
                var newList = list.Where(x => x.ActivityFilingState == (int)ActivityFilingState.FilingPass && x.ActivityStateDate < DateTime.Now && x.ErrorProbability == 0 && x.ActualSaleNumber == 0).ToList();
                if (!newList.Any())
                {
                    return resultList;
                }
                foreach (var activityFilingInfo in newList)
                {
                    var saleNumber = _activityFilingWrite.GetGoodsRealSale(activityFilingInfo.GoodsID,
                        activityFilingInfo.WarehouseID, activityFilingInfo.FilingCompanyID, activityFilingInfo.FilingTerraceID, activityFilingInfo.ActivityStateDate,
                        activityFilingInfo.ActivityEndDate.AddDays(1));
                    var errorProbability = activityFilingInfo.ProspectSaleNumber != 0 ? Convert.ToDecimal(Math.Round(Convert.ToDecimal((saleNumber - activityFilingInfo.ProspectSaleNumber) / Convert.ToDecimal(activityFilingInfo.ProspectSaleNumber)) * 100, 2)) : 0;
                    if (activityFilingInfo.ActivityEndDate < DateTime.Now && activityFilingInfo.ActualSaleNumber == 0 &&
                        activityFilingInfo.ErrorProbability == 0) //活动结束
                    {
                        _activityFilingWrite.UpdateGoodsSaleNumber(activityFilingInfo.ID, saleNumber, errorProbability);
                        activityFilingInfo.ActualSaleNumber = saleNumber;
                        activityFilingInfo.ErrorProbability = errorProbability;
                    }
                    else if (activityFilingInfo.ActivityEndDate > DateTime.Now)
                    {
                        activityFilingInfo.ActualSaleNumber = saleNumber;
                        activityFilingInfo.ErrorProbability = errorProbability;
                    }
                    resultList.Remove(activityFilingInfo);
                    resultList.Add(activityFilingInfo);
                }
            }
            return resultList;
        }
        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void FilingDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            int total;
            var startIndex = ActivityFilingRad.CurrentPageIndex + 1;
            var size = ActivityFilingRad.PageSize;
            var title = activityTitle.Text;
            var goodsName = activityGoodsName.Text;
            var startDate = startDateTime.DbSelectedDate;
            var endDate = endDateTime.DbSelectedDate;
            var saleTerrace = RadSaleTerrace.SelectedValue;
            var state = RadFilingState.SelectedValue.ToInt();
            var list = GetData(title, goodsName, startDate.ToDateTime(), endDate.ToDateTime(), new Guid(saleTerrace), state, startIndex, size, out total);
            var receiptNo = ActivityFilingRad.MasterTableView.Columns.FindByUniqueName("GoodsName");//文本
            var prospectSaleNumber = ActivityFilingRad.MasterTableView.Columns.FindByUniqueName("ProspectSaleNumber");//预估销量
            var normelSaleNumber = ActivityFilingRad.MasterTableView.Columns.FindByUniqueName("NormalSaleNumber");//正常销量
            var realSaleNumber = ActivityFilingRad.MasterTableView.Columns.FindByUniqueName("ActualSaleNumber");//实际销量
            var error = ActivityFilingRad.MasterTableView.Columns.FindByUniqueName("ErrorProbability");//误差率
            var totalResult = _activityFilingWrite.TotalNumber(title, goodsName, startDate.ToDateTime(), endDate.ToDateTime(), new Guid(saleTerrace), state);
            receiptNo.FooterText = "销量汇总：";
            if (totalResult != null)
            {
                prospectSaleNumber.FooterText = totalResult.TotalProspectSaleNumber.ToString(CultureInfo.InvariantCulture);
                normelSaleNumber.FooterText = totalResult.TotalNormalSaleNumber.ToString(CultureInfo.InvariantCulture);
                realSaleNumber.FooterText = totalResult.TotalActualSaleNumber.ToString(CultureInfo.InvariantCulture);
                error.FooterText = (totalResult.TotalProspectSaleNumber > 0 || totalResult.TotalProspectSaleNumber<0) ?Convert.ToDecimal(Math.Round(((totalResult.TotalActualSaleNumber - totalResult.TotalProspectSaleNumber) / totalResult.TotalProspectSaleNumber * 100), 2)) + "%" : "0%";
            }
            else
            {
                prospectSaleNumber.FooterText = string.Empty;
                normelSaleNumber.FooterText = string.Empty;
                realSaleNumber.FooterText = string.Empty;
                error.FooterText = string.Empty;
            }
            receiptNo.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
            prospectSaleNumber.FooterStyle.HorizontalAlign=HorizontalAlign.Center;
            normelSaleNumber.FooterStyle.HorizontalAlign=HorizontalAlign.Center;
            realSaleNumber.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
            error.FooterStyle.HorizontalAlign = HorizontalAlign.Center;
            ActivityFilingRad.DataSource = list.OrderByDescending(o=>o.CreateDate);
            ActivityFilingRad.VirtualItemCount = total;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="soure"></param>
        /// <param name="e"></param>
        protected void Search_ActivityFiling(object soure, EventArgs e)
        {
            ActivityFilingRad.Rebind();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ActivityFiling_OnItemCommand(object sender, GridCommandEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item != null)
            {
                var personnelInfo = CurrentSession.Personnel.Get();
                var dataItem = item;
                var id = new Guid(dataItem.GetDataKeyValue("ID").ToString());
                if (e.CommandName == "Delete")
                {
                    var result =_activityFilingWrite.UpdateFilingState(id,(int)ActivityFilingState.FilingDelete);
                    if (result)
                    {
                        var activityOperateLogModel = new ActivityOperateLogModel
                        {
                            OperatePersonnelID = personnelInfo.PersonnelId,
                            OperatePersonnelName = personnelInfo.RealName,
                            OperateDate = DateTime.Now,
                            ActivityFilingID = id
                        };
                        activityOperateLogModel.Description = "[" + activityOperateLogModel.OperateDate + "]" + activityOperateLogModel.OperatePersonnelName + "删除活动报备单";
                        _activityOperateLog.InsertLog(activityOperateLogModel);
                        ActivityFilingRad.Rebind();
                    }
                }
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RAM_OnAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(ActivityFilingRad, e);
        }
    }
}
using System;
using System.Collections.Generic;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.UI.Web.Base;
using ERP.Environment;

namespace ERP.UI.Web.Windows
{
    public partial class ShowProportionForm : WindowsPage
    {
        private readonly ITaxrateProportion _taxrateProportion = new TaxrateProportionDao();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["GoodsType"]))
                {
                    var dataSource = _taxrateProportion.GetTaxrateProportionInfos(Convert.ToInt32(Request.QueryString["GoodsType"]));
                    var operateTypes = Enum.Attribute.EnumAttribute.GetDict<OperateType>();
                    int index = 0;
                    List< ShowRecord > dataList=new List<ShowRecord>();
                    foreach (var item in dataSource)
                    {
                        index++;
                        dataList.Add(new ShowRecord(index,item.UpdateDate,item.ViewPercentage,item.Operator, operateTypes[item.OperateType],item.Remark));
                    }
                    RgRecord.DataSource = dataList;
                    RgRecord.DataBind();
                }
            }
        }

        protected void BtnOkClick(object sender, EventArgs e)
        {
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
    }

    [Serializable]
    public class ShowRecord
    {
        public int Id { get; set; }

        public string UpdateDate { get; set; }

        public string CurrentTaxrate { get; set; }

        public string Operator { get; set; }

        public string OperateType { get; set; }

        public string Remark { get; set; }

        public ShowRecord(int id,DateTime updateTime,string currentTaxrate,string operatorName,string operateType,string remark)
        {
            Id = id;
            UpdateDate = updateTime.ToString("yyyy-MM-dd HH:mm:ss");
            Operator = operatorName;
            OperateType = operateType;
            Remark = remark;
            CurrentTaxrate = currentTaxrate;
        }
    }
}
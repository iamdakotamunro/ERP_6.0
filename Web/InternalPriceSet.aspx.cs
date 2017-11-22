using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.Environment;
using Telerik.Web.UI;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Implement.Inventory;
using ERP.SAL;
using Keede.Ecsoft.Model;
using ERP.Model;
using MIS.Enum;

namespace ERP.UI.Web
{
    public partial class InternalPriceSet : BasePage
    {
        readonly IGoodsCenterSao _goodsAttributeGroupSao = new GoodsCenterSao();
        private readonly IInternalPriceSetDao _internalPriceSetDao = new InternalPriceSetDao(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FilialeDic = MISService.GetAllFiliales().Where(act => act.IsActive && (act.FilialeTypes.Contains((int)FilialeType.LogisticsCompany))).ToList();
            }
            RAM.ResponseScripts.Add("removeClass();");
        }

        private IList<AttributeGroupInfo> AttrGroupList
        {
            get { return _goodsAttributeGroupSao.GetAttrGroupList().OrderBy(p => p.OrderIndex).ToList(); }
        }

        protected string GetMatchType(object matchType)
        {
            return EnumAttribute.GetKeyName((MatchType)matchType);
        }
        
        public string GetEnumName(object goodsType)
        {
            return EnumAttribute.GetKeyName((GoodsKindType)goodsType);
        }

        private string GetPent(string num)
        {
            var persents = Math.Truncate((Convert.ToDecimal(num)) * 10000) / 100;
            return string.Format("{0}%", persents == 100 ? "100.00" : persents.ToString(CultureInfo.InvariantCulture));
        }

        private void BindGroups()
        {
            int num = 0;
            var listGoods = new List<InternalPriceSetInfo>();
            

            IDictionary<Int32, string> lstEnum = EnumAttribute.GetDict<GoodsKindType>();
            foreach (KeyValuePair<Int32, string> kp in lstEnum)
            {
                if (kp.Key != (int)GoodsKindType.NoSet)
                {
                    listGoods.Add(new InternalPriceSetInfo
                    {
                        GoodsType = kp.Key,
                        GoodsTypeName = kp.Value,
                        ReserveProfitRatio = "0"
                    });
                }
            }
            RadGrid_InvoiceRollList.DataSource = listGoods;
            internalPriceSetList = _internalPriceSetDao.GetInternalPriceSetInfoList(num);
        }

        /// <summary>绑定数据源
        /// </summary>
        protected void RadGrid_InvoiceRollList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            ReadInvoiceRollList();
        }

        /// <summary>列表源数据
        /// </summary>
        private void ReadInvoiceRollList()
        {
            BindGroups();
        }


        protected void GridInvoiceRollListItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                string row1 = "<tr>";
                int i = 0;
                foreach (var filiale in FilialeDic)
                {
                    row1 += "<td class='Group' style='width:160px;padding-top:5px; padding-bottom:5px; border-bottom:1px solid #3d556c;" + (" border-left:1px solid #3d556c;") + "'>" + filiale.Name + "</td>";
                }
                row1 = row1 + "</tr>";
                var headerText = "<table cellspacing='0' cellpadding='0' style=\"width: 100%;\">" + row1 + "</table>";
                e.Item.Cells[3].Text = headerText;
            }
            else if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var goodsType = Convert.ToInt32(((GridDataItem)e.Item).GetDataKeyValue("GoodsType").ToString());
                string row = "<tr>";
                int i = 0;
                foreach (var filiale in FilialeDic)
                {
                    if (internalPriceSetList.Any(ent => ent.GoodsType == goodsType && ent.HostingFilialeId == filiale.ID))
                    {
                        row += "<td style='text-align:center;'>" + GetPent(internalPriceSetList.First(ent => ent.GoodsType == goodsType && ent.HostingFilialeId == filiale.ID).ReserveProfitRatio) + "</td>";
                    }
                    else
                    {
                        row += "<td style='text-align:center;'>" + 0 + "</td>";
                    }
                }
                row = row + "</tr>";
                var headerText = "<table cellspacing='0' cellpadding='0' style=\"width: 100%;\">" + row + "</table>";
                e.Item.Cells[3].Text = headerText;
            }
        }


        public List<FilialeInfo> FilialeDic
        {
            get
            {
                if (ViewState["FilialeDic"] == null) return new List<FilialeInfo>();
                return (List<FilialeInfo>)ViewState["FilialeDic"];
            }
            set { ViewState["FilialeDic"] = value; }
        }

        public IList<InternalPriceSetInfo> internalPriceSetList
        {
            get
            {
                if (ViewState["InternalPriceSetList"] == null) return new List<InternalPriceSetInfo>();
                return (IList<InternalPriceSetInfo>)ViewState["InternalPriceSetList"];
            }
            set { ViewState["InternalPriceSetList"] = value; }
        }
    }
}
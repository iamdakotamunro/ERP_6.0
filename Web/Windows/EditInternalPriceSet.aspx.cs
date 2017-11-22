using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.Enum;
using ERP.Enum.Attribute;
using Keede.Ecsoft.Model;
using ERP.SAL;
using MIS.Enum;

namespace ERP.UI.Web.Windows
{
    public partial class EditInternalPriceSet : WindowsPage
    {
        private readonly IInternalPriceSetDao _internalPriceSetDao = new InternalPriceSetDao(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FilialeDic = MISService.GetAllFiliales().Where(act => act.IsActive && (act.FilialeTypes.Contains((int)FilialeType.LogisticsCompany))).ToList();
                string goodsType = Request.QueryString["GoodsType"];
                BindGroups(Convert.ToInt32(goodsType));
            }
        }

        private void BindGroups(int node)
        {
            var listGroup = _internalPriceSetDao.GetInternalPriceSetInfoList(node);

            String html = string.Empty;
            string result = string.Empty;
            html += "<table border='0' cellpadding='0' cellspacing='0' width='100%'>";

            html += "<tr>";
            html += "<th>商品类型</th>";
            html += "<td>" + EnumAttribute.GetKeyName((GoodsKindType)node) + "</td><input type='hidden' id='goodsType' value=" + node + "> ";
            foreach (var filiale in FilialeDic)
            {
                html += "<tr>";
                html += "<th>" + filiale.Name + "</th>";
                foreach (var list in listGroup)
                {
                   
                    if (list.HostingFilialeId == filiale.ID)
                    {
                        result = list.ReserveProfitRatio;
                        break;
                    }
                }
                html += "<td><input type='text' value='" + result + "' id=" + filiale.ID + " /></td>";
                html += "</tr>";
                result = "0.00";
            }
            html += "</tr>";

            html += "<tr><td style='text-align:right;'><button type='button' onclick='btnSubmit()'>确定</button>&nbsp;&nbsp;&nbsp;<button type='button' onclick='btnCancel()'>取消</button></td></tr>";
            html += "</table>";
            PriceSet.InnerHtml = html;
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
        
    }
}
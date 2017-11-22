using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class CompanyCussentAuthorityPage : System.Web.UI.Page
    {
        static readonly ICompanyCussentRelation _companyCussentRelation = new CompanyCussentRelation(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HiddenField1.Value = Request.QueryString["CompanyId"];
            }
        }

        #region 数据列表相关
        protected void RG_CompanyCussentAuthority_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var companyId = new Guid(Request.QueryString["CompanyId"]);
            GridDataBind(companyId);
        }

        //Grid数据源
        protected void GridDataBind(Guid companyId)
        {
            var companyCussentRelationInfo = new List<CompanyCussentRelationInfo>();
            var list = _companyCussentRelation.GetCompanyCussentRelationInfoList(companyId);
            if (list != null && list.Count > 0)
            {
                foreach (var accountNo in list.Select(ent => ent.AccountNo).Distinct())
                {
                    var info = list.Where(ent => ent.AccountNo == accountNo).ToList();
                    companyCussentRelationInfo.Add(new CompanyCussentRelationInfo
                    {
                        AccountNo = accountNo,
                        AccountName = info.First().AccountName + "(" + accountNo + ")",
                        CompanyId = companyId,
                        CompanyName = info.First().CompanyName,
                        SaleFilialeId = Guid.Empty,
                        SaleFilialeName = String.Join(",", info.Select(ent => ent.SaleFilialeName))
                    });
                }
            }
            RG_CompanyCussentAuthority.DataSource = companyCussentRelationInfo;
        }
        #endregion



        protected void RG_CompanyCussentAuthority_OnItemCommand(object sender, GridCommandEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item != null)
            {
                var dataItem = item;
                var accountNo = dataItem.GetDataKeyValue("AccountNo").ToString();
                var companyId = dataItem.GetDataKeyValue("CompanyId").ToString();
                if (e.CommandName == "Delete")
                {
                    _companyCussentRelation.Delete(accountNo, new Guid(companyId));
                    GridDataBind(new Guid(companyId));
                    RG_CompanyCussentAuthority.Rebind();
                }
            }
        }
    }
}
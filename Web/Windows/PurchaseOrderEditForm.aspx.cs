using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class PurchaseOrderEditForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 加载页面数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Rgd_PurchasingDetail_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var list = new List<PurchasingDetailInfo>();
            Rgd_PurchasingDetail.DataSource = list;
        }
    }
}
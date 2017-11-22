using System;
using System.Collections.Generic;
using ERP.Model;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class AgainStorageRecordApplyInForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RgGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var list = new List<StorageRecordDetailInfo>();
            RGGoods.DataSource = list;
        }
    }
}
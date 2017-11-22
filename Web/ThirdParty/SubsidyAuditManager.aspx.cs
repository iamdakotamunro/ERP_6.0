using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.SAL.MemberCenterSAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;
using System.Text;
using ERP.BLL.Implement.Organization;
using ERP.Enum.ThirdParty;
using ERP.Model.ThirdParty;

namespace ERP.UI.Web.ThirdParty
{
    public partial class SubsidyAuditManager : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSaleFilialeData();
                //LoadSalePlatformData();
                LoadStateData();
                LoadTypeData();
            }
        }

        #region 数据准备

        /// <summary>
        /// 销售公司
        /// </summary>
        public void LoadSaleFilialeData()
        {
            RcbSaleFiliale.DataSource = CacheCollection.Filiale.GetHeadList().Where(ent => ent.IsActive && ent.FilialeTypes.Contains((int)FilialeType.SaleCompany));
            RcbSaleFiliale.DataTextField = "Name";
            RcbSaleFiliale.DataValueField = "ID";
            RcbSaleFiliale.DataBind();
            RcbSaleFiliale.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
        }

        /// <summary>
        /// 销售平台
        /// </summary>
        public void LoadSalePlatformData(Guid saleFilialeId)
        {
            RcbSalePlatform.DataSource = CacheCollection.SalePlatform.GetListByFilialeId(saleFilialeId);
            RcbSalePlatform.DataTextField = "Name";
            RcbSalePlatform.DataValueField = "ID";
            RcbSalePlatform.DataBind();
            RcbSalePlatform.Items.Insert(0, new RadComboBoxItem(string.Empty, Guid.Empty.ToString()));
        }

        /// <summary>
        /// 加载状态列表
        /// </summary>
        public void LoadStateData()
        {
            var list = EnumAttribute.GetDict<SubsidyApplyState>();
            list.Remove((int)SubsidyApplyState.WaitRemittance);
            list.Remove((int)SubsidyApplyState.Finished);
            RcbState.DataSource = list;
            RcbState.DataTextField = "Value";
            RcbState.DataValueField = "Key";
            RcbState.DataBind();
        }

        /// <summary>
        /// 加载补贴类型列表
        /// </summary>
        public void LoadTypeData()
        {
            var list = EnumAttribute.GetDict<SubsidyType>();
            RcbType.DataSource = list;
            RcbType.DataTextField = "Value";
            RcbType.DataValueField = "Key";
            RcbType.DataBind();
        }
        #endregion

        /// <summary>
        /// 销售公司下拉选择
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RcbSaleFilialeOnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var saleFilialeId = new Guid(RcbSaleFiliale.SelectedValue);
            RcbSalePlatform.Items.Clear();

            if (!saleFilialeId.Equals(Guid.Empty))
            {
                RtbMember.Enabled = true;
                LoadSalePlatformData(saleFilialeId);
            }
            else
            {
                RtbMember.Enabled = false;
                RtbMember.Text = string.Empty;
            }
        }

        /// <summary>
        /// 点击搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSearchClick(object sender, EventArgs e)
        {
            RgSubsidy.Rebind();
        }

        /// <summary>
        /// 加载数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgSubsidyNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var startTime = RdpStartTime.SelectedDate != null ? RdpStartTime.SelectedDate.Value : DateTime.MinValue;
            var endTime = RdpEndTime.SelectedDate != null ? RdpEndTime.SelectedDate.Value : DateTime.MinValue;
            var searchKey = RtbMember.Text.Trim();
            var searchState = string.IsNullOrEmpty(RcbState.SelectedValue) ? (int) SubsidyApplyState.All : Convert.ToInt32(RcbState.SelectedValue);
            var searchType = string.IsNullOrEmpty(RcbType.SelectedValue) ? (int)SubsidyType.All : Convert.ToInt32(RcbType.SelectedValue);
            var saleFilialeId = new Guid(RcbSaleFiliale.SelectedValue);
            var salePlatformId = string.IsNullOrEmpty(RcbSalePlatform.SelectedValue) ? Guid.Empty : new Guid(RcbSalePlatform.SelectedValue);
            List<ERP.Model.ThirdParty.SubsidyApplyDTO> dataSource=new List<SubsidyApplyDTO>();
            RgSubsidy.DataSource = dataSource;
        }

        public string GetSaleFilialeName(object saleFilialeId)
        {
            return FilialeManager.GetName(new Guid(saleFilialeId.ToString()));
        }
    }
}
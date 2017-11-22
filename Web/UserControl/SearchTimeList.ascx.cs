using System;
using ERP.Enum;
using ERP.Enum.Attribute;
using Telerik.Web.UI;

namespace ERP.UI.Web.UserControl
{
    public partial class SearchTimeList : System.Web.UI.UserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //if (!Page.IsPostBack)
            //{
                String sValue = rcbSearhTime.SelectedValue;
                rcbSearhTime.DataSource = EnumAttribute.GetDict<SearchTime>();
                rcbSearhTime.DataBind();

                if (!String.IsNullOrEmpty(sValue))
                {
                    rcbSearhTime.SelectedValue = sValue;
                    CurrentSearchTime = (SearchTime)Convert.ToInt32(TehComboBox.SelectedValue);
                }
                else
                    rcbSearhTime.SelectedValue = ((int)CurrentSearchTime).ToString();
            //}
        }

        public SearchTime CurrentSearchTime
        {
            get
            {
                if (ViewState["CurrentSearchTime"] == null)
                    ViewState["CurrentSearchTime"] = DefaultSearchTime;
                return (SearchTime)ViewState["CurrentSearchTime"];
            }
            set
            {
                ViewState["CurrentSearchTime"] = value;
            }
        }
        private SearchTime defaultSearchTime;
        public SearchTime DefaultSearchTime
        {
            set
            {
                defaultSearchTime = value;
            }
            get
            {
                return defaultSearchTime;
            }
        }
        public RadComboBox TehComboBox
        {
            get
            {
                return this.rcbSearhTime;
            }
        }

 

    }
}
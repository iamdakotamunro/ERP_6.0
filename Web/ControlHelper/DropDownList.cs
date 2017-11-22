using System.Collections.Generic;
using Framework.Common;

namespace ERP.UI.Web.ControlHelper
{
    public class DropDownList
    {
        public static void Binding<T>(FineUI.DropDownList control, IEnumerable<T> tList)
        {
            Binding(control, tList, string.Empty, string.Empty, string.Empty);
        }

        public static void Binding<T>(FineUI.DropDownList control, IEnumerable<T> tList, string dataTextField,
            string dataValueField)
        {
            Binding(control, tList, dataTextField, dataValueField, string.Empty);
        }

        public static void Binding<T>(FineUI.DropDownList control, IEnumerable<T> tList, string dataTextField, string dataValueField,
            string defaultSelectedValue)
        {
            if (!dataTextField.IsNullOrEmpty())
            {
                control.DataTextField = dataTextField;
            }
            if (!dataValueField.IsNullOrEmpty())
            {
                control.DataValueField = dataValueField;
            }
            control.DataSource = tList;
            control.DataBind();
            if (!defaultSelectedValue.IsNullOrEmpty())
            {
                control.SelectedValue = defaultSelectedValue;
            }
        }
    }
}
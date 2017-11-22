using System;

namespace ERP.UI.Web.UserControl
{
    /// <summary>
    /// Func : This is a custom checkbox  for keede , This checkbox will have a confirm dialog when click it
    /// Coder: dyy
    /// Date : 2010.23.March
    /// </summary>
    public partial class ConfirmCheckBox : System.Web.UI.UserControl
    {
        /// <summary>
        /// The Click Event for this Control
        /// </summary>
        public event EventHandler CheckedChanged;
        private const String CNST_TIP = "是否确认此操作?";
        private const Int32 CNST_TYP_LENGTH = 30;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            cbState.DataBinding += cbState_DataBinding;
        }

        #region Property

        /// <summary>
        ///Set Or Get enable popup the confirm dialog(Not effect now)
        /// </summary>
        public bool EnableAlert { get; set; }

        //private Boolean _checked;
        /// <summary>
        /// Set Or Get checkbox checked
        /// </summary>
        public Boolean Checked
        {
            get
            {
                if (hfChecked.Value == "true" || hfChecked.Value == "True") { return true; }
                return false;
            }
            set { hfChecked.Value = value.ToString();
            cbState.Checked = value;
            }
        }

        private String tiptrue;
        /// <summary>
        /// Set Or Get the text of the confrim dialog when checking the checkbox 
        /// </summary>
        public String CheckingTip
        {
            get
            {
                if (String.IsNullOrEmpty(tiptrue))
                    return CNST_TIP;
                return tiptrue.Substring(0, CNST_TYP_LENGTH > tiptrue.Length ? tiptrue.Length : CNST_TYP_LENGTH);
            }
            set { tiptrue = value; }
        }

        private String tipfalse;
        /// <summary>
        /// Set Or Get the text of the confrim dialog when unchecking the checkbox 
        /// </summary>
        public String UncheckingTip
        {
            get
            {
                if (String.IsNullOrEmpty(tipfalse))
                    return CNST_TIP;
                return tipfalse.Substring(0, CNST_TYP_LENGTH > tipfalse.Length ? tipfalse.Length : CNST_TYP_LENGTH);
            }
            set { tipfalse = value; }
        }
        #endregion

        protected void lbState_Click(object sender, EventArgs e)
        {
            if (CheckedChanged != null)
            {
                CheckedChanged(this, e);
            }
        }

        void cbState_DataBinding(object sender, EventArgs e)
        {
            cbState.Checked = Checked;
        }
    }
}

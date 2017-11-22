using System;

namespace ERP.UI.Web.UserControl
{
    /// <summary>
    /// </summary>
    public partial class OverwriteCheckBox : System.Web.UI.UserControl
    {
        public event EventHandler CheckedChanged;
        private const String CNST_TIP = "温馨提示：是否确定设置该资金帐号作为主账号，一经设置不能取消和删除！";
        private const Int32 CNST_TYP_LENGTH = 30;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            cbState.DataBinding += cbState_DataBinding;
        }

        #region [属性]

        public bool EnableAlert { get; set; }

        public Boolean Checked
        {
            get
            {
                if (hfChecked.Value == "true" || hfChecked.Value == "True") { return true; }
                return false;
            }
            set
            {
                hfChecked.Value = value.ToString();
                cbState.Checked = value;
            }
        }

        private String tiptrue;


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
            cbState.Enabled = Checked != true;
        }
    }
}

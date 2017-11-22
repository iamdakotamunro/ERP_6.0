using System;
using System.Web.UI;

namespace ERP.UI.Web.Common
{
    [Serializable]
    internal class DynamicControlParameter
    {
        private string _TypeName;
        private string _Id;
        private string _ParentId;
        private object _ViewState;

        public string TypeName
        {
            get { return _TypeName; }
            set { _TypeName = value; }
        }

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string ParentId
        {
            get { return _ParentId; }
            set { _ParentId = value; }
        }

        public object ViewState
        {
            get { return _ViewState; }
            set { _ViewState = value; }
        }
    }

    internal class DynamicControlItem
    {
        public Control Control { get; set; }

        public DynamicControlParameter Parameter { get; set; }
    }
}


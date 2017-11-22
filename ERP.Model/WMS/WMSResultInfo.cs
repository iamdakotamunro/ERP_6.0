using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.WMS
{
    [Serializable]
    public class WMSResultInfo
    {
        public bool IsSuccess { get; set; }

        public string Msg { get; set; }

        public WMSResultInfo(bool isSuccess,string msg)
        {
            IsSuccess = isSuccess;
            Msg = msg;
        }

        public WMSResultInfo() { }
    }
}

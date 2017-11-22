using System;

namespace ERP.Model
{
    /// <summary>申通双11临时接口传输模型   2016年11月3日   陈重文
    /// </summary>
    public class StoTempInfo
    {
        public String OrderNo { get; set; }

        public string billNo { get; set; }

        public string sName { get { return "可得眼镜"; } }

        public string name { get; set; }

        public string prov { get; set; }

        public string city { get; set; }

        public string district { get; set; }

        public string address { get; set; }

        public string districtCode { get; set; }

        public string Container { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.WMS
{
    public class OrderCarriageInfo
    {
        /// <summary>包裹重量
        /// </summary>
        public int PackageWeight { get; set; }

        /// <summary>运费
        /// </summary>
        public decimal Carriage { get; set; }

        /// <summary>省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>城市
        /// </summary>
        public string City { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /// <summary>
    /// 发货地址
    /// </summary>
    [Serializable]
    public class ShippAddress
    {
        /// <summary>
        /// 省份名称(注意：直辖市直接传上海，北京;自治区一定写上xx自治区，其余一定带上省)
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 地区名称
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 城镇名称
        /// </summary>
        public string Town { get; set; }

        /// <summary>
        /// 详细地址（不包含省，市，区，城镇）
        /// </summary>
        public string AddressDetail { get; set; }

        /// <summary>
        /// 发货公司名称（如：百秀大药房） 
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 联系方式（手机或电话）
        /// </summary>
        public string ContactWay { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// 发货仓库ID
        /// </summary>
        public Guid DeliverWarehouseId { get; set; }

        /// <summary>
        /// 销售公司ID或销售平台ID
        /// </summary>
        public Guid SaleCompanyId { get; set; }
    }
}

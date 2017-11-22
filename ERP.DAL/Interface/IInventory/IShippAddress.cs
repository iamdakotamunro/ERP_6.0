using ERP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.DAL.Interface.IInventory
{
    public interface IShippAddress
    {
        /// <summary>
        /// 根据“发货仓库ID”和“销售公司ID或销售平台ID”获取发货地址信息
        /// </summary>
        /// <param name="deliverWarehouseId">发货仓库ID</param>
        /// <param name="saleFilialeId">销售公司ID</param>
        /// <param name="salePlatformId">销售平台ID</param>
        /// <returns></returns>
        ShippAddress GetShippAddress(Guid deliverWarehouseId, Guid saleFilialeId, Guid salePlatformId);
    }
}

using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace ERP.SAL.WMS
{
    /// <summary>仓库权限
    /// </summary>
    [Serializable]
    public class WarehouseAuth
    {
        /// <summary>仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>仓库名称
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>储位信息
        /// </summary>
        public List<StorageAuth> Storages { get; set; }
    }

    /// <summary>仓库权限
    /// </summary>
    [Serializable]
    public class WarehouseFilialeAuth
    {
        /// <summary>仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>仓库名称
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>储位信息
        /// </summary>
        public List<HostingFilialeAuth> FilialeAuths { get; set; }
    }

    /// <summary>储位
    /// </summary>
   [Serializable]
    public class StorageAuth
    {
        /// <summary>储位类型
        /// </summary>
        public byte StorageType { get; set; }

        /// <summary>储位名称
        /// </summary>
        public string StorageTypeName { get; set; }

        /// <summary>
        /// 实体还是虚拟（true 实体，false 虚拟）
        /// </summary>
        public bool IsReal { get; set; }

        /// <summary>托管公司信息
        /// </summary>
        public List<HostingFilialeAuth> Filiales { get; set; }
    }

    /// <summary>托管公司
    /// </summary>
    [Serializable]
    public class HostingFilialeAuth
    {
        /// <summary>托管公司ID
        /// </summary>
        public Guid HostingFilialeId { get; set; }

        /// <summary>托管公司名称
        /// </summary>
        public string HostingFilialeName { get; set; }

        public List<ProxyFiliale> ProxyFiliales { get; set; } 

        public HostingFilialeAuth() { }

        public HostingFilialeAuth(Guid hostingFilialeId, String hostingFilialeName)
        {
            HostingFilialeId = hostingFilialeId;
            HostingFilialeName = hostingFilialeName;
        }
    }

    /// <summary>采购单位对内公司的往来单位
    /// </summary>
    [Serializable]
    public class PurchaseFilialeAuth
    {
        /// <summary>采购单位对内公司ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>采购单位对内公司名称
        /// </summary>
        public string CompanyName { get; set; }
        
    }
    /// <summary>采购单位内外
    /// </summary>
    [Serializable]
    public class PurchaseFilialeInnerOut
    {
        /// <summary>采购单位对内公司ID
        /// </summary>
        public int InnerOutId { get; set; }

        /// <summary>采购单位对内公司名称
        /// </summary>
        public string InnerOutName { get; set; }

    }
    /// <summary>代发公司
    /// </summary>
    [Serializable]
    public class ProxyFiliale
    {
        /// <summary>代发公司ID
        /// </summary>
        public Guid ProxyFilialeId { get; set; }

        /// <summary>代发公司名称
        /// </summary>
        public string ProxyFilialeName { get; set; }

        public List<Int32> GoodsTypes { get; set; }   

        public ProxyFiliale() { }

        public ProxyFiliale(Guid proxyFilialeId,string proxyFilialeName,List<Int32> goodsTypes)
        {
            ProxyFilialeId = proxyFilialeId;
            ProxyFilialeName = proxyFilialeName;
            GoodsTypes = goodsTypes;
        }

    }

    [Serializable]
    public class WarehouseAuthAndFilialeDTO
    {
        /// <summary>
        /// 授权仓库
        /// </summary>
        public Dictionary<Guid,String> WarehouseDics { get; set; }

        /// <summary>
        /// 所有托管公司
        /// </summary>
        public Dictionary<Guid, String> HostingFilialeDic { get; set; }
    }
}

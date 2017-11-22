using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 采购单类
    /// </summary>
    [Serializable]
    [DataContract]
    public class PurchasingInfo
    {
        /// <summary>
        /// 采购单ID
        /// </summary>
        [DataMember]
        public Guid PurchasingID { get; set; }

        /// <summary>
        /// 采购单号
        /// </summary>
        [DataMember]
        public string PurchasingNo { get; set; }

        /// <summary>
        /// 备货日期
        /// </summary>
        [DataMember]
        public string PurchasingToDate { get; set; }

        /// <summary>
        /// 定货日期
        /// </summary>
        [DataMember]
        public string NextPurchasingDate { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        [DataMember]
        public Guid CompanyID { get; set; }

        /// <summary>
        /// 公司名
        /// </summary>
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        [DataMember]
        public Guid FilialeID { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        [DataMember]
        public Guid WarehouseID { get; set; }

        /// <summary>
        /// 采购状态
        /// </summary>
        [DataMember]
        public int PurchasingState { get; set; }

        /// <summary>
        /// 采购类型
        /// </summary>
        [DataMember]
        public int PurchasingType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 采购组ID
        /// </summary>
        [DataMember]
        public Guid PmId { get; set; }

        /// <summary>
        /// 采购组名
        /// </summary>
        [DataMember]
        public string PmName { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        [DataMember]
        public decimal SumPrice { get; set; }

        /// <summary>
        /// 到货时间
        /// </summary>
        [DataMember]
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        [DataMember]
        public string Director { get; set; }

        /// <summary>
        /// 是否异常数据
        /// </summary>
        [DataMember]
        public bool IsException { get; set; }

        /// <summary>
        /// 责任人ID
        /// </summary>
        [DataMember]
        public Guid PersonResponsible { get; set; }

        /// <summary>
        /// 责任人名称
        /// </summary>
        [DataMember]
        public string PersonResponsibleName { get; set; }

        /// <summary>
        /// 采购公司
        /// </summary>
        [DataMember]
        public Guid PurchasingFilialeId { get; set; }

        /// <summary>
        /// 采购部分余下金额
        /// </summary>
        [DataMember]
        public decimal SurplusMoney { get; set; }

        /// <summary>
        /// 采购分组ID
        /// </summary>
        [DataMember]
        public Guid? PurchaseGroupId { get; set; }

        /// <summary>
        /// 是否报税
        /// </summary>
        /// <remarks>目前所有都要报税 By Jerry Bai 2017/6/2</remarks>
        public bool IsOut
        {
            get { return true; }
            set { }
        }

        /// <summary>
        /// 采购单提交人
        /// </summary>
        [DataMember]
        public string PurchasingPersonName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PurchasingInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchasingID">采购单ID</param>
        /// <param name="purchasingNo">采购单号</param>
        /// <param name="companyID">公司ID</param>
        /// <param name="companyName">公司名</param>
        /// <param name="filialeID">分公司ID</param>
        /// <param name="warehouseID">仓库ID</param>
        /// <param name="purchasingState">采购单状态</param>
        /// <param name="purchasingType">采购单类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="description">描述</param>
        /// <param name="pmId">采购组ID</param>
        /// <param name="pmName">采购组名</param>
        public PurchasingInfo(Guid purchasingID, string purchasingNo, Guid companyID, string companyName, Guid filialeID,
            Guid warehouseID, int purchasingState, int purchasingType, DateTime startTime, DateTime endTime,
            string description, Guid pmId, string pmName)
        {
            PurchasingID = purchasingID;
            PurchasingNo = purchasingNo;
            CompanyID = companyID;
            CompanyName = companyName;
            FilialeID = filialeID;
            WarehouseID = warehouseID;
            PurchasingState = purchasingState;
            PurchasingType = purchasingType;
            StartTime = startTime;
            EndTime = endTime;
            Description = description;
            PmId = pmId;
            PmName = pmName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchasingID">采购单ID</param>
        /// <param name="purchasingNo">采购单号</param>
        /// <param name="companyID">公司ID</param>
        /// <param name="companyName">公司名</param>
        /// <param name="filialeID">分公司ID</param>
        /// <param name="warehouseID">仓库ID</param>
        /// <param name="purchasingState">采购单状态</param>
        /// <param name="purchasingType">采购单类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="description">描述</param>
        /// <param name="pmId">采购组ID</param>
        /// <param name="pmName">采购组名</param>
        /// <param name="sumPrice">总价</param>
        public PurchasingInfo(Guid purchasingID, string purchasingNo, Guid companyID, string companyName, Guid filialeID,
            Guid warehouseID, int purchasingState, int purchasingType, DateTime startTime, DateTime endTime,
            string description, Guid pmId, string pmName, decimal sumPrice)
        {
            PurchasingID = purchasingID;
            PurchasingNo = purchasingNo;
            CompanyID = companyID;
            CompanyName = companyName;
            FilialeID = filialeID;
            WarehouseID = warehouseID;
            PurchasingState = purchasingState;
            PurchasingType = purchasingType;
            StartTime = startTime;
            EndTime = endTime;
            Description = description;
            PmId = pmId;
            PmName = pmName;
            SumPrice = sumPrice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchasingID">采购单ID</param>
        /// <param name="purchasingNo">采购单号</param>
        /// <param name="companyID">公司ID</param>
        /// <param name="companyName">公司名</param>
        /// <param name="filialeID">分公司ID</param>
        /// <param name="warehouseID">仓库ID</param>
        /// <param name="purchasingState">采购单状态</param>
        /// <param name="purchasingType">采购单类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="description">描述</param>
        public PurchasingInfo(Guid purchasingID, string purchasingNo, Guid companyID, string companyName, Guid filialeID, Guid warehouseID, int purchasingState, int purchasingType, DateTime startTime, DateTime endTime, string description)
        {
            PurchasingID = purchasingID;
            PurchasingNo = purchasingNo;
            CompanyID = companyID;
            CompanyName = companyName;
            FilialeID = filialeID;
            WarehouseID = warehouseID;
            PurchasingState = purchasingState;
            PurchasingType = purchasingType;
            StartTime = startTime;
            EndTime = endTime;
            Description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <param name="purchasingNo"></param>
        /// <param name="companyID"></param>
        /// <param name="companyName"></param>
        /// <param name="filialeID"></param>
        /// <param name="warehouseID"></param>
        /// <param name="purchasingState"></param>
        /// <param name="purchasingType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="description"></param>
        /// <param name="pmId"></param>
        /// <param name="pmName"></param>
        /// <param name="purchasingPersonName"></param>
        public PurchasingInfo(Guid purchasingID, string purchasingNo, Guid companyID, string companyName, Guid filialeID,
            Guid warehouseID, int purchasingState, int purchasingType, DateTime startTime, DateTime endTime,
            string description, Guid pmId, string pmName, String purchasingPersonName)
        {
            PurchasingID = purchasingID;
            PurchasingNo = purchasingNo;
            CompanyID = companyID;
            CompanyName = companyName;
            FilialeID = filialeID;
            WarehouseID = warehouseID;
            PurchasingState = purchasingState;
            PurchasingType = purchasingType;
            StartTime = startTime;
            EndTime = endTime;
            Description = description;
            PmId = pmId;
            PmName = pmName;
            PurchasingPersonName = purchasingPersonName;
        }
    }
}

using System;
using ERP.Enum;


namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 报备类
    /// </summary>
    [Serializable]
    public class FilingReportInfo
    {
        #region 属性

        /// <summary>
        /// 缺货状态
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 采购量
        /// </summary>
        public int Demand { get; set; }

        /// <summary>
        /// 销售总量
        /// </summary>
        public int SalesNumber { get; set; }

        /// <summary>
        /// 均值备货量
        /// </summary>
        public int MeanNumber { get; set; }

        /// <summary>
        /// 建议备货量
        /// </summary>
        public int ProposalNumber { get; set; }

        /// <summary>
        /// 总备货量
        /// </summary>
        public int TotalNumber { get; set; }

        /// <summary>
        /// 缺货状态
        /// </summary>
        public bool GoodsState { get; set; }

        /// <summary>
        /// 所在备货公司
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 所在公司仓库
        /// </summary>
        public Guid WareHouseId { get; set; }

        /// <summary>
        /// 所在公司仓库
        /// </summary>
        public bool SaleType { get; set; }

        /// <summary>
        /// 备份天数
        /// </summary>
        public int BackupDays { get; set; }

        /// <summary>
        /// 进货类别
        /// </summary>
        public FilingType FilingType { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public Guid SuppliersId { get; set; }

        private string suppliers;               //供应商
        /// <summary>
        /// 供应商
        /// </summary>
        public string Suppliers
        {
            get { return suppliers.Replace("*", ""); }
            set { suppliers = value; }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime Endtime { get; set; }

        /// <summary>
        /// 采购主键
        /// </summary>
        public Guid FilingId { get; set; }

        /// <summary>
        /// 子商品id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 现有库存
        /// </summary>
        public int NonceFilialeGoodsStock { get; set; }

        /// <summary>
        /// 需求量
        /// </summary>
        public int NonceRequest { get; set; }

        /// <summary>
        /// 子商品名
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 采购价
        /// </summary>
        public decimal PurchasePrice { get; set; }

        //private decimal unitPrice;

        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        public FilingReportInfo()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filingId">所在备货公司</param>
        /// <param name="goodsId">子商品id</param>
        /// <param name="goodsName">子商品名</param>
        /// <param name="goodsCode">商品编号</param>
        /// <param name="specification">缺货状态</param>
        /// <param name="nonceFilialeGoodsStock">现有库存</param>
        /// <param name="nonceRequest">需求量</param>
        /// <param name="demand">采购量</param>
        /// <param name="filialeId">所在备货公司</param>
        /// <param name="wareHouseId">所在公司仓库</param>
        /// <param name="suppliers">供应商</param>
        /// <param name="filingType">进货类别</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="purchasePrice">采购价</param>
        /// <param name="suppliersId">供应商ID</param>
        public FilingReportInfo(Guid filingId, Guid goodsId, string goodsName, string goodsCode, string specification, int nonceFilialeGoodsStock,
            int nonceRequest, int demand, Guid filialeId, Guid wareHouseId, string suppliers, FilingType filingType, DateTime startTime, DateTime endTime
            , decimal purchasePrice, Guid suppliersId)
        {
            FilingId = filingId;
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Specification = specification;
            NonceFilialeGoodsStock = nonceFilialeGoodsStock;
            NonceRequest = nonceRequest;
            Demand = demand;
            FilialeId = filialeId;
            WareHouseId = wareHouseId;

            this.suppliers = suppliers;
            FilingType = filingType;
            StartTime = startTime;
            Endtime = endTime;
            PurchasePrice = purchasePrice;
            SuppliersId = suppliersId;
        }
        #endregion
    }
}

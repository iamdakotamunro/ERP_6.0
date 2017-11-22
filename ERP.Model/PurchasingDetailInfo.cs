using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class PurchasingDetailInfo
    {
        /// <summary>
        /// 采购商品ID
        /// </summary>
        public Guid PurchasingGoodsID { get; set; }

        /// <summary>
        /// 采购单ID
        /// </summary>
        public Guid PurchasingID { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsID { get; set; }

        /// <summary>
        /// 商品名
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid CompanyID { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 计划采购量（double）
        /// </summary>
        public double PlanQuantity { get; set; }

        /// <summary>
        /// 计划采购量（decimal）
        /// </summary>
        public Decimal DPlanQuantity
        {
            get { return Convert.ToDecimal(PlanQuantity); }
        }

        /// <summary>
        /// 实际采购量（double）
        /// </summary>
        public double RealityQuantity { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 商品类别 :赠品 非赠品 未定价商品
        /// </summary>
        public int PurchasingGoodsType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 计划报备数量
        /// </summary>
        public double PlanStocking { get; set; }

        /// <summary>
        /// 计划报备数量
        /// </summary>
        public double DayAvgStocking { get; set; }

        /// <summary>
        /// 备货天数
        /// </summary>
        public int SixtyDays { get; set; }

        /// <summary>
        /// 到货日期 (备货日+备货天数)
        /// </summary>
        public string PurchasingToDate { get; set; }

        /// <summary>
        /// 15天销售量
        /// </summary>
        public int Fifteentotal { get; set; }

        /// <summary>
        /// 60天日均销量
        /// </summary>
        public int SixtyDaySales { get; set; }
        /// <summary>
        /// 30天日均销量
        /// </summary>
        public int ThirtyDaySales { get; set; }
        /// <summary>
        /// 11天日均销量
        /// </summary>
        public int ElevenDaySales { get; set; }
        /// <summary>
        /// 是否异常数据
        /// </summary>
        public bool IsException { get; set; }
        /// <summary>
        /// 批文号
        /// </summary>
        public string ApprovalNo { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseID { get; set; }

        /// <summary>
        /// 修改前价格(或绑定价格)
        /// </summary>
        public decimal CPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PurchasingDetailInfo()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchasingID">采购单ID</param>
        /// <param name="goodsID">商品ID</param>
        /// <param name="goodsName">商品名</param>
        /// <param name="units">计量单位</param>
        /// <param name="goodsCode">商品编码</param>
        /// <param name="specification">规格</param>
        /// <param name="companyID">公司ID</param>
        /// <param name="price">价格</param>
        /// <param name="planQuantity">计划采购量（double）</param>
        /// <param name="realityQuantity">实际采购量（double）</param>
        /// <param name="state">状态</param>
        /// <param name="description">描述</param>
        /// <param name="purchasingGoodsID">采购商品ID</param>
        /// <param name="purchasingGoodsType">商品类别 :赠品 非赠品 未定价商品</param>
        /// <param name="planStocking">计划报备数量</param>
        /// <param name="dayAvgStocking">计划报备数量</param>
        /// <param name="sixtyDays">60天日均销量</param>
        public PurchasingDetailInfo(Guid purchasingID, Guid goodsID, string goodsName, string units, string goodsCode, string specification,
            Guid companyID, decimal price, double planQuantity, double realityQuantity, int state, string description, Guid purchasingGoodsID,
            int purchasingGoodsType, double planStocking, double dayAvgStocking, int sixtyDays)
        {
            PurchasingID = purchasingID;
            GoodsID = goodsID;
            GoodsName = goodsName;
            Units = units;
            GoodsCode = goodsCode;
            Specification = specification;
            CompanyID = companyID;
            Price = price;
            PlanQuantity = planQuantity;
            RealityQuantity = realityQuantity;
            State = state;
            Description = description;
            PurchasingGoodsID = purchasingGoodsID;
            PurchasingGoodsType = purchasingGoodsType;
            PlanStocking = planStocking;
            DayAvgStocking = dayAvgStocking;
            SixtyDays = sixtyDays;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchasingID">采购单ID</param>
        /// <param name="goodsID">商品ID</param>
        /// <param name="goodsName">商品名</param>
        /// <param name="units">计量单位</param>
        /// <param name="goodsCode">商品编码</param>
        /// <param name="specification">规格</param>
        /// <param name="companyID">公司ID</param>
        /// <param name="price">价格</param>
        /// <param name="planQuantity">计划采购量（double）</param>
        /// <param name="realityQuantity">实际采购量（double）</param>
        /// <param name="state">状态</param>
        /// <param name="description">描述</param>
        /// <param name="purchasingGoodsID">采购商品ID</param>
        /// <param name="purchasingGoodsType">商品类别 :赠品 非赠品 未定价商品</param>
        /// <param name="planStocking">计划报备数量</param>
        /// <param name="dayAvgStocking">计划报备数量</param>
        /// <param name="sixtyDays">60天日均销量</param>
        /// <param name="fifteentotal">15天销售量</param>
        public PurchasingDetailInfo(Guid purchasingID, Guid goodsID, string goodsName, string units, string goodsCode, string specification,
            Guid companyID, decimal price, double planQuantity, double realityQuantity, int state, string description, Guid purchasingGoodsID,
            int purchasingGoodsType, double planStocking, double dayAvgStocking, int sixtyDays, int fifteentotal)
        {
            PurchasingID = purchasingID;
            GoodsID = goodsID;
            GoodsName = goodsName;
            Units = units;
            GoodsCode = goodsCode;
            Specification = specification;
            CompanyID = companyID;
            Price = price;
            PlanQuantity = planQuantity;
            RealityQuantity = realityQuantity;
            State = state;
            Description = description;
            PurchasingGoodsID = purchasingGoodsID;
            PurchasingGoodsType = purchasingGoodsType;
            PlanStocking = planStocking;
            DayAvgStocking = dayAvgStocking;
            SixtyDays = sixtyDays;
            Fifteentotal = Math.Abs(fifteentotal);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchasingID">采购单ID</param>
        /// <param name="goodsID">商品ID</param>
        /// <param name="goodsName">商品名</param>
        /// <param name="units">计量单位</param>
        /// <param name="goodsCode">商品编码</param>
        /// <param name="specification">规格</param>
        /// <param name="companyID">公司ID</param>
        /// <param name="price">价格</param>
        /// <param name="planQuantity">计划采购量（double）</param>
        /// <param name="realityQuantity">实际采购量（double）</param>
        /// <param name="state">状态</param>
        /// <param name="description">描述</param>
        /// <param name="purchasingGoodsID">采购商品ID</param>
        /// <param name="purchasingGoodsType">商品类别 :赠品 非赠品 未定价商品</param>
        public PurchasingDetailInfo(Guid purchasingID, Guid goodsID, string goodsName, string units, string goodsCode, string specification, Guid companyID, decimal price, double planQuantity, double realityQuantity, int state, string description, Guid purchasingGoodsID, int purchasingGoodsType)
        {
            PurchasingID = purchasingID;
            GoodsID = goodsID;
            GoodsName = goodsName;
            Units = units;
            GoodsCode = goodsCode;
            Specification = specification;
            CompanyID = companyID;
            Price = price;
            PlanQuantity = planQuantity;
            RealityQuantity = realityQuantity;
            State = state;
            Description = description;
            PurchasingGoodsID = purchasingGoodsID;
            PurchasingGoodsType = purchasingGoodsType;
        }

        public void SetPrice(decimal price, decimal cPrice)
        {
            Price = price;
            CPrice = cPrice;
        }
    }
}

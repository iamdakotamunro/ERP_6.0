using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品月需求
    /// </summary>
    [Serializable]
    public class GoodsMonthDemandInfo
    {
        /// <summary>
        /// 需求ID
        /// </summary>
        public Guid DemandId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 需求量
        /// </summary>
        public double Demand { get; set; }

        /// <summary>
        /// 公司名
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsMonthDemandInfo() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="demandId">需求ID</param>
        /// <param name="goodsId">商品ID</param>
        /// <param name="goodsName">商品名</param>
        /// <param name="goodsCode">商品编号</param>
        /// <param name="demand">需求量</param>
        /// <param name="companyName">公司名</param>
        /// <param name="updateTime">更新时间</param>
        /// <param name="filialeId">分公司ID</param>
        /// <param name="specification">规格</param>
        /// <param name="times">时间</param>
        public GoodsMonthDemandInfo(Guid demandId, Guid goodsId, string goodsName, string goodsCode, double demand, string companyName, DateTime updateTime, Guid filialeId, string specification, int times)
        {
            DemandId = demandId;
            GoodsId = goodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Demand = demand;
            CompanyName = companyName;
            UpdateTime = updateTime;
            FilialeId = filialeId;
            Specification = specification;
            Times = times;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is GoodsMonthDemandInfo)
                return (obj as GoodsMonthDemandInfo).GoodsId == GoodsId && (obj as GoodsMonthDemandInfo).Specification == Specification;
            return base.Equals(obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

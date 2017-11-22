using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 进货申报商品统计实体
    /// 备注:统计一段时间内的进货申报商品的采购次数及总量
    /// </summary>
    [Serializable]
    public class PurchaseStatisticsInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public PurchaseStatisticsInfo()
        {
        }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsID { get; set; }

        /// <summary>
        /// 商品名
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 采购次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 商品总量
        /// </summary>
        public double TotalCount { get; set; }

        /// <summary>
        /// 统计一段时间内的进货申报商品的采购次数及总量
        /// </summary>
        /// <param name="goodsid">商品ID</param>
        /// <param name="goodsname">商品名</param>
        /// <param name="totalcount">商品总量</param>
        /// <param name="count">采购次数</param>
        public PurchaseStatisticsInfo(Guid goodsid, string goodsname, double totalcount, int count)
        {
            GoodsID = goodsid;
            GoodsName = goodsname;
            TotalCount = totalcount;
            Count = count;
        }
    }
}

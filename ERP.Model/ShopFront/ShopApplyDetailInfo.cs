using System;
using System.Runtime.Serialization;

namespace ERP.Model.ShopFront
{
    /// <summary>
    /// 联盟店退换货申请明细共有模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShopApplyDetailInfo
    {
        #region
        /// <summary>
        /// 退换货明细Id
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

        /// <summary>
        /// 退换货申请Id
        /// </summary>
        [DataMember]
        public Guid ApplyID { get; set; }

        /// <summary>
        /// 退换货主商品Id
        /// </summary>
        [DataMember]
        public Guid GoodsID { get; set; }

        /// <summary>
        /// 退换货子商品Id
        /// </summary>
        [DataMember]
        public Guid RealGoodsID { get; set; }

        /// <summary>
        /// 退换货商品名称
        /// </summary>
        [DataMember]
        public string GoodsName { get; set; }

        /// <summary>
        /// 退换货商品code
        /// </summary>
        [DataMember]
        public string GoodsCode { get; set; }

        /// <summary>
        /// 退换货商品规格
        /// </summary>
        [DataMember]
        public string Specification { get; set; }

        /// <summary>
        /// 退换货商品价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 退换货商品数量
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        [DataMember]
        public string Units { get; set; }

        /// <summary>
        /// 批号
        /// add by lcj at 2015.9.29
        /// </summary>
        [DataMember]
        public string BatchNo { get; set; }

        /// <summary>
        /// 有效日期
        /// </summary>
        /// zal 2016-04-07
        [DataMember]
        public DateTime? EffectiveDate { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ShopApplyDetailInfo()
        {
            
        }

        /// <summary>
        /// 构造函数(退货)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applyId"></param>
        /// <param name="goodsId"></param>
        /// <param name="realGoodsId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsCode"></param>
        /// <param name="specification"></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <param name="units"> </param>
        public ShopApplyDetailInfo(Guid id, Guid applyId, Guid goodsId, Guid realGoodsId, string goodsName, string goodsCode,
            string specification,decimal price,int quantity,string units)
        {
            ID = id;
            ApplyID = applyId;
            GoodsID = goodsId;
            RealGoodsID = realGoodsId;
            GoodsName = goodsName;
            GoodsCode = goodsCode;
            Specification = specification;
            Price = price;
            Quantity = quantity;
            Units = units;
        }
        #endregion
    }
}

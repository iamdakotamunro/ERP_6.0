using System;
using System.Runtime.Serialization;

namespace ERP.Model.ShopFront
{
    /// <summary>
    /// 退换货申请明细模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShopExchangedApplyDetailInfo:ShopApplyDetailInfo
    {
        #region  换货特有
        /// <summary>
        /// 换货主商品Id
        /// </summary>
        [DataMember]
        public Guid BarterGoodsID { get; set; }

        /// <summary>
        /// 换货子商品Id
        /// </summary>
        [DataMember]
        public Guid BarterRealGoodsID { get; set; }

        /// <summary>
        /// 换货商品名称
        /// </summary>
        [DataMember]
        public string BarterGoodsName { get; set; }

        /// <summary>
        /// 换货商品编号
        /// </summary>
        [DataMember]
        public string BarterGoodsCode { get; set; }

        /// <summary>
        /// 换货商品规格
        /// </summary>
        [DataMember]
        public string BarterSpecification { get; set; }

        #endregion

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ShopExchangedApplyDetailInfo()
        {
            
        }

        /// <summary>
        /// 构造函数(换货)
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
        /// <param name="units"></param>
        /// <param name="barterGoodsId"> </param>
        /// <param name="barterRealGoodsId"> </param>
        /// <param name="barterGoodsName"> </param>
        /// <param name="barterGoodsCode"> </param>
        /// <param name="barterSpecification"> </param>
        public ShopExchangedApplyDetailInfo(Guid id, Guid applyId, Guid goodsId, Guid realGoodsId, string goodsName, string goodsCode,
            string specification, decimal price, int quantity,string units, Guid barterGoodsId, Guid barterRealGoodsId, string barterGoodsName,
            string barterGoodsCode, string barterSpecification)
            :base(id,applyId,goodsId,realGoodsId,goodsName,goodsCode,specification,price,quantity,units)
        {
            BarterGoodsID = barterGoodsId;
            BarterRealGoodsID = barterRealGoodsId;
            BarterGoodsName = barterGoodsName;
            BarterGoodsCode = barterGoodsCode;
            BarterSpecification = barterSpecification;
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace ERP.Model
{
    /// <summary>
    /// 商品税率比例
    /// </summary>
    [Serializable]
    [DataContract]
    public class TaxrateProportionInfo
    {
        /// <summary>
        /// 主键标识
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        [DataMember]
        public int GoodsType { get; set; }

        /// <summary>
        /// 商品类型编码
        /// </summary>
        [DataMember]
        public string GoodsTypeCode { get; set; }

        /// <summary>
        /// 当前税率值(如：5.00%，Percentage=5.00，ViewPercentage=5.00% UsingPercentage=0.05)
        /// </summary>
        [DataMember]
        public decimal Percentage { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [DataMember]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [DataMember]
        public string Operator { get; set; }

        /// <summary>
        /// 操作类型(新增、编辑、删除)
        /// </summary>
        [DataMember]
        public byte OperateType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 显示的税率值(如：当前税率值为5,那么现实的值为5.00%)
        /// </summary>
        [IgnoreDataMember]
        public string ViewPercentage => string.Format("{0}%", Percentage.ToString());

        /// <summary>
        /// 用于计算的税率值(如：当前税率值为5,那么用于计算的值为0.05)
        /// </summary>
        [IgnoreDataMember]
        public decimal UsingPercentage => (Percentage / 100);

        /// <summary>
        /// 
        /// </summary>
        public TaxrateProportionInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="goodsType"></param>
        /// <param name="goodsTypeCode"></param>
        /// <param name="percentage"></param>
        /// <param name="updateDate"></param>
        /// <param name="operatorName"></param>
        /// <param name="operateType"></param>
        /// <param name="remark"></param>
        public TaxrateProportionInfo(Guid id, int goodsType,string goodsTypeCode, decimal percentage, DateTime updateDate,string operatorName,byte operateType ,string remark)
        {
            Id = id;
            GoodsType = goodsType;
            Percentage = percentage;
            UpdateDate = updateDate;
            Operator = operatorName;
            OperateType = operateType;
            Remark = remark;
            GoodsTypeCode = goodsTypeCode;
        }
    }
}

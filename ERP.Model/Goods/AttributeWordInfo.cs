using System;
using System.Collections.Generic;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 词绑定组,如：品牌（博士伦，强生等）
    /// </summary>
    [Serializable]
    public class AttributeWordInfo
    {
        ///<summary>
        ///</summary>
        public int WordId { get; set; }

        /// <summary>
        /// 所属属性组ID
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 词名
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// 序号（同一组类从1开始）
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 比较方式(大于等于，等于，小于等于，两者之间)
        /// </summary>
        public int CompareType { get; set; }

        /// <summary>
        /// 数值比较是最小值
        /// </summary>
        public string WordValue { get; set; }

        /// <summary>
        /// 比较值上限
        /// </summary>
        public string TopValue { get; set; }

        /// <summary>
        /// 是否在前台显示（默认：是）
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GoodsQuantity { set; get; }

        /// <summary>
        /// 属性词图片
        /// </summary>
        /// zal 2016-04-13
        public string AttrWordImage { set; get; }

        #region --> 扩展
        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GoodsCode { get; set; }

        #endregion
    }

    /// <summary>
    /// 属性词和商品
    /// </summary>
    [Serializable]
    public class AttrWordsGoodsInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 多选时1,2,3
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AttributeGroupInfo AttrGroupInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, string> WordIdAndWordName { get; set; }
    }
}

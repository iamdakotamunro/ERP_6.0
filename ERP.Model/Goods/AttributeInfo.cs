using System;
using System.Collections.Generic;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 商品绑定组，关键词
    /// </summary>
    [Serializable]
    public class AttributeInfo
    {
        ///<summary>
        /// 商品编号
        ///</summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        ///<summary>
        /// 分组编号
        ///</summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }

        ///<summary>
        /// 关键词编号
        ///</summary>
        public int WordId { get; set; }

        ///<summary>
        /// 比较值
        ///</summary>
        public string Value { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int MatchType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMChoice { get; set; }

        /// <summary>
        /// IdAndName
        /// </summary>
        public List<AttributeWordInfo> AttributeWordList { get; set; }
    }
}

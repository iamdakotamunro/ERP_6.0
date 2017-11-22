using System;
using System.Collections.Generic;

namespace ERP.Model.Goods
{
    ///<summary>
    /// 商品属性分组,如：品牌，价位，颜色
    ///</summary>
    [Serializable]
    public class AttributeGroupInfo
    {
        ///<summary>
        /// 商品属性组编号
        ///</summary>
        public int GroupId { get; set; }

        ///<summary>
        /// 比较类型（文字匹配，数值匹配）
        ///</summary>
        public int MatchType { get; set; }

        ///<summary>
        /// 组名
        ///</summary>
        public string GroupName { get; set; }

        ///<summary>
        /// 排序
        ///</summary>
        public int OrderIndex { get; set; }

        ///<summary>
        /// 是否启用筛选
        ///</summary>
        public bool EnabledFilter { get; set; }

        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMChoice { get; set; }
        /// <summary>
        /// 是否优先筛选
        /// zal 2015-09-09
        /// </summary>
        public bool IsPriorityFilter { set; get; }
        /// <summary>
        /// 属性词 
        /// </summary>
        public List<AttributeWordInfo> AttrWordsList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSelect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GoodsQuantity { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 是否上传图片
        /// zal 2016-04-08
        /// </summary>
        public bool IsUploadImage { set; get; }
    }
}

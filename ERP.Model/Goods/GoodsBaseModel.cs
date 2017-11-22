using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model.Goods
{
    /// <summary>商品基础模型【可扩展】      ADD   2014-08-20   陈重文   
    /// </summary>
    public class GoodsBaseModel
    {
        /// <summary>商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>商品Id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>商品名称
        /// </summary>
        public string GoodsName { get; set; }
    }
}

using System;

namespace ERP.Model
{
    /// <summary>
    /// 到货声明
    /// </summary>
    public class GoodsStatementInfo
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
        public string Specification { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsStatement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime StatementTime { get; set; }
    }
}

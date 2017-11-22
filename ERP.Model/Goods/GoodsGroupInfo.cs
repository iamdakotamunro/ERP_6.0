using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GoodsGroupInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GroupName { get; set; }
    }
}

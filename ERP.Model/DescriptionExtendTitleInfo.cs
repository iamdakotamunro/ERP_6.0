using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DescriptionExtendTitleInfo
    {
        /// <summary>
        /// ID 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 标题 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 位置 
        /// </summary>
        public int Position { get; set; }
    }
}

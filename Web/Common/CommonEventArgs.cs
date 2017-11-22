using System;

namespace ERP.UI.Web.Common
{
    /// <summary>
    /// 通用搜索事件 by dyy
    /// </summary>
    public class CommonSearchClickEventArgs : EventArgs
    {
        /// <summary>
        /// 需要查询的关键字
        /// </summary>
        public string SearchText { get; set; }
    }
}
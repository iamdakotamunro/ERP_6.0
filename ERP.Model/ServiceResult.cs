using System;
using System.Collections.Generic;

namespace ERP.Model
{
    [Serializable]
    public class ServiceResult<T> where T: new ()
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 分页则为总条数，不分为返回条数
        /// </summary>
        public int DataCount { get; set; }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        public IList<T>  DataList { get; set; }

        /// <summary>
        /// 返回单条记录
        /// </summary>
        public T Info { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}

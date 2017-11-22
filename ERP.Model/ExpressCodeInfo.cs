using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 快递号接口查询信息
    /// </summary>
    [Serializable]
    public class ExpressCodeInfo
    {
        /// <summary>
        /// 快递ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 快递代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 0.关闭,1.打开
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// 区别请求方式
        /// </summary>
        public int ApiType { get; set; }
    }
}

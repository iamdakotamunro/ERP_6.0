//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2006年5月19日
// 文件创建人:马力
// 最后修改时间:2006年5月19日
// 最后一次修改人:马力
//================================================
using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 订单模型
    /// </summary>
    [Serializable]
    public class SalesReturnInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 需求数量
        /// </summary>
        public int Counts { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        public int Finish { get; set; }

        /// <summary>
        /// Counts/Finish的值
        /// </summary>
        public double Per { get; set; }


        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public SalesReturnInfo() { }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="goodsid">商品ID</param>
        /// <param name="goodsname">商品名</param>
        /// <param name="goodscode">商品编号</param>
        /// <param name="units">计量单位</param>
        /// <param name="counts">需求数量</param>
        /// <param name="finish">完成数量</param>
        /// <param name="per">Counts/Finish的值</param>
        public SalesReturnInfo(Guid goodsid, string goodsname, string goodscode, string units, int counts, int finish, double per)
        {
            GoodsId = goodsid;
            GoodsName = goodsname;
            GoodsCode = goodscode;
            Units = units;
            Counts = counts;
            Finish = finish;
            Per = per;
        }
    }
}

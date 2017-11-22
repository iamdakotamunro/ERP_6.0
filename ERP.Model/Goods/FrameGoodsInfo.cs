using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FrameGoodsInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 框架腿长
        /// </summary>
        public int TempleLength { get; set; }

        /// <summary>
        /// 框架外宽
        /// </summary>
        public int Besiclometer { get; set; }

        /// <summary>
        /// 镜架内宽
        /// </summary>
        public int FrameWithinWidth { get; set; }

        /// <summary>
        /// 镜片高度
        /// </summary>
        public int OpticalVerticalHeight { get; set; }

        /// <summary>
        /// 镜片宽度
        /// </summary>
        public int EyeSize { get; set; }

        /// <summary>
        /// 鼻梁宽度
        /// </summary>
        public int NoseWidth { get; set; }

    }
}

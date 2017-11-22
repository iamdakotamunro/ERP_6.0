using System;

namespace ERP.Model.Goods
{
    ///<summary>
    ///</summary>
    public class GoodsSyncInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 如果是门店，则FromSourceId字段，保存的是门店公司id如果是网站，则FromSourceId字段，保存的是 website网站的标识id
        /// </summary>
        public Guid SaleFilialeId { get; set; }


        /// <summary>
        /// False未同步,TURE已同步
        /// </summary>
        public Boolean SyncChecked { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsSyncInfo()
        {
            SyncChecked = false;//未选中
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="saleFilialeId">如果是门店，则FromSourceId字段，保存的是门店公司id如果是网站，则FromSourceId字段，保存的是 website网站的标识id</param>
        /// <param name="syncChecked">False未同步,TURE已同步</param>
        public GoodsSyncInfo(Guid goodsId, Guid saleFilialeId, Boolean syncChecked)
            : this()
        {
            GoodsId = goodsId;
            SaleFilialeId = saleFilialeId;
            SyncChecked = syncChecked;
        }
    }
}

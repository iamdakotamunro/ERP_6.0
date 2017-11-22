//Author: zal
//createtime:2016/03/23
//Description: 

using System;
using System.Data;

namespace ERP.Model
{
    public class PurchasingGoodsModel
    {
        public PurchasingGoodsModel()
        { }
        public PurchasingGoodsModel(IDataReader reader)
        {
            _goodsId = Guid.Parse(reader["GoodsID"].ToString());
        }

        #region 数据库字段
        private Guid _goodsId;
        #endregion

        #region 字段属性
        /// <summary>
        /// 主键GoodsId
        /// </summary>
        public Guid GoodsId
        {
            set { _goodsId = value; }
            get { return _goodsId; }
        }
        #endregion
    }
}

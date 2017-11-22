//Author: 张安龙
//createtime:2015/7/17 14:06:21
//Description: 

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ERP.Model
{
    public class GoodsPriceChange
    {
        public GoodsPriceChange()
        { }
        public GoodsPriceChange(IDataReader reader)
        {
            _id = System.Guid.Parse(reader["Id"].ToString());
            if (reader["Name"] != DBNull.Value)
            {
                _name = reader["Name"].ToString();
            }
            if (reader["Datetime"] != DBNull.Value)
            {
                _datetime = System.DateTime.Parse(reader["Datetime"].ToString());
            }
            if (reader["GoodsId"] != DBNull.Value)
            {
                _goodsId = System.Guid.Parse(reader["GoodsId"].ToString());
            }
            if (reader["GoodsName"] != DBNull.Value)
            {
                _goodsName = reader["GoodsName"].ToString();
            }
            if (reader["GoodsCode"] != DBNull.Value)
            {
                _goodsCode = reader["GoodsCode"].ToString();
            }
            if (reader["SaleFilialeId"] != DBNull.Value)
            {
                _saleFilialeId = System.Guid.Parse(reader["SaleFilialeId"].ToString());
            }
            if (reader["SaleFilialeName"] != DBNull.Value)
            {
                _saleFilialeName = reader["SaleFilialeName"].ToString();
            }
            if (reader["SalePlatformId"] != DBNull.Value)
            {
                _salePlatformId = System.Guid.Parse(reader["SalePlatformId"].ToString());
            }
            if (reader["SalePlatformName"] != DBNull.Value)
            {
                _salePlatformName = reader["SalePlatformName"].ToString();
            }
            if (reader["OldPrice"] != DBNull.Value)
            {
                _oldPrice = decimal.Parse(reader["OldPrice"].ToString());
            }
            if (reader["NewPrice"] != DBNull.Value)
            {
                _newPrice = decimal.Parse(reader["NewPrice"].ToString());
            }
            if (reader["Quota"] != DBNull.Value)
            {
                _quota = decimal.Parse(reader["Quota"].ToString());
            }
            if (reader["Type"] != DBNull.Value)
            {
                _type = int.Parse(reader["Type"].ToString());
            }
        }

        #region 数据库字段
        private System.Guid _id;
        private string _name;
        private System.DateTime _datetime;
        private System.Guid _goodsId;
        private string _goodsName;
        private string _goodsCode;
        private System.Guid _saleFilialeId;
        private string _saleFilialeName;
        private System.Guid _salePlatformId;
        private string _salePlatformName;
        private decimal _oldPrice;
        private decimal _newPrice;
        private decimal _quota;
        private int _type;
        #endregion

        #region 字段属性
        /// <summary>
        /// 主键Id
        /// </summary>
        public System.Guid Id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 修改人姓名
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 修改时间
        /// </summary>
        public System.DateTime Datetime
        {
            set { _datetime = value; }
            get { return _datetime; }
        }
        /// <summary>
        /// 商品id
        /// </summary>
        public System.Guid GoodsId
        {
            set { _goodsId = value; }
            get { return _goodsId; }
        }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName
        {
            set { _goodsName = value; }
            get { return _goodsName; }
        }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode
        {
            set { _goodsCode = value; }
            get { return _goodsCode; }
        }
        /// <summary>
        /// 公司ID
        /// </summary>
        public System.Guid SaleFilialeId
        {
            set { _saleFilialeId = value; }
            get { return _saleFilialeId; }
        }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string SaleFilialeName
        {
            set { _saleFilialeName = value; }
            get { return _saleFilialeName; }
        }
        /// <summary>
        /// 销售平台ID
        /// </summary>
        public System.Guid SalePlatformId
        {
            set { _salePlatformId = value; }
            get { return _salePlatformId; }
        }
        /// <summary>
        /// 销售平台名称
        /// </summary>
        public string SalePlatformName
        {
            set { _salePlatformName = value; }
            get { return _salePlatformName; }
        }
        /// <summary>
        /// 修改前价格
        /// </summary>
        public decimal OldPrice
        {
            set { _oldPrice = value; }
            get { return _oldPrice; }
        }
        /// <summary>
        /// 修改后价格
        /// </summary>
        public decimal NewPrice
        {
            set { _newPrice = value; }
            get { return _newPrice; }
        }
        /// <summary>
        /// 修改额度
        /// </summary>
        public decimal Quota
        {
            set { _quota = value; }
            get { return _quota; }
        }
        /// <summary>
        /// 0:销售价；1:加盟价；2:批发价
        /// </summary>
        public int Type
        {
            set { _type = value; }
            get { return _type; }
        }
        #endregion
    }
}
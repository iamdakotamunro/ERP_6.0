//Author: zal
//createtime:2016-5-10 19:38:06
//Description: 

using System;
using System.Data;
namespace ERP.Model
{
    public class StockRequirementDetailInfo
    { 
        public StockRequirementDetailInfo()
        {}
        public StockRequirementDetailInfo(IDataReader reader)
        {
            _id = System.Guid.Parse(reader["Id"].ToString());
            if(reader["SourceName"]!=DBNull.Value)
            {
                _sourceName = reader["SourceName"].ToString();
            }           
            if(reader["ReceiptNo"]!=DBNull.Value)
            {
                _receiptNo = reader["ReceiptNo"].ToString();
            }           
            _goodsId = System.Guid.Parse(reader["GoodsId"].ToString());
            if(reader["GoodsName"]!=DBNull.Value)
            {
                _goodsName = reader["GoodsName"].ToString();
            }           
            _warehouseId = System.Guid.Parse(reader["WarehouseId"].ToString());
            _quantity = int.Parse(reader["Quantity"].ToString());
            if(reader["DemandQuantity"]!=DBNull.Value)
            {
                _demandQuantity = int.Parse(reader["DemandQuantity"].ToString());
            }           
            if(reader["NonceGoodsStock"]!=DBNull.Value)
            {
                _nonceGoodsStock = int.Parse(reader["NonceGoodsStock"].ToString());
            }           
            if(reader["CreateDateTime"]!=DBNull.Value)
            {
                _createDateTime = System.DateTime.Parse(reader["CreateDateTime"].ToString());
            }           
            if(reader["Remark"]!=DBNull.Value)
            {
                _remark = reader["Remark"].ToString();
            }           
        }
        
        #region 数据库字段
        private System.Guid _id;   
        private string _sourceName;   
        private string _receiptNo;   
        private System.Guid _goodsId;   
        private string _goodsName;   
        private System.Guid _warehouseId;   
        private int _quantity;   
        private int _demandQuantity;   
        private int _nonceGoodsStock;   
        private System.DateTime _createDateTime;   
        private string _remark;   
        #endregion  
        
        #region 字段属性
        /// <summary>
        /// 
        /// </summary>
        public System.Guid  Id 
        {
            set{_id=value;}
            get{return _id;}
        }
        /// <summary>
        /// 
        /// </summary>
        public string  SourceName 
        {
            set{_sourceName=value;}
            get{return _sourceName;}
        }
        /// <summary>
        /// 
        /// </summary>
        public string  ReceiptNo 
        {
            set{_receiptNo=value;}
            get{return _receiptNo;}
        }
        /// <summary>
        /// 
        /// </summary>
        public System.Guid  GoodsId 
        {
            set{_goodsId=value;}
            get{return _goodsId;}
        }
        /// <summary>
        /// 
        /// </summary>
        public string  GoodsName 
        {
            set{_goodsName=value;}
            get{return _goodsName;}
        }
        /// <summary>
        /// 
        /// </summary>
        public System.Guid  WarehouseId 
        {
            set{_warehouseId=value;}
            get{return _warehouseId;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int  Quantity 
        {
            set{_quantity=value;}
            get{return _quantity;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int  DemandQuantity 
        {
            set{_demandQuantity=value;}
            get{return _demandQuantity;}
        }
        /// <summary>
        /// 
        /// </summary>
        public int  NonceGoodsStock 
        {
            set{_nonceGoodsStock=value;}
            get{return _nonceGoodsStock;}
        }
        /// <summary>
        /// 
        /// </summary>
        public System.DateTime  CreateDateTime 
        {
            set{_createDateTime=value;}
            get{return _createDateTime;}
        }
        /// <summary>
        /// 
        /// </summary>
        public string  Remark 
        {
            set{_remark=value;}
            get{return _remark;}
        }
        #endregion  
    }
}
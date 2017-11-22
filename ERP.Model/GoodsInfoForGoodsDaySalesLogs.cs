//Author: zal
//createtime:2016/1/29 14:19:50
//Description: 

using System;
using System.Data;
namespace ERP.Model
{
    public class GoodsInfoForGoodsDaySalesLogs
    { 
        public GoodsInfoForGoodsDaySalesLogs()
        {}
        public GoodsInfoForGoodsDaySalesLogs(IDataReader reader)
        {
            _id = Guid.Parse(reader["Id"].ToString());
            if(reader["GoodsId"]!=DBNull.Value)
            {
                _goodsId = Guid.Parse(reader["GoodsId"].ToString());
            }           
            if(reader["GoodsInfoValue"]!=DBNull.Value)
            {
                _goodsInfoValue = reader["GoodsInfoValue"].ToString();
            }           
            if(reader["Type"]!=DBNull.Value)
            {
                _type = int.Parse(reader["Type"].ToString());
            }           
            if(reader["State"]!=DBNull.Value)
            {
                _state = int.Parse(reader["State"].ToString());
            }           
            if(reader["CreateTime"]!=DBNull.Value)
            {
                _createTime = DateTime.Parse(reader["CreateTime"].ToString());
            }           
        }
        
        #region 数据库字段
        private Guid _id;   
        private Guid _goodsId;   
        private string _goodsInfoValue;   
        private int _type;   
        private int _state;   
        private DateTime _createTime;   
        #endregion  
        
        #region 字段属性
        /// <summary>
        /// 主键
        /// </summary>
        public Guid  Id 
        {
            set{_id=value;}
            get{return _id;}
        }
        /// <summary>
        /// 商品Id
        /// </summary>
        public Guid  GoodsId 
        {
            set{_goodsId=value;}
            get{return _goodsId;}
        }
        /// <summary>
        /// 商品信息的值
        /// </summary>
        public string  GoodsInfoValue 
        {
            set{_goodsInfoValue=value;}
            get{return _goodsInfoValue;}
        }
        /// <summary>
        /// 0:GoodsName(商品名称);1:GoodsCode(商品编码);2:BrandId(品牌id);3:ClassId(直属分类id)
        /// </summary>
        public int  Type 
        {
            set{_type=value;}
            get{return _type;}
        }
        /// <summary>
        /// 0:未处理;1:已处理;
        /// </summary>
        public int  State 
        {
            set{_state=value;}
            get{return _state;}
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime  CreateTime 
        {
            set{_createTime=value;}
            get{return _createTime;}
        }
        #endregion  
    }
}
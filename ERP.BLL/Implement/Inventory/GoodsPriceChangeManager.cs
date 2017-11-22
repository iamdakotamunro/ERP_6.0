//Author: 张安龙
//createtime:2015/7/17 14:12:44
//Description: 
using System;
using System.Collections.Generic;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;

namespace ERP.BLL.Implement.Inventory
{

    /// <summary>
    /// 该类提供了一系列操作 "GoodsPriceChange" 表的方法;
    /// </summary>
    public class GoodsPriceChangeManager : BllInstance<GoodsPriceChangeManager>
    {
        readonly IGoodsPriceChange _goodsPriceChange;

        public GoodsPriceChangeManager(GlobalConfig.DB.FromType fromType)
        {
            _goodsPriceChange = new GoodsPriceChangeDal(fromType);
        }

        public GoodsPriceChangeManager(IGoodsPriceChange goodsPriceChange)
        {
            _goodsPriceChange = goodsPriceChange;
        }

        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回GoodsPriceChange表的所有数据  
        /// </summary>
        /// <returns></returns>        
        public List<GoodsPriceChange> GetAllGoodsPriceChange()
        {
            return _goodsPriceChange.GetAllGoodsPriceChange();
        }

        /// <summary>
        /// 根据条件返回GoodsPriceChange表的所有数据 
        /// </summary>
        /// <param name="saleFilialeId">公司ID</param>
        /// <param name="salePlatformId">销售平台ID</param>
        /// <param name="goodsName">商品名称</param>
        /// <param name="goodsId">商品编号</param>
        /// <param name="type">0:销售价；1:加盟价；2:批发价</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total">数据总数</param>
        /// <returns></returns>

        public List<GoodsPriceChange> GetAllGoodsPriceChange(Guid? saleFilialeId, Guid? salePlatformId, string goodsName, Guid goodsId, int type, int pageIndex, int pageSize, out int total)
        {
            return _goodsPriceChange.GetAllGoodsPriceChange(saleFilialeId, salePlatformId,
             goodsName, goodsId, type, pageIndex, pageSize, out total);
        }

        /// <summary>
        /// 根据GoodsPriceChange表的Id字段返回数据  
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>        
        public GoodsPriceChange GetGoodsPriceChangeById(Guid id)
        {
            return _goodsPriceChange.GetGoodsPriceChangeById(id);
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据GoodsPriceChange表的Id字段删除数据 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>返回受影响的行数</returns>
        public bool DeleteGoodsPriceChangeById(Guid id)
        {
            return _goodsPriceChange.DeleteGoodsPriceChangeById(id);
        }
        #endregion
        #region update data
        /// <summary>
        /// 根据GoodsPriceChange表的Id字段更新数据 
        /// </summary>
        /// <param name="goodsPriceChange">goodsPriceChange</param>
        /// <returns>返回受影响的行数</returns>
        public bool UpdateGoodsPriceChangeById(GoodsPriceChange goodsPriceChange)
        {
            return _goodsPriceChange.UpdateGoodsPriceChangeById(goodsPriceChange);
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向GoodsPriceChange表插入一条数据，插入成功则返回自增列数值，插入不成功则返回-1 
        /// </summary>
        /// <param name="goodsPriceChange">GoodsPriceChange</param>        
        /// <returns></returns>
        public bool AddGoodsPriceChange(GoodsPriceChange goodsPriceChange)
        {
            return _goodsPriceChange.AddGoodsPriceChange(goodsPriceChange);
        }

        /// <summary>
        /// 向GoodsPriceChange表中批量插入数据
        /// </summary>
        /// <param name="goodsPriceChangeList"></param>
        /// <returns></returns>
        public bool AddBulkGoodsPriceChange(List<GoodsPriceChange> goodsPriceChangeList)
        {
            return _goodsPriceChange.AddBulkGoodsPriceChange(goodsPriceChangeList);
        }
        #endregion
        #endregion

    }
}
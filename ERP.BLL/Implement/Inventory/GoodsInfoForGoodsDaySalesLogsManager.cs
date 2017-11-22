//Author: zal
//createtime:2016/1/29 14:20:25
//Description: 

using System;
using System.Collections.Generic;
using ERP.Model;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Implement.Inventory;

namespace ERP.BLL.Implement.Inventory
{

    /// <summary>
    /// 该类提供了一系列操作 "GoodsInfoForGoodsDaySalesLogs" 表的方法;
    /// </summary>
    public class GoodsInfoForGoodsDaySalesLogsManager
    {
        readonly IGoodsInfoForGoodsDaySalesLogs _goodsInfoForGoodsDaySalesLogsDal;

        public GoodsInfoForGoodsDaySalesLogsManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _goodsInfoForGoodsDaySalesLogsDal = new GoodsInfoForGoodsDaySalesLogsDal(fromType);
        }

        public GoodsInfoForGoodsDaySalesLogsManager(IGoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs)
        {
            _goodsInfoForGoodsDaySalesLogsDal = goodsInfoForGoodsDaySalesLogs;
        }

        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回GoodsInfoForGoodsDaySalesLogs表的所有数据  
        /// </summary>
        /// <returns></returns>        
        public List<GoodsInfoForGoodsDaySalesLogs> GetAllGoodsInfoForGoodsDaySalesLogs()
        {
            return _goodsInfoForGoodsDaySalesLogsDal.GetAllGoodsInfoForGoodsDaySalesLogs();
        }
        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的Id字段返回数据  
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>        
        public GoodsInfoForGoodsDaySalesLogs GetGoodsInfoForGoodsDaySalesLogsById(Guid id)
        {
            return _goodsInfoForGoodsDaySalesLogsDal.GetGoodsInfoForGoodsDaySalesLogsById(id);
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的Id字段删除数据 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>返回受影响的行数</returns>
        public bool DeleteGoodsInfoForGoodsDaySalesLogsById(Guid id)
        {
            return _goodsInfoForGoodsDaySalesLogsDal.DeleteGoodsInfoForGoodsDaySalesLogsById(id);
        }
        #endregion
        #region update data
        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的Id字段更新数据 
        /// </summary>
        /// <param name="goodsInfoForGoodsDaySalesLogs">goodsInfoForGoodsDaySalesLogs</param>
        /// <returns>返回受影响的行数</returns>
        public bool UpdateGoodsInfoForGoodsDaySalesLogsById(GoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs)
        {
            return _goodsInfoForGoodsDaySalesLogsDal.UpdateGoodsInfoForGoodsDaySalesLogsById(goodsInfoForGoodsDaySalesLogs);
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向GoodsInfoForGoodsDaySalesLogs表插入一条数据，插入成功则返回自增列数值，插入不成功则返回-1 
        /// </summary>
        /// <param name="goodsInfoForGoodsDaySalesLogs">GoodsInfoForGoodsDaySalesLogs</param>        
        /// <returns></returns>
        public bool AddGoodsInfoForGoodsDaySalesLogs(GoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs)
        {
            return _goodsInfoForGoodsDaySalesLogsDal.AddGoodsInfoForGoodsDaySalesLogs(goodsInfoForGoodsDaySalesLogs);
        }
        #endregion
        #endregion
    }
}
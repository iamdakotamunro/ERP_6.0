using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IGoodsPriceChange
    {
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回GoodsPriceChange表的所有数据 
        /// </summary>
        /// <returns></returns>        
        List<GoodsPriceChange> GetAllGoodsPriceChange();

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
        List<GoodsPriceChange> GetAllGoodsPriceChange(Guid? saleFilialeId, Guid? salePlatformId,
            string goodsName, Guid goodsId, int type, int pageIndex, int pageSize, out int total);

        /// <summary>
        /// 根据GoodsPriceChange表的Id字段返回数据 
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>       
        GoodsPriceChange GetGoodsPriceChangeById(Guid id);
        #endregion
        #region delete data
        /// <summary>
        /// 根据GoodsPriceChange表的Id字段删除数据 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        bool DeleteGoodsPriceChangeById(Guid id);
        #endregion
        #region update data
        /// <summary>
        /// 根据GoodsPriceChange表的Id字段更新数据 
        /// </summary> 
        /// <param name="goodsPriceChange">goodsPriceChange</param>
        /// <returns></returns>       
        bool UpdateGoodsPriceChangeById(GoodsPriceChange goodsPriceChange);
        #endregion
        #region insert data
        /// <summary>
        /// 向GoodsPriceChange表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="goodsPriceChange">GoodsPriceChange</param>       
        /// <returns></returns>        
        bool AddGoodsPriceChange(GoodsPriceChange goodsPriceChange);

        /// <summary>
        /// 向GoodsPriceChange表中批量插入数据
        /// </summary>
        /// <param name="goodsPriceChangeList"></param>
        /// <returns></returns>
        bool AddBulkGoodsPriceChange(List<GoodsPriceChange> goodsPriceChangeList);
        #endregion

        #endregion
    }
}

using ERP.Model;
using System.Collections.Generic;

namespace ERP.DAL.Interface.IInventory
{
    public interface IGoodsInfoForGoodsDaySalesLogs
    {
        #region .对本表的维护.
        #region select data

        /// <summary>
        /// 返回GoodsInfoForGoodsDaySalesLogs表的所有数据 
        /// </summary>
        /// <returns></returns>        
        List<GoodsInfoForGoodsDaySalesLogs> GetAllGoodsInfoForGoodsDaySalesLogs();
        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的Id字段返回数据 
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>       
        GoodsInfoForGoodsDaySalesLogs GetGoodsInfoForGoodsDaySalesLogsById(System.Guid id);

        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的goodsId、type、state字段返回数据
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="type">0:GoodsName(商品名称);1:GoodsCode(商品编码);2:BrandId(品牌id);3:ClassId(直属分类id)</param>
        /// <param name="state">0:未处理;1:已处理;</param>
        /// <returns></returns>
        GoodsInfoForGoodsDaySalesLogs GetGoodsInfoForGoodsDaySalesLogsByGoodsIdAndTypeAndState(System.Guid goodsId, int type, int state);
        #endregion
        #region delete data

        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的Id字段删除数据 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        bool DeleteGoodsInfoForGoodsDaySalesLogsById(System.Guid id);
        #endregion
        #region update data
        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的Id字段更新数据 
        /// </summary> 
        /// <param name="goodsInfoForGoodsDaySalesLogs">GoodsInfoForGoodsDaySalesLogs</param>
        /// <returns></returns>       
        bool UpdateGoodsInfoForGoodsDaySalesLogsById(GoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs);
        #endregion
        #region insert data

        /// <summary>
        /// 向GoodsInfoForGoodsDaySalesLogs表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="goodsInfoForGoodsDaySalesLogs">GoodsInfoForGoodsDaySalesLogs</param>       
        /// <returns></returns>        
        bool AddGoodsInfoForGoodsDaySalesLogs(GoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs);

        #endregion

        #endregion
    }
}

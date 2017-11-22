//Author: zal
//createtime:2016/1/29 14:20:13
//Description:

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Keede.DAL.Helper;
using ERP.Model;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 该类提供了一系列操作 "GoodsInfoForGoodsDaySalesLogs" 表的方法;
    /// </summary>
    public class GoodsInfoForGoodsDaySalesLogsDal : IGoodsInfoForGoodsDaySalesLogs
    {
        public GoodsInfoForGoodsDaySalesLogsDal(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        const string SQL_SELECT = "select [Id],[GoodsId],[GoodsInfoValue],[Type],[State],[CreateTime] from [GoodsInfoForGoodsDaySalesLogs] ";

        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回GoodsInfoForGoodsDaySalesLogs表的所有数据 
        /// </summary>
        /// <returns></returns>        
        public List<GoodsInfoForGoodsDaySalesLogs> GetAllGoodsInfoForGoodsDaySalesLogs()
        {
            List<GoodsInfoForGoodsDaySalesLogs> goodsInfoForGoodsDaySalesLogsList = new List<GoodsInfoForGoodsDaySalesLogs>();
            string sql = SQL_SELECT;
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null);
            while (reader.Read())
            {
                GoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs = new GoodsInfoForGoodsDaySalesLogs(reader);
                goodsInfoForGoodsDaySalesLogsList.Add(goodsInfoForGoodsDaySalesLogs);
            }
            reader.Close();
            return goodsInfoForGoodsDaySalesLogsList;
        }
        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的Id字段返回数据 
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>       
        public GoodsInfoForGoodsDaySalesLogs GetGoodsInfoForGoodsDaySalesLogsById(System.Guid id)
        {
            GoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs = null;
            string sql = SQL_SELECT + "where [Id] = @Id";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@Id",id)
            };
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            if (reader.Read())
            {
                goodsInfoForGoodsDaySalesLogs = new GoodsInfoForGoodsDaySalesLogs(reader);
            }
            reader.Close();
            return goodsInfoForGoodsDaySalesLogs;
        }

        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的goodsId、type、state字段返回数据
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="type">0:GoodsName(商品名称);1:GoodsCode(商品编码);2:BrandId(品牌id);3:ClassId(直属分类id)</param>
        /// <param name="state">0:未处理;1:已处理;</param>
        /// <returns></returns>
        public GoodsInfoForGoodsDaySalesLogs GetGoodsInfoForGoodsDaySalesLogsByGoodsIdAndTypeAndState(Guid goodsId,int type,int state)
        {
            GoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs = null;
            string sql = SQL_SELECT + "where [GoodsId] = @GoodsId and [Type] = @Type and [State] = @State";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@GoodsId",goodsId),
            new SqlParameter("@Type",type),
            new SqlParameter("@State",state)
            };
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            if (reader.Read())
            {
                goodsInfoForGoodsDaySalesLogs = new GoodsInfoForGoodsDaySalesLogs(reader);
            }
            reader.Close();
            return goodsInfoForGoodsDaySalesLogs;
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的Id字段删除数据 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        public bool DeleteGoodsInfoForGoodsDaySalesLogsById(System.Guid id)
        {
            string sql = "delete from [GoodsInfoForGoodsDaySalesLogs] where [Id] = @Id";
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@Id",id)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region update data
        /// <summary>
        /// prepare parameters 
        /// </summary>
        public static SqlParameter[] PrepareCommandParameters(GoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs)
        {
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@Id",goodsInfoForGoodsDaySalesLogs.Id),
            new SqlParameter("@GoodsId",goodsInfoForGoodsDaySalesLogs.GoodsId),
            new SqlParameter("@GoodsInfoValue",goodsInfoForGoodsDaySalesLogs.GoodsInfoValue),
            new SqlParameter("@Type",goodsInfoForGoodsDaySalesLogs.Type),
            new SqlParameter("@State",goodsInfoForGoodsDaySalesLogs.State),
            new SqlParameter("@CreateTime",goodsInfoForGoodsDaySalesLogs.CreateTime)    
            };
            return paras;
        }
        /// <summary>
        /// 根据GoodsInfoForGoodsDaySalesLogs表的Id字段更新数据 
        /// </summary> 
        /// <param name="goodsInfoForGoodsDaySalesLogs">GoodsInfoForGoodsDaySalesLogs</param>
        /// <returns></returns>       
        public bool UpdateGoodsInfoForGoodsDaySalesLogsById(GoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs)
        {
            string sql = "update [GoodsInfoForGoodsDaySalesLogs] set [GoodsId] = @GoodsId,[GoodsInfoValue] = @GoodsInfoValue,[Type] = @Type,[State] = @State,[CreateTime] = @CreateTime where [Id] = @Id";
            SqlParameter[] paras = PrepareCommandParameters(goodsInfoForGoodsDaySalesLogs);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向GoodsInfoForGoodsDaySalesLogs表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="goodsInfoForGoodsDaySalesLogs">GoodsInfoForGoodsDaySalesLogs</param>       
        /// <returns></returns>        
        public bool AddGoodsInfoForGoodsDaySalesLogs(GoodsInfoForGoodsDaySalesLogs goodsInfoForGoodsDaySalesLogs)
        {
            string sql = "insert into [GoodsInfoForGoodsDaySalesLogs]([Id],[GoodsId],[GoodsInfoValue],[Type],[State],[CreateTime])values(@Id,@GoodsId,@GoodsInfoValue,@Type,@State,@CreateTime)";
            SqlParameter[] paras = PrepareCommandParameters(goodsInfoForGoodsDaySalesLogs);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #endregion
    }
}

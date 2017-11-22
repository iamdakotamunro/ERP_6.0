//Author: 张安龙
//createtime:2015/7/17 14:11:34
//Description:

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;
using System.Data;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 该类提供了一系列操作 "GoodsPriceChange" 表的方法;
    /// </summary>
    public class GoodsPriceChangeDal : IGoodsPriceChange
    {
        const string SQL_SELECT = "select [Id],[Name],[Datetime],[GoodsId],[GoodsName],[GoodsCode],[SaleFilialeId],[SaleFilialeName],[SalePlatformId],[SalePlatformName],[OldPrice],[NewPrice],[Quota],[Type] from [GoodsPriceChange] ";

        public GoodsPriceChangeDal(GlobalConfig.DB.FromType fromType)
        {

        }
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回GoodsPriceChange表的所有数据 
        /// </summary>
        /// <returns></returns>        
        public List<GoodsPriceChange> GetAllGoodsPriceChange()
        {
            List<GoodsPriceChange> goodsPriceChangeList = new List<GoodsPriceChange>();
            
            string sql = SQL_SELECT;
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null);
            while (reader.Read())
            {
                GoodsPriceChange goodsPriceChange = new GoodsPriceChange(reader);
                goodsPriceChangeList.Add(goodsPriceChange);
            }
            reader.Close();
            return goodsPriceChangeList;
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
            var sqlsb = new StringBuilder(SQL_SELECT + " where 1=1");
            if (!string.IsNullOrEmpty(saleFilialeId.ToString()) && saleFilialeId != Guid.Empty)
            {
                sqlsb.AppendFormat(@" AND [SaleFilialeId]='{0}'", saleFilialeId);
            }
            if (!string.IsNullOrEmpty(salePlatformId.ToString()) && salePlatformId != Guid.Empty)
            {
                sqlsb.AppendFormat(@" AND [SalePlatformId]='{0}'", salePlatformId);
            }
            if (!string.IsNullOrEmpty(goodsName))
            {
                sqlsb.AppendFormat(@" AND [GoodsName]='{0}'", goodsName);
            }
            if (goodsId != Guid.Empty)
            {
                sqlsb.AppendFormat(@" AND [GoodsId]='{0}'", goodsId);
            }
            if (type != -1)
            {
                sqlsb.AppendFormat(@" AND [Type]='{0}'", type);
            }
            using (var db = DatabaseFactory.Create())
            {
                var list = new Keede.DAL.Helper.Sql.PageQuery(pageIndex, pageSize, sqlsb.ToString(), " Datetime DESC");
                var pageItem = db.SelectByPage<GoodsPriceChange>(true, list);
                total = (int)pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }
        /// <summary>
        /// 根据GoodsPriceChange表的Id字段返回数据 
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>       
        public GoodsPriceChange GetGoodsPriceChangeById(Guid id)
        {
            GoodsPriceChange goodsPriceChange = null;
            IDataReader reader = null;

            const string SQL = SQL_SELECT + "where [Id] = @Id";
            SqlParameter[] paras = {
            new SqlParameter("@Id",id)
            };
            reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, paras);
            if (reader.Read())
            {
                goodsPriceChange = new GoodsPriceChange(reader);
            }
            reader.Close();
            return goodsPriceChange;
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据GoodsPriceChange表的Id字段删除数据 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        public bool DeleteGoodsPriceChangeById(System.Guid id)
        {
            string sql = "delete from [GoodsPriceChange] where [Id] = @Id";
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
        public SqlParameter[] PrepareCommandParameters(GoodsPriceChange goodsPriceChange)
        {
            SqlParameter[] paras = new SqlParameter[]{
            new SqlParameter("@Id",goodsPriceChange.Id),
            new SqlParameter("@Name",goodsPriceChange.Name),
            new SqlParameter("@Datetime",goodsPriceChange.Datetime),
            new SqlParameter("@GoodsId",goodsPriceChange.GoodsId),
            new SqlParameter("@GoodsName",goodsPriceChange.GoodsName),
            new SqlParameter("@GoodsCode",goodsPriceChange.GoodsCode),
            new SqlParameter("@SaleFilialeId",goodsPriceChange.SaleFilialeId),
            new SqlParameter("@SaleFilialeName",goodsPriceChange.SaleFilialeName),
            new SqlParameter("@SalePlatformId",goodsPriceChange.SalePlatformId),
            new SqlParameter("@SalePlatformName",goodsPriceChange.SalePlatformName),
            new SqlParameter("@OldPrice",goodsPriceChange.OldPrice),
            new SqlParameter("@NewPrice",goodsPriceChange.NewPrice),
            new SqlParameter("@Quota",goodsPriceChange.Quota),
            new SqlParameter("@Type",goodsPriceChange.Type)
            };
            return paras;
        }
        /// <summary>
        /// 根据GoodsPriceChange表的Id字段更新数据 
        /// </summary> 
        /// <param name="goodsPriceChange">goodsPriceChange</param>
        /// <returns></returns>       
        public bool UpdateGoodsPriceChangeById(GoodsPriceChange goodsPriceChange)
        {
            string sql = "update [GoodsPriceChange] set [Name] = @Name,[Datetime] = @Datetime,[GoodsId]=@GoodsId,[GoodsName] = @GoodsName,[GoodsCode] = @GoodsCode,[SaleFilialeId] = @SaleFilialeId,[SaleFilialeName] = @SaleFilialeName,[SalePlatformId] = @SalePlatformId,[SalePlatformName] = @SalePlatformName,[OldPrice] = @OldPrice,[NewPrice] = @NewPrice,[Quota] = @Quota,[Type] = @Type where [Id] = @Id";
            SqlParameter[] paras = PrepareCommandParameters(goodsPriceChange);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向GoodsPriceChange表插入一条数据，返回自增列数值，插入不成功则返回-1
        /// </summary>
        /// <param name="goodsPriceChange">GoodsPriceChange</param>       
        /// <returns></returns>
        public bool AddGoodsPriceChange(GoodsPriceChange goodsPriceChange)
        {
            string sql = "insert into [GoodsPriceChange]([Id],[Name],[Datetime],[GoodsId],[GoodsName],[GoodsCode],[SaleFilialeId],[SaleFilialeName],[SalePlatformId],[SalePlatformName],[OldPrice],[NewPrice],[Quota],[Type])values(@Id,@Name,@Datetime,@GoodsId,@GoodsName,@GoodsCode,@SaleFilialeId,@SaleFilialeName,@SalePlatformId,@SalePlatformName,@OldPrice,@NewPrice,@Quota,@Type)";
            SqlParameter[] paras = PrepareCommandParameters(goodsPriceChange);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 向GoodsPriceChange表中批量插入数据
        /// </summary>
        /// <param name="goodsPriceChangeList"></param>
        /// <returns></returns>
        public bool AddBulkGoodsPriceChange(List<GoodsPriceChange> goodsPriceChangeList)
        {
            Dictionary<string, string> dics = new Dictionary<string, string>();
            dics.Add("Id", "Id");
            dics.Add("Name", "Name");
            dics.Add("Datetime", "Datetime");
            dics.Add("GoodsId", "GoodsId");
            dics.Add("GoodsName", "GoodsName");
            dics.Add("GoodsCode", "GoodsCode");
            dics.Add("SaleFilialeId", "SaleFilialeId");
            dics.Add("SaleFilialeName", "SaleFilialeName");
            dics.Add("SalePlatformId", "SalePlatformId");
            dics.Add("SalePlatformName", "SalePlatformName");
            dics.Add("OldPrice", "OldPrice");
            dics.Add("NewPrice", "NewPrice");
            dics.Add("Quota", "Quota");
            dics.Add("Type", "Type");
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, goodsPriceChangeList, "GoodsPriceChange", dics) > 0;
        }
        #endregion
        #endregion

    }
}

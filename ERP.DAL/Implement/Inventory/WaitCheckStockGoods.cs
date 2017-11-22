using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// add by dinghq 2011-06-29
    /// </summary>
    public class WaitCheckStockGoods : IWaitCheckStockGoods
    {
        public WaitCheckStockGoods(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        #region IWaitCheckStockGoods 成员

        public int InsertWaitCheckGoodsInfo(Keede.Ecsoft.Model.WaitCheckGoodsInfo info)
        {
            const string SQL = "INSERT lmshop_WaitCheckGoods([GoodsId],[GoodsName],[State],[WarehouseId]) VALUES(@goodsId,@goodsName,@state,@warehouseId)";
            var sqlparams = new[]{
               new SqlParameter("@goodsId",info.GoodsId),
               new SqlParameter("@goodsName",info.GoodsName),
               new SqlParameter("@state",info.State),
               new SqlParameter("@warehouseId",info.WarehouseId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlparams);

        }

        public IList<Keede.Ecsoft.Model.WaitCheckGoodsInfo> GetWaitCheckGoodsInfo(Guid warehouseId, WaitCheckGoodsState state)
        {
            const string SQL = "SELECT [GoodsId],[GoodsName],[State],[WarehouseId] FROM lmshop_WaitCheckGoods WHERE [WarehouseId]=@warehouseId AND [State]=@state";

            var sqlparams = new[]{
                 new SqlParameter("@state",(int)state),
                 new SqlParameter("@warehouseId",warehouseId)
             };
            IList<Keede.Ecsoft.Model.WaitCheckGoodsInfo> list = new List<Keede.Ecsoft.Model.WaitCheckGoodsInfo>();

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, sqlparams))
            {
                while (dr.Read())
                {
                    var entity = new Keede.Ecsoft.Model.WaitCheckGoodsInfo
                                     {
                                         GoodsId = dr.GetGuid(0),
                                         GoodsName = dr.GetString(1),
                                         State = dr.GetInt32(2),
                                         WarehouseId = dr.GetGuid(3)
                                     };
                    list.Add(entity);
                }
            }

            return list;
        }

        public IList<Keede.Ecsoft.Model.WaitCheckGoodsInfo> GetWaitCheckGoodsInfo(Guid warehouseId, WaitCheckGoodsState state, string goodsName)
        {
            const string SQL = "SELECT [GoodsId],[GoodsName],[State],[WarehouseId] FROM lmshop_WaitCheckGoods WHERE [WarehouseId]=@warehouseId AND [State]=@state AND CHARINDEX(@goodsname,GoodsName)>0";

            var sqlparams = new[]{
                 new SqlParameter("@state",(int)state),
                 new SqlParameter("@warehouseId",warehouseId),
                 new SqlParameter("@goodsname",goodsName)
             };
            IList<Keede.Ecsoft.Model.WaitCheckGoodsInfo> list = new List<Keede.Ecsoft.Model.WaitCheckGoodsInfo>();

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, sqlparams))
            {
                while (dr.Read())
                {
                    var entity = new Keede.Ecsoft.Model.WaitCheckGoodsInfo
                                     {
                                         GoodsId = dr.GetGuid(0),
                                         GoodsName = dr.GetString(1),
                                         State = dr.GetInt32(2),
                                         WarehouseId = dr.GetGuid(3)
                                     };
                    list.Add(entity);
                }
            }

            return list;
        }

        public int UpdateWaitCheckGoodsState(Guid goodsId, Guid warehouseId, WaitCheckGoodsState state)
        {
            const string SQL = "UPDATE lmshop_WaitCheckGoods SET [state]=@state WHERE GoodsId=@goodsId AND WarehouseId=@warehouseId";
            var sqlparams = new[]{
               new SqlParameter("@state",(int)state),
               new SqlParameter("@goodsId",goodsId),
               new SqlParameter("@warehouseId",warehouseId)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlparams);
        }

        public Keede.Ecsoft.Model.WaitCheckGoodsInfo GetCheckGoodsInfo(Guid goodsId, Guid warehouseId)
        {
            const string SQL = "SELECT [GoodsId],[GoodsName],[State],[WarehouseId] FROM lmshop_WaitCheckGoods WHERE [WarehouseId]=@warehouseId AND [GoodsId]=@goodsId";

            var sqlparams = new[]{
                 new SqlParameter("@warehouseId",warehouseId),
                 new SqlParameter("@goodsId",goodsId)
             };

            Keede.Ecsoft.Model.WaitCheckGoodsInfo entity = null;

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, sqlparams))
            {
                if (dr.Read())
                {
                    entity = new Keede.Ecsoft.Model.WaitCheckGoodsInfo
                                 {
                                     GoodsId = dr.GetGuid(0),
                                     GoodsName = dr.GetString(1),
                                     State = dr.GetInt32(2),
                                     WarehouseId = dr.GetGuid(3)
                                 };
                }
            }
            return entity;
        }

        #endregion
    }
}

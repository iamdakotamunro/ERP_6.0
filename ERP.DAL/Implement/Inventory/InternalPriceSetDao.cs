using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class InternalPriceSetDao : IInternalPriceSetDao
    {
        public InternalPriceSetDao(GlobalConfig.DB.FromType fromType)
        {

        }

        #region [数据库读取获取StorageRecordInfo表信息]

        /// <summary> 获取StorageRecordInfo表信息
        /// </summary>
        /// <param name="dr">IDataReader</param>
        /// <returns></returns>
        private static InternalPriceSetInfo ReaderInternalPriceSetInfo(IDataReader dr)
        {
            var internalPriceSetInfo = new InternalPriceSetInfo
            {
                GoodsType = dr["GoodsType"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GoodsType"].ToString()),
                HostingFilialeId = dr["HostingFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["HostingFilialeId"].ToString()),
                ReserveProfitRatio = dr["ReserveProfitRatio"] == DBNull.Value ? "0.00" : dr["ReserveProfitRatio"].ToString()
            };
            return internalPriceSetInfo;
        }

        #endregion

        public IList<InternalPriceSetInfo> GetInternalPriceSetInfoList(int goodstype)
        {
            IList<InternalPriceSetInfo> internalPriceSetInfoList = new List<InternalPriceSetInfo>();
            var sql = @"SELECT [GoodsType]
      ,[HostingFilialeId]
      ,[ReserveProfitRatio]
  FROM [InternalPriceSet]";
            if (goodstype != 0)
            {
                sql = sql + " WHERE GoodsType=" + goodstype;
            }
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (rdr.Read())
                {
                    internalPriceSetInfoList.Add(ReaderInternalPriceSetInfo(rdr));
                }
            }
            return internalPriceSetInfoList;
        }

        public InternalPriceSetInfo GetInternalPriceSetInfoList(int goodstype, Guid hostingFilialeId)
        {
            InternalPriceSetInfo internalPriceSetInfoList = new InternalPriceSetInfo();
            var sql = @"SELECT [GoodsType] ,[HostingFilialeId] ,[ReserveProfitRatio] FROM [InternalPriceSet]";
            if (goodstype != 0)
            {
                sql = sql + " WHERE GoodsType=" + goodstype + " AND HostingFilialeId = '" + hostingFilialeId + "'";
            }
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (rdr.Read())
                {
                    internalPriceSetInfoList.GoodsType = rdr["GoodsType"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(rdr["GoodsType"].ToString());
                    internalPriceSetInfoList.HostingFilialeId = rdr["HostingFilialeId"] == DBNull.Value
                        ? Guid.Empty
                        : new Guid(rdr["HostingFilialeId"].ToString());
                    internalPriceSetInfoList.ReserveProfitRatio = rdr["ReserveProfitRatio"] == DBNull.Value
                        ? "0.00"
                        : rdr["ReserveProfitRatio"].ToString();
                }
            }
            return internalPriceSetInfoList;
        }

        public bool UpdateInternalPriceSetInfo(int goodsType, Guid hostingFilialeId, string reserveProfitRatio)
        {
            const string SQL = @"if exists (select top 1 1 from InternalPriceSet where GoodsType = @GoodsType AND HostingFilialeId = @HostingFilialeId)
begin
    UPDATE InternalPriceSet SET ReserveProfitRatio=@ReserveProfitRatio where GoodsType = @GoodsType AND HostingFilialeId = @HostingFilialeId
end
else
begin
    insert into InternalPriceSet([GoodsType]
           ,[HostingFilialeId]
           ,[ReserveProfitRatio])
    values(@GoodsType, @HostingFilialeId, @ReserveProfitRatio)
end";
            var parm = new[]
            {
                new Parameter("@GoodsType", goodsType),
                new Parameter("@HostingFilialeId", hostingFilialeId),
                new Parameter("@ReserveProfitRatio", reserveProfitRatio)
            };
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL, parm);
            }
        }

        public Dictionary<Int32, decimal> GetGoodsTypeInternalPriceSets(Guid hostingFilialeId,
            IEnumerable<Int32> goodsTypes)
        {
            Dictionary<Int32, decimal> internalPriceDics = new Dictionary<int, decimal>();
            if (goodsTypes == null || !goodsTypes.Any())
            {
                return internalPriceDics;
            }
            var sql = string.Format(@"SELECT [GoodsType],[ReserveProfitRatio] FROM [InternalPriceSet] WHERE HostingFilialeId='{0}' AND GoodsType IN({1})", hostingFilialeId, string.Join(",", goodsTypes));
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (rdr.Read())
                {
                    if (rdr["GoodsType"] == DBNull.Value) continue;
                    internalPriceDics.Add(Convert.ToInt32(rdr["GoodsType"]), rdr["ReserveProfitRatio"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["ReserveProfitRatio"]));
                }
            }
            return internalPriceDics;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 商品采购设置变更记录
    /// </summary>
    public class PurchaseSetLog : IPurchaseSetLog
    {
        public PurchaseSetLog(GlobalConfig.DB.FromType fromType)
        {

        }

        #region [SQL语句]
        private const string SQL_SELECT_PURCHASESETLOG = "SELECT [LogId],[GoodsId],[GoodsName],[WarehouseId],[OldValue],[ChangeValue],[NewValue],[ChangeReason],[ChangeDate],[Applicant],[Auditor],[Statue],[LogType],[HostingFilialeId] FROM [lmshop_PurchaseSetLog]";
        private const string SQL_INSERT_PURCHASESETLOG = "INSERT INTO [lmshop_PurchaseSetLog]([LogId],[GoodsId],[GoodsName],[WarehouseId],[OldValue],[ChangeValue],[NewValue],[ChangeReason],[ChangeDate],[Applicant],[Auditor],[Statue],[LogType],[HostingFilialeId]) VALUES(@LogId,@GoodsId,@GoodsName,@WarehouseId,@OldValue,@ChangeValue,@NewValue,@ChangeReason,@ChangeDate,@Applicant,@Auditor,@Statue,@LogType,@HostingFilialeId)";
        private const string SQL_UPDATE_PURCHASESETLOG = "UPDATE [lmshop_PurchaseSetLog] SET [LogId]=@LogId,[GoodsName]=@GoodsName,[WarehouseId]=@WarehouseId,[OldValue]=@OldValue,[ChangeValue]=@ChangeValue,[NewValue]=@NewValue,[ChangeReason]=@ChangeReason,[ChangeDate]=@ChangeDate,[Applicant]=@Applicant,[Auditor]=@Auditor,[Statue]=@Statue,[LogType]=@LogType,[HostingFilialeId]=@HostingFilialeId WHERE [LogId]=@LogId";
        private const string SQL_DELETE_PURCHASESETLOG = "DELETE FROM [lmshop_PurchaseSetLog] WHERE [LogId]=@LogId";
        #endregion

        #region [参数]
        private const string PARM_LOGID = "@LogId";
        private const string PARM_GOODSID = "@GoodsId";
        private const string PARM_GOODSNAME = "@GoodsName";
        private const string PARM_WAREHOUSEID = "@WarehouseId";
        private const string PARM_OLDVALUE = "@OldValue";
        private const string PARM_CHANGEVALUE = "@ChangeValue";
        private const string PARM_NEWVALUE = "@NewValue";
        private const string PARM_CHANGEREASON = "@ChangeReason";
        private const string PARM_CHANGEDATE = "@ChangeDate";
        private const string PARM_APPLICANT = "@Applicant";
        private const string PARM_AUDITOR = "@Auditor";
        private const string PARM_STATUE = "@Statue";
        private const string PARM_LOGTYPE = "@LogType";
        private const string PARM_HOSTINGFILIALEID = "@HostingFilialeId";
        #endregion


        private static SqlParameter[] GetPurchaseSetLogParameters()
        {
            var parms = new[] {
                new SqlParameter(PARM_LOGID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_GOODSID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_GOODSNAME, SqlDbType.VarChar, 100),
                new SqlParameter(PARM_WAREHOUSEID,SqlDbType.UniqueIdentifier), 
                new SqlParameter(PARM_OLDVALUE, SqlDbType.Float),
                new SqlParameter(PARM_CHANGEVALUE, SqlDbType.Float),
                new SqlParameter(PARM_NEWVALUE, SqlDbType.Float),
                new SqlParameter(PARM_CHANGEREASON, SqlDbType.VarChar,1000),
                new SqlParameter(PARM_CHANGEDATE, SqlDbType.DateTime),
                new SqlParameter(PARM_APPLICANT, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_AUDITOR, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_STATUE, SqlDbType.Int),
                new SqlParameter(PARM_LOGTYPE, SqlDbType.Int),
                new SqlParameter(PARM_HOSTINGFILIALEID,SqlDbType.UniqueIdentifier) 
            };
            return parms;
        }

        private PurchaseSetLogInfo ReaderPurchaseSetLog(IDataReader dr)
        {
            var info = new PurchaseSetLogInfo
            {
                LogId = dr["LogId"] == DBNull.Value ? Guid.Empty : new Guid(dr["LogId"].ToString()),
                GoodsId = dr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsId"].ToString()),
                GoodsName = dr["GoodsName"] == DBNull.Value ? string.Empty : dr["GoodsName"].ToString(),
                WarehouseId = dr["WarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(dr["WarehouseId"].ToString()),
                OldValue = dr["OldValue"] == DBNull.Value ? 0 : decimal.Parse(dr["OldValue"].ToString()),
                ChangeValue = dr["ChangeValue"] == DBNull.Value ? 0 : decimal.Parse(dr["ChangeValue"].ToString()),
                NewValue = dr["NewValue"] == DBNull.Value ? 0 : decimal.Parse(dr["NewValue"].ToString()),
                ChangeReason = dr["ChangeReason"] == DBNull.Value ? string.Empty : dr["ChangeReason"].ToString(),
                ChangeDate = dr["ChangeDate"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dr["ChangeDate"].ToString()),
                Applicant = dr["Applicant"] == DBNull.Value ? Guid.Empty : new Guid(dr["Applicant"].ToString()),
                Auditor = dr["Auditor"] == DBNull.Value ? Guid.Empty : new Guid(dr["Auditor"].ToString()),
                Statue = dr["Statue"] == DBNull.Value ? 0 : int.Parse(dr["Statue"].ToString()),
                LogType = dr["LogType"] == DBNull.Value ? 0 : int.Parse(dr["LogType"].ToString()),
                HostingFilialeId = dr["HostingFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["HostingFilialeId"].ToString()),
            };
            return info;
        }

        /// <summary>
        /// 获取所有商品采购设置变更信息
        /// </summary>
        /// <returns></returns>
        public IList<PurchaseSetLogInfo> GetPurchaseSetLogList()
        {
            IList<PurchaseSetLogInfo> list = new List<PurchaseSetLogInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_PURCHASESETLOG, null))
            {
                while (dr.Read())
                {
                    list.Add(ReaderPurchaseSetLog(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取所有商品采购设置变更信息
        /// </summary>
        /// <returns></returns>
        public IList<PurchaseSetLogInfo> GetPurchaseSetLogList(Guid goodsId, Guid warehouseId, Guid hostingFilialeId)
        {
            IList<PurchaseSetLogInfo> list = new List<PurchaseSetLogInfo>();
            const string SQL = SQL_SELECT_PURCHASESETLOG + " WHERE GoodsId=@GoodsId And WarehouseId=@WarehouseId AND HostingFilialeId=@HostingFilialeId";
            var parms = new[]
                           {
                               new SqlParameter(PARM_GOODSID, SqlDbType.UniqueIdentifier) { Value = goodsId },
                               new SqlParameter(PARM_WAREHOUSEID, SqlDbType.UniqueIdentifier) { Value = warehouseId },
                               new SqlParameter(PARM_HOSTINGFILIALEID, SqlDbType.UniqueIdentifier) { Value = hostingFilialeId }
                           };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (dr.Read())
                {
                    list.Add(ReaderPurchaseSetLog(dr));
                }
            }
            return list;
        }
        public IList<PurchaseSetLogInfo> GetPurchaseSetLogListByPage(Guid goodsId, Guid warehouseId,Guid hostingFilialeId, int startPage, int pageSize, out long recordCount)
        {
            var strbSql = new StringBuilder(SQL_SELECT_PURCHASESETLOG);
            strbSql.Append(" WHERE 1=1");
            if (goodsId != Guid.Empty)
            {
                strbSql.Append(" AND GoodsId='").Append(goodsId).Append("'");
            }
            if (warehouseId != Guid.Empty)
            {
                strbSql.Append(" AND WarehouseId='").Append(warehouseId).Append("'");
            }
            if (hostingFilialeId != Guid.Empty)
            {
                strbSql.AppendFormat(" AND HostingFilialeId ='{0}'",hostingFilialeId);
            }
            strbSql.Append("AND LogType=1");
            using (var db = DatabaseFactory.Create())
            {
                var pageQuery = new Keede.DAL.Helper.Sql.PageQuery(startPage, pageSize, strbSql.ToString(), " ChangeDate DESC ");
                var pageItem = db.SelectByPage<PurchaseSetLogInfo>(true, pageQuery);
                recordCount = pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }

        /// <summary>
        /// 获取商品采购设置变更信息
        /// </summary>
        /// <param name="logId">日志ID</param>
        /// <returns></returns>
        public PurchaseSetLogInfo GetPurchaseSetLogInfo(Guid logId)
        {
            const string SQL = SQL_SELECT_PURCHASESETLOG + " WHERE LogId=@LogId";
            var parm = new SqlParameter(PARM_LOGID, SqlDbType.UniqueIdentifier) { Value = logId };

            var info = new PurchaseSetLogInfo();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (dr.Read())
                {
                    info = ReaderPurchaseSetLog(dr);
                }
            }
            return info;
        }

        /// <summary>
        /// 添加商品采购设置变更信息
        /// </summary>
        /// <param name="info"></param>
        public void AddPurchaseSetLog(PurchaseSetLogInfo info)
        {
            SqlParameter[] parms = GetPurchaseSetLogParameters();
            parms[0].Value = info.LogId;
            parms[1].Value = info.GoodsId;
            parms[2].Value = info.GoodsName;
            parms[3].Value = info.WarehouseId;
            parms[4].Value = Convert.ToDouble(info.OldValue);
            parms[5].Value = Convert.ToDouble(info.ChangeValue);
            parms[6].Value = Convert.ToDouble(info.NewValue);
            parms[7].Value = info.ChangeReason;
            parms[8].Value = info.ChangeDate;
            parms[9].Value = info.Applicant;
            parms[10].Value = info.Auditor;
            parms[11].Value = info.Statue;
            parms[12].Value = info.LogType;
            parms[13].Value = info.HostingFilialeId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_PURCHASESETLOG, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("更新设置变更信息失败!", ex);
            }
        }

        /// <summary>
        /// 修改商品采购设置变更信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdatePurchaseSetLog(PurchaseSetLogInfo info)
        {
            SqlParameter[] parms = GetPurchaseSetLogParameters();
            parms[0].Value = info.LogId;
            parms[1].Value = info.GoodsId;
            parms[2].Value = info.GoodsName;
            parms[3].Value = info.WarehouseId;
            parms[4].Value = Convert.ToDouble(info.OldValue);
            parms[5].Value = Convert.ToDouble(info.ChangeValue);
            parms[6].Value = Convert.ToDouble(info.NewValue);
            parms[7].Value = info.ChangeReason;
            parms[8].Value = info.ChangeDate;
            parms[9].Value = info.Applicant;
            parms[10].Value = info.Auditor;
            parms[11].Value = info.Statue;
            parms[12].Value = info.LogType;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_PURCHASESETLOG, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 根据商品ID删除商品采购设置变更信息
        /// </summary>
        /// <param name="logId"></param>
        public void DeletePurchaseSetLog(Guid logId)
        {
            var parm = new Parameter(PARM_LOGID, logId);
            using (var db = DatabaseFactory.Create())
            {
                db.Execute(false, SQL_DELETE_PURCHASESETLOG, parm);
            }
        }


        /// <summary>
        /// 用于判断是否存在未审核的调价记录
        /// </summary>
        /// <param name="goodsIds">主商品Id列表</param>
        /// <param name="status">审核状态</param>
        /// <param name="valueType">0,调高，1,调低，-1、所有</param>
        /// <param name="logType">商品采购设置变更值类型：1、采购价，2、报备天数</param>
        /// <returns></returns>
        public bool IsExistNoAuditSetLog(List<Guid> goodsIds, int status, int valueType, int logType)
        {
            var builder = new StringBuilder(@"SELECT 1 FROM lmshop_PurchaseSetLog WHERE GoodsId IN( ");
            for (int i = 0; i < goodsIds.Count; i++)
            {
                builder.AppendFormat(i == goodsIds.Count - 1 ? "'{0}'" : "'{0}',", goodsIds[i]);
            }
            builder.Append(") ");
            if (status != -1)
            {
                builder.Append(" AND Statue=" + status);
            }
            if (valueType != -1)
            {
                builder.Append(valueType == 0 ? " AND ChangeValue>0 " : " AND ChangeValue<0 ");
            }
            if (logType != 0)
            {
                builder.Append(" AND LogType=" + logType);
            }
            builder.Append(" ORDER BY ChangeDate DESC ");
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(true, builder.ToString(), null) > 0;
            }
        }
    }
}

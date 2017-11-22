using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model.WMS;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 商品采购设置
    /// </summary>
    public class PurchaseSet : IPurchaseSet
    {
        public PurchaseSet(GlobalConfig.DB.FromType fromType)
        {

        }

        #region [SQL语句]
        private const string SQL_SELECT_PURCHASESET = @"
        SELECT [lmShop_CompanyCussent].[CompanyName] AS CompanyName,[GoodsId],[GoodsName],[WarehouseId],[lmshop_PurchaseSet].[CompanyId],[PurchasePrice],[CostPrice],[PersonResponsible],[Memo],[PromotionId],[LastPurchasingDate],[IsStockUp],[PurchaseGroupId],[FilingForm],[StockUpDay],[FirstWeek],[SecondWeek],[ThirdWeek],[FourthWeek],[FilingTrigger],[Insufficient],[IsDelete],[HostingFilialeId] 
        FROM [lmshop_PurchaseSet] With(NOLOCK)
        INNER JOIN [lmShop_CompanyCussent] With(NOLOCK) ON [lmshop_PurchaseSet].CompanyId=[lmShop_CompanyCussent].CompanyId ";

        //        private const string SQL_DELETE_PURCHASESET_WAREHOUSEID = @"
        //DELETE FROM [lmshop_PurchasePromotion]
        //WHERE [PromotionId] IN (
        //	SELECT [PromotionId] FROM [lmshop_PurchaseSet] WHERE [GoodsId]=@GoodsId And [WarehouseId]=@WarehouseId
        //);
        //DELETE FROM [lmshop_PurchaseSet] WHERE [GoodsId]=@GoodsId And [WarehouseId]=@WarehouseId";
        #endregion

        #region [参数]
        private const string PARM_GOODSID = "@GoodsId";
        private const string PARM_GOODSNAME = "@GoodsName";
        private const string PARM_WAREHOUSEID = "@WarehouseId";
        private const string PARM_COMPANYID = "@CompanyId";
        private const string PARM_PURCHASEPRICE = "@PurchasePrice";
        private const string PARM_PERSONRESPONSIBLE = "@PersonResponsible";
        private const string PARM_MEMO = "@Memo";
        private const string PARM_PROMOTIONID = "@PromotionId";
        private const string PARM_ISSTOCKUP = "@IsStockUp";

        private const string PARM_PURCHASEGROUPID = "@PurchaseGroupId";
        private const string PARM_FILING_FORM = "@FilingForm";
        private const string PARM_STOCKUPDAY = "@StockUpDay";
        private const string PARM_FIRSTWEEK = "@FirstWeek";
        private const string PARM_SECONDWEEK = "@SecondWeek";
        private const string PARM_THIRDWEEK = "@ThirdWeek";
        private const string PARM_FOURTHWEEK = "@FourthWeek";
        private const string PARM_FILINGTRIGGER = "@FilingTrigger";
        private const string PARM_INSUFFICIENT = "@Insufficient";
        private const string PARM_HOSTINGFILIALEID = "@HostingFilialeId";
        #endregion

        private static SqlParameter[] GetPurchaseSetParameters()
        {
            var parms = new[] {
                new SqlParameter(PARM_GOODSID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_GOODSNAME, SqlDbType.VarChar, 100),
                new SqlParameter(PARM_WAREHOUSEID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_PURCHASEPRICE, SqlDbType.Float),
                new SqlParameter(PARM_PERSONRESPONSIBLE, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_MEMO, SqlDbType.VarChar,5000),
                new SqlParameter(PARM_PROMOTIONID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_ISSTOCKUP, SqlDbType.Bit),
                new SqlParameter(PARM_PURCHASEGROUPID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_FILING_FORM, SqlDbType.Int),
                new SqlParameter(PARM_STOCKUPDAY, SqlDbType.Int),
                new SqlParameter(PARM_FIRSTWEEK, SqlDbType.Int),
                new SqlParameter(PARM_SECONDWEEK, SqlDbType.Int),
                new SqlParameter(PARM_THIRDWEEK, SqlDbType.Int),
                new SqlParameter(PARM_FOURTHWEEK, SqlDbType.Int),
                new SqlParameter(PARM_FILINGTRIGGER, SqlDbType.Int),
                new SqlParameter(PARM_INSUFFICIENT, SqlDbType.Int),
                new SqlParameter(PARM_HOSTINGFILIALEID,SqlDbType.UniqueIdentifier)
            };
            return parms;
        }

        private PurchaseSetInfo ReaderPurchaseSet(IDataReader dr)
        {
            var info = new PurchaseSetInfo
            {
                GoodsId = SqlRead.GetGuid(dr, "GoodsId"),
                GoodsName = SqlRead.GetString(dr, "GoodsName"),
                WarehouseId = SqlRead.GetGuid(dr, "WarehouseId"),
                CompanyId = SqlRead.GetGuid(dr, "CompanyId"),
                PurchasePrice = SqlRead.GetDecimal(dr, "PurchasePrice"),
                PersonResponsible = SqlRead.GetGuid(dr, "PersonResponsible"),
                Memo = SqlRead.GetString(dr, "Memo"),
                PromotionId = SqlRead.GetGuid(dr, "PromotionId"),
                IsStockUp = SqlRead.GetBoolean(dr, "IsStockUp"),
                PurchaseGroupId = SqlRead.GetGuid(dr, "PurchaseGroupId"),
                FilingForm = SqlRead.GetInt(dr, "FilingForm"),
                StockUpDay = SqlRead.GetInt(dr, "StockUpDay"),
                FirstWeek = SqlRead.GetInt(dr, "FirstWeek"),
                SecondWeek = SqlRead.GetInt(dr, "SecondWeek"),
                ThirdWeek = SqlRead.GetInt(dr, "ThirdWeek"),
                FourthWeek = SqlRead.GetInt(dr, "FourthWeek"),
                FilingTrigger = SqlRead.GetInt(dr, "FilingTrigger"),
                Insufficient = SqlRead.GetInt(dr, "Insufficient"),
                IsDelete = Convert.ToInt32(dr["IsDelete"]),
                CompanyName = SqlRead.GetString(dr, "CompanyName"),
                HostingFilialeId = SqlRead.GetGuid(dr, "HostingFilialeId")
            };
            return info;
        }

        /// <summary>
        /// 获取所有商品采购设置信息
        /// </summary>
        /// <returns></returns>
        public IList<PurchaseSetInfo> GetPurchaseSetList()
        {
            IList<PurchaseSetInfo> list = new List<PurchaseSetInfo>();
            const string SQL = SQL_SELECT_PURCHASESET + " WHERE IsDelete=1 ORDER BY GoodsName ";
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, null))
            {
                while (dr.Read())
                {
                    list.Add(ReaderPurchaseSet(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取所有商品采购设置信息
        /// </summary>
        /// <returns></returns>
        public IList<PurchaseSetInfo> GetPurchaseSetListToPage(bool load, List<Guid> goodsIdList, string goodsName, Guid companyId, Guid personResponsible, int filingForm,
            int stockUpDay, int statue, int startPage, int pageSize, out long recordCount)
        {
            var strbSql = new StringBuilder("");
            if (load)
            {
                strbSql.Append(@"SELECT 
	ps.[GoodsId],ps.[GoodsName],ps.[WarehouseId],ps.[CompanyId],ps.[PurchasePrice],[PersonResponsible],ps.[HostingFilialeId],
[Memo],ISNULL(pp.[PromotionId],'00000000-0000-0000-0000-000000000000') AS PromotionId,IsStockUp,ps.[PurchaseGroupId],[FilingForm],[StockUpDay],[FirstWeek],[SecondWeek],
[ThirdWeek],[FourthWeek],[FilingTrigger],[Insufficient],[IsDelete],cc.CompanyName,cpg.PurchaseGroupName
FROM [lmshop_PurchaseSet] ps
LEFT JOIN [lmShop_CompanyCussent] cc ON cc.CompanyId=ps.CompanyId 
LEFT JOIN [CompanyPurchaseGoup] cpg ON cpg.CompanyId=ps.CompanyId AND cpg.PurchaseGroupId=ps.PurchaseGroupId 
LEFT JOIN (select PromotionId,GoodsId,WarehouseId from  dbo.lmshop_PurchasePromotion where StartDate <= getdate() and  EndDate>= getdate()) as pp
 ON pp.GoodsId=ps.GoodsId AND ps.WarehouseId=pp.WarehouseId WHERE ps.IsDelete=1 ORDER BY pp.PromotionId DESC  ");
            }
            else
            {
                strbSql.Append(@"select 
    [GoodsId],[GoodsName],[WarehouseId],[CompanyId],[PurchasePrice],[PersonResponsible],
	[Memo],IsStockUp,[PurchaseGroupId],[FilingForm],[StockUpDay],[FirstWeek],
	[SecondWeek],[ThirdWeek],[FourthWeek],[FilingTrigger],[Insufficient],[IsDelete],[HostingFilialeId],
	CompanyName,PurchaseGroupName  
from  (
	SELECT 
		ps.[GoodsId],ps.[GoodsName],
		ps.[WarehouseId],ps.[CompanyId],ps.[PurchasePrice],[PersonResponsible],ps.[HostingFilialeId],
		[Memo],ps.[PromotionId],IsStockUp,
		ps.[PurchaseGroupId],[FilingForm],[StockUpDay],[FirstWeek],[SecondWeek],
		[ThirdWeek],[FourthWeek],[FilingTrigger],[Insufficient],[IsDelete],
		cc.CompanyName,cpg.PurchaseGroupName,
		dbo.fun_GetPurchaseSetLogByGoodsIDWarehouseID(ps.[GoodsId],ps.[WarehouseId]) as [Statue]
	FROM [lmshop_PurchaseSet] ps
	LEFT JOIN [lmShop_CompanyCussent] cc ON cc.CompanyId=ps.CompanyId 
	LEFT JOIN [CompanyPurchaseGoup] cpg ON cpg.CompanyId=ps.CompanyId 
	AND cpg.PurchaseGroupId=ps.PurchaseGroupId WHERE 1=1
	");
                if (goodsIdList.Count == 0)
                {
                    if (!String.IsNullOrWhiteSpace(goodsName))
                        strbSql.Append(" AND ps.GoodsName like '%").Append(goodsName).Append("%'");
                }
                else
                {
                    var strbGoodsIds = new StringBuilder(string.Empty);
                    foreach (var goodsId in goodsIdList)
                    {
                        if (string.IsNullOrEmpty(strbGoodsIds.ToString()))
                            strbGoodsIds.Append("'").Append(goodsId).Append("'");
                        else
                            strbGoodsIds.Append(",'").Append(goodsId).Append("'");
                    }
                    strbSql.Append(" AND (ps.GoodsId IN (").Append(strbGoodsIds).Append(")");
                    if (!String.IsNullOrWhiteSpace(goodsName))
                        strbSql.Append(" or ps.GoodsName like '%").Append(goodsName).Append("%')");
                }

                if (companyId != Guid.Empty)
                {
                    strbSql.Append(" AND ps.CompanyId='").Append(companyId).Append("'");
                }
                if (personResponsible != Guid.Empty)
                {
                    strbSql.Append(" AND PersonResponsible='").Append(personResponsible).Append("'");
                }
                if (filingForm != 0)
                {
                    strbSql.Append(" AND FilingForm=").Append(filingForm);
                }
                if (stockUpDay != 0)
                {
                    strbSql.Append(" AND StockUpDay=").Append(stockUpDay);
                }
                strbSql.AppendFormat(@" )I1 {0} ORDER BY I1.GoodsName ", statue != -1 ? string.Format(" WHERE I1.[Statue]  = {0} ", statue) : "");
            }
            using (var db = DatabaseFactory.Create())
            {
                var pageItem = db.Select<PurchaseSetInfo>(true, strbSql.ToString()).ToList();
                recordCount = pageItem.Count;
                return pageItem.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 获取所有商品采购设置信息
        /// </summary>
        /// <returns></returns>
        public IList<PurchaseSetInfo> GetPurchaseSetListByWarehouseId(Guid warehouseId)
        {
            IList<PurchaseSetInfo> list = new List<PurchaseSetInfo>();
            string sql = SQL_SELECT_PURCHASESET + " WHERE WarehouseId='" + warehouseId + "' AND IsDelete=1 ORDER BY GoodsName ";
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                while (dr.Read())
                {
                    list.Add(ReaderPurchaseSet(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取所有商品采购设置
        /// </summary>
        /// <returns></returns>
        public List<PurchaseSetInfo> GetPurchaseSetListByWarehouseIdAndCompanyId(Guid warehouseId, Guid companyId)
        {
            var list = new List<PurchaseSetInfo>();
            string sql = SQL_SELECT_PURCHASESET + "WHERE WarehouseId=@WarehouseId AND [lmshop_PurchaseSet].CompanyId=@CompanyId AND IsDelete=1 ORDER BY GoodsName ";
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, new[]{
                new SqlParameter("@WarehouseId",warehouseId),
                new SqlParameter("@CompanyId",companyId)  }))
            {
                while (dr.Read())
                {
                    list.Add(ReaderPurchaseSet(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="goodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public IList<PurchaseSetInfo> GetPurchaseSetList(List<Guid> goodsIdList, Guid warehouseId)
        {
            IList<PurchaseSetInfo> list = new List<PurchaseSetInfo>();
            if (goodsIdList.Count > 0)
            {
                var strbSql = new StringBuilder(SQL_SELECT_PURCHASESET);
                string goodsStr = string.Format(" EXISTS(select id as GoodsId from splitToTable({0},',') #temp WHERE #temp.id=lmshop_PurchaseSet.GoodsId) ", "'" + string.Join(",", goodsIdList.ToArray()) + "'");
                strbSql.Append(" WHERE ").Append(goodsStr);
                strbSql.Append(" AND IsDelete=1");
                if (warehouseId != Guid.Empty)
                    strbSql.Append(string.Format(" AND WarehouseId ='{0}'", warehouseId));
                strbSql.Append(" ORDER BY GoodsName");
                using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString(), null))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            var info = ReaderPurchaseSet(dr);
                            list.Add(info);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="goodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <param name="isDelete"></param>
        /// <returns>0:禁用；1:启用；2:全部</returns>
        /// zal 2017-03-16
        public IList<PurchaseSetInfo> GetPurchaseSetList(List<Guid> goodsIdList, Guid warehouseId, int isDelete)
        {
            IList<PurchaseSetInfo> list = new List<PurchaseSetInfo>();
            if (goodsIdList.Count == 0)
            {
                return list;
            }

            string goodsStr = string.Format("select id as GoodsId from splitToTable({0},',')", "'" + string.Join(",", goodsIdList.ToArray()) + "'");
            var strbSql = new StringBuilder();
            strbSql.Append(@"
                SELECT B.[CompanyName],A.[GoodsId],[GoodsName],[WarehouseId],A.[CompanyId],[PurchasePrice],[CostPrice],[PersonResponsible],[Memo],[PromotionId],[LastPurchasingDate],[IsStockUp],[PurchaseGroupId],[FilingForm],[StockUpDay],[FirstWeek],[SecondWeek],[ThirdWeek],[FourthWeek],[FilingTrigger],[Insufficient],[IsDelete],[HostingFilialeId] 
                FROM [lmshop_PurchaseSet] AS A With(NOLOCK)
                INNER JOIN [lmShop_CompanyCussent] AS B With(NOLOCK) ON A.CompanyId=B.CompanyId
                INNER JOIN (").Append(goodsStr).Append(")t ON t.GoodsId=A.GoodsId WHERE 1=1 ");

            if (isDelete.Equals(0))
            {
                strbSql.Append(" AND IsDelete=0 ");
            }
            else if (isDelete.Equals(1))
            {
                strbSql.Append(" AND IsDelete=1 ");
            }

            strbSql.Append(warehouseId.Equals(Guid.Empty) ? string.Empty : string.Format(" AND WarehouseId ='{0}'", warehouseId));

            strbSql.Append(" ORDER BY GoodsName");
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString(), null))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        var info = ReaderPurchaseSet(dr);
                        list.Add(info);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public PurchaseSetInfo GetPurchaseSetInfo(Guid goodsId, Guid hostingFilialeId, Guid warehouseId)
        {
            const string SQL = SQL_SELECT_PURCHASESET + " WHERE GoodsId=@GoodsId AND WarehouseId=@WarehouseId AND HostingFilialeId=@HostingFilialeId";
            var parms = new[]
                           {
                               new SqlParameter(PARM_GOODSID, SqlDbType.UniqueIdentifier) { Value = goodsId },
                               new SqlParameter(PARM_WAREHOUSEID, SqlDbType.UniqueIdentifier) { Value = warehouseId },
                               new SqlParameter(PARM_HOSTINGFILIALEID, SqlDbType.UniqueIdentifier) { Value = hostingFilialeId }
                           };

            PurchaseSetInfo info = null;
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                if (dr.Read())
                {
                    info = ReaderPurchaseSet(dr);
                }
            }
            return info;
        }

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public List<PurchaseSetInfo> GetPurchaseSetInfo(Guid goodsId, Guid warehouseId)
        {
            const string SQL = SQL_SELECT_PURCHASESET + " WHERE GoodsId=@GoodsId AND  WarehouseId=@WarehouseId";
            var parms = new[]
                           {
                               new SqlParameter(PARM_GOODSID, SqlDbType.UniqueIdentifier) { Value = goodsId },
                               new SqlParameter(PARM_WAREHOUSEID, SqlDbType.UniqueIdentifier) { Value = warehouseId }
                           };

            List<PurchaseSetInfo> info = new List<PurchaseSetInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (dr.Read())
                {
                    info.Add(ReaderPurchaseSet(dr));
                }
            }
            return info;
        }

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="hostingFilialeId"></param>
        /// <param name="goodsIds"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public List<PurchaseSetInfo> GetPurchaseSetInfoList(IEnumerable<Guid> goodsIds, Guid warehouseId, Guid hostingFilialeId)
        {
            StringBuilder builder = new StringBuilder(SQL_SELECT_PURCHASESET);
            builder.AppendFormat(
                " WHERE  WarehouseId=@WarehouseId AND HostingFilialeId=@HostingFilialeId AND GoodsId IN('{0}') ",
                string.Join("','", goodsIds));
            var parms = new[]
                           {
                               new SqlParameter(PARM_HOSTINGFILIALEID, SqlDbType.UniqueIdentifier) { Value = hostingFilialeId },
                               new SqlParameter(PARM_WAREHOUSEID, SqlDbType.UniqueIdentifier) { Value = warehouseId }
                           };

            List<PurchaseSetInfo> source = new List<PurchaseSetInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), parms))
            {
                while (dr.Read())
                {
                    source.Add(ReaderPurchaseSet(dr));
                }
            }
            return source;
        }

        public List<KeyValuePair<Guid, Guid>> GetKeyAndValueGuids(IEnumerable<Guid> goodsIds, IEnumerable<Guid> warehouseIds)
        {
            string sql = string.Format(@"select WarehouseId,HostingFilialeId,GoodsId from lmshop_PurchaseSet where IsDelete=1 and GoodsId IN('{0}') and WarehouseId IN('{1}')", string.Join("','", goodsIds), string.Join("','", warehouseIds));

            List<PurchaseSetInfo> list = new List<PurchaseSetInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (dr.Read())
                {
                    list.Add(new PurchaseSetInfo
                    {
                        WarehouseId = dr["WarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(dr["WarehouseId"].ToString()),
                        HostingFilialeId = dr["HostingFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["HostingFilialeId"].ToString()),
                        GoodsId = dr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsId"].ToString())
                    });
                }
            }
            return list.GroupBy(ent => new { ent.WarehouseId, ent.HostingFilialeId })
                       .Select(ent => new KeyValuePair<Guid, Guid>(ent.Key.WarehouseId, ent.Key.HostingFilialeId))
                       .ToList();
        }

        /// <summary>
        /// 缓存取商品采购设置中获取责任人ID和责任人名字
        /// </summary>
        /// <returns></returns>
        public Dictionary<KeyValuePair<Guid, Guid>, List<Guid>> GetKeyAndValueGuids()
        {
            string sql = "select WarehouseId,HostingFilialeId,GoodsId from lmshop_PurchaseSet where IsDelete=1 ";
            var dic = new Dictionary<KeyValuePair<Guid, Guid>, List<Guid>>();
            List<PurchaseSetInfo> list = new List<PurchaseSetInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql))
            {
                while (dr.Read())
                {
                    list.Add(new PurchaseSetInfo
                    {
                        WarehouseId = dr["WarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(dr["WarehouseId"].ToString()),
                        HostingFilialeId = dr["HostingFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["HostingFilialeId"].ToString()),
                        GoodsId = dr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsId"].ToString())
                    });
                }
            }
            foreach (var dics in list.GroupBy(ent => new { ent.WarehouseId, ent.HostingFilialeId }))
            {
                dic.Add(new KeyValuePair<Guid, Guid>(dics.Key.WarehouseId, dics.Key.HostingFilialeId), list.Where(ent => ent.WarehouseId == dics.Key.WarehouseId && ent.HostingFilialeId == dics.Key.HostingFilialeId).Select(ent => ent.GoodsId).ToList());
            }
            return dic;
        }

        /// <summary>
        /// 商品采购设置中获取责任人ID和责任人名字
        /// </summary>
        /// <returns></returns>
        public IList<PurchaseSetInfo> GetPersonList()
        {
            const string SQL = @"SELECT DISTINCT(PersonResponsible) AS PersonResponsible FROM [lmshop_PurchaseSet] WHERE IsDelete = 1";
            IList<PurchaseSetInfo> list = new List<PurchaseSetInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, null))
            {
                while (dr.Read())
                {
                    var info = new PurchaseSetInfo
                    {
                        PersonResponsible = dr["PersonResponsible"] == DBNull.Value ? Guid.Empty : new Guid(dr["PersonResponsible"].ToString())
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>
        /// 添加商品采购设置
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        public bool AddPurchaseSet(PurchaseSetInfo info, out string errorMessage)
        {
            errorMessage = string.Empty;
            const string SQL = @"
INSERT INTO [lmshop_PurchaseSet]([GoodsId],[GoodsName],[WarehouseId],[CompanyId],[PurchasePrice],[PersonResponsible],[Memo],[PromotionId],[IsStockUp],[PurchaseGroupId],[FilingForm],[StockUpDay],[FirstWeek],[SecondWeek],[ThirdWeek],[FourthWeek],[FilingTrigger],[Insufficient],[HostingFilialeId])
VALUES(@GoodsId,@GoodsName,@WarehouseId,@CompanyId,@PurchasePrice,@PersonResponsible,@Memo,@PromotionId,@IsStockUp,@PurchaseGroupId,@FilingForm,@StockUpDay,@FirstWeek,@SecondWeek,@ThirdWeek,@FourthWeek,@FilingTrigger,@Insufficient,@HostingFilialeId)";
            SqlParameter[] parms = GetPurchaseSetParameters();
            parms[0].Value = info.GoodsId;
            parms[1].Value = info.GoodsName;
            parms[2].Value = info.WarehouseId;
            parms[3].Value = info.CompanyId;
            parms[4].Value = Convert.ToDouble(info.PurchasePrice);
            parms[5].Value = info.PersonResponsible;
            parms[6].Value = info.Memo;
            parms[7].Value = info.PromotionId;
            parms[8].Value = info.IsStockUp;
            parms[9].Value = info.PurchaseGroupId;
            parms[10].Value = info.FilingForm;
            parms[11].Value = info.StockUpDay;
            parms[12].Value = info.FirstWeek;
            parms[13].Value = info.SecondWeek;
            parms[14].Value = info.ThirdWeek;
            parms[15].Value = info.FourthWeek;
            parms[16].Value = info.FilingTrigger;
            parms[17].Value = info.Insufficient;
            parms[18].Value = info.HostingFilialeId;
            try
            {
                var result = SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
                return result > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 修改商品采购设置
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        public int UpdatePurchaseSet(PurchaseSetInfo info, out string errorMessage)
        {
            errorMessage = string.Empty;
            const string SQL_UPDATE_PURCHASESET = @"
UPDATE [lmshop_PurchaseSet]
   SET 
      [GoodsName]=@GoodsName
      ,[CompanyId]=@CompanyId
      ,[PurchasePrice]=@PurchasePrice
      ,[PersonResponsible]=@PersonResponsible
      ,[Memo]=@Memo
      ,[PromotionId]=@PromotionId
      ,[IsStockUp]=@IsStockUp
      ,[PurchaseGroupId]=@PurchaseGroupId
      ,[FilingForm]=@FilingForm
      ,[StockUpDay]=@StockUpDay
      ,[FirstWeek]=@FirstWeek
      ,[SecondWeek]=@SecondWeek
      ,[ThirdWeek]=@ThirdWeek
      ,[FourthWeek]=@FourthWeek
      ,[FilingTrigger]=@FilingTrigger
      ,[Insufficient]=@Insufficient
WHERE [GoodsId]=@GoodsId AND [WarehouseId]=@WarehouseId AND [HostingFilialeId] = @HostingFilialeId";
            SqlParameter[] parms = GetPurchaseSetParameters();
            parms[0].Value = info.GoodsId;
            parms[1].Value = info.GoodsName;
            parms[2].Value = info.WarehouseId;
            parms[3].Value = info.CompanyId;
            parms[4].Value = Convert.ToDouble(info.PurchasePrice);
            parms[5].Value = info.PersonResponsible;
            parms[6].Value = info.Memo;
            parms[7].Value = info.PromotionId;
            parms[8].Value = info.IsStockUp;
            parms[9].Value = info.PurchaseGroupId;
            parms[10].Value = info.FilingForm;
            parms[11].Value = info.StockUpDay;
            parms[12].Value = info.FirstWeek;
            parms[13].Value = info.SecondWeek;
            parms[14].Value = info.ThirdWeek;
            parms[15].Value = info.FourthWeek;
            parms[16].Value = info.FilingTrigger;
            parms[17].Value = info.Insufficient;
            parms[18].Value = info.HostingFilialeId;
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_PURCHASESET, parms);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                throw new ApplicationException(ex.Message, ex);
            }
        }

        /// <summary>采购已分配管理批量转移供应商
        /// </summary>
        public void BatchTransferCompany(Guid oldCompanyId, Guid newCompanyId)
        {
            const string SQL = "UPDATE [lmshop_PurchaseSet] SET [CompanyId]=@newCompanyId,PurchaseGroupId='00000000-0000-0000-0000-000000000000'  WHERE [CompanyId]=@oldCompanyId";
            var sqlParams = new[]
                                {
                                    new SqlParameter("@oldCompanyId",oldCompanyId),
                                    new SqlParameter("@newCompanyId", newCompanyId)
                                };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlParams);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询商品是否备货，用户库存查询
        /// </summary>
        /// <returns></returns>
        public string GetGoodsIsReady(Guid goodsId)
        {
            const string SQL = @"select IsStockUp from lmshop_PurchaseSet where isstockup=1 and GoodsId=@GoodsId and IsDelete=1";
            try
            {
                IDataReader sdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, new SqlParameter("@GoodsId", goodsId));
                if (sdr.Read())
                {
                    return "备货";
                }
                return "每日进货";
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public Dictionary<Guid, String> GetGoodsIsReadyByWarehouseId(Guid warehouseId, IEnumerable<Guid> goodsIds)
        {
            const string SQL = @"select GoodsId,IsStockUp from lmshop_PurchaseSet where isstockup=1 and IsDelete=1 and WarehouseId=@WarehouseId AND GoodsId IN('{0}') group by GoodsId,IsStockUp";
            var paras = new SqlParameter("@WarehouseId", warehouseId);
            Dictionary<Guid, String> dic = new Dictionary<Guid, string>();
            try
            {
                using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format(SQL, string.Join("','", goodsIds)), paras))
                {
                    while (dr.Read() && !dic.ContainsKey(dr.GetGuid(0)))
                    {
                        dic.Add(dr.GetGuid(0), "备货");
                    }
                }
                return dic;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public int UpdatePurchaseSetToPurchaseGroupId(List<Guid> goodsIds, Guid companyId, Guid purchaseGroupId, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (goodsIds.Count > 0)
            {
                var strbGoodsIds = new StringBuilder();
                foreach (var id in goodsIds)
                {
                    if (string.IsNullOrEmpty(strbGoodsIds.ToString()))
                    {
                        strbGoodsIds.Append("'").Append(id).Append("'");
                    }
                    else
                    {
                        strbGoodsIds.Append(",'").Append(id).Append("'");
                    }
                }
                string sql = string.Format("UPDATE [lmshop_PurchaseSet] SET PurchaseGroupId='{0}' WHERE GoodsId IN ({1}) AND CompanyId='{2}'", purchaseGroupId, strbGoodsIds, companyId);
                try
                {
                    return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    throw new ApplicationException(ex.Message, ex);
                }
            }
            return 1;
        }

        public int UpdatePurchaseSetDefault(Guid companyId, Guid purchaseGroupId, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (companyId != Guid.Empty && purchaseGroupId != Guid.Empty)
            {
                string sql = string.Format("UPDATE [lmshop_PurchaseSet] SET PurchaseGroupId='{0}' WHERE PurchaseGroupId='{1}' AND CompanyId='{2}'", Guid.Empty, purchaseGroupId, companyId);
                try
                {
                    return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    throw new ApplicationException(ex.Message, ex);
                }
            }
            return 1;
        }

        /// <summary>新版删除商品采购设置（假删除）
        /// </summary>
        /// <param name="goodsIds">商品ID集合</param>
        /// <returns>受影响的行数</returns>
        public int NewDeletePurchaseSet(List<Guid> goodsIds)
        {
            if (goodsIds.Count == 0)
            {
                return 0;
            }
            var strbGoodsIds = new StringBuilder();
            foreach (var goodsId in goodsIds)
            {
                if (string.IsNullOrEmpty(strbGoodsIds.ToString()))
                    strbGoodsIds.Append("'").Append(goodsId).Append("'");
                else
                    strbGoodsIds.Append(",'").Append(goodsId).Append("'");
            }
            var strbSql = new StringBuilder();
            strbSql.Append("UPDATE [lmshop_PurchaseSet] SET IsDelete=0 WHERE [GoodsId] IN (").Append(strbGoodsIds).Append(");");
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, strbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        /// <summary>新版删除商品采购设置（假删除）
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="state">状态枚举（0禁用，1启用）</param>
        /// <returns>受影响的行数</returns>
        public int NewDeletePurchaseSet(Guid goodsId, Guid warehouseId, State state)
        {
            int isDelete = state == State.Enable ? 1 : 0;
            const string SQL = @"UPDATE [lmshop_PurchaseSet] SET IsDelete=@IsDelete WHERE [GoodsId]=@GoodsId And [WarehouseId]=@WarehouseId";

            var parms = new[]
                           {
                               new SqlParameter("@IsDelete", SqlDbType.Int) { Value = isDelete },
                               new SqlParameter(PARM_GOODSID, SqlDbType.UniqueIdentifier) { Value = goodsId },
                               new SqlParameter(PARM_WAREHOUSEID, SqlDbType.UniqueIdentifier) { Value = warehouseId }
                           };
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        /// <summary>根据采购负责人ID获取其绑定的供应商ID集合
        /// </summary>
        /// <param name="personnelId">采购人负责人ID</param>
        /// <returns></returns>
        public IList<CompanyCussentInfo> GetCompanyIds(Guid personnelId)
        {
            const string SQL = @"
SELECT CC.CompanyId,CompanyName FROM lmShop_CompanyCussent AS CC
		INNER JOIN (
		SELECT DISTINCT CompanyId
		FROM [dbo].[lmshop_PurchaseSet]
		WHERE PersonResponsible=@PersonnelId
		) 
AS PS ON CC.CompanyId=PS.CompanyId
WHERE CC.CompanyType=1--供应商 
 AND CC.[State]=1--启用";
            IList<CompanyCussentInfo> list = new List<CompanyCussentInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, new SqlParameter("@PersonnelId", personnelId)))
            {
                while (dr.Read())
                {
                    var info = new CompanyCussentInfo
                    {
                        CompanyId = SqlRead.GetGuid(dr, "CompanyId"),
                        CompanyName = SqlRead.GetString(dr, "CompanyName")
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>根据供应商ID获取绑定此供应商的商品ID集合   2015-04-17  陈重文
        /// </summary>
        /// <param name="companyId">供应商ID</param>
        /// <returns></returns>
        public IList<Guid> GetGoodsIdByCompanyId(Guid companyId)
        {
            const string SQL = @"select distinct GoodsId from lmshop_PurchaseSet  where CompanyId=@CompanyId";
            IList<Guid> goodsIds = new List<Guid>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, new SqlParameter("@CompanyId", companyId)))
            {
                while (dr.Read())
                {
                    goodsIds.Add(SqlRead.GetGuid(dr, "GoodsId"));
                }
            }
            return goodsIds;
        }

        /// <summary>获取采购责任人所负责商品ID集合  2015-04-30  陈重文
        /// </summary>
        /// <param name="personnelId">责任人ID</param>
        /// <param name="companyId">供应商ID</param>
        /// <returns></returns>
        public IList<Guid> GetGoodsIdByPersonnelId(Guid personnelId, Guid companyId)
        {
            string sql = @"select distinct GoodsId from lmshop_PurchaseSet  where 1=1";
            if (personnelId != Guid.Empty)
                sql += " AND PersonResponsible='" + personnelId + "'";
            if (companyId != Guid.Empty)
                sql += " AND CompanyId='" + companyId + "'";
            IList<Guid> goodsIds = new List<Guid>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                while (dr.Read())
                {
                    goodsIds.Add(SqlRead.GetGuid(dr, "GoodsId"));
                }
            }
            return goodsIds;
        }

        /// <summary>获取仓库下所有的商品采购设置，含供应商名称（库存周转率使用） 2015-06-13  陈重文  
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <returns></returns>
        public IList<PurchaseSetInfo> GetAllPurchaseSet(Guid warehouseId)
        {
            const string SQL = @"
select ps.CompanyId,ps.PersonResponsible,ps.GoodsId,cc.CompanyName from lmshop_PurchaseSet ps
inner join lmShop_CompanyCussent cc
on ps.CompanyId=cc.CompanyId
where IsDelete=1
and ps.WarehouseId=@WarehouseId";
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<PurchaseSetInfo>(true, SQL, new Parameter("@WarehouseId", warehouseId)).ToList();
            }
        }

        /// <summary>
        /// 根据仓库id和主商品Id获得供应商
        /// 2016-08-04  
        /// 文雯  
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="goodsId">主商品Id</param>
        /// <returns></returns>
        public Guid GetCompanyByWarehouseIdAndGoodsId(Guid warehouseId, Guid hostingFilialeId, Guid goodsId)
        {
            const string SQL = @"
            SELECT CompanyId FROM lmshop_PurchaseSet WITH(NOLOCK)
            WHERE WarehouseId=@WarehouseId AND HostingFilialeId=@HostingFilialeId 
            AND GoodsId=@GoodsId";
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, new[]{
                new SqlParameter("@WarehouseId",warehouseId),
                new SqlParameter("@HostingFilialeId",hostingFilialeId),
                new SqlParameter("@GoodsId",goodsId)  }))
            {
                if (dr.Read())
                {
                    return SqlRead.GetGuid(dr, "CompanyId");
                }
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 根据仓库Id获取供应商Id
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        /// zal 2017-04-19
        public Dictionary<Guid, Guid> GetCompanyIdByWarehouseId(Guid warehouseId, Guid hostingFilialeId)
        {
            Dictionary<Guid, Guid> dicGoodsIdAndCompanyId = new Dictionary<Guid, Guid>();
            const string SQL = @"
            SELECT GoodsId,CompanyId FROM lmshop_PurchaseSet WITH(NOLOCK)
            WHERE WarehouseId=@WarehouseId AND HostingFilialeId=@HostingFilialeId";
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, new[] { new SqlParameter("@WarehouseId", warehouseId), new SqlParameter("@HostingFilialeId", hostingFilialeId) }))
            {
                while (dr.Read())
                {
                    dicGoodsIdAndCompanyId.Add(SqlRead.GetGuid(dr, "GoodsId"), SqlRead.GetGuid(dr, "CompanyId"));
                }
            }
            return dicGoodsIdAndCompanyId;
        }


        public bool IsExist(Guid warehouseId, Guid hostingFiliaeId, Guid goodsId)
        {
            const string SQL = @"SELECT COUNT(*) FROM lmshop_PurchaseSet WITH(NOLOCK) WHERE WarehouseId=@WarehouseId AND HostingFilialeId=@HostingFilialeId AND GoodsId=@GoodsId";
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<Int32>(true, SQL, new[] { new Parameter("@WarehouseId", warehouseId), new Parameter("@HostingFilialeId", hostingFiliaeId), new Parameter("@GoodsId", goodsId) }) > 0;
            }
        }
    }
}

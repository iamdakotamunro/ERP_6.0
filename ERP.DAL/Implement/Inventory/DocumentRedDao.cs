using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    /************************************************************************************ 
     * 创建人：  zal
     * 创建时间：2016/09/30
     * 描述    :
     * =====================================================================
     * 修改时间：2016/09/30 
     * 修改人  ：  
     * 描述    ：
     */
    public class DocumentRedDao : IDocumentRedDao
    {
        private const string SELECE = @"SELECT 
           [RedId]
          ,[FilialeId]
          ,[ThirdCompanyId]
          ,[TradeCode]
          ,[DateCreated]
          ,[Transactor]
          ,[Description]
          ,[AccountReceivable]
          ,[SubtotalQuantity]
          ,[RedType]
          ,[DocumentType]
          ,[State]
          ,[WarehouseId]
          ,[AuditTime]
          ,[StorageType]
          ,[LinkTradeCode]
          ,[LinkTradeId]
          ,[LinkDateCreated]
          ,[LinkDescription]  
          ,[Memo]
          ,[IsOut]
          FROM DocumentRed ";

        public DocumentRedDao(GlobalConfig.DB.FromType fromType)
        {

        }

        /// <summary>
        /// 红冲单添加
        /// </summary>
        /// <param name="documentRedInfo">红冲单据模型</param>
        public bool InsertDocumentRed(DocumentRedInfo documentRedInfo)
        {
            #region 红冲单据
            //红冲单据
            const string SQL_INSERT_STORAGERECORD = @"INSERT INTO [DocumentRed]
           ([RedId]
           ,[FilialeId]
           ,[ThirdCompanyId]
           ,[TradeCode]
           ,[DateCreated]
           ,[Transactor]
           ,[Description]
           ,[AccountReceivable]
           ,[SubtotalQuantity]
           ,[RedType]
           ,[DocumentType]
           ,[State]
           ,[WarehouseId]
           ,[AuditTime]
           ,[StorageType]
           ,[LinkTradeCode]
           ,[LinkTradeId]
           ,[LinkDateCreated]
           ,[LinkDescription]
           ,[Memo]
           ,[IsOut])
     VALUES
           (@RedId,@FilialeId,@ThirdCompanyId,@TradeCode,@DateCreated,@Transactor,@Description,@AccountReceivable,@SubtotalQuantity,@RedType,
            @DocumentType,@State,@WarehouseId,@AuditTime,@StorageType,@LinkTradeCode,@LinkTradeId,@LinkDateCreated,@LinkDescription,@Memo,@IsOut)";

            var parms = new[]
                {
                    new SqlParameter("@RedId", documentRedInfo.RedId), 
                    new SqlParameter("@FilialeId", documentRedInfo.FilialeId),
                    new SqlParameter("@ThirdCompanyId", documentRedInfo.ThirdCompanyId),
                    new SqlParameter("@TradeCode", documentRedInfo.TradeCode),
                    new SqlParameter("@DateCreated", documentRedInfo.DateCreated),
                    new SqlParameter("@Transactor", documentRedInfo.Transactor),
                    new SqlParameter("@Description", documentRedInfo.Description),
                    new SqlParameter("@AccountReceivable", Math.Round(documentRedInfo.AccountReceivable, 2)),
                    new SqlParameter("@SubtotalQuantity", documentRedInfo.SubtotalQuantity),
                    new SqlParameter("@RedType", documentRedInfo.RedType),
                    new SqlParameter("@DocumentType", documentRedInfo.DocumentType),
                    new SqlParameter("@State", documentRedInfo.State),
                    new SqlParameter("@WarehouseId", documentRedInfo.WarehouseId),
                    new SqlParameter("@AuditTime", documentRedInfo.AuditTime),
                    new SqlParameter("@StorageType", documentRedInfo.StorageType),
                    new SqlParameter("@LinkTradeCode", documentRedInfo.LinkTradeCode),
                    new SqlParameter("@LinkTradeId", documentRedInfo.LinkTradeId),
                    new SqlParameter("@LinkDateCreated", documentRedInfo.LinkDateCreated),
                    new SqlParameter("@LinkDescription", documentRedInfo.LinkDescription),
                    new SqlParameter("@Memo", documentRedInfo.Memo),
                    new SqlParameter("@IsOut", documentRedInfo.IsOut)
                };
            #endregion
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_STORAGERECORD, parms) > 0;
        }

        /// <summary>
        /// 批量添加红冲单据明细
        /// </summary>
        /// <param name="documentRedDetailList">红冲单据明细List</param>
        public bool BatchInsertDocumentRedDetail(IList<DocumentRedDetailInfo> documentRedDetailList)
        {
            var dic = new Dictionary<string, string>
                {
                    {"ID","ID"},{"RedId","RedId"},{"GoodsId","GoodsId"},{"RealGoodsId","RealGoodsId"},{"GoodsName","GoodsName"},{"GoodsCode","GoodsCode"},
                    {"Specification","Specification"},{"Quantity","Quantity"},{"UnitPrice","UnitPrice"},{"OldUnitPrice","OldUnitPrice"},{"Units","Units"}
                };

            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, documentRedDetailList, "DocumentRedDetail", dic) > 0;
        }

        /// <summary>
        /// 更新红冲单数据
        /// </summary>
        /// <param name="redId">红冲单据ID</param>
        /// <param name="accountReceivable">总金额</param>
        /// <param name="description">描述</param>
        /// <param name="memo">备注</param>
        /// <param name="state"></param>
        /// zal 2016-09-29
        public bool UpdateDocumentRedByRedId(Guid redId, decimal accountReceivable, string description, string memo, int state)
        {
            const string SQL = @"Update DocumentRed SET AccountReceivable=@AccountReceivable,Description=@Description,[Memo]=[Memo]+@Memo,[State]=@State WHERE RedId=@RedId";
            var parm = new[]{
                              new SqlParameter("@RedId",redId),
                              new SqlParameter("@AccountReceivable", accountReceivable),
                              new SqlParameter("@Description", description),
                              new SqlParameter("@Memo",memo),
                              new SqlParameter("@State",state), 
                             };

            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parm) > 0;
        }

        /// <summary>
        /// 更新红冲单据状态和描述
        /// </summary>
        /// <param name="redId">出入库单据ID</param>
        /// <param name="state">单据状态</param>
        /// <param name="description">描述</param>
        /// <param name="memo">备注</param>
        public bool UpdateStateDocumentRed(Guid redId, DocumentRedState state, string description, string memo)
        {
            var sql = new StringBuilder();
            sql.Append("Update DocumentRed SET State=@State,Description=@Description,[Memo]=[Memo]+@Memo ");
            sql.Append(!state.Equals((int)DocumentRedState.Finished)
                ? ",AuditTime=GETDATE() WHERE RedId=@RedId"
                : " WHERE RedId=@RedId");

            var parm = new[]{
                              
                              new SqlParameter("@State", (int)state),
                              new SqlParameter("@Description",description),
                              new SqlParameter("@Memo",memo),
                              new SqlParameter("@RedId",redId)
                             };

            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql.ToString(), parm) > 0;
        }
        
        /// <summary>
        /// 根据红冲Id删除数据
        /// </summary>
        /// <param name="redId"></param>
        /// <returns></returns>
        /// zal 2016-09-29
        public bool DelDocumentRedByRedId(Guid redId)
        {
            const string SQL = @"delete DocumentRed WHERE RedId=@RedId ";
            var parm = new[]{
                              new SqlParameter("@RedId",redId)
                            };

            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parm) > 0;
        }

        /// <summary>
        /// 根据红冲Id删除红冲明细
        /// </summary>
        /// <param name="redId"></param>
        /// <returns></returns>
        /// zal 2016-09-29
        public bool DelDocumentRedDetailByRedId(Guid redId)
        {
            const string SQL = @"delete DocumentRedDetail WHERE RedId=@RedId ";
            var parm = new[]{
                              new SqlParameter("@RedId",redId)
                            };

            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parm) > 0;
        }

        /// <summary>
        /// 红冲单据分页查询
        /// </summary>
        /// <param name="warehouseId">仓库</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="redType">红冲类型</param>
        /// <param name="documentType">单据类型</param>
        /// <param name="state">状态</param>
        /// <param name="no">单号</param>
        /// <param name="startPage">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public IList<DocumentRedInfo> GetDocumentRedListToPage(Guid warehouseId, DateTime startTime, DateTime endTime, int redType, int documentType, int state, string no, int startPage, int pageSize, out long recordCount)
        {
            var sql = new StringBuilder(@"
           SELECT [RedId]
          ,[FilialeId]
          ,[ThirdCompanyId]
          ,[TradeCode]
          ,[DateCreated]
          ,[Transactor]
          ,[Description]
          ,[AccountReceivable]
          ,[SubtotalQuantity]
          ,[RedType]
          ,[DocumentType]
          ,[State]
          ,[WarehouseId]
          ,[AuditTime]
          ,[StorageType]
          ,[LinkTradeCode]
          ,[LinkTradeId]
          ,[LinkDateCreated]
          ,[LinkDescription]
          ,[Memo]
          ,[IsOut]
           FROM [DocumentRed] WITH(NOLOCK) WHERE 1=1");
            if (warehouseId != Guid.Empty)
            {
                sql.Append(" AND WarehouseId='").Append(warehouseId).Append("'");
            }
            if (startTime != DateTime.MinValue)
            {
                sql.Append(" AND DateCreated>='").Append(startTime).Append("'");
            }
            if (endTime != DateTime.MinValue)
            {
                sql.Append(" AND DateCreated<'").Append(endTime).Append("'");
            }
            if (!redType.Equals(0))
            {
                sql.Append(" AND RedType='").Append(redType).Append("'");
            }
            if (!state.Equals(0))
            {
                sql.Append(" AND State='").Append(state).Append("'");
            }
            if (!documentType.Equals(-1))
            {
                sql.Append(" AND DocumentType ='").Append(documentType).Append("'");
            }
            if (!string.IsNullOrEmpty(no))
            {
                sql.Append(" AND (TradeCode like '%").Append(no).Append("%'");
                sql.Append(" OR LinkTradeCode like '%").Append(no).Append("%')");
            }

            using (var db = DatabaseFactory.Create())
            {
                var pageQuery = new Keede.DAL.Helper.Sql.PageQuery(startPage, pageSize, sql.ToString(), " DateCreated DESC ");
                var pageItem = db.SelectByPage<DocumentRedInfo>(true, pageQuery);
                recordCount = pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }

        /// <summary>
        /// 根据单据红冲ID获取记录信息
        /// </summary>
        /// <param name="redId">记录Id</param>
        /// <returns></returns>
        public DocumentRedInfo GetDocumentRed(Guid redId)
        {
            var parm = new SqlParameter("@RedId", SqlDbType.UniqueIdentifier) { Value = redId };
            var stockInfo = new DocumentRedInfo();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format("{0} WHERE RedId=@RedId", SELECE), parm))
            {
                if (rdr.Read())
                {
                    stockInfo = ReaderDocumentRedInfo(rdr);
                }
            }
            return stockInfo;
        }

        /// <summary>
        /// 获取红冲单
        /// </summary>
        /// <param name="redId"></param>
        /// <returns></returns>
        public DocumentRedInfo GetDocumentRedByNewRedId(Guid redId)
        {
            var parm = new[] {new  SqlParameter("@RedId", SqlDbType.UniqueIdentifier) { Value = redId }, new SqlParameter("@DocumentType", SqlDbType.Int) { Value = (Int32)DocumentType.RedDocument } };
            var stockInfo = new DocumentRedInfo();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format("{0} WHERE LinkTradeCode =(SELECT LinkTradeCode FROM DocumentRed WHERE RedId=@RedId) and DocumentType=@DocumentType", SELECE), parm))
            {
                if (rdr.Read())
                {
                    stockInfo = ReaderDocumentRedInfo(rdr);
                }
            }
            return stockInfo;
        }

        /// <summary>
        /// 获取出入库单据的红冲单据
        /// </summary>
        /// <param name="linkeTradeCode"></param>
        /// <returns></returns>
        public IList<DocumentRedInfo> GetDocumentRedInfoByLinkTradeCode(string linkeTradeCode)
        {
            var parm = new SqlParameter("@LinkTradeCode", SqlDbType.VarChar) { Value = linkeTradeCode };
            var stockInfos = new List<DocumentRedInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format("{0} WHERE LinkTradeCode=@LinkTradeCode", SELECE), parm))
            {
                while (rdr.Read())
                {
                    stockInfos.Add(ReaderDocumentRedInfo(rdr));
                }
            }
            return stockInfos;
        }

        /// <summary>
        /// 获取出入库单据红冲生成的新单
        /// </summary>
        /// <param name="linkTradeId"></param>
        /// <returns></returns>
        public IList<DocumentRedInfo> GetDocumentRedInfoByLinkTradeId(Guid linkTradeId)
        {
            var parm = new SqlParameter("@LinkTradeId", SqlDbType.UniqueIdentifier) { Value = linkTradeId };
            var stockInfos = new List<DocumentRedInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format("{0} WHERE LinkTradeId=@LinkTradeId",SELECE), parm))
            {
                while (rdr.Read())
                {
                    stockInfos.Add(ReaderDocumentRedInfo(rdr));
                }
            }
            return stockInfos;
        }

        /// <summary>
        ///  根据单据红冲记录ID获取明细
        /// </summary>
        /// <param name="redId">红冲记录ID</param>
        /// <returns></returns>
        public IList<DocumentRedDetailInfo> GetDocumentRedDetailListByRedId(Guid redId)
        {
            const string SQL = @"
            SELECT 
             A.[ID]
            ,A.[RedId]
            ,A.[GoodsId]
            ,A.[GoodsName]
            ,A.[GoodsCode]
            ,A.[RealGoodsId]
            ,A.[Specification]
            ,A.[Quantity]
            ,A.[UnitPrice]
            ,A.[OldUnitPrice]
            ,A.[Units] 
            FROM DocumentRedDetail AS A WITH(NOLOCK) 
            INNER JOIN DocumentRed AS B WITH(NOLOCK) ON A.RedId=B.RedId AND A.RedId=@RedId
            ORDER BY A.Specification ASC";

            var parm = new SqlParameter("@RedId", SqlDbType.UniqueIdentifier) { Value = redId };
            IList<DocumentRedDetailInfo> documentRedDetailList = new List<DocumentRedDetailInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (dr.Read())
                {
                    var documentRedDetailInfo = new DocumentRedDetailInfo
                    {
                        Id = dr["ID"] == DBNull.Value ? Guid.Empty : new Guid(dr["ID"].ToString()),
                        RedId = dr["RedId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RedId"].ToString()),
                        GoodsId = dr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsId"].ToString()),
                        GoodsName = dr["GoodsName"] == DBNull.Value ? string.Empty : dr["GoodsName"].ToString(),
                        GoodsCode = dr["GoodsCode"] == DBNull.Value ? string.Empty : dr["GoodsCode"].ToString(),
                        RealGoodsId = dr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RealGoodsId"].ToString()),
                        Specification = dr["Specification"] == DBNull.Value ? string.Empty : dr["Specification"].ToString(),
                        Quantity = dr["Quantity"] == DBNull.Value ? 0 : int.Parse(dr["Quantity"].ToString()),
                        UnitPrice = dr["UnitPrice"] == DBNull.Value ? 0 : decimal.Parse(dr["UnitPrice"].ToString()),
                        OldUnitPrice = dr["OldUnitPrice"] == DBNull.Value ? 0 : decimal.Parse(dr["OldUnitPrice"].ToString()),
                        Units = dr["Units"] == DBNull.Value ? string.Empty : dr["Units"].ToString()
                    };
                    documentRedDetailList.Add(documentRedDetailInfo);
                }
            }
            return documentRedDetailList;
        }

        /// <summary>
        ///  根据出入库ID获取出入库明细
        /// </summary>
        /// <param name="stockId">出入库ID</param>
        /// <returns></returns>
        public IList<DocumentRedDetailInfo> GetDocumentRedDetailListByStockId(Guid stockId)
        {
            const string SQL = @"
             SELECT 
             A.[GoodsId]
            ,A.[GoodsName]
            ,A.[GoodsCode]
            ,A.[RealGoodsId]
            ,A.[Specification]
            ,A.[Quantity]
            ,A.[UnitPrice]
            FROM StorageRecordDetail AS A WITH(NOLOCK) 
            INNER JOIN StorageRecord AS B WITH(NOLOCK) ON A.StockId=B.StockId AND A.StockId=@StockId
            ORDER BY A.Specification ASC";

            var parm = new SqlParameter("@StockId", SqlDbType.UniqueIdentifier) { Value = stockId };
            IList<DocumentRedDetailInfo> documentRedDetailList = new List<DocumentRedDetailInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (dr.Read())
                {
                    var documentRedDetailInfo = new DocumentRedDetailInfo
                    {
                        GoodsId = dr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsId"].ToString()),
                        GoodsName = dr["GoodsName"] == DBNull.Value ? string.Empty : dr["GoodsName"].ToString(),
                        GoodsCode = dr["GoodsCode"] == DBNull.Value ? string.Empty : dr["GoodsCode"].ToString(),
                        RealGoodsId = dr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RealGoodsId"].ToString()),
                        Specification = dr["Specification"] == DBNull.Value ? string.Empty : dr["Specification"].ToString(),
                        Quantity = dr["Quantity"] == DBNull.Value ? 0 : int.Parse(dr["Quantity"].ToString()),
                        UnitPrice = dr["UnitPrice"] == DBNull.Value ? 0 : decimal.Parse(dr["UnitPrice"].ToString()),
                        OldUnitPrice = dr["UnitPrice"] == DBNull.Value ? 0 : decimal.Parse(dr["UnitPrice"].ToString())
                    };
                    documentRedDetailList.Add(documentRedDetailInfo);
                }
            }
            return documentRedDetailList;
        }

        /// <summary> 获取DocumentRedInfo表信息
        /// </summary>
        /// <param name="dr">IDataReader</param>
        /// <returns></returns>
        private static DocumentRedInfo ReaderDocumentRedInfo(IDataReader dr)
        {
            var documentRedInfo = new DocumentRedInfo
            {
                RedId = dr["RedId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RedId"].ToString()),
                FilialeId = dr["FilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["FilialeId"].ToString()),
                ThirdCompanyId = dr["ThirdCompanyId"] == DBNull.Value ? Guid.Empty : new Guid(dr["ThirdCompanyId"].ToString()),
                TradeCode = dr["TradeCode"] == DBNull.Value ? string.Empty : dr["TradeCode"].ToString(),
                DateCreated = dr["DateCreated"] == DBNull.Value ? DateTime.Parse("1900-01-01") : DateTime.Parse(dr["DateCreated"].ToString()),
                Transactor = dr["Transactor"] == DBNull.Value ? string.Empty : dr["Transactor"].ToString(),
                Description = dr["Description"] == DBNull.Value ? string.Empty : dr["Description"].ToString(),
                AccountReceivable = dr["AccountReceivable"] == DBNull.Value ? 0 : decimal.Parse(dr["AccountReceivable"].ToString()),
                SubtotalQuantity = dr["SubtotalQuantity"] == DBNull.Value ? 0 : decimal.Parse(dr["SubtotalQuantity"].ToString()),
                RedType = dr["RedType"] == DBNull.Value ? 0 : int.Parse(dr["RedType"].ToString()),
                DocumentType = dr["DocumentType"] == DBNull.Value ? 0 : int.Parse(dr["DocumentType"].ToString()),
                State = dr["State"] == DBNull.Value ? 0 : int.Parse(dr["State"].ToString()),
                WarehouseId = dr["WarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(dr["WarehouseId"].ToString()),
                AuditTime = dr["AuditTime"] == DBNull.Value ? DateTime.Parse("1900-01-01") : Convert.ToDateTime(dr["AuditTime"]),
                StorageType = dr["StorageType"] == DBNull.Value ? 0 : int.Parse(dr["StorageType"].ToString()),
                LinkTradeCode = dr["LinkTradeCode"] == DBNull.Value ? string.Empty : dr["LinkTradeCode"].ToString(),
                LinkTradeId = dr["LinkTradeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["LinkTradeId"].ToString()),
                LinkDateCreated = dr["LinkDateCreated"] == DBNull.Value ? DateTime.Parse("1900-01-01") : Convert.ToDateTime(dr["LinkDateCreated"]),
                LinkDescription = dr["LinkDescription"] == DBNull.Value ? string.Empty : dr["LinkDescription"].ToString(),
                Memo = dr["Memo"] == DBNull.Value ? string.Empty : dr["Memo"].ToString(),
                //IsOut = dr["IsOut"] != DBNull.Value && bool.Parse(dr["IsOut"].ToString())
            };
            return documentRedInfo;
        }
    }
}

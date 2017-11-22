using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.DAL.RWSplitting;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 借记单
    /// </summary>
    public class DebitNote : IDebitNote
    {
        public DebitNote(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        #region [SQL_DebitNote 语句]
        private const string SQL_SELECT_DEBITNOTE = "SELECT [PurchasingId],[PurchasingNo],[CompanyId],[PresentAmount],[CreateDate],[FinishDate],[State],[WarehouseId],[Memo],[PersonResponsible],[NewPurchasingId],[PurchaseGroupId],[Title],[ActivityTimeStart],[ActivityTimeEnd] FROM [lmshop_DebitNote]";
        private const string SQL_INSERT_DEBITNOTE = @"
IF EXISTS(SELECT 0 FROM [lmshop_DebitNote] WHERE [PurchasingId]=@PurchasingId)
BEGIN
	UPDATE [lmshop_DebitNote] SET [PresentAmount]+=@PresentAmount WHERE [PurchasingId]=@PurchasingId
END
ELSE
BEGIN
    INSERT INTO [lmshop_DebitNote]([PurchasingId],[PurchasingNo],
[CompanyId],[PresentAmount],[CreateDate],[FinishDate],[State],[WarehouseId],[Memo],[PersonResponsible],[PurchaseGroupId],[Title],[ActivityTimeStart],[ActivityTimeEnd]) 
VALUES(@PurchasingId,@PurchasingNo,@CompanyId,@PresentAmount,@CreateDate,@FinishDate,@State,@WarehouseId,@Memo,@PersonResponsible,@PurchaseGroupId,@Title,@ActivityTimeStart,@ActivityTimeEnd)
END
";
        private const string SQL_DELETE_DEBITNOTE = "DELETE FROM [lmshop_DebitNote] WHERE [PurchasingId]=@PurchasingId;";
        #endregion

        #region [SQL_DebitNoteDetail 语句]
        private const string SQL_SELECT_DEBITNOTEDETAIL = "SELECT [PurchasingId],[GoodsId],[GoodsName],[Specification],[GivingCount],[ArrivalCount],[Price],[State],[Amount],[Memo],[ID] FROM [lmshop_DebitNoteDetail]";
        private const string SQL_INSERT_DEBITNOTEDETAIL = @"
IF EXISTS(SELECT 0 FROM [lmshop_DebitNoteDetail] WHERE [PurchasingId]=@PurchasingId AND [GoodsId]=@GoodsId)
BEGIN
	UPDATE [lmshop_DebitNoteDetail] SET [GivingCount]+=@GivingCount WHERE [PurchasingId]=@PurchasingId AND [GoodsId]=@GoodsId
END
ELSE
BEGIN
	INSERT INTO [lmshop_DebitNoteDetail]([PurchasingId],[GoodsId],[GoodsName],[Specification],[GivingCount],[ArrivalCount],[Price],[State],[Amount],[Memo],[ID]) VALUES(@PurchasingId,@GoodsId,@GoodsName,@Specification,@GivingCount,@ArrivalCount,@Price,@State,@Amount,@Memo,@ID)
END
";
        private const string SQL_DELETE_DEBITNOTEDETAIL = "DELETE FROM [lmshop_DebitNoteDetail] WHERE [PurchasingId]=@PurchasingId;";
        #endregion

        #region [参数 DebitNote]
        private const string PARM_PURCHASINGID = "@PurchasingId";
        private const string PARM_PURCHASINGNO = "@PurchasingNo";
        private const string PARM_COMPANYID = "@CompanyId";
        private const string PARM_PRESENTAMOUNT = "@PresentAmount";
        private const string PARM_CREATEDATE = "@CreateDate";
        private const string PARM_FINISHDATE = "@FinishDate";
        private const string PARM_STATE = "@State";
        private const string PARM_WAREHOUSEID = "@WarehouseId";
        private const string PARM_MEMO = "@Memo";
        private const string PARM_PERSONRESPONSIBLE = "@PersonResponsible";
        private const string PARM_NEWPURCHASINGID = "@NewPurchasingId";
        #endregion

        private static SqlParameter[] GetDebitNotetParameters()
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_PURCHASINGID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_PURCHASINGNO, SqlDbType.VarChar, 50),
                                new SqlParameter(PARM_COMPANYID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_PRESENTAMOUNT, SqlDbType.Float),
                                new SqlParameter(PARM_CREATEDATE, SqlDbType.DateTime),
                                new SqlParameter(PARM_FINISHDATE, SqlDbType.DateTime),
                                new SqlParameter(PARM_STATE, SqlDbType.Int),
                                new SqlParameter(PARM_WAREHOUSEID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_MEMO, SqlDbType.VarChar, 5000),
                                new SqlParameter(PARM_PERSONRESPONSIBLE, SqlDbType.UniqueIdentifier),
                                new SqlParameter("@PurchaseGroupId",SqlDbType.UniqueIdentifier),
                                new SqlParameter("@Title",SqlDbType.VarChar),
                                new SqlParameter("@ActivityTimeStart",SqlDbType.DateTime),
                                new SqlParameter("@ActivityTimeEnd",SqlDbType.DateTime)
                            };
            return parms;
        }

        #region [参数 DebitNoteDetail]
        private const string PARM_GOODSID = "@GoodsId";
        private const string PARM_GOODSNAME = "@GoodsName";
        private const string PARM_SPECIFICATION = "@Specification";
        private const string PARM_GIVINGCOUNT = "@GivingCount";
        private const string PARM_ARRIVALCOUNT = "@ArrivalCount";
        private const string PARM_PRICE = "@Price";
        private const string PARM_AMOUNT = "@Amount";
        private const string PARM_ID = "@ID";

        #endregion

        private static SqlParameter[] GetDebitNoteDetailParameters()
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_PURCHASINGID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_GOODSID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_GOODSNAME, SqlDbType.VarChar, 100),
                                new SqlParameter(PARM_SPECIFICATION, SqlDbType.VarChar, 200),
                                new SqlParameter(PARM_GIVINGCOUNT, SqlDbType.Int),
                                new SqlParameter(PARM_ARRIVALCOUNT, SqlDbType.Int),
                                new SqlParameter(PARM_PRICE, SqlDbType.Float),
                                new SqlParameter(PARM_STATE, SqlDbType.Int),
                                new SqlParameter(PARM_AMOUNT, SqlDbType.Float),
                                new SqlParameter(PARM_MEMO, SqlDbType.VarChar, 5000),
                                new SqlParameter(PARM_ID, SqlDbType.UniqueIdentifier)
                            };
            return parms;
        }

        private static DebitNoteInfo ReaderDebitNote(IDataReader dr)
        {
            var info = new DebitNoteInfo
            {
                PurchasingId = dr["PurchasingId"] == DBNull.Value ? Guid.Empty : new Guid(dr["PurchasingId"].ToString()),
                PurchasingNo = dr["PurchasingNo"] == DBNull.Value ? string.Empty : dr["PurchasingNo"].ToString(),
                CompanyId = dr["CompanyId"] == DBNull.Value ? Guid.Empty : new Guid(dr["CompanyId"].ToString()),
                PresentAmount = dr["PresentAmount"] == DBNull.Value ? 0 : decimal.Parse(dr["PresentAmount"].ToString()),
                CreateDate = dr["CreateDate"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dr["CreateDate"].ToString()),
                FinishDate = dr["FinishDate"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dr["FinishDate"].ToString()),
                State = dr["State"] == DBNull.Value ? 0 : int.Parse(dr["State"].ToString()),
                WarehouseId = dr["WarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(dr["WarehouseId"].ToString()),
                Memo = dr["Memo"] == DBNull.Value ? string.Empty : dr["Memo"].ToString(),
                PersonResponsible = dr["PersonResponsible"] == DBNull.Value ? Guid.Empty : new Guid(dr["PersonResponsible"].ToString()),
                NewPurchasingId = dr["NewPurchasingId"] == DBNull.Value ? Guid.Empty : new Guid(dr["NewPurchasingId"].ToString()),
                PurchaseGroupId = dr["PurchaseGroupId"] == DBNull.Value ? (Guid?)null : new Guid(dr["PurchaseGroupId"].ToString()),
                Title = dr["Title"] == DBNull.Value ? string.Empty : dr["Title"].ToString()
            };
            return info;
        }

        private DebitNoteDetailInfo ReaderDebitNoteDetail(IDataReader dr)
        {
            var info = new DebitNoteDetailInfo
            {
                PurchasingId = dr["PurchasingId"] == DBNull.Value ? Guid.Empty : new Guid(dr["PurchasingId"].ToString()),
                GoodsId = dr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsId"].ToString()),
                GoodsName = dr["GoodsName"] == DBNull.Value ? string.Empty : dr["GoodsName"].ToString(),
                Specification = dr["Specification"] == DBNull.Value ? string.Empty : dr["Specification"].ToString(),
                GivingCount = dr["GivingCount"] == DBNull.Value ? 0 : int.Parse(dr["GivingCount"].ToString()),
                ArrivalCount = dr["ArrivalCount"] == DBNull.Value ? 0 : int.Parse(dr["ArrivalCount"].ToString()),
                Price = dr["Price"] == DBNull.Value ? 0 : decimal.Parse(dr["Price"].ToString()),
                State = dr["State"] == DBNull.Value ? 0 : int.Parse(dr["State"].ToString()),
                Amount = dr["Amount"] == DBNull.Value ? 0 : decimal.Parse(dr["Amount"].ToString()),
                Memo = dr["Memo"] == DBNull.Value ? string.Empty : dr["Memo"].ToString(),
                Id = dr["ID"] == DBNull.Value ? Guid.Empty : new Guid(dr["ID"].ToString())
            };
            return info;
        }

        /// <summary>
        /// 获取所有借记单
        /// </summary>
        /// <returns></returns>
        public IList<DebitNoteInfo> GetDebitNoteList()
        {
            IList<DebitNoteInfo> list = new List<DebitNoteInfo>();
            const string SQL = SQL_SELECT_DEBITNOTE + " ORDER BY CreateDate ASC";
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, null))
            {
                while (dr.Read())
                {
                    list.Add(ReaderDebitNote(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// 根据条件筛选数据
        /// </summary>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="state">借记单状态</param>
        /// <param name="warehouseId">仓库id</param>
        /// <param name="companyId">供应商id</param>
        /// <param name="personResponsibleId">责任人id</param>
        /// <param name="activityTimeStart">活动开始时间</param>
        /// <param name="activityTimeEnd">活动结束时间</param>
        /// <param name="titleOrMemo">标题或备注</param>
        /// <param name="startPage">开始页索引</param>
        /// <param name="pageSize">每页显示的数量</param>
        /// <param name="recordCount">查询数据的总和</param>
        /// <returns></returns>
        public IList<DebitNoteInfo> GetDebitNoteList(DateTime startTime, DateTime endTime, int state, Guid warehouseId, Guid companyId, Guid personResponsibleId,string activityTimeStart,string activityTimeEnd,string titleOrMemo, int startPage, int pageSize, out long recordCount)
        {
            var strb = new StringBuilder(SQL_SELECT_DEBITNOTE);
            strb.Append(" WHERE 1=1 ");
            if (startTime != DateTime.MinValue)
                strb.Append(" AND [CreateDate]>='").Append(startTime.ToString("yyyy-MM-dd 00:00:00")).Append("'");
            if (endTime != DateTime.MinValue)
                strb.Append(" AND [CreateDate]<'").Append(endTime.AddDays(1).ToString("yyyy-MM-dd 00:00:00")).Append("'");
            if (state != -1)
                strb.Append(" AND [State]=").Append(state);
            if (warehouseId != Guid.Empty)
                strb.Append(" AND [WarehouseId]='").Append(warehouseId).Append("'");
            if (companyId != Guid.Empty)
                strb.Append(" AND [CompanyId]='").Append(companyId).Append("'");
            if (personResponsibleId != Guid.Empty)
                strb.Append(" AND [PersonResponsible]='").Append(personResponsibleId).Append("'");
            if (!string.IsNullOrEmpty(activityTimeStart))
            {
                strb.Append(" AND [ActivityTimeStart]>='").Append(DateTime.Parse(activityTimeStart)).Append("'");
            }
            if (!string.IsNullOrEmpty(activityTimeEnd))
            {
                strb.Append(" AND [ActivityTimeEnd]<='").Append(DateTime.Parse(activityTimeEnd).AddDays(1).AddMilliseconds(-1)).Append("'");
            }
            if (!string.IsNullOrEmpty(titleOrMemo))
            {
                strb.Append(" AND [Title] like '%").Append(titleOrMemo).Append("%' or [Memo] like '%").Append(titleOrMemo).Append("%'");
            }

            using (var db = DatabaseFactory.Create())
            {
                var pageQuery = new Keede.DAL.Helper.Sql.PageQuery(startPage, pageSize, strb.ToString(), " [State],[CreateDate] DESC ");
                var pageItem = db.SelectByPage<DebitNoteInfo>(true, pageQuery, null);
                recordCount = pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }

        /// <summary>
        /// 根据采购单ID获取借记单
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <returns></returns>
        public DebitNoteInfo GetDebitNoteInfo(Guid purchasingId)
        {
            const string SQL = SQL_SELECT_DEBITNOTE + " WHERE [PurchasingId]=@PurchasingId";
            var parm = new SqlParameter(PARM_PURCHASINGID, SqlDbType.UniqueIdentifier) { Value = purchasingId };

            DebitNoteInfo info = null;
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (dr.Read())
                {
                    info = ReaderDebitNote(dr);
                }
            }
            return info;
        }

        /// <summary>
        /// 根据新采购单ID获取借记单
        /// </summary>
        /// <param name="newPurchasingId"></param>
        /// <returns></returns>
        public DebitNoteInfo GetDebitNoteInfoByNewPurchasingId(Guid newPurchasingId)
        {
            const string SQL = SQL_SELECT_DEBITNOTE + " WHERE [NewPurchasingId]=@NewPurchasingId";
            var parm = new SqlParameter(PARM_NEWPURCHASINGID, SqlDbType.UniqueIdentifier) { Value = newPurchasingId };

            DebitNoteInfo info = null;
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (dr.Read())
                {
                    info = ReaderDebitNote(dr);
                }
            }
            return info;
        }

        public Guid GetPurchasingIdByNewPurchasingId(Guid newPurchasingId)
        {
            const string SQL = @"SELECT [PurchasingId] FROM [lmshop_DebitNote] WHERE [NewPurchasingId]=@NewPurchasingId";
            var parm = new SqlParameter(PARM_NEWPURCHASINGID, SqlDbType.UniqueIdentifier) { Value = newPurchasingId };
            var obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL, parm);
            return obj!=null && obj!=DBNull.Value?new Guid(obj.ToString()) : Guid.Empty;
        }

        /// <summary>
        /// 添加借记单和明细
        /// </summary>
        /// <param name="info"></param>
        /// <param name="debitNoteDetailList">借记单明细</param>
        public bool AddPurchaseSetAndDetail(DebitNoteInfo info, List<DebitNoteDetailInfo> debitNoteDetailList)
        {
            SqlParameter[] parms = GetDebitNotetParameters();
            parms[0].Value = info.PurchasingId;
            parms[1].Value = info.PurchasingNo;
            parms[2].Value = info.CompanyId;
            parms[3].Value = info.PresentAmount;
            parms[4].Value = info.CreateDate;
            parms[5].Value = info.FinishDate == DateTime.MinValue ? SqlDateTime.MinValue : info.FinishDate;
            parms[6].Value = info.State;
            parms[7].Value = info.WarehouseId;
            parms[8].Value = info.Memo;
            parms[9].Value = info.PersonResponsible;
            parms[10].Value = info.PurchaseGroupId;
            parms[11].Value = info.Title;
            parms[12].Value = info.ActivityTimeStart;
            parms[13].Value = info.ActivityTimeEnd;

            SqlParameter[] detailParms = GetDebitNoteDetailParameters();

            using (var conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, SQL_INSERT_DEBITNOTE, parms);
                    foreach (var detailInfo in debitNoteDetailList)
                    {
                        detailParms[0].Value = info.PurchasingId;
                        detailParms[1].Value = detailInfo.GoodsId;
                        detailParms[2].Value = detailInfo.GoodsName;
                        detailParms[3].Value = detailInfo.Specification;
                        detailParms[4].Value = detailInfo.GivingCount;
                        detailParms[5].Value = detailInfo.ArrivalCount;
                        detailParms[6].Value = detailInfo.Price;
                        detailParms[7].Value = detailInfo.State;
                        detailParms[8].Value = detailInfo.Amount;
                        detailParms[9].Value = detailInfo.Memo;
                        detailParms[10].Value = detailInfo.Id;
                        SqlHelper.ExecuteNonQuery(trans, SQL_INSERT_DEBITNOTEDETAIL, detailParms);
                    }
                    trans.Commit();
                }
                catch (SqlException ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
            return true;
        }

        /// <summary>
        /// 删除借记单
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        public void DeleteDebitNote(Guid purchasingId)
        {
            const string SQL = SQL_DELETE_DEBITNOTEDETAIL + " " + SQL_DELETE_DEBITNOTE;
            var parm = new SqlParameter(PARM_PURCHASINGID, SqlDbType.UniqueIdentifier) { Value = purchasingId };

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 根据采购单ID修改借记单状态
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="state"></param>
        public void UpdateDebitNoteStateByPurchasingId(Guid purchasingId, int state)
        {
            string sql = "UPDATE [lmshop_DebitNote] SET [State]=@State ";
            if (state == (int)DebitNoteState.AllComplete)
            {
                sql += ",[FinishDate]='" + DateTime.Now + "'";
            }
            sql += " WHERE [PurchasingId]=@PurchasingId;";
            var parms = new[]
                                     {
                                         new SqlParameter(PARM_STATE,SqlDbType.Int),
                                         new SqlParameter(PARM_PURCHASINGID,SqlDbType.UniqueIdentifier) 
                                     };
            parms[0].Value = state;
            parms[1].Value = purchasingId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 根据新采购单ID修改借记单状态
        /// </summary>
        /// <param name="newPurchasingId"></param>
        /// <param name="state"></param>
        public void UpdateDebitNoteStateByNewPurchasingId(Guid newPurchasingId, int state)
        {
            string sql = "UPDATE [lmshop_DebitNote] SET [State]=@State ";
            if (state == (int)DebitNoteState.AllComplete)
            {
                sql += ",[FinishDate]='" + DateTime.Now + "'";
            }
            sql += " WHERE [NewPurchasingId]=@NewPurchasingId;";
            var parms = new[]
                                     {
                                         new SqlParameter(PARM_STATE,SqlDbType.Int),
                                         new SqlParameter(PARM_NEWPURCHASINGID,SqlDbType.UniqueIdentifier) 
                                     };
            parms[0].Value = state;
            parms[1].Value = newPurchasingId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 根据采购单ID修改借记单新采购单ID
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="newPurchasingId"></param>
        public void UpdateDebitNoteNewPurchasingIdByPurchasingId(Guid purchasingId, Guid newPurchasingId)
        {
            const string SQL = "UPDATE [lmshop_DebitNote] SET [NewPurchasingId]=@NewPurchasingId WHERE [PurchasingId]=@PurchasingId;";
            var parms = new[]
                                     {
                                         new SqlParameter("@NewPurchasingId",SqlDbType.UniqueIdentifier),
                                         new SqlParameter(PARM_PURCHASINGID,SqlDbType.UniqueIdentifier) 
                                     };
            parms[0].Value = newPurchasingId;
            parms[1].Value = purchasingId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 获取借记单明细
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        /// <returns></returns>
        public IList<DebitNoteDetailInfo> GetDebitNoteDetailList(Guid purchasingId)
        {
            const string SQL = SQL_SELECT_DEBITNOTEDETAIL + " WHERE [PurchasingId]=@PurchasingId";
            var parm = new SqlParameter(PARM_PURCHASINGID, SqlDbType.UniqueIdentifier) { Value = purchasingId };

            IList<DebitNoteDetailInfo> list = new List<DebitNoteDetailInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (dr.Read())
                {
                    list.Add(ReaderDebitNoteDetail(dr));
                }
            }
            return list;
        }

        /// <summary>
        /// 根据采购单ID和商品ID更改借记单明细状态
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="goodsId"></param>
        /// <param name="state"></param>
        /// <param name="arrivalCount">实到数量</param>
        /// <param name="isUpdate">是否包含更改“实到数量”</param>
        public void UpdateDebitNoteDetail(Guid purchasingId, Guid goodsId, int state, int arrivalCount, bool isUpdate)
        {
            string sql = "UPDATE [lmshop_DebitNoteDetail] SET [State]=@State";
            if (isUpdate)
            {
                sql += ",ArrivalCount=@ArrivalCount";
            }
            sql += " WHERE [PurchasingId]=@PurchasingId AND [GoodsId]=@GoodsId;";
            var parms = new[]
                                     {
                                         new SqlParameter(PARM_PURCHASINGID,SqlDbType.UniqueIdentifier),
                                         new SqlParameter(PARM_GOODSID,SqlDbType.UniqueIdentifier),
                                         new SqlParameter(PARM_STATE,SqlDbType.Int) ,
                                         new SqlParameter(PARM_ARRIVALCOUNT,SqlDbType.Int) 
                                     };
            parms[0].Value = purchasingId;
            parms[1].Value = goodsId;
            parms[2].Value = state;
            parms[3].Value = arrivalCount;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 添加借记单明细
        /// </summary>
        /// <param name="detailInfo"></param>
        public void AddDebitNoteDetail(DebitNoteDetailInfo detailInfo)
        {
            SqlParameter[] detailParms = GetDebitNoteDetailParameters();
            try
            {
                detailParms[0].Value = detailInfo.PurchasingId;
                detailParms[1].Value = detailInfo.GoodsId;
                detailParms[2].Value = detailInfo.GoodsName;
                detailParms[3].Value = detailInfo.Specification;
                detailParms[4].Value = detailInfo.GivingCount;
                detailParms[5].Value = detailInfo.ArrivalCount;
                detailParms[6].Value = detailInfo.Price;
                detailParms[7].Value = detailInfo.State;
                detailParms[8].Value = detailInfo.Amount;
                detailParms[9].Value = detailInfo.Memo;
                detailParms[10].Value = detailInfo.Id;
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_DEBITNOTEDETAIL, detailParms);
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>添加赠品借记单备注  ADD 2015-02-06  陈重文
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="memo"> </param>
        public void AddDebitNoteMemo(Guid purchasingId, string memo)
        {
            const string sql = @"UPDATE [lmshop_DebitNote] SET Memo=Memo+@Memo WHERE PurchasingId=@PurchasingId";
            var parms = new[]
                                     {
                                         new SqlParameter("@Memo",SqlDbType.VarChar){Value =memo },
                                         new SqlParameter("@PurchasingId",SqlDbType.UniqueIdentifier){Value = purchasingId}
                                     };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}

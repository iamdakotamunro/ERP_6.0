using System;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>往来帐核对操作类
    /// </summary>
    public class ReckoningCheck : IReckoningCheck
    {
        public ReckoningCheck(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        private const string SQL_SELECT_RECKONINGCHECK_BY_RECKONINGID = "SELECT  [ReckoningId] ,[Memo] ,[DateCreated] FROM [dbo].[ReckoningCheck] WHERE ReckoningId=@ReckoningId";
        private const string SQL_UPDATE_RECKONINGCHECK_BY_RECKONINGID = "UPDATE [dbo].[ReckoningCheck]  SET [Memo] =@Memo  ,[DateCreated] =@DateCreated WHERE ReckoningId=@ReckoningId";
        private const string SQL_DELETE_RECKONINGCHECK_BY_RECKONINGID = "DELETE FROM [dbo].[ReckoningCheck] WHERE ReckoningId=@ReckoningId ";

        private const string PARM_RECKONING_ID = "@ReckoningId";
        private const string PARM_MEMO = "@Memo";
        private const string PARM_DATE_CREATED = "@DateCreated";

        /// <summary>根据账单编号查询往来账核对信息
        /// </summary>
        /// <returns></returns>
        public ReckoningCheckInfo GetReckoningCheckByReckoningId(Guid reckoningId)
        {
            var parm = new SqlParameter(PARM_RECKONING_ID, SqlDbType.UniqueIdentifier) { Value = reckoningId };
            ReckoningCheckInfo reckoningCheckInfo;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_RECKONINGCHECK_BY_RECKONINGID, parm))
            {

                reckoningCheckInfo = rdr.Read() ? new ReckoningCheckInfo { Memo = rdr["Memo"].ToString(), DateCreated = DateTime.Parse(rdr["DateCreated"].ToString()), ReckoningId = new Guid(rdr["ReckoningId"].ToString()) } : null;
            }
            return reckoningCheckInfo;
        }

        /// <summary>插入一条往来账核对信息
        /// </summary>
        /// <param name="reckoningCheckInfo"></param>
        public void InsertReckoningCheck(ReckoningCheckInfo reckoningCheckInfo)
        {
            const string SQL_INSERT_RECKONINGCHECK = @"
IF EXISTS(SELECT ReckoningId FROM [ReckoningCheck]  WHERE ReckoningId=@ReckoningId)
    UPDATE dbo.ReckoningCheck SET [Memo]=[Memo]+@Memo WHERE ReckoningId=@ReckoningId
 ELSE		
	INSERT INTO ReckoningCheck([ReckoningId],[Memo],DateCreated) VALUES (@ReckoningId,@Memo,@DateCreated);";
            var parms = new[]
                            {
                                new SqlParameter(PARM_RECKONING_ID, SqlDbType.UniqueIdentifier){Value = reckoningCheckInfo.ReckoningId}, 
                                new SqlParameter(PARM_MEMO, SqlDbType.VarChar){Value = reckoningCheckInfo.Memo}, 
                                new SqlParameter(PARM_DATE_CREATED, SqlDbType.DateTime){Value = reckoningCheckInfo.DateCreated}
                            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_RECKONINGCHECK, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>更新往来账核对信息
        /// </summary>
        /// <param name="reckoningCheckInfo"> </param>
        public void UpdateReckoningCheck(ReckoningCheckInfo reckoningCheckInfo)
        {
            var parm = new[]
                            {
                                new SqlParameter(PARM_RECKONING_ID, SqlDbType.UniqueIdentifier){Value = reckoningCheckInfo.ReckoningId}, 
                                new SqlParameter(PARM_MEMO, SqlDbType.VarChar){Value = reckoningCheckInfo.Memo}, 
                                new SqlParameter(PARM_DATE_CREATED, SqlDbType.DateTime){Value = reckoningCheckInfo.DateCreated}
                            };

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_RECKONINGCHECK_BY_RECKONINGID, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary> 删除往来账核对信息
        /// </summary>
        /// <param name="reckoningId"></param>
        public void DeleteReckoningCheck(Guid reckoningId)
        {
            var parm = new[]
                            {
                                new SqlParameter(PARM_RECKONING_ID, SqlDbType.UniqueIdentifier){Value = reckoningId}
                            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_RECKONINGCHECK_BY_RECKONINGID, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

    }
}

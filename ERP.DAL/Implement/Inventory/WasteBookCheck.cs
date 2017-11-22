using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;
using ERP.Environment;
using Dapper;
using Keede.DAL.RWSplitting;

namespace ERP.DAL.Implement.Inventory
{
    public class WasteBookCheck : IWasteBookCheck
    {
        public WasteBookCheck(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        private const string SQL_DELETE_WASTEBOOKCHECK = "DELETE FROM WasteBookCheck WHERE WasteBookId=@WasteBookId;";
        private const string SQL_SELECT_WASTEBOOKCHECK = "SELECT WasteBookId,CheckMoney,Memo,DateCreated FROM WasteBookCheck WHERE WasteBookId=@WasteBookId;";

        private const string SQL_UPDATE_WASTEBOOKCHECK = "UPDATE WasteBookCheck SET CheckMoney=@CheckMoney,Memo=@Memo,DateCreated=@DateCreated WHERE WasteBookId=@WasteBookId;";
        private const string SQL_INSERT_WASTEBOOKCHECK = "INSERT INTO WasteBookCheck values (@WasteBookId,@CheckMoney,@Memo,@DateCreated)";

        private const string PARM_WASTEBOOKID = "@WasteBookId";
        private const string PARM_CHECKMONEY = "@CheckMoney";
        private const string PARM_MEMO = "@Memo";
        private const string PARM_DATECREATED = "@DateCreated";

        /// <summary>
        /// 添加一笔账目核对记录
        /// </summary>
        /// <param name="wasteBookCheck">账目信息核对类</param>
        public void Insert(WasteBookCheckInfo wasteBookCheck)
        {
            SqlParameter[] parms = GetInsertParameters();
            parms[0].Value = wasteBookCheck.WasteBookId;
            parms[1].Value = wasteBookCheck.CheckMoney;
            parms[2].Value = wasteBookCheck.Memo;
            parms[3].Value = DateTime.Now;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_WASTEBOOKCHECK, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        ///<summary>
        ///更改一笔账目核对记录
        /// </summary>
        /// <param name="wasteBookCheck">账目信息核对类</param>
        public void Update(WasteBookCheckInfo wasteBookCheck)
        {
            SqlParameter[] parms = GetInsertParameters();
            parms[0].Value = wasteBookCheck.WasteBookId;
            parms[1].Value = wasteBookCheck.CheckMoney;
            parms[2].Value = wasteBookCheck.Memo;
            parms[3].Value = DateTime.Now;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_WASTEBOOKCHECK, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 删除一笔核对记录
        /// </summary>
        /// <param name="wasteBookId"></param>
        public void DeleteWasteBookCheck(Guid wasteBookId)
        {
            var parm = new SqlParameter(PARM_WASTEBOOKID, SqlDbType.UniqueIdentifier) { Value = wasteBookId };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_WASTEBOOKCHECK, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        /// <summary>
        /// 获取指定资金帐号核对记录
        /// </summary>
        /// <param name="wasteBookId">帐号操作id</param>
        /// <returns></returns>
        public WasteBookCheckInfo GetWasteBookCheck(Guid wasteBookId)
        {
            var parm = new SqlParameter(PARM_WASTEBOOKID, SqlDbType.UniqueIdentifier) { Value = wasteBookId };
            WasteBookCheckInfo wasteBookCheckInfo;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_WASTEBOOKCHECK, parm))
            {
                wasteBookCheckInfo = rdr.Read() ? new WasteBookCheckInfo { CheckMoney = decimal.Parse(rdr["CheckMoney"].ToString()), DateCreated = DateTime.Parse(rdr["DateCreated"].ToString()), Memo = rdr["Memo"].ToString(), WasteBookId = new Guid(rdr["WasteBookId"].ToString()) } : null;
            }
            return wasteBookCheckInfo;
        }

        private static SqlParameter[] GetInsertParameters()
        {
            var parms = new[] {
                new SqlParameter(PARM_WASTEBOOKID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_CHECKMONEY, SqlDbType.Decimal),
                new SqlParameter(PARM_MEMO, SqlDbType.VarChar),
                new SqlParameter(PARM_DATECREATED, SqlDbType.DateTime)
            };
            return parms;
        }
    }
}

using System;
using System.Collections.Generic;
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
    public class ManuallyCheckBillDal : IManuallyCheckBill
    {
        const string SQL_SELECT = "SELECT [Id],[CheckBillPersonnelId],[SalePlatformId],[TradeCode],[CheckState],[ThirdOrderTotalAmount],[UnusualOrderQuantity],[ConfirmTotalAmount],[ReceiptState],[CheckBillDate],[State],[Memo] FROM [dbo].[ManuallyCheckBill] ";
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回ManuallyCheckBill表的所有数据 
        /// </summary>
        /// <returns></returns>        
        public List<ManuallyCheckBillInfo> GetAllManuallyCheckBill()
        {
            List<ManuallyCheckBillInfo> manuallyCheckBillList = new List<ManuallyCheckBillInfo>();
            string sql = SQL_SELECT;
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null);
            while (reader.Read())
            {
                ManuallyCheckBillInfo manuallyCheckBill = new ManuallyCheckBillInfo(reader);
                manuallyCheckBillList.Add(manuallyCheckBill);
            }
            reader.Close();
            return manuallyCheckBillList;
        }

        /// <summary>
        /// 根据条件获取对账记录
        /// </summary>
        /// <param name="checkBillPersonnelId"></param>
        /// <param name="tradeCode"></param>
        /// <param name="checkState"></param>
        /// <param name="checkBillDateStart"></param>
        /// <param name="checkBillDateEnd"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="receiptState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<ManuallyCheckBillInfo> GetAllManuallyCheckBill(Guid checkBillPersonnelId, string tradeCode, CheckType checkState, DateTime checkBillDateStart, DateTime checkBillDateEnd, Guid salePlatformId, int receiptState, int pageIndex, int pageSize, out int total)
        {
            var sb = new StringBuilder(SQL_SELECT + " WHERE 1=1 ");
            if (!checkBillPersonnelId.Equals(Guid.Empty))
            {
                sb.AppendFormat(@" AND CheckBillPersonnelId='{0}'", checkBillPersonnelId);
            }
            if (!string.IsNullOrEmpty(tradeCode))
            {
                sb.AppendFormat(@" AND TradeCode='{0}'", tradeCode);
            }
            if (!checkState.Equals(CheckType.AllCheck))
            {
                sb.AppendFormat(@" AND CheckState={0}", (int)checkState);
            }
            if (checkBillDateStart != DateTime.MinValue)
            {
                sb.AppendFormat(@" AND CheckBillDate>='{0}'", checkBillDateStart);
            }
            if (checkBillDateEnd != DateTime.MaxValue)
            {
                sb.AppendFormat(@" AND CheckBillDate<'{0}'", checkBillDateEnd.AddDays(1));
            }
            if (!salePlatformId.Equals(Guid.Empty))
            {
                sb.AppendFormat(@" AND SalePlatformId='{0}'", salePlatformId);
            }
            if (receiptState != -1)
            {
                sb.AppendFormat(@" AND ReceiptState={0} ", receiptState);
            }

            using (var db = DatabaseFactory.Create())
            {
                var list = new Keede.DAL.Helper.Sql.PageQuery(pageIndex, pageSize, sb.ToString(), " CheckBillDate DESC");
                var pageItem = db.SelectByPage<ManuallyCheckBillInfo>(true, list);
                total = (int)pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }

        /// <summary>
        /// 根据ManuallyCheckBill表的id字段返回数据  
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        public List<ManuallyCheckBillInfo> GetManuallyCheckBillById(Guid id)
        {
            List<ManuallyCheckBillInfo> manuallyCheckBillList = new List<ManuallyCheckBillInfo>();
            string sql = SQL_SELECT + "where [Id] = @Id";
            SqlParameter[] paras = {
                new SqlParameter("@Id",id)
            };
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            while (reader.Read())
            {
                ManuallyCheckBillInfo manuallyCheckBill = new ManuallyCheckBillInfo(reader);
                manuallyCheckBillList.Add(manuallyCheckBill);
            }
            reader.Close();
            return manuallyCheckBillList;
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据ManuallyCheckBill表的id字段删除数据 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        public bool DeleteManuallyCheckBillById(Guid id)
        {
            string sql = "delete from [ManuallyCheckBill] where [Id] = @Id";
            SqlParameter[] paras = {
                new SqlParameter("@Id",id)
            };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region update data
        /// <summary>
        /// prepare parameters 
        /// </summary>
        public static SqlParameter[] PrepareCommandParameters(ManuallyCheckBillInfo manuallyCheckBillInfo)
        {
            SqlParameter[] paras = {
                new SqlParameter("@Id",manuallyCheckBillInfo.Id),
                new SqlParameter("@CheckBillPersonnelId",manuallyCheckBillInfo.CheckBillPersonnelId),
                new SqlParameter("@SalePlatformId",manuallyCheckBillInfo.SalePlatformId),
                new SqlParameter("@TradeCode",manuallyCheckBillInfo.TradeCode),
                new SqlParameter("@CheckState",manuallyCheckBillInfo.CheckState),
                new SqlParameter("@ThirdOrderTotalAmount",manuallyCheckBillInfo.ThirdOrderTotalAmount),
                new SqlParameter("@UnusualOrderQuantity",manuallyCheckBillInfo.UnusualOrderQuantity),
                new SqlParameter("@ConfirmTotalAmount",manuallyCheckBillInfo.ConfirmTotalAmount),
                new SqlParameter("@ReceiptState",manuallyCheckBillInfo.ReceiptState),
                new SqlParameter("@CheckBillDate",manuallyCheckBillInfo.CheckBillDate),
                new SqlParameter("@State",manuallyCheckBillInfo.State),
                new SqlParameter("@Memo",manuallyCheckBillInfo.Memo)
            };
            return paras;
        }
        /// <summary>
        /// 根据ManuallyCheckBill表的Id字段更新数据 
        /// </summary> 
        /// <param name="manuallyCheckBillInfo">manuallyCheckBillInfo</param>
        /// <returns></returns>       
        public bool UpdateManuallyCheckBillByBillId(ManuallyCheckBillInfo manuallyCheckBillInfo)
        {
            string sql = "update [ManuallyCheckBill] set [CheckBillPersonnelId] = @CheckBillPersonnelId,[SalePlatformId]=@SalePlatformId,[TradeCode] = @TradeCode,[CheckState] = @CheckState,[ThirdOrderTotalAmount] = @ThirdOrderTotalAmount,[UnusualOrderQuantity] = @UnusualOrderQuantity,[ConfirmTotalAmount] = @ConfirmTotalAmount,[ReceiptState] = @ReceiptState,[CheckBillDate] = @CheckBillDate,[State] = @State,[Memo] = @Memo where [Id] = @Id";
            SqlParameter[] paras = PrepareCommandParameters(manuallyCheckBillInfo);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向ManuallyCheckBill表插入一条数据
        /// </summary>
        /// <param name="manuallyCheckBillInfo">manuallyCheckBillInfo</param>       
        /// <returns></returns>        
        public bool AddManuallyCheckBill(ManuallyCheckBillInfo manuallyCheckBillInfo)
        {
            string sql = "insert into [ManuallyCheckBill]([Id],[CheckBillPersonnelId],[SalePlatformId],[TradeCode],[CheckState],[ThirdOrderTotalAmount],[UnusualOrderQuantity],[ConfirmTotalAmount],[ReceiptState],[CheckBillDate],[State],[Memo])values(@Id,@CheckBillPersonnelId,@SalePlatformId,@TradeCode,@CheckState,@ThirdOrderTotalAmount,@UnusualOrderQuantity,@ConfirmTotalAmount,@ReceiptState,@CheckBillDate,@State,@Memo)";
            SqlParameter[] paras = PrepareCommandParameters(manuallyCheckBillInfo);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #endregion
    }
}

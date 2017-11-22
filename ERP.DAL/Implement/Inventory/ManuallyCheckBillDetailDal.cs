using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;


namespace ERP.DAL.Implement.Inventory
{
    public class ManuallyCheckBillDetailDal : IManuallyCheckBillDetail
    {
        const string SQL_SELECT = "SELECT [Id],[ManuallyCheckBillId],[SystemOrderNo],[ThirdOrderNo],[OrderTime],[MemberId],[SystemOrderAmount],[ThirdOrderAmount],[Balance],[ConfirmAmount],[ContactsReckoningDifference] FROM [dbo].[ManuallyCheckBillDetail] ";
        #region .对本表的维护.
        #region select data
        /// <summary>
        /// 返回ManuallyCheckBillDetail表的所有数据 
        /// </summary>
        /// <returns></returns>        
        public List<ManuallyCheckBillDetailInfo> GetAllManuallyCheckBillDetail()
        {
            List<ManuallyCheckBillDetailInfo> manuallyCheckBillDetailList = new List<ManuallyCheckBillDetailInfo>();

            string sql = SQL_SELECT;
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null);
            while (reader.Read())
            {
                ManuallyCheckBillDetailInfo manuallyCheckBillDetail = new ManuallyCheckBillDetailInfo(reader);
                manuallyCheckBillDetailList.Add(manuallyCheckBillDetail);
            }
            reader.Close();
            return manuallyCheckBillDetailList;
        }
        /// <summary>
        /// 根据ManuallyCheckBillDetail表的id字段返回数据  
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        public List<ManuallyCheckBillDetailInfo> GetManuallyCheckBillDetailById(Guid id)
        {
            List<ManuallyCheckBillDetailInfo> manuallyCheckBillDetailList = new List<ManuallyCheckBillDetailInfo>();

            string sql = SQL_SELECT + "where [Id] = @Id";
            SqlParameter[] paras = {
                new SqlParameter("@Id",id)
            };
            var reader = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, paras);
            while (reader.Read())
            {
                ManuallyCheckBillDetailInfo manuallyCheckBillDetail = new ManuallyCheckBillDetailInfo(reader);
                manuallyCheckBillDetailList.Add(manuallyCheckBillDetail);
            }
            reader.Close();
            return manuallyCheckBillDetailList;
        }

        /// <summary>
        /// 根据ManuallyCheckBillDetail表的ManuallyCheckBillId字段返回数据  
        /// </summary>
        /// <param name="manuallyCheckBillId"></param>
        /// <param name="orderNo">系统订单号或者第三方订单号</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>        
        public List<ManuallyCheckBillDetailInfo> GetManuallyCheckBillDetailByManuallyCheckBillId(Guid manuallyCheckBillId, string orderNo, int pageIndex, int pageSize, out int total)
        {
            string sql = SQL_SELECT + "where [ManuallyCheckBillId] = '" + manuallyCheckBillId + "'";

            if (!string.IsNullOrEmpty(orderNo))
            {
                sql += " AND (SystemOrderNo='" + orderNo + "' OR ThirdOrderNo='" + orderNo + "')";
            }

            using (var db = DatabaseFactory.Create())
            {
                var list = new Keede.DAL.Helper.Sql.PageQuery(pageIndex, pageSize, sql, " OrderTime DESC");
                var pageItem = db.SelectByPage<ManuallyCheckBillDetailInfo>(true, list);
                total = (int)pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }
        #endregion
        #region delete data
        /// <summary>
        /// 根据ManuallyCheckBillDetail表的id字段删除数据 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>        
        public bool DeleteManuallyCheckBillDetailById(Guid id)
        {
            string sql = "delete from [ManuallyCheckBillDetail] where [Id] = @Id";
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
        public static SqlParameter[] PrepareCommandParameters(ManuallyCheckBillDetailInfo manuallyCheckBillDetailInfo)
        {
            SqlParameter[] paras = {
                new SqlParameter("@Id",manuallyCheckBillDetailInfo.Id),
                new SqlParameter("@ManuallyCheckBillId",manuallyCheckBillDetailInfo.ManuallyCheckBillId),
                new SqlParameter("@SystemOrderNo",manuallyCheckBillDetailInfo.SystemOrderNo),
                new SqlParameter("@ThirdOrderNo",manuallyCheckBillDetailInfo.ThirdOrderNo),
                new SqlParameter("@OrderTime",manuallyCheckBillDetailInfo.OrderTime),
                new SqlParameter("@MemberId",manuallyCheckBillDetailInfo.MemberId),
                new SqlParameter("@SystemOrderAmount",manuallyCheckBillDetailInfo.SystemOrderAmount),
                new SqlParameter("@ThirdOrderAmount",manuallyCheckBillDetailInfo.ThirdOrderAmount),
                new SqlParameter("@Balance",manuallyCheckBillDetailInfo.Balance),
                new SqlParameter("@ConfirmAmount",manuallyCheckBillDetailInfo.ConfirmAmount),
                new SqlParameter("@ContactsReckoningDifference",manuallyCheckBillDetailInfo.ContactsReckoningDifference)
            };
            return paras;
        }
        /// <summary>
        /// 根据ManuallyCheckBillDetail表的Id字段更新数据 
        /// </summary> 
        /// <param name="manuallyCheckBillDetailInfo">ManuallyCheckBillDetailInfo</param>
        /// <returns></returns>       
        public bool UpdateManuallyCheckBillDetailById(ManuallyCheckBillDetailInfo manuallyCheckBillDetailInfo)
        {
            string sql = "update [ManuallyCheckBillDetail] set [ManuallyCheckBillId] = @ManuallyCheckBillId,[SystemOrderNo]=@SystemOrderNo,[ThirdOrderNo] = @ThirdOrderNo,[OrderTime] = @OrderTime,[MemberId] = @MemberId,[SystemOrderAmount] = @SystemOrderAmount,[ThirdOrderAmount] = @ThirdOrderAmount,[Balance] = @Balance,[ConfirmAmount] = @ConfirmAmount,[ContactsReckoningDifference] = @ContactsReckoningDifference where [Id] = @Id";
            SqlParameter[] paras = PrepareCommandParameters(manuallyCheckBillDetailInfo);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }
        #endregion
        #region insert data
        /// <summary>
        /// 向ManuallyCheckBillDetail表插入一条数据
        /// </summary>
        /// <param name="manuallyCheckBillDetailInfo">ManuallyCheckBillDetailInfo</param>       
        /// <returns></returns>        
        public bool AddManuallyCheckBillDetail(ManuallyCheckBillDetailInfo manuallyCheckBillDetailInfo)
        {
            string sql = "insert into [ManuallyCheckBill]([Id],[ManuallyCheckBillId],[SystemOrderNo],[ThirdOrderNo],[OrderTime],[MemberId],[SystemOrderAmount],[ThirdOrderAmount],[Balance],[ConfirmAmount],[ContactsReckoningDifference])values(@Id,@ManuallyCheckBillId,@SystemOrderNo,@ThirdOrderNo,@IsCheck,@IsReceipt,@OrderTime,@MemberId,@SystemOrderAmount,@ThirdOrderAmount,@Balance,@ConfirmAmount,@ContactsReckoningDifference)";
            SqlParameter[] paras = PrepareCommandParameters(manuallyCheckBillDetailInfo);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, paras) > 0;
        }

        /// <summary>
        /// 批量插入人工对账明细
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddBatchManuallyCheckBillDetail(IList<ManuallyCheckBillDetailInfo> list)
        {
            var dics = new Dictionary<string, string>
            {
                {"Id","Id"},{"ManuallyCheckBillId","ManuallyCheckBillId"},{"SystemOrderNo","SystemOrderNo"},
                {"ThirdOrderNo","ThirdOrderNo"},{"OrderTime","OrderTime"},{"MemberId","MemberId"},
                { "SystemOrderAmount","SystemOrderAmount"},{"ThirdOrderAmount","ThirdOrderAmount"},{"Balance","Balance"},
                { "ConfirmAmount","ConfirmAmount"},{"ContactsReckoningDifference","ContactsReckoningDifference"}
            };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, list, "ManuallyCheckBillDetail", dics) > 0;
        }
        #endregion
        #endregion
    }
}

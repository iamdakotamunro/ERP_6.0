using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ERP.DAL.Interface.IShop;
using ERP.Environment;
using ERP.Model.ShopFront;
using Dapper;
using Keede.DAL.RWSplitting;
using System.Linq;
using System.Transactions;

namespace ERP.DAL.Implement.Shop
{
    /// <summary>
    /// 
    /// </summary>
    public class ShopExchangedApplyDal : IShopExchangedApply
    {
        public ShopExchangedApplyDal(GlobalConfig.DB.FromType fromType) { }

        #region 相关sql

        /// <summary>
        /// 添加退换货申请
        /// </summary>
        private const string SQL_INSERT_SHOPEXCHANGEDAPPLY = @"INSERT INTO ShopExchangedApply
           (ApplyID,ApplyNo,ShopID,ShopName,ApplyTime,SubQuantity,SubPrice,ExchangedState,IsBarter,[Description],MsgID)
     VALUES(@ApplyID,@ApplyNo,@ShopID,@ShopName,@ApplyTime,@SubQuantity,@SubPrice,@ExchangedState,@IsBarter,@Description,@MsgID)";

        /// <summary>
        /// 修改退换货申请
        /// </summary>
        private const string SQL_UPDATE_SHOPEXCHANGEDAPPLY = @"UPDATE ShopExchangedApply SET ApplyTime=@ApplyTime,SubQuantity=@SubQuantity,
SubPrice=@SubPrice,ExchangedState=@ExchangedState,[Description]=@Description WHERE ApplyID=@ApplyID";

        /// <summary>
        /// 修改退换货申请状态
        /// </summary>
        private const string SQL_UPDATE_SHOPEXCHANGEDAPPLY_STATE = @"UPDATE ShopExchangedApply SET ExchangedState=@ExchangedState,
[Description]=[Description]+@Description WHERE ApplyID=@ApplyID";

        /// <summary>
        /// 删除退换货申请
        /// </summary>
        private const string SQL_DELETE_SHOPEXCHANGEDAPPLY = @"DELETE ShopExchangedApply WHERE ApplyID=@ApplyID ";

        /// <summary>
        /// 查询退换货申请
        /// </summary>
        private const string SQL_SELECT_SHOPEXCHANGEDAPPLY = @"SELECT A.ApplyID,A.ApplyNo,A.ShopID,A.ShopName,A.ApplyTime,A.SubQuantity,
A.SubPrice,A.ExchangedState,A.IsBarter,A.[Description],ISNULL(A.MsgID,'00000000-0000-0000-0000-000000000000') AS MsgID,
ISNULL(M.ApplyContent,'') AS MsgContent FROM ShopExchangedApply A 
LEFT JOIN ShopRefundMessage M ON A.MsgID=M.MsgID ";

        /// <summary>
        /// 换货申请
        /// </summary>
        private const string SQL_SELECT_SHOPBARTERAPPLY = @"SELECT A.ApplyID,A.ApplyNo,A.ShopID,A.ShopName,A.ApplyTime,A.SubQuantity,
A.SubPrice,A.ExchangedState,A.IsBarter,A.[Description] FROM ShopExchangedApply A ";

        /// <summary>
        /// 获取退换货申请状态
        /// </summary>
        private const string SQL_SELECT_EXCHANGEDSTATE = "SELECT ExchangedState FROM ShopExchangedApply WHERE ApplyID='{0}' ";

        /// <summary>
        /// 获取退换货申请状态
        /// </summary>
        private const string SQL_SELECT_EXCHANGEDSTATEBYNO ="SELECT ExchangedState FROM ShopExchangedApply WHERE ApplyNo='{0}' ";
        #endregion

        #region 相关参数
        private const string PARM_APPLYID = "@ApplyID";
        private const string PARM_APPLYNO = "@ApplyNo";
        private const string PARM_SHOPID = "@ShopID";
        private const string PARM_SHOPNAME = "@ShopName";
        private const string PARM_APPLYTIME = "@ApplyTime";
        private const string PARM_SUBQUANTITY = "@SubQuantity";
        private const string PARM_SUBPRICE = "@SubPrice";
        private const string PARM_EXCHANGEDSTATE = "@ExchangedState";
        private const string PARM_ISBARTER = "@IsBarter";
        private const string PARM_DESCRIPTION = "@Description";
        private const string PARM_MSGID = "@MsgID";
        #endregion

        /// <summary>
        /// 添加退换货申请
        /// </summary>
        /// <param name="applyInfo"></param>
        public void InsertShopExchangedApply(ShopExchangedApplyInfo applyInfo)
        {
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    conn.Execute(SQL_INSERT_SHOPEXCHANGEDAPPLY, new
                    {
                        ApplyID = applyInfo.ApplyID,
                        ApplyNo = applyInfo.ApplyNo,
                        ShopID = applyInfo.ShopID,
                        ShopName = applyInfo.ShopName,
                        ApplyTime = applyInfo.ApplyTime,
                        SubQuantity = applyInfo.SubQuantity,
                        SubPrice = applyInfo.SubPrice,
                        ExchangedState = applyInfo.ExchangedState,
                        IsBarter = applyInfo.IsBarter,
                        Description = applyInfo.Description,
                        MsgID = applyInfo.MsgID,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退换货申请插入失败!", ex);
            }
        }

        /// <summary>
        /// 修改退换货申请
        /// 数量、金额、时间、备注、状态（可修改）
        /// </summary>
        /// <param name="applyInfo"></param>
        /// <returns></returns>
        public int UpdateShopExchangedApply(ShopExchangedApplyInfo applyInfo)
        {
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(SQL_UPDATE_SHOPEXCHANGEDAPPLY, new
                    {
                        ApplyTime = applyInfo.ApplyTime,
                        SubQuantity = applyInfo.SubQuantity,
                        SubPrice = applyInfo.SubPrice,
                        ExchangedState = applyInfo.ExchangedState,
                        Description = applyInfo.Description,
                        ApplyID = applyInfo.ApplyID
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退换货申请修改失败!", ex);
            }
        }

        /// <summary>
        /// 修改退换货申请状态同时添加备注
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="state"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public int UpdateExchangeState(Guid applyId, int state, string description)
        {
            var parms = new[] {
                new SqlParameter(PARM_EXCHANGEDSTATE, SqlDbType.Int){Value = state},
                new SqlParameter(PARM_DESCRIPTION, SqlDbType.VarChar){Value = description},
                new SqlParameter(PARM_APPLYID, SqlDbType.UniqueIdentifier){Value = applyId}
            };
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(SQL_UPDATE_SHOPEXCHANGEDAPPLY_STATE, new {
                        ExchangedState = state,
                        Description = description,
                        ApplyID = applyId });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退换货申请状态修改失败!", ex);
            }
        }

        /// <summary>
        /// 获取当月的退货记录
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// zal 2017-03-15
        public Dictionary<string, decimal> SelectOneMonthReturnedApplyList(Guid shopId, DateTime dateTime)
        {
            var sql = @"
            SELECT ApplyNo,ISNULL(SubPrice,0) AS ReturnSum 
            FROM ShopExchangedApply WITH(NOLOCK)
            WHERE ExchangedState=9 AND IsBarter=1 
            AND ApplyTime>='" + Convert.ToDateTime(dateTime.ToString("yyyy-MM-01")) + @"' AND ApplyTime<'" + Convert.ToDateTime(dateTime.AddMonths(1).ToString("yyyy-MM-01")) + @"'
            AND ShopID='" + shopId + @"'
            ORDER BY ApplyTime DESC
            ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query(sql).ToDictionary(kv => (string)kv.ApplyNo, kv => (decimal)kv.ReturnSum);
            }
        }

        /// <summary>
        /// 删除退换货申请
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public int DeleteShopExchangedApply(Guid applyId)
        {
            var parms = new[] {
                new SqlParameter(PARM_APPLYID, SqlDbType.UniqueIdentifier){Value = applyId}
            };
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(SQL_DELETE_SHOPEXCHANGEDAPPLY, new { ApplyID = applyId });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退换货申请删除失败!", ex);
            }
        }

        /// <summary>
        /// 获取退换货申请
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public ShopExchangedApplyInfo GetShopExchangedApplyInfo(Guid applyId)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPEXCHANGEDAPPLY);
            builder.Append(" WHERE A.ApplyID=@ApplyID");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<ShopExchangedApplyInfo>(builder.ToString(), new { ApplyID = applyId });
            }
        }

        /// <summary>
        /// 通过退换货申请单号获取退换货申请
        /// </summary>
        /// <param name="applyNo"></param>
        /// <returns></returns>
        public ShopExchangedApplyInfo GetShopExchangedApplyInfoByApplyNo(string applyNo)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPEXCHANGEDAPPLY);
            builder.Append(" WHERE A.ApplyNo=@ApplyNo ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<ShopExchangedApplyInfo>(builder.ToString(), new { ApplyNo = applyNo });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isBarter"></param>
        /// <param name="applyNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="goodsIds"></param>
        /// <param name="shopId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IEnumerable<ShopExchangedApplyInfo> GetShopExchangedApplyList(bool isBarter, string applyNo,
            DateTime startTime, DateTime endTime,IList<Guid> goodsIds, Guid shopId, int state)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPEXCHANGEDAPPLY);
            var goodsStr = new StringBuilder();
            builder.AppendFormat(" WHERE A.ApplyTime >= '{0}'", startTime);
            builder.AppendFormat(" AND A.ApplyTime < '{0}'", endTime);
            builder.AppendFormat(" AND A.IsBarter={0}",isBarter?1:0);
            if(!string.IsNullOrEmpty(applyNo))
            {
                builder.AppendFormat(" AND A.ApplyNo='{0}'",applyNo);
            }
            if (shopId != Guid.Empty)
            {
                builder.AppendFormat(" AND A.ShopID='{0}'",shopId);
            }
            if(state!=-1)
            {
                builder.AppendFormat(" AND A.ExchangedState={0}",state);
            }
            
            if (goodsIds!=null && goodsIds.Count>0)
            {
                builder.Append(" AND A.ApplyID IN(SELECT ApplyID FROM ShopExchangedApplyDetail WHERE GoodsID IN(");
                for (int i = 0; i < goodsIds.Count; i++)
                {
                    if (goodsIds[i] == Guid.Empty)
                        continue;
                    goodsStr.AppendFormat("'{0}'", goodsIds[i]).Append(goodsIds.Count - 1 == i ? "" : ",");
                }
                if (goodsStr.Length > 0)
                {
                    builder.Append(goodsStr).Append(")");
                }
                if(!isBarter) //换货
                {
                    builder.AppendFormat(" OR BarterGoodsID IN ({0})", goodsStr);
                }
                builder.Append(" GROUP BY ApplyID ) ");
            }
            builder.Append(" ORDER BY A.ApplyTime DESC ");
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ShopExchangedApplyInfo>(builder.ToString());
            }
        }

        /// <summary>
        /// 获取换货申请列表
        /// </summary>
        /// <param name="applyNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="goodsIds"></param>
        /// <param name="shopId"></param>
        /// <param name="state"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <returns></returns>
        public IEnumerable<ShopExchangedApplyInfo> GetShopBarterApplyList(string applyNo, DateTime startTime, DateTime endTime, 
            IList<Guid> goodsIds, Guid shopId, int state, string goodsNameOrCode)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPBARTERAPPLY);
            builder.AppendFormat(" WHERE A.ApplyTime >= '{0}'", startTime);
            builder.AppendFormat(" AND A.ApplyTime < '{0}'", endTime);
            builder.Append(" AND A.IsBarter=0 ");
            if (!string.IsNullOrEmpty(applyNo))
            {
                builder.AppendFormat(" AND A.ApplyNo='{0}'",applyNo);
            }
            if (shopId != Guid.Empty)
            {
                builder.AppendFormat(" AND A.ShopID='{0}'",shopId);
            }
            if (state != -1)
            {
                builder.AppendFormat(" AND A.ExchangedState={0}",state);
            }
            if (!string.IsNullOrEmpty(goodsNameOrCode))
            {
                builder.AppendFormat(@" AND A.ApplyID IN(SELECT ApplyID FROM ShopExchangedApplyDetail 
                                     WHERE GoodsName Like '{0}%' OR GoodsCode Like '{0}%' GROUP BY ApplyID)",goodsNameOrCode);
            }
            builder.Append(" ORDER BY A.ApplyTime DESC ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ShopExchangedApplyInfo>(builder.ToString());
            }
        }

        /// <summary>
        /// 查询退货申请
        /// </summary>
        /// <param name="applyNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="goodsIds"></param>
        /// <param name="shopId"></param>
        /// <param name="state"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <returns></returns>
        public IEnumerable<ShopExchangedApplyInfo> GetShopRefundApplyList(string applyNo, DateTime startTime, DateTime endTime,
            IList<Guid> goodsIds, Guid shopId, int state, string goodsNameOrCode)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPEXCHANGEDAPPLY);
            builder.AppendFormat(" WHERE A.ApplyTime >= '{0}'", startTime);
            builder.AppendFormat(" AND A.ApplyTime < '{0}'", endTime);
            builder.Append(" AND A.IsBarter=1");
            if (!string.IsNullOrEmpty(applyNo))
            {
                builder.AppendFormat(" AND A.ApplyNo='{0}'",applyNo);
            }
            if (shopId != Guid.Empty)
            {
                builder.AppendFormat(" AND A.ShopID='{0}'", shopId);
            }
            if (state != -1)
            {
                builder.AppendFormat(" AND A.ExchangedState={0}",state);
            }
            if (!string.IsNullOrEmpty(goodsNameOrCode))
            {
                builder.AppendFormat(@" AND A.ApplyID IN(SELECT ApplyID FROM ShopExchangedApplyDetail 
                                     WHERE GoodsName Like '{0}%' OR GoodsCode Like '{0}%' GROUP BY ApplyID)", goodsNameOrCode);
            }
            builder.Append(" ORDER BY A.ApplyTime DESC ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ShopExchangedApplyInfo>(builder.ToString());
            }
        }

        /// <summary>
        /// 获取退换货单的状态
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="applyNo"> </param>
        /// <returns></returns>
        public int GetExchangeState(Guid applyId,string applyNo)
        {
            string sql = string.Format(
                applyId != Guid.Empty ? SQL_SELECT_EXCHANGEDSTATE : SQL_SELECT_EXCHANGEDSTATEBYNO,
                applyId != Guid.Empty ? applyId.ToString() : applyNo);

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(sql);
            }
        } 
    }
}

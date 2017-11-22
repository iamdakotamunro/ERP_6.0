using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using ERP.DAL.Interface.IShop;
using ERP.Environment;
using ERP.Model.ShopFront;
using Keede.DAL.RWSplitting;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Shop
{

    public class ShopRefundMessageDal : IShopRefundMessage
    {
        public ShopRefundMessageDal(GlobalConfig.DB.FromType fromType) { }

        #region  相关sql

        /// <summary>
        /// 添加退货留言
        /// </summary>
        private const string SQL_INSERT_SHOPREFUNDMESSAGE = @"INSERT INTO ShopRefundMessage
(MsgID,ShopID,ShopName,CreateTime,ApplyContent,ApplyState,[Description]) 
 VALUES(@MsgID,@ShopID,@ShopName,@CreateTime,@ApplyContent,@ApplyState,@Description)";

        /// <summary>
        /// 修改退货留言
        /// </summary>
        private const string SQL_UPDATE_SHOPREFUNDMESSAGE = @"UPDATE ShopRefundMessage 
SET ShopID=@ShopID,ShopName=@ShopName,CreateTime=@CreateTime,ApplyContent=@ApplyContent,ApplyState=@ApplyState,[Description]=@Description 
 WHERE MsgID=@MsgID";

        /// <summary>
        /// 修改退货留言状态和添加备注
        /// </summary>
        private const string SQL_UPDATE_SHOPREFUNDMESSAGE_STATE = @"UPDATE ShopRefundMessage 
SET ApplyState=@ApplyState,[Description]=[Description]+@Description WHERE MsgID=@MsgID ";

        /// <summary>
        /// 删除退货留言
        /// </summary>
        private const string SQL_DELETE_SHOPREFUNDMESSAGE = @"DELETE ShopRefundMessage WHERE MsgID=@MsgID ";

        /// <summary>
        /// 查询退货留言
        /// </summary>
        private const string SQL_SELECT_SHOPREFUNDMESSAGE = @"SELECT MsgID,ShopID,ShopName,CreateTime,ApplyContent,ApplyState,[Description] 
 FROM ShopRefundMessage ";

        /// <summary>
        /// 获取退货留言数量
        /// </summary>
        private const string SQL_SELECT_REFUNDMESSAGECOUNT = @"SELECT COUNT(*) FROM ShopRefundMessage ";

        /// <summary>
        /// 获取店铺最近的退货留言Id
        /// </summary>
        private const string SQL_SELECT_LASTEDMSGID = @"SELECT TOP 1 SR.MsgID FROM ShopRefundMessage SR 
LEFT JOIN ShopExchangedApply SE ON SR.MsgID=SE.MsgID
WHERE SR.ShopID=@ShopID AND SR.ApplyState=@ApplyState AND (SE.MsgID='00000000-0000-0000-0000-000000000000' OR SE.MsgID IS NULL)
ORDER BY SR.CreateTime DESC";
        #endregion

        #region  相关参数设置

        private const string PARM_MSGID = "@MsgID";
        private const string PARM_SHOPID = "@ShopID";
        private const string PARM_SHOPNAME = "@ShopName";
        private const string PARM_CREATETIME = "@CreateTime";
        private const string PARM_APPLYCONTENT = "@ApplyContent";
        private const string PARM_APPLYSTATE = "@ApplyState";
        private const string PARM_DESCRIPTION = "@Description";
        #endregion

        /// <summary>
        /// 插入退货留言
        /// </summary>
        /// <param name="messageInfo"></param>
        public void InsertShopRefundMessage(ShopRefundMessageInfo messageInfo)
        {
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    conn.Execute(SQL_INSERT_SHOPREFUNDMESSAGE, new
                    {
                        MsgID = messageInfo.MsgID,
                        ShopID = messageInfo.ShopID,
                        ShopName = messageInfo.ShopName,
                        CreateTime = messageInfo.CreateTime,
                        ApplyContent = messageInfo.ApplyContent,
                        ApplyState = messageInfo.ApplyState,
                        Description = string.IsNullOrEmpty(messageInfo.Description) ? string.Empty : messageInfo.Description,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退货留言插入失败!", ex);
            }
        }

        /// <summary>
        /// 修改退货留言
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <returns></returns>
        public int UpdateShopRefundMessage(ShopRefundMessageInfo messageInfo)
        {
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(SQL_UPDATE_SHOPREFUNDMESSAGE, new
                    {
                        MsgID = messageInfo.MsgID,
                        ShopID = messageInfo.ShopID,
                        ShopName = messageInfo.ShopName,
                        CreateTime = messageInfo.CreateTime,
                        ApplyContent = messageInfo.ApplyContent,
                        ApplyState = messageInfo.ApplyState,
                        Description = string.IsNullOrEmpty(messageInfo.Description) ? string.Empty : messageInfo.Description,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退货留言修改失败!", ex);
            }
        }

        /// <summary>
        /// 删除退货留言
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public int DeleteShopRefundMessage(Guid msgId)
        {
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(SQL_DELETE_SHOPREFUNDMESSAGE, new
                    {
                        MsgID = msgId,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退货留言删除失败!", ex);
            }
        }

        /// <summary>
        /// 获取特定状态下的退货留言数量
        /// </summary>
        /// <param name="shopId">店铺Id</param>
        /// <param name="state"> </param>
        /// <returns></returns>
        public int GetMessageCount(Guid shopId, int state)
        {
            var builder = new StringBuilder(SQL_SELECT_REFUNDMESSAGECOUNT);
            builder.Append(" WHERE ApplyState=").Append(state);
            if (shopId != Guid.Empty)
            {
                if (shopId != Guid.Empty)
                {
                    builder.Append(" AND ShopID='").Append(shopId).Append("'");
                }
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(builder.ToString());
            }
        }

        /// <summary>
        /// 修改退货留言状态、添加备注信息
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="state"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public int SetMessageState(Guid msgId, int state, string description)
        {
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(SQL_UPDATE_SHOPREFUNDMESSAGE_STATE, new
                    {
                        ApplyState = state,
                        Description = description,
                        MsgID = msgId,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退货留言修改失败!", ex);
            }
        }

        /// <summary>
        /// 获取指定退货留言
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShopRefundMessageInfo GetShopRefundMessageInfo(Guid id)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPREFUNDMESSAGE);
            builder.Append(" WHERE MsgID=@MsgID");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<ShopRefundMessageInfo>(builder.ToString(), new
                {
                    MsgID = id,
                });
            }
        }

        /// <summary>
        /// 获取最近的退货留言Id
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="state"> </param>
        /// <returns></returns>
        public Guid GetLastedRefundMsgId(Guid shopId, int state)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<Guid>(SQL_SELECT_LASTEDMSGID, new
                {
                    ShopID = shopId,
                    ApplyState = state,
                });
            }
        }

        /// <summary>
        /// 根据店铺、时间、留言状态查询退货留言
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="state">审核状态</param>
        /// /// <param name="shopId">店铺Id</param>
        /// <param name="ascOrDesc">升序还是降序 false 升序，true 降序</param>
        public IEnumerable<ShopRefundMessageInfo> GetShopRefundMesageList(DateTime startTime, DateTime
            endTime, int state, Guid shopId, bool ascOrDesc)
        {
            var builder = new StringBuilder(SQL_SELECT_SHOPREFUNDMESSAGE);
            builder.AppendFormat(" WHERE CreateTime BETWEEN '{0}' AND '{1}'", startTime, endTime);
            if (state != -1)
            {
                builder.AppendFormat(" AND ApplyState={0}", state);
            }
            if (shopId != Guid.Empty)
            {
                builder.AppendFormat(" AND ShopID = '{0}'", shopId);
            }
            builder.Append(" ORDER BY CreateTime ").Append(ascOrDesc ? " DESC " : " ASC ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ShopRefundMessageInfo>(builder.ToString()).AsList();
            }
        }
    }
}

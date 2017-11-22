using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    public class PaymentNotice : IPaymentNotice
    {
        public PaymentNotice(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        #region [SQL]语句
        //插入语句
        //        private const string SQL_PN_INSERT = @"Insert into lmShop_PaymentNotice ([PayId],[OrderId],[OrderNo],[PayBank],[PayPrince],
        //[PayTime],[PayName],[PayDes],[PayState]) 
        //values(@PayId,@OrderId,@OrderNo,@PayBank,@PayPrince,@PayTime,@PayName,@PayDes,@PayState) ";
        //修改状态
        private const string SQL_PN_UPDATE_STATE = @"Update lmShop_PaymentNotice Set PayState=@PayState ,PayDes=PayDes+@PayDes Where PayId=@PayId";

        //删除语句
        private const string SQL_PN_DELETE = @"delete from lmShop_PaymentNotice Where PayId=@PayId";

        //插入语句
        private const string SQL_PN_INSERT =
            @"
IF NOT EXISTS(SELECT TOP 1 PayId FROM lmShop_PaymentNotice WHERE PayId=@PayId)
BEGIN
Insert into lmShop_PaymentNotice ([PayId],[OrderId],[OrderNo],[PayBank],[PayPrince],
[PayTime],[PayName],[PayDes],[PayState],SaleFilialeId,SalePlatformId) 
values(@PayId,@OrderId,@OrderNo,@PayBank,@PayPrince,@PayTime,@PayName,@PayDes,@PayState,@SaleFilialeId,@SalePlatformId) 
END
";

        //修改语句
        private const string SQL_UPDATE_INFO_BY_PAYID = @"UPDATE [lmShop_PaymentNotice]
                                               SET [PayBank] = @PayBank
                                                  ,[PayPrince] = @PayPrince     
                                                  ,[PayName] = @PayName 
                                                  ,[PayTime] = @PayTime                                          
                                                    WHERE [PayId] = @PayId ";

        //查询语句
        private const string SQL_PN_ISEXIST_BY_ORDERNO_PAYSTATE = "select 0 from lmShop_PaymentNotice where OrderNo=@OrderNo and PayState=3";
        private const string SQL_SELECT_PN_BY_ORDERNO = @"SELECT [PayId],[OrderId],[OrderNo],[PayBank],[PayPrince] ,[PayTime],[PayName],[PayDes],[PayState],SaleFilialeId,SalePlatformId FROM lmShop_PaymentNotice where OrderNo=@OrderNo and PayState=3";

        //删除语句
        private const string SQL_DELETE_PN_BY_PAYID = "delete from lmShop_PaymentNotice where PayId=@PayId and PayState=3";
        #endregion

        #region [SQL]参数
        private const string PAYID = "@PayId";
        private const string ORDERID = "@OrderId";
        private const string ORDERNO = "@OrderNo";
        private const string PAYBANK = "@PayBank";
        private const string PAYPRINCE = "@PayPrince";
        private const string PAYNAME = "@PayName";
        private const string PAYTIME = "@PayTime";
        private const string PAYDES = "@PayDes";
        private const string PAYSTATE = "@PayState";
        private const string SALE_FILIALE_ID = "@SaleFilialeId";
        private const string SALE_PLATFORM_ID = "@SalePlatformId";

        private SqlParameter[] GetInsertParms(PaymentNoticeInfo pnInfo)
        {
            var parms = new[] {
                new SqlParameter(PAYID, SqlDbType.UniqueIdentifier),
                new SqlParameter(ORDERID, SqlDbType.UniqueIdentifier),
                new SqlParameter(ORDERNO, SqlDbType.VarChar),
                new SqlParameter(PAYBANK, SqlDbType.VarChar),
                new SqlParameter(PAYPRINCE, SqlDbType.Decimal),
                new SqlParameter(PAYTIME, SqlDbType.DateTime),
                new SqlParameter(PAYDES, SqlDbType.VarChar),
                new SqlParameter(PAYSTATE, SqlDbType.Int),
                new SqlParameter(PAYNAME,SqlDbType.VarChar), 
                new SqlParameter(SALE_FILIALE_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(SALE_PLATFORM_ID,SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = pnInfo.PayId;
            parms[1].Value = pnInfo.OrderId;
            parms[2].Value = pnInfo.OrderNo;
            parms[3].Value = pnInfo.PayBank;
            parms[4].Value = pnInfo.PayPrince;
            parms[5].Value = pnInfo.PayTime;
            parms[6].Value = string.IsNullOrEmpty(pnInfo.PayDes) ? "" : pnInfo.PayDes;
            parms[7].Value = pnInfo.PayState;
            parms[8].Value = pnInfo.PayName;
            parms[9].Value = pnInfo.SaleFilialeId;
            parms[10].Value = pnInfo.SalePlatformId;
            return parms;
        }
        #endregion

        #region IPaymentNotice 成员
        public void UpdatePayNoticState(Guid payid, PayState yon, string des)
        {
            var parms = new[] {
                           new SqlParameter(PAYID, SqlDbType.UniqueIdentifier),
                           new SqlParameter(PAYSTATE, SqlDbType.Int),
                           new SqlParameter(PAYDES, SqlDbType.VarChar)
            };
            parms[0].Value = payid;
            parms[1].Value = (int)yon;
            parms[2].Value = des;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_PN_UPDATE_STATE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IList<PaymentNoticeInfo> GetPayNoticListByPayState(int pystate, string keyword)
        {
            IList<PaymentNoticeInfo> pnlist = new List<PaymentNoticeInfo>();
            //根据状态查询
            string sqlPnSelectBystate = @" Select [PayId],[OrderId],[OrderNo],[PayBank],[PayPrince],
[PayTime],[PayName],[PayDes],[PayState],SaleFilialeId,SalePlatformId From lmShop_PaymentNotice Where PayState=@PayState ";
            var parms = new[]
                                       {
                                           new SqlParameter("@KeyWord", SqlDbType.VarChar),
                                           new SqlParameter(PAYSTATE, SqlDbType.Int)
                                       };
            parms[0].Value = keyword;
            parms[1].Value = pystate;
            if (!string.IsNullOrEmpty(keyword))
                sqlPnSelectBystate += " And (OrderNo=@KeyWord or PayName=@KeyWord) ";
            sqlPnSelectBystate += " Order By  PayTime ";
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sqlPnSelectBystate, parms))
            {
                while (rdr.Read())
                {
                    var pninfo = new PaymentNoticeInfo
                        {
                            PayId = rdr.GetGuid(0),
                            OrderId = rdr.GetGuid(1),
                            OrderNo = rdr.GetString(2),
                            PayBank = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                            PayPrince = rdr.GetDecimal(4),
                            PayTime = rdr.IsDBNull(5) ? DateTime.MinValue : rdr.GetDateTime(5),
                            PayName = rdr.IsDBNull(6) ? "" : rdr.GetString(6),
                            PayDes = rdr.IsDBNull(7) ? "" : rdr.GetString(7),
                            PayState = rdr.IsDBNull(8) ? 0 : rdr.GetInt32(8),
                            SaleFilialeId = rdr.IsDBNull(9) ? Guid.Empty : rdr.GetGuid(9),
                            SalePlatformId = rdr.IsDBNull(10) ? Guid.Empty : rdr.GetGuid(10)
                        };
                    pnlist.Add(pninfo);
                }
            }
            return pnlist;
        }

        public PaymentNoticeInfo GetPayNoticInfoByPayid(Guid payid)
        {
            PaymentNoticeInfo pninfo = null;
            const string SQL_PN_SELECT_BY_PAYID = @"Select [PayId],[OrderId],[OrderNo],[PayBank],[PayPrince],[PayTime],[PayName],[PayDes],[PayState],SaleFilialeId,SalePlatformId From lmShop_PaymentNotice Where PayId=@PayId ";
            var parm = new SqlParameter(PAYID, SqlDbType.UniqueIdentifier) { Value = payid };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_PN_SELECT_BY_PAYID, parm))
            {
                if (rdr.Read())
                {
                    pninfo = new PaymentNoticeInfo
                             {
                                 PayId = SqlRead.GetGuid(rdr, "PayId"),
                                 OrderId = SqlRead.GetGuid(rdr, "OrderId"),
                                 OrderNo = SqlRead.GetString(rdr, "OrderNo"),
                                 PayBank = SqlRead.GetString(rdr, "PayBank"),
                                 PayPrince = SqlRead.GetDecimal(rdr, "PayPrince"),
                                 PayTime = SqlRead.GetDateTime(rdr, "PayTime"),
                                 PayName = SqlRead.GetString(rdr, "PayName"),
                                 PayDes = SqlRead.GetString(rdr, "PayDes"),
                                 PayState = SqlRead.GetInt(rdr, "PayState"),
                                 SaleFilialeId = SqlRead.GetGuid(rdr, "SaleFilialeId"),
                                 SalePlatformId = SqlRead.GetGuid(rdr, "SalePlatformId")
                             };
                }
            }
            return pninfo;
        }

        #endregion

        #region Delete
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="payid"></param>
        public void Delete(Guid payid)
        {
            var parms = new[] {
                           new SqlParameter(PAYID, SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = payid;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_PN_DELETE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        public void Insert(PaymentNoticeInfo pnInfo)
        {
            SqlParameter[] parms = GetInsertParms(pnInfo);
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_PN_INSERT, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 是否存在这个订到款通知(待确认)
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public bool IsExistByOrderNoAndPaystate(string orderNo)
        {
            var parm = new SqlParameter(ORDERNO,orderNo);
            bool isExist = false;
            try
            {
                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_PN_ISEXIST_BY_ORDERNO_PAYSTATE, parm))
                {
                    if (rdr.Read())
                    {
                        isExist = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return isExist;
        }

        /// <summary>
        /// 根据订单号获取支付信息记录
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        public IList<PaymentNoticeInfo> GetListByOrderNo(string orderNo)
        {
            var parm = new SqlParameter(ORDERNO, orderNo);
            IList<PaymentNoticeInfo> pnlist = new List<PaymentNoticeInfo>();
            try
            {
                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_PN_BY_ORDERNO, parm))
                {
                    if (rdr.Read())
                    {
                        var info = new PaymentNoticeInfo
                        {
                            OrderId = new Guid(rdr["PayId"].ToString()),
                            PayId = new Guid(rdr["PayId"].ToString()),
                            OrderNo = Convert.ToString(rdr["OrderNo"]),
                            PayBank = Convert.ToString(rdr["PayBank"]),
                            PayPrince = Convert.ToDecimal(rdr["PayPrince"]),
                            PayTime = Convert.ToDateTime(rdr["PayTime"]),
                            PayName = Convert.ToString(rdr["PayName"]),
                            PayDes = Convert.ToString(rdr["PayDes"]),
                            PayState = Convert.ToInt32(rdr["PayState"]),
                            SaleFilialeId = rdr["SaleFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SaleFilialeId"].ToString()),
                            SalePlatformId = rdr["SalePlatformId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SalePlatformId"].ToString())
                        };
                        pnlist.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return pnlist;
        }

        /// <summary>
        /// 修改到款通知信息
        /// </summary>
        /// <param name="pnInfo"></param>
        public void UpdatePayNoticeInfo(PaymentNoticeInfo pnInfo)
        {
            var parms = new[] {
                 new SqlParameter(PAYID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PAYBANK, SqlDbType.VarChar,100),
                new SqlParameter(PAYPRINCE, SqlDbType.Decimal),
                new SqlParameter(PAYNAME,SqlDbType.VarChar,100), 
                new SqlParameter(PAYTIME,SqlDbType.DateTime)
            };
            parms[0].Value = pnInfo.PayId;
            parms[1].Value = pnInfo.PayBank;
            parms[2].Value = pnInfo.PayPrince;
            parms[3].Value = pnInfo.PayName;
            parms[4].Value = pnInfo.PayTime;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_INFO_BY_PAYID, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 根据PAYID删除待付款确认信息
        /// </summary>
        public void DeletePayNoticeByPayId(Guid payid)
        {
            var parms = new[] {
                 new SqlParameter(PAYID, SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = payid;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_PN_BY_PAYID, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}

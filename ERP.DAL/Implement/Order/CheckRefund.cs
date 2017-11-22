using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using ERP.DAL.Interface.IOrder;
using ERP.Environment;
using Keede.Ecsoft.Model;
using Dapper;
using Keede.DAL.RWSplitting;
using System.Transactions;

namespace ERP.DAL.Implement.Order
{
    public class CheckRefund : ICheckRefund
    {
        public CheckRefund(GlobalConfig.DB.FromType fromType) { }

        #region ICheckRefund 成员

        /// <summary>
        /// 添加退换货商品明细
        /// </summary>
        private const string SQL_INSERT_CHECKREFUNDDETAIL = @"delete lmShop_CheckRefundDetails where RefundId=@RefundId; 
INSERT INTO lmShop_CheckRefundDetails(Id, RefundId, GoodsId, RealGoodsId, GoodsCode, GoodsName,Specification, Quantity, SellPrice, ReturnCount, ReturnType, ReturnReason)
  VALUES(@Id, @RefundId, @GoodsId, @RealGoodsId, @GoodsCode, @GoodsName,@Specification, @Quantity, @SellPrice, @ReturnCount, @ReturnType, @ReturnReason)";

        /// <summary>
        /// 更新退换货检查记录状态，检查不通过原因和退货仓库(总后台)，重启原因(分后台编辑)
        /// </summary>
        private const string SQL_UPDATE_CHECKREFUND = @"UPDATE lmShop_CheckRefund SET CheckState=@CheckState,Remark=@Remark,WarehouseId=@WarehouseId,ReStartReason=@ReStartReason,CheckFilialeId=@CheckFilialeId WHERE RefundId=@RefundId";

        /// <summary>
        /// 删除退换货检查记录
        /// </summary>
        private const string SQL_DELETE_CHECKREFUND = @"DELETE lmShop_CheckRefundDetails WHERE RefundId=@RefundId;
 DELETE lmShop_CheckRefund WHERE RefundId=@RefundId;";

        /// <summary>
        /// 根据退换货编号获取检查明细(总后台明细，分后台退货原因，状态，重启原因)
        /// </summary>
        private const string SQL_SELECT_CHECKREFUND = @"
SELECT 
    RefundId,RefundNo,OrderId,OrderNo,Consignee,ExpressNo,ExpressName,CreateDate,CheckState,Remark,WarehouseId,SaleFilialeId,SalePlatformId,Amount,ReStartReason,CheckFilialeId,IsTransfer 
FROM lmShop_CheckRefund
WHERE 1=1";

        /// <summary>
        /// 根据退换货编号，获取退换货商品明细
        /// </summary>
        private const string SQL_SELECT_CHECKREFUNDDETAILLIST = @"SELECT Id, RefundId, GoodsId, RealGoodsId, GoodsCode, GoodsName,Specification, Quantity, SellPrice, ReturnCount, ReturnType, ReturnReason, DamageCount FROM lmShop_CheckRefundDetails WHERE ReturnType<>3 AND RefundId=@RefundId";


        private const string SQL_INSERT_CHECKREFUND_SERVER = @"
IF NOT EXISTS(SELECT RefundId FROM lmShop_CheckRefund WHERE RefundId=@RefundId)
BEGIN
	INSERT INTO lmShop_CheckRefund(RefundId,RefundNo,OrderId,ExpressNo,ExpressName,CreateDate,CheckState,Remark,WarehouseId,ReStartReason,OrderNo,Consignee,SaleFilialeId,SalePlatformId)
	VALUES(@RefundId,@RefundNo,@OrderId,@ExpressNo,@ExpressName,@CreateDate,@CheckState,@Remark,@WarehouseId,@ReStartReason,@OrderNo,@Consignee,@SaleFilialeId,@SalePlatformId)
END 
ELSE
BEGIN
	UPDATE lmShop_CheckRefund SET ExpressNo=@ExpressNo,ExpressName=@ExpressName,CreateDate=@CreateDate,CheckState=@CheckState,Remark=@Remark,ReStartReason=@ReStartReason,OrderNo=@OrderNo,Consignee=@Consignee,SaleFilialeId=@SaleFilialeId,SalePlatformId=@SalePlatformId 
	WHERE RefundId=@RefundId
END
delete lmShop_CheckRefundDetails where RefundId=@RefundId;";

        /// <summary>
        /// 添加退换货商品明细
        /// </summary>
        private const string SQL_INSERT_CHECKREFUNDDETAIL_SERVER = @" 
INSERT INTO lmShop_CheckRefundDetails(Id, RefundId, GoodsId, RealGoodsId, GoodsCode, GoodsName,Specification, Quantity, SellPrice, ReturnCount, ReturnType, ReturnReason)
  VALUES(@Id, @RefundId, @GoodsId, @RealGoodsId, @GoodsCode, @GoodsName,@Specification, @Quantity, @SellPrice, @ReturnCount, @ReturnType, @ReturnReason)";
        
        /// <summary>
        /// 添加退换货检查记录
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public bool InsertCheckRefund(CheckRefundInfo refundInfo)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_INSERT_CHECKREFUND_SERVER, new
                {
                    RefundId = refundInfo.RefundId,
                    RefundNo = refundInfo.RefundNo,
                    OrderId = refundInfo.OrderId,
                    ExpressNo = refundInfo.ExpressNo,
                    ExpressName = refundInfo.ExpressName,
                    CreateDate = refundInfo.CreateTime,
                    CheckState = refundInfo.CheckState,
                    Remark = refundInfo.Remark,
                    WarehouseId = refundInfo.WarehouseId,
                    ReStartReason = refundInfo.ReStartReason,
                }) > 0;
            }
        }

        /// <summary>
        /// 总后台修改退换货检查记录检查状态，不通过原因，退货仓库,重启原因(分后台编辑)
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public bool UpdateCheckRefund(CheckRefundInfo refundInfo)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_UPDATE_CHECKREFUND, new
                {
                    RefundId = refundInfo.RefundId,
                    CheckState = refundInfo.CheckState,
                    Remark = refundInfo.Remark,
                    WarehouseId = refundInfo.WarehouseId,
                    ReStartReason = refundInfo.ReStartReason,
                    CheckFilialeId = refundInfo.CheckFilialeId,
                }) > 0;
            }
        }

        /// <summary>
        /// 先删除退换货检查商品明细,删除退换货检查记录，
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        public bool DeleteCheckRefund(Guid refundId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_DELETE_CHECKREFUND, new
                {
                    RefundId = refundId,
                }) > 0;
            }
        }

        /// <summary>
        /// 添加退换货检查商品明细
        /// </summary>
        /// <param name="detailInfo"></param>
        /// <returns></returns>
        public bool InsertCheckDetails(CheckRefundDetailInfo detailInfo)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_INSERT_CHECKREFUNDDETAIL, new
                {
                    Id = detailInfo.Id,
                    RefundId = detailInfo.RefundId,
                    GoodsId = detailInfo.GoodsId,
                    RealGoodsId = detailInfo.RealGoodsId,
                    GoodsCode = detailInfo.GoodsCode,
                    GoodsName = detailInfo.GoodsName,
                    Specification = detailInfo.Specification,
                    Quantity = detailInfo.Quantity,
                    SellPrice = detailInfo.SellPrice,
                    ReturnCount = detailInfo.ReturnCount,
                    ReturnType = detailInfo.ReturnType,
                    ReturnReason = detailInfo.ReturnReason,
                }) > 0;
            }
        }

        /// <summary>
        /// 根据条件获取退换货检查记录
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="checkState">检查状态</param>
        /// <param name="checkFilialeId">检查公司</param>
        /// <returns></returns>
        public IList<CheckRefundInfo> GetCheckRefundInfo(string keywords, DateTime startTime, DateTime endTime, int checkState, Guid checkFilialeId)
        {
            var sql = new StringBuilder(SQL_SELECT_CHECKREFUND);
            sql.Append(" AND RefundId IN (SELECT RefundId FROM lmShop_CheckRefundDetails WHERE ReturnType IN (0,1,2,4))");
            if (checkFilialeId != Guid.Empty)
            {
                sql.Append(" AND CheckFilialeId=@CheckFilialeId ");
            }
            if (!string.IsNullOrEmpty(keywords))
            {
                sql.Append(" AND (RefundNo LIKE '%'+@Keywords+'%' OR OrderNo LIKE '%'+@Keywords+'%' OR Consignee LIKE '%'+@Keywords+'%' OR ExpressNo LIKE '%'+@Keywords+'%')");
            }
            if (checkState != -1)
            {
                sql.Append(" AND CheckState=@CheckState ");
            }
            sql.Append(" AND CreateDate >= '" + startTime + "' AND CreateDate < '" + endTime + "'");
            sql.Append(" ORDER BY CreateDate DESC ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CheckRefundInfo>(sql.ToString(), new
                {
                    Keywords = keywords,
                    CheckFilialeId = checkFilialeId,
                    CheckState = checkState,
                }).AsList();
            }
        }

        /// <summary>
        /// 根据退换货号获取退换货商品详细
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        public IList<CheckRefundDetailInfo> GetCheckRefundDetails(Guid refundId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CheckRefundDetailInfo>(SQL_SELECT_CHECKREFUNDDETAILLIST, new
                {
                    RefundId = refundId,
                }).AsList();
            }
        }

        /// <summary>
        /// 更改退回商品检查中损坏数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="damageCount">损坏数量</param>
        /// <param name="realGoodsId"></param>
        /// <param name="specification"></param>
        public bool UpdateCheckRefundDetails(Guid id, int damageCount, Guid realGoodsId, string specification)
        {
            const string SQL = "UPDATE [lmShop_CheckRefundDetails] SET [RealGoodsId]=@RealGoodsId,[Specification]=@Specification,[DamageCount]=@DamageCount WHERE [Id]=@Id";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    RealGoodsId = realGoodsId,
                    Specification = specification,
                    DamageCount = damageCount,
                    Id = id
                }) > 0;
            }
        }

        #endregion


        /// <summary>
        /// 添加退换货信息以及清单信息
        /// </summary>
        /// <param name="refundInfo">退换货信息</param>
        /// <param name="refundDetailList">清单信息</param>
        /// <returns></returns>
        public bool InsertCheckRefundAndDetailList(CheckRefundInfo refundInfo, IList<CheckRefundDetailInfo> refundDetailList)
        {
            bool flag;
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        flag = conn.Execute(SQL_INSERT_CHECKREFUND_SERVER, new
                        {
                            RefundId = refundInfo.RefundId,
                            RefundNo = refundInfo.RefundNo,
                            OrderId = refundInfo.OrderId,
                            ExpressNo = refundInfo.ExpressNo,
                            ExpressName = refundInfo.ExpressName,
                            CreateDate = refundInfo.CreateTime,
                            CheckState = refundInfo.CheckState,
                            Remark = refundInfo.Remark,
                            WarehouseId = refundInfo.WarehouseId,
                            ReStartReason = refundInfo.ReStartReason,
                            OrderNo = refundInfo.OrderNo,
                            Consignee = refundInfo.Consignee,
                            SaleFilialeId = refundInfo.SaleFilialeId,
                            SalePlatformId = refundInfo.SalePlatformId,
                        }, trans) > 0;

                        if (flag)
                        {
                            foreach (var detailInfo in refundDetailList)
                            {
                                if (!flag) continue;
                                flag = conn.Execute(SQL_INSERT_CHECKREFUNDDETAIL_SERVER, new
                                {
                                    Id = detailInfo.Id,
                                    RefundId = detailInfo.RefundId,
                                    GoodsId = detailInfo.GoodsId,
                                    RealGoodsId = detailInfo.RealGoodsId,
                                    GoodsCode = detailInfo.GoodsCode ?? string.Empty,
                                    GoodsName = detailInfo.GoodsName ?? string.Empty,
                                    Specification = detailInfo.Specification ?? string.Empty,
                                    Quantity = detailInfo.Quantity,
                                    SellPrice = detailInfo.SellPrice,
                                    ReturnCount = detailInfo.ReturnCount,
                                    ReturnType = detailInfo.ReturnType,
                                    ReturnReason = detailInfo.ReturnReason ?? string.Empty,
                                }, trans) > 0;
                            }
                            if (flag)
                                trans.Commit();
                            else
                                trans.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("添加退换货商品检查失败!", ex);
            }
            return flag;
        }


        /// <summary>
        /// 修改退换货检查信息
        /// </summary>
        /// <param name="refundInfo">退换货检查信息</param>
        /// <returns></returns>
        public bool UpdateCheckRefund_Server(CheckRefundInfo refundInfo)
        {
            const string SQL = @"UPDATE lmShop_CheckRefund SET CheckState=@CheckState,ReStartReason=@ReStartReason WHERE RefundId=@RefundId";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    CheckState = refundInfo.CheckState,
                    ReStartReason = refundInfo.ReStartReason ?? string.Empty,
                    RefundId = refundInfo.RefundId
                }) > 0;
            }
        }

        /// <summary>
        /// 获取退换货检查信息
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        public CheckRefundInfo GetCheckRefundInfo(Guid refundId)
        {
            var checkRefund = new CheckRefundInfo();
            try
            {
                var strbSql = new StringBuilder(SQL_SELECT_CHECKREFUND);
                strbSql.Append(string.Format(" AND RefundId='{0}'", refundId));
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.QueryFirstOrDefault<CheckRefundInfo>(strbSql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("获取退换货检查信息失败!", ex);
            }
        }

        /// <summary>
        /// 删除退换货检查信息
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        public bool DeleteCheckRefundInfo(Guid refundId)
        {
            const string SQL = @"DELETE lmShop_CheckRefundDetails WHERE RefundId=@RefundId;
 DELETE lmShop_CheckRefund WHERE RefundId=@RefundId;";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new { RefundId = refundId }) > 0;
            }
        }

        /// <summary>
        /// 供前台使用修改退换检查物流信息
        /// </summary>
        /// <param name="checkRefund"></param>
        /// <returns></returns>
        public int ModifyCheckRefundExpress(CheckRefundInfo checkRefund)
        {
            const string SQL = @"UPDATE lmShop_CheckRefund SET ExpressNo=@ExpressNo,ExpressName=@ExpressName WHERE RefundId=@RefundId ";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    ExpressNo = checkRefund.ExpressNo ?? string.Empty,
                    ExpressName = checkRefund.ExpressName ?? string.Empty,
                    RefundId = checkRefund.RefundId
                });
            }
        }

        /// <summary>
        /// 获取退回商品检查信息
        /// </summary>
        /// <returns></returns>
        public IList<CheckRefundInfo> GetCheckRefundList()
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CheckRefundInfo>(SQL_SELECT_CHECKREFUND).AsList();
            }
        }

        /// <summary>
        /// 更改退回商品检查
        /// </summary>
        /// <param name="refundId"></param>
        /// <param name="checkState"></param>
        /// <param name="remark"></param>
        /// <param name="checkFilialeId">检查公司</param>
        public int UpdateCheckRefund(Guid refundId, int checkState, string remark, Guid checkFilialeId)
        {
            const string SQL = @"UPDATE lmShop_CheckRefund SET CheckState=@CheckState,Remark=@Remark,CheckFilialeId=@CheckFilialeId WHERE RefundId=@RefundId";
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.Execute(SQL, new
                    {
                        CheckState = checkState,
                        Remark = remark,
                        CheckFilialeId = checkFilialeId,
                        RefundId = refundId
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("更改退回商品检查状态、备注失败!", ex);
            }
        }

        /// <summary>
        /// 获取退回商品检查明细
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        public IList<CheckRefundDetailInfo> GetCheckRefundDetailList(Guid refundId)
        {
            const string SQL = @"SELECT Id, RefundId, GoodsId, RealGoodsId, GoodsCode, GoodsName,Specification, Quantity, 
SellPrice, ReturnCount, ReturnType, ReturnReason,DamageCount 
FROM lmShop_CheckRefundDetails WHERE ReturnType<>3 AND RefundId=@RefundId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CheckRefundDetailInfo>(SQL, new
                {
                    RefundId = refundId
                }).AsList();
            }
        }

        /// <summary>
        /// 退回商品检查是否移交
        /// </summary>
        /// <param name="refundId"></param>
        /// <param name="isTransfer">是否移交</param>
        /// <returns></returns>
        public bool UpdateCheckRefundIsTransfer(Guid refundId, bool isTransfer)
        {
            const string SQL = @"UPDATE lmShop_CheckRefund SET IsTransfer=@IsTransfer WHERE RefundId=@RefundId";
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    conn.Execute(SQL, new
                    {
                        IsTransfer = isTransfer,
                        RefundId = refundId
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("退回商品检查是否移交失败!", ex);
            }
        }

        /// <summary>
        /// 商品检查状态更改为检查未通过(售后单作废)
        /// </summary>
        /// <param name="refundId"></param>
        /// <param name="remark"></param>
        public bool UpdateCheckRefundRefuse(Guid refundId, string remark)
        {
            const string SQL = @"UPDATE lmShop_CheckRefund SET CheckState=2,Remark=@Remark WHERE RefundId=@RefundId";
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    conn.Execute(SQL, new
                    {
                        Remark = remark,
                        RefundId = refundId
                    });
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region 需求：1599 增加快递公司快递号 输入操作   add by liangcanren at 2015-04-17
        /// <summary>
        /// 插入退回检查快递单号
        /// </summary>
        /// <param name="refundId"></param>
        /// <param name="expressNo"></param>
        /// <returns></returns>
        public bool UpdateCheckRefundExpressNo(Guid refundId, string expressNo)
        {
            const string SQL = @"UPDATE lmShop_CheckRefund SET ExpressNo=@ExpressNo WHERE RefundId=@RefundId";
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    conn.Execute(SQL, new
                    {
                        ExpressNo = expressNo,
                        RefundId = refundId
                    });
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 获取联盟店退换货检查信息
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        public CheckRefundInfo GetShopCheckRefundInfo(Guid refundId)
        {
            const string SQL = @" SELECT TOP 1 RefundId,RefundNo, CR.OrderId,S.ApplyNo as OrderNo,S.ShopName as Consignee, CR.ExpressNo, 
 ExpressName, CreateDate as CreateTime, CheckState,Remark, CR.WarehouseId,CR.Amount,CR.ReStartReason,CheckFilialeId,IsTransfer,SaleFilialeId 
 FROM lmShop_CheckRefund CR INNER JOIN ShopExchangedApply S ON CR.OrderId=S.ApplyID WHERE RefundId=@RefundId";
            try
            {
                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
                {
                    return conn.QueryFirstOrDefault<CheckRefundInfo>(SQL, new
                    {
                        RefundId = refundId
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("获取联盟店退换货检查信息失败!", ex);
            }
        }

        /// <summary>
        /// 根据条件获取联盟店退换货检查记录
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="checkState">检查状态</param>
        /// <param name="checkFilialeIds">检查公司</param>
        /// <returns></returns>
        public IList<CheckRefundInfo> GetShopCheckRefundList(string keywords, DateTime startTime, DateTime endTime, int checkState, List<Guid> checkFilialeIds)
        {
            var builder = new StringBuilder(@"SELECT CR.RefundId,CR.RefundNo, CR.OrderId,S.ApplyNo as OrderNo,S.ShopName as Consignee, CR.ExpressNo, CR.ExpressName, 
CR.CreateDate, CR.CheckState,CR.Remark, CR.WarehouseId,
CR.Amount,CR.ReStartReason,CR.CheckFilialeId,CR.IsTransfer,CR.SaleFilialeId FROM lmShop_CheckRefund CR INNER JOIN ShopExchangedApply S ON CR.OrderId=S.ApplyID
WHERE CR.CreateDate BETWEEN '{0}' AND '{1}' ");
            if (checkFilialeIds != null && checkFilialeIds.Count != 0)
            {
                builder.Append(" AND S.ShopID IN( ");
                for (int i = 0; i < checkFilialeIds.Count; i++)
                {
                    builder.AppendFormat("'{0}'", checkFilialeIds[i]);
                    if (checkFilialeIds.Count - 1 != i)
                    {
                        builder.Append(",");
                    }
                    builder.Append(")");
                }
            }
            if (!string.IsNullOrEmpty(keywords))
            {
                builder.AppendFormat(" AND CR.RefundNo LIKE '%{0}%' ", keywords);
            }
            if (checkState != -1)
            {
                builder.Append(" AND CR.CheckState= ").Append(checkState);
            }
            //AND CR.RefundId IN( SELECT RefundId FROM lmShop_CheckRefundDetails WHERE ReturnType in(0,1,2,4)) 
            builder.Append(@" ORDER BY CR.CreateDate DESC");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CheckRefundInfo>(string.Format(builder.ToString(), startTime, endTime)).AsList();
            }
        }

        private CheckRefundInfo ShopReaderCheckRefund(SqlDataReader rdr)
        {
            var checkRefundInfo = new CheckRefundInfo
            {
                RefundId = rdr["RefundId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["RefundId"].ToString()),
                RefundNo = rdr["RefundNo"] == DBNull.Value ? string.Empty : rdr["RefundNo"].ToString(),
                OrderId = rdr["OrderId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["OrderId"].ToString()),
                OrderNo = rdr["ApplyNo"] == DBNull.Value ? string.Empty : rdr["ApplyNo"].ToString(),
                Consignee = rdr["ShopName"] == DBNull.Value ? string.Empty : rdr["ShopName"].ToString(),
                ExpressNo = rdr["ExpressNo"] == DBNull.Value ? string.Empty : rdr["ExpressNo"].ToString(),
                ExpressName = rdr["ExpressName"] == DBNull.Value ? string.Empty : rdr["ExpressName"].ToString(),
                CreateTime = rdr["CreateDate"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(rdr["CreateDate"].ToString()),
                CheckState = rdr["CheckState"] == DBNull.Value ? 0 : int.Parse(rdr["CheckState"].ToString()),
                Remark = rdr["Remark"] == DBNull.Value ? string.Empty : rdr["Remark"].ToString(),
                WarehouseId = rdr["WarehouseId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["WarehouseId"].ToString()),
                Amount = rdr["Amount"] == DBNull.Value ? 0 : decimal.Parse(rdr["Amount"].ToString()),
                ReStartReason = rdr["ReStartReason"] == DBNull.Value ? string.Empty : rdr["ReStartReason"].ToString(),
                CheckFilialeId = rdr["CheckFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["CheckFilialeId"].ToString()),
                IsTransfer = rdr["IsTransfer"] != DBNull.Value && bool.Parse(rdr["IsTransfer"].ToString()),
                SaleFilialeId = rdr["SaleFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SaleFilialeId"].ToString())
            };
            return checkRefundInfo;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="orderId"></param>
        /// <param name="applyNo"> </param>
        /// <returns></returns>
        public IList<CheckRefundInfo> GetShopCheckRefundList(Guid shopId, Guid orderId, string applyNo)
        {
            var builder = new StringBuilder(@"SELECT CR.RefundId,CR.RefundNo, CR.OrderId,S.ApplyNo as OrderNo,S.ShopName as Consignee, CR.ExpressNo, CR.ExpressName, 
CR.CreateDate, CR.CheckState,CR.Remark, CR.WarehouseId,
CR.Amount,CR.ReStartReason,CR.CheckFilialeId,CR.IsTransfer,CR.SaleFilialeId FROM lmShop_CheckRefund CR INNER JOIN ShopExchangedApply S ON CR.OrderId=S.ApplyID
WHERE S.ShopID='{0}' ");
            if (orderId != Guid.Empty)
            {
                builder.AppendFormat(" AND CR.OrderId='{0}'", orderId);
            }
            if (!string.IsNullOrEmpty(applyNo))
            {
                builder.AppendFormat(" AND (CR.RefundNo='{0}' OR S.ApplyNo='{0}')", applyNo);
            }
            builder.Append(" ORDER BY CR.CreateDate DESC ");

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CheckRefundInfo>(string.Format(builder.ToString(), shopId)).AsList();
            }
        }

        /// <summary>
        /// 获取联盟店退货检查明细
        /// </summary>
        /// <param name="refundId"></param>
        /// <param name="applyId"></param>
        /// <param name="checkState"></param>
        /// <returns></returns>
        public IList<CheckRefundDetailInfo> GetShopCheckRefundDetailList(Guid refundId, Guid applyId, int checkState)
        {
            var builder = new StringBuilder(@"SELECT CRD.Id, CRD.RefundId, CRD.GoodsId, CRD.RealGoodsId, CRD.GoodsCode, CRD.GoodsName,CRD.Specification, CRD.Quantity, 
CRD.SellPrice, CRD.ReturnCount, CRD.ReturnType, CRD.ReturnReason,CRD.DamageCount 
from lmShop_CheckRefundDetails CRD 
INNER JOIN lmShop_CheckRefund CR
ON CRD.RefundId=CR.RefundId  WHERE ");
            
            if (refundId != Guid.Empty)
            {
                builder.AppendFormat(" CR.RefundId='{0}' ", refundId);
            }
            else
            {
                builder.AppendFormat(" CR.OrderId='{0}' ", applyId);
            }
            if (checkState >= 0)
            {
                builder.Append(" AND CR.CheckState=").Append(checkState);
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<CheckRefundDetailInfo>(builder.ToString()).AsList();
            }
        }


 

    }
}

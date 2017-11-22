//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2007年5月27日
// 文件创建人:马力
// 最后修改时间:2007年5月27日
// 最后一次修改人:马力
//================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AllianceShop.Common.Extension;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;
using Keede.DAL.Helper.Sql;
using Keede.DAL.RWSplitting;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    public class Invoice : IInvoice
    {
        public Invoice(GlobalConfig.DB.FromType fromType)
        {

        }

        private const string SQL_INSERT_INVOICE_NEW = "INSERT INTO lmShop_Invoice([InvoiceId],[MemberId],[InvoiceName],[InvoiceContent],[Receiver],[PostalCode],[Address],[RequestTime],[InvoiceSum],[InvoiceState],[PurchaserType],[TaxpayerID],[CancelPersonel],AcceptedTime,InvoiceCode,InvoiceNo,NoteType,SaleFilialeId,SalePlatformId,IsShopInvoice,[IsAfterwardsApply]) VALUES(@InvoiceId,@MemberId,@InvoiceName,@InvoiceContent,@Receiver,@PostalCode,@Address,@RequestTime,@InvoiceSum,@InvoiceState,@PurchaserType,@TaxpayerID,@CancelPersonel,@AcceptedTime,@InvoiceCode,@InvoiceNo,@NoteType,@SaleFilialeId,@SalePlatformId,@IsShopInvoice,@IsAfterwardsApply);";
        private const string SQL_SELECT_INVOICE = @"SELECT InvoiceId,MemberId,InvoiceName,InvoiceContent,Receiver,PostalCode,
Address,RequestTime,InvoiceSum,InvoiceState,SaleFilialeId,InvoiceNo,InvoiceCode,AcceptedTime,SalePlatformId,IsAfterwardsApply FROM lmShop_Invoice WHERE InvoiceId=@InvoiceId;";
        private const string SQL_SELECT_INVOICE_BY_GOODS_ORDER = @"SELECT I.InvoiceId,I.MemberId,I.InvoiceName,I.InvoiceContent,I.Receiver,I.PostalCode,I.Address,
                                                                    I.RequestTime,I.InvoiceSum,
                                                                    I.InvoiceState,I.AcceptedTime,I.PurchaserType,I.InvoiceNo,I.IsCommit,I.SaleFilialeId,I.InvoiceState,I.IsAfterwardsApply FROM 
                                                                    lmShop_Invoice AS I
                                                                    INNER JOIN lmshop_OrderInvoice OI ON OI.InvoiceId=I.InvoiceId 
                                                                    WHERE OI.OrderId=@OrderId ";
        private const string S_QL_SELECT_FILIALE_ID_BY_INVOICE_ID = @"select o.FilialeId from lmshop_OrderInvoice i left join lmshop_GoodsOrder o WITH(NOLOCK) on i.OrderId=o.OrderId where i.InvoiceId=@InvoiceId";
        private const string SQL_SELECT_INVOICE_LIST = @"SELECT InvoiceId,MemberId,InvoiceName,InvoiceContent,Receiver,PostalCode,Address,RequestTime,InvoiceSum,InvoiceState,InvoiceCode,InvoiceNo,IsCommit,InvoiceChCode,InvoiceChNo,SaleFilialeId,SalePlatformId,IsAfterwardsApply FROM lmShop_Invoice;";
        private const string SQL_UPDATE_INVOICE_BY_ORDER_ID = @"update lmshop_GoodsOrder set InvoiceState=@InvoiceState where OrderId=@OrderId;
Update lmShop_Invoice set InvoiceState=@InvoiceState,CancelPersonel=@CancelPersonel Where InvoiceId in (select InvoiceId from lmshop_OrderInvoice where OrderId=@OrderId); ";
        private const string SQL_SELECT_INVOICE_LIST_BY_MEMBER = "SELECT InvoiceId,MemberId,InvoiceName,InvoiceContent,Receiver,PostalCode,Address,RequestTime,InvoiceSum,InvoiceState,SaleFilialeId,SalePlatformId,IsAfterwardsApply FROM lmShop_Invoice WHERE MemberId=@MemberId;";
        private const string SQL_UPDATE_INVOICE_BY_INVOICESTATE = @"
UPDATE lmShop_Invoice SET InvoiceState=@InvoiceState WHERE InvoiceId=@InvoiceId;
UPDATE lmShop_Invoice SET AcceptedTime=GETDATE() WHERE InvoiceId=@InvoiceId and @InvoiceState=2 ;
UPDATE lmShop_GoodsOrder SET InvoiceState=@InvoiceState WHERE OrderId IN (SELECT OrderId FROM lmShop_OrderInvoice WHERE InvoiceId=@InvoiceId);";

        private const string UPDATE_SQL_UPDATE_INVOICE_BY_INVOICESTATE = @"
UPDATE lmShop_Invoice SET InvoiceState=@InvoiceState WHERE InvoiceId=@InvoiceId;
UPDATE lmShop_GoodsOrder SET InvoiceState=@InvoiceState WHERE OrderId IN (SELECT OrderId FROM lmShop_OrderInvoice WHERE InvoiceId=@InvoiceId);";


        private const string SQL_DELETE_INVOICE_BY_INVOICEID = @"delete from lmshop_OrderInvoice where invoiceId=@InvoiceId
                                                            delete from lmShop_Invoice where invoiceId=@InvoiceId";

        #region [T-SQL]

        //修改
        //private const string SQL_UPDATE_INVOICE_BY_INVOICESTATE = "UPDATE lmShop_Invoice SET InvoiceState=@InvoiceState WHERE InvoiceId=@InvoiceId;UPDATE lmShop_GoodsOrder SET InvoiceState=@InvoiceState WHERE OrderId IN (SELECT OrderId FROM lmShop_OrderInvoice WHERE InvoiceId=@InvoiceId);";
        private const string SQL_UPDATE_INVOICE_BY_INVOICESTATE_SERVER = @" declare @state int;select @state=InvoiceState from lmShop_Invoice where InvoiceId=@InvoiceId;
                                  if(@InvoiceState=3 or @InvoiceState=4 or @InvoiceState=5)
	                                  begin
			                                  if(@state=1)--申请中
			                                  begin
				                                  UPDATE lmShop_Invoice SET InvoiceState=3 WHERE InvoiceId=@InvoiceId;
				                                  UPDATE lmShop_GoodsOrder SET InvoiceState=3 WHERE OrderId IN (SELECT OrderId FROM lmShop_OrderInvoice WHERE InvoiceId=@InvoiceId);
			                                  end
			                                  else if(@state=2)--已开发票
			                                  begin
			                                       UPDATE lmShop_Invoice SET InvoiceState=4 WHERE InvoiceId=@InvoiceId;
				                                   UPDATE lmShop_GoodsOrder SET InvoiceState=4 WHERE OrderId IN (SELECT OrderId FROM lmShop_OrderInvoice WHERE InvoiceId=@InvoiceId);
			                                  end
                                			  
	                                  end
                                  else
	                                  begin
		                                  UPDATE lmShop_Invoice SET InvoiceState=@InvoiceState WHERE InvoiceId=@InvoiceId;
		                                  UPDATE lmShop_GoodsOrder SET InvoiceState=@InvoiceState WHERE OrderId IN (SELECT OrderId FROM lmShop_OrderInvoice WHERE InvoiceId=@InvoiceId);
	                                  end";

        private const string SQL_UPDATE_INVOICE_INVOICESUM = "UPDATE lmShop_Invoice set InvoiceSum=@InvoiceSum where InvoiceId=@InvoiceId";//

        /// <summary>
        /// 
        /// </summary>
        public const string SQL_UPDATE_INVOICE = @"UPDATE [dbo].[lmShop_Invoice]
                                                   SET [MemberId] = @MemberId
                                                      ,[InvoiceName] = @InvoiceName
                                                      ,[InvoiceContent] = @InvoiceContent
                                                      ,[Receiver] = @Receiver
                                                      ,[PostalCode] = @PostalCode
                                                      ,[Address] = @Address
                                                      ,[RequestTime] = @RequestTime
                                                      ,[InvoiceSum] = @InvoiceSum
                                                      ,[InvoiceState] = @InvoiceState     
                                                      ,[PurchaserType]=@PurchaserType                                                
                                                 WHERE [InvoiceId] = @InvoiceId";

        //查询

        //删除
        private const string SQL_DELETE_INVOICE_BY_ORDERID = @"declare @invoiceId uniqueidentifier
                                                            set @invoiceId=(select top 1 InvoiceId from lmshop_OrderInvoice where OrderId=@orderId)
                                                            delete from lmshop_OrderInvoice where OrderId=@orderId and invoiceId=@invoiceId
                                                            delete from lmShop_Invoice where invoiceId=@invoiceId";
        private const string SQL_SELECT_INVOICE_BY_GOODS_ORDER_SERVER = @"SELECT I.InvoiceId,I.MemberId,I.InvoiceName,I.InvoiceContent,I.Receiver,I.PostalCode,I.Address,
                            I.RequestTime,I.InvoiceSum,I.InvoiceState,I.AcceptedTime,I.PurchaserType,I.InvoiceNo,I.IsCommit,I.SaleFilialeId,I.SalePlatformId,I.IsAfterwardsApply   
                            FROM lmShop_Invoice AS I
                            INNER JOIN lmshop_OrderInvoice OI ON OI.InvoiceId=I.InvoiceId 
                            --order by I.InvoiceId
                            WHERE OI.OrderId=@OrderId";

        #endregion

        private const string PARM_INVOICEID = "@InvoiceId";
        private const string PARM_MEMBERID = "@MemberId";
        private const string PARM_INVOICENAME = "@InvoiceName";
        private const string PARM_INVOICECONTENT = "@InvoiceContent";
        private const string PARM_RECEIVER = "@Receiver";
        private const string PARM_POSTALCODE = "@PostalCode";
        private const string PARM_ADDRESS = "@Address";
        private const string PARM_REQUESTTIME = "@RequestTime";
        private const string PARM_INVOICESUM = "@InvoiceSum";
        private const string PARM_INVOICESTATE = "@InvoiceState";
        private const string PARM_ORDERID = "@OrderId";
        private const string PARM_CANCELPERSONEL = "@CancelPersonel";
        private const string PARM_SALEFILIALEID = "@SaleFilialeId";
        private const string PARM_IS_AFTERWARDS_APPLY = "@IsAfterwardsApply";

        /// <summary> 删除之前的老发票添加新发票
        /// </summary>
        /// <param name="invoice">发票类</param>
        /// <param name="dictOrderIdOrderNo">订单数组</param>
        public bool Insert(InvoiceInfo invoice, Dictionary<Guid, string> dictOrderIdOrderNo)
        {
            var parms = new[]
                {
                    new Parameter("InvoiceId",invoice.InvoiceId),
                    new Parameter("MemberId",invoice.MemberId),
                    new Parameter("InvoiceName",invoice.InvoiceName),
                    new Parameter("InvoiceContent",invoice.InvoiceContent),
                    new Parameter("Receiver",invoice.Receiver),
                    new Parameter("PostalCode",invoice.PostalCode),
                    new Parameter("Address",invoice.Address),
                    new Parameter("RequestTime",invoice.RequestTime),
                    new Parameter("InvoiceSum",invoice.InvoiceSum),
                    new Parameter("InvoiceState",invoice.InvoiceState),
                    new Parameter("PurchaserType",invoice.PurchaserType),
                    new Parameter("TaxpayerID",invoice.TaxpayerID),
                    new Parameter("CancelPersonel",invoice.CancelPersonel),
                    new Parameter("SaleFilialeId",invoice.SaleFilialeId),
                    new Parameter("SalePlatformId",invoice.SalePlatformId),
                    new Parameter("IsShopInvoice",invoice.IsShopInvoice),
                    new Parameter("IsAfterwardsApply",invoice.IsAfterwardsApply)
                };

            const string INSERT_ORDER_INVOICE = @"
IF NOT EXISTS(SELECT TOP 1 InvoiceId FROM lmshop_OrderInvoice WHERE InvoiceId=@InvoiceId AND OrderId=@OrderId)
	BEGIN
		INSERT INTO lmshop_OrderInvoice (InvoiceId,OrderId,DeliverWarehouseId,OrderNo) VALUES (@InvoiceId,@OrderId,@DeliverWarehouseId,@OrderNo);
	END
";
            const string INSERT_INVOICE = @"
IF NOT EXISTS(SELECT TOP 1 InvoiceId FROM lmShop_Invoice WHERE InvoiceId=@InvoiceId)
	BEGIN
        INSERT INTO lmShop_Invoice([InvoiceId],[MemberId],[InvoiceName],[InvoiceContent],[Receiver],[PostalCode],[Address],[RequestTime],[InvoiceSum],[InvoiceState],[PurchaserType],[TaxpayerID],[CancelPersonel],[SaleFilialeId],[SalePlatformId],[IsShopInvoice],[IsAfterwardsApply]) VALUES(@InvoiceId,@MemberId,@InvoiceName,@InvoiceContent,@Receiver,@PostalCode,@Address,@RequestTime,@InvoiceSum,@InvoiceState,@PurchaserType,@TaxpayerID,@CancelPersonel,@SaleFilialeId,@SalePlatformId,@IsShopInvoice,@IsAfterwardsApply);
    END
ELSE
	BEGIN
        UPDATE lmShop_Invoice SET [MemberId]=@MemberId,[InvoiceName]=@InvoiceName,[InvoiceContent]=@InvoiceContent,[Receiver]=@Receiver,[PostalCode]=@PostalCode,[Address]=@Address,[RequestTime]=@RequestTime,[InvoiceSum]=@InvoiceSum,[InvoiceState]=@InvoiceState,[PurchaserType]=@PurchaserType,[TaxpayerID]=@TaxpayerID,[CancelPersonel]=@CancelPersonel,[SaleFilialeId]=@SaleFilialeId,[SalePlatformId]=@SalePlatformId,IsShopInvoice=@IsShopInvoice,IsAfterwardsApply=@IsAfterwardsApply WHERE [InvoiceId]=@InvoiceId;
    END
";

            using (var db = DatabaseFactory.Create())
            {
                db.BeginTransaction();

                db.Execute(false, INSERT_INVOICE, parms);
                foreach (var idNo in dictOrderIdOrderNo)
                {
                    db.Execute(false, INSERT_ORDER_INVOICE, new Parameter("InvoiceId", invoice.InvoiceId), new Parameter("OrderId", idNo.Key), new Parameter("DeliverWarehouseId", invoice.DeliverWarehouseId), new Parameter("OrderNo", idNo.Value));
                }
                return db.CompleteTransaction();
            }
        }

        /// <summary>添加发票
        /// </summary>
        /// <param name="invoice">发票类</param>
        public void Insert(InvoiceInfo invoice)
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_INVOICEID, SqlDbType.UniqueIdentifier){Value =  invoice.InvoiceId},
                                new SqlParameter(PARM_MEMBERID, SqlDbType.UniqueIdentifier){Value =invoice.MemberId },
                                new SqlParameter(PARM_INVOICENAME, SqlDbType.VarChar, 128){Value = invoice.InvoiceName},
                                new SqlParameter(PARM_INVOICECONTENT, SqlDbType.VarChar, 128){Value = invoice.InvoiceContent },
                                new SqlParameter(PARM_RECEIVER, SqlDbType.VarChar, 64){Value = invoice.Receiver },
                                new SqlParameter(PARM_POSTALCODE, SqlDbType.VarChar, 8){Value = invoice.PostalCode},
                                new SqlParameter(PARM_ADDRESS, SqlDbType.VarChar, 128){Value =string.IsNullOrEmpty(invoice.Address) ? DBNull.Value : (object)invoice.Address},
                                new SqlParameter(PARM_REQUESTTIME, SqlDbType.DateTime){Value =  invoice.RequestTime},
                                new SqlParameter(PARM_INVOICESUM, SqlDbType.Float){Value = invoice.InvoiceSum},
                                new SqlParameter(PARM_INVOICESTATE, SqlDbType.Int){Value = invoice.InvoiceState},
                                new SqlParameter("@PurchaserType", SqlDbType.Int){Value =(int) invoice.PurchaserType},
                                new SqlParameter("@TaxpayerID", SqlDbType.VarChar){Value = invoice.TaxpayerID},
                                new SqlParameter("@CancelPersonel",SqlDbType.VarChar,128){Value = invoice.CancelPersonel},
                                new SqlParameter("@AcceptedTime", SqlDbType.DateTime){Value = invoice.AcceptedTime},
                                new SqlParameter("@InvoiceCode", SqlDbType.VarChar,128){Value = invoice.InvoiceCode},
                                new SqlParameter("@InvoiceNo", SqlDbType.BigInt){Value = invoice.InvoiceNo},
                                new SqlParameter("@NoteType", SqlDbType.Int){Value = invoice.NoteType},
                                new SqlParameter(PARM_SALEFILIALEID,SqlDbType.UniqueIdentifier){Value = invoice.SaleFilialeId},
                                new SqlParameter("@SalePlatformId",SqlDbType.UniqueIdentifier){Value = invoice.SalePlatformId},
                                new SqlParameter("@IsShopInvoice",SqlDbType.BigInt){Value = invoice.IsShopInvoice},
                                new SqlParameter("@IsAfterwardsApply",SqlDbType.BigInt){Value = invoice.IsAfterwardsApply}
                            };
            var parmsinvoice = new[]
             {
                new SqlParameter(PARM_INVOICEID, SqlDbType.UniqueIdentifier)
             };
            parmsinvoice[0].Value = invoice.InvoiceId;

            var parmsOrder = new[]
             {
                new SqlParameter(PARM_INVOICEID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_ORDERID, SqlDbType.UniqueIdentifier)
             };
            parmsOrder[0].Value = invoice.InvoiceId;

            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, SQL_DELETE_INVOICE_BY_INVOICEID, parmsinvoice);

                    SqlHelper.ExecuteNonQuery(trans, SQL_INSERT_INVOICE_NEW, parms);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        /// <summary>
        /// 获取指定的发票索取记录
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceInfo GetInvoice(Guid invoiceId)
        {
            var parm = new SqlParameter(PARM_INVOICEID, SqlDbType.UniqueIdentifier) { Value = invoiceId };
            InvoiceInfo invoiceInfo;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_INVOICE, parm))
            {
                invoiceInfo = rdr.Read() ? new InvoiceInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr[5] == DBNull.Value ? null : rdr.GetString(5), rdr.GetString(6), rdr.GetDateTime(7), float.Parse(rdr.GetValue(8).ToString()), rdr.GetInt32(9))
                {
                    SaleFilialeId = rdr[10] == DBNull.Value ? Guid.Empty : rdr.GetGuid(10),
                    InvoiceNo = rdr.GetInt64(11),
                    InvoiceCode = rdr[12] == DBNull.Value ? string.Empty : rdr.GetString(12),
                    AcceptedTime = rdr[13] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(13),
                    SalePlatformId = rdr[14] == DBNull.Value ? Guid.Empty : rdr.GetGuid(14),
                    IsAfterwardsApply = rdr["IsAfterwardsApply"] != DBNull.Value && Convert.ToBoolean(rdr["IsAfterwardsApply"])
                } : new InvoiceInfo();
            }
            return invoiceInfo;
        }

        /// <summary>
        /// Func : 根据订单号，获取指定的发票索取记录
        /// Code : dyy
        /// Date : 2009 Nov 26th
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public InvoiceInfo GetInvoiceByGoodsOrder_Server(Guid orderId)
        {
            var parm = new SqlParameter("@OrderId", SqlDbType.UniqueIdentifier) { Value = orderId };
            InvoiceInfo invoiceInfo;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_INVOICE_BY_GOODS_ORDER_SERVER, parm))
            {
                if (rdr.Read())
                {
                    invoiceInfo = new InvoiceInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3),
                                                  rdr.GetString(4), rdr[5] == DBNull.Value ? null : rdr.GetString(5),
                                                  rdr.GetString(6), rdr.GetDateTime(7),
                                                  float.Parse(rdr.GetValue(8).ToString()), rdr.GetInt32(9))
                    {
                        AcceptedTime = rdr["AcceptedTime"] == DBNull.Value ? DateTime.MinValue : (DateTime)rdr["AcceptedTime"],
                        InvoiceNo = rdr["InvoiceNo"] == DBNull.Value ? 0 : long.Parse(rdr["InvoiceNo"].ToString()),
                        IsCommit = rdr["IsCommit"] != DBNull.Value && bool.Parse(rdr["IsCommit"].ToString()),
                        SaleFilialeId = rdr["SaleFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SaleFilialeId"].ToString()),
                        SalePlatformId = rdr["SalePlatformId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SalePlatformId"].ToString()),
                        IsAfterwardsApply = rdr["IsAfterwardsApply"] != DBNull.Value && Convert.ToBoolean(rdr["IsAfterwardsApply"])
                    };
                }
                else
                {
                    invoiceInfo = new InvoiceInfo();
                }
            }
            return invoiceInfo;
        }


        /// <summary>
        /// 返回所有发票列表
        /// </summary>
        /// <returns></returns>
        public IList<InvoiceInfo> GetInvoiceList()
        {
            IList<InvoiceInfo> invoiceList = new List<InvoiceInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_INVOICE_LIST, null))
            {
                while (rdr.Read())
                {
                    var invoiceInfo = new InvoiceInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr[5] == DBNull.Value ? null : rdr.GetString(5), rdr.GetString(6), rdr.GetDateTime(7), float.Parse(rdr.GetValue(8).ToString()), rdr.GetInt32(9))
                    {
                        SaleFilialeId = rdr[10] == DBNull.Value ? Guid.Empty : rdr.GetGuid(10),
                        SalePlatformId = rdr[11] == DBNull.Value ? Guid.Empty : rdr.GetGuid(11),
                        IsAfterwardsApply = rdr["IsAfterwardsApply"] != DBNull.Value && Convert.ToBoolean(rdr["IsAfterwardsApply"])
                    };

                    invoiceList.Add(invoiceInfo);
                }
            }
            return invoiceList;
        }

        /// <summary>
        /// 根据条件查找发票
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="isOrderComplete"></param>
        /// <param name="orderNo"></param>
        /// <param name="invoiceName"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="address"></param>
        /// <param name="invoiceContent"></param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="isNeedManual">是否需要手动打印发票</param>
        /// <param name="warehouseIds"></param>
        /// <param name="invoiceType"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="cancelPersonnel"></param>
        /// <returns></returns>
        public IList<InvoiceInfo> GetInvoiceList(DateTime startTime, DateTime endTime, bool isOrderComplete, string orderNo, string invoiceName, string invoiceNo, string address, string invoiceContent, InvoiceState invoiceState, bool isNeedManual, IEnumerable<Guid> warehouseIds,int invoiceType,Guid saleFilialeId,string cancelPersonnel)
        {
            if (warehouseIds == null || !warehouseIds.Any()) return new List<InvoiceInfo>();
            var builder = new StringBuilder(@"
SELECT 
    Distinct(I.InvoiceId),I.MemberId,I.InvoiceName,I.InvoiceContent,
    I.Receiver,I.PostalCode,I.Address,I.RequestTime,I.InvoiceSum,
    I.InvoiceState,I.CancelPersonel,I.IsCommit,I.NoteType,
    I.AcceptedTime,I.InvoiceCode,I.InvoiceNo,I.IsNeedManual,
    I.SaleFilialeId,I.SalePlatformId,I.IsAfterwardsApply
FROM lmShop_Invoice I with(nolock)
INNER JOIN lmshop_OrderInvoice OI with(nolock) ON OI.InvoiceId=I.InvoiceId ");

            builder.Append(" WHERE I.RequestTime >= '").Append(startTime).Append("'");
            builder.Append(" AND I.RequestTime < '").Append(endTime).Append("'");
            if (invoiceType != -1)
            {
                builder.AppendFormat(" AND I.NoteType={0} ", invoiceType);
            }
            if (saleFilialeId != Guid.Empty)
            {
                builder.AppendFormat(" AND I.SaleFilialeId='{0}'", saleFilialeId);
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                builder.Append(" AND OI.OrderNo='").Append(orderNo).Append("'");
            }
            if (!string.IsNullOrEmpty(invoiceName))
            {
                builder.Append(" AND I.InvoiceName='").Append(invoiceName).Append("'");
            }
            if (!string.IsNullOrEmpty(invoiceNo))
            {
                var objNotNumberPattern = new Regex("^[0-9]*$");
                if (objNotNumberPattern.IsMatch(invoiceNo))
                {
                    builder.Append(" AND I.InvoiceNo=").Append(invoiceNo);
                }
            }
            if (!string.IsNullOrEmpty(address))
            {
                builder.Append(" AND LOWER(I.Address) LIKE '%").Append(address).Append("%'");
            }
            if (!string.IsNullOrEmpty(invoiceContent))
            {
                builder.Append(" AND I.InvoiceContent='").Append(invoiceContent).Append("'");
            }
            if (invoiceState != InvoiceState.All)
                builder.Append(" AND I.InvoiceState=").Append((int)invoiceState);
            //add by dyy at 2009 Nov 26th 只显示已完成的订单
            builder.Append(" AND I.IsAfterwardsApply = ").Append(isOrderComplete ? 1 : 0);
            if (isNeedManual)
            {
                builder.Append(" AND I.IsNeedManual = ").Append((int)IsNeedManualType.Yes);
            }
            if (warehouseIds.Count()==1)
            {
                builder.Append(" AND OI.DeliverWarehouseId='").Append(warehouseIds.First()).Append("'");
            }
            else
            {
                builder.AppendFormat(" AND OI.DeliverWarehouseId IN('{0}')",string.Join("','",warehouseIds));
            }
            if (!string.IsNullOrEmpty(cancelPersonnel))
            {
                builder.AppendFormat(" AND I.CancelPersonel LIKE '%{0}%'", cancelPersonnel);
            }

            builder.Append(" ORDER BY I.RequestTime;");

            IList<InvoiceInfo> invoiceList = new List<InvoiceInfo>();

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), null))
            {
                while (rdr.Read())
                {
                    var invoiceInfo = new InvoiceInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), 
                        rdr[5] == DBNull.Value ? null : rdr.GetString(5), rdr.GetString(6), rdr.GetDateTime(7), float.Parse(rdr.GetValue(8).ToString()), rdr.GetInt32(9))
                    {
                        CancelPersonel = rdr[10] == DBNull.Value ? "" : rdr.GetString(10),
                        IsCommit = rdr["IsCommit"] != DBNull.Value && (bool)rdr["IsCommit"],
                        NoteType = (InvoiceNoteType)(int.Parse(rdr["NoteType"].ToString())),
                        AcceptedTime = rdr["AcceptedTime"] != DBNull.Value
                                               ? Convert.ToDateTime(rdr["AcceptedTime"])
                                               : DateTime.MinValue,
                        InvoiceCode = rdr["InvoiceCode"] != DBNull.Value ? rdr["InvoiceCode"].ToString() : "-",
                        InvoiceNo = rdr["InvoiceNo"] != DBNull.Value ? (long)rdr["InvoiceNo"] : 0,
                        IsNeedManual = rdr["IsNeedManual"] != DBNull.Value && (bool)rdr["IsNeedManual"],
                        SaleFilialeId = rdr["SaleFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SaleFilialeId"].ToString()),
                        SalePlatformId = rdr["SalePlatformId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SalePlatformId"].ToString()),
                        IsAfterwardsApply = rdr["IsAfterwardsApply"] != DBNull.Value && Convert.ToBoolean(rdr["IsAfterwardsApply"])
                    };
                    invoiceList.Add(invoiceInfo);
                }
            }
            return invoiceList;
        }

        /// <summary>
        /// 根据条件查找发票(分页)
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="isOrderComplete"></param>
        /// <param name="orderNo"> </param>
        /// <param name="invoiceName"> </param>
        /// <param name="invoiceNo"> </param>
        /// <param name="address"> </param>
        /// <param name="invoiceContent"> </param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="isNeedManual">是否需要手动打印发票</param>
        /// <param name="warehouseId"> </param>
        /// <param name="permissionFilialeId"> </param>
        /// <param name="permissionBranchId"> </param>
        /// <param name="permissionPositionId"> </param>
        /// <param name="invoiceType"></param>
        /// <param name="saleFilialeid"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="cancelPersonel"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public IList<InvoiceInfo> GetInvoiceListByPage(DateTime startTime, DateTime endTime, bool isOrderComplete,
            string orderNo, string invoiceName, string invoiceNo, string address, string invoiceContent,
            InvoiceState invoiceState, bool isNeedManual, Guid warehouseId, Guid permissionFilialeId,
            Guid permissionBranchId, Guid permissionPositionId, int invoiceType, Guid saleFilialeid,string cancelPersonel, int pageIndex,
            int pageSize, out int recordCount)
        {
            var builder = new StringBuilder(@"select I.InvoiceId,I.MemberId,I.InvoiceName,I.InvoiceContent,
    I.Receiver,I.PostalCode,I.Address,I.RequestTime,I.InvoiceSum,
    I.InvoiceState,I.CancelPersonel,I.IsCommit,I.NoteType,
    I.AcceptedTime,I.InvoiceCode,I.InvoiceNo,I.IsNeedManual,
    I.SaleFilialeId,I.SalePlatformId,I.IsAfterwardsApply from 
(SELECT I.InvoiceId,I.MemberId,I.InvoiceName,I.InvoiceContent,
    I.Receiver,I.PostalCode,I.Address,I.RequestTime,I.InvoiceSum,
    I.InvoiceState,I.CancelPersonel,I.IsCommit,I.NoteType,
    I.AcceptedTime,I.InvoiceCode,I.InvoiceNo,I.IsNeedManual,
    I.SaleFilialeId,I.SalePlatformId,I.IsAfterwardsApply
FROM lmShop_Invoice I with(nolock) ");
            builder.AppendFormat(" WHERE I.RequestTime >= '{0}' AND I.RequestTime < '{1}'", startTime,endTime);
            if (invoiceType != -1)
            {
                builder.AppendFormat(" AND I.NoteType={0} ", invoiceType);
            }
            if (saleFilialeid != Guid.Empty)
            {
                builder.AppendFormat(" AND I.SaleFilialeId='{0}'", saleFilialeid);
            }
            if (!string.IsNullOrEmpty(invoiceName))
            {
                builder.Append(" AND I.InvoiceName='").Append(invoiceName).Append("'");
            }
            if (!string.IsNullOrEmpty(invoiceNo))
            {
                var objNotNumberPattern = new Regex("^[0-9]*$");
                if (objNotNumberPattern.IsMatch(invoiceNo))
                {
                    builder.Append(" AND I.InvoiceNo=").Append(invoiceNo);
                }
            }
            if (!string.IsNullOrEmpty(address))
            {
                builder.Append(" AND LOWER(I.Address) LIKE '%").Append(address).Append("%'");
            }
            if (!string.IsNullOrEmpty(invoiceContent))
            {
                builder.Append(" AND I.InvoiceContent='").Append(invoiceContent).Append("'");
            }
            
            //add by dyy at 2009 Nov 26th 只显示已完成的订单
            builder.Append(" AND I.IsAfterwardsApply = ").Append(isOrderComplete ? 1 : 0);

            if (invoiceState != InvoiceState.All)
                builder.Append(" AND I.InvoiceState=").Append((int)invoiceState);

            if (isNeedManual)
            {
                builder.Append(" AND I.IsNeedManual = ").Append((int)IsNeedManualType.Yes);
            }
            if (!string.IsNullOrEmpty(cancelPersonel))
            {
                builder.AppendFormat(" AND I.CancelPersonel LIKE '%{0}%'", cancelPersonel);
            }

            builder.Append(" ) as I INNER JOIN lmshop_OrderInvoice OI with(nolock) ON OI.InvoiceId=I.InvoiceId WHERE ");

            if (warehouseId != Guid.Empty)
            {
                builder.Append(" OI.DeliverWarehouseId='").Append(warehouseId).Append("'");
            }
            else
            {
                builder.Append(" OI.DeliverWarehouseId IN(");
                builder.Append(" SELECT WarehouseID FROM WarehousePermission WHERE ");
                builder.Append("FilialeID='").Append(permissionFilialeId).Append("'");
                builder.Append(" AND BranchID='").Append(permissionBranchId).Append("'");
                builder.Append(" AND PositionID='").Append(permissionPositionId).Append("')");
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                builder.Append(" AND OI.OrderNo='").Append(orderNo).Append("'");
            }

            using (var db = DatabaseFactory.Create())
            {
                var pageQuery = new PageQuery(pageIndex, pageSize, builder.ToString(), " RequestTime DESC");
                var pageItem = db.SelectByPage<InvoiceInfo>(true, pageQuery,null);
                recordCount = (int)pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }


        /// <summary>
        /// 返回指定会员的发票列表
        /// </summary>
        /// <param name="memberId">会员编号</param>
        /// <returns></returns>
        public IList<InvoiceInfo> GetInvoiceList(Guid memberId)
        {
            var parm = new SqlParameter(PARM_MEMBERID, SqlDbType.UniqueIdentifier) { Value = memberId };
            IList<InvoiceInfo> invoiceList = new List<InvoiceInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_INVOICE_LIST_BY_MEMBER, parm))
            {
                while (rdr.Read())
                {
                    var invoiceInfo = new InvoiceInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr[5] == DBNull.Value ? null : rdr.GetString(5), rdr.GetString(6), rdr.GetDateTime(7), float.Parse(rdr.GetValue(8).ToString()), rdr.GetInt32(9))
                    {
                        SaleFilialeId = rdr[10] == DBNull.Value ? Guid.Empty : rdr.GetGuid(10),
                        SalePlatformId = rdr[11] == DBNull.Value ? Guid.Empty : rdr.GetGuid(11),
                        IsAfterwardsApply = rdr["IsAfterwardsApply"] != DBNull.Value && Convert.ToBoolean(rdr["IsAfterwardsApply"])
                    };
                    invoiceList.Add(invoiceInfo);
                }
            }
            return invoiceList;
        }

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        public bool SetInvoiceState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
UPDATE lmShop_Invoice SET InvoiceState=" + (int)invoiceState + @"
FROM lmShop_Invoice A
WHERE A.InvoiceId='" + invoiceId + @"' AND A.IsCommit=0

UPDATE lmShop_Invoice SET AcceptedTime=GETDATE()
FROM lmShop_Invoice A
WHERE A.InvoiceId='" + invoiceId + @"' AND A.InvoiceState=2 AND A.IsCommit=0

UPDATE lmShop_GoodsOrder SET InvoiceState=" + (int)invoiceState + @"
FROM lmShop_GoodsOrder A
INNER JOIN lmShop_OrderInvoice B ON A.OrderId=B.OrderId
INNER JOIN lmShop_Invoice C ON B.InvoiceId=C.InvoiceId AND C.IsCommit=0
WHERE C.InvoiceId='" + invoiceId + "'");

            if (invoiceState == InvoiceState.Waste)
            {
                sb.Append(@"
UPDATE lmShop_Invoice SET NoteType=1,CancelPersonel='" + cancelPersonel + @"'
FROM lmShop_Invoice A
WHERE A.InvoiceId='" + invoiceId + @"' AND A.IsCommit=0");
            }

            string sql = @"BEGIN TRANSACTION " +
                        sb +
                        @"
                        IF @@error <> 0
                           BEGIN
                             ROLLBACK TRANSACTION
                           END
                        ELSE
                           BEGIN
                             COMMIT TRANSACTION
                           END";


            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, null) > 0;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceIdList">发票编号列表</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        /// zal 2016-12-27
        public bool BatchSetInvoiceState(List<Guid> invoiceIdList, InvoiceState invoiceState, string cancelPersonel)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
select id as InvoiceId from splitToTable('{0}',',') #temp

UPDATE lmShop_Invoice SET InvoiceState=" + invoiceState + @"
FROM lmShop_Invoice A
INNER JOIN #temp B ON A.InvoiceId=B.InvoiceId AND A.IsCommit=0

UPDATE lmShop_Invoice SET AcceptedTime=GETDATE()
FROM lmShop_Invoice A
INNER JOIN #temp B ON A.InvoiceId=B.InvoiceId AND A.InvoiceState=2 AND A.IsCommit=0

UPDATE lmShop_GoodsOrder SET InvoiceState=" + invoiceState + @"
FROM lmShop_GoodsOrder A
INNER JOIN lmShop_OrderInvoice B ON A.OrderId=B.OrderId
INNER JOIN lmShop_Invoice C ON B.InvoiceId=C.InvoiceId AND C.IsCommit=0
INNER JOIN #temp D ON C.InvoiceId=D.InvoiceId", string.Join(",", invoiceIdList.ToArray()));

            if (invoiceState == InvoiceState.Waste)
            {
                sb.Append(@"
UPDATE lmShop_Invoice SET NoteType=1,CancelPersonel='" + cancelPersonel + @"'
FROM lmShop_Invoice A
INNER JOIN #temp B ON A.InvoiceId=B.InvoiceId AND A.IsCommit=0");
            }

            string sql = @"BEGIN TRANSACTION " +
                        sb +
                        @"
                        IF @@error <> 0
                           BEGIN
                             ROLLBACK TRANSACTION
                           END
                        ELSE
                           BEGIN
                             COMMIT TRANSACTION
                           END";


            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, null) > 0;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="invoiceState">发票状态</param>
        /// <param name="cancelPersonel">作废申请人</param>
        public bool UpdateSetInvoiceState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel)
        {
            const string SQL = @"SELECT IsCommit FROM [lmShop_Invoice] WHERE InvoiceId=@InvoiceId";
            var isCommit = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL,
                                                   new SqlParameter("@InvoiceId", invoiceId));
            if (Convert.ToBoolean(isCommit))
            {
                return false;
            }

            var parms = new[] {
                    new SqlParameter(PARM_INVOICEID, SqlDbType.UniqueIdentifier),
                    new SqlParameter(PARM_INVOICESTATE, SqlDbType.Int),
                    new SqlParameter(PARM_CANCELPERSONEL,SqlDbType.VarChar,50)
                };
            parms[0].Value = invoiceId;
            parms[1].Value = invoiceState;

            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {

                    SqlHelper.ExecuteNonQuery(trans, UPDATE_SQL_UPDATE_INVOICE_BY_INVOICESTATE, parms);
                    if (invoiceState == InvoiceState.Waste)
                    {
                        const string SQL2 = "UPDATE lmShop_Invoice SET NoteType=1,CancelPersonel=@CancelPersonel WHERE InvoiceId=@InvoiceId;";
                        var sqlparams = new[] { new SqlParameter("@CancelPersonel", cancelPersonel), new SqlParameter("@InvoiceId", invoiceId) };
                        SqlHelper.ExecuteNonQuery(trans, SQL2, sqlparams);
                    }
                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// 提交作废申请人 add by FanGuan 2012-06-25
        /// </summary>
        /// <param name="invoiceId">发票ID</param>
        /// <param name="cancelPersonel">作废申请人</param>
        public void SetCancelPersonel(Guid invoiceId, string cancelPersonel)
        {
            const string SQL = @"SELECT IsCommit FROM [lmShop_Invoice] WHERE InvoiceId=@InvoiceId";
            var isCommit = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL, new SqlParameter("@InvoiceId", invoiceId));
            if (!Convert.ToBoolean(isCommit))
            {
                const string SQL2 = "UPDATE lmShop_Invoice SET CancelPersonel=@CancelPersonel WHERE InvoiceId=@InvoiceId;";
                var parms = new[] {
                    new SqlParameter(PARM_INVOICEID, SqlDbType.UniqueIdentifier),
                    new SqlParameter(PARM_CANCELPERSONEL,SqlDbType.VarChar,50)
                };
                parms[0].Value = invoiceId;
                parms[1].Value = cancelPersonel;
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL2, parms);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="invoiceState"></param>
        /// <param name="cancelPersonel"></param>
        public void WasteState(Guid invoiceId, InvoiceState invoiceState, string cancelPersonel)
        {
            var parms = new[] {
                    new SqlParameter(PARM_ORDERID,invoiceId ),
                    new SqlParameter(PARM_INVOICESTATE, invoiceState),
                    new SqlParameter("@CancelPersonel",cancelPersonel==""?(object)DBNull.Value:cancelPersonel)

                };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_INVOICE_BY_ORDER_ID, parms);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }

        }

        private static SqlParameter[] GetParameters()
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_INVOICEID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_MEMBERID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_INVOICENAME, SqlDbType.VarChar, 128),
                                new SqlParameter(PARM_INVOICECONTENT, SqlDbType.VarChar, 128),
                                new SqlParameter(PARM_RECEIVER, SqlDbType.VarChar, 64),
                                new SqlParameter(PARM_POSTALCODE, SqlDbType.VarChar, 8),
                                new SqlParameter(PARM_ADDRESS, SqlDbType.VarChar, 128),
                                new SqlParameter(PARM_REQUESTTIME, SqlDbType.DateTime),
                                new SqlParameter(PARM_INVOICESUM, SqlDbType.Float),
                                new SqlParameter(PARM_INVOICESTATE, SqlDbType.Int),
                                new SqlParameter("@PurchaserType", SqlDbType.Int),
                                new SqlParameter("@TaxpayerID", SqlDbType.VarChar),
                                new SqlParameter("@CancelPersonel",SqlDbType.VarChar,128),
                                new SqlParameter(PARM_SALEFILIALEID,SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_IS_AFTERWARDS_APPLY,SqlDbType.Int)
                            };
            return parms;
        }

        /// <summary>
        /// 查找开出发票的总金额 add by dinghq 2011-04-12
        /// </summary>
        /// <param name="start">指定起始时间</param>
        /// <param name="end">指定结束时间</param>
        /// <param name="state">指定状态</param>
        /// <returns></returns>
        public decimal GetInvioceTotal(DateTime start, DateTime end, InvoiceState state)
        {
            const string SQL = "SELECT SUM(InvoiceSum) AS InvoiceSum FROM lmShop_Invoice WHERE NoteType=0  AND [AcceptedTime] BETWEEN @start AND @end";
            var param = new[] { new SqlParameter("state", (int)state), new SqlParameter("@start", start), new SqlParameter("@end", end) };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, param))
            {
                if (rdr != null)
                {
                    if (rdr.Read())
                    {
                        return rdr[0] == DBNull.Value ? 0 : (decimal)rdr.GetDouble(0);
                    }
                    rdr.Close();
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取发票某个时间段的统计信息
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="keyword"></param>
        /// <param name="invoicestate"></param>
        /// <param name="yesorno">是否显示订单重复记录</param>
        /// <returns></returns>
        public IList<InvoiceInfo> GetInvoiceStatistcsInfoList(DateTime starttime, DateTime endtime, string keyword, int invoicestate, YesOrNo yesorno)
        {
            var param = new[] { new SqlParameter("@start", starttime), new SqlParameter("@end", endtime),
                new SqlParameter("@keyword", keyword),new SqlParameter("@InvoiceState",invoicestate) };
            IList<InvoiceInfo> ilist = new List<InvoiceInfo>();
            string sql = @"   select  distinct  i.InvoiceId,InvoiceName,InvoiceContent,AcceptedTime,InvoiceSum,i.InvoiceState,
    STUFF((Select ','+ t.OrderNo FROM 
((select o.OrderNo from lmshop_OrderInvoice g left join lmshop_GoodsOrder o WITH(NOLOCK) on g.OrderId=o.OrderId 
where g.InvoiceId=i.InvoiceId )) t FOR XML PATH('')),1,1,'') as ordernos,InvoiceNo,InvoiceCode,IsCommit,InvoiceChNo,InvoiceChCode,
NoteType,TaxpayerID 
   from lmShop_Invoice I left join lmshop_OrderInvoice OI ON I.InvoiceId=OI.InvoiceId
   left join lmshop_GoodsOrder G WITH(NOLOCK) on OI.OrderId=G.OrderId 
   where 1=1 and  AcceptedTime  BETWEEN @start AND @end  ";
            if (!string.IsNullOrEmpty(keyword))
                sql += @" and (charindex(@keyword,InvoiceName)>0 or 
   charindex(@keyword,g.OrderNo)>0 or charindex(@keyword,g.Consignee)>0 or charindex(@keyword,InvoiceNo)>0) ";
            if (invoicestate == (int)InvoiceState.WasteRequest)
            {
                sql += " and i.InvoiceState=@InvoiceState ";
            }
            else
            {
                sql += " and i.InvoiceState in (2,4,5) ";
            }
            if (YesOrNo.Yes == yesorno)
            {
                #region 选择重复的订单号
                string sqlAgOrders = " select ordernos from (" + sql + ") t group by ordernos having COUNT( ordernos)>=2 ";

                #endregion
                sql += @" and   STUFF((Select ','+ t.OrderNo FROM 
((select o.OrderNo from lmshop_OrderInvoice g left join lmshop_GoodsOrder o WITH(NOLOCK) on g.OrderId=o.OrderId where g.InvoiceId=i.InvoiceId )) t FOR XML PATH('')),1,1,'') in (" + sqlAgOrders + ") ";
            }
            sql += " order by AcceptedTime,ordernos ";
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, param))
            {

                while (rdr.Read())
                {
                    var info = new InvoiceInfo(rdr.GetGuid(0), rdr.IsDBNull(1) ? "" : rdr.GetString(1), rdr.IsDBNull(2) ? "" : rdr.GetString(2)
                        , rdr.IsDBNull(3) ? DateTime.MinValue : rdr.GetDateTime(3), rdr.IsDBNull(4) ? 0 : float.Parse(rdr.GetValue(4).ToString()), rdr.IsDBNull(5) ? 0 : rdr.GetInt32(5), rdr.IsDBNull(6) ? "" : rdr.GetString(6))
                    {
                        InvoiceNo = rdr["InvoiceNo"] != DBNull.Value ? Convert.ToInt64(rdr["InvoiceNo"]) : 0,
                        InvoiceCode = rdr["InvoiceCode"].ToString(),
                        InvoiceChCode = rdr["InvoiceChCode"].ToString(),
                        InvoiceChNo = rdr["InvoiceChNo"] != DBNull.Value ? Convert.ToInt64(rdr["InvoiceChNo"]) : 0,
                        IsCommit = rdr["IsCommit"] != DBNull.Value && Convert.ToBoolean(rdr["IsCommit"]),
                        NoteType = (InvoiceNoteType)(int.Parse(rdr["NoteType"].ToString())),
                        TaxpayerID = rdr["TaxpayerID"].ToString()
                    };
                    ilist.Add(info);
                }
                rdr.Close();

            }
            return ilist;
        }

        /// <summary>
        /// 获取所在发票所在公司
        /// </summary>
        /// <param name="invoiceID">发票ID</param>
        /// <returns>公司id</returns>
        public string GetOrderFilieIdByInvoiceID(Guid invoiceID)
        {
            var parm = new SqlParameter("@InvoiceId", SqlDbType.UniqueIdentifier) { Value = invoiceID };
            using (
                var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true,
                                                            S_QL_SELECT_FILIALE_ID_BY_INVOICE_ID, parm))
            {
                if (rdr.Read())
                {
                    return rdr.GetGuid(0).ToString();
                }
                return "";
            }
        }

        /// <summary>
        /// 新增领取的发票卷
        /// </summary>
        /// <param name="roll"></param>
        /// <returns></returns>
        public bool InsertRoll(InvoiceRoll roll)
        {
            const string SQL = "INSERT INTO [lmShop_InvoiceRoll] VALUES (@Id,@Receiptor,@CreateTime,@InvoiceCode,@InvoiceStartNo,@InvoiceEndNo,@InvoiceCount,@InvoiceRollCount,@FilialeId)";
            var sqlParams = new[]
                                {
                                    new SqlParameter("@Id", roll.Id),
                                    new SqlParameter("@Receiptor", roll.Receiptor),
                                    new SqlParameter("@CreateTime", roll.CreateTime),
                                    new SqlParameter("@InvoiceCode", roll.InvoiceCode),
                                    new SqlParameter("@InvoiceStartNo", roll.InvoiceStartNo),
                                    new SqlParameter("@InvoiceEndNo", roll.InvoiceEndNo),
                                    new SqlParameter("@InvoiceCount", roll.InvoiceCount),
                                    new SqlParameter("@InvoiceRollCount", roll.InvoiceRollCount),
                                    new SqlParameter("@FilialeId",roll.FilialeId)
                                };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlParams) > 0;
        }

        /// <summary>
        /// 编辑购买的发票卷
        /// </summary>
        /// <param name="roll"></param>
        /// <returns></returns>
        public bool UpdateRoll(InvoiceRoll roll)
        {
            const string SQL = "UPDATE [lmShop_InvoiceRoll] SET Receiptor=@Receiptor,CreateTime=@CreateTime,InvoiceCode=@InvoiceCode,InvoiceStartNo=@InvoiceStartNo,InvoiceEndNo=@InvoiceEndNo,InvoiceCount=@InvoiceCount,InvoiceRollCount=@InvoiceRollCount, FilialeId=@FilialeId WHERE Id=@Id";
            var sqlParams = new[]
                                {
                                    new SqlParameter("@Id", roll.Id),
                                    new SqlParameter("@Receiptor", roll.Receiptor),
                                    new SqlParameter("@CreateTime", roll.CreateTime),
                                    new SqlParameter("@InvoiceCode", roll.InvoiceCode),
                                    new SqlParameter("@InvoiceStartNo", roll.InvoiceStartNo),
                                    new SqlParameter("@InvoiceEndNo", roll.InvoiceEndNo),
                                    new SqlParameter("@InvoiceCount", roll.InvoiceCount),
                                    new SqlParameter("@InvoiceRollCount", roll.InvoiceRollCount),
                                    new SqlParameter("@FilialeId",roll.FilialeId),
                                };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlParams) > 0;
        }

        /// <summary>
        /// 新增领取的发票卷
        /// </summary>
        /// <param name="rollDetail"></param>
        /// <returns></returns>
        public bool InsertRollDetail(InvoiceRollDetail rollDetail)
        {
            const string SQL = @"INSERT INTO [lmShop_InvoiceRollDetail](RollId,StartNo,EndNo,State,Remark) VALUES (@RollId,@StartNo,@EndNo,@State,@Remark)";
            var sqlParams = new[]
                                {
                                    new SqlParameter("@RollId",rollDetail.RollId),
                                    new SqlParameter("@StartNo",rollDetail.StartNo),
                                    new SqlParameter("@EndNo",rollDetail.EndNo),
                                    new SqlParameter("@State",rollDetail.State),
                                    new SqlParameter("@Remark",rollDetail.Remark)
                                };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlParams) > 0;
        }

        /// <summary>
        /// 删除指定发票号ID下的所有发票
        /// </summary>
        /// <param name="rollId"></param>
        /// <returns></returns>
        public bool DeleteRollDetail(Guid rollId)
        {
            const string SQL = @"DELETE FROM [lmShop_InvoiceRollDetail] WHERE RollId=@RollId";
            var sqlParams = new[]
                                {
                                    new SqlParameter("@RollId",rollId)
                                };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlParams) > 0;
        }

        /// <summary>
        /// 新增领取的发票卷
        /// </summary>
        /// <returns></returns>
        public IList<InvoiceRoll> GetRollList()
        {
            IList<InvoiceRoll> rollList = new List<InvoiceRoll>();
            const string SQL = @"SELECT Id,Receiptor,CreateTime,InvoiceCode,InvoiceStartNo,InvoiceEndNo,InvoiceCount,InvoiceRollCount,FilialeId FROM [lmShop_InvoiceRoll] ORDER BY CreateTime DESC";
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL))
            {
                while (dr.Read())
                {
                    rollList.Add(new InvoiceRoll
                    {
                        CreateTime = dr["CreateTime"].ToDateTime(),
                        Id = dr["Id"].ToGuid(),
                        InvoiceCode = dr["InvoiceCode"].ToString(),
                        InvoiceStartNo = dr["InvoiceStartNo"] != DBNull.Value ? Convert.ToInt64(dr["InvoiceStartNo"]) : 0,
                        InvoiceEndNo = dr["InvoiceEndNo"] != DBNull.Value ? Convert.ToInt64(dr["InvoiceEndNo"]) : 0,
                        InvoiceCount = dr["InvoiceCount"].ToInt(),
                        InvoiceRollCount = dr["InvoiceRollCount"].ToInt(),
                        Receiptor = dr["Receiptor"].ToString(),
                        FilialeId = dr["FilialeId"].ToGuid()
                    });
                }
            }
            return rollList;
        }

        /// <summary>
        /// 新增领取的发票卷信息
        /// </summary>
        /// <returns></returns>
        public IList<InvoiceRollDetail> GetRollDetailList(Guid rollId)
        {
            IList<InvoiceRollDetail> rollDetailList = new List<InvoiceRollDetail>();
            const string SQL = @"SELECT RollId,StartNo,EndNo,State,Remark,ISNULL(IsSubmit,0) AS IsSubmit FROM [lmShop_InvoiceRollDetail] WHERE RollId = @RollId ORDER BY StartNo ASC";
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, new SqlParameter("@RollId", rollId)))
            {
                while (dr.Read())
                {
                    rollDetailList.Add(new InvoiceRollDetail
                    {
                        EndNo = dr["EndNo"] != DBNull.Value ? Convert.ToInt64(dr["EndNo"]) : 0,
                        State = (InvoiceRollState)dr["State"],
                        Remark = dr["Remark"].ToString(),
                        RollId = dr["RollId"].ToGuid(),
                        StartNo = dr["StartNo"] != DBNull.Value ? Convert.ToInt64(dr["StartNo"]) : 0,
                        IsSubmit = bool.Parse(dr["IsSubmit"].ToString() == "0" ? "false" : "true")
                    });
                }
            }
            return rollDetailList;
        }

        /// <summary>
        /// 合计发票卷状态
        /// </summary>
        /// <param name="rollId"></param>
        /// <returns></returns>
        public int SumRollDeatilState(Guid rollId)
        {
            const string SQL = @"SELECT SUM([State]) FROM [lmShop_InvoiceRollDetail] WHERE RollId = @RollId";
            var value = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL, new SqlParameter("@RollId", rollId));
            if (value != null)
            {
                return value.ToInt();
            }
            return -1;
        }

        /// <summary>
        /// 分发发票卷
        /// </summary>
        /// <param name="rollId"></param>
        /// <param name="startNo"></param>
        /// <param name="endNo"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool DistributeInvoiceRoll(Guid rollId, long startNo, long endNo, string remark)
        {
            const string SQL =
                "UPDATE [lmShop_InvoiceRollDetail] SET [State]=@State,Remark=ISNULL(Remark,'')+@Remark WHERE RollId = @RollId AND StartNo=@StartNo AND EndNo=@EndNo AND [State]=0";
            var sqlParams = new[]
                                {
                                    new SqlParameter("@State",InvoiceRollState.Distribute),
                                    new SqlParameter("@RollId",rollId),
                                    new SqlParameter("@StartNo",startNo),
                                    new SqlParameter("@EndNo",endNo),
                                    new SqlParameter("@Remark",remark)
                                };
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlParams) > 0;
        }

        /// <summary>
        /// 发票报送到
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="filialeId"></param>
        public void InvoiceCommit(DateTime startDate, DateTime endDate, Guid filialeId)
        {
            const string SQL = @"
UPDATE [lmShop_Invoice] SET IsCommit=1
FROM(
	SELECT 
		IV.InvoiceId AS ID,
		IsCommit,
		G.OrderId,
		G.SaleFilialeId
	FROM lmShop_Invoice IV
	LEFT JOIN lmshop_OrderInvoice OI on OI.InvoiceId=IV.InvoiceId
	LEFT JOIN lmShop_GoodsOrder G on G.OrderId=oi.OrderId
	WHERE IV.AcceptedTime>=@StartDate AND IV.AcceptedTime<=@EndDate AND IV.InvoiceState IN (2,4,5) AND G.SaleFilialeId=@SaleFilialeId 
) A
WHERE InvoiceId=A.ID";
            var sqlParams = new[]
                                {
                                    new SqlParameter("@StartDate",startDate),
                                    new SqlParameter("@EndDate",endDate),
                                    new SqlParameter("@SaleFilialeId",filialeId)
                                };
            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlParams);
        }

        /// <summary>
        /// 遗失上报
        /// </summary>
        public void LostSubmit(Guid rollId, long startNo, long endNo, InvoiceRollState state)
        {
            const string SQL =
                @"UPDATE [lmShop_InvoiceRollDetail] SET IsSubmit=1 WHERE RollId = @RollId AND StartNo=@StartNo AND EndNo=@EndNo AND [State]=@State";
            var sqlParams = new[]
                                {
                                    new SqlParameter("@State",state),
                                    new SqlParameter("@RollId",rollId),
                                    new SqlParameter("@StartNo",startNo),
                                    new SqlParameter("@EndNo",endNo)
                                };
            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlParams);
        }

        /// <summary>
        /// 指定状态获取发票卷信息
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<InvoiceRollDetail> GetRollDetailListByState(InvoiceRollState state)
        {
            const string SQL = @"
SELECT 
	RollId,IR.InvoiceCode,StartNo,EndNo 
FROM lmShop_InvoiceRollDetail IRD
INNER JOIN lmShop_InvoiceRoll IR ON IR.Id = IRD.RollId
WHERE IRD.[State]=@State AND ISNULL(IsSubmit,0)<>1 ";

            IList<InvoiceRollDetail> rollDetailList = new List<InvoiceRollDetail>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, new SqlParameter("@State", state)))
            {
                while (dr.Read())
                {
                    rollDetailList.Add(new InvoiceRollDetail
                    {
                        EndNo = dr["EndNo"] != DBNull.Value ? Convert.ToInt64(dr["EndNo"]) : 0,
                        InvoiceCode = dr["InvoiceCode"].ToString(),
                        RollId = dr["RollId"].ToGuid(),
                        StartNo = dr["StartNo"] != DBNull.Value ? Convert.ToInt64(dr["StartNo"]) : 0
                    });
                }
            }
            return rollDetailList;
        }

        /// <summary>
        /// 删除指定的发票卷
        /// </summary>
        public void DeleteRollDetail(Guid rollId, long startNo, long endNo, InvoiceRollState state)
        {
            const string SQL = @"DELETE FROM [lmShop_InvoiceRollDetail] WHERE RollId=@RollId AND StartNo=@StartNo AND EndNo=@EndNo AND [State]<>@State";
            var sqlParams = new[]
                                {
                                    new SqlParameter("@State",state),
                                    new SqlParameter("@RollId",rollId),
                                    new SqlParameter("@StartNo",startNo),
                                    new SqlParameter("@EndNo",endNo)
                                };
            SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, sqlParams);
        }

        /// <summary>
        /// 发票汇总搜索
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="noteType"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="pageSize"> </param>
        /// <param name="pageIndex"> </param>
        /// <param name="recordCount"> </param>
        /// <returns></returns>
        public IList<InvoiceBriefInfo> Search(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo, int pageSize, int pageIndex, out int recordCount)
        {
            var sql = new StringBuilder(@"
SELECT 
		distinct		
		IV.InvoiceId,
		IV.InvoiceName AS InvoicePayerName,
		IV.RequestTime AS CreateDate,
		IV.InvoiceSum AS TotalMoney,
		IV.InvoiceState AS [State],
		IV.InvoiceCode,
		IV.Address,
		IV.InvoiceContent,
		IV.IsCommit,
		IV.PurchaserType,
		IV.TaxpayerID,
		IV.InvoiceNo, 
		IV.InvoiceChCode AS RetreatInvoiceCode,
		IV.InvoiceChNo AS RetreatInvoiceNo,
		IV.NoteType,
		IV.AcceptedTime AS PrintDate,
		IV.SaleFilialeId,
	OI.OrderNo AS OrderNo
FROM lmShop_Invoice IV WITH(NOLOCK)
LEFT JOIN lmshop_OrderInvoice OI WITH(NOLOCK)
ON OI.InvoiceId=IV.InvoiceId ");
            sql.Append(" WHERE IV.AcceptedTime >= @STime AND IV.AcceptedTime < @ETime ");
            var paramList = new List<Parameter>
                          {
                                    new Parameter("@STime", startTime),
                                    new Parameter("@ETime", endTime)
                          };
            if (noteType > -1)
            {
                sql.Append(" AND IV.NoteType = @NoteType ");
                paramList.Add(new Parameter("@NoteType", noteType));
            }

            if (!string.IsNullOrEmpty(invoiceNo))
            {
                sql.Append(" AND IV.InvoiceNo=@InvoiceNo ");
                paramList.Add(new Parameter("@InvoiceNo", invoiceNo));
            }

            if (filialeId != Guid.Empty)
            {
                sql.Append(" AND IV.SaleFilialeId=@SaleFilialeId ");
                paramList.Add(new Parameter("@SaleFilialeId", filialeId));
            }

            using (var db = DatabaseFactory.Create())
            {
                var pageQuery = new PageQuery(pageIndex, pageSize, sql.ToString(), " InvoiceNo DESC");
                var pageItem = db.SelectByPage<InvoiceBriefInfo>(true, pageQuery, paramList.ToArray());
                recordCount = (int)pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }

        public IList<InvoiceNoteStatisticsInfo> InvoiceNoteStatistics(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo)
        {
            StringBuilder sb = new StringBuilder(@"
SELECT 
(
 CASE
    I1.NoteType  
         WHEN  0 THEN '0'
         WHEN  1 THEN '1'
         WHEN  2 THEN '2'
 END  
) AS NoteType,SUM(I1.TotalMoney) AS TotalMoney,
COUNT(NoteType) AS NoteTypeCount
 FROM (
	SELECT 
				DISTINCT		
				IV.InvoiceId,
				IV.InvoiceName AS InvoicePayerName,
				IV.RequestTime AS CreateDate,
				IV.InvoiceSum AS TotalMoney,
				IV.InvoiceState AS [State],
				IV.InvoiceCode,
				IV.[Address],
				IV.InvoiceContent,
				IV.IsCommit,
				IV.PurchaserType,
				IV.TaxpayerID,
				IV.InvoiceNo,
				IV.InvoiceChCode AS RetreatInvoiceCode,
				IV.InvoiceChNo AS RetreatInvoiceNo,
				IV.NoteType,
				IV.AcceptedTime AS PrintDate,
				IV.SaleFilialeId
		FROM lmShop_Invoice IV WITH(NOLOCK)
		LEFT JOIN lmshop_OrderInvoice OI WITH(NOLOCK)
		ON OI.InvoiceId=IV.InvoiceId 
		WHERE IV.AcceptedTime >= @STime 
		AND IV.AcceptedTime < @ETime
        AND IV.NoteType>-1 
");
            if (noteType > -1)
                sb.Append(" AND IV.NoteType = @NoteType ");
            if (!string.IsNullOrEmpty(invoiceNo))
            {
                sb.Append(" AND IV.InvoiceNo=@InvoiceNo ");
            }
            if (filialeId != Guid.Empty)
            {
                sb.Append(" AND IV.SaleFilialeId=@SaleFilialeId ");
            }
            sb.Append(@") I1	
GROUP BY (
 CASE
    I1.NoteType  
         WHEN  0 THEN '0'
         WHEN  1 THEN '1'
         WHEN  2 THEN '2'
 END  
)");
            var sqlParams = new[]
                                {
                                    new SqlParameter("@STime", startTime),
                                    new SqlParameter("@ETime", endTime),
                                    new SqlParameter("@NoteType", noteType),
                                    new SqlParameter("@InvoiceNo", invoiceNo),
                                    new SqlParameter("@SaleFilialeId", filialeId)
                                };
            IList<InvoiceNoteStatisticsInfo> list = new List<InvoiceNoteStatisticsInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sb.ToString(), sqlParams))
            {
                while (dr.Read())
                {
                    InvoiceNoteStatisticsInfo info = new InvoiceNoteStatisticsInfo
                    {
                        NoteType = (InvoiceNoteType)Convert.ToInt32(dr["NoteType"]),
                        TotalMoney = float.Parse(dr["TotalMoney"].ToString()),
                        TotalQuantity = Convert.ToInt32(dr["NoteTypeCount"])
                    };
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>发票汇总导出Excel专用
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="noteType"></param>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        public IList<InvoiceBriefInfo> OutPutExcelInvoice(DateTime startTime, DateTime endTime, Guid filialeId, int noteType, string invoiceNo)
        {
            var sql = new StringBuilder(@"
SELECT 
		distinct		
		IV.InvoiceId,
		IV.InvoiceName AS InvoicePayerName,
		IV.RequestTime AS CreateDate,
		IV.InvoiceSum AS TotalMoney,
		IV.InvoiceState AS [State],
		IV.InvoiceCode,
		IV.Address,
		IV.InvoiceContent,
		IV.IsCommit,
		IV.PurchaserType,
		IV.TaxpayerID,
		IV.InvoiceNo, 
		IV.InvoiceChCode AS RetreatInvoiceCode,
		IV.InvoiceChNo AS RetreatInvoiceNo,
		IV.NoteType,
		IV.AcceptedTime AS PrintDate,
		IV.SaleFilialeId,
	dbo.fun_getOrderNosByInvoiceId(IV.InvoiceId) AS OrderNo
FROM lmShop_Invoice IV WITH(NOLOCK)
LEFT JOIN lmshop_OrderInvoice OI WITH(NOLOCK)
ON OI.InvoiceId=IV.InvoiceId ");
            sql.Append(" WHERE IV.AcceptedTime >= @STime AND IV.AcceptedTime < @ETime AND IV.NoteType>-1");
            if (noteType > -1)
            {
                sql.Append(" AND IV.NoteType = @NoteType ");
            }
            if (!string.IsNullOrEmpty(invoiceNo))
            {
                sql.Append(" AND IV.InvoiceNo=@InvoiceNo ");
            }
            if (filialeId != Guid.Empty)
            {
                sql.Append(" AND IV.SaleFilialeId=@SaleFilialeId ");
            }
            const string ORDER_BY = " ORDER BY IV.InvoiceNo DESC";
            sql.Append(ORDER_BY);
            var sqlParams = new[]
                                {
                                    new SqlParameter("@STime", startTime),
                                    new SqlParameter("@ETime", endTime),
                                    new SqlParameter("@NoteType", noteType),
                                    new SqlParameter("@InvoiceNo", invoiceNo),
                                    new SqlParameter("@SaleFilialeId", filialeId)
                                };
            IList<InvoiceBriefInfo> list = new List<InvoiceBriefInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), sqlParams))
            {
                while (dr.Read())
                {
                    list.Add(mapping(dr));
                }
            }
            return list;
        }

        private InvoiceBriefInfo mapping(IDataReader dr)
        {
            var info = new InvoiceBriefInfo
            {
                CreateDate = dr["CreateDate"].ToString().ToDateTime(),
                InvoiceCode = dr["InvoiceCode"].ToString(),
                InvoiceId = dr["InvoiceId"].ToString().ToGuid(),
                InvoiceNo = long.Parse(dr["InvoiceNo"].ToString()),
                InvoicePayerName = dr["InvoicePayerName"].ToString(),
                NoteType = (InvoiceNoteType)dr["NoteType"],
                //OrderId = ConvertHelper.To<Guid>(dr["OrderId"]),
                OrderNo = dr["OrderNo"].ToString(),
                PrintDate = dr["PrintDate"] != DBNull.Value ? dr["PrintDate"].ToString().ToDateTime() : DateTime.MinValue,
                RetreatInvoiceCode = dr["RetreatInvoiceCode"] != DBNull.Value ? dr["RetreatInvoiceCode"].ToString() : "-",
                RetreatInvoiceNo = dr["RetreatInvoiceNo"] != DBNull.Value ? long.Parse(dr["RetreatInvoiceNo"].ToString()) : 0,
                State = (InvoiceState)dr["State"],
                Address = dr["State"].ToString(),
                InvoiceContent = dr["InvoiceContent"].ToString(),
                IsCommit = (bool)dr["IsCommit"],
                PurchaserType = (InvoicePurchaserType)dr["PurchaserType"],
                TaxpayerID = dr["TaxpayerID"].ToString(),
                TotalMoney = float.Parse(dr["TotalMoney"].ToString()),
                SaleFilialeId = dr["SaleFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["SaleFilialeId"].ToString())
            };
            return info;
        }

        #region [修改]
        /// <summary>
        /// 修改一条发票记录
        /// </summary>
        /// <param name="invoice">发票信息</param>
        public void Update(InvoiceInfo invoice)
        {
            SqlParameter[] parms = GetParameters();
            parms[0].Value = invoice.InvoiceId;
            parms[1].Value = invoice.MemberId;
            parms[2].Value = invoice.InvoiceName;
            parms[3].Value = invoice.InvoiceContent;
            parms[4].Value = invoice.Receiver;
            parms[5].Value = invoice.PostalCode;
            parms[6].Value = string.IsNullOrEmpty(invoice.Address) ? DBNull.Value : (object)invoice.Address;
            parms[7].Value = invoice.RequestTime;
            parms[8].Value = invoice.InvoiceSum;
            parms[9].Value = invoice.InvoiceState;
            parms[10].Value = (int)invoice.PurchaserType;
            parms[11].Value = invoice.TaxpayerID;
            parms[12].Value = invoice.CancelPersonel;
            parms[13].Value = invoice.SaleFilialeId;
            parms[14].Value = invoice.IsAfterwardsApply;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_INVOICE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 设置发票状态
        /// </summary>
        /// <param name="invoiceId">发票编号</param>
        /// <param name="invoiceState">发票状态</param>
        public void SetInvoiceState(Guid invoiceId, InvoiceState invoiceState)
        {
            var parms = new[] {
                    new SqlParameter(PARM_INVOICEID, SqlDbType.UniqueIdentifier),
                    new SqlParameter(PARM_INVOICESTATE, SqlDbType.Int)
                };
            parms[0].Value = invoiceId;
            parms[1].Value = invoiceState;

            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, SQL_UPDATE_INVOICE_BY_INVOICESTATE_SERVER, parms);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        /// <summary>
        /// 修改发票金额--不作废,只是修改
        /// </summary>
        /// <param name="invoiceId">发票Id</param>
        /// <param name="invoiceSum">金额</param>
        public void SetInvoiceSum(Guid invoiceId, float invoiceSum)
        {
            var parms = new[] {
                    new SqlParameter(PARM_INVOICEID, SqlDbType.UniqueIdentifier),
                    new SqlParameter(PARM_INVOICESUM, SqlDbType.Float)
                };

            parms[0].Value = invoiceId;
            parms[1].Value = invoiceSum;

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_INVOICE_INVOICESUM, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="invoiceInfo"></param>
        public void UpdateInvoice(InvoiceInfo invoiceInfo)
        {
            const string SQL = @"UPDATE lmShop_Invoice SET InvoiceName=@InvoiceName,InvoiceContent=@InvoiceContent,
[Address]=@Address,InvoiceCode=@InvoiceCode,InvoiceNo=@InvoiceNo,NoteType=@NoteType,InvoiceSum=@InvoiceSum,
Receiver=@Receiver,RequestTime=@RequestTime, AcceptedTime=@AcceptedTime,SaleFilialeId=@SaleFilialeId WHERE InvoiceId=@InvoiceId";
            var parms = new[] {
                    new SqlParameter("@InvoiceName",invoiceInfo.InvoiceName),
                    new SqlParameter("@InvoiceContent", invoiceInfo.InvoiceContent),
                    new SqlParameter("@Address", invoiceInfo.Address),
                    new SqlParameter("@InvoiceCode", invoiceInfo.InvoiceCode),
                    new SqlParameter("@InvoiceNo", invoiceInfo.InvoiceNo),
                    new SqlParameter("@NoteType", invoiceInfo.NoteType),
                    new SqlParameter("@InvoiceSum", invoiceInfo.InvoiceSum),
                    new SqlParameter("@Receiver", invoiceInfo.Receiver),
                    new SqlParameter("@RequestTime", invoiceInfo.RequestTime),
                    new SqlParameter("@AcceptedTime", invoiceInfo.AcceptedTime==DateTime.MinValue?(object)DBNull.Value:invoiceInfo.AcceptedTime),
                    new SqlParameter("@SaleFilialeId", invoiceInfo.SaleFilialeId),
                    new SqlParameter("@InvoiceId", invoiceInfo.InvoiceId)
            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 根据OrderId删除lmShop_Invoice
        /// </summary>
        /// <param name="orderId"></param>
        public void DeleteInvoiceByOrderId(Guid orderId)
        {
            var parms = new[] { new SqlParameter("@orderId", SqlDbType.UniqueIdentifier) };
            parms[0].Value = orderId;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_INVOICE_BY_ORDERID, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Func : 根据订单号，获取指定的发票索取记录
        /// Code : dyy
        /// Date : 2009 Nov 26th
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public InvoiceInfo GetInvoiceByGoodsOrder(Guid orderId)
        {
            var parm = new SqlParameter(PARM_ORDERID, SqlDbType.UniqueIdentifier) { Value = orderId };
            InvoiceInfo invoiceInfo;
            try
            {
                using (
                    var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true,
                                                                SQL_SELECT_INVOICE_BY_GOODS_ORDER, parm))
                {
                    if (rdr.Read())
                    {
                        invoiceInfo = new InvoiceInfo
                        {
                            InvoiceId = rdr["InvoiceId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["InvoiceId"].ToString()),
                            MemberId = rdr["MemberId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["MemberId"].ToString()),
                            InvoiceName = rdr["InvoiceName"] == DBNull.Value ? string.Empty : rdr["InvoiceName"].ToString(),
                            InvoiceContent = rdr["InvoiceContent"] == DBNull.Value ? string.Empty : rdr["InvoiceContent"].ToString(),
                            Receiver = rdr["Receiver"] == DBNull.Value ? string.Empty : rdr["Receiver"].ToString(),
                            PostalCode = rdr["PostalCode"] == DBNull.Value ? string.Empty : rdr["PostalCode"].ToString(),
                            Address = rdr["Address"] == DBNull.Value ? string.Empty : rdr["Address"].ToString(),
                            RequestTime = rdr["RequestTime"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(rdr["RequestTime"].ToString()),
                            InvoiceSum = rdr["InvoiceSum"] == DBNull.Value ? 0 : double.Parse(rdr["InvoiceSum"].ToString()),
                            AcceptedTime = rdr["AcceptedTime"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(rdr["AcceptedTime"].ToString())
                        };
                        string purchaserType = rdr["PurchaserType"] == DBNull.Value ? "0" : rdr["PurchaserType"].ToString();
                        invoiceInfo.InvoiceNo = rdr["InvoiceNo"] == DBNull.Value ? 0 : long.Parse(rdr["InvoiceNo"].ToString());
                        invoiceInfo.IsCommit = rdr["IsCommit"] != DBNull.Value && bool.Parse(rdr["IsCommit"].ToString());
                        invoiceInfo.PurchaserType = (InvoicePurchaserType)int.Parse(purchaserType);
                        invoiceInfo.SaleFilialeId = rdr["SaleFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SaleFilialeId"].ToString());
                        invoiceInfo.InvoiceState = Convert.ToInt32(rdr["InvoiceState"]);
                        invoiceInfo.IsAfterwardsApply = rdr["IsAfterwardsApply"] != DBNull.Value && Convert.ToBoolean(rdr["IsAfterwardsApply"]);
                    }
                    else
                    {
                        invoiceInfo = new InvoiceInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return invoiceInfo;
        }


        /// <summary>
        /// 根据订单号，发票状态，订单是否完成发货，发票是否提交，开始时间，截止时间获取发票集合。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public IList<SimpleInvoiceDetailInfo> GetInvoiceList(string orderNo, byte invoiceState, bool isFinished, bool isCommit, DateTime fromTime, DateTime toTime)
        {
            var sql = new StringBuilder(@" select a.InvoiceId,a.InvoiceName,a.Receiver,a.InvoiceContent,a.PostalCode,a.InvoiceSum,c.OrderNo,a.InvoiceState,a.[Address],a.InvoiceCode,a.InvoiceNo,a.IsCommit
                                              from lmShop_Invoice a
                                              inner join lmshop_OrderInvoice b
                                              on a.InvoiceId=b.InvoiceId
                                              inner join lmShop_GoodsOrder c
                                              on b.OrderId = c.OrderId
                                              where 1=1");
            if (!string.IsNullOrEmpty(orderNo))
            {
                sql.Append(" AND c.OrderNo = '").Append(orderNo).Append("'");
            }
            if (invoiceState != (int)InvoiceState.All)
                sql.Append(" AND a.InvoiceState=").Append((int)invoiceState);

            //add by dyy at 2009 Nov 26th 只显示已完成的订单
            sql.Append(" AND a.IsCommit = ").Append(isCommit ? 1 : 0);

            if (isFinished)
            {
                sql.Append(" AND c.OrderState=9");
            }

            if (fromTime != DateTime.MinValue)
            {
                sql.Append(" AND a.RequestTime >= '").Append(fromTime).Append("'");
            }
            if (toTime != DateTime.MinValue)
            {
                sql.Append(" AND a.RequestTime <= '").Append(toTime).Append("'");
            }

            IList<SimpleInvoiceDetailInfo> simpleInvoiceDetailList = new List<SimpleInvoiceDetailInfo>();

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql.ToString(), null))
            {
                while (rdr.Read())
                {
                    var simpleInvoiceDetailInfo = new SimpleInvoiceDetailInfo
                    {
                        InvoiceId = rdr["InvoiceId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["InvoiceId"].ToString()),
                        InvoiceName = rdr["InvoiceName"] == DBNull.Value ? "" : rdr["InvoiceName"].ToString(),
                        Receiver = rdr["Receiver"] == DBNull.Value ? "" : rdr["Receiver"].ToString(),
                        InvoiceContent = rdr["InvoiceContent"] == DBNull.Value ? "" : rdr["InvoiceContent"].ToString(),
                        PostalCode = rdr["PostalCode"] == DBNull.Value ? "" : rdr["PostalCode"].ToString(),
                        InvoiceSum = rdr["InvoiceSum"] == DBNull.Value ? 0 : double.Parse(rdr["InvoiceSum"].ToString()),
                        OrderNo = rdr["OrderNo"] == DBNull.Value ? "" : rdr["OrderNo"].ToString(),
                        InvoiceState = Convert.ToInt32(rdr["InvoiceState"]),
                        Address = rdr["Address"] == DBNull.Value ? "" : rdr["Address"].ToString(),
                        InvoiceCode = rdr["InvoiceCode"] != DBNull.Value ? rdr["InvoiceCode"].ToString() : "-",
                        InvoiceNo = rdr["InvoiceNo"] != DBNull.Value ? (long)rdr["InvoiceNo"] : 0,
                        IsCommit = rdr["IsCommit"] != DBNull.Value && (bool)rdr["IsCommit"],
                    };
                    simpleInvoiceDetailList.Add(simpleInvoiceDetailInfo);
                }
            }
            return simpleInvoiceDetailList;
        }

        /// <summary>
        /// 根据发票ID获取发票信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public SimpleInvoiceDetailInfo GetInvoiceInfo(Guid invoiceId)
        {
            const string SQL = @"select a.InvoiceId,a.InvoiceName,a.Receiver,a.InvoiceContent,a.PostalCode,a.InvoiceSum,c.OrderNo,a.InvoiceState,a.[Address],a.InvoiceCode,a.InvoiceNo,a.IsCommit
                                              from lmShop_Invoice a
                                              inner join lmshop_OrderInvoice b
                                              on a.InvoiceId=b.InvoiceId
                                              inner join lmShop_GoodsOrder c
                                              on b.OrderId = c.OrderId
                                              where a.InvoiceId = @InvoiceId";
            var parm = new SqlParameter("@InvoiceId", SqlDbType.UniqueIdentifier) { Value = invoiceId };
            SimpleInvoiceDetailInfo simpleInvoiceDetailInfo = null;
            try
            {
                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
                {
                    if (rdr.Read())
                    {
                        simpleInvoiceDetailInfo = new SimpleInvoiceDetailInfo
                        {
                            InvoiceId = rdr["InvoiceId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["InvoiceId"].ToString()),
                            InvoiceName = rdr["InvoiceName"] == DBNull.Value ? "" : rdr["InvoiceName"].ToString(),
                            Receiver = rdr["Receiver"] == DBNull.Value ? "" : rdr["Receiver"].ToString(),
                            InvoiceContent = rdr["InvoiceContent"] == DBNull.Value ? "" : rdr["InvoiceContent"].ToString(),
                            PostalCode = rdr["PostalCode"] == DBNull.Value ? "" : rdr["PostalCode"].ToString(),
                            InvoiceSum = rdr["InvoiceSum"] == DBNull.Value ? 0 : double.Parse(rdr["InvoiceSum"].ToString()),
                            OrderNo = rdr["OrderNo"] == DBNull.Value ? "" : rdr["OrderNo"].ToString(),
                            InvoiceState = Convert.ToInt32(rdr["InvoiceState"]),
                            Address = rdr["Address"] == DBNull.Value ? "" : rdr["Address"].ToString(),
                            InvoiceCode = rdr["InvoiceCode"] != DBNull.Value ? rdr["InvoiceCode"].ToString() : "-",
                            InvoiceNo = rdr["InvoiceNo"] != DBNull.Value ? (long)rdr["InvoiceNo"] : 0,
                            IsCommit = rdr["IsCommit"] != DBNull.Value && (bool)rdr["IsCommit"]
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return simpleInvoiceDetailInfo;
        }

        /// <summary>
        /// 根据发票号码获取发票信息
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public SimpleInvoiceDetailInfo GetInvoiceInfo(long invoiceNo)
        {
            const string SQL = @"select a.InvoiceId,a.InvoiceName,a.Receiver,a.InvoiceContent,a.PostalCode,a.InvoiceSum,c.OrderNo,a.InvoiceState,a.[Address],a.InvoiceCode,a.InvoiceNo,a.IsCommit
                                              from lmShop_Invoice a
                                              inner join lmshop_OrderInvoice b
                                              on a.InvoiceId=b.InvoiceId
                                              inner join lmShop_GoodsOrder c
                                              on b.OrderId = c.OrderId
                                              where a.InvoiceNo = @InvoiceNo";
            var parm = new SqlParameter("@InvoiceNo", SqlDbType.BigInt) { Value = invoiceNo };
            SimpleInvoiceDetailInfo simpleInvoiceDetailInfo = null;
            try
            {
                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true,SQL, parm))
                {
                    if (rdr.Read())
                    {
                        simpleInvoiceDetailInfo = new SimpleInvoiceDetailInfo
                        {
                            InvoiceId = rdr["InvoiceId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["InvoiceId"].ToString()),
                            InvoiceName = rdr["InvoiceName"] == DBNull.Value ? "" : rdr["InvoiceName"].ToString(),
                            Receiver = rdr["Receiver"] == DBNull.Value ? "" : rdr["Receiver"].ToString(),
                            InvoiceContent = rdr["InvoiceContent"] == DBNull.Value ? "" : rdr["InvoiceContent"].ToString(),
                            PostalCode = rdr["PostalCode"] == DBNull.Value ? "" : rdr["PostalCode"].ToString(),
                            InvoiceSum = rdr["InvoiceSum"] == DBNull.Value ? 0 : double.Parse(rdr["InvoiceSum"].ToString()),
                            OrderNo = rdr["OrderNo"] == DBNull.Value ? "" : rdr["OrderNo"].ToString(),
                            InvoiceState = Convert.ToInt32(rdr["InvoiceState"]),
                            Address = rdr["Address"] == DBNull.Value ? "" : rdr["Address"].ToString(),
                            InvoiceCode = rdr["InvoiceCode"] != DBNull.Value ? rdr["InvoiceCode"].ToString() : "-",
                            InvoiceNo = rdr["InvoiceNo"] != DBNull.Value ? (long)rdr["InvoiceNo"] : 0,
                            IsCommit = rdr["IsCommit"] != DBNull.Value && (bool)rdr["IsCommit"]
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return simpleInvoiceDetailInfo;
        }


        /// <summary>
        /// 获取发票品名的名称集合。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public List<string> GetInvoiceItem()
        {
            var builder = new StringBuilder(@"SELECT [ItemContent] FROM [lmShop_InvoiceItem]");
            var list = new List<string>();

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), null))
            {
                while (dr.Read())
                {
                    list.Add(dr.GetString(0));
                }
            }
            return list;
        }

        /// <summary>
        /// 根据发票ID更新发票状态，发票号，发票代码。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public bool UpdateInvoiceStateWithInvoiceNo(Guid invoiceId, byte invoiceState, long invoiceno, string invoicecode)
        {
            const string SQL = "UPDATE [lmShop_Invoice] SET InvoiceState=@InvoiceState,InvoiceNo=@InvoiceNo,InvoiceCode=@InvoiceCode WHERE InvoiceId=@InvoiceId";
            var parms = new[] {
                new SqlParameter("@InvoiceId",SqlDbType.UniqueIdentifier),
                new SqlParameter("@InvoiceState",SqlDbType.Int),
                new SqlParameter("@InvoiceNo",SqlDbType.BigInt),
                new SqlParameter("@InvoiceCode",SqlDbType.VarChar,128)
            };
            parms[0].Value = invoiceId;
            parms[1].Value = invoiceState;
            parms[2].Value = invoiceno;
            parms[3].Value = invoicecode;

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据发票卷详细表开始和截止号码，状态=分发
        /// 获取发票卷表发票卷代码，发票卷所属公司ID，
        /// </summary>
        /// <param name="startNo"></param>
        /// <param name="endNo"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public InvoiceRoll GetInvoiceRollByStartNoandEndNo(long startNo, long endNo)
        {
            const string SQL = @"select b.InvoiceCode,b.FilialeId from lmShop_InvoiceRollDetail a
                                    inner join lmShop_InvoiceRoll b
                                    on a.RollId = b.Id
                                    where a.StartNo=@StartNo
                                    and a.EndNo=@EndNo 
                                    and a.state=1";
            var parms = new[] {
                new SqlParameter("@StartNo",SqlDbType.BigInt),
                new SqlParameter("@EndNo",SqlDbType.BigInt)
            };
            parms[0].Value = startNo;
            parms[1].Value = endNo;

            InvoiceRoll invoiceRoll = null;
            try
            {
                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true,SQL, parms))
                {
                    if (rdr.Read())
                    {
                        invoiceRoll = new InvoiceRoll
                        {
                            InvoiceCode = rdr["InvoiceCode"] != DBNull.Value ? rdr["InvoiceCode"].ToString() : "",
                            FilialeId = rdr["FilialeId"] != DBNull.Value ? new Guid(rdr["FilialeId"].ToString()) : Guid.Empty,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return invoiceRoll;
        }

        /// <summary>
        /// 查询当前发票卷使用到的最大发票号
        /// 发票状态：已开
        /// </summary>
        /// <param name="invoiceStartNo"></param>
        /// <param name="invoiceEndNo"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-29
        public long GetInvoiceMaxInvoiceNoByInvoiceNo(long invoiceStartNo, long invoiceEndNo)
        {
            const string SQL = @"select max(InvoiceNo) from lmShop_Invoice where InvoiceNo>=@InvoiceStartNo and InvoiceNo<=@InvoiceEndNo and InvoiceState=2";
            var parms = new[]
                            {
                                new SqlParameter("@InvoiceStartNo", SqlDbType.BigInt),
                                new SqlParameter("@InvoiceEndNo", SqlDbType.BigInt)
                            };

            parms[0].Value = invoiceStartNo;
            parms[1].Value = invoiceEndNo;

            object result = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL, parms);
            if (result != DBNull.Value)
            {
                return long.Parse(result.ToString());
            }
            return 0;
        }


        /// <summary>
        /// 根据发票ID获取发票打印所属数据
        /// 状态为已开
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        public SimpleInvoiceInfo GetInvoicePrintData(Guid invoiceId)
        {
            const string SQL = @"select a.InvoiceName,a.InvoiceContent,a.InvoiceSum,c.OrderNo,a.InvoiceCode,a.InvoiceNo
                                              from lmShop_Invoice a
                                              inner join lmshop_OrderInvoice b
                                              on a.InvoiceId=b.InvoiceId
                                              inner join lmShop_GoodsOrder c
                                              on b.OrderId = c.OrderId
                                              where a.InvoiceId = @InvoiceId and InvoiceState=2";
            var parm = new SqlParameter("@InvoiceId", SqlDbType.UniqueIdentifier) { Value = invoiceId };
            SimpleInvoiceInfo simpleInvoiceInfo = null;
            try
            {
                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
                {
                    if (rdr.Read())
                    {
                        simpleInvoiceInfo = new SimpleInvoiceInfo
                        {
                            InvoiceName = rdr["InvoiceName"] == DBNull.Value ? "" : rdr["InvoiceName"].ToString(),
                            InvoiceContent = rdr["InvoiceContent"] == DBNull.Value ? "" : rdr["InvoiceContent"].ToString(),
                            InvoiceSum = rdr["InvoiceSum"] == DBNull.Value ? 0 : double.Parse(rdr["InvoiceSum"].ToString()),
                            OrderNo = rdr["OrderNo"] == DBNull.Value ? "" : rdr["OrderNo"].ToString(),
                            InvoiceCode = rdr["InvoiceCode"] != DBNull.Value ? rdr["InvoiceCode"].ToString() : "-",
                            InvoiceNo = rdr["InvoiceNo"] != DBNull.Value ? (long)rdr["InvoiceNo"] : 0,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return simpleInvoiceInfo;
        }


        /// <summary>
        /// 根据订单号取得发票信息
        /// </summary>
        /// <param name="orderId"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-30
        public List<SimpleInvoiceInfo> GetInvoiceByOrderId(Guid orderId)
        {
            const string SQL = @"select c.OrderNo,a.InvoiceName,a.InvoiceContent,a.InvoiceSum,a.InvoiceCode,a.InvoiceNo from lmShop_Invoice a
                                    inner join lmshop_OrderInvoice b
                                    on a.InvoiceId = b.InvoiceId
                                    inner join lmShop_GoodsOrder c
                                    on b.OrderId = c.OrderId
                                    where b.OrderId = @OrderId";
            var parm = new SqlParameter("@OrderId", SqlDbType.UniqueIdentifier) { Value = orderId };
            var simpleInvoiceList = new List<SimpleInvoiceInfo>();
            try
            {
                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
                {
                    while (rdr.Read())
                    {
                        var simpleInvoiceInfo = new SimpleInvoiceInfo
                        {
                            InvoiceName = rdr["InvoiceName"] == DBNull.Value ? "" : rdr["InvoiceName"].ToString(),
                            InvoiceContent = rdr["InvoiceContent"] == DBNull.Value ? "" : rdr["InvoiceContent"].ToString(),
                            InvoiceSum = rdr["InvoiceSum"] == DBNull.Value ? 0 : double.Parse(rdr["InvoiceSum"].ToString()),
                            OrderNo = rdr["OrderNo"] == DBNull.Value ? "" : rdr["OrderNo"].ToString(),
                            InvoiceCode = rdr["InvoiceCode"] != DBNull.Value ? rdr["InvoiceCode"].ToString() : "-",
                            InvoiceNo = rdr["InvoiceNo"] != DBNull.Value ? (long)rdr["InvoiceNo"] : 0,
                        };
                        simpleInvoiceList.Add(simpleInvoiceInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return simpleInvoiceList;
        }

        /// <summary>
        /// 根据订单号取得发票信息
        /// </summary>
        /// <param name="orderNo"></param>
        /// For WMS
        /// <returns></returns>
        public List<SimpleInvoiceDetailInfo> GetInvoiceByOrderNo(string orderNo)
        {
            const string SQL = @"select a.InvoiceId,a.InvoiceName,a.Receiver,a.InvoiceContent,a.PostalCode,a.InvoiceSum,a.InvoiceState,a.[Address],a.InvoiceCode,a.InvoiceNo,a.IsCommit from lmShop_Invoice a
                                    inner join lmshop_OrderInvoice b
                                    on a.InvoiceId = b.InvoiceId
                                    where b.OrderNo = @OrderNo";
            var parm = new SqlParameter("@OrderNo", SqlDbType.VarChar) { Value = orderNo };
            var simpleInvoiceList = new List<SimpleInvoiceDetailInfo>();
            try
            {
                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
                {
                    while (rdr.Read())
                    {
                        var simpleInvoiceInfo = new SimpleInvoiceDetailInfo
                        {
                            InvoiceId = rdr["InvoiceId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["InvoiceId"].ToString()),
                            InvoiceName = rdr["InvoiceName"] == DBNull.Value ? "" : rdr["InvoiceName"].ToString(),
                            Receiver = rdr["Receiver"] == DBNull.Value ? "" : rdr["Receiver"].ToString(),
                            InvoiceContent = rdr["InvoiceContent"] == DBNull.Value ? "" : rdr["InvoiceContent"].ToString(),
                            PostalCode = rdr["PostalCode"] == DBNull.Value ? "" : rdr["PostalCode"].ToString(),
                            InvoiceSum = rdr["InvoiceSum"] == DBNull.Value ? 0 : double.Parse(rdr["InvoiceSum"].ToString()),
                            InvoiceState = Convert.ToInt32(rdr["InvoiceState"]),
                            Address = rdr["Address"] == DBNull.Value ? "" : rdr["Address"].ToString(),
                            InvoiceCode = rdr["InvoiceCode"] != DBNull.Value ? rdr["InvoiceCode"].ToString() : "-",
                            InvoiceNo = rdr["InvoiceNo"] != DBNull.Value ? (long)rdr["InvoiceNo"] : 0,
                            IsCommit = rdr["IsCommit"] != DBNull.Value && (bool)rdr["IsCommit"]
                        };
                        simpleInvoiceList.Add(simpleInvoiceInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return simpleInvoiceList;
        }

        /// <summary>
        /// 根据发票ID查询发票信息
        /// </summary>
        /// <param name="invoiceId"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        public InvoiceInfo GetInvoiceByInvoiceId(Guid invoiceId)
        {
            const string SQL = @"select InvoiceId, MemberId, InvoiceName, InvoiceContent, Receiver, PostalCode, Address, RequestTime, InvoiceSum, 
                                        InvoiceState, AcceptedTime, PurchaserType, InvoiceCode, InvoiceNo, IsCommit, InvoiceChCode, NoteType, InvoiceChNo, TaxpayerID, CancelPersonel, IsNeedManual, 
                                        SaleFilialeId, SalePlatformId, IsShopInvoice, IsAfterwardsApply from lmShop_Invoice
                                        where InvoiceId = @InvoiceId";
            var parm = new SqlParameter("@InvoiceId", SqlDbType.UniqueIdentifier) { Value = invoiceId };
            InvoiceInfo invoiceInfo = null;
            try
            {
                using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
                {
                    if (rdr.Read())
                    {
                        invoiceInfo = new InvoiceInfo
                        {
                            InvoiceId = rdr["InvoiceId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["InvoiceId"].ToString()),
                            MemberId = rdr["MemberId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["MemberId"].ToString()),
                            InvoiceName = rdr["InvoiceName"] == DBNull.Value ? string.Empty : rdr["InvoiceName"].ToString(),
                            InvoiceContent = rdr["InvoiceContent"] == DBNull.Value ? string.Empty : rdr["InvoiceContent"].ToString(),
                            Receiver = rdr["Receiver"] == DBNull.Value ? string.Empty : rdr["Receiver"].ToString(),
                            PostalCode = rdr["PostalCode"] == DBNull.Value ? string.Empty : rdr["PostalCode"].ToString(),
                            Address = rdr["Address"] == DBNull.Value ? string.Empty : rdr["Address"].ToString(),
                            RequestTime = rdr["RequestTime"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(rdr["RequestTime"].ToString()),
                            InvoiceSum = rdr["InvoiceSum"] == DBNull.Value ? 0 : double.Parse(rdr["InvoiceSum"].ToString()),
                            AcceptedTime = rdr["AcceptedTime"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(rdr["AcceptedTime"].ToString()),
                            InvoiceNo = rdr["InvoiceNo"] == DBNull.Value ? 0 : long.Parse(rdr["InvoiceNo"].ToString()),
                            IsCommit = rdr["IsCommit"] != DBNull.Value && bool.Parse(rdr["IsCommit"].ToString()),
                            PurchaserType = (InvoicePurchaserType)int.Parse(rdr["PurchaserType"] == DBNull.Value ? "0" : rdr["PurchaserType"].ToString()),
                            SaleFilialeId = rdr["SaleFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SaleFilialeId"].ToString()),
                            InvoiceState = Convert.ToInt32(rdr["InvoiceState"]),
                            IsAfterwardsApply = rdr["IsAfterwardsApply"] != DBNull.Value && Convert.ToBoolean(rdr["IsAfterwardsApply"]),
                            InvoiceCode = rdr["InvoiceCode"] != DBNull.Value ? rdr["InvoiceCode"].ToString() : "-",
                            InvoiceChCode = rdr["InvoiceChCode"].ToString(),
                            NoteType = (InvoiceNoteType)(int.Parse(rdr["NoteType"].ToString())),
                            InvoiceChNo = rdr["InvoiceChNo"] != DBNull.Value ? Convert.ToInt64(rdr["InvoiceChNo"]) : 0,
                            TaxpayerID = rdr["TaxpayerID"].ToString(),
                            CancelPersonel = rdr["CancelPersonel"] == DBNull.Value ? "" : rdr.GetString(10),
                            IsNeedManual = rdr["IsNeedManual"] != DBNull.Value && (bool)rdr["IsNeedManual"],
                            SalePlatformId = rdr["SalePlatformId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["SalePlatformId"].ToString()),
                            IsShopInvoice = rdr["IsShopInvoice"] != DBNull.Value && (bool)rdr["IsShopInvoice"]
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return invoiceInfo;
        }

        /// <summary>
        /// 根据发票ID更新红冲发票号，红冲发票代码。
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        public bool UpdateInvoiceChCodeAndInvoiceChNoByinvoiceId(Guid invoiceId, string invoiceChCode, long invoiceChNo)
        {
            const string SQL = "UPDATE [lmShop_Invoice] SET InvoiceChCode=@InvoiceChCode,InvoiceChNo=@InvoiceChNo WHERE InvoiceId=@InvoiceId";
            var parms = new[] {
                new SqlParameter("@InvoiceId",SqlDbType.UniqueIdentifier),
                new SqlParameter("@InvoiceChNo",SqlDbType.BigInt),
                new SqlParameter("@InvoiceChCode",SqlDbType.VarChar,128)
            };
            parms[0].Value = invoiceId;
            parms[1].Value = invoiceChNo;
            parms[2].Value = invoiceChCode;

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// 添加发票订单关系表
        /// </summary>
        /// <param name="invoiceId">发票ID</param>
        /// <param name="orderId">订单ID</param>
        /// <param name="orderNo">订单号</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        public bool InsertOrderInvoice(Guid invoiceId, Guid orderId, string orderNo)
        {
            const string SQL = "insert into lmshop_OrderInvoice(InvoiceId, OrderId, OrderNo) values(@InvoiceId, @OrderId, @OrderNo)";
            var parms = new[]
            {
                new SqlParameter("@InvoiceId", SqlDbType.UniqueIdentifier),
                new SqlParameter("@OrderId", SqlDbType.UniqueIdentifier),
                new SqlParameter("@OrderNo", SqlDbType.VarChar, 128)
            };
            parms[0].Value = invoiceId;
            parms[1].Value = orderId;
            parms[2].Value = orderNo;

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加发票
        /// </summary>
        /// <param name="invoice"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        public bool InsertInvoice(InvoiceInfo invoice)
        {
            const string SQL =
               @"insert into lmShop_Invoice(InvoiceId, MemberId, InvoiceName, InvoiceContent, Receiver, PostalCode, Address, RequestTime, InvoiceSum, InvoiceState, AcceptedTime, PurchaserType, InvoiceCode, InvoiceNo, IsCommit, NoteType, TaxpayerID, CancelPersonel, IsNeedManual, SaleFilialeId, SalePlatformId, IsShopInvoice, IsAfterwardsApply) 
                    values(@InvoiceId, @MemberId, @InvoiceName, @InvoiceContent, @Receiver, @PostalCode, @Address, @RequestTime, @InvoiceSum, @InvoiceState, @AcceptedTime, @PurchaserType, @InvoiceCode, @InvoiceNo, @IsCommit, @NoteType, @TaxpayerID, @CancelPersonel, @IsNeedManual, @SaleFilialeId, @SalePlatformId, @IsShopInvoice, @IsAfterwardsApply)";
            var parms = new[]
                            {
                                new SqlParameter("@InvoiceId", SqlDbType.UniqueIdentifier){Value =  invoice.InvoiceId},
                                new SqlParameter("@MemberId", SqlDbType.UniqueIdentifier){Value =invoice.MemberId },
                                new SqlParameter("@InvoiceName", SqlDbType.VarChar, 128){Value = invoice.InvoiceName},
                                new SqlParameter("@InvoiceContent", SqlDbType.VarChar, 128){Value = invoice.InvoiceContent },
                                new SqlParameter("@Receiver", SqlDbType.VarChar, 64){Value = invoice.Receiver },
                                new SqlParameter("@PostalCode", SqlDbType.VarChar, 8){Value = invoice.PostalCode},
                                new SqlParameter("@Address", SqlDbType.VarChar, 128){Value =string.IsNullOrEmpty(invoice.Address) ? DBNull.Value : (object)invoice.Address},
                                new SqlParameter("@RequestTime", SqlDbType.DateTime){Value =  invoice.RequestTime},
                                new SqlParameter("@InvoiceSum", SqlDbType.Float){Value = invoice.InvoiceSum},
                                new SqlParameter("@InvoiceState", SqlDbType.Int){Value = invoice.InvoiceState},
                                new SqlParameter("@PurchaserType", SqlDbType.Int){Value =(int) invoice.PurchaserType},
                                new SqlParameter("@TaxpayerID", SqlDbType.VarChar){Value = invoice.TaxpayerID},
                                new SqlParameter("@CancelPersonel",SqlDbType.VarChar,128){Value = invoice.CancelPersonel},
                                new SqlParameter("@AcceptedTime", SqlDbType.DateTime){Value = invoice.AcceptedTime},
                                new SqlParameter("@InvoiceCode", SqlDbType.VarChar,128){Value = invoice.InvoiceCode},
                                new SqlParameter("@InvoiceNo", SqlDbType.BigInt){Value = invoice.InvoiceNo},
                                new SqlParameter("@NoteType", SqlDbType.Int){Value = invoice.NoteType},
                                new SqlParameter("@SaleFilialeId",SqlDbType.UniqueIdentifier){Value = invoice.SaleFilialeId},
                                new SqlParameter("@SalePlatformId",SqlDbType.UniqueIdentifier){Value = invoice.SalePlatformId},
                                new SqlParameter("@IsShopInvoice",SqlDbType.BigInt){Value = invoice.IsShopInvoice},
                                new SqlParameter("@IsAfterwardsApply",SqlDbType.BigInt){Value = invoice.IsAfterwardsApply},
                                new SqlParameter("@IsCommit",SqlDbType.BigInt){Value = invoice.IsCommit},
                                //new SqlParameter("@InvoiceChCode",SqlDbType.VarChar,128){Value = invoice.InvoiceChCode},
                                //new SqlParameter("@InvoiceChNo",SqlDbType.BigInt){Value = invoice.InvoiceChNo},
                                new SqlParameter("@IsNeedManual",SqlDbType.BigInt){Value = invoice.IsNeedManual}
                            };
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据发票ID更新发票状态
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        public bool UpdateInvoiceStateByinvoiceId(Guid invoiceId, InvoiceState invoiceState)
        {
            const string SQL = "UPDATE [lmShop_Invoice] SET InvoiceState=@InvoiceState WHERE InvoiceId=@InvoiceId";
            var parms = new[] {
                new SqlParameter("@InvoiceId",SqlDbType.UniqueIdentifier),
                new SqlParameter("@InvoiceState",SqlDbType.Int)
            };
            parms[0].Value = invoiceId;
            parms[1].Value = invoiceState;

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 根据订单号查询发票ID
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        public Guid GetInvoiceIdByOrderNo(string orderNo)
        {
            const string SQL = @"select a.InvoiceId from lmShop_Invoice a
                                        inner join lmshop_OrderInvoice b
                                        on a.InvoiceId = b.InvoiceId
                                        inner join lmShop_GoodsOrder c
                                        on b.OrderId = c.OrderId
                                        where c.OrderNo = @OrderNo";
            var parm = new SqlParameter("@OrderNo", SqlDbType.VarChar, 128) { Value = orderNo };
            object result = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL, parm);
            if (result != DBNull.Value)
            {
                return new Guid(result.ToString());
            }
            return Guid.Empty;
        }


        /// <summary>
        /// 根据发票卷ID和发票起始号修改发票卷详细表状态
        /// </summary>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-07-01
        public bool UpdateInvoiceStateByinvoiceId(Guid rollId, long startNo, int state)
        {
            const string SQL = "update lmShop_InvoiceRollDetail set [state]=@state where RollId=@RollId and StartNo=@StartNo";
            var parms = new[] {
                new SqlParameter("@state",SqlDbType.Int),
                new SqlParameter("@RollId",SqlDbType.UniqueIdentifier),
                new SqlParameter("@StartNo",SqlDbType.BigInt)
            };
            parms[0].Value = state;
            parms[1].Value = rollId;
            parms[1].Value = startNo;

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>更新发票抬头和发票内容
        /// </summary>
        /// <returns></returns>
        public bool SetInvoiceNameAndInvoiceContent(Guid invoiceId, string invoiceName, string invoiceContent)
        {
            var sql = String.Format("update [lmShop_Invoice] set InvoiceName='{0}', InvoiceContent='{1}' where InvoiceId='{2}' ", invoiceName, invoiceContent, invoiceId);
            return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, null) > 0;
        }

        /// <summary>通过订单ID获取发票号码和发票是否报税    (Key:发票号码，Value:是否报税)
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<long, bool> GetInvoiceNoAndIsCommitByOrderId(Guid orderId)
        {
            var sql = String.Format(@"select i1.InvoiceNo,i1.IsCommit from lmShop_Invoice i1
                                                                inner join lmshop_OrderInvoice i2 on i1.InvoiceId =i2.InvoiceId
                                                                where i2.OrderId = '{0}'", orderId);
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                if (rdr.Read())
                {
                    return new KeyValuePair<long, bool>(SqlRead.GetLong(rdr, "InvoiceNo"), SqlRead.GetBoolean(rdr, "IsCommit")); ;
                }
                return new KeyValuePair<long, bool>(default(long), default(Boolean));
            }
        }
    }
}

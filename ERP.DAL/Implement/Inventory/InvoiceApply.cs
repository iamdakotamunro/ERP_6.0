using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Enum.ApplyInvocie;
using Keede.Ecsoft.Model;
using Keede.DAL.Helper;
using ERP.Environment;
using Keede.DAL.RWSplitting;
using ERP.Model.Finance;
using ERP.Model.Invoice;
using Keede.DAL.Helper.Sql;

namespace ERP.DAL.Implement.Inventory
{
    public class InvoiceApply : IInvoiceApply
    {

        public bool Insert(InvoiceApplyInfo invoiceApplyInfo, List<InvoiceApplyDetailInfo> detailInfos)
        {
            const string SQL = @"INSERT INTO InvoiceApply(ApplyId,TradeCode,ThirdPartyCode,TargetId,ApplyDateTime,ApplyState,ApplyType,ApplySourceType,ApplyKind,Amount,InvoiceTitleType,Title,TaxpayerNumber,BankName,
	BankAccountNo,ContactAddress,ContactTelephone,Receiver,Telephone,[Address],ApplyRemark,RetreatRemark,SaleFilialeId,SalePlatformId) VALUES(@ApplyId,@TradeCode,@ThirdPartyCode,@TargetId,@ApplyDateTime,@ApplyState,@ApplyType,@ApplySourceType,@ApplyKind,@Amount,@InvoiceTitleType,@Title,@TaxpayerNumber,@BankName,
	@BankAccountNo,@ContactAddress,@ContactTelephone,@Receiver,@Telephone,@Address,@ApplyRemark,@RetreatRemark,@SaleFilialeId,@SalePlatformId)";

            const string SQL_DETAIL = @"INSERT INTO InvoiceApplyDetail(Id,ApplyId,Tradecode,LinkTradeCodes,PayBalanceAmount,PayRebateAmount,TotalPayAmount) VALUES(@Id,@ApplyId,@Tradecode,@LinkTradeCodes,@PayBalanceAmount,@PayRebateAmount,@TotalPayAmount)";
            try
            {
                using (var db = DatabaseFactory.Create())
                {
                    db.BeginTransaction();
                    db.Execute(false,SQL, new[] {
                        new Parameter("@ApplyId", invoiceApplyInfo.ApplyId),
                        new Parameter("@TradeCode", invoiceApplyInfo.TradeCode),
                        new Parameter("@ThirdPartyCode", invoiceApplyInfo.ThirdPartyCode),
                        new Parameter("@TargetId", invoiceApplyInfo.TargetId),
                        new Parameter("@ApplyDateTime", invoiceApplyInfo.ApplyDateTime),
                        new Parameter("@ApplyState", invoiceApplyInfo.ApplyState),
                        new Parameter("@ApplyType", invoiceApplyInfo.ApplyType),
                        new Parameter("@ApplySourceType", invoiceApplyInfo.ApplySourceType),
                        new Parameter("@ApplyKind", invoiceApplyInfo.ApplyKind),
                        new Parameter("@Amount", invoiceApplyInfo.Amount),
                        new Parameter("@InvoiceTitleType", invoiceApplyInfo.InvoiceTitleType),
                        new Parameter("@Title", invoiceApplyInfo.Title),
                        new Parameter("@TaxpayerNumber", invoiceApplyInfo.TaxpayerNumber),
                        new Parameter("@BankName", invoiceApplyInfo.BankName),
                        new Parameter("@BankAccountNo", invoiceApplyInfo.BankAccountNo),
                        new Parameter("@ContactAddress", invoiceApplyInfo.ContactAddress),
                        new Parameter("@ContactTelephone", invoiceApplyInfo.ContactTelephone),
                        new Parameter("@Receiver", invoiceApplyInfo.Receiver),
                        new Parameter("@Telephone", invoiceApplyInfo.Telephone),
                        new Parameter("@Address", invoiceApplyInfo.Address),
                        new Parameter("@ApplyRemark", invoiceApplyInfo.ApplyRemark),
                        new Parameter("@RetreatRemark", invoiceApplyInfo.RetreatRemark),
                        new Parameter("@SaleFilialeId", invoiceApplyInfo.SaleFilialeId),
                        new Parameter("@SalePlatformId", invoiceApplyInfo.SalePlatformId)
                    });
                    if (detailInfos != null && detailInfos.Count > 0)
                        foreach (var invoiceApplyDetailInfo in detailInfos)
                        {
                            db.Execute(false,SQL_DETAIL, new[] {
                            new Parameter("@Id", invoiceApplyDetailInfo.Id),
                            new Parameter("@ApplyId", invoiceApplyDetailInfo.ApplyId),
                            new Parameter("@Tradecode", invoiceApplyDetailInfo.Tradecode),
                            new Parameter("@LinkTradeCodes", invoiceApplyDetailInfo.LinkTradeCodes),
                            new Parameter("@PayBalanceAmount", invoiceApplyDetailInfo.PayBalanceAmount),
                            new Parameter("@PayRebateAmount", invoiceApplyDetailInfo.PayRebateAmount),
                            new Parameter("@TotalPayAmount", invoiceApplyDetailInfo.TotalPayAmount)
                        });
                        }
                    return db.CompleteTransaction();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Modify(InvoiceApplyInfo invoiceApplyInfo, List<InvoiceApplyDetailInfo> detailInfos)
        {
            const string SQL = @"UPDATE InvoiceApply SET ApplyState=@ApplyState,ApplyType=@ApplyType,ApplyKind,Amount,InvoiceTitleType,Title=@Title,TaxpayerNumber=@TaxpayerNumber,BankName=@BankName,
	BankAccountNo=@BankAccountNo,ContactAddress=@ContactAddress,ContactTelephone=@ContactTelephone,Receiver=@Receiver,Telephone=@Telephone,[Address]=@Address,ApplyRemark=@ApplyRemark,RetreatRemark,SaleFilialeId=@SaleFilialeId,SalePlatformId=@SalePlatformId WHERE ApplyId=@ApplyId";

            const string SQL_DETAIL = @"INSERT INTO InvoiceApplyDetail(Id,ApplyId,Tradecode,LinkTradeCodes,PayBalanceAmount,PayRebateAmount,TotalPayAmount) VALUES(@Id,@ApplyId,@Tradecode,@LinkTradeCodes,@PayBalanceAmount,@PayRebateAmount,@TotalPayAmount)";

            const string SQL_DELETE = @"DELETE InvoiceApplyDetail WHERE ApplyId=@ApplyId";
            try
            {
                using (var db = DatabaseFactory.Create())
                {
                    db.BeginTransaction();
                    db.Execute(false, SQL, new[] {
                        new Parameter("@ApplyId", invoiceApplyInfo.ApplyId),
                        new Parameter("@TargetId", invoiceApplyInfo.TargetId),
                        new Parameter("@ApplyState", invoiceApplyInfo.ApplyState),
                        new Parameter("@ApplyType", invoiceApplyInfo.ApplyType),
                        new Parameter("@ApplyKind", invoiceApplyInfo.ApplyKind),
                        new Parameter("@Amount", invoiceApplyInfo.Amount),
                        new Parameter("@InvoiceTitleType", invoiceApplyInfo.InvoiceTitleType),
                        new Parameter("@Title", invoiceApplyInfo.Title),
                        new Parameter("@TaxpayerNumber", invoiceApplyInfo.TaxpayerNumber),
                        new Parameter("@BankName", invoiceApplyInfo.BankName),
                        new Parameter("@BankAccountNo", invoiceApplyInfo.BankAccountNo),
                        new Parameter("@ContactAddress", invoiceApplyInfo.ContactAddress),
                        new Parameter("@ContactTelephone", invoiceApplyInfo.ContactTelephone),
                        new Parameter("@Receiver", invoiceApplyInfo.Receiver),
                        new Parameter("@Telephone", invoiceApplyInfo.Telephone),
                        new Parameter("@Address", invoiceApplyInfo.Address),
                        new Parameter("@ApplyRemark", invoiceApplyInfo.ApplyRemark),
                        new Parameter("@SaleFilialeId", invoiceApplyInfo.SaleFilialeId),
                        new Parameter("@SalePlatformId", invoiceApplyInfo.SalePlatformId)
                    });
                    db.Execute(false, SQL_DELETE, new[] {
                        new Parameter("@ApplyId", invoiceApplyInfo.ApplyId)
                    });
                    if (detailInfos != null && detailInfos.Count > 0)
                        foreach (var invoiceApplyDetailInfo in detailInfos)
                        {
                            db.Execute(false, SQL_DETAIL, new[] {
                            new Parameter("@Id", invoiceApplyDetailInfo.Id),
                            new Parameter("@ApplyId", invoiceApplyDetailInfo.ApplyId),
                            new Parameter("@Tradecode", invoiceApplyDetailInfo.Tradecode),
                            new Parameter("@LinkTradeCodes", invoiceApplyDetailInfo.LinkTradeCodes),
                            new Parameter("@PayBalanceAmount", invoiceApplyDetailInfo.PayBalanceAmount),
                            new Parameter("@PayRebateAmount", invoiceApplyDetailInfo.PayRebateAmount),
                            new Parameter("@TotalPayAmount", invoiceApplyDetailInfo.TotalPayAmount)
                        });
                        }
                    return db.CompleteTransaction();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Update(Guid applyId, int applyState,string applyRemark, string retreatRemark, Guid personnelId)
        {
            StringBuilder builder = new StringBuilder(@"UPDATE InvoiceApply SET ApplyState=@ApplyState,ApplyRemark=ApplyRemark+@ApplyRemark, RetreatRemark=RetreatRemark + @RetreatRemark  ");
            if (applyState == (int)ApplyInvoiceState.Finished)
            {
                builder.AppendFormat(",FinishTime=GETDATE(),AuditorId='{0}' ", personnelId);
            }
            builder.Append(" WHERE ApplyId=@ApplyId ");
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false,builder.ToString(), new[] { new Parameter("@ApplyState", applyState), new Parameter("@ApplyRemark", applyRemark), new Parameter("@RetreatRemark", retreatRemark), new Parameter("@ApplyId", applyId) });
            }
        }

        public bool UpdateRemark(Guid applyId, string applyRemark)
        {
            string sql= @"UPDATE InvoiceApply SET ApplyRemark=ApplyRemark+@ApplyRemark WHERE ApplyId=@ApplyId  ";
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, sql, new[] { new Parameter("@ApplyRemark", applyRemark), new Parameter("@ApplyId", applyId) });
            }
        }

        public List<InvoiceApplyInfo> GetInvoiceApplyInfos(DateTime startTime, DateTime endTime, string searchKey, IEnumerable<int> applyStates, int applyType, int applySourceType, Guid saleFilialeId, Guid salePlatformId)
        {
            StringBuilder builder = new StringBuilder(@"SELECT ApplyId,TradeCode,ThirdPartyCode,TargetId,ApplyDateTime,ApplyState,ApplyType,ApplySourceType,ApplyKind,Amount,InvoiceTitleType,Title,TaxpayerNumber,BankName,
	BankAccountNo,ContactAddress,ContactTelephone,Receiver,Telephone,[Address],ApplyRemark,RetreatRemark,SaleFilialeId,SalePlatformId,AuditorId,FinishTime FROM InvoiceApply ");
            bool hasWhere = false;
            if (startTime != DateTime.MinValue)
            {
                builder.AppendFormat(" {0} ApplyDateTime >= '{1}'", hasWhere ? "AND" : "WHERE", startTime);
                hasWhere = true;
            }
            if (endTime != DateTime.MinValue)
            {
                builder.AppendFormat(" {0} ApplyDateTime <= '{1}'", hasWhere ? "AND" : "WHERE", endTime);
                hasWhere = true;
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                builder.AppendFormat(" {0} (TradeCode='{1}' OR ThirdPartyCode='{1}' OR ApplyId IN(SELECT ApplyId FROM InvoiceApplyDetail WHERE Tradecode='{1}')) ", hasWhere ? "AND" : "WHERE", searchKey);
                hasWhere = true;
            }
            if (applyStates != null && applyStates.Any())
            {
                builder.AppendFormat(" {0} ApplyState IN({1})", hasWhere ? "AND" : "WHERE", string.Join(",", applyStates));
                hasWhere = true;
            }
            if (applySourceType != 0)
            {
                builder.AppendFormat(" {0} ApplyType ={1}", hasWhere ? "AND" : "WHERE", applyType);
                hasWhere = true;
            }
            if (applySourceType != 0)
            {
                builder.AppendFormat(" {0} ApplySouceType ={1}", hasWhere ? "AND" : "WHERE", applySourceType);
                hasWhere = true;
            }
            if (saleFilialeId != default(Guid))
            {
                builder.AppendFormat(" {0} SaleFilialeId ='{1}'", hasWhere ? "AND" : "WHERE", saleFilialeId);
                hasWhere = true;
            }
            if (salePlatformId != default(Guid))
            {
                builder.AppendFormat(" {0} SalePlatformId = '{1}'", hasWhere ? "AND" : "WHERE", salePlatformId);
            }
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<InvoiceApplyInfo>(true,builder.ToString()).ToList();
            }
        }

        public List<InvoiceApplyInfo> GetInvoiceApplyInfosWithPage(DateTime startTime, DateTime endTime, string searchKey, IEnumerable<int> applyStates, int invoiceType, int applySourceType, int applyKind,Guid salePlatformId,int pageIndex, int pageSize, out long total)
        {
            StringBuilder builder = new StringBuilder(@"SELECT ApplyId,TradeCode,ThirdPartyCode,TargetId,ApplyDateTime,ApplyState,ApplyType,ApplySourceType,ApplyKind,Amount,InvoiceTitleType,Title,TaxpayerNumber,BankName,
	BankAccountNo,ContactAddress,ContactTelephone,Receiver,Telephone,[Address],ApplyRemark,RetreatRemark,SaleFilialeId,SalePlatformId,AuditorId,FinishTime FROM InvoiceApply ");
            builder.AppendFormat(" WHERE ApplySourceType={0} ", applySourceType);
            if (startTime != DateTime.MinValue)
            {
                builder.AppendFormat(" AND ApplyDateTime >= '{0}'", startTime);
            }
            if (endTime != DateTime.MinValue)
            {
                builder.AppendFormat(" AND ApplyDateTime <= '{0}'", endTime);
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                if (applySourceType == (int) ApplyInvoiceSourceType.Order)
                {
                    builder.AppendFormat(" AND (TradeCode='{0}' OR ThirdPartyCode='{0}') ", searchKey);
                }
                else
                {
                    builder.AppendFormat(" AND (TradeCode='{0}' OR ApplyId IN(SELECT ApplyId FROM InvoiceApplyDetail WHERE Tradecode='{0}')) ", searchKey);
                }
            }
            if (applyStates != null && applyStates.Any())
            {
                builder.AppendFormat(" AND ApplyState IN({0})", string.Join(",", applyStates));
            }
            if (invoiceType != 0)
            {
                builder.AppendFormat(" AND ApplyType ={0}", invoiceType);
            }
            if (applyKind != 0)
            {
                builder.AppendFormat(" {AND ApplyKind ={0}", applyKind);
            }
            if (salePlatformId != default(Guid))
            {
                builder.AppendFormat(" {AND SalePlatformId = '{0}'", salePlatformId);
            }
            using (var db = DatabaseFactory.Create())
            {
                var result=db.SelectByPage<InvoiceApplyInfo>(true, new PageQuery(pageIndex,pageSize,builder.ToString()));
                total = result.RecordCount;
                return result.Items.ToList();
            }
        }
        
        public List<InvoiceApplyDetailInfo> GetInvoiceApplyDetailInfos(Guid applyId)
        {
            const string SQL = @"SELECT Id,ApplyId,Tradecode,LinkTradeCodes,PayBalanceAmount,PayRebateAmount,TotalPayAmount FROM InvoiceApplyDetail WHERE ApplyId=@ApplyId";
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<InvoiceApplyDetailInfo>(true,SQL, new Parameter("@ApplyId", applyId)).ToList();
            }
        }

        public InvoiceApplyInfo GetInvoiceApplyInfo(Guid applyId)
        {
            const string SQL = @"SELECT ApplyId,TradeCode,ThirdPartyCode,TargetId,ApplyDateTime,ApplyState,ApplyType,ApplySourceType,ApplyKind,Amount,InvoiceTitleType,Title,TaxpayerNumber,BankName,
	BankAccountNo,ContactAddress,ContactTelephone,Receiver,Telephone,[Address],ApplyRemark,RetreatRemark,SaleFilialeId,SalePlatformId,AuditorId,FinishTime FROM InvoiceApply WHERE ApplyId=@ApplyId";
            using (var db = DatabaseFactory.Create())
            {
                var result = db.Select<InvoiceApplyInfo>(true,SQL, new Parameter("@ApplyId", applyId));
                return result?.FirstOrDefault();
            }
        }

        public bool IsExists(Guid applyId, string invoiceNo)
        {
            const string SQL = @"SELECT COUNT(*) FROM InvoiceRelation WHERE ApplyId=@ApplyId AND InvoiceNo=@InvoiceNo";
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(false,SQL, new[] { new Parameter("@ApplyId", applyId), new Parameter("@InvoiceNo", invoiceNo) }) > 0;
            }
        }

        public bool InsertRelation(InvoiceRelationInfo relationInfo, List<InvoiceBindGoods> bindGoods)
        {
            const string SQL = @"INSERT INTO InvoiceRelation(InvoiceId,ApplyId,RequestTime,InvoiceNo,InvoiceCode,UnTaxFee,TaxFee,TotalFee,Remark) VALUES(@InvoiceId,@ApplyId,@RequestTime,@InvoiceNo,@InvoiceCode,@UnTaxFee,@TaxFee,@TotalFee,@Remark)";

            const string SQL_BIND = @"INSERT INTO InvoiceBindGoods(Id,InvoiceId,GoodsId,GoodsCode,Quantity,UnitPrice) VALUES(@Id,@InvoiceId,@GoodsId,@GoodsCode,@Quantity,@UnitPrice)";

            bool result=true;
            using (var db = DatabaseFactory.Create())
            {
                db.BeginTransaction();
                if (relationInfo.IsCanEdit)
                {
                    result = db.Execute(false, SQL, new[] {
                        new Parameter("@InvoiceId", relationInfo.InvoiceId),
                        new Parameter("@ApplyId", relationInfo.ApplyId),
                        new Parameter("@RequestTime", relationInfo.RequestTime),
                        new Parameter("@InvoiceNo", relationInfo.InvoiceNo),
                        new Parameter("@InvoiceCode", relationInfo.InvoiceCode),
                        new Parameter("@UnTaxFee", relationInfo.UnTaxFee),
                        new Parameter("@TaxFee", relationInfo.TaxFee),
                        new Parameter("@TotalFee", relationInfo.TotalFee),
                        new Parameter("@Remark", relationInfo.Remark??string.Empty)
                    });
                }
                if (result)
                {
                    foreach (var item in bindGoods)
                    {
                        result = db.Execute(false, SQL_BIND, new[] {
                            new Parameter("@Id", item.Id),
                            new Parameter("@InvoiceId", relationInfo.InvoiceId),
                            new Parameter("@GoodsId", item.GoodsId),
                            new Parameter("@GoodsCode", item.GoodsCode),
                            new Parameter("@Quantity", item.Quantity),
                            new Parameter("@UnitPrice", item.UnitPrice)
                        });
                        if (!result)
                            return false;
                    }
                    db.CompleteTransaction();
                }
            }
            return result;
        }

        public List<InvoiceRelationInfo> GetInvoiceRelationInfos(Guid applyId)
        {
            const string SQL = @"SELECT InvoiceId,ApplyId,RequestTime,InvoiceNo,InvoiceCode,UnTaxFee,TaxFee,TotalFee,Remark FROM InvoiceRelation WHERE ApplyId=@ApplyId";
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<InvoiceRelationInfo>(true,SQL, new Parameter("@ApplyId", applyId)).ToList();
            }
        }

        public List<InvoiceDTO> GetBindGoodses(DateTime startTime, DateTime endTime)
        {
            const string SQL = @"SELECT I.GoodsId,I.GoodsCode,I.Quantity,I.UnitPrice,I.Id,I.InvoiceId,A.TradeCode,A.FinishTime,A.SaleFilialeId AS FilialeId,A.TargetId AS ThirdCompanyId,A.AuditorId,R.Remark FROM InvoiceBindGoods AS I
 INNER JOIN InvoiceRelation AS R ON I.InvoiceId=R.InvoiceId
 INNER JOIN InvoiceApply AS A ON R.ApplyId=A.ApplyId
 WHERE A.FinishTime BETWEEN @StartTime AND @EndTime";
            Dictionary<Guid,InvoiceDTO> invoiceDtos=new Dictionary<Guid, InvoiceDTO>();
            var parm =new []{new SqlParameter("@StartTime", startTime), new SqlParameter("@EndTime", endTime) };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                if (rdr.Read() && rdr["InvoiceId"]!=null)
                {
                    var invoiceId = new Guid(rdr["InvoiceId"].ToString());
                    if (invoiceDtos.ContainsKey(invoiceId))
                    {
                        invoiceDtos[invoiceId].Details.Add(new InvoiceDTO.Detail
                        {
                            GoodsCode = rdr["GoodsCode"].ToString(),
                            GoodsId = new Guid(rdr["GoodsId"].ToString()),
                            Id = Convert.ToInt32(rdr["Id"]),
                            Quantity = Convert.ToInt32(rdr["Quantity"]),
                            UnitPrice = Convert.ToDecimal(rdr["UnitPrice"])
                        });
                    }
                    else
                    {
                        invoiceDtos.Add(invoiceId, new InvoiceDTO
                        {
                            AuditorId = new Guid(rdr["AuditorId"].ToString()),
                            FilialeId = new Guid(rdr["FilialeId"].ToString()),
                            ThirdCompanyId = new Guid(rdr["ThirdCompanyId"].ToString()),
                            FinishTime=Convert.ToDateTime(rdr["FinishTime"]),
                            Remark = rdr["Remark"].ToString(),
                            TradeCode = rdr["TradeCode"].ToString(),
                            Details = new List<InvoiceDTO.Detail>
                            {
                                new InvoiceDTO.Detail
                                {
                                    GoodsCode = rdr["GoodsCode"].ToString(),
                                    GoodsId = new Guid(rdr["GoodsId"].ToString()),
                                    Id = Convert.ToInt32(rdr["Id"]),
                                    Quantity = Convert.ToInt32(rdr["Quantity"]),
                                    UnitPrice = Convert.ToDecimal(rdr["UnitPrice"])
                                }
                            }
                        });
                    }
                    
                }
            }
            return invoiceDtos.Values.ToList();
        }
    }
}

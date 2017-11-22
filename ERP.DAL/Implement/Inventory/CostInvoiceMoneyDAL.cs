using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Model;
using Keede.DAL.Helper;
using ERP.Environment;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>▄︻┻┳═一 费用发票金额数据层   ADD 2014-12-20  陈重文
    /// </summary>
    public class CostInvoiceMoneyDAL : ICostInvoiceMoney
    {
        public CostInvoiceMoneyDAL(Environment.GlobalConfig.DB.FromType fromType) { }

        /// <summary>保存公司具体年月份费用发票金额
        /// </summary>
        /// <param name="costInvoiceMoneyInfo">费用发票金额模型</param>
        /// <returns>Return:true/false</returns>
        public bool SaveCostInvoiceMoney(CostInvoiceMoneyInfo costInvoiceMoneyInfo)
        {
            const string SQL = @"
IF NOT EXISTS(SELECT Limit FROM [CostInvoiceMoney] WHERE FilialeId=@FilialeId AND DateYear=@DateYear AND DateMonth=@DateMonth)
    INSERT INTO [CostInvoiceMoney] (FilialeId,DateYear,DateMonth,Limit) VALUES(@FilialeId,@DateYear,@DateMonth,@Limit);
ELSE
    UPDATE [CostInvoiceMoney] SET Limit=@Limit WHERE FilialeId=@FilialeId AND DateYear=@DateYear AND DateMonth=@DateMonth;";
            var parms = new[]
                           {
                               new SqlParameter("@FilialeId", SqlDbType.UniqueIdentifier),
                               new SqlParameter("@DateYear", SqlDbType.Int),
                               new SqlParameter("@DateMonth", SqlDbType.Int),
                               new SqlParameter("@Limit", SqlDbType.Decimal)
                           };
            try
            {
                parms[0].Value = costInvoiceMoneyInfo.FilialeId;
                parms[1].Value = costInvoiceMoneyInfo.DateYear;
                parms[2].Value = costInvoiceMoneyInfo.DateMonth;
                parms[3].Value = costInvoiceMoneyInfo.Limit;
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms) > 0;
            }
            catch (Exception exp)
            {
                SAL.LogCenter.LogService.LogError(string.Format("费用发票金额保存失败,FilialeId={0},DateYear={1},DateMonth={2},Limit={3}", costInvoiceMoneyInfo.FilialeId, costInvoiceMoneyInfo.DateYear, costInvoiceMoneyInfo.DateMonth, costInvoiceMoneyInfo.Limit), "财务管理", exp);
                return false;
            }
        }
        
        /// <summary>获取具体年月份费用发票金额
        /// </summary>
        /// <returns>Return:费用发票金额集合</returns>
        public IList<CostInvoiceMoneyInfo> GetCostInvoiceMoneyList(int dateYear, int dateMonth)
        {
            const string SQL = @"
SELECT [FilialeId]
      ,[DateYear]
      ,[DateMonth]
      ,[Limit]
  FROM [CostInvoiceMoney]
WHERE DateYear=@DateYear AND DateMonth=@DateMonth";
            var parms = new[] {
                new SqlParameter("@DateYear", SqlDbType.Int){Value = dateYear},
                new SqlParameter("@DateMonth", SqlDbType.Int){Value = dateMonth}
             };
            IList<CostInvoiceMoneyInfo> list = new List<CostInvoiceMoneyInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (rdr.Read())
                {
                    var info = new CostInvoiceMoneyInfo
                    {
                        FilialeId = new Guid(rdr["FilialeId"].ToString()),
                        Limit = Convert.ToDecimal(rdr["Limit"]),
                    };
                    list.Add(info);
                }
            }
            return list;
        }
    }
}

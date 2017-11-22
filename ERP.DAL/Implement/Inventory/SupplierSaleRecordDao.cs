using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model.Report;
using Keede.DAL.Helper;
using Keede.DAL.RWSplitting;

namespace ERP.DAL.Implement.Inventory
{
    public class SupplierSaleRecordDao:ISupplierSaleRecord
    {
        /// <summary>
        /// 判断某月份是否已存档
        /// </summary>
        /// <param name="dayTime"></param>
        /// <returns></returns>
        public bool IsExists(DateTime dayTime)
        {
            const string SQL = @"SELECT COUNT(*) FROM SupplierSaleRecord WHERE DayTime='{0}'";
            using (var db = DatabaseFactory.CreateRdb())
            {
                return db.GetValue<int>(true, string.Format(SQL,dayTime))>0;
            }
        }

        /// <summary>
        /// 添加公司对应供应商销售记录
        /// </summary>
        /// <param name="supplierSaleRecordInfos"></param>
        /// <returns></returns>
        public bool InsertSaleRecord(IList<SupplierSaleRecordInfo> supplierSaleRecordInfos)
        {
            var dics = new Dictionary<string, string>
                {
                    {"CompanyID","CompanyID"},{"Quantity","Quantity"},{"TotalSettlePrice","TotalSettlePrice"},{"DayTime","DayTime"}
                };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_REPORT_DB_NAME, supplierSaleRecordInfos, "SupplierSaleRecord", dics)>0;
        }

        /// <summary>
        /// 供应商销量页面显示数据(对应公司的销量) PASS
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public IList<SupplierSaleReportInfo> SelectSupplierSaleReportInfos(int year)
        {
            const string SQL = @"
SELECT CompanyID,ABS(sum(case when Month(DayTime)=1 then TotalSettlePrice else 0  end)) as January,
ABS(sum(case when Month(DayTime)=2 then TotalSettlePrice else 0 end)) as February,
ABS(sum(case when Month(DayTime)=3 then TotalSettlePrice else 0  end)) as March,
ABS(sum(case when Month(DayTime)=4 then TotalSettlePrice else 0  end))as April, 
ABS(sum(case when Month(DayTime)=5 then TotalSettlePrice else 0  end)) as May, 
ABS(sum(case when Month(DayTime)=6 then TotalSettlePrice else 0  end)) as June, 
ABS(sum(case when Month(DayTime)=7 then TotalSettlePrice else 0  end)) as July, 
ABS(sum(case when Month(DayTime)=8 then TotalSettlePrice else 0  end)) as August, 
ABS(sum(case when Month(DayTime)=9 then TotalSettlePrice else 0  end)) as September, 
ABS(sum(case when Month(DayTime)=10 then TotalSettlePrice else 0  END)) as October, 
ABS(sum(case when Month(DayTime)=11 then TotalSettlePrice else 0  end)) as November, 
ABS(sum(case when Month(DayTime)=12 then TotalSettlePrice else 0  end)) as December  FROM SupplierSaleRecord 
WHERE YEAR(DayTime)={0}
GROUP BY CompanyID ";
            using (var db = DatabaseFactory.CreateRdb())
            {
                return db.Select<SupplierSaleReportInfo>(true, string.Format(SQL,year)).ToList();
            }
        }

        /// <summary>
        /// 获取当月已存在的销售数据
        /// </summary>
        /// <param name="dayTime"></param>
        /// <param name="supplierSaleRecordInfos"></param>
        /// <returns></returns>
        public bool SelectSupplierSaleRecordInfos(DateTime dayTime,IList<SupplierSaleRecordInfo> supplierSaleRecordInfos)
        {
            const string SQL = @"DELETE SupplierSaleRecord WHERE DayTime='{0}'";
            var dics = new Dictionary<string, string>
                {
                    {"CompanyID","CompanyID"},{"DayTime","DayTime"},{"Quantity","Quantity"},{"TotalSettlePrice","TotalSettlePrice"}
                };
            var flag = false;
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_REPORT_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans,string.Format(SQL,dayTime));
                    var result2 = SqlHelper.BatchInsert(trans, supplierSaleRecordInfos, "SupplierSaleRecord", dics) > 0;
                    if (result2)
                    {
                        trans.Commit();
                        flag = true;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception("当月销售数据存储异常", ex);
                }
                finally
                {
                    connection.Close();
                }
            }
            return flag;
        }
    }
}

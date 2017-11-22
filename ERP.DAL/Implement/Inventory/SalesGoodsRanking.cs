using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    public class SalesGoodsRanking : ISalesGoodsRanking
    {
        public SalesGoodsRanking(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        /// <summary>
        /// 获取商品销售排行
        /// </summary>
        /// <para>
        /// Code by Ruanjianfeng 2012-2-28
        /// 统计销量分为非0元销量和0元销量，Li Zhongkai,2015-05-27
        /// </para>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId">品牌下的商品ID</param>
        /// <param name="goodsName"></param>
        /// <param name="goodsCode"></param>
        /// <param name="warehouseId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="isContainDisableSalePlatform"> </param>
        /// <returns></returns>
        public IList<SalesGoodsRankingInfo> GetGoodsSalesRanking(int top, string goodsClassList,
            Guid brandId, string goodsName,
            string goodsCode, Guid warehouseId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform)
        {
            const string SQL = @"
SELECT 
    GSS.GoodsID,
    GSS.GoodsName,
    GSS.GoodsCode,
    ISNULL(SUM(GSS.SellPrice),0) AS GoodsPrice,
    SUM(case when GSS.SellPrice = 0 then GSS.GoodsSales else 0 end) AS ZeroNumber,
    SUM(case when GSS.SellPrice <> 0 then GSS.GoodsSales else 0 end) AS SalesNumber,
    GSS.SalePlatformId,GSS.SeriesId,GSS.BrandId 
FROM [lmshop_GoodsDaySalesStatistics] AS GSS 
WHERE 1=1 ";

            var paramList = new List<SqlParameter>();
            var cmdText = new StringBuilder();
            cmdText.AppendLine("WITH TEMP AS( ");
            if (isContainDisableSalePlatform)
            {
                cmdText.AppendFormat(SQL);
            }
            else
            {
                cmdText.AppendFormat(@"SELECT 
    GSS.GoodsID,
    GSS.GoodsName,
    GSS.GoodsCode,
    ISNULL(SUM(GSS.SellPrice),0) AS GoodsPrice,
    SUM(case when GSS.SellPrice = 0 then GSS.GoodsSales else 0 end) AS ZeroNumber,
    SUM(case when GSS.SellPrice <> 0 then GSS.GoodsSales else 0 end) AS SalesNumber,GSS.SalePlatformId,GSS.BrandId  
FROM [lmshop_GoodsDaySalesStatistics] AS GSS 
LEFT JOIN dbo.SalePlatform AS SP ON GSS.SalePlatformId=SP.Id
AND SP.IsActive=1 
WHERE 1=1");
            }
            if (!string.IsNullOrEmpty(goodsClassList))
            {
                cmdText.AppendFormat(" AND GSS.ClassId IN ({0})", goodsClassList);
            }
            if (Guid.Empty != brandId)
            {
                cmdText.AppendFormat(" AND GSS.BrandId = '{0}'", brandId);
            }
            if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
            {
                cmdText.Append(string.Format(" And (GSS.DayTime >='{0}' And GSS.DayTime < '{1}')", startTime, endTime));
            }
            if (!string.IsNullOrEmpty(goodsName))
            {
                cmdText.Append(" AND GSS.GoodsName like '%'+@GoodsName+'%'");
                paramList.Add(new SqlParameter("@GoodsName", goodsName));
            }
            if (!string.IsNullOrEmpty(goodsCode))
            {
                cmdText.Append(" AND GSS.GoodsCode=@GoodsCode");
                paramList.Add(new SqlParameter("@GoodsCode", goodsCode));
            }
            if (warehouseId != Guid.Empty)
            {
                cmdText.Append(" AND GSS.DeliverWarehouseId='").Append(warehouseId).Append("'");
            }
            if (salefilialeId != Guid.Empty)
            {
                cmdText.Append(" AND GSS.SaleFilialeId='").Append(salefilialeId).Append("'");
            }
            if (salePlatformId != Guid.Empty)
            {
                cmdText.Append(" AND GSS.SalePlatformId='").Append(salePlatformId).Append("'");
            }
            cmdText.Append(" GROUP BY GSS.GoodsID,GSS.GoodsName,GSS.GoodsCode,GSS.SalePlatformId,GSS.SeriesId,GSS.BrandId");
            cmdText.Append(@")
SELECT GoodsID
,GoodsCode
,(SELECT TOP 1 GoodsName FROM TEMP AS B WHERE A.GoodsID=B.GoodsID) AS GoodsName
,SUM(GoodsPrice) AS GoodsPrice
,SUM(SalesNumber) AS SalesNumber,SUM(ZeroNumber) AS ZeroNumber,SalePlatformId,GSS.SeriesId,BrandId  FROM TEMP AS A
GROUP BY GoodsID,GoodsCode,SalePlatformId,GSS.SeriesId,BrandId ORDER BY SalesNumber DESC");//dbo.fun_GetGoodsName(a.GoodsID) as
            var salesRankingList = new List<SalesGoodsRankingInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, cmdText.ToString(), paramList.ToArray()))
            {
                while (dr.Read())
                {
                    var info = new SalesGoodsRankingInfo
                    {
                        GoodsId = SqlRead.GetGuid(dr, "GoodsID"),
                        GoodsName = SqlRead.GetString(dr, "GoodsName"),
                        GoodsCode = SqlRead.GetString(dr, "GoodsCode"),
                        GoodsPrice = SqlRead.GetDecimal(dr, "GoodsPrice"),
                        SalesNumber = SqlRead.GetInt(dr, "SalesNumber"),
                        ZeroNumber = SqlRead.GetInt(dr, "ZeroNumber"),
                        SalePlatformId = SqlRead.GetGuid(dr, "SalePlatformId"),
                        SeriesId = SqlRead.GetGuid(dr, "SeriesId"),
                        BrandId = SqlRead.GetGuid(dr, "BrandId")
                    };
                    salesRankingList.Add(info);
                }
            }
            return salesRankingList;
        }

        /// <summary>
        /// 按商品销量进行查询 包含系列
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        public IList<SaleRaningShowInfo> GetGoodsSaleRankingBySeriesId(int top, string goodsClassList,
            Guid brandId, string goodsName,
            Guid goodsId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform)
        {
            const string SQL = @"
declare @CurrentBeginDate  datetime,@CurrentEndDate  datetime,
        @CycleBeginDate    datetime,@CycleEndDate    datetime
   set  @CurrentBeginDate = '{0}'
   set  @CurrentEndDate   = '{1}'
   set  @CycleBeginDate   = dateadd(dd,-datediff(dd,@CurrentBeginDate,@CurrentEndDate)-1,@CurrentBeginDate)
   set  @CycleEndDate     = dateadd(ms,-3,@CurrentBeginDate)

select 
 top {4} I1.Id,(
	  case
		 when (select GST.GoodsCode from [lmshop_GoodsSaleTemp] AS GST where GST.GoodsID=I1.Id) is not null
		 then (select GST.GoodsCode from [lmshop_GoodsSaleTemp] AS GST where GST.GoodsID=I1.Id)
		 else '系列编号'   
	  end
  ) as GoodsCode,(
	  case
		 when (select GST.GoodsName from [lmshop_GoodsSaleTemp] AS GST where GST.GoodsID=I1.Id) is not null
		 then (select GST.GoodsName from [lmshop_GoodsSaleTemp] AS GST where GST.GoodsID=I1.Id)
		 else (select SST.SeriesName from [lmshop_SaleSeriesTemp] AS SST where SST.SeriesID=I1.Id)  
	  end
  ) as GoodsName,I1.GoodsPrice,I1.ZeroNumber,I1.SalesNumber,I1.PurchasePrice,I2.PreGoodsPrice,I2.PreZeroNumber,I2.PreSalesNumber
from (
	select
	 (
		  case
			 when (GSS.SeriesId is null or GSS.SeriesId = '00000000-0000-0000-0000-000000000000')
			 then GSS.GoodsID
			 else GSS.SeriesId   
		  end
	  ) as Id,
	  SUM(GSS.SellPrice) AS GoodsPrice,
	  SUM(ISNULL(GSS.AvgSettlePrice,0)) AS PurchasePrice,
	  sum(case when GSS.SellPrice = 0 then GSS.GoodsSales else 0 end) as ZeroNumber,
	  sum(case when GSS.SellPrice <> 0 then GSS.GoodsSales else 0 end) as SalesNumber      
	from  lmshop_GoodsDaySalesStatistics AS GSS with(nolock)
	{2}
	where GSS.DayTime>=@CurrentBeginDate and GSS.DayTime<@CurrentEndDate {3}
	group by (
			  case
				 when (GSS.SeriesId is null or GSS.SeriesId = '00000000-0000-0000-0000-000000000000')
				 then GSS.GoodsID
				 else GSS.SeriesId    
			  end
			 )
) I1   left join  (      
select
 (
	  case
		 when (GSS.SeriesId is null or GSS.SeriesId = '00000000-0000-0000-0000-000000000000')
		 then GSS.GoodsID
		 else GSS.SeriesId   
	  end
  ) as Id,
  SUM(GSS.SellPrice) AS PreGoodsPrice,
  sum(case when GSS.SellPrice = 0 then GSS.GoodsSales else 0 end) as PreZeroNumber,
  sum(case when GSS.SellPrice <> 0 then GSS.GoodsSales else 0 end) as PreSalesNumber      
from  lmshop_GoodsDaySalesStatistics AS GSS with(nolock)
{2}
where GSS.DayTime>=@CycleBeginDate and GSS.DayTime<@CycleEndDate {3}
group by (
		  case
			 when (GSS.SeriesId is null or GSS.SeriesId = '00000000-0000-0000-0000-000000000000')
			 then GSS.GoodsID
			 else GSS.SeriesId    
		  end
         )		 
 ) I2 on i1.Id  = i2.Id
 order by i1.SalesNumber desc
 ";
            var builder = new StringBuilder(" ");
            const string LEFT = @" left join (select ID as SalePlatformId from dbo.SalePlatform where IsActive=1) AS SPF on GSS.SalePlatformId = SPF.SalePlatformId ";
            if (!string.IsNullOrEmpty(goodsClassList))
            {
                builder.AppendFormat(" AND GSS.ClassId IN ({0})", goodsClassList);
            }
            if (Guid.Empty != brandId)
            {
                builder.AppendFormat(" AND GSS.BrandId = '{0}'", brandId);
            }
            if (salefilialeId != Guid.Empty)
            {
                builder.Append(" AND GSS.SaleFilialeId='").Append(salefilialeId).Append("'");
            }
            if (salePlatformId != Guid.Empty)
            {
                builder.Append(" AND GSS.SalePlatformId='").Append(salePlatformId).Append("'");
            }
            if (!string.IsNullOrEmpty(goodsName))
            {
                //builder.AppendFormat(" AND GSS.GoodsName like '%{0}%'", goodsName);
                builder.AppendFormat(" AND GSS.GoodsID in(select GST.GoodsID from [lmshop_GoodsSaleTemp] AS GST where GST.GoodsName like '%{0}%')", goodsName);
            }
            if (Guid.Empty != goodsId)
            {
                builder.AppendFormat(" AND GSS.GoodsID='{0}'", goodsId);
            }
            var salesRankingList = new List<SaleRaningShowInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true,
                string.Format(SQL, startTime, endTime, !isContainDisableSalePlatform ? LEFT : " ", builder, top), null))
            {
                while (dr.Read())
                {
                    var info = new SaleRaningShowInfo
                    {
                        Id = SqlRead.GetString(dr, "GoodsCode").Contains("系列") ? Guid.Empty : SqlRead.GetGuid(dr, "Id"),
                        SeriesId = SqlRead.GetString(dr, "GoodsCode").Contains("系列") ? SqlRead.GetGuid(dr, "Id") : Guid.Empty,
                        GoodsCode = SqlRead.GetString(dr, "GoodsCode"),
                        Name = SqlRead.GetString(dr, "GoodsName"),
                        GoodsPrice = SqlRead.GetDecimal(dr, "GoodsPrice"),
                        SalesNumber = SqlRead.GetInt(dr, "SalesNumber"),
                        ZeroNumber = SqlRead.GetInt(dr, "ZeroNumber"),
                        PreGoodsPrice = SqlRead.GetDecimal(dr, "PreGoodsPrice"),
                        PreSalesNumber = SqlRead.GetInt(dr, "PreSalesNumber"),
                        PreZeroNumber = SqlRead.GetInt(dr, "PreZeroNumber"),
                        PurchasePrice = SqlRead.GetDecimal(dr, "PurchasePrice")
                    };
                    salesRankingList.Add(info);
                }
            }
            return salesRankingList;
        }

        /// <summary>
        /// 按商品销量进行查询 不包含系列
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        public IList<SaleRaningShowInfo> GetGoodsSaleRankingBySale(int top, string goodsClassList,
            Guid brandId, string goodsName,
            Guid goodsId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform)
        {
            const string SQL = @"
---计算上一周期时间
declare @time1 datetime,@time2 datetime,@days int
set @days=datediff(DD,'{0}','{1}')
SET @time2=dateadd(s,-1,'{0}') -- 结束时间 
SET @time1=CONVERT(varchar(100), DATEADD(DD,-@days,@time2), 23);--参数2 开始日期

with cte_temp1 as(
SELECT TOP {4} GSS.GoodsID,GSS.GoodsCode,
SUM(GSS.SellPrice) AS GoodsPrice,
SUM(case when GSS.SellPrice = 0 then GSS.GoodsSales else 0 end) AS ZeroNumber,
SUM(case when GSS.SellPrice <> 0 then GSS.GoodsSales else 0 end) AS SalesNumber
FROM [lmshop_GoodsDaySalesStatistics] AS GSS with(nolock) 
{2}
WHERE GSS.DayTime>='{0}' AND GSS.DayTime<'{1}' {3} 
GROUP BY GSS.GoodsID,GSS.GoodsCode
ORDER BY SalesNumber DESC
),
cte_temp2 as(
SELECT GSS.GoodsID,
SUM(GSS.SellPrice) AS PreGoodsPrice,
SUM(case when GSS.SellPrice = 0 then GSS.GoodsSales else 0 end) AS PreZeroNumber,
SUM(case when GSS.SellPrice <> 0 then GSS.GoodsSales else 0 end) AS PreSalesNumber  
FROM [lmshop_GoodsDaySalesStatistics] AS GSS with(nolock) 
{2}
WHERE GSS.DayTime>=@time1 AND GSS.DayTime<@time2 {3}
GROUP BY GSS.GoodsID
),
cte_temp3 as(
SELECT GSS.GoodsID, SUM(ISNULL(GSS.AvgSettlePrice,0)) AS PurchasePrice 
FROM [lmshop_GoodsDaySalesStatistics] AS GSS with(nolock) 
{2}  
WHERE GSS.DayTime>='{0}' AND GSS.DayTime<'{1}' {3} 
GROUP BY GSS.GoodsID
)

SELECT 
(select GST.GoodsName from [lmshop_GoodsSaleTemp] AS GST where A.GoodsID=GST.GoodsID) as GoodsName,
A.GoodsId,A.GoodsCode,A.GoodsPrice,A.ZeroNumber,A.SalesNumber,
B.PreGoodsPrice,B.PreZeroNumber,B.PreSalesNumber,
C.PurchasePrice
FROM cte_temp1 AS A
Left JOIN cte_temp2 AS B on A.GoodsID=B.GoodsID
inner JOIN cte_temp3 AS C on A.GoodsID=C.GoodsID ";
            var builder = new StringBuilder(" ");
            const string LEFT = @" LEFT JOIN dbo.SalePlatform AS SP ON GSS.SalePlatformId=SP.Id AND SP.IsActive=1 ";
            if (!string.IsNullOrEmpty(goodsClassList))
            {
                builder.AppendFormat(" AND GSS.ClassId IN ({0})", goodsClassList);
            }
            if (Guid.Empty != brandId)
            {
                builder.AppendFormat(" AND GSS.BrandId = '{0}'", brandId);
            }
            if (salefilialeId != Guid.Empty)
            {
                builder.Append(" AND GSS.SaleFilialeId='").Append(salefilialeId).Append("'");
            }
            if (salePlatformId != Guid.Empty)
            {
                builder.Append(" AND GSS.SalePlatformId='").Append(salePlatformId).Append("'");
            }
            if (!string.IsNullOrEmpty(goodsName))
            {
                builder.AppendFormat(" AND GSS.GoodsID in(select GST.GoodsID from [lmshop_GoodsSaleTemp] AS GST where GST.GoodsName like '%{0}%')", goodsName);
            }
            if (Guid.Empty != goodsId)
            {
                builder.AppendFormat(" AND GSS.GoodsID='{0}'", goodsId);
            }
            var salesRankingList = new List<SaleRaningShowInfo>();

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true,
                string.Format(SQL, startTime, endTime, !isContainDisableSalePlatform ? LEFT : " ", builder, top), null))
            {
                while (dr.Read())
                {
                    var info = new SaleRaningShowInfo
                    {
                        Id = SqlRead.GetGuid(dr, "GoodsID"),
                        Name = SqlRead.GetString(dr, "GoodsName"),
                        GoodsCode = SqlRead.GetString(dr, "GoodsCode"),
                        GoodsPrice = SqlRead.GetDecimal(dr, "GoodsPrice"),
                        SalesNumber = SqlRead.GetInt(dr, "SalesNumber"),
                        ZeroNumber = SqlRead.GetInt(dr, "ZeroNumber"),
                        PreGoodsPrice = SqlRead.GetDecimal(dr, "PreGoodsPrice"),
                        PreSalesNumber = SqlRead.GetInt(dr, "PreSalesNumber"),
                        PreZeroNumber = SqlRead.GetInt(dr, "PreZeroNumber"),
                        PurchasePrice = SqlRead.GetDecimal(dr, "PurchasePrice")
                    };
                    salesRankingList.Add(info);
                }
            }
            return salesRankingList;
        }

        /// <summary>
        /// 按平台分组进行查询
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        public IList<SaleRaningShowInfo> GetGoodsSaleRankingBySalePlate(int top, string goodsClassList,
            Guid brandId, string goodsName,
            Guid goodsId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform)
        {
            const string SQL = @"
---计算上一周期时间
declare @time1 datetime,@time2 datetime,@days int
set @days=datediff(DD,'{0}','{1}')
SET @time2=dateadd(s,-1,'{0}') -- 结束时间 
SET @time1=CONVERT(varchar(100), DATEADD(DD,-@days,@time2), 23);--参数2 开始日期

with cte_temp1 as(
SELECT TOP {4} GSS.SalePlatformId,
SUM(GSS.SellPrice) AS GoodsPrice,
SUM(ISNULL(GSS.AvgSettlePrice,0)) AS PurchasePrice,
SUM(case when GSS.SellPrice = 0 then GSS.GoodsSales else 0 end) AS ZeroNumber,
SUM(case when GSS.SellPrice <> 0 then GSS.GoodsSales else 0 end) AS SalesNumber
FROM [lmshop_GoodsDaySalesStatistics] AS GSS with(nolock) 
{2}
WHERE GSS.DayTime>='{0}' AND GSS.DayTime<'{1}' {3} 
GROUP BY GSS.SalePlatformId
ORDER BY SalesNumber DESC
),
cte_temp2 as(
SELECT GSS.SalePlatformId,
SUM(GSS.SellPrice) AS PreGoodsPrice,
SUM(case when GSS.SellPrice = 0 then GSS.GoodsSales else 0 end) AS PreZeroNumber,
SUM(case when GSS.SellPrice <> 0 then GSS.GoodsSales else 0 end) AS PreSalesNumber  
FROM [lmshop_GoodsDaySalesStatistics] AS GSS with(nolock) 
{2}
WHERE GSS.DayTime>=@time1 AND GSS.DayTime<@time2 {3}
GROUP BY GSS.SalePlatformId
)

SELECT 
A.SalePlatformId,A.GoodsPrice,A.ZeroNumber,A.SalesNumber,
B.PreGoodsPrice,B.PreZeroNumber,B.PreSalesNumber,
A.PurchasePrice
FROM cte_temp1 AS A
Left JOIN cte_temp2 AS B on A.SalePlatformId=B.SalePlatformId";
            var builder = new StringBuilder(" ");
            const string LEFT = @" LEFT JOIN dbo.SalePlatform AS SP ON GSS.SalePlatformId=SP.Id AND SP.IsActive=1 ";
            if (!string.IsNullOrEmpty(goodsClassList))
            {
                builder.AppendFormat(" AND GSS.ClassId IN ({0})", goodsClassList);
            }
            if (Guid.Empty != brandId)
            {
                builder.AppendFormat(" AND GSS.BrandId = '{0}'", brandId);
            }
            if (salefilialeId != Guid.Empty)
            {
                builder.Append(" AND GSS.SaleFilialeId='").Append(salefilialeId).Append("'");
            }
            if (salePlatformId != Guid.Empty)
            {
                builder.Append(" AND GSS.SalePlatformId='").Append(salePlatformId).Append("'");
            }
            if (!string.IsNullOrEmpty(goodsName))
            {
                //builder.AppendFormat(" AND GSS.GoodsName like '%{0}%'", goodsName);
                builder.AppendFormat(" AND GSS.GoodsID in(select GST.GoodsID from [lmshop_GoodsSaleTemp] AS GST where GST.GoodsName like '%{0}%')", goodsName);
            }
            if (Guid.Empty != goodsId)
            {
                builder.AppendFormat(" AND GSS.GoodsID='{0}'", goodsId);
            }
            var salesRankingList = new List<SaleRaningShowInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true,
                string.Format(SQL, startTime, endTime, !isContainDisableSalePlatform ? LEFT : " ", builder, top), null))
            {
                while (dr.Read())
                {
                    var info = new SaleRaningShowInfo
                    {
                        Id = SqlRead.GetGuid(dr, "SalePlatformId"),
                        GoodsPrice = SqlRead.GetDecimal(dr, "GoodsPrice"),
                        SalesNumber = SqlRead.GetInt(dr, "SalesNumber"),
                        ZeroNumber = SqlRead.GetInt(dr, "ZeroNumber"),
                        PreGoodsPrice = SqlRead.GetDecimal(dr, "PreGoodsPrice"),
                        PreSalesNumber = SqlRead.GetInt(dr, "PreSalesNumber"),
                        PreZeroNumber = SqlRead.GetInt(dr, "PreZeroNumber"),
                        SalePlatformId = SqlRead.GetGuid(dr, "SalePlatformId"),
                        PurchasePrice = SqlRead.GetDecimal(dr, "PurchasePrice")
                    };
                    salesRankingList.Add(info);
                }
            }
            return salesRankingList;
        }

        /// <summary>
        /// 按品牌分组进行查询
        /// </summary>
        /// <param name="top"></param>
        /// <param name="goodsClassList"></param>
        /// <param name="brandId"></param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isContainDisableSalePlatform"></param>
        /// <returns></returns>
        public IList<SaleRaningShowInfo> GetGoodsSaleRankingByBrand(int top, string goodsClassList,
            Guid brandId, string goodsName,
            Guid goodsId, Guid salefilialeId, Guid salePlatformId,
            DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform)
        {
            const string SQL = @"
---计算上一周期时间
declare @time1 datetime,@time2 datetime,@days int
set @days=datediff(DD,'{0}','{1}')
SET @time2=dateadd(s,-1,'{0}') -- 结束时间 
SET @time1=CONVERT(varchar(100), DATEADD(DD,-@days,@time2), 23);--参数2 开始日期

with cte_temp1 as(
SELECT TOP {4} GSS.BrandId,
SUM(GSS.SellPrice) AS GoodsPrice,
SUM(ISNULL(GSS.AvgSettlePrice,0)) AS PurchasePrice,
SUM(case when GSS.SellPrice = 0 then GSS.GoodsSales else 0 end) AS ZeroNumber,
SUM(case when GSS.SellPrice <> 0 then GSS.GoodsSales else 0 end) AS SalesNumber
FROM [lmshop_GoodsDaySalesStatistics] AS GSS with(nolock) 
{2}
WHERE GSS.DayTime>='{0}' AND GSS.DayTime<'{1}' {3} 
GROUP BY GSS.BrandId
ORDER BY SalesNumber DESC
),
cte_temp2 as(
SELECT GSS.BrandId,
SUM(GSS.SellPrice) AS PreGoodsPrice,
SUM(case when GSS.SellPrice = 0 then GSS.GoodsSales else 0 end) AS PreZeroNumber,
SUM(case when GSS.SellPrice <> 0 then GSS.GoodsSales else 0 end) AS PreSalesNumber  
FROM [lmshop_GoodsDaySalesStatistics] AS GSS with(nolock) 
{2}
WHERE GSS.DayTime>=@time1 AND GSS.DayTime<@time2 {3}
GROUP BY GSS.BrandId
)

SELECT 
A.BrandId,A.GoodsPrice,A.ZeroNumber,A.SalesNumber,
B.PreGoodsPrice,B.PreZeroNumber,B.PreSalesNumber,
A.PurchasePrice
FROM cte_temp1 AS A
Left JOIN cte_temp2 AS B on A.BrandId=B.BrandId";
            var builder = new StringBuilder(" ");
            const string LEFT = @" LEFT JOIN dbo.SalePlatform AS SP ON GSS.SalePlatformId=SP.Id AND SP.IsActive=1 ";
            if (!string.IsNullOrEmpty(goodsClassList))
            {
                builder.AppendFormat(" AND GSS.ClassId IN ({0})", goodsClassList);
            }
            if (Guid.Empty != brandId)
            {
                builder.AppendFormat(" AND GSS.BrandId = '{0}'", brandId);
            }
            if (salefilialeId != Guid.Empty)
            {
                builder.Append(" AND GSS.SaleFilialeId='").Append(salefilialeId).Append("'");
            }
            if (salePlatformId != Guid.Empty)
            {
                builder.Append(" AND GSS.SalePlatformId='").Append(salePlatformId).Append("'");
            }
            if (!string.IsNullOrEmpty(goodsName))
            {
                //builder.AppendFormat(" AND GSS.GoodsName like '%{0}%'", goodsName);
                builder.AppendFormat(" AND GSS.GoodsID in(select GST.GoodsID from [lmshop_GoodsSaleTemp] AS GST where GST.GoodsName like '%{0}%')", goodsName);
            }
            if (Guid.Empty != goodsId)
            {
                builder.AppendFormat(" AND GSS.GoodsID='{0}'", goodsId);
            }
            var salesRankingList = new List<SaleRaningShowInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true,
                string.Format(SQL, startTime, endTime, !isContainDisableSalePlatform ? LEFT : " ", builder, top), null))
            {
                while (dr.Read())
                {
                    var info = new SaleRaningShowInfo
                    {
                        Id = SqlRead.GetGuid(dr, "BrandId"),
                        GoodsPrice = SqlRead.GetDecimal(dr, "GoodsPrice"),
                        SalesNumber = SqlRead.GetInt(dr, "SalesNumber"),
                        ZeroNumber = SqlRead.GetInt(dr, "ZeroNumber"),
                        PreGoodsPrice = SqlRead.GetDecimal(dr, "PreGoodsPrice"),
                        PreSalesNumber = SqlRead.GetInt(dr, "PreSalesNumber"),
                        PreZeroNumber = SqlRead.GetInt(dr, "PreZeroNumber"),
                        BrandId = SqlRead.GetGuid(dr, "BrandId"),
                        PurchasePrice = SqlRead.GetDecimal(dr, "PurchasePrice")
                    };
                    salesRankingList.Add(info);
                }
            }
            return salesRankingList;
        }

        public IList<SalesGoodsRankingInfo> GetGoodsSalesRankingByGoodsIds(List<Guid> goodsIds,
            Guid warehouseId, Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime)
        {

            const string SQL = @"
SELECT 
    GSS.GoodsID,
    GSS.GoodsName,
    GSS.GoodsCode,
    ISNULL(SUM(GSS.SellPrice),0) AS GoodsPrice,
    SUM(GSS.GoodsSales) AS SalesNumber,GSS.SalePlatformId,GSS.SeriesId,GSS.BrandId 
FROM [lmshop_GoodsDaySalesStatistics] AS GSS 
WHERE 1=1 ";

            var cmdText = new StringBuilder();
            cmdText.AppendFormat(SQL);
            if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
            {
                cmdText.Append(string.Format(" And (GSS.DayTime >='{0}' And GSS.DayTime < '{1}')", startTime, endTime));
            }
            var strbGoodsIds = new StringBuilder();
            if (goodsIds.Count > 0)
            {
                foreach (var goodsId in goodsIds)
                {
                    if (string.IsNullOrEmpty(strbGoodsIds.ToString()))
                        strbGoodsIds.Append("'").Append(goodsId).Append("'");
                    else
                        strbGoodsIds.Append(",'").Append(goodsId).Append("'");
                }
                cmdText.Append(" AND GSS.GoodsID IN (").Append(strbGoodsIds).Append(")");
            }
            if (warehouseId != Guid.Empty)
            {
                cmdText.Append(" AND GSS.DeliverWarehouseId='").Append(warehouseId).Append("'");
            }
            if (salefilialeId != Guid.Empty)
            {
                cmdText.Append(" AND GSS.SaleFilialeId='").Append(salefilialeId).Append("'");
            }
            if (salePlatformId != Guid.Empty)
            {
                cmdText.Append(" AND GSS.SalePlatformId='").Append(salePlatformId).Append("'");
            }
            cmdText.Append(" GROUP BY GSS.GoodsID,GSS.GoodsName,GSS.GoodsCode,GSS.SalePlatformId,GSS.SeriesId,GSS.BrandId");
            cmdText.Append(" ORDER BY SalesNumber DESC");
            var salesRankingList = new List<SalesGoodsRankingInfo>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, cmdText.ToString()))
            {
                while (dr.Read())
                {
                    var info = new SalesGoodsRankingInfo
                    {
                        GoodsId = SqlRead.GetGuid(dr, "GoodsID"),
                        GoodsName = SqlRead.GetString(dr, "GoodsName"),
                        GoodsCode = SqlRead.GetString(dr, "GoodsCode"),
                        GoodsPrice = SqlRead.GetDecimal(dr, "GoodsPrice"),
                        SalesNumber = SqlRead.GetInt(dr, "SalesNumber"),
                        SalePlatformId = SqlRead.GetGuid(dr, "SalePlatformId"),
                        SeriesId = SqlRead.GetGuid(dr, "SeriesId"),
                        BrandId = SqlRead.GetGuid(dr, "BrandId")
                    };
                    salesRankingList.Add(info);
                }
            }
            return salesRankingList;
        }

        public bool UpdateGoodsSalesRankingGoodsName(Guid goodsId, string goodsName, string goodsCode,
            Guid seriesId, Guid brandId)
        {
            var builder = new StringBuilder(@"UPDATE lmshop_GoodsDaySalesStatistics SET GoodsName=@GoodsName,GoodsCode=@GoodsCode ");
            if (seriesId != Guid.Empty)
            {
                builder.AppendFormat(" ,SeriesId='{0}'", seriesId);
            }
            if (brandId != Guid.Empty)
            {
                builder.AppendFormat(" ,BrandId='{0}'", brandId);
            }
            builder.Append(" WHERE GoodsID=@GoodsID ");
            var parms = new[]
                            {
                                new SqlParameter("@GoodsName", SqlDbType.VarChar),
                                new SqlParameter("@GoodsCode", SqlDbType.VarChar),
                                new SqlParameter("@GoodsID", SqlDbType.UniqueIdentifier)
                            };
            parms[0].Value = goodsName;
            parms[1].Value = goodsCode;
            parms[2].Value = goodsId;
            try
            {
                var result = SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, builder.ToString(), parms);
                return result > 0;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 更新销量表中系列ID
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="seriesId"></param>
        /// <returns></returns>
        public bool UpdateGoodsSaleSeriesId(Guid goodsId, Guid seriesId)
        {
            const string SQL = @"UPDATE lmshop_GoodsDaySalesStatistics SET SeriesId=@SeriesId WHERE GoodsID=@GoodsID  ";
            var parms = new[]
                            {
                                new SqlParameter("@SeriesId", seriesId),
                                new SqlParameter("@GoodsID", goodsId)
                            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
                return true;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 获取单个商品一段时间内销量
        /// 统计0元销售数量，Li Zhongkai,2015-05-27
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IList<SalesGoodsRankingInfo> GetSalesRankingChart(Guid goodsId, Guid warehouseId,Guid hostingFilialeId,
            Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime)
        {
            var salesRankingList = new List<SalesGoodsRankingInfo>();
            if (goodsId != Guid.Empty)
            {
                var strbSql = new StringBuilder();
                strbSql.Append(@"
select GoodsID, 
    SUM(case when SellPrice = 0 then GoodsSales else 0 end ) as ZeroNumber,
    SUM(case when SellPrice <> 0 then GoodsSales else 0 end ) as SalesNumber,
    SUM(SellPrice) AS GoodsPrice,CONVERT(VARCHAR(10),DayTime,120) as DayTime 
from lmshop_GoodsDaySalesStatistics");
                strbSql.Append(" WHERE GoodsID='").Append(goodsId).Append("'");
                if (warehouseId != Guid.Empty)
                    strbSql.Append(" AND DeliverWarehouseId='").Append(warehouseId).Append("'");
                if (hostingFilialeId != Guid.Empty)
                    strbSql.Append(" AND HostingFilialeId='").Append(hostingFilialeId).Append("'");
                if (salefilialeId != Guid.Empty)
                    strbSql.Append(" AND SaleFilialeId='").Append(salefilialeId).Append("'");
                if (salePlatformId != Guid.Empty)
                    strbSql.Append(" AND SalePlatformId='").Append(salePlatformId).Append("'");
                if (startTime != DateTime.MinValue)
                    strbSql.Append(" AND DayTime>='").Append(startTime.ToString("yyy-MM-dd 00:00:00")).Append("'");
                if (endTime != DateTime.MinValue)
                    strbSql.Append(" AND DayTime<='").Append(endTime.ToString("yyy-MM-dd 23:59:59.997")).Append("'");
                strbSql.Append(" GROUP BY GoodsID,CONVERT(VARCHAR(10),DayTime,120)");
                strbSql.Append(" ORDER BY DayTime");

                using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString()))
                {
                    while (dr.Read())
                    {
                        var info = new SalesGoodsRankingInfo
                        {
                            GoodsId = SqlRead.GetGuid(dr, "GoodsID"),
                            GoodsPrice = SqlRead.GetDecimal(dr, "GoodsPrice"),
                            SalesNumber = SqlRead.GetInt(dr, "SalesNumber"),
                            ZeroNumber = SqlRead.GetInt(dr, "ZeroNumber"),
                            DayTime = SqlRead.GetDateTime(dr, "DayTime")
                        };
                        salesRankingList.Add(info);
                    }
                }
            }
            return salesRankingList;
        }

        /// <summary>
        /// 获取系列商品一段时间内销量
        /// </summary>
        /// <param name="seriesId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="salefilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IList<SalesGoodsRankingInfo> GetSalesRankingChartBySeriesId(Guid seriesId,
            Guid warehouseId,Guid hostingFilialeId, Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime)
        {
            var salesRankingList = new List<SalesGoodsRankingInfo>();
            if (seriesId != Guid.Empty)
            {
                var strbSql = new StringBuilder();
                strbSql.Append(@"
select SeriesId, 
    SUM(case when SellPrice = 0 then GoodsSales else 0 end ) as ZeroNumber,
    SUM(case when SellPrice <> 0 then GoodsSales else 0 end ) as SalesNumber,
    SUM(SellPrice) AS GoodsPrice,CONVERT(VARCHAR(10),DayTime,120) as DayTime 
from lmshop_GoodsDaySalesStatistics");
                strbSql.Append(" WHERE SeriesId='").Append(seriesId).Append("'");
                if (warehouseId != Guid.Empty)
                    strbSql.Append(" AND DeliverWarehouseId='").Append(warehouseId).Append("'");
                if (hostingFilialeId != Guid.Empty)
                    strbSql.Append(" AND HostingFilialeId='").Append(hostingFilialeId).Append("'");
                if (salefilialeId != Guid.Empty)
                    strbSql.Append(" AND SaleFilialeId='").Append(salefilialeId).Append("'");
                if (salePlatformId != Guid.Empty)
                    strbSql.Append(" AND SalePlatformId='").Append(salePlatformId).Append("'");
                if (startTime != DateTime.MinValue)
                    strbSql.Append(" AND DayTime>='").Append(startTime.ToString("yyy-MM-dd 00:00:00")).Append("'");
                if (endTime != DateTime.MinValue)
                    strbSql.Append(" AND DayTime<='").Append(endTime.ToString("yyy-MM-dd 23:59:59.997")).Append("'");
                strbSql.Append(" GROUP BY SeriesId,CONVERT(VARCHAR(10),DayTime,120)");
                strbSql.Append(" ORDER BY DayTime");

                using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString()))
                {
                    while (dr.Read())
                    {
                        var info = new SalesGoodsRankingInfo
                        {
                            SeriesId = SqlRead.GetGuid(dr, "SeriesId"),
                            GoodsPrice = SqlRead.GetDecimal(dr, "GoodsPrice"),
                            SalesNumber = SqlRead.GetInt(dr, "SalesNumber"),
                            ZeroNumber = SqlRead.GetInt(dr, "ZeroNumber"),
                            DayTime = SqlRead.GetDateTime(dr, "DayTime")
                        };
                        salesRankingList.Add(info);
                    }
                }
            }
            return salesRankingList;
        }


        /// <summary>
        /// 供应商销量查询
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IList<CompanySaleStatisticsInfo> SelectGoodsSaleStatisticsInfos(DateTime startTime, DateTime endTime)
        {
            const string SQL = @"SELECT GoodsId,SaleFilialeId,Month(DayTime) AS CurrentMonth,
Sum(GoodsSales) Quantity FROM lmshop_GoodsDaySalesStatistics 
WHERE DayTime BETWEEN @StartTime AND @EndTime GROUP BY SaleFilialeId,GoodsId,Month(DayTime) ORDER BY CurrentMonth ";
            var companySaleStatistics = new List<CompanySaleStatisticsInfo>();
            var parms = new[]
                            {
                                new SqlParameter("@StartTime", startTime),
                                new SqlParameter("@EndTime", endTime)
                            };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (dr.Read())
                {
                    var info = new CompanySaleStatisticsInfo
                    {
                        GoodsId = SqlRead.GetGuid(dr, "GoodsID"),
                        SaleFilialeId = SqlRead.GetGuid(dr, "SaleFilialeId"),
                        Month = SqlRead.GetInt(dr, "CurrentMonth"),
                        Quantity = SqlRead.GetInt(dr, "Quantity")
                    };
                    companySaleStatistics.Add(info);
                }
            }
            return companySaleStatistics;
        }

        /// <summary>
        /// 明细查询
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filialeId"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public IList<CompanySaleStatisticsInfo> SelectCompanySaleStatisticsInfosByFilialeId(DateTime startTime, DateTime endTime, Guid filialeId,
            string companyName)
        {
            var builder = new StringBuilder(@"SELECT GS.GoodsId,GS.SaleFilialeId,GS.M,GS.Quantity,PS.CompanyId,CC.CompanyName FROM
(SELECT GoodsId,SaleFilialeId,Month(DayTime) AS CurrentMonth,
Sum(GoodsSales) Quantity from lmshop_GoodsDaySalesStatistics 
WHERE BETWEEN @StartTime AND @EndTime ");
            if (filialeId != Guid.Empty)
            {
                builder.AppendFormat(" AND SaleFilialeId='{0}' ", filialeId);
            }
            builder.Append(@" GROUP BY SaleFilialeId,GoodsId,Month(DayTime)) AS GS
INNER JOIN lmshop_PurchaseSet PS ON GS.GoodsId=PS.GoodsId
INNER JOIN lmshop_CompanyCussent CC ON PS.CompanyId=CC.CompanyId ");
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                builder.AppendFormat(" WHERE CC.CompanyName LIKE '%{0}%' ", companyName);
            }
            builder.Append(" ORDER BY CurrentMonth ");
            var companySaleStatistics = new List<CompanySaleStatisticsInfo>();
            var parms = new[]
                            {
                                new SqlParameter("@StartTime", startTime),
                                new SqlParameter("@EndTime", endTime)
                            };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), parms))
            {
                while (dr.Read())
                {
                    var info = new CompanySaleStatisticsInfo
                    {
                        GoodsId = SqlRead.GetGuid(dr, "GoodsID"),
                        SaleFilialeId = SqlRead.GetGuid(dr, "SaleFilialeId"),
                        Month = SqlRead.GetInt(dr, "CurrentMonth"),
                        Quantity = SqlRead.GetInt(dr, "Quantity"),
                        CompanyId = SqlRead.GetGuid(dr, "CompanyId"),
                        CompanyName = SqlRead.GetString(dr, "CompanyName")
                    };
                    companySaleStatistics.Add(info);
                }
            }
            return companySaleStatistics;
        }



        /// <summary>
        /// 根据开始时间,截止时间获取指定时间段内的商品销量数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        public IList<GoodsSalesInfo> GetAllRealGoodsSaleNumber(DateTime startTime, DateTime endTime)
        {
            const string SQL = @"SELECT GoodsId,DeliverWarehouseId,SaleFilialeId,RealGoodsID,SUM(GoodsSales) AS Quantity 
  FROM [lmshop_GoodsDaySalesStatistics] with(nolock) where DayTime between @StartTime and @EndTime group by DeliverWarehouseId,SaleFilialeId,RealGoodsID,GoodsId";
            var goodsSalesInfo = new List<GoodsSalesInfo>();
            var parms = new[]
                            {
                                new SqlParameter("@StartTime", SqlDbType.DateTime),
                                new SqlParameter("@EndTime", SqlDbType.DateTime)
                            };
            parms[0].Value = startTime;
            parms[1].Value = endTime;

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (dr.Read())
                {
                    var info = new GoodsSalesInfo
                    {
                        DeliverWarehouseId = SqlRead.GetGuid(dr, "DeliverWarehouseId"),
                        SaleFilialeId = SqlRead.GetGuid(dr, "SaleFilialeId"),
                        RealGoodsId = SqlRead.GetGuid(dr, "RealGoodsId"),
                        GoodsId = SqlRead.GetGuid(dr, "GoodsId"),
                        Quantity = SqlRead.GetInt(dr, "Quantity")
                    };
                    goodsSalesInfo.Add(info);
                }
            }
            return goodsSalesInfo;
        }

        /// <summary>
        /// 根据开始时间,截止时间获取指定时间段内的商品日均销量数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IList<GoodsAvgDaySalesInfo> GetAvgRealGoodsSaleNumber(DateTime startTime, DateTime endTime)
        {
            const string SQL = @"
select	B.DeliverWarehouseId
		,B.SaleFilialeId
		,B.GoodsId
		,B.RealGoodsID
		,cast(SUM(GoodsSales)/StasticsDayCount as decimal(10,3)) Quantity
from
(
	select DeliverWarehouseId,SaleFilialeId,RealGoodsID,min(DayTime) FirstSaleDate, cast(DATEDIFF(day,min(DayTime),@EndTime)+1 as decimal(10,3)) StasticsDayCount
	from lmshop_GoodsDaySalesStatistics with(nolock)
	where DayTime between @StartTime and @EndTime
	group by DeliverWarehouseId,SaleFilialeId,RealGoodsID
) A
inner join lmshop_GoodsDaySalesStatistics B with(nolock) on B.DeliverWarehouseId=A.DeliverWarehouseId and B.SaleFilialeId=A.SaleFilialeId and B.RealGoodsID=A.RealGoodsID
where B.DayTime between @StartTime and @EndTime
group by B.DeliverWarehouseId,B.SaleFilialeId,B.GoodsId,B.RealGoodsID,A.StasticsDayCount";
            var goodsAvgDaySalesInfoList = new List<GoodsAvgDaySalesInfo>();
            var parms = new[]
                            {
                                new SqlParameter("@StartTime", SqlDbType.DateTime),
                                new SqlParameter("@EndTime", SqlDbType.DateTime)
                            };
            parms[0].Value = startTime;
            parms[1].Value = endTime;

            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (dr.Read())
                {
                    var info = new GoodsAvgDaySalesInfo
                    {
                        DeliverWarehouseId = SqlRead.GetGuid(dr, "DeliverWarehouseId"),
                        SaleFilialeId = SqlRead.GetGuid(dr, "SaleFilialeId"),
                        GoodsId = SqlRead.GetGuid(dr, "GoodsId"),
                        RealGoodsId = SqlRead.GetGuid(dr, "RealGoodsId"),
                        AvgQuantity = SqlRead.GetDecimal(dr, "Quantity")
                    };
                    goodsAvgDaySalesInfoList.Add(info);
                }
            }
            return goodsAvgDaySalesInfoList;
        }

        /// <summary>
        /// 根据开始时间,截止时间，子商品ID，销售平台 获取时间段内具体销售平台某个子商品的销量。
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="realGoodsId">子商品ID</param>
        /// <param name="salePlatformId">销售平台</param>
        /// For WMS
        /// <returns></returns>
        /// ww 2016-06-28
        public int GetRealGoodsSaleNumber(DateTime startTime, DateTime endTime, Guid realGoodsId, Guid salePlatformId)
        {
            const string SQL = @"select sum(GoodsSales) from [lmshop_GoodsDaySalesStatistics] with(nolock)
                                    where DayTime between @StartTime and @EndTime
                                    and RealGoodsID = @RealGoodsID
                                    and SalePlatformId = @SalePlatformId";
            var parms = new[]
                            {
                                new SqlParameter("@StartTime", SqlDbType.DateTime),
                                new SqlParameter("@EndTime", SqlDbType.DateTime),
                                new SqlParameter("@RealGoodsID", SqlDbType.UniqueIdentifier),
                                new SqlParameter("@SalePlatformId", SqlDbType.UniqueIdentifier)
                            };

            parms[0].Value = startTime;
            parms[1].Value = endTime;
            parms[2].Value = realGoodsId;
            parms[3].Value = salePlatformId;

            object result = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL, parms);
            if (result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }

        /// <summary>根据销售公司获取其所有商品的销量
        /// </summary>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">截止时间</param>
        /// <param name="saleFilialeId">销售公司ID</param>
        /// <returns>Key：商品GoodsId, Value: 销量</returns>
        public Dictionary<Guid, int> GetGoodsSaleBySaleFilialeId(DateTime fromTime, DateTime toTime, Guid saleFilialeId)
        {
            string sql = string.Format(@"
            SELECT GoodsId,SUM(GoodsSales) GoodsSales  FROM lmshop_GoodsDaySalesStatistics WITH(NOLOCK)
            WHERE SaleFilialeId='{0}' 
            AND DayTime>='{1}' AND DayTime<'{2}'
            GROUP BY GoodsId ", saleFilialeId, fromTime, toTime);
            var saleFilialeIdGoodsSaleDic = new Dictionary<Guid, int>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                while (dr.Read())
                {
                    saleFilialeIdGoodsSaleDic.Add(SqlRead.GetGuid(dr, "GoodsId"), SqlRead.GetInt(dr, "GoodsSales"));
                }
            }
            return saleFilialeIdGoodsSaleDic;
        }

        /// <summary>根据销售平台获取其所有商品的销量
        /// </summary>
        /// <param name="fromTime">开始时间</param>
        /// <param name="toTime">截止时间</param>
        /// <param name="salePlatformIdList">销售平台ID集合</param>
        /// <returns>Key：商品GoodsId, Value: 销量</returns>
        /// zal 2017-07-27
        public Dictionary<Guid, int> GetGoodsSaleBySalePlatformIdList(DateTime fromTime, DateTime toTime, List<Guid> salePlatformIdList)
        {
            string sql = string.Format(@"
            SELECT GoodsId,SUM(GoodsSales) AS GoodsSales 
            FROM lmshop_GoodsDaySalesStatistics WITH(NOLOCK)
            WHERE SalePlatformId IN('{0}') AND DayTime>='{1}' AND DayTime<'{2}'
            GROUP BY GoodsId ", string.Join("','", salePlatformIdList), fromTime, toTime);
            var saleFilialeIdGoodsSaleDic = new Dictionary<Guid, int>();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, null))
            {
                while (dr.Read())
                {
                    saleFilialeIdGoodsSaleDic.Add(SqlRead.GetGuid(dr, "GoodsId"), SqlRead.GetInt(dr, "GoodsSales"));
                }
            }
            return saleFilialeIdGoodsSaleDic;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Environment;
using ERP.Model;


namespace ERP.DAL.Implement.Inventory
{
    public partial class CompanyCussent
    {
        
        #region  应付款查询
        /// <summary>获取供应商应付款 ADD 2015-03-12  陈重文
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="companyName">供应商名称</param>
        /// <param name="year">年份</param>
        /// <param name="initData"></param>
        /// <returns></returns>
        public IList<CompanyPaymentDaysInfo> GetCompanyPaymentDaysList(Guid filialeId, string companyName, int year, bool initData)
        {
            #region [SQL]
            const string SQL = @"
/*-----------------------------------2.有账期--------------------------------------*/
DECLARE @CompanyID      UNIQUEIDENTIFIER            --往来单位ID 
       ,@CompanyName    VARCHAR(128)                --往来单位名称
       ,@PaymentDays    INT                         --账期 
       ,@time1          datetime               --账期日期(根据账期计算账期日期)(开始日期)
       ,@time2          datetime               --账期日期(根据账期计算账期日期)(结束日期)
       ,@Jan              DECIMAL(18,2)            --一月
		,@Feb              DECIMAL(18,2)           --二月
		,@Mar              DECIMAL(18,2)            --三月
		,@Apr              DECIMAL(18,2)            --四月
		,@May              DECIMAL(18,2)            --五月
		,@Jun              DECIMAL(18,2)             --六月
		,@July             DECIMAL(18,2)             --七月
		,@Aug              DECIMAL(18,2)             --八月
		,@Sept             DECIMAL(18,2)            --九月
		,@Oct              DECIMAL(18,2)             --十月
		,@Nov              DECIMAL(18,2)             --十一月
		,@December         DECIMAL(18,2)             --十二月
        ,@Jan1              DECIMAL(18,2)            --一月
		,@Feb2              DECIMAL(18,2)           --二月
		,@Mar3              DECIMAL(18,2)            --三月
		,@Apr4              DECIMAL(18,2)            --四月
		,@May5              DECIMAL(18,2)            --五月
		,@Jun6              DECIMAL(18,2)             --六月
		,@July7             DECIMAL(18,2)             --七月
		,@Aug8              DECIMAL(18,2)             --八月
		,@Sept9             DECIMAL(18,2)            --九月
		,@Oct10              DECIMAL(18,2)             --十月
		,@Nov11              DECIMAL(18,2)             --十一月
		,@December12         DECIMAL(18,2)             --十二月		
		,@TheMonthNum      int                       --月份循环值
		,@TempPaymentDaysDue DECIMAL(18,2)           --临时账期应付款
		,@SumInAccountReceivable DECIMAL(18,2)       --入库总金额
		,@SumOutAccountReceivable DECIMAL(18,2)       --出库总金额
		,@SumInIsChecked DECIMAL(18,2)        --已对账总金额
		,@SumOutIsChecked DECIMAL(18,2)       --未账总金额
		,@TempIsChecked DECIMAL(18,2)         --临时已付应付款     
		
--申明一个临时表来存储往来单位账期数据
DECLARE  @CompanyPaymentDaysInfo TABLE(
    CompanyName      VARCHAR(128)      NULL,        --往来单位名称
    PaymentDays      INT               NULL,        --账期
    Jan              DECIMAL(18,2)     NULL,        --一月
    Feb              DECIMAL(18,2)     NULL,        --二月
    Mar              DECIMAL(18,2)     NULL,        --三月
    Apr              DECIMAL(18,2)     NULL,        --四月
    May              DECIMAL(18,2)     NULL,        --五月
    Jun              DECIMAL(18,2)     NULL,        --六月
    July             DECIMAL(18,2)     NULL,        --七月
    Aug              DECIMAL(18,2)     NULL,        --八月
    Sept             DECIMAL(18,2)     NULL,        --九月
    Oct              DECIMAL(18,2)     NULL,        --十月
    Nov              DECIMAL(18,2)     NULL,        --十一月
    December         DECIMAL(18,2)     NULL,         --十二月
    Jan1              DECIMAL(18,2)    NULL,        --一月
    Feb2              DECIMAL(18,2)    NULL,        --二月
    Mar3              DECIMAL(18,2)    NULL,        --三月
    Apr4              DECIMAL(18,2)    NULL,        --四月
    May5              DECIMAL(18,2)    NULL,        --五月
    Jun6              DECIMAL(18,2)    NULL,        --六月
    July7             DECIMAL(18,2)    NULL,        --七月
    Aug8              DECIMAL(18,2)    NULL,        --八月
    Sept9             DECIMAL(18,2)    NULL,        --九月
    Oct10              DECIMAL(18,2)   NULL,        --十月
    Nov11              DECIMAL(18,2)   NULL,        --十一月
    December12         DECIMAL(18,2)   NULL         --十二月    
) 
--循环变量
DECLARE @W_Count INT, @W_Num INT ,@FilileIdIsoutTrue  uniqueidentifier,@FilileIdIsoutFalse  uniqueidentifier
----临时往来单位表
SELECT ROW_NUMBER() OVER(ORDER BY CompanyID) AS RowID,CompanyId,CompanyName,PaymentDays INTO #TempCompanyCussent 
    FROM lmShop_CompanyCussent 
    WHERE {6} {0}--参数1 供应商名称
-- (PaymentDays>0 OR (PaymentDays=0 AND CompanyType=1))
--循环变量赋值
SET @W_Num =1
SELECT @W_Count=COUNT(0) FROM #TempCompanyCussent
--循环往来单位
WHILE(@W_Num <= @W_Count)
BEGIN
        --获取往来单位
		SELECT @CompanyID = CompanyId,@CompanyName=CompanyName,@PaymentDays=PaymentDays FROM #TempCompanyCussent  WHERE RowID = @w_num
		IF({5}=0)
		BEGIN
			SET @PaymentDays=0
		END
		SET @time1= dateadd(mm,-@PaymentDays,DATEADD(MM,DATEDIFF(MM,0,'{1}'),0))--参数2 开始日期
        SET @time2= dateadd(mm,-@PaymentDays,DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,'{2}')+1, 0)))  --参数2 结束日期      		
		--清除上次循环的变量值
		SET @Jan  =NULL
		SET @Feb  =NULL
		SET @Mar  =NULL
		SET @Apr  =NULL
		SET @May  =NULL
		SET @Jun  =NULL
		SET @July =NULL
		SET @Aug  =NULL
		SET @Sept =NULL
		SET @Oct  =NULL
		SET @Nov  =NULL
		SET @December  =NULL	
		SET @Jan1  =NULL
		SET @Feb2  =NULL
		SET @Mar3  =NULL
		SET @Apr4  =NULL
		SET @May5  =NULL
		SET @Jun6  =NULL
		SET @July7 =NULL
		SET @Aug8  =NULL
		SET @Sept9 =NULL
		SET @Oct10  =NULL
		SET @Nov11  =NULL
		SET @December12  =NULL												
				SET @TheMonthNum=1
				WHILE(@TheMonthNum <=12)
				BEGIN
				      if(@TheMonthNum=1)
				      begin
						SET @time1=DATEADD(mm,0,@time1)
						SET @time2=DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0))
				      end else
				      begin
						SET @time1=DATEADD(mm,1,@time1)
						SET @time2=DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0))
				      end	
				    --开票   	      					  					  
					SELECT
						@SumInAccountReceivable = sum(case when LinkTradeType = 1  then abs(AccountReceivable) else 0   end),
						@SumInIsChecked=sum(case when LinkTradeType = 1 and IsChecked =0  then abs(AccountReceivable) else 0   end) ,
						@SumOutAccountReceivable = sum(case when LinkTradeType = 2 then abs(AccountReceivable) else 0   end) ,
						@SumOutIsChecked=sum(case when LinkTradeType = 2 and IsChecked =0  then abs(AccountReceivable) else 0   end) 
					FROM lmShop_Reckoning 
					with(nolock)
					where ThirdCompanyID = @CompanyID 
					and   DateCreated between  @time1 and  @time2 
					and AuditingState=1 AND [State]<>2 and LinkTradeType in (1,2) 
					and (
						LinkTradeCode like 'RK%' or LinkTradeCode like 'SL%'   or LinkTradeCode like 'SO%'
						or LinkTradeCode like 'SI%'   or LinkTradeCode like 'TSI%'  or LinkTradeCode like 'TSO%'
						or LinkTradeCode like 'TR%') and TradeCode not like 'GT%'
					and IsOut  = {3} {4}
					      					
				    set @TempPaymentDaysDue=ISNULL(@SumInAccountReceivable,0)-ISNULL(@SumOutAccountReceivable,0) 
				    set @TempIsChecked     =ISNULL(@SumInIsChecked,0)-ISNULL(@SumOutIsChecked,0) 			        					
			  if(@TheMonthNum  = 1)  begin 
					--1月份应付款 
					set @Jan  = @TempPaymentDaysDue	
					set @Jan1 = @TempIsChecked						
				end
			  if(@TheMonthNum  = 2)  begin 
					--2月份应付款 
					set @Feb  = @TempPaymentDaysDue
					set @Feb2 =	@TempIsChecked				
				end
			  if(@TheMonthNum  = 3)  begin 
					--3月份应付款 
					set @Mar=@TempPaymentDaysDue
					set @Mar3 =	@TempIsChecked					
				end
			  if(@TheMonthNum  = 4)  begin 
					--4月份应付款 
					set @Apr=@TempPaymentDaysDue	
					set @Apr4 =	@TempIsChecked				
				end
			  if(@TheMonthNum  = 5)  begin 
					--5月份应付款 
					set @May=@TempPaymentDaysDue
					set @May5 =	@TempIsChecked					
				end
			  if(@TheMonthNum  = 6)  begin 
					--6月份应付款 
					set @Jun=@TempPaymentDaysDue
					set @Jun6 =	@TempIsChecked					
				end
			  if(@TheMonthNum  = 7)  begin 
					--7月份应付款 
					set @July=@TempPaymentDaysDue
					set @July7 = @TempIsChecked
				end
			  if(@TheMonthNum  = 8)  begin 
					--8月份应付款 
					set @Aug=@TempPaymentDaysDue
					set @Aug8 =	@TempIsChecked
				end	
			  if(@TheMonthNum  = 9)  begin 
					--9月份应付款 
					set @Sept=@TempPaymentDaysDue
					set @Sept9 =	@TempIsChecked
				end	
			  if(@TheMonthNum  = 10) begin 
					--10月份应付款 
					set @Oct=@TempPaymentDaysDue
					set @Oct10 =	@TempIsChecked
				end	
			  if(@TheMonthNum  = 11) begin 
					--11月份应付款 
					set @Nov=@TempPaymentDaysDue
					set @Nov11 =	@TempIsChecked
				end	
			  if(@TheMonthNum  = 12) begin 
					--12月份应付款 
					set @December=@TempPaymentDaysDue
					set @December12 =	@TempIsChecked
				end
				SET @TheMonthNum +=1
				SET @TempPaymentDaysDue = null		
		 end  --循环结束
        --插入临时表 				 				 		 	 		 		    					
		INSERT INTO @CompanyPaymentDaysInfo (CompanyName,PaymentDays,Jan,Feb,Mar,Apr,May,Jun,July,Aug,Sept,Oct,Nov,December
		,Jan1,Feb2,Mar3,Apr4,May5,Jun6,July7,Aug8,Sept9,Oct10,Nov11,December12)
		SELECT @CompanyName,@PaymentDays,@Jan,@Feb,@Mar,@Apr,@May,@Jun,@July,@Aug,@Sept,@Oct,@Nov,@December	
		,@Jan1,@Feb2,@Mar3,@Apr4,@May5,@Jun6,@July7,@Aug8,@Sept9,@Oct10,@Nov11,@December12
			  		
   SET 	@W_Num +=1	 --循环累加
END
--输出往来单位账期数据
SELECT CompanyName,PaymentDays,Jan,Feb,Mar,Apr,May,Jun,July,Aug,Sept,Oct,Nov,December
		,Jan1,Feb2,Mar3,Apr4,May5,Jun6,July7,Aug8,Sept9,Oct10,Nov11,December12 FROM @CompanyPaymentDaysInfo WHERE CompanyName  NOT IN
(
	SELECT CompanyName FROM @CompanyPaymentDaysInfo  where 
	Jan=0 and Feb=0 and Mar=0 and Apr=0 and May=0 and 
	Jun=0 and July=0 and Aug=0 and Sept=0 and Oct=0 and Nov=0 and December=0
)
--删除临时表
DROP TABLE #TempCompanyCussent";
            #endregion

            var filialeIdStr = string.Empty;
            var companyNameStr = string.Empty;
            var isOut = 0;
            if (filialeId != Guid.Empty)
            {
                filialeIdStr = string.Format(" AND FilialeId='{0}'", filialeId);
                isOut = 1;
            }
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                companyNameStr = string.Format(" AND CompanyName LIKE '%{0}%'", companyName);
            }
            var startTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year));
            var endTime = Convert.ToDateTime(string.Format("{0}-12-01 00:00:00", year));
            var newSQL = string.Format(SQL, companyNameStr, startTime, endTime, isOut, filialeIdStr, initData ? 1 : 0, initData ?
                companyName==null?" PaymentDays>0 ":"(PaymentDays >0 OR (PaymentDays=0 AND CompanyType=1))" : " CompanyType=1 ");
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CompanyPaymentDaysInfo>(true, newSQL).ToList();
            }
        }

        /// <summary>获取没有设置账期的供应商当月应付款 ADD 2015-06-11  陈重文
        /// 
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="companyName">供应商名称</param>
        /// <param name="initData"></param>
        /// <returns></returns>
        public IList<CompanyPaymentDaysInfo> GetCompanyNotPaymentDaysList(Guid filialeId, string companyName, bool initData)
        {
            #region [SQL]

            const string SQL = @"
        DECLARE @time1          datetime               --账期日期(根据账期计算账期日期)(开始日期)
       ,@time2          datetime               --账期日期(根据账期计算账期日期)(结束日期)
		,@SumInAccountReceivable DECIMAL(18,2)       --入库总金额
		,@SumOutAccountReceivable DECIMAL(18,2)       --出库总金额
		,@TempPaymentDaysDue DECIMAL(18,2)           --临时账期应付款
		,@CompanyID      UNIQUEIDENTIFIER            --往来单位ID 
		,@CompanyName    VARCHAR(128)                --往来单位名称
SET @time1= DATEADD(MM,DATEDIFF(MM,0,GETDATE()),0)--开始日期
SET @time2= DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,GETDATE())+1, 0))  --结束日期    

--申明一个临时表来存储往来账
DECLARE @TEMP_Reckoning TABLE      
(      
  CompanyId UNIQUEIDENTIFIER,
  FilialeId UNIQUEIDENTIFIER,
  AccountReceivable DECIMAL(18,4),
  LinkTradeType int,
  IsOut bit,
  IsChecked int
);

INSERT INTO @TEMP_Reckoning (CompanyId,FilialeId,AccountReceivable,LinkTradeType,IsOut,IsChecked)
select ThirdCompanyID,FilialeId,AccountReceivable,LinkTradeType,IsOut,IsChecked from lmShop_Reckoning with(nolock) where  DateCreated BETWEEN  @time1 AND  @time2 AND AuditingState=1 AND [STATE]=1 and LinkTradeType in (1,2)

--申明一个临时表来存储往来单位账期数据
DECLARE  @CompanyPaymentDaysInfo TABLE(
    CompanyName      VARCHAR(128)      NULL,           --往来单位名称
    PaymentDaysDue   DECIMAL(18,2)     NULL            --当前账期
) 

--循环变量
DECLARE @W_Count INT, @W_Num INT 
----临时往来单位表
SELECT 
	ROW_NUMBER() OVER(ORDER BY CompanyID) AS RowID,CompanyId,CompanyName,PaymentDays
	INTO #TempCompanyCussent 
FROM lmShop_CompanyCussent
WHERE PaymentDays=0 and CompanyType=1 {0} --参数1 供应商名称

--循环变量赋值
SET @W_Num =1
SELECT @W_Count=COUNT(0) FROM #TempCompanyCussent
--循环往来单位
WHILE(@W_Num <= @W_Count)
BEGIN
	--获取往来单位
	SELECT @CompanyID = CompanyId,@CompanyName=CompanyName
	FROM #TempCompanyCussent  WHERE RowID = @w_num
	--入库总金额		      					  					  
	SELECT @SumInAccountReceivable=SUM(ABS(AccountReceivable)) FROM @TEMP_Reckoning
	WHERE AND CompanyID = @CompanyID AND IsOut ={2} --参数3 IsOut值 
    AND [State]<>2  AND LEFT(TradeCode,2)<>'GT'	
    AND LEFT(LinkTradeCode,2) NOT IN('LI','LO','BI','BO') 
	AND LinkTradeType=1     
    {1}  --参数2  公司ID 
    {3}  --入库单往来帐			
	--出库总金额		      					  					  
	SELECT @SumOutAccountReceivable=SUM(ABS(AccountReceivable)) FROM @TEMP_Reckoning
	WHERE CompanyID = @CompanyID
	AND  AND [State]<>2  AND LEFT(TradeCode,2)<>'GT'	
    AND LEFT(LinkTradeCode,2) NOT IN('LI','LO','BI','BO') 
	AND IsOut  ={2} --参数3 IsOut值
	AND LinkTradeType=2 --出库单往来帐	
    {1}  --参数2  公司ID			      					
	set @TempPaymentDaysDue=ISNULL(@SumInAccountReceivable,0)-ISNULL(@SumOutAccountReceivable,0)
	--插入临时表 				 				 		 	 		 		    					
	INSERT INTO @CompanyPaymentDaysInfo (CompanyName,PaymentDaysDue)
	SELECT @CompanyName,@TempPaymentDaysDue 
	set @TempPaymentDaysDue=0			        					
	SET @W_Num +=1	 --循环累加		  
END

select CompanyName,PaymentDaysDue from @CompanyPaymentDaysInfo where PaymentDaysDue>0

drop table #TempCompanyCussent";

            #endregion

            var filialeIdStr = string.Empty;
            var companyNameStr = string.Empty;
            var isOut = 0;
            if (filialeId != Guid.Empty)
            {
                filialeIdStr = string.Format(" AND FilialeId='{0}'", filialeId);
                isOut = 1;
            }
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                companyNameStr = string.Format(" AND CompanyName LIKE '{0}%'", companyName);
            }
            var newSQL = string.Format(SQL, companyNameStr, filialeIdStr, isOut, initData ? " " : "AND IsChecked=0 ");
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CompanyPaymentDaysInfo>(true, newSQL).ToList();
            }
        }

        /// <summary>  新加获取无账期数据 add by liangcanren at 2015-08-10
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public CompanyPaymentDaysInfo GetCompanyNotPaymentDaysInfos(Guid filialeId, int year)
        {
            const string SQL = @"DECLARE @time1          datetime               --账期日期(根据账期计算账期日期)(开始日期)
       ,@time2          datetime               --账期日期(根据账期计算账期日期)(结束日期)
		,@Jan              DECIMAL(18,2)            --一月
		,@Feb              DECIMAL(18,2)           --二月
		,@Mar              DECIMAL(18,2)            --三月
		,@Apr              DECIMAL(18,2)            --四月
		,@May              DECIMAL(18,2)            --五月
		,@Jun              DECIMAL(18,2)             --六月
		,@July             DECIMAL(18,2)             --七月
		,@Aug              DECIMAL(18,2)             --八月
		,@Sept             DECIMAL(18,2)            --九月
		,@Oct              DECIMAL(18,2)             --十月
		,@Nov              DECIMAL(18,2)             --十一月
		,@December         DECIMAL(18,2)             --十二月
        ,@Jan1              DECIMAL(18,2)            --一月
		,@Feb2              DECIMAL(18,2)           --二月
		,@Mar3              DECIMAL(18,2)            --三月
		,@Apr4              DECIMAL(18,2)            --四月
		,@May5              DECIMAL(18,2)            --五月
		,@Jun6              DECIMAL(18,2)             --六月
		,@July7             DECIMAL(18,2)             --七月
		,@Aug8              DECIMAL(18,2)             --八月
		,@Sept9             DECIMAL(18,2)            --九月
		,@Oct10              DECIMAL(18,2)             --十月
		,@Nov11              DECIMAL(18,2)             --十一月
		,@December12         DECIMAL(18,2)             --十二月		
		,@TheMonthNum      int                       --月份循环值
		,@TempPaymentDaysDue DECIMAL(18,2)           --临时账期应付款
		,@SumInAccountReceivable DECIMAL(18,2)       --入库总金额
		,@SumOutAccountReceivable DECIMAL(18,2)       --出库总金额
		,@SumInIsChecked DECIMAL(18,2)        --已对账总金额
		,@SumOutIsChecked DECIMAL(18,2)       --未账总金额
		,@TempIsChecked DECIMAL(18,2)         --临时已付应付款   
		
SET @time1= dateadd(mm,0,DATEADD(MM,DATEDIFF(MM,0,'{0}'),0))--参数2 开始日期
SET @time2= dateadd(mm,0,DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,'{1}')+1, 0)))  --参数2 结束日期  

SET @TheMonthNum=1
WHILE(@TheMonthNum <=12)
BEGIN
	if(@TheMonthNum=1)
	  begin
		SET @time1=DATEADD(mm,0,@time1)
		SET @time2=DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0))
	  end else
	  begin
		SET @time1=DATEADD(mm,1,@time1)
		SET @time2=DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0))
	  end	
	SELECT
		@SumInAccountReceivable = sum(case when LinkTradeType = 1  then abs(AccountReceivable) else 0   end),
		@SumInIsChecked=sum(case when LinkTradeType = 1 and IsChecked =0  then abs(AccountReceivable) else 0   end) ,
		@SumOutAccountReceivable = sum(case when LinkTradeType = 2 then abs(AccountReceivable) else 0   end) ,
		@SumOutIsChecked=sum(case when LinkTradeType = 2 and IsChecked =0  then abs(AccountReceivable) else 0   end) 
	FROM lmShop_Reckoning AS R with(nolock) 
    INNER JOIN lmshop_CompanyCussent CC ON R.ThirdCompanyID=CC.CompanyID AND CC.PaymentDays=0 AND CC.CompanyType=1
	where R.DateCreated between  @time1 and  @time2 
	and AuditingState=1 AND R.[State]<>2 and LinkTradeType in (1,2) 
	and (
		LinkTradeCode like 'RK%' or LinkTradeCode like 'SL%'   or LinkTradeCode like 'SO%'
		or LinkTradeCode like 'SI%'   or LinkTradeCode like 'TSI%'  or LinkTradeCode like 'TSO%'
		or LinkTradeCode like 'TR%') and TradeCode not like 'GT%'
	and IsOut  = {2} {3}
	      					
    set @TempPaymentDaysDue=ISNULL(@SumInAccountReceivable,0)-ISNULL(@SumOutAccountReceivable,0) 
    set @TempIsChecked     =ISNULL(@SumInIsChecked,0)-ISNULL(@SumOutIsChecked,0) 	
	
	if(@TheMonthNum  = 1)  begin 
					--1月份应付款 
					set @Jan  = @TempPaymentDaysDue	
					set @Jan1 = @TempIsChecked						
				end
			  if(@TheMonthNum  = 2)  begin 
					--2月份应付款 
					set @Feb  = @TempPaymentDaysDue
					set @Feb2 =	@TempIsChecked				
				end
			  if(@TheMonthNum  = 3)  begin 
					--3月份应付款 
					set @Mar=@TempPaymentDaysDue
					set @Mar3 =	@TempIsChecked					
				end
			  if(@TheMonthNum  = 4)  begin 
					--4月份应付款 
					set @Apr=@TempPaymentDaysDue	
					set @Apr4 =	@TempIsChecked				
				end
			  if(@TheMonthNum  = 5)  begin 
					--5月份应付款 
					set @May=@TempPaymentDaysDue
					set @May5 =	@TempIsChecked					
				end
			  if(@TheMonthNum  = 6)  begin 
					--6月份应付款 
					set @Jun=@TempPaymentDaysDue
					set @Jun6 =	@TempIsChecked					
				end
			  if(@TheMonthNum  = 7)  begin 
					--7月份应付款 
					set @July=@TempPaymentDaysDue
					set @July7 = @TempIsChecked
				end
			  if(@TheMonthNum  = 8)  begin 
					--8月份应付款 
					set @Aug=@TempPaymentDaysDue
					set @Aug8 =	@TempIsChecked
				end	
			  if(@TheMonthNum  = 9)  begin 
					--9月份应付款 
					set @Sept=@TempPaymentDaysDue
					set @Sept9 =	@TempIsChecked
				end	
			  if(@TheMonthNum  = 10) begin 
					--10月份应付款 
					set @Oct=@TempPaymentDaysDue
					set @Oct10 =	@TempIsChecked
				end	
			  if(@TheMonthNum  = 11) begin 
					--11月份应付款 
					set @Nov=@TempPaymentDaysDue
					set @Nov11 =	@TempIsChecked
				end	
			  if(@TheMonthNum  = 12) begin 
					--12月份应付款 
					set @December=@TempPaymentDaysDue
					set @December12 =@TempIsChecked
				end
	set @TempPaymentDaysDue=0			  
	SET @TheMonthNum +=1	 --循环累加		  
END

select @Jan AS Jan,@Feb AS Feb,@Mar AS Mar,@Apr AS Apr,@May AS May,@Jun AS Jun,@July AS July,@Aug AS Aug,
@Sept AS Sept,@Oct AS Oct,@Nov AS Nov,@December AS December	
		,@Jan1 AS Jan1,@Feb2 AS Feb2,@Mar3 AS ar3,@Apr4 AS Apr4,@May5 AS May5,@Jun6 AS Jun6,
		@July7 AS July7 ,@Aug8 AS Aug8,@Sept9 AS Sept9,@Oct10 AS Oct10,@Nov11 AS Nov11,@December12 AS December12
";
            var filialeIdStr = string.Empty;
            var isOut = 0;
            if (filialeId != Guid.Empty)
            {
                filialeIdStr = string.Format(" AND FilialeId='{0}'", filialeId);
                isOut = 1;
            }
            var startTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year));
            var endTime = Convert.ToDateTime(string.Format("{0}-12-01 00:00:00", year));
            var newSQL = string.Format(SQL, startTime, endTime, isOut, filialeIdStr);
            using (var db = DatabaseFactory.Create())
            {
                return db.Single<CompanyPaymentDaysInfo>(true, newSQL);
            }
        }
#endregion

        #region  采购入库金额报表
        /// <summary>获取采购入库金额 ADD 2015-08-07  梁灿仁
        /// </summary>
        /// <param name="filialeId">公司ID</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        public CompanyPaymentDaysInfo GetPurchasingCompanyPaymentDaysInfo(Guid filialeId, int year)
        {
            #region [SQL]
            const string SQL = @"
declare @time1 datetime,
@time2 datetime,
@TheMonthNum int,
@TempPaymentDaysDue decimal(18,4),
@FilialeId UNIQUEIDENTIFIER

declare @Jan DECIMAL(18,4) ,
@Feb    DECIMAL(18,4) ,
@Mar    DECIMAL(18,4) ,
@Apr    DECIMAL(18,4) ,
@May    DECIMAL(18,4) ,
@Jun    DECIMAL(18,4) ,
@July    DECIMAL(18,4) ,
@Aug    DECIMAL(18,4) ,
@Sept    DECIMAL(18,4),
@Oct    DECIMAL(18,4) ,
@Nov    DECIMAL(18,4) ,
@December    DECIMAL(18,4)       

set @FilialeId='{0}'
SET @time1= dateadd(mm,0,DATEADD(MM,DATEDIFF(MM,0,'{1}'),0))--参数2 开始日期
SET @time2= dateadd(mm,0,DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,'{2}')+1, 0)))  --参数2 结束日期 
		    				
SET @TheMonthNum=1
WHILE(@TheMonthNum <=12)
BEGIN
	 set @TempPaymentDaysDue=0
      if(@TheMonthNum=1)
      begin
		SET @time1=DATEADD(mm,0,@time1)
		SET @time2=DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0))
      end else
      begin
		SET @time1=DATEADD(mm,1,@time1)
		SET @time2=DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0))
      end	
    --开票         					  					  
	SELECT @TempPaymentDaysDue=
		(ISNULL(sum(case when LinkTradeType = 1  then abs(AccountReceivable) else 0   end),0)
		-ISNULL(sum(case when LinkTradeType = 2 then abs(AccountReceivable) else 0   end),0))
	FROM lmShop_Reckoning with(nolock)
	where  DateCreated between  @time1 and  @time2 
	and AuditingState=1 AND [State]<>2 and LinkTradeType in (1,2) 
	and (
		LinkTradeCode like 'RK%' or LinkTradeCode like 'SL%'   or LinkTradeCode like 'SO%'
		or LinkTradeCode like 'SI%'   or LinkTradeCode like 'TSI%'  or LinkTradeCode like 'TSO%'
		or LinkTradeCode like 'TR%') and TradeCode not like 'GT%'
	and IsOut  = {3} {4}
	group by Month(DateCreated)	
	
	if(@TheMonthNum  = 1)  begin 
		--1月份应付款 
		set @Jan  = @TempPaymentDaysDue					
				end
			  if(@TheMonthNum  = 2)  begin 
					--2月份应付款 
					set @Feb  = @TempPaymentDaysDue		
				end
			  if(@TheMonthNum  = 3)  begin 
					--3月份应付款 
					set @Mar=@TempPaymentDaysDue			
				end
			  if(@TheMonthNum  = 4)  begin 
					--4月份应付款 
					set @Apr=@TempPaymentDaysDue			
				end
			  if(@TheMonthNum  = 5)  begin 
					--5月份应付款 
					set @May=@TempPaymentDaysDue			
				end
			  if(@TheMonthNum  = 6)  begin 
					--6月份应付款 
					set @Jun=@TempPaymentDaysDue			
				end
			  if(@TheMonthNum  = 7)  begin 
					--7月份应付款 
					set @July=@TempPaymentDaysDue
				end
			  if(@TheMonthNum  = 8)  begin 
					--8月份应付款 
					set @Aug=@TempPaymentDaysDue
				end	
			  if(@TheMonthNum  = 9)  begin 
					--9月份应付款 
					set @Sept=@TempPaymentDaysDue
				end	
			  if(@TheMonthNum  = 10) begin 
					--10月份应付款 
					set @Oct=@TempPaymentDaysDue
				end	
			  if(@TheMonthNum  = 11) begin 
					--11月份应付款 
					set @Nov=@TempPaymentDaysDue
				end	
			  if(@TheMonthNum  = 12) begin 
					--12月份应付款 
					set @December=@TempPaymentDaysDue
				end	        					
SET @TheMonthNum +=1
end  --循环结束

--输出往来单位账期数据
SELECT @FilialeId AS FilialeId,
@Jan AS Jan,@Feb AS Feb,@Mar AS Mar,@Apr AS Apr,@May AS May,@Jun AS Jun,@July AS July,
@Aug AS Aug,@Sept AS Sept,@Oct AS Oct,@Nov AS Nov,@December AS December	";
            #endregion
            var filialeIdStr = string.Empty;
            var isOut = 0;
            if (filialeId != Guid.Empty)
            {
                filialeIdStr = string.Format(" AND FilialeId='{0}'", filialeId);
                isOut = 1;
            }
            var startTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year));
            var endTime = Convert.ToDateTime(string.Format("{0}-12-01 00:00:00", year));
            var newSQL = string.Format(SQL, filialeId, startTime, endTime, isOut, filialeIdStr);
            using (var db = DatabaseFactory.Create())
            {
                return db.Single<CompanyPaymentDaysInfo>(true, newSQL);
            }
        }

        /// <summary>
        /// 获取采购供应商入库金额明细 ADD 2015-08-07  梁灿仁
        /// </summary>
        /// <param name="filialeId"></param>
        /// <param name="year"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public IList<CompanyPaymentDaysInfo> GetPurchasingCompanyPaymentDaysInfos(Guid filialeId, int year, string companyName)
        {
            #region [SQL]
            const string SQL = @"declare @time1 datetime,@time2 datetime,@TheMonthNum int

--申明一个临时表来存储往来单位账期数据
DECLARE  @CompanyPaymentDaysInfo TABLE(
    CompanyId      UNIQUEIDENTIFIER      NULL,        --往来单位名称
    CurrentMonth      INT               NULL,        --账期
    TotalAmount    DECIMAL(18,4)     NULL       --一月
) 

SET @time1= dateadd(mm,0,DATEADD(MM,DATEDIFF(MM,0,'{0}'),0))--参数2 开始日期
SET @time2= dateadd(mm,0,DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,'{1}')+1, 0)))  --参数2 结束日期 
		    				
SET @TheMonthNum=1
WHILE(@TheMonthNum <=12)
BEGIN
      if(@TheMonthNum=1)
      begin
		SET @time1=DATEADD(mm,0,@time1)
		SET @time2=DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0))
      end else
      begin
		SET @time1=DATEADD(mm,1,@time1)
		SET @time2=DATEADD(MS,-3,DATEADD(MM, DATEDIFF(MM,0,@time1)+1, 0))
      end	
    --开票   
    INSERT INTO @CompanyPaymentDaysInfo 	      					  					  
	SELECT ThirdCompanyID AS CompanyId,Month(DateCreated) AS CurrentMonth,
		(ISNULL(sum(case when LinkTradeType = 1  then abs(AccountReceivable) else 0   end),0)
		-ISNULL(sum(case when LinkTradeType = 2 then abs(AccountReceivable) else 0   end),0)) AS TotalAmount
	FROM lmShop_Reckoning with(nolock)
	where  DateCreated between  @time1 and  @time2 
	and AuditingState=1 AND [State]<>2 and LinkTradeType in (1,2) 
	and (
		LinkTradeCode like 'RK%' or LinkTradeCode like 'SL%'   or LinkTradeCode like 'SO%'
		or LinkTradeCode like 'SI%'   or LinkTradeCode like 'TSI%'  or LinkTradeCode like 'TSO%'
		or LinkTradeCode like 'TR%') and TradeCode not like 'GT%'
	and IsOut  = {2} {3}
	group by ThirdCompanyID,Month(DateCreated)	
		        					
SET @TheMonthNum +=1
end  --循环结束

--输出往来单位账期数据
SELECT C.*,CC.CompanyName FROM (
SELECT CompanyId,
SUM((case when CurrentMonth=1 then TotalAmount else 0 end)) AS Jan,
SUM((case when CurrentMonth=2 then TotalAmount else 0 end)) AS Feb,
SUM((case when CurrentMonth=3 then TotalAmount else 0 end)) AS Mar,
SUM((case when CurrentMonth=4 then TotalAmount else 0 end)) AS Apr,
SUM((case when CurrentMonth=5 then TotalAmount else 0 end)) AS May,
SUM((case when CurrentMonth=6 then TotalAmount else 0 end)) AS Jun,
SUM((case when CurrentMonth=7 then TotalAmount else 0 end)) AS July,
SUM((case when CurrentMonth=8 then TotalAmount else 0 end)) AS Aug,
SUM((case when CurrentMonth=9 then TotalAmount else 0 end)) AS Sep,
SUM((case when CurrentMonth=10 then TotalAmount else 0 end)) AS Oct,
SUM((case when CurrentMonth=11 then TotalAmount else 0 end)) AS Nov,
SUM((case when CurrentMonth=12 then TotalAmount else 0 end)) AS Decmber
 FROM @CompanyPaymentDaysInfo
 GROUP BY CompanyId) C
 INNER JOIN lmshop_CompanyCussent CC on CC.CompanyId=C.CompanyId
 {4}	";
            #endregion
            var filialeIdStr = string.Empty;
            var isOut = 0;
            var companyStr = string.IsNullOrEmpty(companyName)
                ? string.Empty
                : string.Format(" WHERE CC.CompanyName LIKE '%{0}%'", companyName);
            if (filialeId != Guid.Empty)
            {
                filialeIdStr = string.Format(" AND FilialeId='{0}'", filialeId);
                isOut = 1;
            }
            var startTime = Convert.ToDateTime(string.Format("{0}-01-01 00:00:00", year));
            var endTime = Convert.ToDateTime(string.Format("{0}-12-01 00:00:00", year));
            var newSQL = string.Format(SQL, startTime, endTime, isOut, filialeIdStr, companyStr);
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CompanyPaymentDaysInfo>(true, newSQL).ToList();
            }
        }
        #endregion
    }
}

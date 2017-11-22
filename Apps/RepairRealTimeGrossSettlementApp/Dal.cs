using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data.SqlClient;
using RepairRealTimeGrossSettlementApp.Model;

namespace RepairRealTimeGrossSettlementApp
{
    public class Dal
    {
        private static void InitHistoryRealTimeGrossSettlement()
        {
            const string SQL_INIT = @"
/*
 * 重新初始化即时结算价，完成时间小于2017/6/1的历史单据
 * Create Time: 2017/7/4
 * Author: Jerry Bai
 *********************************
 * Update Time: 2017/7/4
 * Update By: Jerry Bai
 * Remark: 填充结算价表的单据数量、单据金额字段
 */
create table #Tmp_StockInOutRecordForHistorySettlementPrice
(
	OccurMonth varchar(6),
	FilialeId uniqueidentifier,
	GoodsId uniqueidentifier,
	PurchaseStockInQuantity int not null,
	PurchaseStockInAmount decimal not null,
	PurchaseReturnStockOutQuantity int not null,
	PurchaseReturnStockOutAmount decimal not null,
	NewStockInQuantity int not null,
	NewStockInAmount decimal not null,
	Primary Key(OccurMonth,FilialeId,GoodsId)
)
go

create table #Tmp_OccurMonth
(
	OccurMonth varchar(6),
	Primary Key(OccurMonth)
)
go

delete from #Tmp_OccurMonth;
delete from #Tmp_StockInOutRecordForHistorySettlementPrice;
delete from RealTimeGrossSettlement where RelatedTradeNo like 'SYS_INIT_%';

if not exists(select top 1 1 from #Tmp_OccurMonth)
begin
	insert into #Tmp_OccurMonth(OccurMonth)
	select convert(varchar(6),t1.YYYY+t2.MM) OccurMonth
	from (
		select 201600 YYYY
		union select 201500 YYYY
	) t1
	join (
		select 1 MM
		union select 2 MM
		union select 3 MM
		union select 4 MM
		union select 5 MM
		union select 6 MM
		union select 7 MM
		union select 8 MM
		union select 9 MM
		union select 10 MM
		union select 11 MM
		union select 12 MM
	) t2 on 1=1
	union select '201701' OccurMonth
	union select '201702' OccurMonth
	union select '201703' OccurMonth
	union select '201704' OccurMonth
	union select '201705' OccurMonth
end

-- 上海可镜采购入库
insert into #Tmp_StockInOutRecordForHistorySettlementPrice(OccurMonth,FilialeId,GoodsId,PurchaseStockInQuantity,PurchaseStockInAmount,PurchaseReturnStockOutQuantity,PurchaseReturnStockOutAmount,NewStockInQuantity,NewStockInAmount)
select	'201705' OccurMonth
		,'58437edc-87b7-4995-a5c0-bb5fd0fe49e0' FilialeId -- 上海可镜
		,t2.GoodsId
		,SUM(t2.Quantity) Quantity
		,SUM(t2.UnitPrice*t2.Quantity) Amount
		,0
		,0
		,0
		,0
from StorageRecord t1 with(nolock)
inner join StorageRecordDetail t2 with(nolock) on t2.StockId=t1.StockId
inner join [156].[Group.GMS20].dbo.Goods t3 with(nolock) on t3.GoodsId=t2.GoodsId and t3.IsDelete=0
where t1.StockType=1 and t1.StockState=4 and t1.AuditTime is not null and '2017-05-31 23:59:59'>=t1.AuditTime and (t3.GoodsType<>5 or t2.UnitPrice<>0)
    and t1.WarehouseId in('84b303f5-2aa6-437d-9d23-3488ad55d278','B5BCDF6E-95D5-4AEE-9B19-6EE218255C05') -- 上海仓
	and t1.TradeCode not in (select LinkTradeCode from DocumentRed with(nolock) where RedType=1 and DocumentType=2 and [State]=5)
group by t2.GoodsId
order by t2.GoodsId

-- 兰溪百秀采购入库
insert into #Tmp_StockInOutRecordForHistorySettlementPrice(OccurMonth,FilialeId,GoodsId,PurchaseStockInQuantity,PurchaseStockInAmount,PurchaseReturnStockOutQuantity,PurchaseReturnStockOutAmount,NewStockInQuantity,NewStockInAmount)
select	'201705' OccurMonth
		,'75621b55-2fa3-4fcf-b68a-039c28f560b6' FilialeId -- 兰溪百秀
		,t2.GoodsId
		,SUM(t2.Quantity) Quantity
		,SUM(t2.UnitPrice*t2.Quantity) Amount
		,0
		,0
		,0
		,0
from StorageRecord t1 with(nolock)
inner join StorageRecordDetail t2 with(nolock) on t2.StockId=t1.StockId
inner join [156].[Group.GMS20].dbo.Goods t3 with(nolock) on t3.GoodsId=t2.GoodsId and t3.IsDelete=0 and (t3.GoodsType<>5 or t2.UnitPrice<>0)
where t1.StockType=1 and t1.StockState=4 and t1.AuditTime is not null and '2017-05-31 23:59:59'>=t1.AuditTime
    and WarehouseId='ef5ae4a5-7f6d-48e8-b9b2-c91a7b6b5e83' -- 兰溪仓
	and t1.TradeCode not in (select LinkTradeCode from DocumentRed with(nolock) where RedType=1 and DocumentType=2 and [State]=5)
group by t2.GoodsId
order by t2.GoodsId


-- 上海可镜采购退货出库
insert into #Tmp_StockInOutRecordForHistorySettlementPrice(OccurMonth,FilialeId,GoodsId,PurchaseStockInQuantity,PurchaseStockInAmount,PurchaseReturnStockOutQuantity,PurchaseReturnStockOutAmount,NewStockInQuantity,NewStockInAmount)
select	'201705' OccurMonth
		,'58437edc-87b7-4995-a5c0-bb5fd0fe49e0' FilialeId -- 上海可镜
		,t2.GoodsId
		,0
		,0
		,SUM(t2.Quantity) Quantity
		,SUM(t2.UnitPrice*t2.Quantity) Amount
		,0
		,0
from StorageRecord t1 with(nolock)
inner join StorageRecordDetail t2 with(nolock) on t2.StockId=t1.StockId
where t1.StockType=5 and t1.StockState=4 and t1.AuditTime is not null and '2017-05-31 23:59:59'>=t1.AuditTime
    and t1.WarehouseId in('84b303f5-2aa6-437d-9d23-3488ad55d278','B5BCDF6E-95D5-4AEE-9B19-6EE218255C05') -- 上海仓
	and not exists(
			select top 1 1
			from #Tmp_StockInOutRecordForHistorySettlementPrice
			where OccurMonth='201705'
				and FilialeId='58437edc-87b7-4995-a5c0-bb5fd0fe49e0' and GoodsId=t2.GoodsId
		)
group by t2.GoodsId

update #Tmp_StockInOutRecordForHistorySettlementPrice
set PurchaseReturnStockOutQuantity=B.Quantity,PurchaseReturnStockOutAmount=B.Amount
from #Tmp_StockInOutRecordForHistorySettlementPrice A
inner join (
	select	'201705' OccurMonth
		    ,'58437edc-87b7-4995-a5c0-bb5fd0fe49e0' FilialeId -- 上海可镜
			,t2.GoodsId
			,SUM(t2.Quantity) Quantity
			,SUM(t2.UnitPrice*t2.Quantity) Amount
	from StorageRecord t1 with(nolock)
	inner join StorageRecordDetail t2 with(nolock) on t2.StockId=t1.StockId
	where t1.StockType=5 and t1.StockState=4 and t1.AuditTime is not null and '2017-05-31 23:59:59'>=t1.AuditTime
        and t1.WarehouseId in('84b303f5-2aa6-437d-9d23-3488ad55d278','B5BCDF6E-95D5-4AEE-9B19-6EE218255C05') -- 上海仓
	group by t2.GoodsId
) B on B.OccurMonth=A.OccurMonth and B.FilialeId=A.FilialeId and B.GoodsId=A.GoodsId

-- 兰溪百秀采购退货出库
insert into #Tmp_StockInOutRecordForHistorySettlementPrice(OccurMonth,FilialeId,GoodsId,PurchaseStockInQuantity,PurchaseStockInAmount,PurchaseReturnStockOutQuantity,PurchaseReturnStockOutAmount,NewStockInQuantity,NewStockInAmount)
select	'201705' OccurMonth
		,'75621b55-2fa3-4fcf-b68a-039c28f560b6' FilialeId -- 兰溪百秀
		,t2.GoodsId
		,0
		,0
		,SUM(t2.Quantity) Quantity
		,SUM(t2.UnitPrice*t2.Quantity) Amount
		,0
		,0
from StorageRecord t1 with(nolock)
inner join StorageRecordDetail t2 with(nolock) on t2.StockId=t1.StockId
where t1.StockType=5 and t1.StockState=4 and t1.AuditTime is not null and '2017-05-31 23:59:59'>=t1.AuditTime
    and WarehouseId='ef5ae4a5-7f6d-48e8-b9b2-c91a7b6b5e83' -- 兰溪仓
	and not exists(
			select top 1 1
			from #Tmp_StockInOutRecordForHistorySettlementPrice
			where OccurMonth='201705'
				and FilialeId='75621b55-2fa3-4fcf-b68a-039c28f560b6' and GoodsId=t2.GoodsId
		)
group by t2.GoodsId

update #Tmp_StockInOutRecordForHistorySettlementPrice
set PurchaseReturnStockOutQuantity=B.Quantity,PurchaseReturnStockOutAmount=B.Amount
from #Tmp_StockInOutRecordForHistorySettlementPrice A
inner join (
	select	'201705' OccurMonth
		    ,'75621b55-2fa3-4fcf-b68a-039c28f560b6' FilialeId -- 兰溪百秀
			,t2.GoodsId
			,SUM(t2.Quantity) Quantity
			,SUM(t2.UnitPrice*t2.Quantity) Amount
	from StorageRecord t1 with(nolock)
	inner join StorageRecordDetail t2 with(nolock) on t2.StockId=t1.StockId
	where t1.StockType=5 and t1.StockState=4 and t1.AuditTime is not null and '2017-05-31 23:59:59'>=t1.AuditTime
        and WarehouseId='ef5ae4a5-7f6d-48e8-b9b2-c91a7b6b5e83' -- 兰溪仓
	group by t2.GoodsId
) B on B.OccurMonth=A.OccurMonth and B.FilialeId=A.FilialeId and B.GoodsId=A.GoodsId


-- 上海可镜入库红冲
insert into #Tmp_StockInOutRecordForHistorySettlementPrice(OccurMonth,FilialeId,GoodsId,PurchaseStockInQuantity,PurchaseStockInAmount,PurchaseReturnStockOutQuantity,PurchaseReturnStockOutAmount,NewStockInQuantity,NewStockInAmount)
select	'201705' OccurMonth
		,'58437edc-87b7-4995-a5c0-bb5fd0fe49e0' FilialeId -- 上海可镜
		,t2.GoodsId
		,0
		,0
		,0
		,0
		,SUM(t2.Quantity) Quantity
		,SUM(t2.UnitPrice*t2.Quantity) Amount
	from DocumentRed t1 with(nolock)
	inner join DocumentRedDetail t2 with(nolock) on t2.RedId=t1.RedId
	inner join StorageRecord t3 with(nolock) on t3.TradeCode=t1.LinkTradeCode
	where t1.RedType=1 and t1.DocumentType=0 and t1.[State]=4 and t3.StockType=1 and t1.AuditTime is not null and '2017-05-31 23:59:59'>=t1.AuditTime
        and t1.WarehouseId in('84b303f5-2aa6-437d-9d23-3488ad55d278','B5BCDF6E-95D5-4AEE-9B19-6EE218255C05') -- 上海仓
		and not exists(
				select top 1 1
				from #Tmp_StockInOutRecordForHistorySettlementPrice
				where OccurMonth='201705'
					and FilialeId='58437edc-87b7-4995-a5c0-bb5fd0fe49e0' and GoodsId=t2.GoodsId
			)
group by t2.GoodsId

update #Tmp_StockInOutRecordForHistorySettlementPrice
set NewStockInQuantity=B.Quantity,NewStockInAmount=B.Amount
from #Tmp_StockInOutRecordForHistorySettlementPrice A
inner join (
	select	'201705' OccurMonth
		    ,'58437edc-87b7-4995-a5c0-bb5fd0fe49e0' FilialeId -- 上海可镜
			,t2.GoodsId
			,SUM(t2.Quantity) Quantity
			,SUM(t2.UnitPrice*t2.Quantity) Amount
	from DocumentRed t1 with(nolock)
	inner join DocumentRedDetail t2 with(nolock) on t2.RedId=t1.RedId
	inner join StorageRecord t3 with(nolock) on t3.TradeCode=t1.LinkTradeCode
	where t1.RedType=1 and t1.DocumentType=0 and t1.[State]=4 and t3.StockType=1 and t1.AuditTime is not null and '2017-05-31 23:59:59'>=t1.AuditTime
        and t1.WarehouseId in('84b303f5-2aa6-437d-9d23-3488ad55d278','B5BCDF6E-95D5-4AEE-9B19-6EE218255C05') -- 上海仓
	group by t2.GoodsId
) B on B.OccurMonth=A.OccurMonth and B.FilialeId=A.FilialeId and B.GoodsId=A.GoodsId

-- 兰溪百秀入库红冲
insert into #Tmp_StockInOutRecordForHistorySettlementPrice(OccurMonth,FilialeId,GoodsId,PurchaseStockInQuantity,PurchaseStockInAmount,PurchaseReturnStockOutQuantity,PurchaseReturnStockOutAmount,NewStockInQuantity,NewStockInAmount)
select	'201705' OccurMonth
		,'75621b55-2fa3-4fcf-b68a-039c28f560b6' FilialeId -- 兰溪百秀
		,t2.GoodsId
		,0
		,0
		,0
		,0
		,SUM(t2.Quantity) Quantity
		,SUM(t2.UnitPrice*t2.Quantity) Amount
	from DocumentRed t1 with(nolock)
	inner join DocumentRedDetail t2 with(nolock) on t2.RedId=t1.RedId
	inner join StorageRecord t3 with(nolock) on t3.TradeCode=t1.LinkTradeCode
	where t1.RedType=1 and t1.DocumentType=0 and t1.[State]=4 and t3.StockType=1 and t1.AuditTime is not null and '2017-05-31 23:59:59'>=t1.AuditTime
        and t1.WarehouseId='ef5ae4a5-7f6d-48e8-b9b2-c91a7b6b5e83' -- 兰溪仓
		and not exists(
				select top 1 1
				from #Tmp_StockInOutRecordForHistorySettlementPrice
				where OccurMonth='201705'
					and FilialeId='75621b55-2fa3-4fcf-b68a-039c28f560b6' and GoodsId=t2.GoodsId
			)
group by t2.GoodsId

update #Tmp_StockInOutRecordForHistorySettlementPrice
set NewStockInQuantity=B.Quantity,NewStockInAmount=B.Amount
from #Tmp_StockInOutRecordForHistorySettlementPrice A
inner join (
	select	'201705' OccurMonth
		    ,'75621b55-2fa3-4fcf-b68a-039c28f560b6' FilialeId -- 兰溪百秀
			,t2.GoodsId
			,SUM(t2.Quantity) Quantity
			,SUM(t2.UnitPrice*t2.Quantity) Amount
	from DocumentRed t1 with(nolock)
	inner join DocumentRedDetail t2 with(nolock) on t2.RedId=t1.RedId
	inner join StorageRecord t3 with(nolock) on t3.TradeCode=t1.LinkTradeCode
	where t1.RedType=1 and t1.DocumentType=0 and t1.[State]=4 and t3.StockType=1 and t1.AuditTime is not null and '2017-05-31 23:59:59'>=t1.AuditTime
        and t1.WarehouseId='ef5ae4a5-7f6d-48e8-b9b2-c91a7b6b5e83' -- 兰溪仓
	group by t2.GoodsId
) B on B.OccurMonth=A.OccurMonth and B.FilialeId=A.FilialeId and B.GoodsId=A.GoodsId





-- 保存各月的即时结算价
insert into RealTimeGrossSettlement(FilialeId,GoodsId,UnitPrice,StockQuantity,GoodsQuantityInBill,GoodsAmountInBill,RelatedTradeType,RelatedTradeNo,OccurTime,CreateTime)
select	t1.FilialeId
		,t1.GoodsId
		,(case
			when t1.PurchaseStockInAmount+t1.PurchaseReturnStockOutAmount+t1.NewStockInAmount>0 and t1.PurchaseStockInQuantity+t1.PurchaseReturnStockOutQuantity+t1.NewStockInQuantity>0 then
				convert(decimal(18,4),ABS((t1.PurchaseStockInAmount+t1.PurchaseReturnStockOutAmount+t1.NewStockInAmount)/(t1.PurchaseStockInQuantity+t1.PurchaseReturnStockOutQuantity+t1.NewStockInQuantity)))
			else
				convert(decimal(18,4),(t1.PurchaseStockInAmount+t1.NewStockInAmount)/(t1.PurchaseStockInQuantity+t1.NewStockInQuantity))
			end) UnitPrice
		,0 StockQuantity
		,(case
			when t1.PurchaseStockInAmount+t1.PurchaseReturnStockOutAmount+t1.NewStockInAmount>0 and t1.PurchaseStockInQuantity+t1.PurchaseReturnStockOutQuantity+t1.NewStockInQuantity>0 then
				ABS(t1.PurchaseStockInQuantity+t1.PurchaseReturnStockOutQuantity+t1.NewStockInQuantity)
			else
				t1.PurchaseStockInQuantity+t1.NewStockInQuantity
			end) GoodsQuantityInBill
		,(case
			when t1.PurchaseStockInAmount+t1.PurchaseReturnStockOutAmount+t1.NewStockInAmount>0 and t1.PurchaseStockInQuantity+t1.PurchaseReturnStockOutQuantity+t1.NewStockInQuantity>0 then
				ABS(t1.PurchaseStockInAmount+t1.PurchaseReturnStockOutAmount+t1.NewStockInAmount)
			else
				t1.PurchaseStockInAmount+t1.NewStockInAmount
			end) GoodsAmountInBill
		,0 RelatedTradeType
		,('SYS_INIT_'+t1.OccurMonth) RelatedTradeNo
		,(case
			when t1.OccurMonth=convert(varchar(6),getdate(),112) then GETDATE()
			else DATEADD(SECOND,-1,DATEADD(MONTH,1,CONVERT(datetime,t1.OccurMonth+'01',112))) end
		) OccurTime
		,GETDATE() CreateTime
from #Tmp_StockInOutRecordForHistorySettlementPrice t1
where t1.PurchaseStockInQuantity>0
	and (t1.PurchaseStockInQuantity+t1.PurchaseReturnStockOutQuantity+t1.NewStockInQuantity)<>0
	and (t1.PurchaseStockInAmount+t1.PurchaseReturnStockOutAmount+t1.NewStockInAmount)<>0
	and not exists(select top 1 1 from RealTimeGrossSettlement with(nolock) where RelatedTradeNo like 'SYS_INIT_%')


-- 取采购设置里的价格
insert into RealTimeGrossSettlement(FilialeId,GoodsId,UnitPrice,StockQuantity,GoodsQuantityInBill,GoodsAmountInBill,RelatedTradeType,RelatedTradeNo,OccurTime,CreateTime)
SELECT	t1.[HostingFilialeId] FilialeId
		,t1.GoodsId
		,t1.PurchasePrice UnitPrice
		,0 StockQuantity
		,0 GoodsQuantityInBill
		,0 GoodsAmountInBill
		,0 RelatedTradeType
		,'SYS_INIT_FromPurchase' RelatedTradeNo
		,IsNull(t1.LastPurchasingDate,convert(date,getdate(),120)) OccurTime
		,GETDATE() CreateTime
FROM [lmshop_PurchaseSet] t1 With(NOLOCK)
where t1.IsDelete=1 and t1.HostingFilialeId<>'00000000-0000-0000-0000-000000000000' and t1.GoodsId<>'00000000-0000-0000-0000-000000000000'
	and not exists (select top 1 1 from RealTimeGrossSettlement where HostingFilialeId=t1.HostingFilialeId and t1.GoodsId=GoodsId and RelatedTradeNo like 'SYS_INIT_%')
order by t1.GoodsId


drop table #Tmp_OccurMonth
go
drop table #Tmp_StockInOutRecordForHistorySettlementPrice
go
";

            const string SQL_REPAIR_201706 = @"
/*
 * 补6月份遗漏的采购入库、采购退货出库对应的结算价记录
 * Create Time: 2017/7/5
 * Author: Jerry Bai
 */
select * into #RealTimeGrossSettlement from RealTimeGrossSettlement where 1=0

-- 6月份 上海可镜商贸 采购入库、采购退货出库缺失记录
insert into #RealTimeGrossSettlement(FilialeId,GoodsId,UnitPrice,StockQuantity,GoodsQuantityInBill,GoodsAmountInBill,RelatedTradeType,RelatedTradeNo,OccurTime,CreateTime)
select	A.FilialeId
		,A.GoodsId
		,(case
			when IsNull(C.StockQuantity,0)>0 and A.StockType=1
				then convert(decimal(18,4),(A.GoodsAmountInBill+C.GoodsAmountInBill)/(C.StockQuantity+A.GoodsQuantityInBill))
			when IsNull(C.StockQuantity,0)>0 and A.StockType=5
				then convert(decimal(18,4),(A.GoodsAmountInBill+C.GoodsAmountInBill)/(C.StockQuantity-A.GoodsQuantityInBill))
			when IsNull(D.GoodsQuantityInBill,0)>0 and A.StockType=1
				then convert(decimal(18,4),(A.GoodsAmountInBill+D.GoodsAmountInBill)/(D.GoodsQuantityInBill+A.GoodsQuantityInBill))
			when IsNull(D.GoodsQuantityInBill,0)>0 and A.StockType=5
				then convert(decimal(18,4),(A.GoodsAmountInBill+D.GoodsAmountInBill)/(D.GoodsQuantityInBill-A.GoodsQuantityInBill))
			else 0 end) UnitPrice
		,(case
			when IsNull(C.StockQuantity,0)>0 and A.StockType=1
				then C.StockQuantity+A.GoodsQuantityInBill
			when IsNull(C.StockQuantity,0)>0 and A.StockType=5
				then C.StockQuantity-A.GoodsQuantityInBill
			else 0 end) StockQuantity
		,A.GoodsQuantityInBill GoodsQuantityInBill
		,A.GoodsAmountInBill GoodsAmountInBill
		,(case A.StockType when 1 then 1 else 2 end) RelatedTradeType
		,A.TradeCode RelatedTradeNo
		,A.AuditTime OccurTime
		,GETDATE() CreateTime
from (
	select	t1.StockType
			,t1.FilialeId
			,t1.TradeCode
			,t1.AuditTime
			,t1.TradeBothPartiesType
			,t2.GoodsId
			,SUM(t2.Quantity) GoodsQuantityInBill
			,sum(t2.UnitPrice*t2.Quantity) GoodsAmountInBill
	from StorageRecord t1
	inner join StorageRecordDetail t2 on t2.StockId=t1.StockId
	where t1.StockType in (1,5) and t1.StockState=4 and AuditTime>='2017/6/1' and AuditTime<'2017/7/1'
		and t1.FilialeId='58437edc-87b7-4995-a5c0-bb5fd0fe49e0'
	group by t1.StockType,t1.FilialeId,t1.TradeCode,t1.AuditTime,t1.TradeBothPartiesType,t2.GoodsId
) A
left join (
	select *
	from RealTimeGrossSettlement
	where OccurTime<'2017/7/1' and RelatedTradeNo not like 'SYS_INIT_%'
		and FilialeId='58437edc-87b7-4995-a5c0-bb5fd0fe49e0'
) B on B.RelatedTradeNo=A.TradeCode and B.FilialeId=A.FilialeId and B.GoodsId=A.GoodsId
left join (
	select *, ROW_NUMBER() over (PARTITION by FilialeId,GoodsId order by FilialeId,GoodsId,OccurTime) g1
	from RealTimeGrossSettlement
	where OccurTime<'2017/7/1' and RelatedTradeNo not like 'SYS_INIT_%'
		and FilialeId='58437edc-87b7-4995-a5c0-bb5fd0fe49e0'
) C on C.FilialeId=A.FilialeId and C.GoodsId=A.GoodsId and C.OccurTime<A.AuditTime and C.g1=1
left join (
	select *
	from RealTimeGrossSettlement
	where RelatedTradeNo like 'SYS_INIT_%'
		and FilialeId='58437edc-87b7-4995-a5c0-bb5fd0fe49e0'
) D on D.FilialeId=A.FilialeId and D.GoodsId=A.GoodsId
where B.UnitPrice is null


-- 6月份 河北可镜网络 采购入库、采购退货出库缺失记录
insert into #RealTimeGrossSettlement(FilialeId,GoodsId,UnitPrice,StockQuantity,GoodsQuantityInBill,GoodsAmountInBill,RelatedTradeType,RelatedTradeNo,OccurTime,CreateTime)
select	A.FilialeId
		,A.GoodsId
		,(case
			when IsNull(C.StockQuantity,0)>0 and A.StockType=1
				then convert(decimal(18,4),(A.GoodsAmountInBill+C.GoodsAmountInBill)/(C.StockQuantity+A.GoodsQuantityInBill))
			when IsNull(C.StockQuantity,0)>0 and A.StockType=5
				then convert(decimal(18,4),(A.GoodsAmountInBill+C.GoodsAmountInBill)/(C.StockQuantity-A.GoodsQuantityInBill))
			when IsNull(D.GoodsQuantityInBill,0)>0 and A.StockType=1
				then convert(decimal(18,4),(A.GoodsAmountInBill+D.GoodsAmountInBill)/(D.GoodsQuantityInBill+A.GoodsQuantityInBill))
			when IsNull(D.GoodsQuantityInBill,0)>0 and A.StockType=5
				then convert(decimal(18,4),(A.GoodsAmountInBill+D.GoodsAmountInBill)/(D.GoodsQuantityInBill-A.GoodsQuantityInBill))
			else 0 end) UnitPrice
		,(case
			when IsNull(C.StockQuantity,0)>0 and A.StockType=1
				then C.StockQuantity+A.GoodsQuantityInBill
			when IsNull(C.StockQuantity,0)>0 and A.StockType=5
				then C.StockQuantity-A.GoodsQuantityInBill
			else 0 end) StockQuantity
		,A.GoodsQuantityInBill GoodsQuantityInBill
		,A.GoodsAmountInBill GoodsAmountInBill
		,(case A.StockType when 1 then 1 else 2 end) RelatedTradeType
		,A.TradeCode RelatedTradeNo
		,A.AuditTime OccurTime
		,GETDATE() CreateTime
from (
	select	t1.StockType
			,t1.FilialeId
			,t1.TradeCode
			,t1.AuditTime
			,t1.TradeBothPartiesType
			,t2.GoodsId
			,SUM(t2.Quantity) GoodsQuantityInBill
			,sum(t2.UnitPrice*t2.Quantity) GoodsAmountInBill
	from StorageRecord t1
	inner join StorageRecordDetail t2 on t2.StockId=t1.StockId
	where t1.StockType in (1,5) and t1.StockState=4 and AuditTime>='2017/6/1' and AuditTime<'2017/7/1'
		and t1.FilialeId='1A856F24-4FC9-49CA-8789-E15542F56D35'
	group by t1.StockType,t1.FilialeId,t1.TradeCode,t1.AuditTime,t1.TradeBothPartiesType,t2.GoodsId
) A
left join (
	select *
	from RealTimeGrossSettlement
	where OccurTime<'2017/7/1' and RelatedTradeNo not like 'SYS_INIT_%'
		and FilialeId='1A856F24-4FC9-49CA-8789-E15542F56D35'
) B on B.RelatedTradeNo=A.TradeCode and B.FilialeId=A.FilialeId and B.GoodsId=A.GoodsId
left join (
	select *, ROW_NUMBER() over (PARTITION by FilialeId,GoodsId order by FilialeId,GoodsId,OccurTime) g1
	from RealTimeGrossSettlement
	where OccurTime<'2017/7/1' and RelatedTradeNo not like 'SYS_INIT_%'
		and FilialeId='1A856F24-4FC9-49CA-8789-E15542F56D35'
) C on C.FilialeId=A.FilialeId and C.GoodsId=A.GoodsId and C.OccurTime<A.AuditTime and C.g1=1
left join (
	select *
	from RealTimeGrossSettlement
	where RelatedTradeNo like 'SYS_INIT_%'
		and FilialeId='1A856F24-4FC9-49CA-8789-E15542F56D35'
) D on D.FilialeId=A.FilialeId and D.GoodsId=A.GoodsId
where B.UnitPrice is null


-- 6月份 百秀浙江 采购入库、采购退货出库缺失记录
insert into #RealTimeGrossSettlement(FilialeId,GoodsId,UnitPrice,StockQuantity,GoodsQuantityInBill,GoodsAmountInBill,RelatedTradeType,RelatedTradeNo,OccurTime,CreateTime)
select	A.FilialeId
		,A.GoodsId
		,(case
			when IsNull(C.StockQuantity,0)>0 and A.StockType=1
				then convert(decimal(18,4),(A.GoodsAmountInBill+C.GoodsAmountInBill)/(C.StockQuantity+A.GoodsQuantityInBill))
			when IsNull(C.StockQuantity,0)>0 and A.StockType=5
				then convert(decimal(18,4),(A.GoodsAmountInBill+C.GoodsAmountInBill)/(C.StockQuantity-A.GoodsQuantityInBill))
			when IsNull(D.GoodsQuantityInBill,0)>0 and A.StockType=1
				then convert(decimal(18,4),(A.GoodsAmountInBill+D.GoodsAmountInBill)/(D.GoodsQuantityInBill+A.GoodsQuantityInBill))
			when IsNull(D.GoodsQuantityInBill,0)>0 and A.StockType=5
				then convert(decimal(18,4),(A.GoodsAmountInBill+D.GoodsAmountInBill)/(D.GoodsQuantityInBill-A.GoodsQuantityInBill))
			else 0 end) UnitPrice
		,(case
			when IsNull(C.StockQuantity,0)>0 and A.StockType=1
				then C.StockQuantity+A.GoodsQuantityInBill
			when IsNull(C.StockQuantity,0)>0 and A.StockType=5
				then C.StockQuantity-A.GoodsQuantityInBill
			else 0 end) StockQuantity
		,A.GoodsQuantityInBill GoodsQuantityInBill
		,A.GoodsAmountInBill GoodsAmountInBill
		,(case A.StockType when 1 then 1 else 2 end) RelatedTradeType
		,A.TradeCode RelatedTradeNo
		,A.AuditTime OccurTime
		,GETDATE() CreateTime
from (
	select	t1.StockType
			,t1.FilialeId
			,t1.TradeCode
			,t1.AuditTime
			,t1.TradeBothPartiesType
			,t2.GoodsId
			,SUM(t2.Quantity) GoodsQuantityInBill
			,sum(t2.UnitPrice*t2.Quantity) GoodsAmountInBill
	from StorageRecord t1
	inner join StorageRecordDetail t2 on t2.StockId=t1.StockId
	where t1.StockType in (1,5) and t1.StockState=4 and AuditTime>='2017/6/1' and AuditTime<'2017/7/1'
		and t1.FilialeId='75621B55-2FA3-4FCF-B68A-039C28F560B6'
	group by t1.StockType,t1.FilialeId,t1.TradeCode,t1.AuditTime,t1.TradeBothPartiesType,t2.GoodsId
) A
left join (
	select *
	from RealTimeGrossSettlement
	where OccurTime<'2017/7/1' and RelatedTradeNo not like 'SYS_INIT_%'
		and FilialeId='75621B55-2FA3-4FCF-B68A-039C28F560B6'
) B on B.RelatedTradeNo=A.TradeCode and B.FilialeId=A.FilialeId and B.GoodsId=A.GoodsId
left join (
	select *, ROW_NUMBER() over (PARTITION by FilialeId,GoodsId order by FilialeId,GoodsId,OccurTime) g1
	from RealTimeGrossSettlement
	where OccurTime<'2017/7/1' and RelatedTradeNo not like 'SYS_INIT_%'
		and FilialeId='75621B55-2FA3-4FCF-B68A-039C28F560B6'
) C on C.FilialeId=A.FilialeId and C.GoodsId=A.GoodsId and C.OccurTime<A.AuditTime and C.g1=1
left join (
	select *
	from RealTimeGrossSettlement
	where RelatedTradeNo like 'SYS_INIT_%'
		and FilialeId='75621B55-2FA3-4FCF-B68A-039C28F560B6'
) D on D.FilialeId=A.FilialeId and D.GoodsId=A.GoodsId
where B.UnitPrice is null


insert into RealTimeGrossSettlement(FilialeId,GoodsId,UnitPrice,StockQuantity,GoodsQuantityInBill,GoodsAmountInBill,RelatedTradeType,RelatedTradeNo,OccurTime,CreateTime)
select * from #RealTimeGrossSettlement


drop table #RealTimeGrossSettlement
";

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                conn.Execute(SQL_INIT);
            }

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                conn.Execute(SQL_REPAIR_201706);
            }
        }

        public static List<string> GetBillNoList(Guid filialeId, DateTime startTime)
        {
            const string SQL = @"
select distinct RelatedTradeNo,OccurTime
from RealTimeGrossSettlement
where RelatedTradeType<>0
	and FilialeId=@FilialeId
    and OccurTime>=@OccurTime
order by OccurTime";

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                return conn.Query(SQL, new { FilialeId = filialeId, OccurTime = startTime }).Select(m => (string)m.RelatedTradeNo).ToList();
            }
        }

        public static List<RealTimeGrossSettlementInfo> GetListByBillNo(Guid filialeId, string billNo)
        {
            const string SQL = @"
select *
from RealTimeGrossSettlement
where RelatedTradeType<>0 and FilialeId=@FilialeId and RelatedTradeNo=@RelatedTradeNo
order by OccurTime";

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                return conn.Query<RealTimeGrossSettlementInfo>(SQL, new { FilialeId = filialeId, RelatedTradeNo = billNo }).ToList();
            }
        }

        public static decimal GetLatestUnitPriceBeforeSpecificTime(Guid filialeId, Guid goodsId, DateTime specificTime, bool isHostingFiliale)
        {
            const string SQL = @"
select  top 1 UnitPrice
from RealTimeGrossSettlement
where FilialeId=@FilialeId and GoodsId=@GoodsId and OccurTime<@SpecificTime
order by OccurTime desc
";
            const string SQL_Init_HostingFiliale = @"
select  top 1 UnitPrice
from RealTimeGrossSettlement
where FilialeId=@FilialeId and GoodsId=@GoodsId and RelatedTradeType=0
order by OccurTime desc
";

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                var result = conn.QueryFirstOrDefault<decimal>(SQL, new { FilialeId = filialeId, GoodsId = goodsId, SpecificTime = specificTime });
                if (result == 0)
                {
                    result = conn.QueryFirstOrDefault<decimal>(SQL_Init_HostingFiliale, new { FilialeId = filialeId, GoodsId = goodsId });
                }
                return result;
            }
        }

        public static RealTimeGrossSettlementInfo GetLatestBeforeSpecificTime(Guid filialeId, Guid goodsId, DateTime specificTime)
        {
            const string SQL = @"
select  top 1 *
from RealTimeGrossSettlement
where FilialeId=@FilialeId and GoodsId=@GoodsId and OccurTime<@SpecificTime
order by OccurTime desc
";
            const string SQL_Init_HostingFiliale = @"
select  top 1 *
from RealTimeGrossSettlement
where FilialeId=@FilialeId and GoodsId=@GoodsId and RelatedTradeType=0
order by OccurTime desc
";

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                var result = conn.QueryFirstOrDefault<RealTimeGrossSettlementInfo>(SQL, new { FilialeId = filialeId, GoodsId = goodsId, SpecificTime = specificTime });
                if (result == null)
                {
                    result = conn.QueryFirstOrDefault<RealTimeGrossSettlementInfo>(SQL_Init_HostingFiliale, new { FilialeId = filialeId, GoodsId = goodsId });
                }
                return result;
            }
        }

        public static IDictionary<Guid, decimal> GetLatestUnitPriceListBeforeSpecificTimeByMultiGoods(Guid filialeId, IEnumerable<Guid> goodsIds, DateTime specificTime)
        {
            Dictionary<Guid, decimal> result = new Dictionary<Guid, decimal>();
            if (goodsIds == null || !goodsIds.Any())
            {
                return result;
            }
            const string SQL = @"
select	GoodsId
		,UnitPrice
from (
	select	ROW_NUMBER() over (partition by GoodsId order by OccurTime desc) g
			,GoodsId
			,UnitPrice
	from RealTimeGrossSettlement
    where FilialeId=@FilialeId and GoodsId in ('{0}') and OccurTime<@SpecificTime
) t1
where t1.g=1";

            const string SQL_Init = @"
select	GoodsId
		,UnitPrice
from (
	select	ROW_NUMBER() over (partition by GoodsId order by OccurTime desc) g
			,GoodsId
			,UnitPrice
	from RealTimeGrossSettlement
    where FilialeId=@FilialeId and GoodsId in ('{0}') and RelatedTradeType=0
) t1
where t1.g=1";

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                result = conn.Query(string.Format(SQL, string.Join("','", goodsIds.Select(m => m.ToString()))), new { FilialeId = filialeId, SpecificTime = specificTime }).ToDictionary(kv => (Guid)kv.GoodsId, kv => (decimal)kv.UnitPrice);
                if (result.Count != goodsIds.Count())
                {
                    var otherGoodsIds = goodsIds.Except(result.Keys);
                    var otherSettleList = conn.Query(string.Format(SQL_Init, string.Join("','", otherGoodsIds.Select(m => m.ToString()))), new { FilialeId = filialeId }).Select(m => new KeyValuePair<Guid, decimal>((Guid)m.GoodsId, (decimal)m.UnitPrice)).ToList();
                    foreach(var otherSettle in otherSettleList)
                    {
                        result.Add(otherSettle.Key, otherSettle.Value);
                    }
                }
                return result;
            }
        }

        public static Guid GetHostingFilialeIdByStorageRecordTradeCode(string tradeCode)
        {
            const string SQL = @"
select t2.RelevanceFilialeId
from StorageRecord t1 with(nolock)
inner join lmShop_CompanyCussent t2 with(nolock) on t2.CompanyID=t1.ThirdCompanyID
where t1.TradeCode=@TradeCode";

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                return conn.QueryFirstOrDefault<Guid>(SQL, new { TradeCode = tradeCode });
            }
        }

        public static void Save(RealTimeGrossSettlementInfo info)
        {
            const string SQL = @"
update RealTimeGrossSettlement
set UnitPrice=@UnitPrice,GoodsAmountInBill=@GoodsAmountInBill,ExtField_1=@ExtField_1
where RelatedTradeType<>0 and FilialeId=@FilialeId and GoodsId=@GoodsId and RelatedTradeNo=@RelatedTradeNo";

            if (info != null)
            {
                using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
                {
                    conn.Execute(SQL, new { FilialeId = info.FilialeId, GoodsId = info.GoodsId, RelatedTradeNo = info.RelatedTradeNo, UnitPrice = info.UnitPrice, GoodsAmountInBill = info.GoodsAmountInBill, ExtField_1 = info.ExtField_1 });
                }
            }
        }

        public static void Delete(RealTimeGrossSettlementInfo info)
        {
            const string SQL = @"
select *
from RealTimeGrossSettlement
where RelatedTradeType<>0 and FilialeId=@FilialeId and GoodsId=@GoodsId and RelatedTradeNo=@RelatedTradeNo";

            const string DEL_SQL = @"
delete from RealTimeGrossSettlement
where RelatedTradeType<>0 and FilialeId=@FilialeId and GoodsId=@GoodsId and RelatedTradeNo=@RelatedTradeNo";

            if (info != null)
            {
                using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
                {
                    var list = conn.Query<RealTimeGrossSettlementInfo>(SQL, new { FilialeId = info.FilialeId, GoodsId = info.GoodsId, RelatedTradeNo = info.RelatedTradeNo });
                    if (list.Count() == 1)
                    {
                        conn.Execute(DEL_SQL, new { FilialeId = info.FilialeId, GoodsId = info.GoodsId, RelatedTradeNo = info.RelatedTradeNo });
                    }
                }
                //
            }
        }

        public static GoodsSplitCombineInfo GetCombineOrSplitGoods(string billNo)
        {
            const string SQL_1 = @"
select GoodsSplitCombineGoodsId GoodsId,GoodsSplitCombineQuantity Quantity
from {0}.dbo.GoodsSplitCombine with(nolock)
where No=@BillNo

select GoodsId,Quantity
from {0}.dbo.GoodsSplitCombineDetail
where GoodsSplitCombineNo=@BillNo";

            const string SQL_2 = @"
select GoodsId,SUM(Quantity) Quantity
from {0}.dbo.GoodsSplitCombineDetail with(nolock)
where GoodsSplitCombineNo=@BillNo
group by GoodsId";

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                var bill = conn.QueryFirstOrDefault<GoodsSplitCombineInfo>(string.Format(SQL_1, GlobalConfig.WmsDbName), new { BillNo = billNo });
                if (bill != null)
                {
                    bill.Details = conn.Query(string.Format(SQL_2, GlobalConfig.WmsDbName), new { BillNo = billNo }).Select(m => new Tuple<Guid, int>((Guid)m.GoodsId, (int)m.Quantity)).ToList();
                }
                return bill;
            }
        }

        /// <summary>
        /// 根据单据红冲记录编号获取单据红冲记录
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public static Tuple<DocumentRedInfo, IList<DocumentRedDetailInfo>> GetDocumentRed(string tradeCode, Guid goodsId)
        {
            const string SQL = @"
SELECT *
FROM DocumentRed WITH(NOLOCK)
WHERE TradeCode=@TradeCode";

            const string SQL_Detail = @"
SELECT *
FROM DocumentRedDetail WITH(NOLOCK) 
WHERE RedId=@RedId AND (@GoodsId='00000000-0000-0000-0000-000000000000' or GoodsId=@GoodsId)";

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                var bill = conn.QueryFirstOrDefault<DocumentRedInfo>(SQL, new { TradeCode = tradeCode });
                IList<DocumentRedDetailInfo> billDetails = null;
                if (bill != null)
                {
                    billDetails = conn.Query<DocumentRedDetailInfo>(SQL_Detail, new { RedId = bill.RedId, GoodsId = goodsId }).AsList();
                }
                return new Tuple<DocumentRedInfo, IList<DocumentRedDetailInfo>>(bill, billDetails);
            }
        }

        /// <summary>
        /// 根据出入库ID，获取出入库记录
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public static Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>> GetStorageRecord(Guid stockId, Guid goodsId)
        {
            const string SQL = @"
SELECT *
FROM StorageRecord WITH(NOLOCK)
WHERE StockId=@StockId";

            const string SQL_Detail = @"
SELECT *
FROM StorageRecordDetail WITH(NOLOCK) 
WHERE StockId=@StockId AND (@GoodsId='00000000-0000-0000-0000-000000000000' or GoodsId=@GoodsId)";

            using (var conn = new SqlConnection(GlobalConfig.ERPConnnectionString))
            {
                var bill = conn.QueryFirstOrDefault<StorageRecordInfo>(SQL, new { StockId = stockId });
                IList<StorageRecordDetailInfo> billDetails = null;
                if (bill != null)
                {
                    billDetails = conn.Query<StorageRecordDetailInfo>(SQL_Detail, new { StockId = stockId, GoodsId = goodsId }).AsList();
                }
                return new Tuple<StorageRecordInfo, IList<StorageRecordDetailInfo>>(bill, billDetails);
            }
        }
    }
}

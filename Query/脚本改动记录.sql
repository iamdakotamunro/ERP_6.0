1、商品日销量表里增加主商品自动，原GoodsId变成RealGoodsId
EXEC sp_rename 'lmshop_GoodsDaySalesStatistics.GoodsId','RealGoodsId','COLUMN'

ALTER TABLE lmshop_GoodsDaySalesStatistics ADD GoodsId uniqueidentifier

update lmshop_GoodsDaySalesStatistics
set GoodsId=rg.GoodsId
from lmShop_RealGoods rg where lmshop_GoodsDaySalesStatistics.RealGoodsId=rg.RealGoodsId


--采购单添加采购单提交人字段
alter table lmShop_Purchasing add PurchasingPersonName varchar(64) null
go

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'采购单提交人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'lmShop_Purchasing', @level2type=N'COLUMN',@level2name=N'PurchasingPersonName'
GO
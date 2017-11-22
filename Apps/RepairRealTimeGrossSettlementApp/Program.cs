using RepairRealTimeGrossSettlementApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairRealTimeGrossSettlementApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Tuple<Guid, Guid>> warehouseIdHostingFilialeIdList = new List<Tuple<Guid, Guid>>();
            // 上海仓->上海可镜商贸
            warehouseIdHostingFilialeIdList.Add(new Tuple<Guid, Guid>(new Guid("84B303F5-2AA6-437D-9D23-3488AD55D278"), new Guid("58437edc-87b7-4995-a5c0-bb5fd0fe49e0")));
            
            // 河北仓->上海可镜商贸
            warehouseIdHostingFilialeIdList.Add(new Tuple<Guid, Guid>(new Guid("5AB6A210-D916-43C0-AE06-1020865D98D8"), new Guid("1A856F24-4FC9-49CA-8789-E15542F56D35")));

            // 兰溪仓->百秀浙江
            warehouseIdHostingFilialeIdList.Add(new Tuple<Guid, Guid>(new Guid("ef5ae4a5-7f6d-48e8-b9b2-c91a7b6b5e83"), new Guid("75621B55-2FA3-4FCF-B68A-039C28F560B6")));
            
            //if (GlobalConfig.InitRealTimeSettlementEnabled)
            //{
            //    Console.WriteLine("重新初始化历史结算价");
            //    Dal.InitHistoryRealTimeGrossSettlement();
            //    Console.WriteLine("重新初始化历史结算价 完成");
            //}

            Console.WriteLine("重新计算初始化之后产生的结算价");

            // 物流配送公司结算价
            Parallel.Invoke(() =>
            {
                // 上海可镜商贸
                Console.WriteLine("上海可镜商贸 结算价重新计算中");
                var filialeId = new Guid("58437edc-87b7-4995-a5c0-bb5fd0fe49e0");
                Repair(filialeId, warehouseIdHostingFilialeIdList);
                Console.WriteLine("上海可镜商贸 计算完成");
            }, () =>
            {
                // 河北可镜网络
                Console.WriteLine("河北可镜网络 结算价重新计算中");
                var filialeId = new Guid("1A856F24-4FC9-49CA-8789-E15542F56D35");
                Repair(filialeId, warehouseIdHostingFilialeIdList);
                Console.WriteLine("河北可镜网络 计算完成");
            }, () =>
            {
                // 百秀浙江
                Console.WriteLine("百秀浙江 结算价重新计算中");
                var filialeId = new Guid("75621B55-2FA3-4FCF-B68A-039C28F560B6");
                Repair(filialeId, warehouseIdHostingFilialeIdList);
                Console.WriteLine("百秀浙江 计算完成");
            });

            // 销售公司结算价
            Parallel.Invoke(() =>
            {
                // 可得光学
                Console.WriteLine("可得光学 结算价重新计算中");
                var filialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698");
                Repair(filialeId, warehouseIdHostingFilialeIdList);
                Console.WriteLine("可得光学 计算完成");
            }, () =>
            {
                // 上海百秀
                Console.WriteLine("上海百秀 结算价重新计算中");
                var filialeId = new Guid("444E0C93-1146-4386-BAE2-CB352DA70499");
                Repair(filialeId, warehouseIdHostingFilialeIdList);
                Console.WriteLine("上海百秀 计算完成");
            }, () =>
            {
                // 镜拓光学
                Console.WriteLine("镜拓光学 结算价重新计算中");
                var filialeId = new Guid("43609645-97DD-4AE4-989D-F3C867969A99");
                Repair(filialeId, warehouseIdHostingFilialeIdList);
                Console.WriteLine("镜拓光学 计算完成");
            });

            Console.WriteLine("结算价 重新计算完成！ 按任意键退出...");
            Console.ReadKey();
        }

        private static void Repair(Guid filialeId, List<Tuple<Guid, Guid>> warehouseIdHostingFilialeIdList)
        {
            try
            {
                var isHostingFiliale = warehouseIdHostingFilialeIdList.Select(m => m.Item2).Contains(filialeId);
                var innerPurchasePriceRate = GlobalConfig.InnerPurchasePriceRate;
                var billNoList = Dal.GetBillNoList(filialeId, GlobalConfig.RealTimeSettlementOccurTimeStartWith);
                var newList = new List<RealTimeGrossSettlementInfo>();
                foreach (var billNo in billNoList)
                {
                    var list = Dal.GetListByBillNo(filialeId, billNo);
                    if (list.Count == 0) continue;
                    if (isHostingFiliale)
                    {
                        int relatedTradeType = list[0].RelatedTradeType;
                        if (relatedTradeType == 1)
                        {
                            // 采购入库
                            foreach (var item in list)
                            {
                                var goodsId = item.GoodsId;
                                var occurTime = item.OccurTime;
                                var lastSettleInfo = Dal.GetLatestBeforeSpecificTime(filialeId, goodsId, occurTime);
                                var lastUnitPrice = lastSettleInfo == null ? 0 : lastSettleInfo.UnitPrice;
                                decimal newUnitPrice = item.UnitPrice;
                                item.GoodsAmountInBill = Math.Abs(item.GoodsAmountInBill);

                                if (item.StockQuantity < item.GoodsQuantityInBill)
                                {
                                    // 采购后的库存应不能小于采购入库单内的数量
                                    newUnitPrice = item.GoodsAmountInBill / item.GoodsQuantityInBill;
                                    if (newUnitPrice == 0)
                                    {
                                        newUnitPrice = lastUnitPrice;// 按当前单据计算结算价，如果为0，则取上次结算价
                                    }
                                }
                                else
                                {
                                    if (lastSettleInfo != null && lastSettleInfo.StockQuantity <= 0
                                        && lastSettleInfo.RelatedTradeType == 2)
                                    {
                                        // 上次的结算价时采购退货，且库存为0的，则按上次退货和这次进货一起算
                                        var x = ((item.StockQuantity - item.GoodsQuantityInBill + lastSettleInfo.GoodsQuantityInBill) * lastUnitPrice
                                            + lastSettleInfo.GoodsAmountInBill
                                            + item.GoodsAmountInBill)
                                            / item.StockQuantity;
                                    }
                                    else
                                    {
                                        newUnitPrice = (lastUnitPrice * (item.StockQuantity - item.GoodsQuantityInBill) + item.GoodsAmountInBill) / item.StockQuantity;
                                    }
                                }

                                newUnitPrice = Math.Round(newUnitPrice, 4);
                                if (item.UnitPrice != newUnitPrice)
                                {
                                    item.UnitPrice = newUnitPrice;
                                    newList.Add(item);
                                    Dal.Save(item);
                                }
                            }
                        }
                        else if (relatedTradeType == 2)
                        {
                            // 采购退货
                            foreach (var item in list)
                            {
                                var goodsId = item.GoodsId;
                                var occurTime = item.OccurTime;
                                var lastSettleInfo = Dal.GetLatestBeforeSpecificTime(filialeId, goodsId, occurTime);
                                var lastUnitPrice = lastSettleInfo == null ? 0 : lastSettleInfo.UnitPrice;
                                decimal newUnitPrice = item.UnitPrice;

                                if (item.StockQuantity <= 0)
                                {
                                    newUnitPrice = lastUnitPrice;
                                }
                                else
                                {
                                    if (lastSettleInfo != null && lastSettleInfo.StockQuantity < item.GoodsQuantityInBill
                                           && lastSettleInfo.RelatedTradeType == 2)
                                    {
                                        newUnitPrice = lastUnitPrice;
                                    }
                                    else if (occurTime < new DateTime(2017, 10, 1))
                                    {
                                        newUnitPrice = (lastUnitPrice * (item.StockQuantity + item.GoodsQuantityInBill) + item.GoodsAmountInBill) / item.StockQuantity;
                                        if (newUnitPrice < 0)
                                        {
                                            newUnitPrice = lastUnitPrice;
                                        }
                                    }
                                    else
                                    {
                                        // 采购退货后，如果当前库存为0，则不计算，直接取上次结算价
                                        newUnitPrice = (lastUnitPrice * (item.StockQuantity + item.GoodsQuantityInBill) + item.GoodsAmountInBill) / item.StockQuantity;
                                    }
                                }

                                newUnitPrice = Math.Round(newUnitPrice, 4);
                                if (item.UnitPrice != newUnitPrice)
                                {
                                    item.UnitPrice = newUnitPrice;
                                    newList.Add(item);
                                    Dal.Save(item);
                                }
                            }
                        }
                        else if (relatedTradeType == 3)
                        {
                            // 单据红冲
                            foreach (var item in list)
                            {
                                var goodsId = item.GoodsId;
                                var occurTime = item.OccurTime;
                                var lastSettleInfo = Dal.GetLatestBeforeSpecificTime(filialeId, goodsId, occurTime);
                                var lastUnitPrice = lastSettleInfo == null ? 0 : lastSettleInfo.UnitPrice;
                                bool originExtField1IsZero = item.ExtField_1 == 0;
                                decimal newUnitPrice = item.UnitPrice;

                                // 重新设置字段的数据：GoodsAmountInBill、ExtField_1                            
                                var newInDocumentPair = Dal.GetDocumentRed(billNo, goodsId);
                                var newInDocument = newInDocumentPair.Item1;// 入库红冲单，对应的新入库单
                                var newInDocumentDetails = newInDocumentPair.Item2;// 入库红冲单，对应的新入库单
                                if (newInDocument == null || newInDocument.DocumentType != 0 || newInDocument.RedType != 1 || newInDocument.State != 4
                                    || newInDocumentDetails == null || newInDocumentDetails.Count == 0)
                                {
                                    Dal.Delete(item);
                                    continue;
                                }
                                var purchaseStockInRecordPair = Dal.GetStorageRecord(newInDocument.LinkTradeId, goodsId);
                                var purchaseStockInRecord = purchaseStockInRecordPair.Item1;// 入库红冲单，对应的原始入库单
                                var purchaseStockInDetails = purchaseStockInRecordPair.Item2;// 入库红冲单，对应的原始入库单明细
                                if (purchaseStockInRecord == null || purchaseStockInRecord.StockType != 1 || purchaseStockInRecord.StockState != 4
                                    || purchaseStockInDetails == null || purchaseStockInDetails.Count == 0)
                                {
                                    Dal.Delete(item);
                                    continue;
                                }

                                item.GoodsAmountInBill = newInDocumentDetails.Sum(m => Math.Abs(m.UnitPrice * m.Quantity));// 红冲单金额
                                item.ExtField_1 = purchaseStockInDetails.Sum(m => Math.Abs(m.UnitPrice * m.Quantity)); // 原采购入库单金额

                                if (item.GoodsAmountInBill == item.ExtField_1)
                                {
                                    // 单据红冲金额和原采购入库单的金额一致，则不需要计算结算价
                                    Dal.Delete(item);
                                    continue;
                                }

                                if (item.StockQuantity <= 0)
                                {
                                    newUnitPrice = lastUnitPrice;
                                }
                                else
                                {
                                    var tmpUnitPrice = (item.StockQuantity * lastUnitPrice + item.GoodsAmountInBill) / (item.StockQuantity + item.GoodsQuantityInBill);// 先按进货方式计算
                                    newUnitPrice = (tmpUnitPrice * (item.StockQuantity + item.GoodsQuantityInBill) - item.ExtField_1) / item.StockQuantity;// 再按退货方式计算
                                }

                                newUnitPrice = Math.Round(newUnitPrice, 4);
                                if (item.UnitPrice != newUnitPrice || originExtField1IsZero)
                                {
                                    item.UnitPrice = newUnitPrice;
                                    newList.Add(item);
                                    Dal.Save(item);
                                }
                            }
                        }
                        else if (relatedTradeType == 4)
                        {
                            // 拆分组合
                            if (billNo.StartsWith("SN", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // 拆分组合的拆分单
                                var bill = Dal.GetCombineOrSplitGoods(billNo);
                                if (bill == null) continue;
                                var originGoodsId = bill.GoodsId;
                                var originGoodsQuantity = bill.Quantity;
                                var targetGoodsIdList = bill.Details.Select(m => m.Item1).ToList();

                                var originGoodsLastUnitPrice = Dal.GetLatestUnitPriceBeforeSpecificTime(filialeId, originGoodsId, list.First().OccurTime, isHostingFiliale);
                                var targetGoodsLastUnitPriceDict = Dal.GetLatestUnitPriceListBeforeSpecificTimeByMultiGoods(filialeId, targetGoodsIdList, list.First().OccurTime);

                                if (originGoodsLastUnitPrice == 0) { continue; }

                                var originGoodsAmount = originGoodsLastUnitPrice * originGoodsQuantity;
                                var totalTargetGoodsAmount = bill.Details.Sum(m => (targetGoodsLastUnitPriceDict.ContainsKey(m.Item1) ? targetGoodsLastUnitPriceDict[m.Item1] : 0) * m.Item2);
                                bill.Details.ForEach(m =>
                                {
                                    var item = list.First(x => x.GoodsId == m.Item1);
                                    decimal newUnitPrice = item.UnitPrice;
                                    var targetGoodsAmount = (targetGoodsLastUnitPriceDict.ContainsKey(m.Item1) ? targetGoodsLastUnitPriceDict[m.Item1] : 0) * m.Item2;
                                    if (targetGoodsAmount > 0)
                                    {
                                        var targetUnitPrice = originGoodsAmount * (targetGoodsAmount / totalTargetGoodsAmount) / m.Item2;
                                        var newGoodsAmountInBill = item.GoodsQuantityInBill * targetUnitPrice;
                                        if (item.StockQuantity > 0)
                                        {
                                            newUnitPrice = ((item.StockQuantity - item.GoodsQuantityInBill) * targetGoodsLastUnitPriceDict[m.Item1] + newGoodsAmountInBill) / item.StockQuantity;

                                        }
                                        else
                                        {
                                            newUnitPrice = targetGoodsLastUnitPriceDict[m.Item1];
                                        }

                                        newUnitPrice = Math.Round(newUnitPrice, 4);
                                        if (item.UnitPrice != newUnitPrice || item.GoodsAmountInBill != newGoodsAmountInBill)
                                        {
                                            item.UnitPrice = newUnitPrice;
                                            item.GoodsAmountInBill = newGoodsAmountInBill;
                                            newList.Add(item);
                                            Dal.Save(item);
                                        }
                                    }
                                });
                            }
                            if (billNo.StartsWith("CN", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // 拆分组合的组合单
                                var bill = Dal.GetCombineOrSplitGoods(billNo);
                                if (bill == null) continue;
                                var originGoodsIdList = bill.Details.Select(m => m.Item1).ToList();
                                var targetGoodsId = bill.GoodsId;
                                var targetGoodsQuantity = bill.Quantity;

                                var originGoodsLastUnitPriceDict = Dal.GetLatestUnitPriceListBeforeSpecificTimeByMultiGoods(filialeId, originGoodsIdList, list.First().OccurTime);
                                var targetGoodsLastUnitPrice = Dal.GetLatestUnitPriceBeforeSpecificTime(filialeId, targetGoodsId, list.First().OccurTime, isHostingFiliale);

                                if (originGoodsLastUnitPriceDict.Count != originGoodsIdList.Count) { continue; }

                                var totalOriginGoodsAmount = bill.Details.Sum(m => (originGoodsLastUnitPriceDict.ContainsKey(m.Item1) ? originGoodsLastUnitPriceDict[m.Item1] : 0) * m.Item2);

                                var item = list.First(m => m.GoodsId == targetGoodsId);
                                decimal newUnitPrice = item.UnitPrice;
                                var targetGoodsAmount = totalOriginGoodsAmount;
                                if (targetGoodsAmount > 0)
                                {
                                    var targetUnitPrice = targetGoodsAmount / targetGoodsQuantity;
                                    var newGoodsAmountInBill = item.GoodsQuantityInBill * targetUnitPrice;
                                    if (item.StockQuantity > 0)
                                    {
                                        newUnitPrice = ((item.StockQuantity - item.GoodsQuantityInBill) * targetGoodsLastUnitPrice + newGoodsAmountInBill) / item.StockQuantity;

                                    }
                                    else
                                    {
                                        newUnitPrice = targetGoodsLastUnitPrice;
                                    }

                                    newUnitPrice = Math.Round(newUnitPrice, 4);
                                    if (item.UnitPrice != newUnitPrice || item.GoodsAmountInBill != newGoodsAmountInBill)
                                    {
                                        item.UnitPrice = newUnitPrice;
                                        item.GoodsAmountInBill = newGoodsAmountInBill;
                                        newList.Add(item);
                                        Dal.Save(item);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var hostingFilialeId = Dal.GetHostingFilialeIdByStorageRecordTradeCode(list.First().RelatedTradeNo);
                        if (hostingFilialeId == Guid.Empty) continue;
                        int relatedTradeType = list[0].RelatedTradeType;
                        if (relatedTradeType == 1)
                        {
                            // 采购入库
                            foreach (var item in list)
                            {
                                var goodsId = item.GoodsId;
                                var occurTime = item.OccurTime;
                                var lastSettleInfo = Dal.GetLatestBeforeSpecificTime(filialeId, goodsId, occurTime);
                                var lastUnitPrice = lastSettleInfo == null ? 0 : lastSettleInfo.UnitPrice;
                                decimal newUnitPrice = item.UnitPrice;
                                item.GoodsAmountInBill = Math.Abs(item.GoodsAmountInBill);

                                if (item.StockQuantity < item.GoodsQuantityInBill)
                                {
                                    // 采购后的库存应不能小于采购入库单内的数量
                                    newUnitPrice = item.GoodsAmountInBill / item.GoodsQuantityInBill;
                                    if (newUnitPrice == 0)
                                    {
                                        newUnitPrice = lastUnitPrice;// 按当前单据计算结算价，如果为0，则取上次结算价
                                    }
                                }
                                else
                                {
                                    if (lastSettleInfo != null && lastSettleInfo.StockQuantity <= 0
                                        && lastSettleInfo.RelatedTradeType == 2)
                                    {
                                        // 上次的结算价时采购退货，且库存为0的，则按上次退货和这次进货一起算
                                        var x = ((item.StockQuantity - item.GoodsQuantityInBill + lastSettleInfo.GoodsQuantityInBill) * lastUnitPrice
                                            + lastSettleInfo.GoodsAmountInBill
                                            + item.GoodsAmountInBill)
                                            / item.StockQuantity;
                                    }
                                    else
                                    {
                                        newUnitPrice = (lastUnitPrice * (item.StockQuantity - item.GoodsQuantityInBill) + item.GoodsAmountInBill) / item.StockQuantity;
                                    }
                                }

                                newUnitPrice = Math.Round(newUnitPrice, 4);
                                if (item.UnitPrice != newUnitPrice)
                                {
                                    item.UnitPrice = newUnitPrice;
                                    newList.Add(item);
                                    Dal.Save(item);
                                }
                            }
                        }
                        else if (relatedTradeType == 2)
                        {
                            // 采购退货
                            foreach (var item in list)
                            {
                                var goodsId = item.GoodsId;
                                var occurTime = item.OccurTime;
                                var lastSettleInfo = Dal.GetLatestBeforeSpecificTime(filialeId, goodsId, occurTime);
                                var lastUnitPrice = lastSettleInfo == null ? 0 : lastSettleInfo.UnitPrice;
                                decimal newUnitPrice = item.UnitPrice;

                                if (item.StockQuantity <= 0)
                                {
                                    newUnitPrice = lastUnitPrice;
                                }
                                else
                                {
                                    if (lastSettleInfo != null && lastSettleInfo.StockQuantity < item.GoodsQuantityInBill
                                           && lastSettleInfo.RelatedTradeType == 2)
                                    {
                                        newUnitPrice = lastUnitPrice;
                                    }
                                    else if (occurTime < new DateTime(2017, 10, 1))
                                    {
                                        newUnitPrice = (lastUnitPrice * (item.StockQuantity + item.GoodsQuantityInBill) + item.GoodsAmountInBill) / item.StockQuantity;
                                        if (newUnitPrice < 0)
                                        {
                                            newUnitPrice = lastUnitPrice;
                                        }
                                    }
                                    else
                                    {
                                        // 采购退货后，如果当前库存为0，则不计算，直接取上次结算价
                                        newUnitPrice = (lastUnitPrice * (item.StockQuantity + item.GoodsQuantityInBill) + item.GoodsAmountInBill) / item.StockQuantity;
                                    }
                                }

                                newUnitPrice = Math.Round(newUnitPrice, 4);
                                if (item.UnitPrice != newUnitPrice)
                                {
                                    item.UnitPrice = newUnitPrice;
                                    newList.Add(item);
                                    Dal.Save(item);
                                }
                            }
                        }
                        else if (relatedTradeType == 3)
                        {
                            // 单据红冲
                            foreach (var item in list)
                            {
                                var goodsId = item.GoodsId;
                                var occurTime = item.OccurTime;
                                var lastSettleInfo = Dal.GetLatestBeforeSpecificTime(filialeId, goodsId, occurTime);
                                var lastUnitPrice = lastSettleInfo == null ? 0 : lastSettleInfo.UnitPrice;
                                bool originExtField1IsZero = item.ExtField_1 == 0;
                                decimal newUnitPrice = item.UnitPrice;

                                // 重新设置字段的数据：GoodsAmountInBill、ExtField_1                            
                                var newInDocumentPair = Dal.GetDocumentRed(billNo, goodsId);
                                var newInDocument = newInDocumentPair.Item1;// 入库红冲单，对应的新入库单
                                var newInDocumentDetails = newInDocumentPair.Item2;// 入库红冲单，对应的新入库单
                                if (newInDocument == null || newInDocument.DocumentType != 0 || newInDocument.RedType != 1 || newInDocument.State != 4
                                    || newInDocumentDetails == null || newInDocumentDetails.Count == 0)
                                {
                                    Dal.Delete(item);
                                    continue;
                                }
                                var purchaseStockInRecordPair = Dal.GetStorageRecord(newInDocument.LinkTradeId, goodsId);
                                var purchaseStockInRecord = purchaseStockInRecordPair.Item1;// 入库红冲单，对应的原始入库单
                                var purchaseStockInDetails = purchaseStockInRecordPair.Item2;// 入库红冲单，对应的原始入库单明细
                                if (purchaseStockInRecord == null || purchaseStockInRecord.StockType != 1 || purchaseStockInRecord.StockState != 4
                                    || purchaseStockInDetails == null || purchaseStockInDetails.Count == 0)
                                {
                                    Dal.Delete(item);
                                    continue;
                                }

                                item.GoodsAmountInBill = newInDocumentDetails.Sum(m => Math.Abs(m.UnitPrice * m.Quantity));// 红冲单金额
                                item.ExtField_1 = purchaseStockInDetails.Sum(m => Math.Abs(m.UnitPrice * m.Quantity)); // 原采购入库单金额

                                if (item.GoodsAmountInBill == item.ExtField_1)
                                {
                                    // 单据红冲金额和原采购入库单的金额一致，则不需要计算结算价
                                    Dal.Delete(item);
                                    continue;
                                }

                                if (item.StockQuantity <= 0)
                                {
                                    newUnitPrice = lastUnitPrice;
                                }
                                else
                                {
                                    var tmpUnitPrice = (item.StockQuantity * lastUnitPrice + item.GoodsAmountInBill) / (item.StockQuantity + item.GoodsQuantityInBill);// 先按进货方式计算
                                    newUnitPrice = (tmpUnitPrice * (item.StockQuantity + item.GoodsQuantityInBill) - item.ExtField_1) / item.StockQuantity;// 再按退货方式计算
                                }

                                newUnitPrice = Math.Round(newUnitPrice, 4);
                                if (item.UnitPrice != newUnitPrice || originExtField1IsZero)
                                {
                                    item.UnitPrice = newUnitPrice;
                                    newList.Add(item);
                                    Dal.Save(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}

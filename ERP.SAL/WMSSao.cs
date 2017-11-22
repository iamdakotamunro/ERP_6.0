using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using ERP.SAL.Goods;
using ERP.SAL.WMS;
using KeedeGroup.WMS.Application.Contract.ExternalInterface;
using KeedeGroup.WMS.Application.Contract.Proxy;
using AddressLibraryDTO = ERP.SAL.WMS.AddressLibraryDTO;
using CityDTO = ERP.SAL.WMS.CityDTO;
using CountryDTO = ERP.SAL.WMS.CountryDTO;
using DistrictDTO = ERP.SAL.WMS.DistrictDTO;
using ErpPurchaseDeclarationBill = ERP.Model.WMS.ErpPurchaseDeclarationBill;
using ExpressBasicDTO = ERP.SAL.WMS.ExpressBasicDTO;
using ExpressCostDTO = ERP.SAL.WMS.ExpressCostDTO;
using HostingFilialeAuth = ERP.SAL.WMS.HostingFilialeAuth;
using OrderGoodsBatchNoDTO = ERP.SAL.WMS.OrderGoodsBatchNoDTO;
using ProvinceDTO = ERP.SAL.WMS.ProvinceDTO;
using ProxyFiliale = ERP.SAL.WMS.ProxyFiliale;
using RequireSearchDTO = ERP.Model.WMS.RequireSearchDTO;
using StorageAuth = ERP.SAL.WMS.StorageAuth;
using WarehouseAuth = ERP.SAL.WMS.WarehouseAuth;
using WarehouseAuthAndFilialeDTO = ERP.SAL.WMS.WarehouseAuthAndFilialeDTO;
using WarehouseBasicDTO = ERP.SAL.WMS.WarehouseBasicDTO;
using WarehouseFilialeAuth = ERP.SAL.WMS.WarehouseFilialeAuth;
using Config.Keede.Library;

namespace ERP.SAL
{
    /// <summary>WMS
    /// </summary>
    public class WMSSao
    {
        private static readonly ERPProxy _erpProxy = new ERPProxy(new ProxySerialize(), ConfManager.GetAppsetting("WmsApiUrl"));

        /// <summary>新增进货单据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ResultInfo InsertInGoodsBill(StorageRecordApplyInDTO request, out string billNo)
        {
            var inGoodsBillAddRequest = new InGoodsBillAddRequest
            {
                HostingFilialeId = request.HostingFilialeId,
                OperatorId = request.OperatorId,
                OperatorName = request.OperatorName,
                PurchaseResponsiblePersonName = string.IsNullOrEmpty(request.PurchaseResponsiblePersonName) ? request.OperatorName : request.PurchaseResponsiblePersonName,
                SourceNo = request.SourceNo,
                StorageType = request.StorageType,
                SupplierName = request.SupplierName,
                WarehouseId = request.WarehouseId,
                SaleFilialeId = request.SaleFilialeId
            };

            var list = request.Details.Select(detail => new InGoodsBillDetailAddRequest
            {
                GoodsId = detail.GoodsId,
                GoodsName = detail.GoodsName,
                InQuantity = detail.InQuantity,
                RealGoodsId = detail.RealGoodsId,
                Sku = detail.Sku == "&nbsp;"
                    ? ""
                    : detail.Sku,
                Unit = detail.Unit,
                BatchNo = detail.BatchNo,
                ShelfType = detail.ShelfType
                
            }).ToList();
            inGoodsBillAddRequest.Details = list;
            var result = _erpProxy.InsertInGoodsBill(inGoodsBillAddRequest);
            billNo = result != null && result.IsSuccess ? result.Msg : "";
            return ResultInfo.ConvertResultInfo(result);
        }

        /// <summary>获取有权限的仓库(含储位托管公司)
        /// </summary>
        /// <param name="operatorId">操作者ID（登录员工ID）</param>
        /// <returns></returns>
        public static List<WarehouseAuth> GetWarehouseAuth(Guid operatorId)
        {
            var result = _erpProxy.GetWarehouseAuth(operatorId);
            return result.IsSuccess ? result.Data.WarehouseAuths.Select(act => new WarehouseAuth
            {
                WarehouseId = act.WarehouseId,
                WarehouseName = act.WarehouseName,
                Storages = act.Storages.Select(s => new StorageAuth
                {
                    StorageType = s.StorageType,
                    StorageTypeName = s.StorageTypeName,
                    IsReal = s.IsReal,
                    Filiales = s.Filiales.Select(f => new HostingFilialeAuth
                    {
                        HostingFilialeId = f.HostingFilialeId,
                        HostingFilialeName = f.HostingFilialeName,
                        ProxyFiliales = f.ProxyFiliales.Select(ent=>new WMS.ProxyFiliale(ent.ProxyFilialeId,ent.ProxyFilialeName,ent.GoodsTypes)).ToList()
                    }).ToList()
                }).ToList()
            }).ToList() : new List<WarehouseAuth>();
        }


        /// <summary>获取有权限的仓库(不含储位信息)
        /// </summary>
        /// <param name="operatorId">操作者ID（登录员工ID）</param>
        /// <returns></returns>
        public static List<WMS.WarehouseFilialeAuth> GetWarehouseAndFilialeAuth(Guid operatorId)
        {
            var result = _erpProxy.GetWarehouseFilialeAuth(operatorId);
            return result.IsSuccess
                ? result.Data.WarehouseAuths.Select(act => new WMS.WarehouseFilialeAuth
                {
                    WarehouseId = act.WarehouseId,
                    WarehouseName = act.WarehouseName,
                    FilialeAuths = act.FilialeAuths.Select(ent => new WMS.HostingFilialeAuth(ent.HostingFilialeId, ent.HostingFilialeName) {
                        ProxyFiliales=ent.ProxyFiliales.Select(p=>new WMS.ProxyFiliale {
                            ProxyFilialeId=p.ProxyFilialeId,
                            ProxyFilialeName=p.ProxyFilialeName,
                            GoodsTypes=p.GoodsTypes
                        }).ToList()
                    }).ToList()
                }).ToList():new List<WMS.WarehouseFilialeAuth>();
        }

        /// <summary>获取有权限的仓库(不含储位信息)
        /// </summary>
        /// <param name="operatorId">操作者ID（登录员工ID）</param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public static WarehouseFilialeAuth GetSingleWarehouseAndFilialeAuth(Guid operatorId,Guid warehouseId)
        {
            var result = _erpProxy.GetWarehouseFilialeAuth(operatorId);
            if(result==null || !result.IsSuccess ||result.Data==null) return null;
            var warehouseFiliale = result.Data.WarehouseAuths.FirstOrDefault(ent => ent.WarehouseId == warehouseId);
            if (warehouseFiliale == null) return null;
            return new WMS.WarehouseFilialeAuth
                {
                    WarehouseId = warehouseFiliale.WarehouseId,
                    WarehouseName = warehouseFiliale.WarehouseName,
                    FilialeAuths = warehouseFiliale.FilialeAuths.Select(ent => new WMS.HostingFilialeAuth(ent.HostingFilialeId, ent.HostingFilialeName)
                    {
                        ProxyFiliales = ent.ProxyFiliales.Select(p => new WMS.ProxyFiliale
                        {
                            ProxyFilialeId = p.ProxyFilialeId,
                            ProxyFilialeName = p.ProxyFilialeName,
                            GoodsTypes = p.GoodsTypes
                        }).ToList()
                    }).ToList()
                };
        }

        public static List<Int32> GetPurchaseGoodsTypes(Guid warehouseId, Guid hostingFilialeId)
        {
            return _erpProxy.GetPurchaseGoodsTypes(warehouseId,hostingFilialeId);
        }  

        /// <summary>
        /// 获取所有授权且可使用仓库
        /// </summary>
        /// <param name="personnelId"></param>
        /// <returns></returns>
        public static Dictionary<Guid, String> GetWarehouseAuthDics(Guid personnelId)
        {
            var result = _erpProxy.GetWarehouseFilialeAuth(personnelId);
            return result.IsSuccess
                ? result.Data.WarehouseAuths.ToDictionary(ent => ent.WarehouseId, ent => ent.WarehouseName)
                : new Dictionary<Guid, string>();
        }

        /// <summary>
        /// 获取仓库列表和物流公司列表
        /// </summary>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        public static WarehouseAuthAndFilialeDTO GetWarehouseAuthDic(Guid operatorId)
        {
            var result = _erpProxy.GetWarehouseFilialeAuth(operatorId);
            Dictionary<Guid, string> warehouseDics = new Dictionary<Guid, string>();
            Dictionary<Guid, string> filialeDics = new Dictionary<Guid, string>();
            if (result!=null && result.IsSuccess && result.Data!=null)
            {
                foreach (var item in result.Data.WarehouseAuths)
                {
                    warehouseDics.Add(item.WarehouseId,item.WarehouseName);
                    foreach (var hostingFilialeAuth in item.FilialeAuths.Where(ent=>!filialeDics.ContainsKey(ent.HostingFilialeId)))
                    {
                        filialeDics.Add(hostingFilialeAuth.HostingFilialeId, hostingFilialeAuth.HostingFilialeName);
                        foreach (var proxyFiliale in hostingFilialeAuth.ProxyFiliales.Where(ent=>!filialeDics.ContainsKey(ent.ProxyFilialeId)))
                        {
                            filialeDics.Add(proxyFiliale.ProxyFilialeId,proxyFiliale.ProxyFilialeName);
                        }
                    }
                }  
            }
            return new WarehouseAuthAndFilialeDTO
            {
                HostingFilialeDic = filialeDics,
                WarehouseDics = warehouseDics
            };
        }

        /// <summary>新增出货单据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool InsertOutGoodsBill(StorageRecordApplyOutDTO request, out string billNo, out string msg)
        {
            var outGoodsBillOutBoundAddRequest = new OutGoodsBillOutBoundAddRequest
            {
                HostingFilialeId = request.HostingFilialeId,
                OperatorId = request.OperatorId,
                OperatorName = request.OperatorName,
                OutBillNo = request.OutBillNo,
                OutDescription = request.OutDescription,
                StorageType = request.StorageType,
                SupplierName = request.SupplierName,
                WarehouseId = request.WarehouseId,
                IsAfterSaleInferior = request.IsAfterSaleInferior,
                SaleFilialeId = request.SaleFilialeId
            };

            var list = request.Details.Select(detail => new OutGoodsBillOutBoundDetailAddRequest
            {
                BatchNo = detail.BatchNo,
                GoodsCode = detail.GoodsCode,
                GoodsId = detail.GoodsId,
                GoodsName = detail.GoodsName,
                OutQuantity = Math.Abs(detail.OutQuantity),
                RealGoodsId = detail.RealGoodsId,
                Sku = detail.Sku == "&nbsp;"
                    ? ""
                    : detail.Sku,
                Unit = detail.Unit,
            }).ToList();
            outGoodsBillOutBoundAddRequest.Details = list;

            var result = _erpProxy.InsertOutGoodsBill(outGoodsBillOutBoundAddRequest);
            billNo = msg = string.Empty;
            if (result != null)
            {
                if (result.IsSuccess)
                {
                    billNo = result.Msg;
                }
                else
                {
                    msg = result.Msg;
                }
            }

            return result != null && result.IsSuccess;
        }

        /// <summary>计算售后退货商品(售后区次品，坏件区坏品)
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, int> CalculateAutoReturnGoods(AutoReturnGoodsRequestDTO autoReturnGoodsRequestDTO)
        {
            var autoReturnGoodsRequest = new AutoReturnGoodsRequest
            {
                HostingFilialeId = autoReturnGoodsRequestDTO.HostingFilialeId,
                StorageType = autoReturnGoodsRequestDTO.StorageType,
                WarehouseId = autoReturnGoodsRequestDTO.WarehouseId
            };
            var result = _erpProxy.CalculateAutoReturnGoods(autoReturnGoodsRequest);
            return result.IsSuccess ? result.Data : new Dictionary<Guid, int>();
        }


        #region  实际库存查询  (公司库存查询、采购明细查看)

        #endregion

        #region 可用库存查询
        /// <summary>
        /// 获取商品可用库存
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, Int32> GoodsCanUsableStockForDicRealGoodsIdAndStockQuantity(Guid warehouseId, IEnumerable<Byte> storageTypes, IEnumerable<Guid> realGoodsIds, Guid hostingFilialeId)
        {
            return _erpProxy.GoodsSusableStock(warehouseId, storageTypes, realGoodsIds, hostingFilialeId);
        }

        /// <summary> 
        /// 公司库存查询(所有储位)
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public static Dictionary<Guid, int> StockSearchByGoodsIds(IEnumerable<Guid> goodsIds, Guid warehouseId, Guid? hostingFilialeId)
        {
            if (goodsIds == null || warehouseId == Guid.Empty)
                return null;

            return _erpProxy.StockTurnover(goodsIds, warehouseId, hostingFilialeId.Equals(Guid.Empty) ? null : hostingFilialeId);
        }

        /// <summary>
        /// 获取商品可用库存
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, Int32> GoodsEffitiveStock(Guid warehouseId, IEnumerable<Byte> storageTypes, IEnumerable<Guid> realGoodsIds, Guid hostingFilialeId)
        {
            return _erpProxy.GoodsEffitiveStock(warehouseId, storageTypes, realGoodsIds, hostingFilialeId);
        }

        #endregion

        /// <summary>赠品绑定
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        public static Dictionary<Guid, int> BindGift(IEnumerable<Guid> goodsIds)
        {
            return _erpProxy.BindGift(goodsIds);
        }

        /// <summary>入库单完成时的结算价计算
        /// </summary>
        /// <param name="realGoodsIds"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public static Dictionary<Guid, int> CalculateSettlementPriceWhenAuditInStorage(IEnumerable<Guid> realGoodsIds, Guid warehouseId)
        {
            return _erpProxy.CalculateSettlementPriceWhenAuditInStorage(realGoodsIds, warehouseId);
        }

        /// <summary>  商品设置页面删除子商品
        /// </summary>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        public static Dictionary<Guid, bool> CanDeleteForGoodsSet(IEnumerable<Guid> realGoodsIds)
        {
            return _erpProxy.CanDeleteForGoodsSet(realGoodsIds);
        }

        /// <summary> 商品设置页面能否操作卖库存商品缺货
        /// </summary>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        public static Dictionary<Guid, bool> CanSetScarcityForGoodsSet(IEnumerable<Guid> realGoodsIds)
        {
            return _erpProxy.CanSetScarcityForGoodsSet(realGoodsIds);
        }

        /// <summary>卖库存商品页面能否设置为缺货
        /// </summary>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        public static Dictionary<Guid, bool> CanSetScarcityForSaleStockGoods(IEnumerable<Guid> realGoodsIds)
        {
            return _erpProxy.CanSetScarcityForSaleStockGoods(realGoodsIds);
        }

        /// <summary>
        /// 根据储位获取商品库存
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="warehouseId"></param>
        /// <param name="storageType"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public static Dictionary<Guid, int> GetGoodsStockByStorageType(IEnumerable<Guid> goodsIds, Guid warehouseId,
            Byte storageType, Guid? hostingFilialeId)
        {
            return _erpProxy.GoodsSusableStock(warehouseId,new List<Byte> { (Byte)storageType}, goodsIds, Guid.Empty);
        }

        public static List<Model.WMS.UpDownGoodsQuantityLendDTO> GetGoodsStockByStorageTypeLend(IEnumerable<Guid> goodsIds, Guid warehouseId,
            Byte storageType, Guid? hostingFilialeId)
        {
            var result = _erpProxy.GoodsSusableLendStock(warehouseId, new List<Byte> { (Byte)storageType }, goodsIds, Guid.Empty);
            return result.Select(ent => new Model.WMS.UpDownGoodsQuantityLendDTO(ent.HostingFilialeId, ent.RealGoodsId, ent.Quantity, ent.ShelfType)).ToList();
        }

        /// <summary> 公司库存查询(所有储位)
        /// </summary>
        /// <param name="realGoodsIds"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public static List<Model.WMS.ErpStockSearchDTO> StockSearchList(IEnumerable<Guid> realGoodsIds,  Guid warehouseId, Guid? hostingFilialeId)
        {
            if (realGoodsIds == null || warehouseId == Guid.Empty)
                return null;
            var  result=_erpProxy.StockSearch(realGoodsIds, null , warehouseId, hostingFilialeId.Equals(Guid.Empty)?null: hostingFilialeId);
            return result.Select(ent => new Model.WMS.ErpStockSearchDTO(ent.FilialeId,ent.FilialeName,ent.StockQuantity)).ToList();
        }

        /// <summary>
        /// 库存查询、采购明细
        /// </summary>
        /// <param name="realGoodsIds"></param>
        /// <param name="storageTypes"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public static Dictionary<Guid,int> StockSearch(IEnumerable<Guid> realGoodsIds, IEnumerable<Byte> storageTypes, Guid warehouseId, Guid? hostingFilialeId)
        {
            Dictionary<Guid,int> dics=new Dictionary<Guid, int>();
            if (realGoodsIds == null || warehouseId == Guid.Empty)
                return dics;
            var result= _erpProxy.StockSearch(realGoodsIds, storageTypes, warehouseId, hostingFilialeId);
            if (result != null)
            {
                var data=result.SelectMany(ent => ent.StockQuantity.Select(act => new KeyAndValue(act.Key, act.Value)));
                dics = data.GroupBy(ent => ent.Key).ToDictionary(k => k.Key, v => v.Sum(act => act.Value));
            }  
            return dics;
        }

        /// <summary>报备统计
        /// </summary>
        /// <param name="realGoodsIds"></param>
        /// <param name="warehouseId"></param>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public static Dictionary<Guid, List<StockStatisticsDTO>> GetStockStatisticsDtos(Dictionary<Guid,int> realGoodsIds, Guid warehouseId, Guid filialeId)
        {
            var dataList = _erpProxy.StockStatistics(warehouseId, filialeId, realGoodsIds);
            if (dataList != null && dataList.Count > 0)
            {
                return dataList.ToDictionary(k => k.Key, v => v.Value.Select(ent => new StockStatisticsDTO
                {
                    CurrentStock = ent.CurrentStock,
                    FilialeId = ent.FilialeId,
                    FilialeName = ent.FilialeName,
                    UppingQuantity = ent.UppingQuantity,
                    RequireQuantity = ent.RequireQuantity,
                    RealGoodsId = ent.RealGoodsId,
                    SubtotalQuantity = ent.SubtotalQuantity
                }).ToList());
            }
            return new Dictionary<Guid, List<StockStatisticsDTO>>();
        }

        public static List<StockStatisticsDTO> GetStockStatisticsDtosForAuto(Guid warehouseId, Guid hostingFilialeId)
        {
            var dataList = _erpProxy.StockStatisticsForAuto(warehouseId, hostingFilialeId);
            if (dataList != null && dataList.Count > 0)
            {
                return dataList.Select(ent => new StockStatisticsDTO
                {
                    CurrentStock = ent.CurrentStock,
                    FilialeId = ent.FilialeId,
                    FilialeName = ent.FilialeName,
                    UppingQuantity = ent.UppingQuantity,
                    RequireQuantity = ent.RequireQuantity,
                    RealGoodsId = ent.RealGoodsId,
                    SubtotalQuantity = ent.SubtotalQuantity
                }).ToList();
            }
            return new List<StockStatisticsDTO>();
        }
        #region 进货申报

        public static Dictionary<Guid, List<PurchaseDeclarationDTO>> GetStockDeclareDtos(Guid warehouseId, IEnumerable<Guid> realGoodsIds)
        {
            var dataList = _erpProxy.PurchaseDeclaration(realGoodsIds, warehouseId);
            if (dataList != null && dataList.Count > 0)
            {
                return dataList.ToDictionary(realGoodsId => realGoodsId.Key, realGoodsId => realGoodsId.Value.Select(ent => new PurchaseDeclarationDTO
                {
                    CurrentQuantity = ent.CurrentQuantity, //当前库存数（不区分公司，即所有公司的总库存数）
                    GoodsId = ent.GoodsId, //
                    Quantity = ent.Quantity, //实际缺货数（不区分公司，即所有公司的总缺货数）
                    RealGoodsId = ent.RealGoodsId,
                    FilialeId=ent.HostingFilialeId
                }).ToList());
            }
            return new Dictionary<Guid, List<PurchaseDeclarationDTO>>();
        }

        public static List<Model.WMS.ErpPurchaseDeclarationBill> GetDeclareBillNos(Guid warehouseId, Guid realGoodsId)
        {
            var result= _erpProxy.DeclarationBillDtos(warehouseId, realGoodsId);
            return result.Select(ent => new Model.WMS.ErpPurchaseDeclarationBill
            {
                HostingFilialeId = ent.HostingFilialeId,
                OrderBills = ent.OrderBills.Select(act=>new ErpPurchaseDeclarationBill.Detail(act.OrderNo, act.OrderId, act.Quantity, act.OrderState,ent.HostingFilialeId)).ToList(),
                UppingQuantity = ent.UppingQuantity,
                StockQuantity = ent.StockQuantity,
                StorageBills = ent.StorageBills
            }).ToList();
        }

        /// <summary>
        /// 公司库存缺货查询
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public static List<OutOfStockGridDTO> FilialeStockLackSearch(Guid warehouseId, Guid hostingFilialeId)
        {
            var data = new List<OutOfStockGridDTO>();
            var dataList = _erpProxy.FilialeStockLackSearch(warehouseId, hostingFilialeId);
            if (dataList != null && dataList.Count > 0)
            {
                var centerSao = new GoodsCenterSao();
                var realGoodsIds = dataList.Select(act => act.RealGoodsId).Distinct().ToList();
                var goodsBaseInfos = centerSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsIds);
                var goodsChildInfos = centerSao.GetRealGoodsListByGoodsId(goodsBaseInfos.Select(act => act.Key).ToList());
                data.AddRange(from dto in dataList
                              let baseInfo = goodsBaseInfos[dto.RealGoodsId]
                              let child = goodsChildInfos.FirstOrDefault(act => act.RealGoodsId == dto.RealGoodsId)
                              select new OutOfStockGridDTO
                              {
                                  GoodsId = baseInfo.GoodsId,
                                  GoodsCode = baseInfo.GoodsCode,
                                  GoodsName = baseInfo.GoodsName,
                                  HostingFilialeId = dto.HostingFilialeId,
                                  TransferFiliales = dto.HaveStockFiliales != null && dto.HaveStockFiliales.Count > 0 ? dto.HaveStockFiliales.Select(act => new FilialeQuantity
                                  {
                                      HostingFilialeId = act.HostingFilialeId,
                                      HostingFilialeName = act.HostingFilialeName,
                                      Quantity = Math.Abs(act.Quantity)
                                  }).ToList() : new List<FilialeQuantity>(),
                                  Sku = child != null ? child.Specification : "",
                                  RealGoodsId = dto.RealGoodsId,
                                  Quantity = Math.Abs(dto.Quantity),
                                  Units = baseInfo.Units
                              });
            }
            return data;
        }

        /// <summary>
        /// 获取所有可用仓库
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, String> GetAllCanUseWarehouseDics()
        {
            return _erpProxy.GetAllCanUseWarehouseDics();
        }
        #endregion

        /// <summary>
        /// 获取地址库
        /// </summary>
        /// <returns></returns>
        public static AddressLibraryDTO GetAddressLibrary()
        {
            var result = _erpProxy.GetAddressLibrary();
            if (!result.IsSuccess)
                throw new Exception(result.Msg);

            return new AddressLibraryDTO
            {
                Cities = result.Data.Cities.Select(ent => new CityDTO
                {
                    CityId = ent.CityId,
                    CityName = ent.CityName,
                    ProvinceId = ent.ProvinceId
                }).ToList(),
                Countries = result.Data.Countries.Select(ent => new CountryDTO
                {
                    CountryId = ent.CountryId,
                    CountryName = ent.CountryName
                }).ToList(),
                Districts = result.Data.Districts.Select(ent => new DistrictDTO
                {
                    CityId = ent.CityId,
                    DistrictId = ent.DistrictId,
                    DistrictName = ent.DistrictName,
                    ZipCode = ent.ZipCode
                }).ToList(),
                Provinces = result.Data.Provinces.Select(ent => new ProvinceDTO
                {
                    CountryId = ent.CountryId,
                    ProvinceId = ent.ProvinceId,
                    ProvinceName = ent.ProvinceName
                }).ToList()
            };
        }

        /// <summary>
        /// 获取仓库具体信息
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public static WarehouseBasicDTO GetWarehouseDetail(Guid warehouseId)
        {
            var result = _erpProxy.GetWarehouseDetail(warehouseId);

            if (!result.IsSuccess)
                return null;

            return new WarehouseBasicDTO
            {
                LogisticFilialeId = result.Data.LogisticFilialeId,
                WarehouseId = result.Data.WarehouseId,
                WarehouseName = result.Data.WarehouseName,
                StorageTypes = result.Data.StorageTypes
            };
        }

        /// <summary>
        /// 获取快递列表
        /// </summary>
        /// <returns></returns>
        public static List<ExpressBasicDTO> GetExpresses()
        {
            return _erpProxy.GetExpresses().Select(ent => new ExpressBasicDTO
            {
                ExpressFullName = ent.ExpressFullName,
                ExpressId = ent.ExpressId,
                ExpressShortName = ent.ExpressShortName,
                PayMode = ent.PayMode,
                WarehouseId = ent.WarehouseId,
                CompanyId = ent.CompanyId
            }).ToList();
        }

        /// <summary>
        /// 获取快递运费列表
        /// </summary>
        /// <returns></returns>
        public static List<ExpressCostDTO> GetExpressCosts()
        {
            return _erpProxy.GetExpressCosts().Select(ent => new ExpressCostDTO
            {
                BillingMethod = ent.BillingMethod,
                ContinuedWeightFee = ent.ContinuedWeightFee,
                DistrictId = ent.DistrictId,
                ExpressId = ent.ExpressId,
                InitialCarriage = ent.InitialCarriage,
                InitialWeight = ent.InitialWeight,
                OperateFee = ent.OperateFee,
                SurfaceFee = ent.SurfaceFee,
                TransitFee = ent.TransitFee
            }).ToList();
        }

        /// <summary> 查询订单批号等信息
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public static List<OrderGoodsBatchNoDTO> GetOrderGoodsBatchNo(String orderNo)
        {
            var result = _erpProxy.GetOrderGoodsBatchNo(orderNo);
            var list = new List<OrderGoodsBatchNoDTO>();
            if (result != null && result.IsSuccess && result.Data.Count > 0)
            {
                list.AddRange(result.Data.Select(item => new OrderGoodsBatchNoDTO
                {
                    BatchNos = item.BatchNos,
                    ExpiryDate = item.ExpiryDate,
                    RealGoodsId = item.RealGoodsId,
                    StockQuantity = item.CanUseQuantity
                }));
            }
            return list;
        }

        /// <summary>获取订单称重计算的运费
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public OrderCarriageInfo GetOrderNoCarriage(string orderNo, out string errorMsg)
        {
            errorMsg = "WMS服务连接失败！";
            var result = _erpProxy.GetOrderNoCarriage(orderNo);
            if (result == null)
            {
                return null;
            }
            if (!result.IsSuccess)
            {
                errorMsg = result.Msg;
                return null;
            }
            var data = result.Data;
            return new OrderCarriageInfo { PackageWeight = data.PackageWeight, Carriage = data.Carriage, Province = data.Province, City = data.City };
        }

        public Dictionary<Guid, int> GetCurrentStockQuantity(Guid warehouseId, byte storageType, IEnumerable<Guid> realGoodsId, Guid? hostingFilialeId)
        {
            return _erpProxy.GetCurrentStockQuantity(warehouseId, storageType, realGoodsId, hostingFilialeId);
        }

        /// <summary>
        /// ERP订单完成插入出库单时获取出货单号
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public static string GetOutGoodsBillNoByOrderNo(string orderNo)
        {
            var result = _erpProxy.GetOutGoodsBillNoByOrderNo(orderNo);
            return result.IsSuccess ? result.Msg : string.Empty;
        }

        public ProcessOrderCertificationInfo GetProcessOrder(string processNo)
        {
            var result = _erpProxy.GetProcessOrder(processNo);
            if (result == null || !result.IsSuccess)
            {
                return null;
            }
            var processOrder = result.Data;
            return new ProcessOrderCertificationInfo
            {
                ProcessNo = processOrder.ProcessNo,
                OrderNos = processOrder.OrderNos,
                Processor = processOrder.Processor,
                ProcessDate = processOrder.ProcessDate,
                Name = processOrder.Name,
                HostingFilialeId = processOrder.HostingFilialeId,
                SkuList = processOrder.SkuList
            };
        }

        public static List<Model.WMS.RequireSearchDTO> GetRequireSearchDtos(Guid warehouseId,IEnumerable<Guid> realGoodsIds,Guid hostingFilialeId)
        {
            var result = _erpProxy.GetRequireSearchDtos(warehouseId,realGoodsIds,hostingFilialeId);
            if (result == null || !result.IsSuccess)
            {
                return new List<RequireSearchDTO>();
            }
            return result.Data.Select(ent => new Model.WMS.RequireSearchDTO(ent.HostingFilialeId,ent.HostingFilialeName,ent.RequiresDics,ent.StockQuantity)).ToList();
        }

        public static Dictionary<Guid, int> GetLackQuantity(Guid warehouseId, IEnumerable<Guid> realGoodsIds)
        {
            return _erpProxy.GetLackQuantity(warehouseId, realGoodsIds);
        }

        /// <summary>
        /// 商品需求查询、返回应该查哪些销售公司的需求
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="filialeId"></param>
        /// <param name="realGoodsId"></param>
        /// <returns></returns>
        public static List<Guid> GetSaleFilialeIds(Guid warehouseId, Guid filialeId,Guid realGoodsId)
        {
            return _erpProxy.GetSaleFilialeIds(warehouseId, filialeId, realGoodsId);
        }

        /// <summary>
        /// 按仓库、商品类型，获取物流配送公司
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="goodsTypes"></param>
        /// <returns></returns>
        public static Guid GetHostingFilialeIdByWarehouseIdGoodsTypes(Guid warehouseId,Guid saleFilialeId, IEnumerable<Int32> goodsTypes)
        {
            var result= _erpProxy.GetHostingFilialeIdByWarehouseId(warehouseId, goodsTypes);
            return result != null && result.IsSuccess ? result.Data : Guid.Empty;
        }

        /// <summary>
        /// 按公司、主商品ID列表，获取主商品库存数：库存数+待上数-待下数
        /// </summary>
        /// <param name="filialeId">公司</param>
        /// <param name="goodsIds">主商品ID列表</param>
        /// <returns></returns>
        public static Dictionary<Guid, int> GetGoodsStockQuantiyByFilialeIdGoodsIds(Guid filialeId, IEnumerable<Guid> goodsIds)
        {
            return _erpProxy.GetGoodsStockQuantiyByHostingFilialeIdGoodsIds(filialeId, goodsIds);
        }

        public static Dictionary<Guid, int> GoodsEffitiveStockBySaleFilialeId(Guid warehouseId, IEnumerable<Byte> storageTypes,IDictionary<Guid,Guid> realGoods,Guid saleFilialeId)
        {
            return _erpProxy.GetGoodsStockQuantityBySaleFilialeId(warehouseId, storageTypes, realGoods, saleFilialeId);
        }
    }
}

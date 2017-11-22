using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model.Goods;
using KeedeGroup.GoodsManageSystem.Public.Enum;
using KeedeGroup.GoodsManageSystem.Public.Model.Basic;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using KeedeGroup.GoodsManageSystem.Public.Model.RequestModel;
using KeedeGroup.GoodsManageSystem.Public.Model.Table;
using FieldInfo = ERP.Model.FieldInfo;
using GoodsExtendInfo = ERP.Model.Goods.GoodsExtendInfo;
using GoodsFieldInfo = ERP.Model.Goods.GoodsFieldInfo;
using GoodsGroupInfo = ERP.Model.Goods.GoodsGroupInfo;
using GoodsInfo = ERP.Model.Goods.GoodsInfo;

namespace ERP.SAL.Goods
{
    public partial class GoodsCenterSao
    {
        #region -- 模型转换

        ChildGoodsInfo ConvertToMyChildGoodsInfo(RealGoodsInfo realGoodsInfo)
        {
            return new ChildGoodsInfo
            {
                Barcode = realGoodsInfo.Barcode,
                GoodsId = realGoodsInfo.GoodsID,
                IsActive = !realGoodsInfo.IsDelete,
                IsScarcity = realGoodsInfo.IsScarcity,
                RealGoodsId = realGoodsInfo.RealGoodsID,
                Specification = realGoodsInfo.Specification,
                OrderIndex = realGoodsInfo.OrderIndex,
                Disable = realGoodsInfo.Disable
            };
        }

        IEnumerable<ChildGoodsInfo> ConvertToMyChildGoodsList(IEnumerable<RealGoodsInfo> realGoodsList)
        {
            return realGoodsList.Select(ConvertToMyChildGoodsInfo);
        }

        private GoodsRequestModel ConverToGoodsRequestModel(GoodsInfo info, Dictionary<Guid, List<Guid>> fields)
        {
            var goodsRequestModel = new GoodsRequestModel
            {
                ClassId = info.ClassId,
                GoodsInfo = new KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsInfo
                {
                    ApprovalNO = info.ApprovalNO,
                    BrandID = info.BrandId,
                    GoodsCode = info.GoodsCode,
                    GoodsID = info.GoodsId,
                    GoodsType = info.GoodsType,
                    IsScarcity = info.IsStockScarcity,
                    MarketPrice = info.MarketPrice,
                    PurchaseName = info.GoodsName,
                    PurchaseState = info.IsOnShelf ? (int)PurchaseState.Sale : (int)PurchaseState.UnSale,
                    SaleStockType = info.SaleStockType,
                    StockInformation = info.StockStatus,
                    Units = info.Units,
                    Weight = info.Weight,
                    BarCode = info.BarCode,
                    PurchaseNameFirstLetter = info.PurchaseNameFirstLetter,
                    SupplierGoodsCode = info.SupplierGoodsCode,
                    Specification = info.Specification,
                    ShelfLife = info.ShelfLife,
                    IsImportedGoods = info.IsImportedGoods,
                    IsLuxury = info.IsLuxury,
                    IsBannedPurchase = info.IsBannedPurchase,
                    IsBannedSale = info.IsBannedSale,
                    ImgPath = info.ImgPath,
                    GoodsAuditState = info.GoodsAuditState,
                    GoodsAuditStateMemo = info.GoodsAuditStateMemo
                },
                GoodsExtendInfo = new KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsExtendInfo
                {
                    ApprovalTime = info.ExpandInfo.ApprovalTime,
                    GoodsID = info.GoodsId,
                    Height = info.ExpandInfo.Height,
                    Length = info.ExpandInfo.Length,
                    PackCount = info.ExpandInfo.PackCount,
                    Width = info.ExpandInfo.Width,
                    ReferencePrice = info.ExpandInfo.ReferencePrice,
                    ImplicitCost = info.ExpandInfo.ImplicitCost,
                    YearDiscount = info.ExpandInfo.YearDiscount,
                    IsStatisticalPerformance = info.ExpandInfo.IsStatisticalPerformance
                },
                GoodsFrameSizeInfo = ConvertToGoodsFrameSizeInfo(info.FrameGoodsInfo),
                GoodsMedicineInfo = ConvertToGoodsMedicineInfo(info.GoodsMedicineInfo),
                GoodsGroupList = info.SaleGroupList.Select(ent => ent.GroupId).ToList(),
                FieldList = fields,
                GoodsQualificationDetailInfos = ConvertToGoodsQualificationDetailInfo(info.GoodsQualificationDetailInfos)
            };
            if (info.DictGoodsPurchase != null && info.DictGoodsPurchase.Count > 0)
            {
                goodsRequestModel.DictGoodsPurchase = new Dictionary<Guid, List<EntityPurchaseType>>();
                foreach (var keyValue in info.DictGoodsPurchase)
                {
                    var entityPurchaseTypeList = new List<EntityPurchaseType>();
                    foreach (var ept in keyValue.Value)
                    {
                        if (ept == Enum.EntityPurchaseType.Direct)
                            entityPurchaseTypeList.Add(EntityPurchaseType.Direct);
                        if (ept == Enum.EntityPurchaseType.Join)
                            entityPurchaseTypeList.Add(EntityPurchaseType.Join);
                        if (ept == Enum.EntityPurchaseType.Alliance)
                            entityPurchaseTypeList.Add(EntityPurchaseType.Alliance);
                    }
                    goodsRequestModel.DictGoodsPurchase.Add(keyValue.Key, entityPurchaseTypeList);
                }
            }
            return goodsRequestModel;
        }

        private GoodsEditRequestModel ConverToGoodsEditRequestModel(GoodsInfo info)
        {
            var goodsEditRequestModel = new GoodsEditRequestModel
            {
                ClassId = info.ClassId,
                GoodsInfo = new KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsInfo
                {
                    ApprovalNO = info.ApprovalNO,
                    BrandID = info.BrandId,
                    GoodsCode = info.GoodsCode,
                    GoodsID = info.GoodsId,
                    GoodsType = info.GoodsType,
                    IsScarcity = info.IsStockScarcity,
                    MarketPrice = info.MarketPrice,
                    PurchaseName = info.GoodsName,
                    PurchaseState = info.IsOnShelf ? (int)PurchaseState.Sale : (int)PurchaseState.UnSale,
                    SaleStockType = info.SaleStockType,
                    StockInformation = info.StockStatus,
                    Units = info.Units,
                    Weight = info.Weight,
                    BarCode = info.BarCode,
                    PurchaseNameFirstLetter = info.PurchaseNameFirstLetter,
                    SupplierGoodsCode = info.SupplierGoodsCode,
                    Specification = info.Specification,
                    ShelfLife = info.ShelfLife,
                    IsImportedGoods = info.IsImportedGoods,
                    IsLuxury = info.IsLuxury,
                    IsBannedPurchase = info.IsBannedPurchase,
                    IsBannedSale = info.IsBannedSale,
                    ImgPath = info.ImgPath,
                    GoodsAuditState = info.GoodsAuditState,
                    GoodsAuditStateMemo = info.GoodsAuditStateMemo
                },
                GoodsExtendInfo = new KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsExtendInfo
                {
                    ApprovalTime = info.ExpandInfo.ApprovalTime,
                    GoodsID = info.GoodsId,
                    Height = info.ExpandInfo.Height,
                    Length = info.ExpandInfo.Length,
                    PackCount = info.ExpandInfo.PackCount,
                    Width = info.ExpandInfo.Width,
                    ReferencePrice = info.ExpandInfo.ReferencePrice,
                    JoinPrice = info.ExpandInfo.JoinPrice,
                    ImplicitCost = info.ExpandInfo.ImplicitCost,
                    YearDiscount = info.ExpandInfo.YearDiscount,
                    IsStatisticalPerformance = info.ExpandInfo.IsStatisticalPerformance
                },
                GoodsMedicineInfo = ConvertToGoodsMedicineInfo(info.GoodsMedicineInfo),
                GoodsFrameSizeInfo = ConvertToGoodsFrameSizeInfo(info.FrameGoodsInfo),
                GoodsGroupList = info.SaleGroupList.Select(ent => ent.GroupId).ToList(),
                GoodsQualificationDetailInfos = ConvertToGoodsQualificationDetailInfo(info.GoodsQualificationDetailInfos)
            };

            if (info.DictGoodsPurchase != null && info.DictGoodsPurchase.Count > 0)
            {
                goodsEditRequestModel.DictGoodsPurchase = new Dictionary<Guid, List<EntityPurchaseType>>();
                foreach (var keyValue in info.DictGoodsPurchase)
                {
                    var entityPurchaseTypeList = new List<EntityPurchaseType>();
                    foreach (var ept in keyValue.Value)
                    {
                        if (ept == Enum.EntityPurchaseType.Direct)
                            entityPurchaseTypeList.Add(EntityPurchaseType.Direct);
                        if (ept == Enum.EntityPurchaseType.Join)
                            entityPurchaseTypeList.Add(EntityPurchaseType.Join);
                        if (ept == Enum.EntityPurchaseType.Alliance)
                            entityPurchaseTypeList.Add(EntityPurchaseType.Alliance);
                    }
                    goodsEditRequestModel.DictGoodsPurchase.Add(keyValue.Key, entityPurchaseTypeList);
                }
            }
            return goodsEditRequestModel;
        }

        GoodsFrameSizeInfo ConvertToGoodsFrameSizeInfo(FrameGoodsInfo frameGoodsInfo)
        {
            var frameInfo = new GoodsFrameSizeInfo
            {
                Besiclometer = frameGoodsInfo.Besiclometer,
                EyeSize = frameGoodsInfo.EyeSize,
                FrameWithinWidth = frameGoodsInfo.FrameWithinWidth,
                GoodsID = frameGoodsInfo.GoodsId,
                NoseWidth = frameGoodsInfo.NoseWidth,
                OpticalVerticalHeight = frameGoodsInfo.OpticalVerticalHeight,
                TempleLength = frameGoodsInfo.TempleLength
            };
            return frameInfo;
        }

        KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsMedicineInfo ConvertToGoodsMedicineInfo(ERP.Model.Goods.GoodsMedicineInfo goodsMedicineInfo)
        {
            if (goodsMedicineInfo == null)
                return null;
            var medicineInfo = new KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsMedicineInfo
            {
                ChemistryName = goodsMedicineInfo.ChemistryName,
                ChemistryNameFirstLetter = goodsMedicineInfo.ChemistryNameFirstLetter,
                MedicineQualityType = goodsMedicineInfo.MedicineQualityType,
                MedicineSaleKindType = goodsMedicineInfo.MedicineSaleKindType,
                MedicineWholesalePrice = goodsMedicineInfo.MedicineWholesalePrice,
                MedicineTaxRateType = goodsMedicineInfo.MedicineTaxRateType,
                QualityStandardDescription = goodsMedicineInfo.QualityStandardDescription,
                MedicineDosageFormType = goodsMedicineInfo.MedicineDosageFormType,
                MedicineStorageConditionType = goodsMedicineInfo.MedicineStorageConditionType,
                MedicineCuringKindType = goodsMedicineInfo.MedicineCuringKindType,
                MedicineCuringCycleType = goodsMedicineInfo.MedicineCuringCycleType,
                MedicineStoreCounterType = goodsMedicineInfo.MedicineStoreCounterType,
                MedicineLibraryManageType = goodsMedicineInfo.MedicineLibraryManageType
            };
            return medicineInfo;
        }

        List<KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsQualificationDetailInfo> ConvertToGoodsQualificationDetailInfo(List<ERP.Model.Goods.GoodsQualificationDetailInfo> goodsQualificationDetailInfo)
        {
            var result = new List<KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsQualificationDetailInfo>();
            if (goodsQualificationDetailInfo == null)
                return result;
            foreach (var info in goodsQualificationDetailInfo)
            {
                result.Add(new KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsQualificationDetailInfo
                {
                    GoodsID = info.GoodsID,
                    GoodsQualificationType = info.GoodsQualificationType,
                    NameOrNo = info.NameOrNo,
                    Path = info.Path,
                    ExtensionName = info.ExtensionName,
                    OverdueDate = info.OverdueDate,
                    LastOperationTime = info.LastOperationTime
                });
            }
            return result;
        }

        GoodsInfo ConvertToMyGoodsInfo(GoodsBaseInfo goodsBaseInfo)
        {
            var goodsInfo = new GoodsInfo
            {
                GoodsId = goodsBaseInfo.GoodsID,
                GoodsCode = goodsBaseInfo.GoodsCode,
                IsStockScarcity = goodsBaseInfo.IsScarcity,
                GoodsType = goodsBaseInfo.GoodsType,
                GoodsName = goodsBaseInfo.PurchaseName,
                MarketPrice = goodsBaseInfo.MarketPrice ?? 0,
                Units = goodsBaseInfo.Units,
                IsOnShelf = (PurchaseState)goodsBaseInfo.PurchaseState == PurchaseState.Sale,
                ClassId = goodsBaseInfo.ClassId,
                BrandId = goodsBaseInfo.BrandId,
                SaleStockType = goodsBaseInfo.SaleStockType,
                ApprovalNO = goodsBaseInfo.ApprovalNo,
                Weight = goodsBaseInfo.Weight,
                OldGoodsCode = goodsBaseInfo.OldGoodsCode,
                ExpandInfo = new GoodsExtendInfo
                {
                    ReferencePrice = goodsBaseInfo.ReferencePrice,
                    JoinPrice = goodsBaseInfo.JoinPrice,
                    ImplicitCost = goodsBaseInfo.ImplicitCost,
                    YearDiscount = goodsBaseInfo.YearDiscount,
                    IsStatisticalPerformance = goodsBaseInfo.IsStatisticalPerformance
                }
            };
            return goodsInfo;
        }

        GoodsInfo ConvertToMyGoodsInfo(GoodsEditRequestModel goodsRequestInfo)
        {
            var goodsInfo = new GoodsInfo
            {
                GoodsId = goodsRequestInfo.GoodsInfo.GoodsID,
                GoodsCode = goodsRequestInfo.GoodsInfo.GoodsCode,
                IsStockScarcity = goodsRequestInfo.GoodsInfo.IsScarcity,
                GoodsType = goodsRequestInfo.GoodsInfo.GoodsType,
                GoodsName = goodsRequestInfo.GoodsInfo.PurchaseName,
                MarketPrice = goodsRequestInfo.GoodsInfo.MarketPrice,
                Units = goodsRequestInfo.GoodsInfo.Units,
                IsOnShelf = (PurchaseState)goodsRequestInfo.GoodsInfo.PurchaseState == PurchaseState.Sale,
                SaleStockType = goodsRequestInfo.GoodsInfo.SaleStockType,
                Weight = goodsRequestInfo.GoodsInfo.Weight,
                BrandId = goodsRequestInfo.GoodsInfo.BrandID,
                ApprovalNO = goodsRequestInfo.GoodsInfo.ApprovalNO,
                ClassId = goodsRequestInfo.ClassId,
                StockStatus = goodsRequestInfo.GoodsInfo.StockInformation,
                BarCode = goodsRequestInfo.GoodsInfo.BarCode,
                PurchaseNameFirstLetter = goodsRequestInfo.GoodsInfo.PurchaseNameFirstLetter,
                SupplierGoodsCode = goodsRequestInfo.GoodsInfo.SupplierGoodsCode,
                Specification = goodsRequestInfo.GoodsInfo.Specification,
                ShelfLife = goodsRequestInfo.GoodsInfo.ShelfLife,
                IsImportedGoods = goodsRequestInfo.GoodsInfo.IsImportedGoods,
                IsLuxury = goodsRequestInfo.GoodsInfo.IsLuxury,
                IsBannedPurchase = goodsRequestInfo.GoodsInfo.IsBannedPurchase,
                IsBannedSale = goodsRequestInfo.GoodsInfo.IsBannedSale,
                ImgPath = goodsRequestInfo.GoodsInfo.ImgPath,
                GoodsAuditState = goodsRequestInfo.GoodsInfo.GoodsAuditState,
                GoodsAuditStateMemo = goodsRequestInfo.GoodsInfo.GoodsAuditStateMemo
            };
            if (goodsRequestInfo.GoodsExtendInfo != null)
                goodsInfo.ExpandInfo = ConvertToMyGoodsExpandInfo(goodsRequestInfo.GoodsExtendInfo);
            if (goodsRequestInfo.GoodsFrameSizeInfo != null)
                goodsInfo.FrameGoodsInfo = ConvertToMyFrameGoodsInfo(goodsRequestInfo.GoodsFrameSizeInfo);
            if (goodsRequestInfo.GoodsMedicineInfo != null)
                goodsInfo.GoodsMedicineInfo = ConvertToMyGoodsMedicineInfo(goodsRequestInfo.GoodsMedicineInfo);
            if (goodsRequestInfo.GoodsGroupList != null)
            {
                goodsInfo.SaleGroupList = new List<GoodsGroupInfo>();
                foreach (var groupId in goodsRequestInfo.GoodsGroupList)
                {
                    goodsInfo.SaleGroupList.Add(new GoodsGroupInfo { GroupId = groupId });
                }
            }
            goodsInfo.GoodsQualificationDetailInfos = ConvertToMyGoodsQualificationDetailInfo(goodsRequestInfo.GoodsQualificationDetailInfos);

            if (goodsRequestInfo.DictGoodsPurchase != null && goodsRequestInfo.DictGoodsPurchase.Count > 0)
            {
                goodsInfo.DictGoodsPurchase = new Dictionary<Guid, List<Enum.EntityPurchaseType>>();
                foreach (var keyValue in goodsRequestInfo.DictGoodsPurchase)
                {
                    var entityPurchaseTypeList = new List<Enum.EntityPurchaseType>();
                    foreach (var ept in keyValue.Value)
                    {
                        if (ept == EntityPurchaseType.Direct)
                            entityPurchaseTypeList.Add(Enum.EntityPurchaseType.Direct);
                        if (ept == EntityPurchaseType.Join)
                            entityPurchaseTypeList.Add(Enum.EntityPurchaseType.Join);
                        if (ept == EntityPurchaseType.Alliance)
                            entityPurchaseTypeList.Add(Enum.EntityPurchaseType.Alliance);
                    }
                    goodsInfo.DictGoodsPurchase.Add(keyValue.Key, entityPurchaseTypeList);
                }
            }

            return goodsInfo;
        }

        GoodsExtendInfo ConvertToMyGoodsExpandInfo(KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsExtendInfo goodsExtendInfo)
        {
            return new GoodsExtendInfo
            {
                ApprovalTime = goodsExtendInfo.ApprovalTime,
                GoodsId = goodsExtendInfo.GoodsID,
                Height = goodsExtendInfo.Height ?? 0,
                Length = goodsExtendInfo.Length ?? 0,
                PackCount = goodsExtendInfo.PackCount ?? 0,
                ReferencePrice = goodsExtendInfo.ReferencePrice,
                JoinPrice = goodsExtendInfo.JoinPrice,
                ImplicitCost = goodsExtendInfo.ImplicitCost,
                YearDiscount = goodsExtendInfo.YearDiscount,
                Width = goodsExtendInfo.Width ?? 0,
                IsStatisticalPerformance = goodsExtendInfo.IsStatisticalPerformance
            };
        }

        FrameGoodsInfo ConvertToMyFrameGoodsInfo(GoodsFrameSizeInfo goodsFrameSizeInfo)
        {
            return new FrameGoodsInfo
            {
                Besiclometer = goodsFrameSizeInfo.Besiclometer,
                EyeSize = goodsFrameSizeInfo.EyeSize,
                FrameWithinWidth = goodsFrameSizeInfo.FrameWithinWidth,
                GoodsId = goodsFrameSizeInfo.GoodsID,
                NoseWidth = goodsFrameSizeInfo.NoseWidth,
                OpticalVerticalHeight = goodsFrameSizeInfo.OpticalVerticalHeight,
                TempleLength = goodsFrameSizeInfo.TempleLength
            };
        }

        ERP.Model.Goods.GoodsMedicineInfo ConvertToMyGoodsMedicineInfo(KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsMedicineInfo goodsMedicineInfo)
        {
            var medicineInfo = new ERP.Model.Goods.GoodsMedicineInfo
            {
                ChemistryName = goodsMedicineInfo.ChemistryName,
                ChemistryNameFirstLetter = goodsMedicineInfo.ChemistryNameFirstLetter,
                MedicineQualityType = goodsMedicineInfo.MedicineQualityType,
                MedicineSaleKindType = goodsMedicineInfo.MedicineSaleKindType,
                MedicineWholesalePrice = goodsMedicineInfo.MedicineWholesalePrice,
                MedicineTaxRateType = goodsMedicineInfo.MedicineTaxRateType,
                QualityStandardDescription = goodsMedicineInfo.QualityStandardDescription,
                MedicineDosageFormType = goodsMedicineInfo.MedicineDosageFormType,
                MedicineStorageConditionType = goodsMedicineInfo.MedicineStorageConditionType,
                MedicineCuringKindType = goodsMedicineInfo.MedicineCuringKindType,
                MedicineCuringCycleType = goodsMedicineInfo.MedicineCuringCycleType,
                MedicineStoreCounterType = goodsMedicineInfo.MedicineStoreCounterType,
                MedicineLibraryManageType = goodsMedicineInfo.MedicineLibraryManageType,
                MedicineStorageModeType = goodsMedicineInfo.MedicineStorageModeType
            };
            return medicineInfo;
        }

        List<ERP.Model.Goods.GoodsQualificationDetailInfo> ConvertToMyGoodsQualificationDetailInfo(List<KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsQualificationDetailInfo> goodsQualificationDetailInfo)
        {
            var result = new List<ERP.Model.Goods.GoodsQualificationDetailInfo>();
            if (goodsQualificationDetailInfo == null)
                return result;
            foreach (var info in goodsQualificationDetailInfo)
            {
                result.Add(new ERP.Model.Goods.GoodsQualificationDetailInfo
                {
                    GoodsID = info.GoodsID,
                    GoodsQualificationType = info.GoodsQualificationType,
                    NameOrNo = info.NameOrNo,
                    Path = info.Path,
                    ExtensionName = info.ExtensionName,
                    OverdueDate = info.OverdueDate,
                    LastOperationTime = info.LastOperationTime
                });
            }
            return result;
        }

        #endregion

        #region --> 生成子商品

        /// <summary>
        /// 生成子商品
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="fieldList"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        public bool CreatRealGoods(Guid goodsId, Dictionary<Guid, List<Guid>> fieldList, out string failMessage)
        {
            failMessage = string.Empty;
            var result = GoodsServerClient.CreatRealGoods(goodsId, fieldList);
            if (result != null)
            {
                failMessage = result.ErrorMsg;
                return result.IsSuccess;
            }
            return false;
        }
        #endregion

        #region --> 添加主商品

        /// <summary>
        /// 添加主商品
        /// </summary>
        /// <param name="goodsInfo"></param>
        /// <param name="fields"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        public bool AddGoods(GoodsInfo goodsInfo, Dictionary<Guid, List<Guid>> fields, out string failMessage)
        {
            var requestModel = ConverToGoodsRequestModel(goodsInfo, fields);
            var result = GoodsServerClient.AddGoods(requestModel);
            failMessage = string.Empty;
            if (result == null) failMessage = "GMS连接异常";
            else if (!result.IsSuccess) failMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region --> 修改主商品信息

        /// <summary>
        /// 修改主商品信息
        /// </summary>
        /// <param name="goodsInfo"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        public bool UpdateGoods(GoodsInfo goodsInfo, out string failMessage)
        {
            var requestModel = ConverToGoodsEditRequestModel(goodsInfo);
            var result = GoodsServerClient.UpdateGoods(requestModel);
            failMessage = string.Empty;
            if (result == null) failMessage = "GMS连接异常";
            else if (!result.IsSuccess) failMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary>
        /// 修改主商品审核状态和商品审核备注
        /// </summary>
        /// <param name="goodsId">商品Id</param>
        /// <param name="goodsState">商品审核状态(0:审核未通过;1:待采购经理审核;2:待质管部审核;3:待负责人终审;4:审核通过;)</param>
        /// <param name="goodsStateMemo">商品审核备注</param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        /// zal 2016-04-21
        public bool UpdateGoodsAuditStateAndAuditStateMemo(Guid goodsId, int goodsState, string goodsStateMemo, out string failMessage)
        {
            var result = GoodsServerClient.AuditGoods(goodsId, goodsState, goodsStateMemo);
            failMessage = string.Empty;
            if (result == null) failMessage = "GMS连接异常";
            else if (!result.IsSuccess) failMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }


        /// <summary>
        /// 批量修改主商品审核状态和商品审核备注
        /// </summary>
        /// <param name="goodsId">商品Id集合</param>
        /// <param name="goodsState">商品审核状态(0:审核未通过;1:待采购经理审核;2:待质管部审核;3:待负责人终审;4:审核通过;)</param>
        /// <param name="goodsStateMemo">商品审核备注</param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        /// ww 2016-06-29
        public bool PlUpdateGoodsAuditStateAndAuditStateMemo(List<Guid> goodsId, int goodsState, string goodsStateMemo, out string failMessage)
        {
            var result = GoodsServerClient.AuditGoodsByGoodsIDs(goodsId, goodsState, goodsStateMemo);
            failMessage = string.Empty;
            if (result == null) failMessage = "GMS连接异常";
            else if (!result.IsSuccess) failMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region --> 删除主商品信息

        /// <summary>
        /// 删除主商品信息
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="personId"> </param>
        /// <param name="errorMessage"></param>
        /// <param name="operatorName"> </param>
        /// <returns></returns>
        public bool DeleteGoods(List<Guid> goodsIds, string operatorName, Guid personId, out string errorMessage)
        {
            var operationModel = new OperationModel { Operator = operatorName, PersonId = personId };
            var result = GoodsServerClient.DeleteGoods(goodsIds, operationModel);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region --> 获取主商品信息
        /// <summary>
        /// 获取主商品信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public GoodsInfo GetGoodsBaseInfoById(Guid goodsId)
        {
            var result = GoodsServerClient.GetGoodsBaseInfo(goodsId);
            if (result != null && result.IsSuccess)
            {
                return result.Data == null ? null : ConvertToMyGoodsInfo(result.Data);
            }
            return null;
        }
        #endregion

        #region --> 根据子商品或无属性商品，获取主商品信息
        /// <summary>
        /// 根据子商品或无属性商品，获取主商品信息
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <returns></returns>
        public GoodsInfo GetGoodsBaseInfoByRealGoodsId(Guid realGoodsId)
        {
            var result = GoodsServerClient.GetGoodsBaseInfoByRealGoodsId(realGoodsId);
            if (result != null && result.IsSuccess)
            {
                return result.Data == null ? null : ConvertToMyGoodsInfo(result.Data);
            }
            return null;
        }
        #endregion

        /// <summary>
        /// Use New ServiceClient
        /// </summary>
        /// <param name="realGoodsIdList"></param>
        /// <returns></returns>
        public Dictionary<Guid, GoodsInfo> GetGoodsBaseListByGoodsIdOrRealGoodsIdList(List<Guid> realGoodsIdList)
        {
            var dic = new Dictionary<Guid, GoodsInfo>();
            var result = GoodsServerClient.GetGoodsBaseListByRealGoodsIdList(realGoodsIdList);
            if (result != null && result.IsSuccess)
            {
                var dicGoods = result.Data ?? new Dictionary<Guid, GoodsBaseInfo>();
                foreach (var keyValueKey in dicGoods)
                {
                    if (keyValueKey.Value != null)
                        dic.Add(keyValueKey.Key, ConvertToMyGoodsInfo(keyValueKey.Value));
                }
            }
            return dic;
        }

        /// <summary>
        /// Use New ServiceClient
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="goodsIdList"></param>
        /// <returns></returns>
        /// zal 2017-06-28
        public List<GoodsSeriesModel> GetGoodsSeriesList(Guid saleFilialeId, List<Guid> goodsIdList)
        {
            var result = GoodsServerClient.GetGoodsSeriesList(saleFilialeId, goodsIdList);
            if (result != null && result.IsSuccess)
            {
                return result.Data ?? new List<GoodsSeriesModel>();
            }
            return new List<GoodsSeriesModel>();
        }

        #region --> 根据主商品名称或主商品Code获取主商品
        /// <summary>
        /// 根据商品名称或商品Code获取商品
        /// </summary>
        /// <param name="goodsCode"></param>
        /// <returns></returns>
        public GoodsInfo GetGoodsBaseInfoByCode(string goodsCode)
        {
            var result = GoodsServerClient.GetGoodsBaseInfoByGoodsCode(goodsCode);
            if (result != null && result.IsSuccess)
            {
                return result.Data == null ? null : ConvertToMyGoodsInfo(result.Data);
            }
            return null;
        }
        #endregion

        #region --> 获取主商品详细信息
        /// <summary>
        /// 获取主商品详细信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public GoodsInfo GetGoodsDetailById(Guid goodsId)
        {
            var result = GoodsServerClient.GetGoodsBaseInfo(goodsId);
            if (result != null && result.IsSuccess)
            {
                return result.Data == null ? null : ConvertToMyGoodsInfo(result.Data);
            }
            return null;
        }
        #endregion

        #region --> 搜索商品信息(分页)

        /// <summary>
        /// 搜索商品信息(分页)
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <param name="hasInformation"></param>
        /// <param name="saleStockType"></param>
        /// <param name="goodsAuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public IList<GoodsInfo> GetGoodsListToPage(Guid classId, string goodsNameOrCode, bool? hasInformation, int? saleStockType, int? goodsAuditState, int pageIndex, int pageSize, out int totalCount)
        {
            IList<GoodsInfo> list = new List<GoodsInfo>();
            totalCount = 0;
            var searchModel = new GoodsSearchModel
            {
                ClassId = classId,
                GoodsNameOrCode = goodsNameOrCode,
                HasInformation = hasInformation,
                SaleStockType = saleStockType,
                GoodsAuditState = goodsAuditState,
                Page = pageIndex,
                PageSize = pageSize
            };
            var result = GoodsServerClient.GetGoodsListByPage(searchModel);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<GoodsGridModel>();
                foreach (var item in items)
                {
                    var goodsInfo = new GoodsInfo
                    {
                        GoodsId = item.GoodsId,
                        GoodsCode = item.GoodsCode,
                        IsStockScarcity = item.IsScarcity,
                        GoodsType = item.GoodsType,
                        GoodsName = item.PurchaseName,
                        MarketPrice = item.MarketPrice,
                        IsOnShelf = (PurchaseState)item.PurchaseState == PurchaseState.Sale,
                        HasRealGoods = item.HasRealGoods,
                        SaleStockType = item.SaleStockType,
                        SaleStockState = item.SaleStockState,
                        GoodsAuditState = item.GoodsAuditState,
                        ImgPath = item.ImgPath,
                        GoodsAuditStateMemo = item.GoodsAuditStateMemo,
                        ExpandInfo = new GoodsExtendInfo
                        {
                            ReferencePrice = item.ReferencePrice
                        },
                        ClassInfo = new GoodsClassInfo
                        {
                            ClassName = item.ClassName
                        }
                    };

                    list.Add(goodsInfo);
                }
                totalCount = result.Total;
            }
            return list;
        }
        #endregion

        #region --> 根据主商品名称或主商品编号获取子商品ID集合
        /// <summary>根据主商品名称或主商品编号获取子商品ID集合
        /// </summary>
        /// <param name="goodsNameOrCode"></param>
        /// <returns></returns>
        public IList<Guid> GetRealGoodsIdListByGoodsNameOrCode(string goodsNameOrCode)
        {
            var result = GoodsServerClient.GetRealGoodsListByGoodsNameOrCode(goodsNameOrCode);
            if (result != null && result.IsSuccess)
            {
                return result.Data ?? new List<Guid>();
            }
            return new List<Guid>();
        }
        #endregion

        #region --> 设置主商品是否缺货

        /// <summary>
        /// 设置商品是否缺货
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="isScarcity"></param>
        /// <param name="operatorName"> </param>
        /// <param name="personId"> </param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns></returns>
        public bool SetGoodsIsScarcity(Guid goodsId, bool isScarcity, string operatorName, Guid personId, out string errorMsg)
        {
            errorMsg = string.Empty;
            var operationModel = new OperationModel { Operator = operatorName, PersonId = personId };
            var result = GoodsServerClient.SetGoodsIsScarcity(goodsId, isScarcity, operationModel);
            if (result != null)
            {
                if (!result.IsSuccess)
                {
                    errorMsg = result.ErrorMsg;
                }
                return result.IsSuccess;
            }
            return false;
        }
        #endregion

        #region --> 设置子商品是否缺货

        /// <summary>
        /// 设置子商品是否缺货
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <param name="isScarcity"></param>
        /// <param name="operatorName"> </param>
        /// <param name="personId"> </param>
        /// <returns></returns>
        public bool SetRealGoodsIsScarcity(Guid realGoodsId, bool isScarcity, string operatorName, Guid personId)
        {
            var operationModel = new OperationModel { Operator = operatorName, PersonId = personId };
            var result = GoodsServerClient.SetRealGoodsIsScarcity(realGoodsId, isScarcity, operationModel);
            return result != null && result.IsSuccess;
        }
        #endregion

        #region --> 设置主商品采购状态

        /// <summary>
        /// 设置主商品采购状态
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="isOnShelf"></param>
        /// <param name="personId"> </param>
        /// <param name="errorMessage"></param>
        /// <param name="operatorName"> </param>
        /// <returns></returns>
        public bool SetPurchaseState(Guid goodsId, bool isOnShelf, string operatorName, Guid personId, out string errorMessage)
        {
            var operationModel = new OperationModel { Operator = operatorName, PersonId = personId };
            var purchaseState = isOnShelf ? PurchaseState.Sale : PurchaseState.UnSale;
            var result = GoodsServerClient.SetPurchaseState(goodsId, purchaseState, operationModel);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region --> 获取主商品简单键值对集合
        /// <summary>
        /// 获取主商品简单键值对集合
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetGoodsSelectList(string goodsNameOrCode)
        {
            var dic = new Dictionary<string, string>();
            var result = GoodsServerClient.GetGoodsItemList(goodsNameOrCode);
            if (result != null && result.IsSuccess)
            {
                var list = result.Data ?? new List<GoodsItemModel>();
                foreach (var goodsItemModel in list)
                {
                    dic.Add(goodsItemModel.GoodsId.ToString(), goodsItemModel.GoodsName);
                }
            }
            return dic;
        }
        #endregion

        #region --> 指定商品类型，获取主商品简单键值对集合
        /// <summary>
        /// 指定商品类型，获取主商品简单键值对集合
        /// </summary>
        /// <param name="goodsNameOrCode"></param>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        public IDictionary<string, string> GetGoodsSelectList(string goodsNameOrCode, int goodsType)
        {
            var dic = new Dictionary<string, string>();
            var result = GoodsServerClient.GetGoodsItemListByGoodsType(goodsNameOrCode, goodsType);
            if (result != null && result.IsSuccess)
            {
                var list = result.Data ?? new List<GoodsItemModel>();
                foreach (var goodsItemModel in list)
                {
                    dic.Add(goodsItemModel.GoodsId.ToString(), goodsItemModel.GoodsName);
                }
            }
            return dic;
        }
        #endregion

        #region --> 根据主商品ID和属性ID获取子商品
        /// <summary>
        /// 根据主商品ID和属性ID获取子商品
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="fieldList"></param>
        /// <returns></returns>
        public IList<ChildGoodsInfo> GetRealGoodsListByGoodsIdAndFields(Guid goodsId, Dictionary<Guid, List<Guid>> fieldList)
        {
            List<List<Guid>> newFieldList = fieldList.Select(keyValuePair => keyValuePair.Value).ToList();
            var result = GoodsServerClient.GetRealGoodsListByGoodsIdAndFields(goodsId, newFieldList);
            if (result != null && result.IsSuccess)
            {
                var list = result.Data ?? new List<RealGoodsInfo>();
                return ConvertToMyChildGoodsList(list).ToList();
            }
            return new List<ChildGoodsInfo>();
        }
        #endregion

        #region --> 根据商品分类ID和采购状态查询主商品

        /// <summary>
        /// 根据商品分类ID和采购状态查询主商品
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="goodsNameOrCode"> </param>
        /// <returns></returns>
        public IList<GoodsInfo> GetGoodsBaseInfoListByClassId(Guid classId, string goodsNameOrCode)
        {
            var result = GoodsServerClient.GetGoodsListByClassId(classId, goodsNameOrCode);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsInfo>();
                return items.Select(info => new GoodsInfo
                {
                    GoodsId = info.GoodsID,
                    GoodsName = info.PurchaseName,
                    GoodsCode = info.GoodsCode,
                    BrandId = info.BrandID,
                    Units = info.Units,
                    MarketPrice = info.MarketPrice,
                    IsStockScarcity = info.IsScarcity,
                    GoodsType = info.GoodsType,
                    IsOnShelf = (PurchaseState)info.PurchaseState == PurchaseState.Sale,
                    SaleStockType = info.SaleStockType,
                    ApprovalNO = info.ApprovalNO,
                    Weight = info.Weight
                }).ToList();
            }
            return new List<GoodsInfo>();
        }
        #endregion

        /// <summary> 根据商品分类ID和采购状态查询主商品GoodsId/GoodsName/GoodsCode
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="goodsNameOrCode"> </param>
        /// <returns></returns>
        public IList<GoodsInfo> GetGoodsInfoListSimpleByClassId(Guid classId, string goodsNameOrCode)
        {
            var result = GoodsServerClient.GetGoodsItemListByClassId(classId, goodsNameOrCode);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<GoodsItemModel>();
                return items.Select(w => new GoodsInfo { GoodsId = w.GoodsId, GoodsName = w.GoodsName, GoodsCode = w.GoodsCode }).ToList();
            }
            return new List<GoodsInfo>();
        }

        #region --> 根据主商品ID获取其所有子商品信息
        /// <summary>根据主商品ID获取所有子商品
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        public IEnumerable<ChildGoodsInfo> GetRealGoodsListByGoodsId(List<Guid> goodsIds)
        {
            var result = GoodsServerClient.GetRealGoodsListByGoodsIds(goodsIds);
            if (result != null && result.IsSuccess)
            {
                var realGoodsList = result.Data ?? new List<RealGoodsInfo>();
                foreach (var item in realGoodsList)
                {
                    yield return ConvertToMyChildGoodsInfo(item);
                }
            }
        }
        #endregion

        #region --> 获取子商品的基本信息
        /// <summary>
        /// 
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <returns></returns>
        public ChildGoodsInfo GetChildGoodsInfo(Guid realGoodsId)
        {
            var result = GoodsServerClient.GetRealGoodsInfoByRealGoodsId(realGoodsId);
            if (result != null && result.IsSuccess)
            {
                return ConvertToMyChildGoodsInfo(result.Data);
            }
            return null;
        }
        #endregion

        #region --> 修改子商品信息

        public bool UpdateChildGoodsInfo(ChildGoodsInfo childGoodsInfo, string operatorName, Guid personId, out string errorMessage)
        {

            var realGoodsInfo = new RealGoodsInfo
            {
                Barcode = childGoodsInfo.Barcode,
                GoodsID = childGoodsInfo.GoodsId,
                IsDelete = !childGoodsInfo.IsActive,
                IsScarcity = childGoodsInfo.IsScarcity,
                RealGoodsID = childGoodsInfo.RealGoodsId,
                OrderIndex = childGoodsInfo.OrderIndex,
                Disable = childGoodsInfo.Disable
            };
            var operationModel = new OperationModel { Operator = operatorName, PersonId = personId };
            var result = GoodsServerClient.UpdateRealGoods(realGoodsInfo, operationModel);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        #endregion

        #region -->还原商品添加时的数据（修改之前用）
        public GoodsInfo GetGoodsInfoBeforeUpdate(Guid goodsId)
        {
            var result = GoodsServerClient.GetGoodsEditRequestModel(goodsId);
            if (result != null && result.IsSuccess)
            {
                GoodsEditRequestModel info = result.Data;
                if (info != null)
                {
                    return ConvertToMyGoodsInfo(info);
                }
            }
            return null;
        }
        #endregion

        #region --> 根据主商品ID集合获取主商品信息

        /// <summary>根据主商品ID集合获取主商品信息
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        public IList<GoodsInfo> GetGoodsListByGoodsIds(List<Guid> goodsIds)
        {
            var result = GoodsServerClient.GetGoodsBaseListByIds(goodsIds);
            if (result != null && result.IsSuccess)
            {
                return result.Data.Select(ConvertToMyGoodsInfo).ToList();
            }
            return new List<GoodsInfo>();
        }
        #endregion

        /// <summary> 商品转移
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="classId"></param>
        /// <param name="failMessage"></param>
        public bool UpdateGoodsClass(Guid goodsId, Guid classId, out string failMessage)
        {
            var result = GoodsServerClient.UpdateGoodsClass(goodsId, classId);
            failMessage = string.Empty;
            if (result == null) failMessage = "GMS连接异常";
            else if (!result.IsSuccess) failMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        /// <summary> 根据主商品(GoodsId)获取子商品(RealGoodsId)集合
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public IEnumerable<Guid> GetRealGoodsIdsByGoodsId(Guid goodsId)
        {
            IList<Guid> list = new List<Guid>();
            var result = GoodsServerClient.GetRealGoodsIdList(goodsId);
            if (result != null && result.IsSuccess)
            {
                list = result.Data;
            }
            return list;
        }

        public bool GetRealGoodsGoodsEditRequestModel(Guid goodsId, out List<GoodsFieldInfo> fieldList, out List<GoodsFieldInfo> selectedFieldList, out string errorMessage)
        {
            fieldList = new List<GoodsFieldInfo>();
            selectedFieldList = new List<GoodsFieldInfo>();
            errorMessage = string.Empty;
            var result = GoodsServerClient.GetRealGoodsGoodsEditRequestModel(goodsId);
            if (result != null && result.IsSuccess)
            {
                var info = result.Data ?? new RealGoodsResponseModel();
                if (info.FieldList.Count > 0)
                {
                    foreach (FieldDetail fieldDetail in info.FieldList)
                    {
                        var goodsFieldInfo = new GoodsFieldInfo
                        {
                            FieldId = fieldDetail.ParentFieldInfo.FieldId,
                            FieldName = fieldDetail.ParentFieldInfo.FieldName,
                            FieldValue = fieldDetail.ParentFieldInfo.FieldValue,
                            ParentFieldId = fieldDetail.ParentFieldInfo.ParentFieldId,
                            OrderIndex = fieldDetail.ParentFieldInfo.OrderIndex,
                            ChildFields = new List<FieldInfo>()
                        };
                        foreach (KeedeGroup.GoodsManageSystem.Public.Model.Table.FieldInfo fieldInfo in fieldDetail.ChildFieldList)
                        {
                            goodsFieldInfo.ChildFields.Add(new GoodsFieldInfo
                            {
                                FieldId = fieldInfo.FieldId,
                                FieldName = fieldInfo.FieldName,
                                FieldValue = fieldInfo.FieldValue,
                                ParentFieldId = fieldInfo.ParentFieldId,
                                OrderIndex = fieldInfo.OrderIndex
                            });
                        }
                        fieldList.Add(goodsFieldInfo);
                    }
                    selectedFieldList.AddRange(info.SelectFieldList.Select(fieldInfo => new GoodsFieldInfo
                    {
                        FieldId = fieldInfo.FieldId,
                        FieldName = fieldInfo.FieldName,
                        FieldValue = fieldInfo.FieldValue,
                        ParentFieldId = fieldInfo.ParentFieldId,
                        OrderIndex = fieldInfo.OrderIndex
                    }));
                }
            }
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }

        public IEnumerable<ChildGoodsInfo> GetRealGoodsListByPage(Guid goodsId, List<Guid> fieldList, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = 0;
            var list = new List<ChildGoodsInfo>();
            var searchModel = new RealGoodsSearchModel
            {
                GoodsId = goodsId,
                FieldList = fieldList,
                Page = pageIndex,
                PageSize = pageSize
            };
            var result = GoodsServerClient.GetRealGoodsListByPage(searchModel);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<RealGoodsInfo>();
                list.AddRange(items.Select(item => new ChildGoodsInfo
                {
                    GoodsId = item.GoodsID,
                    RealGoodsId = item.RealGoodsID,
                    Specification = item.Specification,
                    Barcode = item.Barcode,
                    IsScarcity = item.IsScarcity,
                    IsActive = item.IsDelete == false,
                    OrderIndex = item.OrderIndex,
                    Disable = item.Disable
                }));
                totalCount = result.Total;
            }
            return list;
        }

        public IEnumerable<GoodsInfo> GetGoodsStockGridList(Guid classId, string goodsNameOrGoodsCode, List<Guid> goodsIdList, int? page, int? pageSize, out int totalCount)
        {
            totalCount = 0;
            var list = new List<GoodsInfo>();
            var flag = page == null || pageSize == null;

            var data = new List<GoodsStockGridModel>();
            if (flag)
            {
                var data1 = GoodsServerClient.GetGoodsStockGridList(classId == Guid.Empty ? (Guid?)null : classId,
                goodsNameOrGoodsCode, goodsIdList);
                if (data1 != null && (data1.IsSuccess || data1.Data.Count > 0))
                {
                    data = data1.Data;
                    totalCount = data.Count;
                }
            }
            else
            {
                var data2 = GoodsServerClient.GetGoodsStockGridListByPage(classId == Guid.Empty ? (Guid?)null : classId, goodsNameOrGoodsCode, goodsIdList, (int)page, (int)pageSize);
                if (data2 != null && data2.IsSuccess)
                {
                    data = data2.Data;
                    totalCount = data2.Total;
                }
            }
            list.AddRange(data.Select(item => new GoodsInfo
            {
                GoodsId = item.GoodsID,
                GoodsName = item.GoodsName,
                GoodsCode = item.GoodsCode,
                IsStockScarcity = item.IsScarcity,
                IsOnShelf = (PurchaseState)item.PurchaseState == PurchaseState.Sale,
                GoodsIds = item.GoodsIds,
            }));
            return list;
        }

        public IEnumerable<ChildGoodsInfo> GetStockDeclareGridList(List<Guid> realgoodsIds)
        {
            var list = new List<ChildGoodsInfo>();
            var result = GoodsServerClient.GetStockDeclareGridList(realgoodsIds);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<StockDeclareGridModel>();
                list.AddRange(items.Select(item => new ChildGoodsInfo
                {
                    GoodsId = item.GoodsID,
                    RealGoodsId = item.RealGoodsID,
                    GoodsName = item.GoodsName,
                    GoodsCode = item.GoodsCode,
                    Specification = item.Specification,
                    Units = item.Units
                }));
            }
            return list;
        }

        #region--> 商品价格查询  ADD 2014-08-08 陈重文

        /// <summary>商品价格查询  ADD 2014-08-08 陈重文
        /// </summary>
        /// <param name="classId">商品分类Id</param>
        /// <param name="brandId">品牌Id</param>
        /// <param name="goodsNameOrCode">商品名称或编号</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns></returns>
        public IList<GoodsPriceSerachInfo> GetGoodsPriceGridByPage(Guid? classId, Guid? brandId, string goodsNameOrCode, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = 0;
            var result = GoodsServerClient.GetGoodsPriceGridByPage(classId, brandId, goodsNameOrCode, pageIndex, pageSize);
            if (result == null || !result.IsSuccess) return null;
            var items = result.Data ?? new List<GoodsPriceGridModel>();
            IList<GoodsPriceSerachInfo> list = items.Select(item => new GoodsPriceSerachInfo
            {
                GoodsID = item.GoodsID,
                GoodsCode = item.GoodsCode,
                GoodsName = item.GoodsName,
                PurchasePrice = 0, //本地数据获取
                JoinPrice = item.JoinPrice,

                ReferencePrice = item.ReferencePrice,
                KeedePrice = item.KeedePrice,
                BaishopPrice = item.BaishopPrice,
                WholesalePrice = item.WholesalePrice,
            }).ToList();
            totalCount = result.Total;
            return list;
        }

        #endregion

        #region [赠品绑定页面使用的一些方法   ADD  2014-08-20  陈重文]

        /// <summary>设置商品的赠品   ADD  2014-08-20  陈重文
        /// </summary>
        /// <param name="goodsGiftList">商品及赠品集合</param>
        /// <returns>true/false</returns>
        public bool SetGoodsGift(Dictionary<Guid, List<Guid>> goodsGiftList)
        {
            var result = GoodsServerClient.SetGoodsGift(goodsGiftList);
            return result.IsSuccess;
        }

        /// <summary>获取商品的赠品   ADD   2014-08-20  陈重文
        /// </summary>
        /// <param name="goodsId">商品Id</param>
        /// <returns>key:赠品商品Id，value:赠品商品名称</returns>
        public Dictionary<Guid, string> GetGoodsGiftList(Guid goodsId)
        {
            var result = GoodsServerClient.GetGoodsGiftList(goodsId);
            if (result.IsSuccess)
            {
                var items = result.Data;
                if (items != null && items.Count > 0)
                {
                    return items.ToDictionary(ent => ent.GoodsId, ent => ent.GoodsName);
                }
            }
            return null;
        }

        /// <summary>获取绑定过该赠品的商品    ADD   2014-08-20  陈重文
        /// </summary>
        /// <param name="giftGoodsId">赠品商品Id</param>
        /// <returns>key:商品Id，value:商品名称</returns>
        public Dictionary<Guid, string> GetGoodsListByGiftID(Guid giftGoodsId)
        {
            var result = GoodsServerClient.GetGoodsListByGiftID(giftGoodsId);
            if (result.IsSuccess)
            {
                var items = result.Data;
                if (items != null && items.Count > 0)
                {
                    return items.ToDictionary(ent => ent.GoodsId, ent => ent.GoodsName);
                }
            }
            return null;
        }

        /// <summary>获取所有绑定过的赠品    ADD   2014-08-20  陈重文
        /// </summary>
        /// <returns>key:赠品商品Id，value:赠品商品名称</returns>
        public Dictionary<Guid, string> GetAllGiftList()
        {
            var result = GoodsServerClient.GetAllGiftList();
            if (result.IsSuccess)
            {
                var items = result.Data;
                if (items != null && items.Count > 0)
                {
                    return items.ToDictionary(ent => ent.GoodsId, ent => ent.GoodsName);
                }
            }
            return null;
        }

        /// <summary>导出商品及赠品   ADD   2014-08-20  陈重文
        /// </summary>
        /// <param name="goodsKindType">商品类型</param>
        /// <param name="brandId">商品品牌Id</param>
        /// <param name="goodsNameOrCode">商品名称或商品编号</param>
        public Dictionary<GoodsBaseModel, List<GoodsBaseModel>> GetGoodsListAndGiftList(int? goodsKindType, Guid? brandId, string goodsNameOrCode)
        {
            var searchModel = new GoodsSearchModel
            {
                BrandID = brandId,
                GoodsType = goodsKindType,
                GoodsNameOrCode = goodsNameOrCode,
                //PurchaseState = PurchaseState.Sale,
            };

            var result = GoodsServerClient.GetGoodsListAndGiftList(searchModel);
            if (result.IsSuccess)
            {

                var items = result.Data;
                if (items != null && items.Count > 0)
                {
                    var goodsBaseDic = new Dictionary<GoodsBaseModel, List<GoodsBaseModel>>();
                    foreach (var item in items)
                    {
                        var goodsBaseModelKey = new GoodsBaseModel
                        {
                            GoodsCode = item.Key.GoodsCode,
                            GoodsId = item.Key.GoodsId,
                            GoodsName = item.Key.GoodsName,
                        };
                        List<GoodsBaseModel> valueList = item.Value.Select(itemValue => new GoodsBaseModel
                        {
                            GoodsCode = itemValue.GoodsCode,
                            GoodsId = itemValue.GoodsId,
                            GoodsName = itemValue.GoodsName,
                        }).ToList();
                        goodsBaseDic.Add(goodsBaseModelKey, valueList);
                    }
                    return goodsBaseDic;
                }
            }
            return null;
        }

        /// <summary>根据商品类型和商品品牌和商品名称及编号搜索商品   ADD  2014-08-20    陈重文
        /// </summary>
        /// <param name="goodsKindType">商品类型</param>
        /// <param name="brandId">商品品牌ID</param>
        /// <param name="goodsNameOrCode">商品名称或编号</param>
        public List<GoodsBaseModel> GetGoodsItemList(int? goodsKindType, Guid? brandId, string goodsNameOrCode)
        {
            var searchModel = new GoodsSearchModel
            {
                BrandID = brandId,
                GoodsType = goodsKindType,
                GoodsNameOrCode = goodsNameOrCode,
                //PurchaseState = PurchaseState.Sale,
            };
            var result = GoodsServerClient.GetGoodsItemList(searchModel);
            if (result.IsSuccess)
            {
                var items = result.Data;
                if (items != null && items.Count > 0)
                {
                    List<GoodsBaseModel> list = items.Select(itemValue => new GoodsBaseModel
                    {
                        GoodsCode = itemValue.GoodsCode,
                        GoodsId = itemValue.GoodsId,
                        GoodsName = itemValue.GoodsName,
                    }).ToList();
                    return list;
                }
            }
            return null;
        }

        #endregion

        /// <summary>根据主商品Id 集合获取商品的价格  ADD 2014-08-22 陈重文
        /// </summary>
        /// <param name="goodsIds">主商品Id集合</param>
        /// <returns></returns>
        public IList<GoodsPriceSerachInfo> GetGoodsPriceByGoodsIds(List<Guid> goodsIds)
        {
            var result = GoodsServerClient.GetGoodsPriceGridByIDs(goodsIds);
            if (result.IsSuccess)
            {
                var items = result.Data;
                if (items != null && items.Count > 0)
                {
                    List<GoodsPriceSerachInfo> list = items.Select(item => new GoodsPriceSerachInfo
                    {
                        GoodsID = item.GoodsID,
                        GoodsCode = item.GoodsCode,
                        GoodsName = item.GoodsName,
                        JoinPrice = item.JoinPrice,
                        ReferencePrice = item.ReferencePrice,
                        KeedePrice = item.KeedePrice,
                        BaishopPrice = item.BaishopPrice
                    }).ToList();
                    return list;
                }
            }
            return null;
        }

        public IList<Guid> GetGoodsIDListByBrandID(Guid brandId)
        {
            var result = GoodsServerClient.GetGoodsIDListByBrandID(brandId);
            if (result.IsSuccess)
            {
                return result.Data;
            }
            return new List<Guid>();
        }

        /// <summary>根据商品分类ID，商品ID集合，商品名称/编号，是否统计绩效获取商品信息（库存周转率查询使用）
        /// </summary>
        /// <param name="classId">商品分类</param>
        /// <param name="goodsIds">商品ID集合</param>
        /// <param name="goodsNameOrCode">商品名称/编号</param>
        /// <param name="isStatisticalPerformance">是否统计绩效</param>
        /// <returns></returns>
        public List<GoodsPerformance> GetGoodsPerformanceList(Guid classId, List<Guid> goodsIds, String goodsNameOrCode, Boolean isStatisticalPerformance)
        {
            var goodsRequest = new GoodsRequest
            {
                ClassId = classId,
                GoodsIds = goodsIds,
                GoodsNameOrCode = goodsNameOrCode,
                IsStatisticalPerformance = isStatisticalPerformance
            };
            var result = GoodsServerClient.GetGoodsPerformanceList(goodsRequest);
            return result.IsSuccess ? result.Data : new List<GoodsPerformance>();
        }
        /// <summary>
        /// 从商品中心获取平台及其对应的第三销售平台信息
        /// </summary>
        /// <returns></returns>
        /// zal 2015-07-16
        public IList<GroupDetailModel> GetGroupDetailList()
        {
            var result = GoodsServerClient.GetGroupDetailList();
            if (result.IsSuccess)
            {
                return result.Data;
            }
            return null;
        }

        /// <summary>
        /// 根据商品Id列表获取商品ID和该商品对应的平台价格信息列表的字典
        /// </summary>
        /// <param name="guidList">商品Id列表</param>
        /// <returns></returns>
        /// zal 2015-07-16
        public Dictionary<Guid, List<GroupGoodsPriceModel>> GetGroupGoodsPriceDictionary(List<Guid> guidList)
        {
            var result = GoodsServerClient.GetGroupGoodsPriceDictionary(guidList);
            if (result.IsSuccess)
            {
                return result.Data;
            }
            return null;
        }

        public string GetProductFilesImage(Guid id, bool isGoodsId)
        {
            var result = GoodsServerClient.GetProductFilesImage(id, isGoodsId);
            if (result != null && result.IsSuccess)
            {
                return result.Data;
            }
            return string.Empty;
        }


        /// <summary>根据商品名称及编号搜索商品   ADD  2016-06-12    文雯
        /// </summary>
        /// <param name="goodsNameOrCode">商品名称或编号</param>
        public List<GoodsBaseModel> GetGoodsItemListByGoodsNameOrCode(string goodsNameOrCode)
        {
            var result = GoodsServerClient.GetGoodsItemList(goodsNameOrCode);
            if (result.IsSuccess)
            {
                var items = result.Data;
                if (items != null && items.Count > 0)
                {
                    List<GoodsBaseModel> list = items.Select(itemValue => new GoodsBaseModel
                    {
                        GoodsCode = itemValue.GoodsCode,
                        GoodsId = itemValue.GoodsId,
                        GoodsName = itemValue.GoodsName,
                    }).ToList();
                    return list;
                }
            }
            return null;
        }

        public Dictionary<Guid, RealGoodsUnitModel> GetDictRealGoodsUnitModel(List<Guid> realGoodsIds)
        {
            var result = GoodsServerClient.GetDictRealGoodsUnitModel(realGoodsIds);
            if (result.IsSuccess)
            {
                var items = result.Data;
                if (items != null && items.Count > 0)
                {
                    return items;
                }
            }
            return null;
        }
    }
}
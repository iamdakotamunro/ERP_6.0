using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model.Goods;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using KeedeGroup.GoodsManageSystem.Public.Model.Table;

namespace ERP.SAL.Goods
{
    /// <summary>商品中心（品牌）
    /// </summary>
    public partial class GoodsCenterSao 
    {
        #region [新增品牌]
        /// <summary>新增品牌
        /// </summary>
        /// <param name="goodsBrandInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool AddBrand(GoodsBrandInfo goodsBrandInfo, out string errorMessage)
        {
            List<BrandQualificationDetailInfo> informationList = null;
            if (goodsBrandInfo.GoodsInformationList != null)
            {
                if (goodsBrandInfo.GoodsInformationList.Any())
                {
                    informationList = goodsBrandInfo.GoodsInformationList.Select(ConvertToBrandInformationInfo).ToList();
                    foreach (var informationInfo in informationList)
                    {
                        informationInfo.BrandID = goodsBrandInfo.BrandId;
                        informationInfo.BrandQualificationName = goodsBrandInfo.Brand;
                    }
                }
            }

            var request = new BrandDetail
            {
                BrandInfo = ConvertToBrandInfo(goodsBrandInfo),
                BrandQualificationList = informationList
            };
            var result = GoodsServerClient.AddBrand(request);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region [修改品牌]
        /// <summary>修改品牌
        /// </summary>
        /// <param name="goodsBrandInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateBrand(GoodsBrandInfo goodsBrandInfo, out string errorMessage)
        {
            List<BrandQualificationDetailInfo> informationList = null;
            if (goodsBrandInfo.GoodsInformationList != null)
            {
                if (goodsBrandInfo.GoodsInformationList.Any())
                {
                    informationList = goodsBrandInfo.GoodsInformationList.Select(ConvertToBrandInformationInfo).ToList();
                    foreach (var informationInfo in informationList)
                    {
                        informationInfo.BrandID = goodsBrandInfo.BrandId;
                        informationInfo.BrandQualificationName = goodsBrandInfo.Brand;
                    }
                }
            }
            var request = new BrandRequestModel
            {
                BrandInfo = ConvertToBrandInfo(goodsBrandInfo),
                BrandQualificationList = informationList
            };
            var result = GoodsServerClient.UpdateBrand(request);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region [删除品牌]
        /// <summary>删除品牌
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool DeleteBrand(Guid brandId, out string errorMessage)
        {
            var result = GoodsServerClient.DeleteBrand(brandId);
            errorMessage = string.Empty;
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region [获取品牌信息]
        /// <summary>获取品牌信息
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
        public GoodsBrandInfo GetBrandDetail(Guid brandId)
        {
            var result = GoodsServerClient.GetBrandDetail(brandId);
            if (result != null && result.IsSuccess)
            {
                return result.Data == null ? null : ConvertToGoodsBrandInfo(result.Data);
            }
            return null;
        }
        #endregion

        #region [设置品牌序号]
        /// <summary>设置品牌序号
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="orderIndex"></param>
        /// <returns></returns>
        public bool UpdateBrandOrderIndex(Guid brandId, int orderIndex)
        {
            var result = GoodsServerClient.UpdateBrandOrderIndex(brandId, orderIndex);
            return result != null && result.IsSuccess;
        }
        #endregion

        #region [根据商品是否有资料获取品牌]
        /// <summary>根据商品是否有资料获取品牌
        /// </summary>
        /// <param name="hasInformation">是否有商品资料</param>
        /// <returns></returns>
        public IEnumerable<GoodsBrandInfo> GetBrandList(bool hasInformation)
        {
            var result = GoodsServerClient.GetBrandList(hasInformation);
            if (result != null && result.IsSuccess)
            {
                var list = result.Data ?? new List<BrandInfo>();
                return ConvertToGoodsBrandList(list);
            }
            return new List<GoodsBrandInfo>();
        }
        #endregion

        #region [获取品牌列表]
        /// <summary>获取品牌列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GoodsBrandInfo> GetAllBrandList()
        {
            var result = GoodsServerClient.GetAllBrandList();
            if (result != null && result.IsSuccess)
            {
                var list = result.Data ?? new List<BrandInfo>();
                return ConvertToGoodsBrandList(list);
            }
            return new List<GoodsBrandInfo>();
        }
        #endregion

        #region [品牌模型转换]

        #region  品牌资质
        /// <summary>
        /// ERP -->  GMS
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static BrandQualificationDetailInfo ConvertToBrandInformationInfo(GoodsInformationInfo info)
        {
            return new BrandQualificationDetailInfo
            {
                BrandID = info.IdentifyId,
                BrandQualificationName = info.IdentifyName,
                Path = info.Path,
                LastOperationTime = info.UploadDate == null ? DateTime.MinValue : Convert.ToDateTime(info.UploadDate),
                OverdueDate = info.OverdueDate == null ? DateTime.MinValue : Convert.ToDateTime(info.OverdueDate),
                ExtensionName = info.ExtensionName
            };
        }
        #endregion

        #region -->(GMS)BrandDetail  > (ERP)GoodsBrandInfo
        static GoodsBrandInfo ConvertToGoodsBrandInfo(BrandDetail detail)
        {
            var infoList = new List<GoodsInformationInfo>();
            if (detail.BrandQualificationList != null)
            {
                infoList.AddRange(detail.BrandQualificationList.Select(info => new GoodsInformationInfo
                                                                            {
                                                                                ExtensionName = info.ExtensionName,
                                                                                ID = info.BrandID,
                                                                                IdentifyId = info.BrandID,
                                                                                Name = info.BrandQualificationName,
                                                                                OverdueDate = info.OverdueDate,
                                                                                Path = info.Path,
                                                                                Type = 0,
                                                                                UploadDate = info.LastOperationTime
                                                                            }));
            }
            var brandInfo = ConvertToGoodsBrandInfo(detail.BrandInfo);
            brandInfo.GoodsInformationList = infoList;
            return brandInfo;
        }
        #endregion

        #region -->(GMS)BrandInfo  > (ERP)GoodsBrandInfo
        static GoodsBrandInfo ConvertToGoodsBrandInfo(BrandInfo info)
        {
            var brandInfo = new GoodsBrandInfo
            {
                OrderIndex = info.OrderIndex,
                Description = info.Description,
                Brand = info.Brand,
                BrandId = info.BrandId,
                BrandLogo = info.BrandLogo,
            };
            return brandInfo;
        }
        #endregion

        #region -->(ERP)GoodsBrandInfo  > (GMS)BrandDetail
        static BrandInfo ConvertToBrandInfo(GoodsBrandInfo info)
        {
            var brandInfo = new BrandInfo
            {
                OrderIndex = info.OrderIndex,
                Description = info.Description,
                Brand = info.Brand,
                BrandId = info.BrandId,
                BrandLogo = info.BrandLogo,
            };
            return brandInfo;
        }
        #endregion

        #region -->(ERP)GoodsBrandInfo  > (GMS)BrandDetail
        static IEnumerable<GoodsBrandInfo> ConvertToGoodsBrandList(IEnumerable<BrandInfo> infoList)
        {
            return infoList.Select(ConvertToGoodsBrandInfo);
        }
        #endregion

        #endregion
    }
}

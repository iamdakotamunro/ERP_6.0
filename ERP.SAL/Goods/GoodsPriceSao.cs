using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model.Goods;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using KeedeGroup.GoodsManageSystem.Public.Model.Table;
using GoodsSalePriceInfo = ERP.Model.Goods.GoodsSalePriceInfo;

namespace ERP.SAL.Goods
{
    public partial class GoodsCenterSao
    {
        #region -- 模型转换

        #region --> (GMS)GoodsSalePriceInfo > (ERP)GoodsSalePriceInfo
        static GoodsSalePriceInfo ConvertToMyGoodsSalePriceInfo(KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsSalePriceInfo goodsSalePriceInfo)
        {
            return new GoodsSalePriceInfo
            {
                GoodsId = goodsSalePriceInfo.GoodsID,
                GroupId = goodsSalePriceInfo.GroupID,
                Price = goodsSalePriceInfo.Price
            };
        }
        static IEnumerable<GoodsSalePriceInfo> ConvertToMyGoodsSalePriceList(IEnumerable<KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsSalePriceInfo> goodsSalePriceList)
        {
            return goodsSalePriceList.Select(ConvertToMyGoodsSalePriceInfo);
        }

        #endregion


        #region --> (GMS)RolePriceBaseInfo > (ERP)GoodsRolePriceInfo
        static GoodsRolePriceInfo ConvertToMyGoodsRolePriceInfo(RolePriceBaseInfo rolePriceBaseInfo)
        {
            return new GoodsRolePriceInfo
            {
                GoodsId = rolePriceBaseInfo.GoodsID,
                GroupId = rolePriceBaseInfo.GroupID,
                Price = rolePriceBaseInfo.Price,
                RoleId = rolePriceBaseInfo.RoleID,
                RoleName = rolePriceBaseInfo.RoleName,
                Discount = rolePriceBaseInfo.Discount
            };
        }
        static IEnumerable<GoodsRolePriceInfo> ConvertToMyGoodsRolePriceList(IEnumerable<RolePriceBaseInfo> goodsRolePriceList)
        {
            return goodsRolePriceList.Select(ConvertToMyGoodsRolePriceInfo);
        }

        static IEnumerable<Model.Goods.ThirdGoodsSalePriceInfo> ConvertToMyThirdPriceList(
            IEnumerable<KeedeGroup.GoodsManageSystem.Public.Model.Table.ThirdGoodsSalePriceInfo> thirdPriceList)
        {
            return thirdPriceList.Select(item => new Model.Goods.ThirdGoodsSalePriceInfo
            {
                GoodsId = item.GoodsId,
                GroupId = item.GroupId,
                SalePlatformId = item.SalePlatformId,
                Price = item.Price,
                IsDefault = item.IsDefault
            });
        }

        #endregion

        #region --> (GMS)GroupInfo > (ERP)GoodsGroupInfo
        static Model.Goods.GoodsGroupInfo ConvertToMyGoodsGroupInfo(GroupInfo groupInfo)
        {
            return new Model.Goods.GoodsGroupInfo
            {
                GroupId = groupInfo.GroupID,
                GroupName = groupInfo.Name,
            };
        }
        #endregion

        #region --> (ERP)GoodsSalePriceInfo > (GMS)GoodsPriceDetail
        static IEnumerable<GroupGoodsPriceRequestModel> ConvertToGoodsPriceDetailList(IEnumerable<GoodsSalePriceInfo> goodsSalePriceList)
        {
            var detailList = new List<GroupGoodsPriceRequestModel>();
            foreach (var item in goodsSalePriceList)
            {
                var detailInfo = new GroupGoodsPriceRequestModel
                {
                    GroupId = item.GroupId,
                    Price = item.Price
                };
                var dicRoldPrice = item.GoodsRolePriceList.ToDictionary(info => info.RoleId, info => info.Price);
                detailInfo.RolePrice = dicRoldPrice;
                detailList.Add(detailInfo);
            }
            return detailList;
        }
        #endregion

        #endregion

        #region --> 更新参考价
        /// <summary>
        /// 更新参考价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="referencePrice"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateReferencePrice(Guid goodsId, decimal referencePrice, out string errorMessage)
        {
            var result = GoodsServerClient.UpdateReferencePrice(goodsId, referencePrice);
            errorMessage = result == null ? "GMS连接异常" : result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region --> 更新加盟价
        /// <summary>
        /// 更新加盟价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="joinPrice"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateJoinPrice(Guid goodsId, decimal joinPrice, out string errorMessage)
        {
            var result = GoodsServerClient.UpdateJoinPrice(goodsId, joinPrice);
            errorMessage = result == null ? "GMS连接异常" : result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region --> 更新批发价
        /// <summary>
        /// 更新批发价
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="wholesalePrice">批发价</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        /// zal 2015-09-10
        public bool UpdateWholesalePrice(Guid goodsId, decimal wholesalePrice, out string errorMessage)
        {
            var result = GoodsServerClient.UpdateWholesalePrice(goodsId, wholesalePrice);
            errorMessage = result == null ? "GMS连接异常" : result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region --> 更新隐性成本
        /// <summary>
        /// 更新隐性成本
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="implicitCost"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateImplicitCost(Guid goodsId, decimal implicitCost, out string errorMessage)
        {
            var result = GoodsServerClient.UpdateImplicitCost(goodsId, implicitCost);
            errorMessage = result == null ? "GMS连接异常" : result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion
        #region --> 更新年终扣率
        /// <summary>
        /// 更新年终扣率
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="yearDiscount"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateYearDiscount(Guid goodsId, decimal yearDiscount, out string errorMessage)
        {
            var result = GoodsServerClient.UpdateYearDiscount(goodsId, yearDiscount);
            errorMessage = result == null ? "GMS连接异常" : result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region --> 价格更新

        /// <summary>
        /// 价格更新
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="goodsSalePriceList"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool SetGoodsPrice(Guid goodsId, IEnumerable<GoodsSalePriceInfo> goodsSalePriceList, out string errorMessage)
        {
            var goodsSalePriceInfos = goodsSalePriceList as GoodsSalePriceInfo[] ?? goodsSalePriceList.ToArray();
            var reqeustModel = ConvertToGoodsPriceDetailList(goodsSalePriceInfos).ToList();
            var thirdGoodsPriceList = new List<ThirdGoodsPriceRequestModel>();
            if (goodsSalePriceList != null && goodsSalePriceInfos.Any())
            {
                thirdGoodsPriceList.AddRange(from goodsSalePriceInfo in goodsSalePriceInfos
                                             where goodsSalePriceInfo.ThirdPriceList != null && goodsSalePriceInfo.ThirdPriceList.Any()
                                             from thirdGoodsInfo in goodsSalePriceInfo.ThirdPriceList
                                             select new ThirdGoodsPriceRequestModel
                                             {
                                                 GroupId = goodsSalePriceInfo.GroupId,
                                                 SalePlatformId = thirdGoodsInfo.SalePlatformId,
                                                 Price = thirdGoodsInfo.Price
                                             });
            }
            var result = GoodsServerClient.SetGoodsPrice(goodsId, reqeustModel, thirdGoodsPriceList);
            errorMessage = result == null ? "GMS连接异常" : result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region --> 根据商品ID获取价格
        /// <summary>
        /// 根据商品ID获取价格
        /// </summary>
        /// <returns></returns>
        public DealGoodsSalePriceInfo GetDealGoodsSalePriceInfoByGoodsId(Guid goodsId)
        {
            var dealGoodsSalePriceInfo = new DealGoodsSalePriceInfo();
            var result = GoodsServerClient.GetGoodsPriceRequestModel(goodsId);
            if (result != null && result.IsSuccess)
            {
                var goodsPriceRequestModel = result.Data;
                if (goodsPriceRequestModel != null)
                {
                    dealGoodsSalePriceInfo.GoodsId = goodsPriceRequestModel.GoodsId;
                    dealGoodsSalePriceInfo.GoodsName = goodsPriceRequestModel.GoodsName;
                    dealGoodsSalePriceInfo.GoodsCode = goodsPriceRequestModel.GoodsCode;
                    dealGoodsSalePriceInfo.MarketPrice = goodsPriceRequestModel.MarketPrice;
                    dealGoodsSalePriceInfo.ReferencePrice = goodsPriceRequestModel.ReferencePrice;
                    dealGoodsSalePriceInfo.JoinPrice = goodsPriceRequestModel.JoinPrice;
                    dealGoodsSalePriceInfo.ImplicitCost = goodsPriceRequestModel.ImplicitCost;
                    dealGoodsSalePriceInfo.YearDiscount = goodsPriceRequestModel.YearDiscount;
                    var groupGoodsPriceList = new List<GoodsSalePriceInfo>();
                    foreach (var groupGoodsPriceModel in goodsPriceRequestModel.GroupGoodsPriceList)
                    {
                        var goodsSalePriceInfo = new GoodsSalePriceInfo();
                        if (groupGoodsPriceModel.GroupInfo != null)
                        {
                            goodsSalePriceInfo.GoodsGroupInfo = ConvertToMyGoodsGroupInfo(groupGoodsPriceModel.GroupInfo);
                            goodsSalePriceInfo.GroupId = goodsSalePriceInfo.GoodsGroupInfo.GroupId;
                            goodsSalePriceInfo.Price = groupGoodsPriceModel.Price;
                        }
                        if (groupGoodsPriceModel.RolePriceList != null && groupGoodsPriceModel.RolePriceList.Count > 0)
                            goodsSalePriceInfo.GoodsRolePriceList = ConvertToMyGoodsRolePriceList(groupGoodsPriceModel.RolePriceList).ToList();
                        if (groupGoodsPriceModel.ThirdPriceList != null && groupGoodsPriceModel.ThirdPriceList.Count > 0)
                            goodsSalePriceInfo.ThirdPriceList = ConvertToMyThirdPriceList(groupGoodsPriceModel.ThirdPriceList).ToList();
                        groupGoodsPriceList.Add(goodsSalePriceInfo);
                    }
                    dealGoodsSalePriceInfo.GroupGoodsPriceList = groupGoodsPriceList;
                }
            }
            return dealGoodsSalePriceInfo;
        }
        #endregion

        #region --> 获取商品在所有平台的销售价
        /// <summary>
        /// 获取商品在所有平台的销售价
        /// </summary>
        /// <returns></returns>
        public IList<GoodsSalePriceInfo> GetGoodsPriceListByGoodsList(List<Guid> goodsIdList)
        {
            var result = GoodsServerClient.GetGoodsPriceListByGoodsList(goodsIdList);
            if (result != null && result.IsSuccess)
            {
                var list = result.Data ?? new List<KeedeGroup.GoodsManageSystem.Public.Model.Table.GoodsSalePriceInfo>();
                return ConvertToMyGoodsSalePriceList(list).ToList();
            }
            return new List<GoodsSalePriceInfo>();
        }
        #endregion
    }
}

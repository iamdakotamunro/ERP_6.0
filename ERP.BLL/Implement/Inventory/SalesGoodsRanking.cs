using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Inventory;
using ERP.Model;
using ERP.DAL.Interface.IInventory;
using ERP.SAL.Goods;
using ERP.SAL.Interface;

namespace ERP.BLL.Implement.Inventory
{
    public class SalesGoodsRankingManager : BllInstance<SalesGoodsRankingManager>
    {
        private readonly ISalesGoodsRanking _salesGoodsRankingDao;
        readonly IGoodsCenterSao _goodsClassSao;

        public SalesGoodsRankingManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _salesGoodsRankingDao = InventoryInstance.GetSalesGoodsRankingDao(fromType);
            _goodsClassSao = new GoodsCenterSao();
        }

        public SalesGoodsRankingManager(IGoodsCenterSao goodsClassSao, ISalesGoodsRanking salesGoodsRankingDao)
        {
            _salesGoodsRankingDao = salesGoodsRankingDao;
            _goodsClassSao = goodsClassSao;
        }

        /// <summary>
        /// 商品销量排行表
        /// </summary>
        /// <param name="top"></param>
        /// <param name="classId"></param>
        /// <param name="brandId">品牌ID</param>
        /// <param name="goodsName"></param>
        /// <param name="goodsId"></param>
        /// <param name="salePlatformId"> </param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="salefilialeId"> </param>
        /// <param name="isContainDisableSalePlatform"> </param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<SaleRaningShowInfo> GetGoodsSalesRanking(int top, Guid classId, Guid brandId, string goodsName, Guid goodsId,
            Guid salefilialeId, Guid salePlatformId, DateTime startTime, DateTime endTime, bool isContainDisableSalePlatform, int type)
        {
            var classList = new StringBuilder();
            string classStrList = string.Empty;
            if (classId != Guid.Empty)
            {
                var childClassList = _goodsClassSao.GetChildClassList(classId);
                foreach (var child in childClassList)
                {
                    classList.Append("'" + child.ClassId + "',");
                }
                if (childClassList.Count == 0)
                {
                    classList.Append("'" + classId + "'");
                    classStrList = classList.ToString();
                }
                else
                {
                    classStrList = classList.ToString();
                    classStrList = classStrList.Substring(0, classStrList.Length - 1);
                }
            }

            switch (type)
            {
                case 0:
                    return _salesGoodsRankingDao.GetGoodsSaleRankingBySale(top, classStrList, brandId, goodsName,
                        goodsId, salefilialeId, salePlatformId, startTime, endTime, isContainDisableSalePlatform);
                case 1:
                    return _salesGoodsRankingDao.GetGoodsSaleRankingBySalePlate(top, classStrList, brandId, goodsName,
                        goodsId, salefilialeId, salePlatformId, startTime, endTime, isContainDisableSalePlatform);
                case 2:
                    return _salesGoodsRankingDao.GetGoodsSaleRankingByBrand(top, classStrList, brandId, goodsName,
                        goodsId, salefilialeId, salePlatformId, startTime, endTime, isContainDisableSalePlatform);
                case 3:
                    return _salesGoodsRankingDao.GetGoodsSaleRankingBySeriesId(top, classStrList, brandId, goodsName,
                        goodsId, salefilialeId, salePlatformId, startTime, endTime, isContainDisableSalePlatform);
                default:
                    return new List<SaleRaningShowInfo>();
            }
        }

        /// <summary>
        /// 更新销量表中系列ID
        /// </summary>
        /// <param name="goodsIdSeriesIds"></param>
        /// <returns></returns>
        public bool UpdateGoodsSaleSeriesId(Dictionary<Guid,Guid> goodsIdSeriesIds)
        {
            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (goodsIdSeriesIds.Any(goodsIdSeriesId => !_salesGoodsRankingDao.UpdateGoodsSaleSeriesId(goodsIdSeriesId.Key, goodsIdSeriesId.Value)))
                    {
                        return false;
                    }
                    ts.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("日销量表商品系列更新失败!",ex);
            }
        }
    }
}

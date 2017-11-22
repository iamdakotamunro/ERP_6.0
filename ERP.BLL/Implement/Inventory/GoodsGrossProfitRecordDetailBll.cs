using ERP.DAL.Interface.IInventory;
using ERP.Model.Report;
using ERP.SAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.Implement.Inventory
{
    public class GoodsGrossProfitRecordDetailBll
    {
        readonly IGoodsGrossProfitRecordDetail _goodsGrossProfitRecordDetail;
        readonly IGoodsCenterSao _goodsInfoSao;

        public GoodsGrossProfitRecordDetailBll(IGoodsGrossProfitRecordDetail goodsGrossProfitRecordDetail, IGoodsCenterSao goodsInfoSao)
        {
            _goodsGrossProfitRecordDetail = goodsGrossProfitRecordDetail;
            _goodsInfoSao = goodsInfoSao;
        }

        /// <summary>
        /// 根据条件合计商品毛利明细表数据
        /// </summary>
        /// <param name="startTIme"></param>
        /// <param name="endTime"></param>
        /// <param name="goodsTypes"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="salePlatformIds"></param>
        /// <param name="orderTypes"></param>
        /// <returns></returns>
        public IList<GoodsGrossProfitInfo> SumGoodsGrossProfitRecordDetailInfos(DateTime startTIme, DateTime endTime, string goodsTypes, string goodsNameOrCode, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            var goodsGrossProfitRecordDetailList = _goodsGrossProfitRecordDetail.SumGoodsGrossProfitRecordDetailInfos(startTIme, endTime, goodsTypes, saleFilialeId, salePlatformIds, orderTypes);
            return ShowGoodsGrossProfitRecordDetailInfos(goodsGrossProfitRecordDetailList, goodsNameOrCode);
        }

        /// <summary>
        /// 汇总同一商品同一公司不同平台的数据
        /// </summary>
        /// <param name="startTIme"></param>
        /// <param name="endTime"></param>
        /// <param name="goodsTypes"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="salePlatformIds"></param>
        /// <param name="orderTypes"></param>
        /// <returns></returns>
        public IList<GoodsGrossProfitInfo> SumGoodsGrossProfitByGoodsIdAndSaleFilialeId(DateTime startTIme, DateTime endTime, string goodsTypes, string goodsNameOrCode, Guid saleFilialeId, string salePlatformIds, string orderTypes)
        {
            var goodsGrossProfitRecordDetailList = _goodsGrossProfitRecordDetail.SumGoodsGrossProfitByGoodsIdAndSaleFilialeId(startTIme, endTime, goodsTypes, saleFilialeId, salePlatformIds, orderTypes);
            return ShowGoodsGrossProfitRecordDetailInfos(goodsGrossProfitRecordDetailList, goodsNameOrCode);
        }

        /// <summary>
        /// 数据组装、商品名称和code赋值
        /// </summary>
        /// <param name="goodsGrossProfitInfoList"></param>
        /// <param name="goodsNameOrCode"></param>
        /// <returns></returns>
        private IList<GoodsGrossProfitInfo> ShowGoodsGrossProfitRecordDetailInfos(IList<GoodsGrossProfitInfo> goodsGrossProfitInfoList, string goodsNameOrCode)
        {
            var dataList = new List<GoodsGrossProfitInfo>();
            if (goodsGrossProfitInfoList != null && goodsGrossProfitInfoList.Count > 0)
            {
                var goodsIds = goodsGrossProfitInfoList.Select(act => act.GoodsId).Distinct().ToList();
                var goodsInfos = _goodsInfoSao.GetGoodsListByGoodsIds(goodsIds);
                if (goodsInfos != null && goodsInfos.Count > 0)
                {
                    if (!string.IsNullOrEmpty(goodsNameOrCode))
                    {
                        goodsInfos =
                            goodsInfos.Where(act => act.GoodsName.Contains(goodsNameOrCode)
                                || act.GoodsCode.Contains(goodsNameOrCode) || act.OldGoodsCode.Contains(goodsNameOrCode)).ToList();
                    }
                    foreach (var goodsGrossProfitInfo in goodsGrossProfitInfoList)
                    {
                        var info = goodsInfos.FirstOrDefault(act => act.GoodsId == goodsGrossProfitInfo.GoodsId);
                        if (info == null) continue;
                        goodsGrossProfitInfo.GoodsName = info.GoodsName;
                        goodsGrossProfitInfo.GoodsCode = info.GoodsCode;
                        dataList.Add(goodsGrossProfitInfo);
                    }
                }
            }
            return dataList;
        }
    }
}

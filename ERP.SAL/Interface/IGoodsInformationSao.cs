using System;
using System.Collections.Generic;
using ERP.Model;
using ERP.Model.Goods;

namespace ERP.SAL.Interface
{
    public partial interface IGoodsCenterSao
    {
        IList<SupplierGoodsInfo> SelectGoodsInformationInfosByPage(string goodsName, string nameOrNo,
            int? state, int? dateState, int pageIndex, int pageSize, out long totalCount);

        /// <summary>
        /// 获取资料信息
        /// </summary>
        /// <param name="identifyId"></param>
        /// <returns></returns>
        IList<GoodsInformationInfo> GetInformation(Guid identifyId);

        /// <summary>
        /// 添加商品资料
        /// </summary>
        /// <param name="identifyId"></param>
        /// <param name="informationList"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool SetInformationList(Guid identifyId, IEnumerable<GoodsInformationInfo> informationList,
            out string errorMessage);

        /// <summary>
        /// 根据GoodsId获取商品资料信息
        /// </summary>
        /// <param name="goodsIds">商品id列表</param>
        /// <returns></returns>
        /// zal 2015-11-23
        Dictionary<Guid, List<GoodsInformationInfo>> GetGoodsInformationList(List<Guid> goodsIds);
    }
}

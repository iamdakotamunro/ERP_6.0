using System;
using System.Collections.Generic;
using ERP.Enum;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IPurchasingDetail
    {
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="info"></param>
        void Insert(PurchasingDetailInfo info);
        /// <summary>
        /// 修改实际来货数量
        /// </summary>
        /// <param name="info"></param>
        void UpdateRealQuantity(PurchasingDetailInfo info, Guid purchasingGoodsId);

        /// <summary>
        /// 修改实际来货数量、状态、价格
        /// </summary>
        /// <param name="info"></param>
        /// <param name="purchasingGoodsId"></param>
        bool UpdatePurchasingDetail(PurchasingDetailInfo info, Guid purchasingGoodsId);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="purchasingID"></param>
        void Delete(Guid purchasingID);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="info"></param>
        void Update(PurchasingDetailInfo info, Guid purchasingGoodsId);
        /// <summary>
        /// 获取指定的采购商品
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <param name="purchGoodsId"></param>
        /// <returns></returns>
        PurchasingDetailInfo GetPurchGoodsId(Guid purchasingID, Guid purchGoodsId);
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <returns></returns>
        List<PurchasingDetailInfo> Select(Guid purchasingID);

        /// <summary>
        /// 删除采购单下的商品
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="goodsId"></param>
        /// <param name="purchasingGoodsId"></param>
        void DeleteByGoodsId(Guid purchasingId, Guid goodsId, Guid purchasingGoodsId);

        /// <summary>
        /// 根据商品ID，供应商ID，仓库ID获取是否有该商品的采购记录
        /// </summary>
        /// <param name="goodsID"></param>
        /// <param name="companyID"></param>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        IList<PurchasingDetailInfo> GetPurchasingDetail(Guid goodsID, Guid companyID, Guid warehouseID, PurchasingType ptype, PurchasingState pstate);

        /// <summary>
        /// 根据商品ID和仓库ID求出该商品在采购中的数量 add by lxm 20110428
        /// </summary>
        /// <param name="goodsID"></param>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        double GetPurchasingNumber(Guid goodsID, Guid warehouseID);
        /// <summary>
        /// 修改指定采购商品完成状态 yes 为完成 no 为未完成
        /// </summary>
        /// <param name="yesno"></param>
        /// <param name="purchasingGoodsID"></param>
        void UpdateGoodState(YesOrNo yesno, Guid purchasingGoodsID);
        /// <summary>
        /// 查询采购的商品明细 (包括60天日均,30天日均,11天日均)
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <returns></returns>
        List<PurchasingDetailInfo> SelectByGoodsDayAvgSales(Guid purchasingID);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <returns></returns>
        List<PurchasingDetailInfo> SelectDetail(Guid purchasingID);

        /// <summary>
        /// 获取子商品的60、30、11天销量
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="endTime"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        PurchasingDetailInfo GetChildGoodsSale(Guid realGoodsId, Guid warehouseId, DateTime endTime,Guid hostingFilialeId);

        /// <summary>
        /// 多条采购单存在同主商品且有一条待调价的记录,
        /// 调价审核通过时同步更新未提交的采购单中的商品采购价
        /// </summary>
        /// <param name="realGoodsIds">子商品列表</param>
        /// <param name="companyId">主商品绑定公司</param>
        /// <param name="price">价格</param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        bool UpdatePurchasingDetailPrice(IEnumerable<Guid> realGoodsIds,Guid companyId,decimal price,Guid warehouseId,Guid hostingFilialeId);
        
        /// <summary>
        /// 修改采购单商品价格，排除赠品
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="realGoodsIds"></param>
        /// <param name="price"></param>
        /// <param name="cprice"></param>
        /// <returns></returns>
        bool UpdateDetailsPrice(Guid purchasingId, List<Guid> realGoodsIds, decimal price, decimal cprice);

        /// <summary>
        /// 获取进货申报生成的采购明细
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="purchasingStates"></param>
        /// <param name="realGoodsIds"></param>
        /// <returns></returns>
        Dictionary<Guid, int> GetStockDeclarePursingGoodsDics(Guid warehouseId, PurchasingState[] purchasingStates,
            IEnumerable<Guid> realGoodsIds);

        Dictionary<Guid, Dictionary<Guid, int>> GetStockDeclarePursingGoodsDicsWithFiliale(Guid warehouseId,
            IEnumerable<Guid> hostingFilialeIds, PurchasingState[] purchasingStates, IEnumerable<Guid> realGoodsIds);
    }
}

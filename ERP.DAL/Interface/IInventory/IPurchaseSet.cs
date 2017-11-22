using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model.WMS;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 商品采购设置
    /// </summary>
    public interface IPurchaseSet
    {
        /// <summary>
        /// 获取所有商品采购设置信息
        /// </summary>
        /// <returns></returns>
        IList<PurchaseSetInfo> GetPurchaseSetList();

        /// <summary>
        /// 获取所有商品采购设置信息
        /// </summary>
        /// <returns></returns>
        IList<PurchaseSetInfo> GetPurchaseSetListToPage(bool load, List<Guid> goodsIdList, string goodsName, Guid companyId, Guid personResponsible,
            int filingForm, int stockUpDay, int statue, int startPage, int pageSize, out long recordCount);

        /// <summary>
        /// 获取所有商品采购设置信息
        /// </summary>
        /// <returns></returns>
        IList<PurchaseSetInfo> GetPurchaseSetListByWarehouseId(Guid warehouseId);

        /// <summary>
        /// 获取所有商品采购设置信息
        /// </summary>
        /// <returns></returns>
        List<PurchaseSetInfo> GetPurchaseSetListByWarehouseIdAndCompanyId(Guid warehouseId, Guid companyId);

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="goodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        IList<PurchaseSetInfo> GetPurchaseSetList(List<Guid> goodsIdList, Guid warehouseId);

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="goodsIdList"></param>
        /// <param name="warehouseId"></param>
        /// <param name="isDelete"></param>
        /// <returns>0:禁用；1:启用；2:全部</returns>
        /// zal 2017-03-16
        IList<PurchaseSetInfo> GetPurchaseSetList(List<Guid> goodsIdList, Guid warehouseId, int isDelete);

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        PurchaseSetInfo GetPurchaseSetInfo(Guid goodsId, Guid hostingFilialeId ,Guid warehouseId);

        ///// <summary>
        ///// 获取商品采购设置
        ///// </summary>
        ///// <param name="goodsId"></param>
        ///// <param name="warehouseId"></param>
        ///// <returns></returns>
        List<PurchaseSetInfo> GetPurchaseSetInfo(Guid goodsId, Guid warehouseId);

        /// <summary>
        /// 获取商品采购设置
        /// </summary>
        /// <param name="hostingFilialeId"></param>
        /// <param name="goodsIds"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        List<PurchaseSetInfo> GetPurchaseSetInfoList(IEnumerable<Guid> goodsIds, Guid warehouseId, Guid hostingFilialeId);

        /// <summary>
        /// 获取指定仓库中已全部绑定指定商品的仓库和物流公司
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="warehouseIds"></param>
        /// <returns></returns>
        List<KeyValuePair<Guid,Guid>> GetKeyAndValueGuids(IEnumerable<Guid> goodsIds,IEnumerable<Guid> warehouseIds);

        /// <summary>
        /// 获取所有指定仓库中已全部绑定指定商品的仓库和物流公司
        /// </summary>
        /// <returns></returns>
        Dictionary<KeyValuePair<Guid, Guid>, List<Guid>> GetKeyAndValueGuids();

        /// <summary>
        /// 商品采购设置中获取责任人ID和责任人名字
        /// </summary>
        /// <returns></returns>
        IList<PurchaseSetInfo> GetPersonList();

        /// <summary>
        /// 添加商品采购设置
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        bool AddPurchaseSet(PurchaseSetInfo info, out string errorMessage);

        /// <summary>
        /// 修改商品采购设置
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        int UpdatePurchaseSet(PurchaseSetInfo info, out string errorMessage);

        /// <summary>采购已分配管理批量转移供应商
        /// </summary>
        void BatchTransferCompany(Guid oldCompanyId, Guid newCompanyId);

        /// <summary>
        /// 查询商品是否备货，用户库存查询
        /// </summary>
        /// <returns></returns>
        string GetGoodsIsReady(Guid goodsId);

        Dictionary<Guid, String> GetGoodsIsReadyByWarehouseId(Guid warehouseId,IEnumerable<Guid> goodsIds);

        int UpdatePurchaseSetToPurchaseGroupId(List<Guid> goodsIds, Guid companyId, Guid purchaseGroupId, out string errorMessage);

        int UpdatePurchaseSetDefault(Guid companyId, Guid purchaseGroupId, out string errorMessage);

        /// <summary>新版删除商品采购设置（假删除）
        /// </summary>
        /// <param name="goodsIds">商品ID集合</param>
        /// <returns>受影响的行数</returns>
        int NewDeletePurchaseSet(List<Guid> goodsIds);

        /// <summary>新版删除商品采购设置（假删除）
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="state">状态枚举（0禁用，1启用）</param>
        /// <returns>受影响的行数</returns>
        int NewDeletePurchaseSet(Guid goodsId, Guid warehouseId, State state);

        /// <summary>根据采购负责人ID获取其绑定的供应商信息(供应商ID，和供应商名称)
        /// </summary>
        /// <param name="personnelId">采购人负责人ID</param>
        /// <returns></returns>
        IList<CompanyCussentInfo> GetCompanyIds(Guid personnelId);

        /// <summary>根据供应商ID获取绑定此供应商的商品ID集合   2015-04-17  陈重文
        /// </summary>
        /// <param name="companyId">供应商ID</param>
        /// <returns></returns>
        IList<Guid> GetGoodsIdByCompanyId(Guid companyId);

        /// <summary>获取采购责任人所负责商品ID集合  2015-04-30  陈重文
        /// </summary>
        /// <param name="personnelId">责任人ID</param>
        /// <param name="companyId">供应商ID</param>
        /// <returns></returns>
        IList<Guid> GetGoodsIdByPersonnelId(Guid personnelId, Guid companyId);

        /// <summary>获取仓库下所有的商品采购设置，含供应商名称（库存周转率使用） 2015-06-13  陈重文  
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <returns></returns>
        IList<PurchaseSetInfo> GetAllPurchaseSet(Guid warehouseId);

        /// <summary>
        /// 根据仓库id和主商品Id获得供应商
        /// 2016-08-04  
        /// 文雯  
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="goodsId">主商品Id</param>
        /// <returns></returns>
        Guid GetCompanyByWarehouseIdAndGoodsId(Guid warehouseId, Guid hostingFilialeId, Guid goodsId);

        /// <summary>
        /// 根据仓库Id获取供应商Id
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        /// zal 2017-04-19
        Dictionary<Guid, Guid> GetCompanyIdByWarehouseId(Guid warehouseId,Guid hostingFilialeId);

        /// <summary>
        /// 判断是否存在采购设置
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFiliaeId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        bool IsExist(Guid warehouseId, Guid hostingFiliaeId, Guid goodsId);
    }
}

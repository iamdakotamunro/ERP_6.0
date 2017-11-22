using System;
using System.Collections.Generic;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 采购单操作接口 2011-03-22  by jiang
    /// </summary>
    public interface IPurchasing
    {
        /// <summary>
        /// 插入一张采购单
        /// </summary>
        /// <param name="pInfo">供应商采购单</param>
        void PurchasingInsert(PurchasingInfo pInfo);

        /// <summary>
        /// 插入一张采购单，含明细
        /// </summary>
        /// <param name="pInfo"></param>
        /// <param name="purchasingDetailInfoList"></param>
        void PurchasingInsertWithDetails(PurchasingInfo pInfo, IList<PurchasingDetailInfo> purchasingDetailInfoList);

        /// <summary>
        /// 更新一张采购单
        /// </summary>
        /// <param name="pInfo">供应商采购单</param>
        void PurchasingUpdate(PurchasingInfo pInfo);

        /// <summary>
        /// 查询采购单
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">完成时间</param>
        /// <param name="companyId">供应商</param>
        /// <param name="wareHouseId">仓库</param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="pState">状态</param>
        /// <param name="pType">采购类别</param>
        /// <param name="serchKey">查询关键字</param>
        /// <param name="goodsId"></param>
        /// <param name="personResponsible">责任人ID</param>
        IList<PurchasingInfo> GetPurchasingList(DateTime startTime, DateTime endTime, Guid companyId, Guid wareHouseId,Guid hostingFilialeId, PurchasingState pState, PurchasingType pType, string serchKey, Guid goodsId, Guid personResponsible);

        /// <summary>
        /// 查询操作人所在组的采购单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="companyId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="pState"></param>
        /// <param name="pType"></param>
        /// <param name="serchKey"></param>
        /// <param name="realGoodsIds"></param>
        /// <param name="pmId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        IList<PurchasingInfo> GetPurchasingList(DateTime startTime, DateTime endTime, Guid companyId, Guid wareHouseId,Guid hostingFilialeId,
                                                PurchasingState pState, PurchasingType pType, string serchKey,
                                                List<Guid> realGoodsIds, Guid pmId, Guid personId);

        /// <summary>
        /// 查询操作人所在组的采购单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="companyId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="pStates"></param>
        /// <param name="pType"></param>
        /// <param name="serchKey"></param>
        /// <param name="goodsList"> </param>
        /// <param name="warehouseIdList"></param>
        /// <param name="personResponsible"></param>
        /// <param name="purchasingFilialeId">采购公司Id</param>
        /// <param name="startPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        IList<PurchasingInfo> GetPurchasingListToPage(DateTime startTime, DateTime endTime, Guid companyId, Guid wareHouseId,
                                                List<int> pStates, PurchasingType pType, string serchKey,
                                                List<Guid> goodsList, List<Guid> warehouseIdList, Guid personResponsible,Guid purchasingFilialeId,
                                                int startPage, int pageSize, out long recordCount);

        /// <summary>
        /// 根据id查询采购单
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <returns></returns>
        PurchasingInfo GetPurchasingById(Guid purchasingId);
        /// <summary>
        /// 删除一条采购记录
        /// </summary>
        /// <param name="purchasingId"></param>
        void DeleteById(Guid purchasingId);
        /// <summary>
        /// 修改采购单的状态
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="purchasingState"></param>
        void PurchasingUpdate(Guid purchasingId, PurchasingState purchasingState, Guid purchasingFilialeId);
        /// <summary>
        /// 查询该仓库下的采购中和部分完成的采购单
        /// </summary>
        IList<PurchasingInfo> GetPurchasingList(Guid wareHouseId);
        /// <summary>
        /// 增加备注
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="description"></param>
        void PurchasingDescription(Guid purchasingId, string description);
        /// <summary>
        /// 根据供应商ID获取采购单列表
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IList<PurchasingInfo> GetPurchasingListByCompanyID(Guid companyId, int purchasingState, int purchasingType, Guid warehouseId);

        /// <summary>
        /// 根据采购单ID找出该采购单的负责人
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <returns></returns>
        Guid GetRealNameByPurchasingID(Guid purchasingId);

        /// <summary>
        /// 根据采购单号查询采购单
        /// Add by liucaijun at 2011-August-17th
        /// </summary>
        /// <param name="purchasingOrder">采购单号</param>
        /// <returns></returns>
        PurchasingInfo GetPurchasingList(string purchasingOrder);

        /// <summary>
        /// 到货说明
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <returns></returns>
        IList<GoodsStatementInfo> GetGoodsStockStatementByPurchacingID(Guid purchasingId);

        /// <summary>
        /// 保存商品缺货声明
        /// </summary>
        /// <param name="goodsid"></param>
        /// <param name="statement"></param>
        void SaveGoodsStatement(Guid goodsid, string statement);

        /// <summary>
        /// 获取各个状态下的采购单 by jiang 2011-10-26
        /// 模型中只返回PurchasingID,PurchasingNo
        /// </summary>
        /// <param name="list"></param>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        IList<PurchasingInfo> GetPurchasingListByState(IList<PurchasingState> list, string searchKey);

        /// <summary>
        /// 根据采购单号查询采购单的金额
        /// Add by liucaijun at 2011-November-04th
        /// </summary>
        /// <param name="purchasingOrder">采购单号</param>
        /// <returns></returns>
        Decimal GetPurchasingAmount(string purchasingOrder);
        /// <summary>
        /// 查询一段时间的采购次数
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        IList<PurchaseStatisticsInfo> GetPurchaseStatisticsList(DateTime starttime, DateTime endtime);

        /// <summary>
        /// 是否采购中
        /// </summary>
        /// <param name="pmId">采购组ID</param>
        /// <returns></returns>
        bool IsPurchasing(Guid pmId);

        /// <summary>
        /// 修改采购单到货时间
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="arrivaltime"></param>
        /// <param name="director"> </param>
        void PurchasingArrivalTime(Guid purchasingId, DateTime arrivaltime, string director);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <returns></returns>
        IList<ERP.Model.PurchasingGoodsAmountInfo> GetGoodsAmountList(Guid purchasingId);

        /// <summary>
        /// 添加和更新采购信息
        /// </summary>
        /// <param name="oldPurchasingId"></param>
        /// <param name="newAddList"></param>
        /// <param name="updateDetailPurchasingIdList"></param>
        void AddNewOrUpdate(Guid oldPurchasingId, IList<PurchasingInfo> newAddList,
                            IDictionary<Guid, string> updateDetailPurchasingIdList);

        /// <summary>根据采购单号查询自采购单
        /// </summary>
        /// <param name="purchasingNo"> </param>
        /// <returns></returns>
        IList<PurchasingInfo> GetPurchasingByMateNo(string purchasingNo);

        /// <summary>
        /// 获取商品的预计到货时间
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <returns></returns>
        DateTime GetGoodsPredictArrivalTime(Guid realGoodsId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="realGoodsIds">null(取所有子商品)</param>
        /// <returns></returns>
        bool SelectPurchasingNoCompleteByGoodsId(Guid goodsId, List<Guid> realGoodsIds);


        /// <summary>
        /// 是否存在特定状态的采购单
        /// </summary>
        /// <param name="realGoodsIds">子商品列表</param>
        /// <param name="state">采购单状态</param>
        /// <returns></returns>
        IList<Guid> GetPurchasingByRealGoodsIdList(IList<Guid> realGoodsIds, int state);


        /// <summary>更新采购单为采购中专用
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        /// <param name="purchasingFilialeId">采购公司ID </param>
        /// <param name="isOut"></param>
        void PurchasingUpdate(Guid purchasingId, Guid purchasingFilialeId, bool isOut);

        /// <summary>赠品借记单生成的采购单IsOut为false(赠品借记单生成采购专用)
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        void PurchasingUpdateIsOut(Guid purchasingId);

        /// <summary>绑定采购单采购公司  陈重文 ADD 2015-06-30
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        /// <param name="purchasingFilialeId">采购公司</param>
        /// <param name="isOut">IsOut</param>
        /// <returns></returns>
        Boolean UpdatePurchasingFilialeId(Guid purchasingId, Guid purchasingFilialeId, Boolean isOut);

        /// <summary>
        /// 修改采购单的仓库 文雯 ADD 2016-05-11
        /// </summary>
        void PurchasingWarehouseId(Guid purchasingId, Guid warehouseId);

        /// <summary>
        /// 修改采购单的状态为部分完成(采购入库完成时使用)
        /// ADD ww
        /// 2016-08-24
        /// </summary>
        bool PurchasingUpdateStateByPartComplete(Guid purchasingId, PurchasingState purchasingState);

        /// <summary>
        /// 修改采购单的状态为完成(采购入库完成时使用)
        /// ADD ww
        /// 2016-08-24
        /// </summary>
        bool PurchasingUpdateStateByAllComplete(Guid purchasingId, PurchasingState purchasingState);
    }
}

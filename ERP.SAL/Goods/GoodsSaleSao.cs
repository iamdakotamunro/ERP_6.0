using System;
using System.Collections.Generic;
using KeedeGroup.GoodsManageSystem.Public.Model.ERP;
using KeedeGroup.GoodsManageSystem.Public.Model.Table;
using KeedeGroup.GoodsManageSystem.Public.Model.RequestModel;

namespace ERP.SAL.Goods
{
    public partial class GoodsCenterSao 
    {
        #region [申请商品卖库存]

        /// <summary>申请商品卖库存
        /// </summary>
        /// <param name="goodSaleStockInfo"></param>
        /// <param name="personId"> </param>
        /// <param name="errorMessage"></param>
        /// <param name="operatorName"> </param>
        /// <returns></returns>
        public bool ApplyGoodsSaleStock(Model.Goods.GoodsSaleStockInfo goodSaleStockInfo, string operatorName, Guid personId, out string errorMessage)
        {
            errorMessage = string.Empty;
            var operationModel = new OperationModel { Operator = operatorName, PersonId = personId };
            var result = GoodsServerClient.ApplyGoodsSaleStock(ConvertToGoodsSaleStaockInfo(goodSaleStockInfo), operationModel);
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region [审核商品卖库存]

        /// <summary>审核商品卖库存
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="saleStockState"></param>
        /// <param name="operatorName"> </param>
        /// <param name="auditor"></param>
        /// <param name="auditReason"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool AuditGoodsSaleStock(Guid goodsId, int saleStockState, string operatorName, Guid auditor, string auditReason, out string errorMessage)
        {
            errorMessage = string.Empty;
            var operationModel = new OperationModel { Operator = operatorName, PersonId = auditor };
            var result = GoodsServerClient.AuditGoodsSaleStock(goodsId, (KeedeGroup.GoodsManageSystem.Public.Enum.SaleStockState)saleStockState, auditor, auditReason, operationModel);
            if (result == null) errorMessage = "GMS连接异常";
            else if (!result.IsSuccess) errorMessage = result.ErrorMsg;
            return result != null && result.IsSuccess;
        }
        #endregion

        #region [根据商品ID获取卖库存信息]
        /// <summary>根据商品ID获取卖库存信息
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        public Model.Goods.GoodsSaleStockInfo GetGoodsSaleStockInfoByGoodsId(Guid goodsId)
        {
            var result = GoodsServerClient.GetGoodsSaleStockInfoByGoodsId(goodsId);
            if (result != null && result.IsSuccess)
            {
                return result.Data == null ? null : ConvertToGoodSaleStaockInfo(result.Data);
            }
            return null;
        }
        #endregion


        #region [搜索卖库存商品(分页)]
        /// <summary> 搜索卖库存商品(分页)
        /// </summary>
        /// <param name="goodsNameOrCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public IList<Model.Goods.GoodsSaleStockGridModel> GetGoodsSaleStockListToPage(string goodsNameOrCode, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = 0;
            IList<Model.Goods.GoodsSaleStockGridModel> list = new List<Model.Goods.GoodsSaleStockGridModel>();
            var searchModel = new GoodsSaleStockSearchModel
            {
                Page = pageIndex,
                PageSize = pageSize,
                GoodsNameOrCode = goodsNameOrCode,
                State = KeedeGroup.GoodsManageSystem.Public.Enum.SaleStockState.Apply
            };

            var result = GoodsServerClient.GetGoodsSaleStockList(searchModel);
            if (result != null && result.IsSuccess)
            {
                var items = result.Data ?? new List<GoodsSaleStockGridModel>();
                foreach (var info in items)
                {
                    list.Add(new Model.Goods.GoodsSaleStockGridModel
                                 {
                                     GoodsId = info.GoodsId,
                                     GoodsName = info.GoodsName,
                                     GoodsCode = info.GoodsCode,
                                     GoodsType = info.GoodsType,
                                     OldSaleStockType = info.OldSaleStockType,
                                     NewSaleStockType = info.NewSaleStockType
                                 });
                }
                totalCount = result.Total;
            }
            return list;
        }
        #endregion

        #region [模型转换]

        #region -->    (GMS)GoodsSaleStockInfo 转换>>  (ERP) GoodSaleStockInfo
        static Model.Goods.GoodsSaleStockInfo ConvertToGoodSaleStaockInfo(GoodsSaleStockInfo info)
        {
            var goodSaleStockInfo = new Model.Goods.GoodsSaleStockInfo
                                        {
                                            Applicant = new Guid(info.Applicant == null ? Guid.Empty.ToString() : info.Applicant.ToString()),
                                            ApplyReason = info.ApplyReason,
                                            ApplyTime = Convert.ToDateTime(info.ApplyTime ?? DateTime.MinValue),
                                            Auditor = new Guid(info.Auditor == null ? Guid.Empty.ToString() : info.Auditor.ToString()),
                                            AuditReason = info.AuditReason,
                                            AuditTime = Convert.ToDateTime(info.AuditTime ?? DateTime.MinValue),
                                            GoodsId = info.GoodsId,
                                            GoodsName = info.GoodsName,
                                            GoodsCode = info.GoodsCode,
                                            ReplenishmentCycle = Convert.ToInt32(info.ReplenishmentCycle ?? -1),
                                            SaleStockType = info.SaleStockType,
                                            SaleStockState = info.State
                                        };
            return goodSaleStockInfo;
        }
        #endregion

        #region -->    (ERP) GoodSaleStockInfo  转换>>  (GMS)GoodsSaleStockInfo
        static GoodsSaleStockInfo ConvertToGoodsSaleStaockInfo(Model.Goods.GoodsSaleStockInfo info)
        {
            var goodSaleStockInfo = new GoodsSaleStockInfo
            {
                Applicant = info.Applicant,
                ApplyReason = info.ApplyReason,
                ApplyTime = info.ApplyTime,
                Auditor = info.Auditor,
                AuditReason = info.AuditReason,
                AuditTime = info.AuditTime,
                GoodsName = info.GoodsName,
                GoodsCode = info.GoodsCode,
                GoodsId = info.GoodsId,
                ReplenishmentCycle = info.ReplenishmentCycle,
                SaleStockType = info.SaleStockType,
            };
            return goodSaleStockInfo;
        }
        #endregion

        #endregion
    }
}

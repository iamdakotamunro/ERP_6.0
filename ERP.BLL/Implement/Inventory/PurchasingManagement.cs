using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IOrder;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>
    /// by jiang 2011-05-06
    /// </summary>
    public class PurchasingManagement : BllInstance<PurchasingManagement>
    {
        readonly IGoodsCenterSao _goodsCenterSao;
        private readonly IPersonnelSao _personnelSao;
        private readonly IGoodsOrderDetail _goodsOrderDetail = new GoodsOrderDetail(Environment.GlobalConfig.DB.FromType.Read);

        public PurchasingManagement(Environment.GlobalConfig.DB.FromType fromType)
        {
            _goodsCenterSao = new GoodsCenterSao();
            _personnelSao = new PersonnelSao();
        }

        public PurchasingManagement(IPersonnelSao personnelSao, IGoodsCenterSao goodsCenterSao)
        {
            _goodsCenterSao = goodsCenterSao;
            _personnelSao = personnelSao;
        }

        /// <summary>
        /// 统计需调拨商品
        /// </summary>
        /// <param name="pmId"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public IList<StorageRecordDetailInfo> GetAllocationGoodsList(Guid pmId, DateTime starttime, DateTime endtime, Guid warehouseId)
        {
            IList<StorageRecordDetailInfo> datasource = new List<StorageRecordDetailInfo>();

            var orderNeedeGoods = _goodsOrderDetail.GetNeedPurchasingGoodses(warehouseId, pmId, starttime, endtime, new List<int> { (int)Enum.OrderState.RequirePurchase });
            if (orderNeedeGoods.Count > 0)
            {
                // 获取商品的可用库存
                Dictionary<Guid, int> stockQuantitys = WMSSao.GetLackQuantity(warehouseId, orderNeedeGoods.Select(ent => ent.RealGoodsId).Distinct());
                List<NeedPurchasingGoods> details = GetList(orderNeedeGoods, stockQuantitys);
                if (details.Any())
                {
                    var goodsDics = _goodsCenterSao.GetDictRealGoodsUnitModel(details.Select(ent => ent.RealGoodsId).Distinct().ToList());
                    foreach (var dic in details.GroupBy(ent=>ent.RealGoodsId))
                    {
                        var goods = goodsDics != null && goodsDics.ContainsKey(dic.Key) ? goodsDics[dic.Key] : null;
                        if (goods == null) continue;
                        datasource.Add(new StorageRecordDetailInfo
                        {
                            GoodsCode = goods.GoodsCode,
                            GoodsName = goods.PurchaseName,
                            Specification = goods.Specification,
                            Quantity = dic.Sum(ent=>ent.Quantity)
                        });
                    }
                }
            }
            
            return datasource;
        }

        /// <summary>
        /// 统计需调拨订单
        /// </summary>
        /// <param name="pmId"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public IList<GoodsOrderInfo> GetAllocationOrdersList(Guid pmId, DateTime starttime, DateTime endtime, Guid warehouseId)
        {
            IList<GoodsOrderInfo> goodsOrderInfos = new List<GoodsOrderInfo>();
            var orderNeedeGoods = _goodsOrderDetail.GetNeedPurchasingGoodses(warehouseId, pmId, starttime, endtime, new List<int> { (int)Enum.OrderState.RequirePurchase });
            if (orderNeedeGoods.Count > 0)
            {
                var orderBaseList = _goodsOrderDetail.GetGoodsQuantityDics(warehouseId, starttime, endtime, pmId);
                // 获取商品的可用库存
                Dictionary<Guid, int> stockQuantitys = WMSSao.GetLackQuantity(warehouseId, orderNeedeGoods.Select(ent => ent.RealGoodsId).Distinct());
                List<NeedPurchasingGoods> details = GetList(orderNeedeGoods, stockQuantitys);
                var orderNos = details.Select(ent => ent.OrderNo).Distinct();
                var filialeDics=MISService.GetAllFiliales().ToDictionary(k=>k.ID,v=>v.Name);
                foreach (var order in orderBaseList.Where(ent=>orderNos.Contains(ent.OrderNo)))
                {
                    if(goodsOrderInfos.Any(ent=>ent.OrderNo==order.OrderNo))continue;
                    goodsOrderInfos.Add(new GoodsOrderInfo
                    {
                        OrderNo = order.OrderNo,
                        Consignee = order.Consignee,
                        EffectiveTime = order.EffectiveTime,
                        Memo = filialeDics.ContainsKey(order.SaleFilialeId) ? filialeDics[order.SaleFilialeId] : "",
                        SaleFilialeId = order.SaleFilialeId
                    });
                }
            }
            return goodsOrderInfos;
        }

        /// <summary>
        /// 按时间取出缺货商品数和缺货订单数
        /// </summary>
        /// <param name="warehouseName"></param>
        /// <param name="personResponsible"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public IList<GoodsAllocateStatistic> GetNeedeGoodsOrderList(Guid warehouseId,string warehouseName,Guid personResponsible, DateTime starttime, DateTime endtime)
        {
            var result = new List<GoodsAllocateStatistic>();
            var orderNeedeGoods = _goodsOrderDetail.GetNeedPurchasingGoodses(warehouseId, personResponsible,starttime, endtime, new List<int> { (int)Enum.OrderState.RequirePurchase });
            if (orderNeedeGoods.Count>0)
            {
                // 缺货数
                Dictionary<Guid, int> stockQuantitys = WMSSao.GetLackQuantity(warehouseId, orderNeedeGoods.Select(ent => ent.RealGoodsId).Distinct());
                foreach (var personResponsibleGroup in orderNeedeGoods.GroupBy(ent => ent.PersonResponsible))
                {
                    var allocateStatistic = new GoodsAllocateStatistic
                    {
                        PersonResponsible = personResponsible,
                        PersonnelName = _personnelSao.GetName(personResponsibleGroup.Key),
                        WarehouseId = warehouseId,
                        WarehouseName = warehouseName
                    };
                    
                    List<NeedPurchasingGoods> details=new List<NeedPurchasingGoods>();
                    foreach (var needPurchasingGoodses in personResponsibleGroup.Where(ent => stockQuantitys.ContainsKey(ent.RealGoodsId)))
                    {
                        if (stockQuantitys[needPurchasingGoodses.RealGoodsId] < 0)
                        {
                            int quantity = needPurchasingGoodses.Quantity;
                            if (quantity > Math.Abs(stockQuantitys[needPurchasingGoodses.RealGoodsId]))
                            {
                                needPurchasingGoodses.Quantity = quantity + stockQuantitys[needPurchasingGoodses.RealGoodsId];
                            }
                            stockQuantitys[needPurchasingGoodses.RealGoodsId] = stockQuantitys[needPurchasingGoodses.RealGoodsId] + quantity;
                            details.Add(needPurchasingGoodses);
                        }
                    }
                    allocateStatistic.OrderCount= details.Select(ent => ent.OrderNo).Distinct().Count();
                    allocateStatistic.GoodsQuantities = details.Sum(ent => ent.Quantity);
                    result.Add(allocateStatistic);
                }
            }
            return result;
        }

        public List<NeedPurchasingGoods> GetList(List<NeedPurchasingGoods> personResponsibleGroup,Dictionary<Guid,int> stockQuantitys)
        {
            List<NeedPurchasingGoods> details = new List<NeedPurchasingGoods>();
            foreach (var needPurchasingGoodses in personResponsibleGroup.Where(ent => stockQuantitys.ContainsKey(ent.RealGoodsId)))
            {
                if (stockQuantitys[needPurchasingGoodses.RealGoodsId] < 0)
                {
                    int quantity = needPurchasingGoodses.Quantity;
                    if (quantity > Math.Abs(stockQuantitys[needPurchasingGoodses.RealGoodsId]))
                    {
                        needPurchasingGoodses.Quantity = quantity + stockQuantitys[needPurchasingGoodses.RealGoodsId];
                    }
                    stockQuantitys[needPurchasingGoodses.RealGoodsId] = stockQuantitys[needPurchasingGoodses.RealGoodsId] + quantity;
                    details.Add(needPurchasingGoodses);
                }
            }
            return details;
        }
    }
}

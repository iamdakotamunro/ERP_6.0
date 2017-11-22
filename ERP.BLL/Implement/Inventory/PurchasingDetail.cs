using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using Keede.Ecsoft.Model;
using ERP.Model;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>
    /// 2011-04-20  by jiang
    /// </summary>
    public class PurchasingDetailManager : BllInstance<PurchasingDetailManager>
    {
        readonly IPurchasingDetail _purchasingDetailDao;
        private readonly IPurchasing _purchasingDao;

        public PurchasingDetailManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _purchasingDao = new DAL.Implement.Inventory.Purchasing(fromType);
            _purchasingDetailDao = InventoryInstance.GetPurchasingDetailDao(fromType);
        }

        public PurchasingDetailManager(IPurchasingDetail purchasingDetailDao,
            IPurchasing purchasingDao)
        {
            _purchasingDao = purchasingDao;
            _purchasingDetailDao = purchasingDetailDao;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="info"></param>
        public void Insert(PurchasingDetailInfo info)
        {

            if ((int)PurchasingGoodsType.Gift == info.PurchasingGoodsType)
            {
                info.Price = 0;
                #region -- 赠品
                IList<PurchasingDetailInfo> ilist = _purchasingDetailDao.Select(info.PurchasingID).Where(p => p.GoodsID == info.GoodsID).Where(p => p.PurchasingGoodsType == (int)PurchasingGoodsType.Gift).ToList();
                if (ilist.Count == 0)
                {
                    //为赠品并且数据库采购明细表里面不存在赠品的商品
                    _purchasingDetailDao.Insert(info);
                }
                else
                {
                    //存在数据库里面采购明细表里面存在赠品
                    PurchasingDetailInfo pdInfo = ilist[0];
                    pdInfo.PlanQuantity = pdInfo.PlanQuantity + info.PlanQuantity;
                    _purchasingDetailDao.Update(pdInfo, pdInfo.PurchasingGoodsID);
                }
                #endregion
            }
            else
            {
                #region -- 非赠品
                IList<PurchasingDetailInfo> ilist = _purchasingDetailDao.Select(info.PurchasingID).Where(p => p.GoodsID == info.GoodsID).Where(p => p.PurchasingGoodsType != (int)PurchasingGoodsType.Gift).ToList();
                if (ilist.Count == 0)
                {
                    //采购明细里面不存在非赠品,直接插入
                    _purchasingDetailDao.Insert(info);
                }
                else
                {
                    //采购明细里面存在非赠品,直接修改采购计划数量
                    PurchasingDetailInfo pdInfo = ilist[0];
                    pdInfo.PlanQuantity = pdInfo.PlanQuantity + info.PlanQuantity;
                    _purchasingDetailDao.Update(pdInfo, pdInfo.PurchasingGoodsID);//修改数量
                }
                #endregion
            }
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="plist"></param>
        public void Save(IList<PurchasingDetailInfo> plist)
        {
            foreach (PurchasingDetailInfo detailInfo in plist)
            {
                Insert(detailInfo);
            }
        }

        /// <summary>
        /// 更改供应商的操作
        /// </summary>
        /// <param name="purasingId"></param>
        /// <param name="purasingGoodsId"></param>
        /// <param name="companyId"></param>
        /// <param name="companyName"></param>
        /// <param name="personnelInfo">当前操作人信息</param>
        /// <returns></returns>
        public string UpdatePurchsingCompany(Guid purasingId, Guid purasingGoodsId, Guid companyId, string companyName, PersonnelInfo personnelInfo)
        {
            PurchasingInfo pInfo = _purchasingDao.GetPurchasingById(purasingId);
            PurchasingDetailInfo dInfo = _purchasingDetailDao.GetPurchGoodsId(purasingId, purasingGoodsId);
            //查出未提交下的该仓库下的类别下的未提交订单下的供应商
            IList<PurchasingInfo> dlist = _purchasingDao.GetPurchasingList(DateTime.MinValue, DateTime.MinValue, companyId, pInfo.WarehouseID, pInfo.PurchasingFilialeId,
                PurchasingState.NoSubmit, (PurchasingType)System.Enum.Parse(typeof(PurchasingType), string.Format("{0}", pInfo.PurchasingType)), "", Guid.Empty, Guid.Empty).Where(act=>act.FilialeID==pInfo.FilialeID).ToList();
            if (dlist.Count >= 1)//如果存在改供应商的采购单就往改采购单里面添加商品
            {
                //如果存在多个默认取第一个
                PurchasingInfo oldPinfo = dlist[0];
                IList<PurchasingDetailInfo> pdList = _purchasingDetailDao.Select(oldPinfo.PurchasingID);//获取该采购单下面的商品集合
                bool flag = false;
                foreach (PurchasingDetailInfo detailInfo in pdList)
                {
                    try
                    {
                        //如果该采购单下存在同商品id同价格同规格的商品那就累加数量
                        if (dInfo.GoodsID == detailInfo.GoodsID && dInfo.GoodsCode == detailInfo.GoodsCode && dInfo.Specification == detailInfo.Specification && dInfo.Price == detailInfo.Price)
                        {
                            detailInfo.PlanQuantity = detailInfo.PlanQuantity + dInfo.PlanQuantity - dInfo.RealityQuantity;
                            _purchasingDetailDao.UpdateRealQuantity(detailInfo, detailInfo.PurchasingGoodsID);
                            flag = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        SAL.LogCenter.LogService.LogError(string.Format("采购单更新供应商异常!purasingId={0}, purasingGoodsId={1}, companyId={2}, companyName={3}", purasingId, purasingGoodsId, companyId, companyName), "采购管理", ex);
                    }
                }
                if (flag == false)
                {
                    _purchasingDetailDao.DeleteByGoodsId(dInfo.PurchasingID, dInfo.GoodsID, dInfo.PurchasingGoodsID);
                    dInfo.PurchasingID = oldPinfo.PurchasingID;
                    dInfo.CompanyID = companyId;
                    //没有改商品直接插入改商品
                    Insert(dInfo);
                }
            }
            else
            {
                Guid pid = Guid.NewGuid();
                var sInfo = new PurchasingInfo(pid, new CodeManager().GetCode(CodeType.PH), companyId, companyName, pInfo.FilialeID, pInfo.WarehouseID, (int)PurchasingState.NoSubmit
                         , pInfo.PurchasingType, DateTime.Now, DateTime.MaxValue, "[更改供应商,操作人:" + personnelInfo.RealName + "]")
                {
                    PmId = pInfo.PmId,
                    ArrivalTime = pInfo.ArrivalTime,
                    Director = pInfo.Director,
                    PersonResponsible = personnelInfo.PersonnelId,
                    PurchasingFilialeId = pInfo.PurchasingFilialeId,
                    //IsOut = true
                };
                _purchasingDao.PurchasingInsert(sInfo);
                _purchasingDetailDao.DeleteByGoodsId(dInfo.PurchasingID, dInfo.GoodsID, dInfo.PurchasingGoodsID);
                dInfo.PurchasingID = pid;
                dInfo.CompanyID = companyId;
                Insert(dInfo);
            }
            return "";
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <returns></returns>
        public List<PurchasingDetailInfo> SelectDetail(Guid purchasingId)
        {
            var list = _purchasingDetailDao.SelectDetail(purchasingId);
            return list;
        }

    }
}

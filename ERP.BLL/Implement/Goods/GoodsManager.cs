using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using OperationLog.Core;
using OperationLog.Core.Attributes;

namespace ERP.BLL.Implement.Goods
{
    public class GoodsManager : BllInstance<GoodsManager>
    {

        private readonly Interface.IOperationLogManager _operationLogManager;
        private readonly IPurchaseSet _purchaseSetManager;
        private readonly IPersonnelSao _personnelSao;
        private readonly IGoodsCenterSao _goodsInfoSao;

        public GoodsManager(Environment.GlobalConfig.DB.FromType fromType = Environment.GlobalConfig.DB.FromType.Read)
        {
            _operationLogManager = new OperationLogManager();
            _personnelSao = new PersonnelSao();
            _goodsInfoSao = new GoodsCenterSao();
            _purchaseSetManager = new PurchaseSet(fromType);
        }

        /// <summary>
        /// for 删除
        /// </summary>
        /// <param name="goodsInfoSao"></param>
        /// <param name="purchaseSet"></param>
        public GoodsManager(IGoodsCenterSao goodsInfoSao, IPurchaseSet purchaseSet)
        {
            _goodsInfoSao = goodsInfoSao;
            _purchaseSetManager = purchaseSet;
            _personnelSao = new PersonnelSao();
            _operationLogManager = new OperationLogManager();
        }

        #region --> 添加主商品

        /// <summary>
        /// 添加主商品
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="goodsInfo"></param>
        /// <param name="fields"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        public bool Add(Guid operatorId, GoodsInfo goodsInfo, Dictionary<Guid, List<Guid>> fields, out string failMessage)
        {
            var success = _goodsInfoSao.AddGoods(goodsInfo, fields, out failMessage);
            if (success)
            {
                AddGoodsSettingOperation(operatorId, goodsInfo.GoodsId, goodsInfo.GoodsCode, OperationPoint.GoodsSettingManager.Add.GetBusinessInfo());
            }
            return success;
        }
        #endregion

        #region --> 修改主商品

        /// <summary>
        /// 修改主商品
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="goodsInfo"></param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        public bool Update(Guid operatorId, GoodsInfo goodsInfo, out string failMessage)
        {
            var success = _goodsInfoSao.UpdateGoods(goodsInfo, out failMessage);
            if (success)
            {
                AddGoodsSettingOperation(operatorId, goodsInfo.GoodsId, goodsInfo.GoodsCode, OperationPoint.GoodsSettingManager.Edit.GetBusinessInfo());
            }
            return success;
        }

        /// <summary>
        /// 修改主商品审核状态和商品审核备注
        /// </summary>
        /// <param name="goodsId">商品Id</param>
        /// <param name="goodsState">商品审核状态(0:审核未通过;1:待采购经理审核;2:待质管部审核;3:待负责人终审;4:审核通过;)</param>
        /// <param name="goodsStateMemo">商品审核备注</param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        /// zal 2016-04-21
        public bool UpdateGoodsAuditStateAndAuditStateMemo(Guid goodsId, int goodsState, string goodsStateMemo, out string failMessage)
        {
            return _goodsInfoSao.UpdateGoodsAuditStateAndAuditStateMemo(goodsId, goodsState, goodsStateMemo, out failMessage);
        }

        /// <summary>
        /// 批量修改主商品审核状态和商品审核备注
        /// </summary>
        /// <param name="goodsId">商品Id集合</param>
        /// <param name="goodsState">商品审核状态(0:审核未通过;1:待采购经理审核;2:待质管部审核;3:待负责人终审;4:审核通过;)</param>
        /// <param name="goodsStateMemo">商品审核备注</param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        /// ww 2016-06-29
        public bool PlUpdateGoodsAuditStateAndAuditStateMemo(List<Guid> goodsId, int goodsState, string goodsStateMemo, out string failMessage)
        {
            return _goodsInfoSao.PlUpdateGoodsAuditStateAndAuditStateMemo(goodsId, goodsState, goodsStateMemo, out failMessage);
        }

        #endregion

        #region --> [私有] 添加商品属性操作添加到管理系统——自动

        /// <summary>
        /// 添加商品属性操作添加到管理系统——自动
        /// </summary>
        /// <param name="personnelId"></param>
        /// <param name="goodsId"></param>
        /// <param name="goodsCode"></param>
        /// <param name="pointAttribute"> </param>
        void AddGoodsSettingOperation(Guid personnelId, Guid goodsId, string goodsCode, EnumPointAttribute pointAttribute)
        {
            var realName = _personnelSao.GetName(personnelId);
            var success = _operationLogManager.Add(personnelId, realName, goodsId, goodsCode, pointAttribute, 1, string.Empty);
            if (!success)
            {
                SAL.LogCenter.LogService.LogError(string.Format("添加商品属性操作添加到管理系统——自动，GoodsCode={0}", goodsCode), "商品管理", null);
            }
        }
        #endregion

        #region --> 根据主商品ID获取所有子商品
        /// <summary>
        /// 根据主商品ID获取所有子商品
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        public IList<ChildGoodsInfo> GetChildGoodsListByGoodsId(List<Guid> goodsIds)
        {
            var list = _goodsInfoSao.GetRealGoodsListByGoodsId(goodsIds);
            return list.OrderBy(ent => ent.GoodsId).ThenBy(w => w.Specification).ToList();
        }
        #endregion

        #region --> 删除主商品信息

        /// <summary>
        /// 删除主商品信息
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <param name="personId"> </param>
        /// <param name="errorMessage"></param>
        /// <param name="operatorName"> </param>
        /// <returns></returns>
        public bool DeleteGoods(List<Guid> goodsIds, string operatorName, Guid personId, out string errorMessage)
        {
            var result = _goodsInfoSao.DeleteGoods(goodsIds, operatorName, personId, out errorMessage);
            if (result)
            {
                //删除商品采购责任人绑定
                _purchaseSetManager.NewDeletePurchaseSet(goodsIds);
            }
            return result;
        }
        #endregion

    }
}

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
        /// for ɾ��
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

        #region --> �������Ʒ

        /// <summary>
        /// �������Ʒ
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

        #region --> �޸�����Ʒ

        /// <summary>
        /// �޸�����Ʒ
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
        /// �޸�����Ʒ���״̬����Ʒ��˱�ע
        /// </summary>
        /// <param name="goodsId">��ƷId</param>
        /// <param name="goodsState">��Ʒ���״̬(0:���δͨ��;1:���ɹ��������;2:���ʹܲ����;3:������������;4:���ͨ��;)</param>
        /// <param name="goodsStateMemo">��Ʒ��˱�ע</param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        /// zal 2016-04-21
        public bool UpdateGoodsAuditStateAndAuditStateMemo(Guid goodsId, int goodsState, string goodsStateMemo, out string failMessage)
        {
            return _goodsInfoSao.UpdateGoodsAuditStateAndAuditStateMemo(goodsId, goodsState, goodsStateMemo, out failMessage);
        }

        /// <summary>
        /// �����޸�����Ʒ���״̬����Ʒ��˱�ע
        /// </summary>
        /// <param name="goodsId">��ƷId����</param>
        /// <param name="goodsState">��Ʒ���״̬(0:���δͨ��;1:���ɹ��������;2:���ʹܲ����;3:������������;4:���ͨ��;)</param>
        /// <param name="goodsStateMemo">��Ʒ��˱�ע</param>
        /// <param name="failMessage"></param>
        /// <returns></returns>
        /// ww 2016-06-29
        public bool PlUpdateGoodsAuditStateAndAuditStateMemo(List<Guid> goodsId, int goodsState, string goodsStateMemo, out string failMessage)
        {
            return _goodsInfoSao.PlUpdateGoodsAuditStateAndAuditStateMemo(goodsId, goodsState, goodsStateMemo, out failMessage);
        }

        #endregion

        #region --> [˽��] �����Ʒ���Բ�����ӵ�����ϵͳ�����Զ�

        /// <summary>
        /// �����Ʒ���Բ�����ӵ�����ϵͳ�����Զ�
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
                SAL.LogCenter.LogService.LogError(string.Format("�����Ʒ���Բ�����ӵ�����ϵͳ�����Զ���GoodsCode={0}", goodsCode), "��Ʒ����", null);
            }
        }
        #endregion

        #region --> ��������ƷID��ȡ��������Ʒ
        /// <summary>
        /// ��������ƷID��ȡ��������Ʒ
        /// </summary>
        /// <param name="goodsIds"></param>
        /// <returns></returns>
        public IList<ChildGoodsInfo> GetChildGoodsListByGoodsId(List<Guid> goodsIds)
        {
            var list = _goodsInfoSao.GetRealGoodsListByGoodsId(goodsIds);
            return list.OrderBy(ent => ent.GoodsId).ThenBy(w => w.Specification).ToList();
        }
        #endregion

        #region --> ɾ������Ʒ��Ϣ

        /// <summary>
        /// ɾ������Ʒ��Ϣ
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
                //ɾ����Ʒ�ɹ������˰�
                _purchaseSetManager.NewDeletePurchaseSet(goodsIds);
            }
            return result;
        }
        #endregion

    }
}

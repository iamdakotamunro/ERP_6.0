using System;
using System.Collections.Generic;
using System.Linq;
using AllianceShop.Contract.DataTransferObject;
using ERP.BLL.Implement.Basis;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.Cache;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Goods;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IGoods;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using Keede.Ecsoft.Model;
using ERP.Enum;
using ERP.Model;
using ERP.SAL;
using GoodsOrderDetail = ERP.DAL.Implement.Order.GoodsOrderDetail;

namespace ERP.BLL.Implement
{
    /// <summary>�{��ߩרTһ �첽������ҵ���  ����޸��ύ  ������  2014-12-25  ��ȫ���Ż����£�
    /// </summary>
    public class ReckoningManager : BllInstance<ReckoningManager>
    {
        private readonly CodeManager _codeManager;
        readonly IReckoning _reckoningDao;
        private readonly ICompanyCussent _companyCussent;
        private readonly IGoodsOrder _goodsOrder;
        private readonly IGoodsOrderDetail _goodsOrderDetail;
        readonly IGoodsOrderDeliver _goodsOrderDeliver;
        readonly IStorageRecordDao _storageRecordDao;

        private static readonly WMSSao _wmsSao = new WMSSao();

        public ReckoningManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _companyCussent = new CompanyCussent(fromType);
            _reckoningDao = InventoryInstance.GetReckoningDao(fromType);
            _goodsOrderDeliver = new GoodsOrderDeliver(fromType);
            _goodsOrder = new DAL.Implement.Order.GoodsOrder(fromType);
            _goodsOrderDetail = new GoodsOrderDetail(fromType);
            _codeManager = new CodeManager();
            _storageRecordDao= new StorageRecordDao(fromType);
        }

        public ReckoningManager(IReckoning reckoning, ICompanyCussent companyCussent,
            IGoodsOrder goodsOrder, IGoodsOrderDetail goodsOrderDetail, IGoodsOrderDeliver goodsOrderDeliver,
            ICode code)
        {
            _reckoningDao = reckoning;
            _companyCussent = companyCussent;
            _goodsOrder = goodsOrder;
            _goodsOrderDetail = goodsOrderDetail;
            _goodsOrderDeliver = goodsOrderDeliver;
            _codeManager = new CodeManager(code);
        }

        #region [���ɶ�Ӧ������]

        /// <summary>�첽��ɶ�������������
        /// </summary>
        /// <param name="orderInfo">����</param>
        /// <param name="orderDetailList">������ϸ</param>
        /// <param name="errorMessage">�쳣��Ϣ</param>
        /// <returns></returns>
        public bool AddByCompleteOrder(GoodsOrderInfo orderInfo, IList<GoodsOrderDetailInfo> orderDetailList, out string errorMessage)
        {
            GoodsOrderDeliverInfo goodsOrderDeliverInfo;
            var reckoningList = NewCreateReckoningInfoList(orderInfo, orderDetailList, out goodsOrderDeliverInfo, out errorMessage).ToList();
            if (reckoningList.Count == 0)
            {
                errorMessage = "�˶����������������ʣ�";
                return true;
            }

            using (var tran = new System.Transactions.TransactionScope())
            {
                //����������
                foreach (var reckoning in reckoningList)
                {
                    if (FilialeManager.IsEntityShopFiliale(reckoning.FilialeId))
                    {
                        if (!AddReckoningToEntityShop(reckoning, out errorMessage))
                        {
                            errorMessage = "�����ŵ�������ʧ�ܣ�" + errorMessage + "\r\n���ݣ�" + new Framework.Core.Serialize.JsonSerializer().Serialize(reckoning);
                            return false;
                        }
                    }
                    else
                    {
                        var success = _reckoningDao.Insert(reckoning, out errorMessage);
                        if (!success)
                        {
                            errorMessage = "����������ʧ�ܣ�" + errorMessage + "\r\n���ݣ�" + new Framework.Core.Serialize.JsonSerializer().Serialize(reckoning);
                            return false;
                        }
                    }
                    
                }
                if (goodsOrderDeliverInfo != null)
                {
                    _goodsOrderDeliver.DeleteOrderDeliver(goodsOrderDeliverInfo.OrderId);
                    var result = _goodsOrderDeliver.InsertOrderDeliver(goodsOrderDeliverInfo);
                    if (!result)
                    {
                        errorMessage = "���붩������˷���Ϣʧ�ܣ�" + errorMessage + "\r\n���ݣ�" + new Framework.Core.Serialize.JsonSerializer().Serialize(goodsOrderDeliverInfo);
                        return false;
                    }
                }
                tran.Complete();
                return true;
            }
        }

        /// <summary>��������ʵ�ʵ���ϵͳ��
        /// </summary>
        /// <param name="reckoningInfo"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool AddReckoningToEntityShop(ReckoningInfo reckoningInfo, out string errorMsg)
        {
            errorMsg = string.Empty;
            var info = new ReckoningRecordDTO
            {
                AccountReceivable = reckoningInfo.AccountReceivable,
                JoinTotalPrice = reckoningInfo.JoinTotalPrice,
                AuditingState = true,
                CompanyID = reckoningInfo.ThirdCompanyID,
                DateCreated = reckoningInfo.DateCreated,
                Description = reckoningInfo.Description,
                ShopID = reckoningInfo.FilialeId,
                NonceTotalled = reckoningInfo.NonceTotalled,
                OriginalTradeCode = reckoningInfo.LinkTradeCode,
                ID = reckoningInfo.ReckoningId,
                ReckoningType = reckoningInfo.ReckoningType,
                TradeCode = reckoningInfo.TradeCode
            };
            if (info.CompanyID == info.ShopID)
            {
                errorMsg = ("������˾ID����Ŀ��˾IDһ�������޷����������ʣ�");
                return false;
            }
            if (!FilialeManager.IsEntityShopFiliale(info.ShopID))
            {
                errorMsg = ("������˾ID�����ŵ깫˾�����޷����������ʣ�");
                return false;
            }
            var headFililaleId = FilialeManager.GetShopHeadFilialeId(info.ShopID);
            var result = PushManager.AddToShop(headFililaleId, "InsertReckoningWithPush", info.TradeCode, info);
            if (!result)
            {
                errorMsg = ("�����ŵ��ERPӦ����ʧ�ܣ�");
                return false;
            }
            return true;
        }
        #endregion

        #region [��ɶ����������������˼���]

        /// <summary>�¼ܹ���ɶ�����������������   2015-01-19  ������  
        /// </summary>
        /// <param name="goodsOrderInfo">������Ϣ</param>
        /// <param name="goodsOrderDetailInfoList">������ϸ</param>
        /// <param name="goodsOrderDeliverInfo">�����˷���Ϣ</param>
        /// <param name="errorMsg">������Ϣ</param>
        /// <returns></returns>
        public IEnumerable<ReckoningInfo> NewCreateReckoningInfoList(GoodsOrderInfo goodsOrderInfo, IList<GoodsOrderDetailInfo> goodsOrderDetailInfoList,out GoodsOrderDeliverInfo goodsOrderDeliverInfo, out string errorMsg)
        {
            if (goodsOrderInfo.HostingFilialeId == Guid.Empty)
            {
                goodsOrderInfo.HostingFilialeId = WMSSao.GetHostingFilialeIdByWarehouseIdGoodsTypes(goodsOrderInfo.DeliverWarehouseId, goodsOrderInfo.SaleFilialeId, goodsOrderDetailInfoList.Select(ent => ent.GoodsType).Distinct());
            }
            //������ӵ������ʼ���
            IList<ReckoningInfo> reckoningList = new List<ReckoningInfo>();
            goodsOrderDeliverInfo = null;
            var orderCarriageInfo = _wmsSao.GetOrderNoCarriage(goodsOrderInfo.OrderNo, out errorMsg);
            if (orderCarriageInfo == null)
            {
                return reckoningList;
            }
            var carriage = orderCarriageInfo.Carriage;
            if (carriage != 0)
            {
                goodsOrderDeliverInfo = new GoodsOrderDeliverInfo
                {
                    OrderId = goodsOrderInfo.OrderId,
                    TotalWeight = orderCarriageInfo.PackageWeight == 0 ? 0 : Convert.ToDouble(orderCarriageInfo.PackageWeight) / 1000,
                    CarriageFee = Convert.ToDouble(orderCarriageInfo.Carriage),
                    ExpressId = goodsOrderInfo.ExpressId,
                    ExpressNo = goodsOrderInfo.ExpressNo,
                    MaxWrongValue = 0,
                    ProvinceName = orderCarriageInfo.Province,
                    CityName = orderCarriageInfo.City
                };
            }

            #region [�����������λ��Ϣ]

            Guid companyId = Express.Instance.Get(goodsOrderInfo.ExpressId).CompanyId;
            CompanyCussentInfo expressCompanyInfo = _companyCussent.GetCompanyCussent(companyId);
            if (expressCompanyInfo == null)
            {
                errorMsg = "��ݹ�˾������������Ϣû�н�����";
                return new List<ReckoningInfo>();
            }

            #endregion

            //��ȡ���۹�˾����
            string saleFilialeName = FilialeManager.GetName(goodsOrderInfo.SaleFilialeId);
            string hostingFilialeName = FilialeManager.GetName(goodsOrderInfo.HostingFilialeId);
            #region [�˷�������]
            if (carriage > 0)
            {
                #region [���۹�˾�Կ�ݹ�˾Ӧ������˷���]
                //���۹�˾�Կ�ݹ�˾��Ӧ������˷���
                var saleFilialeToCarriage = new ReckoningInfo
                {
                    ContructType = ContructType.Insert,
                    ReckoningId = Guid.NewGuid(),
                    TradeCode = _codeManager.GetCode(CodeType.PY),
                    DateCreated = DateTime.Now,
                    ReckoningType = (int)ReckoningType.Defray,
                    State = (int)ReckoningStateType.Currently,
                    IsChecked = (int)CheckType.NotCheck,
                    AuditingState = (int)AuditingState.Yes,
                    LinkTradeCode = goodsOrderInfo.ExpressNo,
                    WarehouseId = goodsOrderInfo.DeliverWarehouseId,
                    FilialeId = goodsOrderInfo.HostingFilialeId,
                    ThirdCompanyID = expressCompanyInfo.CompanyId,
                    Description = string.Format("[��ɶ���,{0}�Կ�ݹ�˾{1}�˷�Ӧ����]", hostingFilialeName, expressCompanyInfo.CompanyName),
                    AccountReceivable = -carriage,
                    JoinTotalPrice = -carriage,
                    ReckoningCheckType = (int)ReckoningCheckType.Carriage,
                    IsOut = goodsOrderInfo.IsOut,
                    LinkTradeType = (int)ReckoningLinkTradeType.Express,
                };
                reckoningList.Add(saleFilialeToCarriage);
                #endregion
            }
            #endregion

            #region ���۹�˾�Կ�ݹ�˾�Ķ���������

            if (goodsOrderInfo.PayMode == (int)PayMode.COD || goodsOrderInfo.PayMode == (int)PayMode.COM)
            {
                #region [���۹�˾�Կ�ݹ�˾�Ķ���������]

                //���۹�˾�Կ�ݹ�˾�Ķ���������
                var saleFilialeToRealTotalPrice = new ReckoningInfo
                {
                    ContructType = ContructType.Insert,
                    ReckoningId = Guid.NewGuid(),
                    TradeCode = _codeManager.GetCode(CodeType.GT),
                    DateCreated = DateTime.Now,
                    ReckoningType = (int)ReckoningType.Income,
                    State = (int)ReckoningStateType.Currently,
                    IsChecked = (int)CheckType.NotCheck,
                    AuditingState = (int)AuditingState.Yes,
                    LinkTradeCode = goodsOrderInfo.ExpressNo,
                    WarehouseId = goodsOrderInfo.DeliverWarehouseId,
                    FilialeId = goodsOrderInfo.SaleFilialeId,
                    ThirdCompanyID = expressCompanyInfo.CompanyId,
                    Description = string.Format("[��ɶ���,{0}�Կ�ݹ�˾{1}�Ķ���Ӧ�ջ���]", saleFilialeName, expressCompanyInfo.CompanyName),
                    AccountReceivable = WebRudder.ReadInstance.CurrencyValue(goodsOrderInfo.RealTotalPrice),
                    ReckoningCheckType = (int)ReckoningCheckType.Collection,
                    IsOut = goodsOrderInfo.IsOut,
                    LinkTradeType = (int)ReckoningLinkTradeType.GoodsOrder,
                };

                reckoningList.Add(saleFilialeToRealTotalPrice);
                #endregion
            }
            #endregion

            return reckoningList;
        }

        #endregion

        #region [�첽�����˲���]

        /// <summary>ִ���첽���������
        /// </summary>
        /// <param name="readCount"></param>
        public void RunAsynAddTask(int readCount)
        {
            var asynList = _reckoningDao.GetAsynList(readCount);
            foreach (var asynInfo in asynList)
            {
                if (asynInfo.ReckoningFromType == Enum.ASYN.ASYNReckoningFromType.CompleteOrder.ToString())
                {
                    using (var tran = new System.Transactions.TransactionScope())
                    {
                        var orderInfo = _goodsOrder.GetGoodsOrder(asynInfo.IdentifyId);
                        if (orderInfo == null) continue;
                        var orderDetailList = _goodsOrderDetail.GetGoodsOrderDetailByOrderId(asynInfo.IdentifyId);
                        string errorMessage;
                        var success = AddByCompleteOrder(orderInfo, orderDetailList, out errorMessage);
                        if (!success)
                        {
                            SAL.LogCenter.LogService.LogError(string.Format("�첽���������ʧ��! IdentifyId={0} {1}", asynInfo.IdentifyId, errorMessage), "�����˹���");
                            continue;
                        }
                        var successDel = _reckoningDao.DeleteAsyn(asynInfo.ID);
                        if (!successDel)
                        {
                            SAL.LogCenter.LogService.LogError(string.Format("ɾ���첽����������ʧ��! ID={0}", asynInfo.ID), "�����˹���");
                            continue;
                        }
                        tran.Complete();
                    }
                }
            }
        }
        #endregion
    }
}
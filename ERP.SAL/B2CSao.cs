using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using ERP.SAL.B2CModel;

namespace ERP.SAL
{
    public class B2CSao
    {
        static Guid GetFilialeIdBySalePlatform(Guid targetId)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var salePlatformInfo = client.Instance.GetSalePlatformInfo(targetId);
                if (salePlatformInfo != null && salePlatformInfo.ID != Guid.Empty)
                {
                    return salePlatformInfo.FilialeId;
                }
                return targetId;
            }
        }
        
        /// <summary>添加银行账户信息
        /// </summary>
        /// <param name="info"></param>
        public static void AddBankAccount(BankAccountInfo info)
        {
            var pushDataId = Guid.NewGuid();
            var filialeId = GetFilialeIdBySalePlatform(info.TargetId);
            using (var client = ClientProxy.CreateB2CWcfClient(info.TargetId))
            {
                client.Instance.BankAccountsInsert(pushDataId, ConvertToB2CModel(info));
            }
        }

        /// <summary>更新银行账户信息
        /// </summary>
        /// <param name="info"></param>
        public static void UpdateBankAccount(BankAccountInfo info)
        {
            var pushDataId = Guid.NewGuid();
            //var filialeId = GetFilialeIdBySalePlatform(info.TargetId);
            using (var client = ClientProxy.CreateB2CWcfClient(info.TargetId))
            {
                client.Instance.BankAccountsUpdate(pushDataId, ConvertToB2CModel(info));
            }
        }

        /// <summary>删除银行账户信息
        /// </summary>
        public static void BankAccountsDelete(Guid targetId, Guid bankAccountId)
        {
            var filialeId = GetFilialeIdBySalePlatform(targetId);
            using (var client = ClientProxy.CreateB2CWcfClient(filialeId))
            {
                client.Instance.BankAccountsDelete(Guid.NewGuid(), bankAccountId);
            }
        }

        /// <summary> 添加绑定银行资金帐号
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="bankAccountId"></param>
        public static void AddBankAccountBinding(Guid targetId, Guid bankAccountId)
        {
            Guid filialeId = GetFilialeIdBySalePlatform(targetId);
            var pushDataId = Guid.NewGuid();
            using (var client = ClientProxy.CreateB2CWcfClient(filialeId))
            {
                client.Instance.AddBankAccountBinding(pushDataId, targetId, bankAccountId);
            }
        }

        /// <summary> 删除绑定银行资金帐号
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="bankAccountId"></param>
        public static void RemoveBankAccountBinding(Guid targetId, Guid bankAccountId)
        {
            Guid filialeId = GetFilialeIdBySalePlatform(targetId);
            var pushDataId = Guid.NewGuid();
            using (var client = ClientProxy.CreateB2CWcfClient(filialeId))
            {
                client.Instance.RemoveBankAccountBinding(pushDataId, targetId, bankAccountId);
            }
        }

        /// <summary>更新B2C银行帐号为主账号
        /// </summary>
        /// <param name="saleFilialeId">销售公司ID</param>
        /// <param name="bankAccountsId">银行ID</param>
        /// <param name="IsMain">是否主账号</param>
        /// <returns></returns>
        public static Boolean SetBankAccountsIsMain(Guid saleFilialeId, Guid bankAccountsId, Boolean IsMain)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                return client.Instance.UpdateIsMain(bankAccountsId, IsMain);
            }
        }

        /// <summary>是不是换货订单（完成订单时不插入销量）
        /// </summary>
        /// <param name="saleFilialeId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool IsRefundOrder(Guid saleFilialeId, Guid orderId)
        {
            //TODO:换货单不插入销量
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                return client.Instance.IsRefundOrder(orderId);
            }
        }
        
        /// <summary>ERP银行账户模型转换成B2C银行账户模型
        /// </summary>
        /// <param name="bankAccountInfo"></param>
        /// <returns></returns>
        private static B2C.Model.ERPExtensionModel.BankAccountInfo ConvertToB2CModel(BankAccountInfo bankAccountInfo)
        {
            return new B2C.Model.ERPExtensionModel.BankAccountInfo
            {
                Accounts = bankAccountInfo.Accounts,
                AccountsKey = bankAccountInfo.AccountsKey,
                AccountsName = bankAccountInfo.AccountsName,
                AccountType = bankAccountInfo.AccountType,
                BankAccountsId = bankAccountInfo.BankAccountsId,
                BankIcon = bankAccountInfo.BankIcon,
                BankName = bankAccountInfo.BankName,
                Description = bankAccountInfo.Description,
                IsBacktrack = bankAccountInfo.IsBacktrack,
                IsDisplay = bankAccountInfo.IsDisplay,
                IsFinish = bankAccountInfo.IsFinish,
                IsMain = bankAccountInfo.IsMain,
                IsUse = bankAccountInfo.IsUse,
                OrderIndex = bankAccountInfo.OrderIndex,
                PaymentInterfaceId = bankAccountInfo.PaymentInterfaceId,
                PaymentType = bankAccountInfo.PaymentType,
                TargetId = bankAccountInfo.TargetId
            };
        }

        public static Dictionary<Guid, int> GetRequireQuantity(Guid saleFilialeId,List<Guid> realGoodsIds, Guid warehouseId)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                return client.Instance.GetRequireRealQuantity(realGoodsIds, warehouseId);
            }
        }

        public static Dictionary<Guid, int> GetAllRequireQuantity(Guid saleFilialeId, List<Guid> realGoodsIds, Guid warehouseId)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                return client.Instance.GetRequireAllQuantity(realGoodsIds, warehouseId);
            }
        }

        public static List<DemandOrderInfo> GetGoodsOrdersByRealGoodsId(Guid saleFilialeId, Guid realGoodsId,Guid warehouseId)
        {
            using (var client = ClientProxy.CreateB2CWcfClient(saleFilialeId))
            {
                var result=client.Instance.GetGoodsOrdersByRealGoodsId(realGoodsId);

                return result!=null && result.IsSuccess && result.Data != null
                    ? result.Data.Where(act=>act.DeliverWarehouseId==warehouseId).Select(act => new DemandOrderInfo
                    {
                        OrderId = act.OrderId,
                        Phone = act.Phone,
                        RealTotalPrice = act.RealTotalPrice,
                        Mobile = act.Mobile,
                        OrderNo = act.OrderNo,
                        Consignee = act.Consignee,
                        Direction=act.Direction,
                        ExpressNo = act.ExpressNo,
                        PayMode = act.PayMode,
                        TotalPrice = act.TotalPrice,
                        Carriage = act.Carriage,
                        OrderState = act.OrderState,
                        Memo = act.Memo,
                        SaleFilialeId = act.SaleFilialeId,
                        DeliverWarehouseId = act.DeliverWarehouseId
                    }).ToList()
                    : new List<DemandOrderInfo>();
            }
        }
    }
}

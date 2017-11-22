using System;
using System.Collections.Generic;
using AllianceShop.Contract.DataTransferObject;
using Keede.Ecsoft.Model;

namespace ERP.SAL
{
    public class ReckoningSao
    {

        /// <summary>添加门店应付帐
        /// </summary>
        /// <param name="filialeid"> </param>
        /// <param name="reckoningInfo"></param>
        public static bool AddShopFrontReckoning(Guid filialeid, ReckoningInfo reckoningInfo)
        {
            var info = new ReckoningRecordDTO
            {
                AccountReceivable = reckoningInfo.AccountReceivable,
                AuditingState = false,
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
            using (var client = ClientProxy.CreateShopStoreWcfClient((filialeid)))
            {
                var result=client.Instance.InsertReckoningRecord(info);
                return result != null && result.IsSuccess;
            }
        }

        /// <summary>
        /// 获取往来资金账号
        /// </summary>
        /// <param name="entityShopId"></param>
        public static IList<BankAccountDTO> GetBankAccountsByEntityShop(Guid entityShopId)
        {
            using (var client = ClientProxy.CreateShopStoreWcfClient(entityShopId))
            {
                var result = client.Instance.SelectBankAccountByPage(entityShopId,1,20);
                return result != null && result.IsSuccess ? result.Data : new List<BankAccountDTO>();
            }
        }
    }
}

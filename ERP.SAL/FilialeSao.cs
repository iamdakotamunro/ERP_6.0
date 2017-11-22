using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.SAL
{
    public class FilialeSao
    {
        public static IEnumerable<FilialeInfo> GetAllFiliale()
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var items = client.Instance.GetAllFiliale();
                foreach (var item in items)
                {
                    if (item.IsActive)
                        yield return new FilialeInfo
                            {
                                Address = item.Address,
                                Code = item.Code,
                                ID = item.ID,
                                IsActive = item.IsActive,
                                ParentId = item.ParentId,
                                Name = item.Name,
                                Rank = item.Rank,
                                FilialeTypes = item.FilialeTypes,
                                ShopJoinType = item.ShopJoinType,
                                GoodsTypes = item.GoodsTypes
                            };
                }
            }
        }

        public static FilialeInfo GetFiliaeInfo(Guid filialeId)
        {
            using (var client = ClientProxy.CreateMISWcfClient())
            {
                var item = client.Instance.GetFilialeInfo(filialeId);
                return new FilialeInfo
                {
                    Address = item.Address,
                    Code = item.Code,
                    ID = item.ID,
                    IsActive = item.IsActive,
                    ParentId = item.ParentId,
                    Name = item.Name,
                    Rank = item.Rank,
                    FilialeTypes = item.FilialeTypes,
                    ShopJoinType = item.ShopJoinType,
                    GoodsTypes = item.GoodsTypes
                };
            }
        }
    }
}

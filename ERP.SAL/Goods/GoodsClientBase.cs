using System;
using KeedeGroup.GoodsManageSystem.Public.Client;

namespace ERP.SAL.Goods
{
    public abstract class GoodsClientBase
    {
        #region -- 初始化服务接口客户端
        internal static ERPGoodsServiceClient GoodsServerClient = new ERPGoodsServiceClient(new Guid("B7E7DFC0-03A6-40AF-9333-B2E8B4695F1D"), "ERP");
        #endregion
    }
}

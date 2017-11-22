using System;
using System.ServiceModel;

namespace CompleteOrderServiceHost
{
    [ServiceContract(Namespace = "http://ERP.Admin.com")]
    public interface IFinish
    {
        [OperationContract]
        bool FinishOrder(DateTime finishDate, Guid warehouseId, Guid expressId,Guid personnelId, string operationer);
    }
}

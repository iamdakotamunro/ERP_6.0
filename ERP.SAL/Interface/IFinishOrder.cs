using System;
using System.ServiceModel;

namespace ERP.SAL.Interface
{
    [ServiceContract(Namespace = "http://ERP.Admin.com")]
    public interface IFinishOrder
    {
        [OperationContract(Action = "http://ERP.Admin.com/IFinish/FinishOrder", ReplyAction = "http://ERP.Admin.com/IFinish/FinishOrderResponse", Name = "FinishOrder")]
        bool FinishOrder(DateTime finishDate, Guid warehouseId, Guid expressId, Guid personnelId, string operationer);
    }
}

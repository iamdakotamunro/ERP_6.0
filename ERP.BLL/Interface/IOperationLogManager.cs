using System;
using System.Collections.Generic;

namespace ERP.BLL.Interface
{
    public interface IOperationLogManager
    {
        IList<Model.OperationLogInfo> GetOperationLogList(Guid identifyKey);

        bool Add(Guid personnelId, string realName, Guid identifyKey,string code,
                 OperationLog.Core.Attributes.EnumPointAttribute enumPointInfo, int workLoad, string remark);

        IDictionary<Guid, IList<Model.OperationLogInfo>> GetOperationLogList(List<Guid> identifyKey);
    }
}

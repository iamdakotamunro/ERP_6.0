using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.BLL.Implement
{
    public class OperationLogManager : Interface.IOperationLogManager
    {
        public IList<OperationLogInfo> GetOperationLogList(Guid identifyKey)
        {
            return SAL.OperationLogSao.GetOperationLogList(identifyKey);
        }

        public bool Add(Guid personnelId, string realName, Guid identifyKey, string code,
                 OperationLog.Core.Attributes.EnumPointAttribute enumPointInfo, int workLoad, string remark)
        {
            var operationLogInfo = OperationLog.Core.OperationPoint.Create(personnelId, realName, identifyKey, code, enumPointInfo, workLoad, remark);
            return Add(operationLogInfo);
        }

        public static bool Add(OperationLog.Core.Model.OperationLogInfo operationLogInfo)
        {
            return SAL.OperationLogSao.Add(operationLogInfo);
        }

        public IDictionary<Guid, IList<OperationLogInfo>> GetOperationLogList(List<Guid> identifyKeys)
        {
            return SAL.OperationLogSao.GetOperationLogList(identifyKeys);
        }
    }
}

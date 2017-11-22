using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;

namespace ERP.SAL
{
    public class OperationLogSao
    {
        static OperationLogInfo ConvertToMyOperationLogInfo(OperationLog.Core.Model.OperationLogInfo operationLogInfo)
        {
            return new OperationLogInfo
                       {
                           IdentifyKey = operationLogInfo.IdentifyKey,
                           Description = operationLogInfo.Description,
                           IsHand = operationLogInfo.IsHand,
                           LogId = operationLogInfo.LogId,
                           OperateTime = operationLogInfo.OperateTime,
                           OperatorId = operationLogInfo.OperatorId,
                           OperatorName = operationLogInfo.OperatorName,
                           PointId = operationLogInfo.PointId,
                           PointName = operationLogInfo.PointName,
                           TypeId = operationLogInfo.TypeId,
                           TypeName = operationLogInfo.TypeName,
                           Workload = operationLogInfo.Workload,
                           IdentifyCode = operationLogInfo.IdentifyCode
                       };
        }

        static IEnumerable<OperationLogInfo> ConvertToMyOperationLogList(IEnumerable<OperationLog.Core.Model.OperationLogInfo> operationLogList)
        {
            return operationLogList.Select(ConvertToMyOperationLogInfo).ToList();
        }

        public static IList<OperationLogInfo> GetOperationLogList(Guid identifyKey)
        {
            using (var client = new OperationLog.Contract.ReadClient("OperationReadService"))
            {
                var list = client.GetList(identifyKey);
                return ConvertToMyOperationLogList(list).ToList();
            }
        }

        public static IDictionary<Guid, IList<OperationLogInfo>> GetOperationLogList(List<Guid> identifyKey)
        {
            IDictionary<Guid, IList<OperationLogInfo>> dics = new Dictionary<Guid, IList<OperationLogInfo>>();
            if (identifyKey != null && identifyKey.Count > 0)
            {
                using (var client = new OperationLog.Contract.ReadClient("OperationReadService"))
                {
                    var list = client.GetListWithGroup(identifyKey.ToArray());
                    foreach (var info in list)
                    {
                        var values = info.Value.Select(ConvertToMyOperationLogInfo).ToList();
                        dics.Add(info.Key, values);
                    }
                }
            }
            return dics;
        }

        public static bool Add(OperationLog.Core.Model.OperationLogInfo operationLogInfo)
        {
            using (var client = new OperationLog.Contract.WriteClient("OperationWriteService"))
            {
                var result = client.AddOperationLog(operationLogInfo);
                return result != null && result.IsSuccess;
            }
        }
    }
}

using System;
using OperationLog.Core;
using OperationLog.Contract;

namespace PrintCertificateBarcode.Core
{
    public class FrameProcessManager
    {

        public static bool AddCheckGlassOperation(Guid personnelId, string realName, Guid identifyKey,string identifyCode)
        {
            var operationLogInfo = OperationPoint.Create(personnelId, realName, identifyKey, identifyCode, 
                OperationPoint.ProcessingFrameGlassManage.Check.GetBusinessInfo(), 1, string.Empty);
            using (var client = new WriteClient("OperationWriteService"))
            {
                var result = client.AddOperationLog(operationLogInfo);
                return result != null && result.IsSuccess;
            }
        }
    }
}

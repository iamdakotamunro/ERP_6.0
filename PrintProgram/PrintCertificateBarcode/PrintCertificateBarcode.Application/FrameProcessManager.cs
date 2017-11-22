using System;
using HRS.Enum.AttributeExtensionTool;
using HRS.Enum.OperationTypePoint;

namespace PrintLabel.Manager
{
    public class FrameProcessManager
    {
        public static ERP.Model.FrameProcessCertificateInfo GetCertificateInfo(string processNo)
        {
            using (var client = new Framework.WCF.WcfClient<ERP.Service.Contract.IService>("Group.ERP"))
            {
                return client.Call(e=>e.GetFrameProcessCertificateInfo(processNo));
            }
        }

        public static void AddCheckGlassOperation(Guid personnelId,string realName,string identifyKey)
        {
            HRS.Model.OperationLogInfo logInfo = new HRS.Model.OperationLogInfo
                {
                     Description = "配镜检查 "+realName+" "+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                      IdentifyKey = identifyKey,
                       IsManual = true,
                        LogId = Guid.NewGuid(),
                         OperateTime =DateTime.Now,
                          PersonnelId = personnelId,
                           PointId = GlassManageOperateState.CheckGlass.GetBusinessKeyCastGuid() ,
                            RealName = realName,
                             TypeId = OperationLogTypeEnum.GlassManage.GetBusinessKeyCastGuid()
                };
            HRS.Model.OperationStatisticsDayInfo operationInfo = new HRS.Model.OperationStatisticsDayInfo
                {
                     PersonnelId = personnelId,
                     PointId = GlassManageOperateState.CheckGlass.GetBusinessKeyCastGuid(),
                     TypeId = OperationLogTypeEnum.GlassManage.GetBusinessKeyCastGuid(),
                     Workload = 1,
                     StatisticsTime = DateTime.Today
                };
            using (var client = new Framework.WCF.WcfClient<ERP.Service.Contract.IService>("Group.ERP"))
            {
                client.Call(e=>e.AddOperationLog(logInfo, operationInfo));
            }
        }
    }
}

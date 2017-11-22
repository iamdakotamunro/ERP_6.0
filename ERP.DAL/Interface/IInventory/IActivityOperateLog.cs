
using System;
using System.Collections.Generic;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IActivityOperateLog
    {
        bool InsertLog(ActivityOperateLogModel activityOperateLogModel);

        List<ActivityOperateLogModel> SelectLogModels(Guid activityId);
    }
}

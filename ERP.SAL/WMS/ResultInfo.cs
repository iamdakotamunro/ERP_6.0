using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.SAL.WMS
{
    public class ResultInfo
    {
        public bool IsSuccess { get; set; }

        public string Msg { get; set; }

        public static ResultInfo Fail(string msg)
        {
            return new ResultInfo { IsSuccess = false, Msg = msg };
        }

        public static ResultInfo Success()
        {
            return new ResultInfo
            {
                IsSuccess = true,
                Msg = string.Empty
            };
        }

        public static ResultInfo ConvertResultInfo(KeedeGroup.WMS.Infrastructure.CrossCutting.ResultInfo resultInfo)
        {
            return resultInfo == null ? ResultInfo.Fail("WMS服务连接异常！") : new ResultInfo { IsSuccess = resultInfo.IsSuccess, Msg = resultInfo.Msg };
        }
    }
}

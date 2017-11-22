using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Model.Finance
{
    [Serializable]
    public class ResultInfo
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public ResultInfo(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public ResultInfo()
        {
            IsSuccess = true;
            Message = string.Empty;
        }
    }
}

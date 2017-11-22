using System;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using Framework.Core.Utility;
using Config.Keede.Library;

namespace ERP.BLL.Implement.Inventory
{
    public class CodeManager
    {
        private readonly ICode _codeDao;

        public CodeManager(ICode code)
        {
            _codeDao = code;
        }

        public CodeManager()
        {
            _codeDao=new Code(GlobalConfig.DB.FromType.Write);
        }

        /// <summary>
        /// 获取指定类型的订单号
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public string GetCode(CodeType codeType)
        {
            DateTime dateTime = DateTime.Now;
            var isTestOrderNo = GlobalConfig.IsTestOrder;
            if (codeType == CodeType.TR)
            {
                return codeType + (dateTime.Year - (isTestOrderNo ? 10 : 0)).ToString().Substring(2, 2) + dateTime.Month.ToString("D2") + dateTime.Day.ToString("D2") + dateTime.Hour.ToString("D2") + dateTime.Minute.ToString("D2") + dateTime.Second.ToString("D2");
            }
            string tradeCode = codeType + (dateTime.Year - (isTestOrderNo ? 10 : 0)).ToString().Substring(2, 2) + dateTime.Month.ToString("D2") + dateTime.Day.ToString("D2") + dateTime.Hour.ToString("D2") + GetCodeValue(codeType, dateTime).ToString("D3");
            return tradeCode;
        }

        /// <summary>
        /// 获取指定类型的当前编号
        /// </summary>
        /// <param name="codeType">单据类型</param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public int GetCodeValue(CodeType codeType, DateTime dateTime)
        {
            if (codeType == CodeType.TR)
            {
                return 1;
            }
            return _codeDao.GetCodeValue(codeType, dateTime);
        }
    }
}

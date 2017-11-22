using ERP.Enum;
using System;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICode
    {
        /// <summary>
        /// 获取指定类型的当前编号
        /// </summary>
        /// <param name="codeType"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        int GetCodeValue(CodeType codeType,DateTime dateTime);
    }
}

using ERP.Enum;
using System;

namespace ERP.DAL.Interface.IInventory
{
    public interface ICode
    {
        /// <summary>
        /// ��ȡָ�����͵ĵ�ǰ���
        /// </summary>
        /// <param name="codeType"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        int GetCodeValue(CodeType codeType,DateTime dateTime);
    }
}

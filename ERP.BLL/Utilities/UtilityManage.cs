using System.Collections.Generic;
using ERP.DAL.Interface.IUtilities;
using ERP.DAL.Factory;

namespace ERP.BLL.Utilities
{
    /// <summary>
    /// 非全字段地操作某个表
    /// </summary>
    public class UtilityManage : BllInstance<UtilityManage>
    {
        private readonly IUtility _utility;
        public UtilityManage(Environment.GlobalConfig.DB.FromType fromType)
        {
            _utility = InventoryInstance.GetUtilityDalDao(fromType);
        }

        #region 非全字段地操作某个表
        /// <summary>
        /// 检查某个字段的是否存在某个值
        /// </summary>
        /// <param name="tableName">表名</param>  
        /// <param name="checkField">列名</param>  
        /// <param name="checkValue">要检查的值</param>  
        /// <returns></returns> 
        public bool CheckExists(string tableName, string checkField, string checkValue)
        {
            return _utility.CheckExists(tableName, checkField, checkValue);
        }
        /// <summary>
        /// 检查存在个数（返回值> 0）
        /// </summary>
        /// <param name="tableName">表名</param>  
        /// <param name="conditionFieldNames">作为条件列的列名(可多列，用英文状态下逗号隔开)</param>
        /// <param name="conditionValues">查询条件值</param>  
        /// <returns>存在的个数</returns> 
        public int CheckExists(string tableName, string conditionFieldNames, object[] conditionValues)
        {
            return _utility.CheckExists(tableName, conditionFieldNames, conditionValues);
        }
        /// <summary>
        /// 根据主键查询一列或多列的值(Datareader中的原始数据，顺序与给定的字段的顺序相同)
        /// </summary>
        /// <param name="tableName">表名</param>  
        /// <param name="fields">列名(如果为多列，请用英文状态下的逗号隔开)</param>  
        /// <param name="pkFieldName">唯一键字段名称</param> 
        /// <param name="pk">唯一键的查询值</param>  
        /// <returns></returns> 
        public object[] GetFieldValueByPk(string tableName, string fields, string pkFieldName, string pk)
        {
            return _utility.GetFieldValueByPk(tableName, fields, pkFieldName, pk);
        }
        /// <summary>
        /// 根据主键查询一列或多列的值(Datareader中的原始数据，顺序与给定的字段的顺序相同)
        /// </summary>
        /// <param name="tableName">表名</param>  
        /// <param name="fields">列名(如果为多列，请用英文状态下的逗号隔开)</param>  
        /// <param name="pkFieldName">唯一键字段名称</param> 
        /// <param name="pkValues">唯一键的查询值</param>  
        /// <returns></returns> 
        public object[] GetFieldValueByPk(string tableName, string fields, string pkFieldName, object[] pkValues)
        {
            return _utility.GetFieldValueByPk(tableName, fields, pkFieldName, pkValues);
        }
        /// <summary>
        /// 获取单列的值的列表(如果想要去重，请使用另一个重载函数)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="targetFieldName">要获取值的列名(单列)</param>
        /// <param name="conditionFieldNames">作为条件列的列名(可多列，用英文状态下逗号隔开)</param>
        /// <param name="conditionValues">查询条件值（要与列名对应）</param>
        /// <returns></returns>
        public List<object> GetSingleFieldValueList(string tableName, string targetFieldName, string conditionFieldNames, object[] conditionValues)
        {
            return _utility.GetSingleFieldValueList(tableName, targetFieldName, conditionFieldNames, conditionValues, false);
        }
        /// <summary>
        /// 获取单列的值的列表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="targetFieldName">要获取值的列名(单列)</param>
        /// <param name="conditionFieldNames">作为条件列的列名(可多列，用英文状态下逗号隔开)</param>
        /// <param name="conditionValues">查询条件值（要与列名对应）</param>
        /// <param name="distinct">是否去重</param>
        /// <returns></returns>
        public List<object> GetSingleFieldValueList(string tableName, string targetFieldName, string conditionFieldNames, object[] conditionValues, bool distinct)
        {
            return _utility.GetSingleFieldValueList(tableName, targetFieldName, conditionFieldNames, conditionValues, distinct);
        }
        /// <summary>
        /// 根据主键更新一列或多列的值
        /// </summary>
        /// <param name="tableName">表名</param>  
        /// <param name="fields">列名(如果为多列，请用英文状态下的逗号隔开)</param>  
        /// <param name="values">值</param>  
        /// <param name="pkFieldName">唯一键字段名称</param> 
        /// <param name="pk">唯一键的查询值</param>  
        /// <returns>受影响的行数</returns> 
        public int UpdateFieldByPk(string tableName, string fields, object[] values, string pkFieldName, string pk)
        {
            return _utility.UpdateFieldByPk(tableName, fields, values, pkFieldName, pk);
        }
        /// <summary>
        /// 根据一个或多个主键跟新一列或多列的值
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fields">列名(如果为多列，请用英文状态下的逗号隔开)</param>
        /// <param name="values">列对应的要更新的值</param>
        /// <param name="pkFieldNames">主键(如果为多列，请用英文状态下的逗号隔开)</param>
        /// <param name="pkValues">主键对应的值</param>
        /// <returns></returns>
        public int UpdateFieldByPk(string tableName, string fields, object[] values, string pkFieldNames, object[] pkValues)
        {
            return _utility.UpdateFieldByPk(tableName, fields, values, pkFieldNames, pkValues);
        }
        #endregion
    }
}

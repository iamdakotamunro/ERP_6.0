using ERP.Environment;
using Keede.DAL.Helper;

namespace ERP.DAL
{
    /// <summary>数据库配置表数据层  ADD  2014-12-30  陈重文
    /// </summary>
    public class ConfigDAL
    {
        private readonly GlobalConfig.DB.FromType _fromType;
        public ConfigDAL(GlobalConfig.DB.FromType fromType)
        {
            _fromType = fromType;
        }

        /// <summary>获取Config表Value值
        /// </summary>
        /// <param name="key">KEY</param>
        /// <returns>Return : Value</returns>
        public string GetConfigValue(string key)
        {
            const string SQL = @"SELECT Value FROM Config WHERE [Key]=@Key";
            using (var db = new Database(GlobalConfig.ERP_DB_NAME))
            {
                return db.GetValue<string>(true, SQL, new Parameter("@Key", key));
            }
        }
    }
}

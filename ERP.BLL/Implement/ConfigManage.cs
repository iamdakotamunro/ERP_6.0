using System;
using System.Collections;
using ERP.DAL;

namespace ERP.BLL.Implement
{
    /// <summary>数据库配置表数据层  ADD  2014-12-30  陈重文
    /// </summary>
    public static class ConfigManage
    {
        static readonly ConfigDAL _configDal=new ConfigDAL(Environment.GlobalConfig.DB.FromType.Write);

        /// <summary>获取Config表Value值
        /// </summary>
        /// <param name="key">KEY</param>
        /// <returns>Return : Value</returns>
        public static string GetConfigValue(string key)
        {
            return _configDal.GetConfigValue(key);
        }
    }
}

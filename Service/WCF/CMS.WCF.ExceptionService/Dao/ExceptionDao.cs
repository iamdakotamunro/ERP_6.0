using System;
using System.Linq;
using System.Collections.Generic;
using CMS.WCF.ExceptionService.Model;
using Framework.Data;

namespace CMS.WCF.ExceptionService.Dao
{
    public class ExceptionDao
    {
        private static Database CreateDatabase()
        {
            return new Database(Config.DB.ConnectionName_CMS);
        }

        /// <summary>
        /// 读取还未发送过的异步数据
        /// </summary>
        /// <returns></returns>
        public static IList<CommandInfo> GetNoSendCommand(int quantity)
        {
            const string SQL =
                @"SELECT TOP {Quantity} * FROM [lmshop_Command] WHERE IsSuccess=0 AND IsSendCommand=1 AND ISNULL(ExceptionCount,0)<5  ORDER BY CommandDate ASC";
            using (var db = CreateDatabase())
            {
                return db.Select<CommandInfo>(SQL.Replace("{Quantity}", quantity.ToString()),
                                              new Parameter(string.Empty, string.Empty)).ToList();
            }
        }

        /// <summary>
        /// 读取失败的异步数据
        /// </summary>
        /// <returns></returns>
        public static IList<CommandInfo> GetFailureExceptionByFailCount(int quantity, int failCount)
        {
            const string SQL =
                @"SELECT TOP {Quantity} * FROM [lmshop_Command] WHERE IsSuccess=0 AND IsSendCommand=1 AND ISNULL(ExceptionCount,0)<@FailCount  ORDER BY CommandDate ASC";
            using (var db = CreateDatabase())
            {
                return db.Select<CommandInfo>(SQL.Replace("{Quantity}", quantity.ToString()),
                                              new Parameter("FailCount", failCount)).ToList();
            }
        }

        /// <summary>
        /// 获取平台来源
        /// </summary>
        /// <returns></returns>
        public static IList<FromSourceInfo> GetFromSources()
        {
            const string SQL = @"SELECT [Id],[EndPointName],[IsDefault],[Name],[FromType] FROM [lmShop_WebSite]";
            using (var db = CreateDatabase())
            {
                return db.Select<FromSourceInfo>(SQL).ToList();
            }
        }


        /// <summary>
        /// 修改异常信息
        /// </summary>
        /// <param name="commandId">异常表属性</param>
        /// <param name="exceptionMsg"></param>
        public static void UpdateExceptionCount(Guid commandId, string exceptionMsg)
        {
            const string SQL =
                @"UPDATE [lmshop_Command] SET Exception=@Exception,ExceptionCount=ISNULL(ExceptionCount,0)+1,ExceptionTotalCount=ISNULL(ExceptionTotalCount,0)+1 WHERE CommandID=@CommandID";
            try
            {
                using (var db = CreateDatabase())
                {
                    db.Execute(SQL, new Parameter("Exception", exceptionMsg),
                               new Parameter("CommandID", commandId));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 删除成功的异常
        /// </summary>
        /// <param name="commandId"></param>
        public static void DeleteExceptionBySuccess(Guid commandId)
        {
            const string SQL = @"DELETE FROM [lmshop_Command] WHERE CommandID=@CommandID";
            try
            {
                using (var db = CreateDatabase())
                {
                    db.Execute(SQL, new Parameter("CommandID", commandId));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 删除前几天的成功异常
        /// </summary>
        /// <param name="day"></param>
        public static void DeleteSuccessExceptionByBeforeDay(int day)
        {
            const string SQL =
                @"DELETE FROM [lmshop_Command] WHERE (IsSuccess=1 OR IsSendCommand=0) AND (DATEDIFF(DD,CommandDate,GETDATE())>@Day)";
            using (var db = CreateDatabase())
            {
                db.Execute(SQL, new Parameter("Day", day));
            }
        }
    }
}

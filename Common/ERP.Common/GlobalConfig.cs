using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Framework.Core.Utility;
using Framework.Core.Extension;
using Config.Keede.Library;

namespace ERP.Environment
{
    public static class GlobalConfig
    {
        static readonly bool _isTestOrder;
        static readonly int _keepYear;
        static readonly bool _isTestWebSite;
        private static readonly string _iamgeSize;
        private static readonly string _ResourceServerImg;
        private static readonly int _Expire;

        /// <summary>
        /// 数据库操作记录
        /// </summary>
        static readonly IDictionary<string, DateTime> _dbExecuteLog = new System.Collections.Concurrent.ConcurrentDictionary<string, DateTime>();

        static readonly Timer _timer = new Timer(20000);


        static GlobalConfig()
        {
            var keepYear = ConfManager.GetAppsetting("KeepYearsData");
            if (!int.TryParse(keepYear, out _keepYear))
            {
                _keepYear = 2;
            }
            _iamgeSize = ConfManager.GetAppsetting("ImageSize");
            _ResourceServerImg = ConfManager.GetAppsetting("ResourceServerImg");
            var expire = ConfManager.GetAppsetting("Expire");
            if (!int.TryParse(expire, out _Expire))
            {
                _Expire = 30;
            }

            _isTestOrder = Configuration.AppSettings["IsTestOrder"] == "true";
            _isTestWebSite = Configuration.AppSettings["IsTestWebSite"] == "true";


            _timer.Elapsed += (sender, e) =>
            {
                var needDelList = _dbExecuteLog.Where(a => (DateTime.Now - a.Value).TotalSeconds >= 20).ToList();
                foreach (var d in needDelList)
                    _dbExecuteLog.Remove(d.Key);
            };
            _timer.Start();
        }

        #region -- 普通配置的属性

        /// <summary>是否是测试网站
        /// </summary>
        public static bool IsTestWebSite
        {
            get { return _isTestWebSite; }
        }

        public static string ImageSize
        {
            get { return _iamgeSize; }
        }

        /// <summary>
        /// 登录退出地址
        /// </summary>
        public static string LoginOutUrl
        {
            get { return ConfManager.GetAppsetting("LoginOutUrl"); }
        }

        /// <summary>
        /// 是否测试下单
        /// </summary>
        public static bool IsTestOrder
        {
            get { return _isTestOrder; }
        }

        /// <summary>
        /// 数据保留年份
        /// </summary>
        public static int KeepYear
        {
            get
            {
                return _keepYear;
            }
        }

        /// <summary>
        /// 资源服务器图片路径
        /// </summary>
        public static string ResourceServerImg
        {
            get { return _ResourceServerImg; }
        }

        /// <summary>
        /// 资源网站
        /// </summary>
        public static string ResourceServerInformation
        {
            get { return ConfManager.GetAppsetting("ResourceServerInformation"); }
        }

        /// <summary>
        /// 默认门店分公司
        /// </summary>
        public static string ShopBranchId
        {
            get { return ConfManager.GetAppsetting("ShopBranchId"); }
        }

        /// <summary>
        /// 资质过期时间，小于等于30天为快过期
        /// </summary>
        public static int Expire
        {
            get
            {
                return _Expire;
            }
        }

        #endregion

        /// <summary>ERP公司ID
        /// </summary>
        public static Guid ERPFilialeID
        {
            get
            {
                var val = ConfManager.GetAppsetting("FilialeId");
                if (string.IsNullOrEmpty(val))
                {
                    throw new ApplicationException("ERP FilialeID Is Not Config!");
                }
                return Guid.Parse(val);
            }
        }

        /// <summary>发货主仓ID
        /// </summary>
        public static Guid MainWarehouseID
        {
            get
            {
                var val = ConfManager.GetAppsetting("MainWarehouseID");
                if (string.IsNullOrEmpty(val))
                {
                    throw new ApplicationException("MainWarehouseID Is Not Config!");
                }
                return Guid.Parse(val);
            }
        }

        /// <summary>
        /// 页面自动刷新延迟时间，单位：毫秒
        /// </summary>
        public static int PageAutoRefreshDelayTime
        {
            get
            {
                var val = ConfManager.GetAppsetting("PageAutoRefreshDelayTime");
                int result = 0;
                if (!int.TryParse(val, out result))
                {
                    result = 1000;// 未设置值，则默认为1秒
                }
                if (result <= 0)
                {
                    result = 100;//设置的值过小，则默认0.1秒
                }
                return result;
            }
        }

        public const string ERP_DB_NAME = "Erp";

        public const string ERP_HISTORY_DB_NAME = "ErpHistory";

        public const string ERP_REPORT_DB_NAME = "ErpReport";

        /// <summary> 根据年份获取不同的数据库连接
        /// </summary>
        /// <returns></returns>
        public static string GetErpDbName(int startyear)
        {
            var yearlist = new List<int>();
            for (var i = 2007; i <= (DateTime.Now.Year - KeepYear); i++)
            {
                yearlist.Add(i);
            }
            if (!yearlist.Contains(startyear))
            {
                return ERP_DB_NAME;
            }
            return ERP_HISTORY_DB_NAME + startyear;
        }

        /// <summary>
        /// 数据库配置信息
        /// </summary>
        public static class DB
        {
            /// <summary>
            /// 数据来源类型
            /// </summary>
            public enum FromType
            {
                /// <summary>
                /// 写主库
                /// </summary>
                Write = 1,

                /// <summary>
                /// 读从库
                /// </summary>
                Read
            }
        }
    }
}
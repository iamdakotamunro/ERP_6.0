using Framework.Common;

namespace CMS.WCF.ExceptionService
{
    public class Config
    {
        static Config()
        {
            _readQuantity = Configuration.AppSettings["ReadQuantity"].ToInt();
            _deleteTime = Configuration.AppSettings["DeleteTime"].ToInt();
        }

        private static readonly int _readQuantity;

        public static int ReadQuantity
        {
            get { return _readQuantity; }
        }

        private static readonly int _deleteTime;

        public static int DeleteTime
        {
            get { return _deleteTime; }
        }

        public class DB
        {
            public static string ConnectionName_CMS
            {
                get { return "db_CMS"; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class WCF
        {
            public static string EndPoint_NewKeede
            {
                get { return "KeedeEndPoint"; }
            }

            public static string EndPoint_Eyesee
            {
                get { return "EyeseeEndPoint"; }
            }

            public static string EndPoint_MIS
            {
                get { return "KeedeMisEndPoint"; }
            }

            public static string EndPoint_Baishop
            {
                get { return "BaishopEndPoint"; }
            }
        }
    }
}
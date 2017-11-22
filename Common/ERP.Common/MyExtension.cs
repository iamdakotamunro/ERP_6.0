
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Environment
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class MyExtension
    {
        /// <summary>
        /// 对小数进行取位，默认保留2个小数点
        /// </summary>
        /// <param name="dcmal"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static decimal Round(this decimal dcmal, int point = 2)
        {
            return Math.Round(dcmal, point);
        }

        #region Object的扩展方法

        public static Guid ObjToGuid(this object str)
        {
            return str == DBNull.Value ? Guid.Empty : new Guid(str.ToString());
        }



        public static DateTime ObjToDatetime(this object str)
        {
            return str == DBNull.Value ? DateTime.MinValue : DateTime.Parse(str.ToString());
        }


        public static int ObjToInt(this object str)
        {
            return str == DBNull.Value ? 0 : int.Parse(str.ToString());
        }
        public static decimal ObjToDecimal(this object str)
        {
            return str == DBNull.Value ? 0 : decimal.Parse(str.ToString());
        }
        #endregion


        #region String的扩展方法

        public static Guid? StrToGuid(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return new Guid(str.ToString());
        }
        
        public static decimal? StrToDecimal(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return decimal.Parse(str.ToString());
        }
        #endregion
    }
}

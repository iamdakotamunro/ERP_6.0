using System;
using System.Text.RegularExpressions;

namespace ERP.Environment
{
    public class MethodHelp
    {
        /// <summary> 返回2个时间相差的间隔,单位:分钟
        /// </summary>
        /// <param name="d1">相对早的时间</param>
        /// <param name="d2">相对晚的时间</param>
        /// <returns></returns>
        public static double GetDateSubtract(DateTime d1, DateTime d2)
        {
            TimeSpan d = d2.Subtract(d1);
            double restr = 0;
            if (d.Days != 0)
            {
                restr += d.Days * 24 * 60;

            }
            if (d.Hours != 0)
            {
                restr += d.Hours * 60;
            }

            if (d.Minutes != 0)
            {
                restr += d.Minutes;
            }
            return restr;
        }

        /// <summary> 验证是否是配货单号
        /// </summary>
        /// <param name="pickNo"></param>
        /// <returns></returns>
        public static bool CheckPickNo(string pickNo)
        {
            var reg = new Regex("[a-z]+[0-9]+\\-[0-9]+\\s[0-9]{2}\\-[0-9]{2}", RegexOptions.IgnoreCase);
            if (reg.IsMatch(pickNo))
                return true;
            return false;
        }

        /// <summary> 检查该字符串是否是guid字符串
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool CheckGuid(string guid)
        {
            try
            {
                Guid newGuid = new Guid(guid);
            }
            catch
            {
                return false;
            }
            return true;
        }


    }
}

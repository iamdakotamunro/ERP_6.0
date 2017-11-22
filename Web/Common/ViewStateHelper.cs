using System;
using System.Web.UI;

namespace ERP.UI.Web.Common
{
    /// <summary>
    /// ViewState帮助类
    /// </summary>
    public class ViewStateHelper : Page
    {
        /// <summary>
        /// 获取ViewState
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defalutVal">默认值</param>
        /// <returns></returns>
        public string GetViewState(string key, string defalutVal = "")
        {
            if (ViewState[key] == null)
                return defalutVal == "" ? string.Empty : defalutVal;
            return ViewState[key].ToString();
        }

        /// <summary>
        /// 获取ViewState
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DateTime? GetViewState_Date(string key)
        {
            if (ViewState[key] == null)
                return null;
            return Convert.ToDateTime(ViewState[key]);
        }
        /// <summary>
        /// 获取ViewState
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Guid GetViewState_Guid(string key)
        {
            if (ViewState[key] == null)
                return Guid.Empty;
            return new Guid(ViewState[key].ToString());
        }


        /// <summary>
        /// 设置ViewState
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void SetViewState(string key, string value)
        {
            ViewState[key] = value;
        }
        /// <summary>
        /// 设置ViewState
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void SetViewState_Date(string key, DateTime? value)
        {
            ViewState[key] = value;
        }

        /// <summary>
        /// 设置ViewState
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void SetViewState_Guid(string key, Guid value)
        {
            ViewState[key] = value;
        }
    }
}

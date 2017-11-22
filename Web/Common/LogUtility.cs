using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using ERP.Enum;

namespace ERP.UI.Web.Common
{
    /// <summary> Func:create the exception log 
    /// </summary>
    public class LogUtility
    {
        /// <summary>
        /// Func : 记录异常信息到 Log文件夹和计算机管理-应用程序-警告
        /// Coder: dyy
        /// Date : 2009 Dec 11th
        /// </summary>
        /// <param name="objErr"></param>
        /// <param name="displayError"></param>
        public void WriteException(Exception objErr, String displayError)
        {
            try
            {
                WriteException(objErr, "KeedeAdminBug", LogType.NotePadLog, displayError);
            }
            catch (Exception ex)
            {
                throw new Exception("写入日志信息失败", ex);
            }
        }

        /// <summary>
        /// Func : 记录异常信息到 Log文件夹和计算机管理-应用程序-警告
        /// Coder: dyy
        /// Date : 2009 Dec 11th
        /// </summary>
        /// <param name="objErr"></param>
        public void WriteException(Exception objErr)
        {
            try
            {
                WriteException(objErr, "KeedeAdminBug", LogType.NotePadLog, String.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception("写入日志信息失败", ex);
            }

        }

        /// <summary>
        /// Func : 记录异常信息到 Log文件夹和计算机管理-应用程序-警告
        /// Coder: dyy
        /// Date : 2006 Dec 11th
        /// </summary>
        /// <param name="objErr"></param>
        /// <param name="appName"></param>
        /// <param name="lType"></param>
        /// <param name="displayError"></param>
        public void WriteException(Exception objErr, String appName, LogType lType, String displayError)
        {
            #region Initial Exception information

            string errortime = "Occur Time:" + DateTime.Now;
            const string ERRORTYPE = "Log Type:" + "throw by manual";
            string erroraddr = "Exception URL: " + HttpContext.Current.Request.Url;
            string errorinfo = "Exception Message: " + objErr.Message;
            string errorsource = "Error Source:" + objErr.Source;
            string errortrace = "Stack Trace:" + objErr.StackTrace;
            #endregion

            #region Write Exception Log to notepad file in LogFiles Folder
            if (lType == LogType.Both || lType == LogType.NotePadLog)
            {
                //Lock the writng action
                StreamWriter writer = null;
                try
                {
                    lock (this)
                    {
                        //m.WaitOne();
                        //Create a Directory per month
                        String filename = "admin" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + ".txt";
                        String path = HttpContext.Current.Server.MapPath("~/LogFiles");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var file = new FileInfo(path + "/" + filename);

                        writer = new StreamWriter(file.FullName, true);
                        writer.WriteLine("Client IP:" + HttpContext.Current.Request.UserHostAddress);
                        if (CurrentSession.Personnel.Get() != null)
                        {
                            writer.WriteLine("Login UserId:" + CurrentSession.Personnel.Get().PersonnelId);
                            writer.WriteLine("Login UserName:" + CurrentSession.Personnel.Get().RealName);
                        }
                        if (!String.IsNullOrEmpty(displayError))
                            writer.WriteLine("Display Exception:" + displayError);
                        writer.WriteLine(errortime);
                        writer.WriteLine(erroraddr);
                        writer.WriteLine(ERRORTYPE);
                        writer.WriteLine(errorinfo);
                        writer.WriteLine(errorsource);
                        writer.WriteLine(errortrace);
                        writer.WriteLine("------------------------------------------------------------------------------------------------------------------");
                        //m.ReleaseMutex();
                    }
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }
            #endregion

            #region Write Exception Log to Windows Application Event in Computer Manager
            if (lType == LogType.Both || lType == LogType.WindowsEvent)
            {
                var objErrBuilder = new StringBuilder();
                objErrBuilder.Append("Error Caught in WebControl.LogException event\n");
                objErrBuilder.Append("Client IP:").Append(HttpContext.Current.Request.UserHostAddress).Append("\n");
                if (CurrentSession.Personnel.Get() != null)
                {
                    objErrBuilder.Append("Login UserId:").Append(CurrentSession.Personnel.Get().PersonnelId).Append("\n");
                    objErrBuilder.Append("Login UserName:").Append(CurrentSession.Personnel.Get().RealName).Append("\n");
                }
                if (!String.IsNullOrEmpty(displayError))
                    objErrBuilder.Append("Display Exception:").Append(displayError).Append("\n");
                objErrBuilder.Append(errortime).Append("\n");
                objErrBuilder.Append(erroraddr).Append("\n");
                objErrBuilder.Append(errorinfo).Append("\n");
                objErrBuilder.Append(errorsource).Append("\n");
                objErrBuilder.Append(errortrace).Append("\n");

                EventLog.WriteEntry(appName, objErrBuilder.ToString(), EventLogEntryType.Warning);
            }
            #endregion
        }
    }
}

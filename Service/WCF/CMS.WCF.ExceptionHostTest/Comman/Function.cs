using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public class Function
{
    #region 序列化某对象
    /// <summary>
    /// 序列化某对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Byte[] SerializationObject(Object obj)
    {
        Byte[] returnBytes;
        IFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, obj);
        StreamReader reader = new StreamReader(stream);
        stream.Seek(0, 0);
        //stream.Position = 0;
        //strReturn = reader.ReadToEnd();
        returnBytes = stream.ToArray();
        stream.Close();
        return returnBytes;
    }
    #endregion

    #region 反序列化某对象
    /// <summary>
    /// 反序列化某对象
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static object DeserializationObject(Byte[] content)
    {
        //ISurrogateSelector s = new SurrogateSelector();
        MemoryStream stream = new MemoryStream(content);
        IFormatter formatter = new BinaryFormatter();
        return formatter.Deserialize(stream);
    }
    #endregion

    #region [获取绝对地址]bin/debug
    /// <summary>
    /// 获取安装服务的绝对路径地址
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>绝对路径</returns>
    public static string GetAbosultePath(string fileName)
    {
        string assemblyFilePath = Assembly.GetExecutingAssembly().Location;
        string assemblyDirPath = Path.GetDirectoryName(assemblyFilePath);
        string absolutePath = Path.Combine(assemblyDirPath, fileName); //EySeeLog服务的地址
        return absolutePath;
    }
    #endregion

    #region [TXT日志文件管理]
    /// <summary>
    /// 发送日志
    /// eg:WriteLog("异常信息");
    /// </summary>
    /// <param name="exceptionMessage">发送结果</param>
    /// <param name="logFileName">日志文件名称</param>
    public static void WriteLog(string exceptionMessage, string logFileName)
    {
        var sb = new StringBuilder();
        sb.Append(DateTime.Now + " | ");
        sb.Append(exceptionMessage);
        try
        {
            string absolutePath = GetAbosultePath(logFileName);
            LogSetting(absolutePath, sb.ToString());

        }
        catch (Exception)
        {
            throw;
        }
    }
    /// <summary>
    /// 日志设置
    /// eg：LogSetting（"E:/123.txt","日志内容"）
    /// </summary>
    /// <param name="savePath">保存路径绝对路径</param>
    /// <param name="bodyStr">内容主体</param>
    private static void LogSetting(string savePath, string bodyStr)
    {
        FileStream fs;
        StreamWriter sw;
        if (!File.Exists(savePath))//如果不存在
        {
            fs = new FileStream(savePath, FileMode.Create, FileAccess.Write); //创建写入文件 
            sw = new StreamWriter(fs);
            sw.WriteLine(bodyStr); //开始写入值

        }
        else
        {
            fs = new FileStream(savePath, FileMode.Append, FileAccess.Write); //跟着结尾写
            sw = new StreamWriter(fs);
            sw.WriteLine(bodyStr); //开始写入值
        }
        sw.Close();
        fs.Close();
    }
    #endregion

    #region ****webconfig中的配置****
    /// <summary>
    /// <appSettings>里的配置
    /// </summary>
    /// <param name="name">配置名称</param>
    /// <returns></returns>
    public static string GetConfig(string name)
    {
        return System.Configuration.ConfigurationManager.AppSettings[name];
    }
    #endregion
}


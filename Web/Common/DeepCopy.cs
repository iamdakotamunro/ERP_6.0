using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ERP.UI.Web.Common
{
    ///深度拷贝 for .net 3.0 above
    ///DYY
    ///2010.7.16
    public static class ExtendMethodClass
    {
        /// <summary>需要被拷贝的对象
        /// </summary>
        /// <param name="src"></param>
        /// <returns>拷贝后返回的对象</returns>
        public static object DeepCopy(this object src)
        {
            BinaryFormatter Formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            MemoryStream stream = new MemoryStream();
            Formatter.Serialize(stream, src);
            stream.Position = 0;
            Object clonedObj = Formatter.Deserialize(stream);
            stream.Close();
            return clonedObj;
        }
    }
}

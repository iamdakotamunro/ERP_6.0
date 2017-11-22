using System;
using Framework.Common;

namespace ERP.UI.Web.Common
{
    /// <summary>
    /// Func : public crypt method with the public key
    /// Coder: dyy
    /// Date : 2010 Jan.9th
    /// </summary>
    public static class CryptPublic
    {
        //private static readonly byte[] _cryptKey = Encoding.UTF8.GetBytes("可得网GREAT..");
        //private static readonly byte[] _cryptIV = Encoding.UTF8.GetBytes("可得网GREAT..");
        private const string KEY = "keede56732tgt64";

        public static String GetEncryptText(String cryptText)
        {
            return DES.Encrypt(cryptText, KEY);
        }

        public static String GetDecryptText(String cryptText)
        {
            return DES.Decrypt(cryptText, KEY);
        }
    }
}

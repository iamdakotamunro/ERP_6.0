using System;
using System.Data;
using System.Data.SqlClient;

namespace ERP.DAL
{
    public static class SqlRead
    {
        public static Guid GetGuid(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? Guid.Empty : new Guid(dr[column].ToString());
        }

        public static string GetString(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? string.Empty : dr[column].ToString();
        }

        public static bool GetBoolean(IDataReader dr, string column)
        {
            return dr[column] != DBNull.Value && bool.Parse(dr[column].ToString());
        }

        public static decimal GetDecimal(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : decimal.Parse(dr[column].ToString());
        }

        public static int GetInt(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? 0 : int.Parse(dr[column].ToString());
        }

        public static DateTime GetDateTime(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(dr[column].ToString());
        }

        public static long GetLong(IDataReader dr, string column)
        {
            return dr[column] == DBNull.Value ? default(long) : long.Parse(dr[column].ToString());
        }
    }
}

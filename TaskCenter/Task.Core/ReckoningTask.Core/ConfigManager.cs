namespace ReckoningTask.Core
{
    public class ConfigManager
    {
        public static void InitERPConnectionName(string connectionName)
        {
            ConfigInfo.ERPConnectionName = connectionName;
        }
    }
}

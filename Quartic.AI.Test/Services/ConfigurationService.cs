namespace Quartic.AI.Test.Services
{
    using System.Configuration;

    public static class ConfigurationService
    {
        public static string GetValue(string settingName)
        {
            return ConfigurationManager.AppSettings[settingName];
        }
    }
}
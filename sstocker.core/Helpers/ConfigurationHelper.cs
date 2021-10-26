using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;

namespace sstocker.core.Helpers
{
    public static class ConfigurationHelper
    {
        private static IConfiguration Configuration;
        private static NameValueCollection AppSettings;

        public static void Initialize(NameValueCollection appSettings)
        {
            AppSettings = appSettings;
        }

        public static void Initialize(IConfiguration config)
        {
            Configuration = config;
        }

        public static string GetConfiguration(string key)
        {
            if (Configuration != null)
            {
                return (string)Configuration.GetValue(typeof(string), key);
            }
            else if(AppSettings != null)
            {
                return AppSettings.Get(key);
            }
            else
            {
                throw new Exception("ConfigurationHelper was not initialized.");
            }
        }
    }
}

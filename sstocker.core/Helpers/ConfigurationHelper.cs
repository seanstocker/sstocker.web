using Microsoft.Extensions.Configuration;

namespace sstocker.core.Helpers
{
    public static class ConfigurationHelper
    {
        private static IConfiguration Configuration;

        public static void Initialize(IConfiguration config)
        {
            Configuration = config;
        }

        public static string GetConfiguration(string key)
        {
            return (string)Configuration.GetValue(typeof(string), key);
        }
    }
}

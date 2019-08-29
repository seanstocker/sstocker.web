using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace sstocker.core.Helpers
{
    public static class SessionHelper
    {
        public const string SessionKeyAccountId = "AccountId";
        public const string SessionKeyAccountName = "Name";

        public static void Set<T>(this ISession session, string key, T value)
        {
            var hostname = Dns.GetHostName();
            var ip = Dns.GetHostEntry(hostname).AddressList[0].ToString();
            session.SetString($"{hostname}_{ip}_{key}", JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var hostname = Dns.GetHostName();
            var ip = Dns.GetHostEntry(hostname).AddressList[0].ToString();
            var value = session.GetString($"{hostname}_{ip}_{key}");

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}

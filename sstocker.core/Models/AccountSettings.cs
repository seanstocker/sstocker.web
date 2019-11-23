using Newtonsoft.Json;
using System.Collections.Generic;

namespace sstocker.core.Models
{
    public class AccountSettings<T>
    {
        public long AccountId;
        public List<AccountSetting<T>> Settings;

        public AccountSettings(long accountId, IEnumerable<AccountSetting<string>> settings = null)
        {
            AccountId = accountId;
            Settings = new List<AccountSetting<T>>();
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    Settings.Add(new AccountSetting<T>
                    {
                        ContextKey = setting.ContextKey,
                        ContextValue = setting.ContextValue,
                        Data = JsonConvert.DeserializeObject<T>(setting.Data)
                    });
                }
            }
        }
    }

    public class AccountSetting<T>
    {
        public string ContextKey;
        public long ContextValue;
        public T Data;
    }
}

using System;

namespace sstocker.budget.Models
{
    public class Account
    {
        public long AccountId;
        public string Username;
        public string Name;
        public string Image;
        public DateTime CreateDateUTC;
    }

    public class AccountWithPassword : Account
    {
        public string Password;
        public int Salt;
    }
}

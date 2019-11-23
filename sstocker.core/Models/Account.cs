using sstocker.core.Helpers;
using System;

namespace sstocker.core.Models
{
    public class Account
    {
        public long AccountId;
        public string Username;
        public string Name;
        public DateTime CreateDateUTC;

        private string _image;

        public string Image { get { return AccountHelper.GetImage(_image); } set { _image = value; } }

        public bool IsDefaultImage()
        {
            return string.IsNullOrWhiteSpace(_image);
        }
    }

    public class AccountWithPassword : Account
    {
        public string Password;
        public int Salt;
    }
}

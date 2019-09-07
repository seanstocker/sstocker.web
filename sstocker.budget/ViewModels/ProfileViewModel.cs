﻿using sstocker.budget.Repositories;

namespace sstocker.budget.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public string CurrentProfileImage;
        public bool IsDefaultImage;

        public ProfileViewModel(long accountId)
        {
            var account = AccountRepository.GetAccount(accountId, true);
            CurrentProfileImage = account.Image;
            IsDefaultImage = account.IsDefaultImage();
        }
    }
}
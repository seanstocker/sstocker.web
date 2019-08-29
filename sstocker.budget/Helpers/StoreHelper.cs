using sstocker.budget.Models;
using sstocker.budget.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.budget.Helpers
{
    public static class StoreHelper
    {
        private static List<Store> Stores;

        static StoreHelper()
        {
            Stores = StoreRepository.GetAllStores();
        }

        public static List<Store> GetStores()
        {
            if (Stores == null || !Stores.Any())
                Stores = StoreRepository.GetAllStores();

            return Stores;
        }

        public static List<string> GetStoreNames()
        {
            return GetStores().Select(c => c.Name).ToList();
        }

        public static bool StoreExists(string name)
        {
            return GetStoreNames().Any(n => n == name);
        }

        public static long GetStoreId(string name)
        {
            return Stores.Single(c => c.Name == name).StoreId;
        }

        public static long GetOrAddStoreId(string name)
        {
            if (!StoreExists(name))
                AddStore(name);
            return GetStoreId(name);
        }

        public static void AddStore(string name)
        {
            if (Stores.Any(c => c.Name == name))
                return;

            StoreRepository.CreateStore(name);
            Stores = StoreRepository.GetAllStores();
        }
    }
}

using sstocker.budget.Models;
using sstocker.budget.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.budget.Helpers
{
    public static class IncomeSourceHelper
    {
        private static List<IncomeSource> IncomeSources;

        static IncomeSourceHelper()
        {
            IncomeSources = IncomeSourceRepository.GetAllIncomeSources();
        }

        public static List<IncomeSource> GetIncomeSources(long accountId = 0)
        {
            if (IncomeSources == null || !IncomeSources.Any())
                IncomeSources = IncomeSourceRepository.GetAllIncomeSources();

            return IncomeSources;
        }

        public static List<string> GetIncomeSourceNames(long accountId = 0)
        {
            return GetIncomeSources(accountId).Select(c => c.Name).ToList();
        }

        public static long GetIncomeSourceId(string name)
        {
            return IncomeSources.Single(c => c.Name == name).IncomeSourceId;
        }

        public static void AddIncomeSource(string name)
        {
            if (IncomeSources.Any(c => c.Name == name))
                return;

            IncomeSourceRepository.CreateIncomeSource(name);
            IncomeSources = IncomeSourceRepository.GetAllIncomeSources();
        }

        public static bool IncomeSourceExists(string name)
        {
            return GetIncomeSourceNames().Any(n => n == name);
        }

        public static long GetOrAddIncomeSourceId(string name)
        {
            if (!IncomeSourceExists(name))
                AddIncomeSource(name);
            return GetIncomeSourceId(name);
        }
    }
}

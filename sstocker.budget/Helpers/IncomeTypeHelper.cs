using sstocker.budget.Models;
using sstocker.budget.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.budget.Helpers
{
    public static class IncomeTypeHelper
    {
        private static List<IncomeType> Types;

        static IncomeTypeHelper()
        {
            Types = IncomeTypeRepository.GetAllIncomeTypes();
        }

        public static List<IncomeType> GetIncomeTypes(long accountId = 0)
        {
            if (Types == null || !Types.Any())
                Types = IncomeTypeRepository.GetAllIncomeTypes();

            return Types;
        }

        public static List<string> GetIncomeTypeNames(long accountId = 0)
        {
            return GetIncomeTypes(accountId).Select(c => c.Name).ToList();
        }

        public static long GetIncomeTypeId(string name)
        {
            return Types.Single(c => c.Name == name).IncomeTypeId;
        }

        public static void AddIncomeType(string name)
        {
            if (Types.Any(c => c.Name == name))
                return;

            IncomeTypeRepository.CreateIncomeType(name);
            Types = IncomeTypeRepository.GetAllIncomeTypes();
        }
    }
}

using sstocker.budget.Models;
using sstocker.budget.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sstocker.budget.Helpers
{
    public static class BankHelper
    {
        public static List<Bank> Banks;
        public static List<BankTypeModel> BankTypes;

        static BankHelper()
        {
            Banks = BankRepository.GetAllBanks();
            BankTypes = BankRepository.GetAllBankTypes();
        }

        public static List<Bank> GetBanks()
        {
            if (Banks == null || !Banks.Any())
                Banks = BankRepository.GetAllBanks();

            return Banks;
        }

        public static List<string> GetBankNames()
        {
            return GetBanks().Select(c => c.Name).ToList();
        }

        public static long GetBankId(string name)
        {
            return Banks.Single(c => c.Name == name).BankId;
        }

        public static void AddBank(string name)
        {
            if (Banks.Any(c => c.Name == name))
                return;

            BankRepository.CreateBank(name);
            Banks = BankRepository.GetAllBanks();
        }

        public static bool BankExists(string name)
        {
            return GetBankNames().Any(n => n == name);
        }

        public static long GetOrAddBankId(string name)
        {
            if (!BankExists(name))
                AddBank(name);
            return GetBankId(name);
        }

        public static List<BankTypeModel> GetBankTypes()
        {
            if (BankTypes == null || !BankTypes.Any())
                BankTypes = BankRepository.GetAllBankTypes();

            return BankTypes;
        }

        public static List<string> GetBankTypeNames()
        {
            return GetBankTypes().Select(c => c.BankType).ToList();
        }

        public static long GetBankTypeId(string name)
        {
            return BankTypes.Single(c => c.BankType == name).BankTypeId;
        }

        public static void AddBankType(string name)
        {
            if (BankTypes.Any(c => c.BankType == name))
                return;

            BankRepository.CreateBankType(name);
            BankTypes = BankRepository.GetAllBankTypes();
        }

        public static bool BankTypeExists(string name)
        {
            return GetBankTypeNames().Any(n => n == name);
        }

        public static long GetOrAddBankTypeId(string name)
        {
            if (!BankTypeExists(name))
                AddBankType(name);
            return GetBankTypeId(name);
        }
    }
}

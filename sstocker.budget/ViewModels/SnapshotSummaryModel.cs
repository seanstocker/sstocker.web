﻿using sstocker.budget.Models;
using sstocker.budget.Repositories;
using sstocker.core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sstocker.budget.ViewModels
{
    public class SnapshotSummaryModel : BaseViewModel
    {
        public List<Snapshot> Snapshots;

        public List<SnapshotTable> MonthlyBankSnapshots;
        public List<SnapshotTable> MonthlyBankTypeSnapshots;
        public List<SnapshotTable> MonthlyAllSnapshots;
        public List<SnapshotTable> YearlyBankSnapshots;
        public List<SnapshotTable> YearlyBankTypeSnapshots;
        public List<SnapshotTable> YearlyAllSnapshots;

        public decimal SnapshotTotal;
        public decimal SnapshotTotalWithoutDebt;
        public decimal SnapshotTotalInBank;

        private List<string> BankBankTypes = new List<string> { "Checking", "Saving" };

        public SnapshotSummaryModel(long accountId)
        {
            Snapshots = SnapshotRepository.GetAccountSnapshots(accountId).OrderByDescending(s => s.SnapShotDate).ToList();
            foreach (var snapshot in Snapshots.Where(snapshot => snapshot.IsDebt))
            {
                snapshot.Amount *= -1;
            }

            MonthlyBankSnapshots = new List<SnapshotTable>();
            MonthlyBankTypeSnapshots = new List<SnapshotTable>();
            MonthlyAllSnapshots = new List<SnapshotTable>();
            YearlyBankSnapshots = new List<SnapshotTable>();
            YearlyBankTypeSnapshots = new List<SnapshotTable>();
            YearlyAllSnapshots = new List<SnapshotTable>();

            var tempMonthlyBankSnapshots = new List<Snapshot>();
            var tempMonthlyBankTypeSnapshots = new List<Snapshot>();
            var tempMonthlyAllSnapshots = new List<Snapshot>();
            var tempYearlyBankSnapshots = new List<Snapshot>();
            var tempYearlyBankTypeSnapshots = new List<Snapshot>();
            var tempYearlyAllSnapshots = new List<Snapshot>();

            foreach (var snapShot in Snapshots)
            {
                var month = snapShot.SnapShotDate.Month;
                var year = snapShot.SnapShotDate.Year;
                var bank = snapShot.Bank;
                var bankType = snapShot.BankType;

                if (!tempMonthlyBankSnapshots.Any(s => s.SnapShotDate.Month == month && s.SnapShotDate.Year == year && s.Bank == bank && s.BankType == bankType))
                {
                    tempMonthlyBankSnapshots.Add(snapShot);
                }

                if (!tempMonthlyBankTypeSnapshots.Any(s => s.SnapShotDate.Month == month && s.SnapShotDate.Year == year && s.Bank == bank && s.BankType == bankType))
                {
                    tempMonthlyBankTypeSnapshots.Add(snapShot);
                }

                if (!tempMonthlyAllSnapshots.Any(s => s.SnapShotDate.Month == month && s.SnapShotDate.Year == year && s.Bank == bank && s.BankType == bankType))
                {
                    tempMonthlyAllSnapshots.Add(snapShot);
                }

                if (!tempYearlyBankSnapshots.Any(s => s.SnapShotDate.Year == year && s.Bank == bank && s.BankType == bankType))
                {
                    tempYearlyBankSnapshots.Add(snapShot);
                }

                if (!tempYearlyBankTypeSnapshots.Any(s => s.SnapShotDate.Year == year && s.Bank == bank && s.BankType == bankType))
                {
                    tempYearlyBankTypeSnapshots.Add(snapShot);
                }

                if (!tempYearlyAllSnapshots.Any(s => s.SnapShotDate.Year == year && s.Bank == bank && s.BankType == bankType))
                {
                    tempYearlyAllSnapshots.Add(snapShot);
                }
            }

            MonthlyBankSnapshots = tempMonthlyBankSnapshots.GroupBy(s => s.Bank).Select(g => new SnapshotTable(g.Key, g.GroupBy(s => new { s.SnapShotDate.Month, s.SnapShotDate.Year }).Select(i => new TableRow { Month = i.Key.Month, Year = i.Key.Year, Amount = i.Sum(x => x.Amount) }))).OrderBy(o => o.Title).ToList();
            MonthlyBankTypeSnapshots = tempMonthlyBankTypeSnapshots.GroupBy(s => s.BankType).Select(g => new SnapshotTable(g.Key, g.GroupBy(s => new { s.SnapShotDate.Month, s.SnapShotDate.Year }).Select(i => new TableRow { Month = i.Key.Month, Year = i.Key.Year, Amount = i.Sum(x => x.Amount) }))).OrderBy(o => o.Title).ToList();
            MonthlyAllSnapshots = tempMonthlyAllSnapshots.GroupBy(s => new { s.Bank, s.BankType }).Select(g => new SnapshotTable($"{g.Key.Bank} - {g.Key.BankType}", g.GroupBy(s => new { s.SnapShotDate.Month, s.SnapShotDate.Year }).Select(i => new TableRow { Month = i.Key.Month, Year = i.Key.Year, Amount = i.Sum(x => x.Amount) }))).OrderBy(o => o.Title).ToList();

            YearlyBankSnapshots = tempYearlyBankSnapshots.GroupBy(s => s.Bank).Select(g => new SnapshotTable(g.Key, g.GroupBy(s => s.SnapShotDate.Year).Select(i => new TableRow { Month = 1, Year = i.Key, Amount = i.Sum(x => x.Amount) }))).OrderBy(o => o.Title).ToList();
            YearlyBankTypeSnapshots = tempYearlyBankTypeSnapshots.GroupBy(s => s.BankType).Select(g => new SnapshotTable(g.Key, g.GroupBy(s => s.SnapShotDate.Year).Select(i => new TableRow { Month = 1, Year = i.Key, Amount = i.Sum(x => x.Amount) }))).OrderBy(o => o.Title).ToList();
            YearlyAllSnapshots = tempYearlyAllSnapshots.GroupBy(s => new { s.Bank, s.BankType }).Select(g => new SnapshotTable($"{g.Key.Bank} - {g.Key.BankType}", g.GroupBy(s => s.SnapShotDate.Year).Select(i => new TableRow { Month = 1, Year = i.Key, Amount = i.Sum(x => x.Amount) }))).OrderBy(o => o.Title).ToList();

            SnapshotTotal = Snapshots.GroupBy(s => new { s.Bank, s.BankType }).Sum(s => s.OrderByDescending(i => i.SnapShotDate).First().Amount);
            SnapshotTotalWithoutDebt = Snapshots.Where(s => !s.IsDebt).GroupBy(s => new { s.Bank, s.BankType }).Sum(s => s.OrderByDescending(i => i.SnapShotDate).First().Amount);
            SnapshotTotalInBank = Snapshots.Where(s => BankBankTypes.Contains(s.BankType)).GroupBy(s => new { s.Bank, s.BankType }).Sum(s => s.OrderByDescending(i => i.SnapShotDate).First().Amount);
        }

        public class SnapshotTable
        {
            public string Title;
            public List<TableRow> Snapshots;

            public SnapshotTable(string title, IEnumerable<TableRow> snapshots)
            {
                Title = title;
                Snapshots = snapshots.ToList(); ;
            }
        }

        public class TableRow
        {
            public int Month;
            public string MonthName { get => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month); }
            public int Year;
            public decimal Amount;
        }
    }
}
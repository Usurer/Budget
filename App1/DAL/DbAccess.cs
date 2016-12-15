using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SQLite;

using App1.Entities;

namespace App1.DAL
{
    class DbAccess
    {
        public bool AddCategory(string name)
        {
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = System.IO.Path.Combine(folder, "sqlite2.db");

            using (var connection = new SQLiteConnection(dbPath))
            {
                var category = new Category { Name = name };
                connection.Insert(category);
                connection.Close();
            }

            return true;
        }

        public Category[] GetCategories()
        {
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = System.IO.Path.Combine(folder, "sqlite2.db");

            var result = Enumerable.Empty<Category>();

            using (var connection = new SQLiteConnection(dbPath))
            {
                result = connection.Table<Category>().ToArray();
            }

            return result.ToArray();
        }

        public bool AddPeriod(DateTime dateOfMonth)
        {
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = System.IO.Path.Combine(folder, "sqlite2.db");

            using (var connection = new SQLiteConnection(dbPath))
            {
                var start = dateOfMonth.AddDays(-dateOfMonth.Day + 1);
                var end = dateOfMonth.AddMonths(1).AddDays(-dateOfMonth.Day);
                var period = new Period { Start = start, End = end };

                connection.Insert(period);
                connection.Close();
            }

            return true;
        }

        public Period[] GetPeriods()
        {
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = System.IO.Path.Combine(folder, "sqlite2.db");

            var result = Enumerable.Empty<Period>();

            using (var connection = new SQLiteConnection(dbPath))
            {
                result = connection.Table<Period>().ToArray();
            }

            return result.ToArray();
        }

        public bool AddBudget(Category category, Period period, float amount)
        {
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = System.IO.Path.Combine(folder, "sqlite2.db");

            using (var connection = new SQLiteConnection(dbPath))
            {
                var budget = new Budget { CategoryId = category.Id, PeriodId = period.Id, Amount = amount };
                connection.Insert(budget);
                connection.Close();
            }

            return true;
        }

        public Budget[] GetBudgets()
        {
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = System.IO.Path.Combine(folder, "sqlite2.db");

            var result = Enumerable.Empty<Budget>();

            using (var connection = new SQLiteConnection(dbPath))
            {
                result = connection.Table<Budget>().ToArray();
            }

            return result.ToArray();
        }

        public bool AddTransaction(Category category, DateTime date, float amount, string note)
        {
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = System.IO.Path.Combine(folder, "sqlite2.db");

            using (var connection = new SQLiteConnection(dbPath))
            {
                var transaction = new Transaction { CategoryId = category.Id, Date = date, Amount = amount, Note = note };
                connection.Insert(transaction);
                connection.Close();
            }

            return true;
        }

        public Transaction[] GetTransactions()
        {
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = System.IO.Path.Combine(folder, "sqlite2.db");

            var result = Enumerable.Empty<Transaction>();

            using (var connection = new SQLiteConnection(dbPath))
            {
                result = connection.Table<Transaction>().ToArray();
            }

            return result.ToArray();
        }

        public string CreateDatabaseIfRequired()
        {

            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = System.IO.Path.Combine(folder, "sqlite2.db");

            if(!System.IO.File.Exists(dbPath))
            {
                CreateDatabase(dbPath);
                return "DB Created";
            }
            return "DB Exists";
        }

        public void CleanDb()
        {
            var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbPath = System.IO.Path.Combine(folder, "sqlite2.db");

            if (System.IO.File.Exists(dbPath))
            {
                System.IO.File.Delete(dbPath);
            }

            CreateDatabase(dbPath);
        }

        private void CreateDatabase(string path)
        {
            using (var connection = new SQLiteConnection(path))
            {
                connection.CreateTable<Category>();
                connection.CreateTable<Period>();
                connection.CreateTable<Transaction>();
                connection.CreateTable<Budget>();
                connection.Close();
            }
        }
    }
}
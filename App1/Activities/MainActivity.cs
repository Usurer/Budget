using System;
using System.Collections.Generic;
using System.Globalization;
using Android.App;
using Android.Widget;
using Android.OS;

using App1.DAL;
using App1.Entities;
using System.Linq;
using Android.Content;
using App1.Adapters;

namespace App1.Activities
{
    [Activity(Label = "App1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button BtnAddCategory { get; set; }

        private Button BtnAddTransaction { get; set; }

        private Button BtnAddBudget { get; set; }

        private Button BtnCleanDb { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            CreateDbIfRequired();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            EnableMainView();
        }

        #region Main view

        private void EnableMainView()
        {
            SetContentView(Resource.Layout.Main);
            BtnAddCategory = FindViewById<Button>(Resource.Id.btnAddCategory);
            BtnAddCategory.Click += AddCategory_Click;

            BtnAddBudget = FindViewById<Button>(Resource.Id.btnAddBudget);
            BtnAddBudget.Click += BtnAddBudget_Click;

            BtnAddTransaction = FindViewById<Button>(Resource.Id.btnAddTransaction);
            BtnAddTransaction.Click += BtnAddTransaction_Click;

            BtnCleanDb = FindViewById<Button>(Resource.Id.btnCleanDb);
            BtnCleanDb.Click += BtnCleanDb_Click;

            LoadCategories();
            LoadBudget();
        }

        private void LoadBudget()
        {
            var dbAccess = new DbAccess();
            var budgets = dbAccess.GetBudgets();
            var period = dbAccess.GetPeriods().Where(x => x.Start <= DateTime.Now && x.End >= DateTime.Now).SingleOrDefault();
            if (period == null)
            {
                return;
            }

            var allBudgetsForPeriod = budgets.Where(x => x.PeriodId == period.Id).ToList();
            if(!allBudgetsForPeriod.Any())
            {
                return;
            }

            var total = allBudgetsForPeriod.Sum(x => x.Amount);

            var transactions = dbAccess.GetTransactions();
            var spent = transactions.Where(x => x.Date >= period.Start && x.Date <= period.End).Sum(x => x.Amount);

            var ctrlLeft = FindViewById<TextView>(Resource.Id.textBudgetLeft);
            var ctrlTotal = FindViewById<TextView>(Resource.Id.textBudgetTotal);

            ctrlLeft.Text = $"{total - spent} left of total ";
            ctrlTotal.Text = $"{total} money in monthly budget";
        }

        private void LoadCategories()
        {
            var dbAccess = new DbAccess();
            var categories = dbAccess.GetCategories();
            var transactions = dbAccess.GetTransactions();

            var categoryBudgets = new List<CategoryBudget>();

            foreach (var category in categories)
            {
                var budget = dbAccess.GetBudgets().FirstOrDefault(x => x.CategoryId == category.Id) 
                    ?? new Budget { Amount = 0 };

                // Getting transactions for current category and month
                var categoryTransactions = transactions.Where(x => x.CategoryId == category.Id && x.Date.Month == DateTime.Now.Month);
                foreach (var t in categoryTransactions)
                {
                    budget.Amount = budget.Amount - t.Amount;
                }

                categoryBudgets.Add(new CategoryBudget
                {
                    Category = category,
                    Budget = budget,
                });
            }

            var list = FindViewById<ListView>(Resource.Id.list);
            list.Adapter = new CategoryBudgetListAdapter(this, categoryBudgets);
        }

        private void BtnCleanDb_Click(object sender, EventArgs e)
        {
            var dbAccess = new DbAccess();
            dbAccess.CleanDb();
            
            LoadCategories();
        }

        private void AddCategory_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CreateCategoryActivity));
            StartActivity(intent);
        }

        private void BtnAddBudget_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CreateBudgetActivity));
            StartActivity(intent);
        }

        private void BtnAddTransaction_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CreateTransactionActivity));
            StartActivity(intent);
        }

        #endregion

        private void CreateDbIfRequired()
        {
            var dbAccess = new DbAccess();
            var msg = "Hello";

            try
            {
                msg = dbAccess.CreateDatabaseIfRequired();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            var label = new Android.Widget.TextView(this)
            {
                Text = msg,
                TextSize = 36
            };
            this.SetContentView(label);
        }
    }
}


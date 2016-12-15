using System;

using Android.App;
using Android.Widget;
using Android.OS;

using App1.DAL;
using App1.Entities;
using System.Linq;

namespace App1
{
    [Activity(Label = "App1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button BtnAddCategory { get; set; }

        private Button BtnAddTransaction { get; set; }

        private Button BtnSaveCategory { get; set; }

        private Button BtnCancelCategory { get; set; }

        private Button BtnAddBudget { get; set; }

        private Button BtnSaveBudget { get; set; }

        private Button BtnCancelBudget { get; set; }

        private Button BtnSaveTransaction { get; set; }

        private Button BtnCancelTransaction { get; set; }

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
        }

        private void LoadCategories()
        {
            var container = FindViewById<LinearLayout>(Resource.Id.containerCategories);
            var dbAccess = new DbAccess();
            var categories = dbAccess.GetCategories();
            var transactions = dbAccess.GetTransactions();

            container.RemoveAllViews();

            foreach (var category in categories)
            {
                var view = new Button(this.BaseContext);
                view.Text = category.Name;
                view.Gravity = Android.Views.GravityFlags.Top;

                var budget = dbAccess.GetBudgets().Where(x => x.CategoryId == category.Id).FirstOrDefault();
                var spend = 0f;
                // Getting transactions for current category and month
                var categoryTransactions = transactions.Where(x => x.CategoryId == category.Id && x.Date.Month == DateTime.Now.Month);
                foreach(var t in categoryTransactions)
                {
                    spend = spend + t.Amount;
                }

                view.Text = view.Text + " " + spend;

                if (budget != null)
                {
                    view.Text = view.Text + " / " + budget.Amount.ToString();
                }

                container.AddView(view);
            }
        }

        private void BtnCleanDb_Click(object sender, EventArgs e)
        {
            var dbAccess = new DbAccess();
            dbAccess.CleanDb();
            
            LoadCategories();
        }

        private void AddCategory_Click(object sender, EventArgs e)
        {
            EnableCreateCategoryView();
        }

        private void BtnAddBudget_Click(object sender, EventArgs e)
        {
            EnableCreateBudgetView();
        }

        private void BtnAddTransaction_Click(object sender, EventArgs e)
        {
            EnableCreateTransactionView();
        }

        #endregion

        #region Create category

        private void BtnCancelCategory_Click(object sender, EventArgs e)
        {
            EnableMainView();
        }

        private void BtnSaveCategory_Click(object sender, EventArgs e)
        {
            var text = FindViewById<EditText>(Resource.Id.txtCreateCategory_Name).Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                var dbAccess = new DbAccess();
                dbAccess.AddCategory(text);
            }

            EnableMainView();
        }

        private void EnableCreateCategoryView()
        {
            SetContentView(Resource.Layout.CreateCategory);

            BtnSaveCategory = FindViewById<Button>(Resource.Id.btnCreateCategory_Save);
            BtnCancelCategory = FindViewById<Button>(Resource.Id.btnCreateCategory_Cancel);

            BtnSaveCategory.Click += BtnSaveCategory_Click;
            BtnCancelCategory.Click += BtnCancelCategory_Click;
        }

        #endregion

        #region Create budget

        private void EnableCreateBudgetView()
        {
            SetContentView(Resource.Layout.CreateBudget);

            BtnSaveBudget = FindViewById<Button>(Resource.Id.btnCreateBudget_Save);
            BtnCancelBudget = FindViewById<Button>(Resource.Id.btnCreateBudget_Cancel);

            BtnSaveBudget.Click += BtnSaveBudget_Click;
            BtnCancelBudget.Click += BtnCancelBudget_Click;

            var spinner = FindViewById<Spinner>(Resource.Id.drdCreateBudget_Category);
            var adapter = new ArrayAdapter<Category>(this, Android.Resource.Layout.SimpleSpinnerItem, GetCategories());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
        }

        private void BtnCancelBudget_Click(object sender, EventArgs e)
        {
            SelectedCategory = null;
            EnableMainView();
        }

        private void BtnSaveBudget_Click(object sender, EventArgs e)
        {
            var spinner = FindViewById<Spinner>(Resource.Id.drdCreateBudget_Category);
            var dateTime = FindViewById<DatePicker>(Resource.Id.dateCreateBudget_Month).DateTime;
            var amountString = FindViewById<EditText>(Resource.Id.txtCreateBudget_Amount).Text;
            float amountNum = 0;
            float.TryParse(amountString, out amountNum);

            var db = new DbAccess();
            var period = db.GetPeriods().Where(x => x.Start <= dateTime && x.End >= dateTime).FirstOrDefault();

            if(period == null)
            {
                db.AddPeriod(dateTime);
                period = db.GetPeriods().Where(x => x.Start <= dateTime && x.End >= dateTime).FirstOrDefault();
            }

            if (SelectedCategory != null)
            {
                db.AddBudget(SelectedCategory, period, amountNum);
            }

            SelectedCategory = null;
            EnableMainView();
        }

        private Category SelectedCategory { get; set; }        

        #endregion

        #region Create transaction

        private void EnableCreateTransactionView()
        {
            SetContentView(Resource.Layout.CreateTransaction);

            BtnSaveTransaction = FindViewById<Button>(Resource.Id.btnCreateTransaction_Save);
            BtnCancelTransaction = FindViewById<Button>(Resource.Id.btnCreateTransaction_Cancel);

            BtnSaveTransaction.Click += BtnSaveTransaction_Click;
            BtnCancelTransaction.Click += BtnCancelTransaction_Click;

            var spinner = FindViewById<Spinner>(Resource.Id.drdCreateTransaction_Category);
            var adapter = new ArrayAdapter<Category>(this, Android.Resource.Layout.SimpleSpinnerItem, GetCategories());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
        }

        private void BtnCancelTransaction_Click(object sender, EventArgs e)
        {
            SelectedCategory = null;
            EnableMainView();
        }

        private void BtnSaveTransaction_Click(object sender, EventArgs e)
        {
            var spinner = FindViewById<Spinner>(Resource.Id.drdCreateTransaction_Category);
            var dateTime = FindViewById<DatePicker>(Resource.Id.dateCreateTransaction_Month).DateTime;
            var amountString = FindViewById<EditText>(Resource.Id.txtCreateTransaction_Amount).Text;
            float amountNum = 0;
            float.TryParse(amountString, out amountNum);

            var db = new DbAccess();
            var period = db.GetPeriods().Where(x => x.Start <= dateTime && x.End >= dateTime).FirstOrDefault();

            if (period == null)
            {
                db.AddPeriod(dateTime);
                period = db.GetPeriods().Where(x => x.Start <= dateTime && x.End >= dateTime).FirstOrDefault();
            }

            if (SelectedCategory != null)
            {
                db.AddTransaction(SelectedCategory, dateTime, amountNum, string.Empty);
            }

            SelectedCategory = null;
            EnableMainView();
        }

        #endregion

        private Category[] GetCategories()
        {
            var dbAccess = new DbAccess();
            return dbAccess.GetCategories();
        }

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

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            var a = spinner.Adapter as ArrayAdapter<Category>;
            SelectedCategory = a.GetItem(e.Position);
        }
    }
}


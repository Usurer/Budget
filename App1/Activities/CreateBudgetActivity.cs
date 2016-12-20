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
using App1.DAL;
using App1.Entities;

namespace App1.Activities
{
    [Activity(Label = "CreateBudgetActivity")]
    public class CreateBudgetActivity : Activity
    {
        private Button BtnSave { get; set; }

        private Button BtnCancel { get; set; }

        private Category SelectedCategory { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateBudget);

            var spinner = FindViewById<Spinner>(Resource.Id.spnCategory);
            var adapter = new ArrayAdapter<Category>(this, Android.Resource.Layout.SimpleSpinnerItem, GetCategories());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);

            BtnSave = FindViewById<Button>(Resource.Id.btnSave);
            BtnCancel = FindViewById<Button>(Resource.Id.btnCancel);

            BtnSave.Click += BtnSave_Click;
            BtnCancel.Click += BtnCancel_Click;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var dateTime = FindViewById<DatePicker>(Resource.Id.dateMonth).DateTime;
            var amountString = FindViewById<EditText>(Resource.Id.txtAmount).Text;
            float amountNum = 0;
            float.TryParse(amountString, out amountNum);

            var db = new DbAccess();
            var period = db.GetPeriods().FirstOrDefault(x => x.Start <= dateTime && x.End >= dateTime);

            if (period == null)
            {
                db.AddPeriod(dateTime);
                period = db.GetPeriods().FirstOrDefault(x => x.Start <= dateTime && x.End >= dateTime);
            }

            if (SelectedCategory != null)
            {
                db.AddBudget(SelectedCategory, period, amountNum);
            }

            SelectedCategory = null;

            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        private Category[] GetCategories()
        {
            var dbAccess = new DbAccess();
            return dbAccess.GetCategories();
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            var adapter = (ArrayAdapter<Category>)spinner.Adapter;
            SelectedCategory = adapter.GetItem(e.Position);
        }
    }
}
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

namespace App1.Activities
{
    [Activity(Label = "EditCategoryActivity")]
    public class CreateCategoryActivity : Activity
    {
        private Button BtnSave { get; set; }

        private Button BtnCancel { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateCategory);

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
            var text = FindViewById<EditText>(Resource.Id.txtName).Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                var dbAccess = new DbAccess();
                dbAccess.AddCategory(text);
            }

            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}
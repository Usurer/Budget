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
using SQLiteNetExtensions.Attributes;

namespace App1.Entities
{
    class Budget
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }

        [ForeignKey(typeof(Period))]
        public int PeriodId { get; set; }

        public float Amount { get; set; }
    }
}
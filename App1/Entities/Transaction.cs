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
    class Transaction
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }

        public float Amount { get; set; }

        public DateTime Date { get; set; }

        public string Note { get; set; }
    }
}
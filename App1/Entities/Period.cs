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

namespace App1.Entities
{
    class Period
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
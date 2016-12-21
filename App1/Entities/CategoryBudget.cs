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

namespace App1.Entities
{
    class CategoryBudget
    {
        public Category Category { get; set; }

        public Budget Budget { get; set; }

        public Period Period { get; set; }
    }
}
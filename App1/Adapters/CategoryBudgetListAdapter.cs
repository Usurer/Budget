using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Entities;

namespace App1.Adapters
{
    class CategoryBudgetListAdapter : BaseAdapter<CategoryBudget>
    {
        private CategoryBudget[] items;
        private Activity context;

        public CategoryBudgetListAdapter(Activity context, IEnumerable<CategoryBudget> items)
        {
            this.items = items.ToArray();
            this.context = context;
        }

        public override long GetItemId(int position)
        {
            return items[position].Category.Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Resource.Layout.Main_CategoryListViewItem, null);
            view.FindViewById<TextView>(Resource.Id.text1).Text = items[position].Category.Name;
            view.FindViewById<TextView>(Resource.Id.text2).Text = items[position].Budget.Amount.ToString(CultureInfo.InvariantCulture);
            return view;
        }

        public override int Count
        {
            get { return items.Length; }
        }

        public override CategoryBudget this[int position]
        {
            get { return items[position]; }
        }
    }
}
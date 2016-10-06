using Android.App;
using Android.Views;
using Android.Widget;
using DigitizeAgri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitizeAgri.Droid
{
    public class WeatherAnalysisAdapter : BaseAdapter<DateTime>
    {
        List<DateTime> items;
        Activity context;
        public WeatherAnalysisAdapter(Activity context, List<DateTime> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override DateTime this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.wa_adapter, null);
            view.FindViewById<TextView>(Resource.Id.day).Text = item.DayOfWeek.ToString();
            view.FindViewById<TextView>(Resource.Id.date).Text = item.Day.ToString();
            return view;
        }
    }
}

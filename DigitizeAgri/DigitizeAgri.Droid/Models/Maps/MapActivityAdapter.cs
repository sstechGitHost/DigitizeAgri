using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Views;
using Android.Widget;

namespace DigitizeAgri.Droid.Models.Maps
{
    internal class SimpleMapDemoActivityAdapter : BaseAdapter<ActivitiesMetaData>
    {
        private readonly List<ActivitiesMetaData> _activities;
        private readonly Context _context;

        public SimpleMapDemoActivityAdapter(Context context, IEnumerable<ActivitiesMetaData> sampleActivities)
        {
            _context = context;
            if (sampleActivities == null)
            {
                _activities = new List<ActivitiesMetaData>(0);
            }
            else
            {
                _activities = sampleActivities.ToList();
            }
        }

        public override int Count { get { return _activities.Count; } }

        public override ActivitiesMetaData this[int position] { get { return _activities[position]; } }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            FeatureRowHolder row = convertView as FeatureRowHolder ?? new FeatureRowHolder(_context);
            ActivitiesMetaData sample = _activities[position];

            row.UpdateFrom(sample);
            return row;
        }
    }
}
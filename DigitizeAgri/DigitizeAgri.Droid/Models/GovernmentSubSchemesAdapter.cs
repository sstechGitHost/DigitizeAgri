
using Android.App;
using Android.Views;
using Android.Widget;
using DigitizeAgri.Droid.Model;
using DigitizeAgri.Models;
using System.Collections.Generic;
namespace DigitizeAgri.Droid
{
    internal class GovernmentSubSchemesAdapterWrapper : Java.Lang.Object
    {
        public TextView lblName { get; set; }
        public TextView lblInfo { get; set; }
    }
    class GovernmentSubSchemesAdapter : BaseAdapter
    {
        private readonly Activity context;
        private readonly List<Scheme> sclist;

        public GovernmentSubSchemesAdapter(Activity context, List<Scheme> list)
        {
            this.context = context;
            this.sclist = list;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position < 0)
                return null;

            var view = (convertView
                ?? context.LayoutInflater.Inflate(
                    Resource.Layout.gssub_adapter, parent, false)
            );

            if (view == null)
                return null;

            var wrapper = view.Tag as GovernmentSubSchemesAdapterWrapper;
            if (wrapper == null)
            {
                wrapper = new GovernmentSubSchemesAdapterWrapper
                {
                    lblName = view.FindViewById<TextView>(Resource.Id.lblName),
                    lblInfo = view.FindViewById<TextView>(Resource.Id.lblInfo)
                };
                view.Tag = wrapper;
            }

            var item = sclist[position];

            wrapper.lblName.Text = item.name;          
            wrapper.lblInfo.Text = "?";

            return view;
        }
        public override int Count
        {
            get { return sclist.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }
    }
}
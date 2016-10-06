
using Android.App;
using Android.Views;
using Android.Widget;
using DigitizeAgri.Droid.Model;
using DigitizeAgri.Models;
using System.Collections.Generic;
namespace DigitizeAgri.Droid
{
    internal class GovernmentSchemesAdapterWrapper : Java.Lang.Object
    {
        public TextView lblName { get; set; }
        public TextView lblTotal { get; set; }
        public TextView lblNew { get; set; }
        public TextView lblInfo { get; set; }
        public TextView lblnsa { get; set; }
    }
    class GovernmentSchemesAdapter : BaseAdapter
    {
        private readonly Activity context;
        private readonly List<Scheme_Category> sclist;

        public GovernmentSchemesAdapter(Activity context, List<Scheme_Category> list)
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
                    Resource.Layout.gs_adapter, parent, false)
            );

            if (view == null)
                return null;

            var wrapper = view.Tag as GovernmentSchemesAdapterWrapper;
            if (wrapper == null)
            {
                wrapper = new GovernmentSchemesAdapterWrapper
                {
                    lblName = view.FindViewById<TextView>(Resource.Id.lblName),
                    lblTotal = view.FindViewById<TextView>(Resource.Id.lblTotal),
                    lblNew = view.FindViewById<TextView>(Resource.Id.lblNew),
                    lblInfo = view.FindViewById<TextView>(Resource.Id.lblInfo),
                    lblnsa = view.FindViewById<TextView>(Resource.Id.NSA)
                };
                view.Tag = wrapper;
            }

            var item = sclist[position];

            wrapper.lblName.Text = item.description;
           
            //if (!string.IsNullOrEmpty(item.Total) || !string.IsNullOrEmpty(item.New))
            //{
            //    wrapper.lblnsa.Visibility = ViewStates.Gone;
            //    wrapper.lblTotal.Visibility = ViewStates.Visible;
            //    wrapper.lblNew.Visibility = ViewStates.Visible;
            //    wrapper.lblTotal.Text = item.Total;
            //    wrapper.lblNew.Text = item.New;
            //}
            //else
            //{
            //    wrapper.lblTotal.Visibility = ViewStates.Gone;
            //    wrapper.lblNew.Visibility = ViewStates.Gone;
            //    wrapper.lblnsa.Visibility = ViewStates.Visible;
            //    wrapper.lblnsa.Text = "No Schemes Available";
            //}
            wrapper.lblTotal.Text = "10";
            wrapper.lblNew.Text = "1";
            //wrapper.lblInfo.Text = item.Info;
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
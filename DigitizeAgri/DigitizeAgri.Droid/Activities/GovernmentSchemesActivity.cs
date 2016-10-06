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
using DigitizeAgri.Droid.Model;
using DigitizeAgri.Models;
using Newtonsoft.Json;

namespace DigitizeAgri.Droid.Activities
{
    [Activity(Label = "Government Schemes")]
    public class GovernmentSchemesActivity : Activity
    {
        public ListView listView;
        public List<Scheme_Category> sclist = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.govt_schemes);
            listView = FindViewById<ListView>(Resource.Id.List);
            SchemeCategories();
        }

        private async void SchemeCategories()
        {
            SAL.Services sal = new SAL.Services();
            sclist = await sal.GetSchemeCategories();
            if (sclist != null || sclist.Count > 0)
            {
                listView.Adapter = new GovernmentSchemesAdapter(this, sclist);
                listView.ItemClick += OnListItemClick;
            }
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var category = sclist[e.Position].category;
            var description = sclist[e.Position].description;
            Scheme_Category scObj = new Scheme_Category();
            scObj.category = category;
            scObj.description = description;
            var intent = new Intent(this, typeof(GovernmentSubSchemesActivity));
            intent.PutExtra("SchemeCategorys", JsonConvert.SerializeObject(scObj));
            StartActivity(intent);
        }
    }
}
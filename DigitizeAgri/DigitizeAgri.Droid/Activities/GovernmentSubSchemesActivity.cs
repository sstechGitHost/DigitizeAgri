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
using DigitizeAgri.Models;
using Newtonsoft.Json;

namespace DigitizeAgri.Droid.Activities
{
    [Activity(Label = "Government Schemes")]
    public class GovernmentSubSchemesActivity : Activity
    {
        Scheme_Category schemecategorysObj;
        string Category=string.Empty;
        string Description = string.Empty;
        List<Scheme> sslist = new List<Scheme>();
        public ListView listView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.schemecategorysObj = JsonConvert.DeserializeObject<Scheme_Category>(Intent.GetStringExtra("SchemeCategorys"));
            
            // Create your application here
            SetContentView(Resource.Layout.govt_subschemes);
            Category = schemecategorysObj.category;
            Description = schemecategorysObj.description;
            TextView header = FindViewById<TextView>(Resource.Id.subcategory);
            header.Text = Description + " Programs";
            listView = FindViewById<ListView>(Resource.Id.sublist);
            Scheme();
            Button btnBack = FindViewById<Button>(Resource.Id.back);
            btnBack.Click += (object sender, EventArgs e) =>
            {
                base.OnBackPressed();
            };
        }

        private async void Scheme()
        {
            SAL.Services sal = new SAL.Services();
            sslist = await sal.GetScheme(Category);
            if (sslist != null || sslist.Count > 0)
            {
                listView.Adapter = new GovernmentSubSchemesAdapter(this, sslist);
            }
        }
        public override void OnBackPressed()
        {
                base.OnBackPressed();
        }
    }
}
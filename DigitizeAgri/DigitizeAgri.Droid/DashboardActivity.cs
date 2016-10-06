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
using DigitizeAgri.Droid.Activities;

namespace DigitizeAgri.Droid
{
    [Activity(Label = "Dashboard")]
    public class DashboardActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);
            Button weatherAnalysisbutton = FindViewById<Button>(Resource.Id.weatherAnalysis);
            Button govtBenefitSchems = FindViewById<Button>(Resource.Id.govtSchemes);
            Button iOTBtn = FindViewById<Button>(Resource.Id.iOTAgriSensor);

            weatherAnalysisbutton.Click += (object sender, EventArgs e) => {
                Console.WriteLine("Weather Analysis Clicked!!");
            };

            govtBenefitSchems.Click += (object sender, EventArgs e) =>
            {
                var intent = new Intent(this, typeof(GovernmentSchemesActivity));
                
                //var geoUri = Android.Net.Uri.Parse("geo:17.385044,78.486671");
                //var geoUri = Android.Net.Uri.Parse("geo:17.385044,78.486671?z=23");
                //var geoUri = Android.Net.Uri.Parse("geo:0,0?q=Home");
                //var intent = new Intent(Intent.ActionView, geoUri);
                StartActivity(intent);
            };

            iOTBtn.Click += async(sender, e) => {

                SAL.Services sal = new SAL.Services();

                string iOTData = await sal.GetIOTData();

                var intent = new Intent(this, typeof(iOTSensorActivity));
                intent.PutExtra("iOTData", iOTData);
                StartActivity(intent);
            };
            weatherAnalysisbutton.Click += (object sender, EventArgs e) =>
            {
                var intent = new Intent(this, typeof(WeatherAnalysisActivity));

                //var intent = new Intent(this, typeof(GeoLocActivity));
                StartActivity(intent);

                //var geoUri = Android.Net.Uri.Parse("geo:17.385044,78.486671");
                //var mapIntent = new Intent(Intent.ActionView, geoUri);
                //StartActivity(mapIntent);

            };

        }
    }
}
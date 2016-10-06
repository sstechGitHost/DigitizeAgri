using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using AndroidUri = Android.Net.Uri;
using DigitizeAgri.Droid.Models.Maps;
using DigitizeAgri.Droid.Activities.Maps;

namespace DigitizeAgri.Droid.Activities
{

    [Activity(Label = "@string/app_name", Icon = "@drawable/icon",MainLauncher=true)]
    public class BasicGeoTagDemoActivity : ListActivity
    {
        public static readonly int InstallGooglePlayServicesId = 1000;
        public static readonly string Tag = "XamarinMapDemo";

        private List<ActivitiesMetaData> _activities;
        private bool _isGooglePlayServicesInstalled;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (resultCode)
            {
                case Result.Ok:
                    // Try again.
                    _isGooglePlayServicesInstalled = true;
                    break;

                default:
                    Log.Debug("MainActivity", "Unknown resultCode {0} for request {1}", resultCode, requestCode);
                    break;
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _isGooglePlayServicesInstalled = TestIfGooglePlayServicesIsInstalled();
            InitializeListView();
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            ActivitiesMetaData activity = _activities[position];
            activity.Start(this);
        }

        private void InitializeListView()
        {
            if (_isGooglePlayServicesInstalled)
            {
                _activities = new List<ActivitiesMetaData>
                                  {
                                      new ActivitiesMetaData(Resource.String.activity_label_autoCaptureLocation, Resource.String.activity_description_autoCaptureLocation, typeof(AutoCaptureLocation)),
                                      new ActivitiesMetaData(Resource.String.activity_label_mapwithmarkers, Resource.String.activity_description_mapwithmarkers, typeof(TapCaptureLocationActivity))
                                  };

                ListAdapter = new SimpleMapDemoActivityAdapter(this, _activities);
            }
            else
            {
                Log.Error("MainActivity", "Google Play Services is not installed");
                ListAdapter = new SimpleMapDemoActivityAdapter(this, null);
            }
        }

        private bool TestIfGooglePlayServicesIsInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(Tag, "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error(Tag, "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
                Dialog errorDialog = GoogleApiAvailability.Instance.GetErrorDialog(this, queryResult, InstallGooglePlayServicesId);
                DigitizeAgri.Droid.Models.Maps.ErrorDialogFragment dialogFrag = new DigitizeAgri.Droid.Models.Maps.ErrorDialogFragment(errorDialog);
                

                dialogFrag.Show(FragmentManager, "GooglePlayServicesDialog");
            }
            return false;
        }
    }
}
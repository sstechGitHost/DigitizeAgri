using System;
using Android.App;
using Android.OS;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Util;
using Android.Widget;
using Android.Locations;
using Android.Gms.Maps;
using System.Collections.Generic;
using Android.Gms.Maps.Model;
using Android.Graphics;
using System.Linq;

namespace DigitizeAgri.Droid.Activities.Maps
{
    [Activity(Label = "FusedLocationProvider")]
    public class AutoCaptureLocation : Activity, GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        GoogleApiClient apiClient;
        LocationRequest locRequest;


        bool _isGooglePlayServicesInstalled;

        ////Lifecycle methods
        private MapFragment _mapFragment;
        GoogleMap map;
        TextView tvvalues = null;
        Button btntake = null;
        Button btndone;
        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Log.Debug("OnCreate", "OnCreate called, initializing views...");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.AutoCaptureLocation);

            _mapFragment = FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map);
            GoogleMapOptions mapOptions = new GoogleMapOptions()
                   .InvokeMapType(GoogleMap.MapTypeSatellite)
                   .InvokeZoomControlsEnabled(false)
                   .InvokeCompassEnabled(true);
            _mapFragment.Map.MapType = GoogleMap.MapTypeSatellite;
            map = _mapFragment.Map;

            _isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();

            if (_isGooglePlayServicesInstalled)
            {
                // pass in the Context, ConnectionListener and ConnectionFailedListener
                apiClient = new GoogleApiClient.Builder(this, this, this)
                    .AddApi(LocationServices.API).Build();

                // generate a location request that we will pass into a call for location updates
                locRequest = new LocationRequest();

            }
            else
            {
                Log.Error("OnCreate", "Google Play Services is not installed");
                Toast.MakeText(this, "Google Play Services is not installed", ToastLength.Long).Show();
                Finish();
            }
            tvvalues = FindViewById<TextView>(Resource.Id.tvvalues);
            btndone = FindViewById<Button>(Resource.Id.btndone);
            btntake = FindViewById<Button>(Resource.Id.btntake);
            var btnClear = FindViewById<Button>(Resource.Id.btnClear);
            Location location = LocationServices.FusedLocationApi.GetLastLocation(apiClient);
            btnClear.Click += (sender, e) =>
            {
                map.Clear();

                locations.Clear();
            };

            btntake.Click += (sender, e) =>
            {
                Isstart = true;
                map.Clear();
                SetupPolygon();

            };




        }
        public void SetupLine()
        {
            if (locations.Count != 0)
            {
                var latLngPoints = new LatLng[locations.Count];
                int index = 0;
                var data = locations.OrderBy(u => u.pos);
                foreach (LocationData loc in data)
                {
                    latLngPoints[index++] = new LatLng(loc.location.Latitude, loc.location.Longitude);
                }
                var polylineMarker = new PolylineOptions().Visible(true).InvokeColor(Color.Red).InvokeWidth(10);

                foreach (var item in latLngPoints)
                {
                    polylineMarker.Add(item);
                }
                //  MarkOnMap("s", latLngPoints[0], Resource.Drawable.MarkerSource);
                //  MarkOnMap("d", latLngPoints[locations.Count-1], Resource.Drawable.MarkerDest);
                map.AddPolyline(polylineMarker);
                //  Toast.MakeText(this, "Added", ToastLength.Long).Show();
            }

        }

        private void Map_PolylineClick(object sender, GoogleMap.PolylineClickEventArgs e)
        {
            Toast.MakeText(this, String.Format("You clicked on Marker ID {0}", 102), ToastLength.Short).Show();
        }
        bool IsGooglePlayServicesInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("AutoCaptureLocation", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("AutoCaptureLocation", "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);

                // Show error dialog to let user debug google play services
            }
            return false;
        }
        bool apiclientloaded = false;
        protected async override void OnResume()
        {
            base.OnResume();
            Log.Debug("OnResume", "OnResume called, connecting to client...");

            apiClient.Connect();
            if (!apiclientloaded)
            {


            }


            btndone.Click += async delegate
            {
                //  SetupPolygon();
                Isstart = false;
                locations.Clear();
                if (apiClient.IsConnected)
                {
                    apiclientloaded = true;


                    // Setting location priority to PRIORITY_HIGH_ACCURACY (100)
                    locRequest.SetPriority(100);

                    // Setting interval between updates, in milliseconds
                    // NOTE: the default FastestInterval is 1 minute. If you want to receive location updates more than 
                    // once a minute, you _must_ also change the FastestInterval to be less than or equal to your Interval
                    locRequest.SetFastestInterval(500);
                    locRequest.SetInterval(1000);

                    Log.Debug("LocationRequest", "Request priority set to status code {0}, interval set to {1} ms",
                        locRequest.Priority.ToString(), locRequest.Interval.ToString());

                    // pass in a location request and LocationListener
                    await LocationServices.FusedLocationApi.RequestLocationUpdates(apiClient, locRequest, this);
                    // In OnLocationChanged (below), we will make calls to update the UI
                    // with the new location data
                }
                else
                {
                    Log.Info("LocationClient", "Please wait for Client to connect");
                }

            };
        }

        protected override async void OnPause()
        {
            base.OnPause();
            Log.Debug("OnPause", "OnPause called, stopping location updates");

            if (apiClient.IsConnected)
            {
                // stop location updates, passing in the LocationListener
                await LocationServices.FusedLocationApi.RemoveLocationUpdates(apiClient, this);

                apiClient.Disconnect();
            }
        }


        ////Interface methods

        public void OnConnected(Bundle bundle)
        {
            // This method is called when we connect to the LocationClient. We can start location updated directly form
            // here if desired, or we can do it in a lifecycle method, as shown above 

            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now connected to client");

        }

        public void OnDisconnected()
        {
            // This method is called when we disconnect from the LocationClient.

            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now disconnected from client");
        }

        public void OnConnectionFailed(ConnectionResult bundle)
        {
            // This method is used to handle connection issues with the Google Play Services Client (LocationClient). 
            // You can check if the connection has a resolution (bundle.HasResolution) and attempt to resolve it

            // You must implement this to implement the IGooglePlayServicesClientOnConnectionFailedListener Interface
            Log.Info("LocationClient", "Connection failed, attempting to reach google play services");
        }
        public static Android.Locations.Location _currentLocation;
        string _locationProvider;
        int pos = 0;
        bool Isstart = false;
        List<LocationData> locations = new List<LocationData>();
        bool iszoom = false;
        public void OnLocationChanged(Location location)
        {
            // This method returns changes in the user's location if they've been requested

            // You must implement this to implement the Android.Gms.Locations.ILocationListener Interface
            Log.Debug("LocationClient", "Location updated");
            _currentLocation = location;
            if (location != null)
            {
                //   setupDrawLine(location);
                Location l = null;
                l = location;
                LocationData ld = new LocationData();
                ld.location = l;
                pos = pos + 1;
                ld.pos = pos;

                locations.Add(ld);
                if (!Isstart)
                {
                    SetupLine();
                }

                ld = null;
                tvvalues.Text = "Lat:" + _currentLocation.Latitude + "--Long:" + _currentLocation.Longitude;
                if (!iszoom)
                {
                    iszoom = true;
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude, location.Longitude), 24);
                    map.MoveCamera(cameraUpdate);
                    map.AnimateCamera(CameraUpdateFactory.ZoomTo(24));
                }


                // _map.SetOnMapClickListener(new OnMapClickListener(this, _map));

            }
            //latitude2.Text = "Latitude: " + location.Latitude.ToString();
            //longitude2.Text = "Longitude: " + location.Longitude.ToString();
            //provider2.Text = "Provider: " + location.Provider.ToString();
        }
        public void setupDrawLine(Location location)
        {
            _currentLocation = location;

            Location l = null;
            l = location;
            // if (!locations.Contains(l))
            // {
            LocationData ld = new LocationData();
            ld.location = l;
            pos = pos + 1;
            ld.pos = pos;

            locations.Add(ld);
            ld = null;
            tvvalues.Text = "Lat:" + _currentLocation.Latitude + "--Long:" + _currentLocation.Longitude;
            Toast.MakeText(this, "New Value Added", ToastLength.Long).Show();
            l = null;

            SetupPolygon();
        }
        public void SetupPolygon()
        {
            if (locations.Count != 0)
            {
                var latLngPoints = new LatLng[locations.Count];
                int index = 0;
                var data = locations;
                foreach (LocationData loc in data)
                {
                    latLngPoints[index++] = new LatLng(loc.location.Latitude, loc.location.Longitude);
                }
                //var polylineMarker = new PolylineOptions().Visible(true).InvokeColor(Color.Red).InvokeWidth(10);
                PolygonOptions rectOptions = new PolygonOptions();
                rectOptions.InvokeFillColor(Android.Graphics.Color.ParseColor("#9689B81E"))
                   .InvokeStrokeColor(Android.Graphics.Color.ParseColor("#9689B81E"))
                   .InvokeStrokeWidth(5);
                foreach (var item in latLngPoints)
                {
                    rectOptions.Add(item);
                }
                //  MarkOnMap("s", latLngPoints[0], Resource.Drawable.MarkerSource);
                //  MarkOnMap("d", latLngPoints[locations.Count-1], Resource.Drawable.MarkerDest);
                Polygon polygon = map.AddPolygon(rectOptions);
                polygon.Clickable = true;
                map.SetOnPolygonClickListener(new OnPolygonClickListener(this, _mapFragment));
                // Toast.MakeText(this, "Added", ToastLength.Long).Show();
            }

        }
        public void OnConnectionSuspended(int i)
        {

        }
    }
    public class OnPolygonClickListener : Java.Lang.Object, Android.Gms.Maps.GoogleMap.IOnPolygonClickListener
    {
        private AutoCaptureLocation mapWithMarkersActivity;
        private MapFragment _mapFragment;

        public OnPolygonClickListener(AutoCaptureLocation mapWithMarkersActivity, MapFragment _mapFragment)
        {
            // TODO: Complete member initialization
            this.mapWithMarkersActivity = mapWithMarkersActivity;
            this._mapFragment = _mapFragment;
        }

        public async void OnPolygonClick(Polygon polygon)
        {
            // int strokeColor = circle.getStrokeColor() ^ 0x00ffffff;
            //circle.setStrokeColor(strokeColor);
            Toast.MakeText(mapWithMarkersActivity, "Next Page", ToastLength.Long).Show();
        }
    }
    public class LocationData
    {
        public Location location { get; set; }
        public int pos { get; set; }
    }

}
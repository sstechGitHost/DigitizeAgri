using System;
using Android.App;
using Android.OS;
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
using Android.Net;
using Android.Runtime;
using Android.Content;


namespace DigitizeAgri.Droid.Activities.Maps
{
    [Activity(Label = "AutoCaptureLocation")]
    public class AutoCaptureLocation : Activity, ILocationListener
    {
        LocationManager locMgr;
        public static Android.Locations.Location _currentLocation;
        List<LocationData> locations = new List<LocationData>();
        int pos = 0;
        bool Isstart = false;
        bool iszoom = false;
        TextView tvvalues = null;
        ConnectivityManager connectivityManager;
        private MapFragment _mapFragment;
        GoogleMap map;
        bool isLmode = false;
        public void OnLocationChanged(Location location)
        {
            if (isLmode)
            {
                bool isvalid = true;
                _currentLocation = location;

                if (location != null)
                {
                    //   setupDrawLine(location);
                    float ac = location.Accuracy;
                    if (ac > 20)
                    {
                        isvalid = false;
                        ShowMassage("Location", "You are in indoor place.Please go to OutDoor Place");
                        // Toast.MakeText(this, "Accuracy" + ac.ToString(), ToastLength.Long).Show();
                    }
                    NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
                    if (activeConnection != null && activeConnection.IsConnected)
                    {

                        if (activeConnection.TypeName == "WIFI")
                        {
                            isvalid = false;
                            tvvalues.Text = "Error:  You are Connected with WIFI. Please Use Mobile Data ";
                            ShowMassage("Network Status", "You are Connected with WIFI. Please Use Mobile Data");
                        }
                        else
                        {
                            isvalid = true;
                            tvvalues.Text = "Start";
                        }
                    }
                    else
                    {
                        isvalid = false;
                        tvvalues.Text = "Error:  Internet Not available ";
                        ShowMassage("Network Status", "Internet Not available ");

                    }
                    if (isvalid)
                    {
                        Location l = null;
                        l = location;
                        LocationData ld = new LocationData();
                        ld.location = l;
                        pos = pos + 1;
                        ld.pos = pos;

                        locations.Add(ld);
                        if (!Isstart)
                        {
                            tvvalues.Text = "Please Go";
                            SetupLine();
                            // tvvalues.Text = "please stop until move Status.";
                        }

                        ld = null;
                        //  tvvalues.Text = "Lat:" + _currentLocation.Latitude + "--Long:" + _currentLocation.Longitude;

                        if (!iszoom)
                        {
                            iszoom = true;
                            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude, location.Longitude), 24);
                            map.MoveCamera(cameraUpdate);
                            map.AnimateCamera(CameraUpdateFactory.ZoomTo(22));
                        }
                    }
                    // _map.SetOnMapClickListener(new OnMapClickListener(this, _map));

                }
            }
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }
        public void ShowMassage(string title, string msg)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(title);
            alert.SetMessage(msg);
            alert.SetCancelable(false);
            alert.SetNegativeButton("OK", delegate { base.OnBackPressed(); });
            alert.Show();

        }
        public void settingMassage()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Location Settings");
            alert.SetMessage("Please Go to inside 'Mode' Opation and Select 'Device Only' Opation");
            alert.SetCancelable(false);
            alert.SetNegativeButton("OK", delegate {
                StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
            });
            alert.Show();

        }
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);



            SetContentView(Resource.Layout.AutoCaptureLocation);

            _mapFragment = FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map);
            GoogleMapOptions mapOptions = new GoogleMapOptions()
                   .InvokeMapType(GoogleMap.MapTypeSatellite)
                   .InvokeZoomControlsEnabled(false)
                   .InvokeCompassEnabled(true);
            _mapFragment.Map.MapType = GoogleMap.MapTypeSatellite;
            map = _mapFragment.Map;

            locMgr = GetSystemService(Context.LocationService) as LocationManager;
            tvvalues = FindViewById<TextView>(Resource.Id.tvvalues);
            var btnStart = FindViewById<Button>(Resource.Id.btnStart);
            var btnDone = FindViewById<Button>(Resource.Id.btnDone);

            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            if (activeConnection != null && activeConnection.IsConnected)
            {

                if (activeConnection.TypeName == "WIFI")
                {
                    tvvalues.Text = "Error:  You are Connected with WIFI. Please Use Mobile Data ";
                    ShowMassage("Network Status", "You are Connected with WIFI. Please Use Mobile Data");
                }
                else
                {
                    tvvalues.Text = "Start";
                    //Settings.Secure


                }
            }
            else
            {
                tvvalues.Text = "Error:  Internet Not available ";
                ShowMassage("Network Status", "Internet Not available ");

            }

            btnStart.Click += (sender, e) =>
            {
                tvvalues.Text = "Please wait Finding Your Location...";
                map.Clear();
                locations.Clear();
            };

            btnDone.Click += (sender, e) =>
            {
                Isstart = true;
                map.Clear();
                List<LocationData> templocations = new List<LocationData>();
                templocations = locations;
                SetupPolygon(templocations);
                tvvalues.Text = "Stop";

            };
            // Create your application here
        }
        public void SetupPolygon(List<LocationData> templocations)
        {
            if (templocations.Count != 0)
            {
                var latLngPoints = new LatLng[templocations.Count];
                int index = 0;
                var data = templocations;
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
                map.SetOnPolygonClickListener(new OnPolygonClickListener1(this, _mapFragment));
                // Toast.MakeText(this, "Added", ToastLength.Long).Show();
            }

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
        protected override void OnResume()
        {
            base.OnResume();
            string Provider = LocationManager.GpsProvider;
            if (Provider != null)
            {
                if (locMgr.IsProviderEnabled(Provider))
                {
                    locMgr.RequestLocationUpdates(Provider, 2000, 1, this);
                    Location cul = locMgr.GetLastKnownLocation(Provider);
                    if (cul != null)
                    {
                        float ac = cul.Accuracy;
                        if (ac > 20)
                        {
                            ShowMassage("Location", "You are in indoor place.Please go to OutDoor Place");
                            // Toast.MakeText(this, "Accuracy" + ac.ToString(), ToastLength.Long).Show();
                        }
                    }

                    //    bool gpsEnabled = Android.Provider.Settings.Secure.IsLocationProviderEnabled(ContentResolver, Provider);

                    //    Android.Provider.Settings.Secure.PutInt(ContentResolver, Android.Provider.Settings.Secure.LocationMode, 3);

                    int intid = Android.Provider.Settings.Secure.GetInt(ContentResolver, Android.Provider.Settings.Secure.LocationMode);
                    if (intid != 1)
                    {
                        settingMassage();
                        // StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                    }
                    else
                        isLmode = true;
                }
                else
                {
                    ShowMassage("GPS Status", Provider + " is not available. Does the device have location services enabled?");
                    // StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                    // Log.Info(tag, Provider + " is not available. Does the device have location services enabled?");
                }
            }
            else
            {
                Toast.MakeText(this, "internet not available", ToastLength.Long).Show();
            }

        }
    }
    public class OnPolygonClickListener1 : Java.Lang.Object, Android.Gms.Maps.GoogleMap.IOnPolygonClickListener
    {
        private AutoCaptureLocation mapWithMarkersActivity;
        private MapFragment _mapFragment;

        public OnPolygonClickListener1(AutoCaptureLocation mapWithMarkersActivity, MapFragment _mapFragment)
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
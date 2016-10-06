using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DigitizeAgri.Droid.Activities.Maps
{
    [Activity(Label = "TapCaptureLocationActivity",MainLauncher=false)]
    public class TapCaptureLocationActivity : Activity, ILocationListener, IOnMapReadyCallback
    {
        private GoogleMap _map;
        private MapFragment _mapFragment;
        LocationManager _locMgr;
        string tag = "TapCaptureLocationActivity";
        static List<LatLng> _locations = new List<LatLng>();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TapCaptureLocation);

            InitMapFragment();

        }
        protected override void OnStart()
        {
            base.OnStart();
            Log.Debug(tag, "OnStart called");
        }
        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug(tag, "OnResume called");
            _locMgr = GetSystemService(Context.LocationService) as LocationManager;


            /////////////////////////////////////////////////////////////////////////////////////////////////////

            var locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Medium;

            string locationProvider = _locMgr.GetBestProvider(locationCriteria, true);

            Log.Debug(tag, "Starting location updates with " + locationProvider.ToString());
            Location location = _locMgr.GetLastKnownLocation(locationProvider);
            if (location != null)
            {
                OnLocationChanged(location);
            }
            _locMgr.RequestLocationUpdates(locationProvider, 0, 0, this);
        }
        protected override void OnPause()
        {
            base.OnPause();
            _locMgr.RemoveUpdates(this);
            Log.Debug(tag, "Location updates paused because application is entering the background");
        }

        protected override void OnStop()
        {
            base.OnStop();
            Log.Debug(tag, "OnStop called");
        }
        private void InitMapFragment()
        {
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeSatellite)
                    .InvokeZoomControlsEnabled(false)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }
            _locMgr = GetSystemService(Context.LocationService) as LocationManager;

            /////////////////////////////////////////////////////

            var locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Medium;

            string locationProvider = _locMgr.GetBestProvider(locationCriteria, true);

            Log.Debug(tag, "Starting location updates with " + locationProvider.ToString());
            Location location = _locMgr.GetLastKnownLocation(locationProvider);
            if (location != null)
            {
                OnLocationChanged(location);
            }
            _locMgr.RequestLocationUpdates(locationProvider, 0, 0, this);
            if (_map == null)
            {
                _map = _mapFragment.Map;
                if (_map != null)
                {

                }
            }
        }
        public void OnLocationChanged(Android.Locations.Location location)
        {
            Log.Debug(tag, "Location changed");
            double latitude = location.Latitude;
            double longitude = location.Longitude;
            LatLng _location = new LatLng(latitude, longitude);

            if (_map == null)
            {
                _map = _mapFragment.Map;
                if (_map != null)
                {
                    _map.MyLocationEnabled = true;
                    _map.UiSettings.ZoomControlsEnabled = true;
                    _map.UiSettings.CompassEnabled = true;

                    _plocations = new List<LatLng>();
                    _plocations.Add(new LatLng(17.4415741824508, 78.3762331679463));
                    _plocations.Add(new LatLng(17.4415434757762, 78.3765141293406));
                    _plocations.Add(new LatLng(17.4413880231566, 78.3765194937587));
                    _plocations.Add(new LatLng(17.4414177702731, 78.3762288093567));

                    Polygon();

                    // We create an instance of CameraUpdate, and move the map to it.
                    if (_plocations.Count > 0)
                    {
                        CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(_plocations[0], 18);
                        _map.MoveCamera(cameraUpdate);
                    }
                    else
                    {
                        CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(_location, 18);
                        _map.MoveCamera(cameraUpdate);
                    }

                    _map.AnimateCamera(CameraUpdateFactory.ZoomTo(18));
                    _map.SetOnMapClickListener(new OnMapClickListener(this, _map));
                    _map.SetOnMapClickListener(new OnMapClickListener(this, _map));

                }
            }
            Button button = FindViewById<Button>(Resource.Id.show);
            button.Click += delegate
            {
                _map.Clear();
                if (_locations.Count > 0)
                {
                    _locations.Clear();
                }
            };
            Button btnDraw = FindViewById<Button>(Resource.Id.draw);
            btnDraw.Click += delegate
            {
                _map.Clear();
                if (_locations.Count > 0)
                {
                    PolygonOptions rectOptions = new PolygonOptions();
                    foreach (var item in _locations)
                    {
                        rectOptions.Add(new LatLng(item.Latitude, item.Longitude));
                    }
                    rectOptions.InvokeFillColor(Android.Graphics.Color.ParseColor("#9689B81E"))
                    .InvokeStrokeColor(Android.Graphics.Color.ParseColor("#9689B81E"))
                    .InvokeStrokeWidth(1);
                    //_map.AddPolygon(rectOptions);
                    Polygon polygon = _map.AddPolygon(rectOptions);
                    polygon.Clickable = true;
                    _map.SetOnPolygonClickListener(new OnPolygonClickListener(this, _mapFragment));
                }
            };
        }
        private class OnPolygonClickListener : Java.Lang.Object, Android.Gms.Maps.GoogleMap.IOnPolygonClickListener
        {
            private TapCaptureLocationActivity tapCaptureLocationActivity;
            private MapFragment _mapFragment;

            public OnPolygonClickListener(TapCaptureLocationActivity tapCaptureLocationActivity, MapFragment _mapFragment)
            {
                // TODO: Complete member initialization
                this.tapCaptureLocationActivity = tapCaptureLocationActivity;
                this._mapFragment = _mapFragment;
            }

            public async void OnPolygonClick(Polygon polygon)
            {
                // int strokeColor = circle.getStrokeColor() ^ 0x00ffffff;
                //circle.setStrokeColor(strokeColor);
                _mapFragment.View.Clickable = false;
                SAL.Services sal = new SAL.Services();

                string iOTData = await sal.GetIOTData();
                var intent = new Intent(tapCaptureLocationActivity, typeof(iOTSensorActivity));
                intent.PutExtra("iOTData", iOTData);
                tapCaptureLocationActivity.StartActivity(intent);
            }
        }

        List<LatLng> _plocations;
        private void Polygon()
        {
            PolygonOptions rectOptions = new PolygonOptions();
            foreach (var item in _plocations)
            {
                rectOptions.Add(new LatLng(item.Latitude, item.Longitude));
            }
            rectOptions.InvokeFillColor(Android.Graphics.Color.ParseColor("#9689B81E"))
            .InvokeStrokeColor(Android.Graphics.Color.ParseColor("#9689B81E"))
            .InvokeStrokeWidth(1);
            //_map.AddPolygon(rectOptions);
            Polygon polygon = _map.AddPolygon(rectOptions);
        }

        private class OnMapClickListener : Java.Lang.Object, Android.Gms.Maps.GoogleMap.IOnMapClickListener
        {
            private TapCaptureLocationActivity tapCaptureLocationActivity;
            private GoogleMap _map;

            public OnMapClickListener(TapCaptureLocationActivity tapCaptureLocationActivity)
            {
                this.tapCaptureLocationActivity = tapCaptureLocationActivity;
            }

            public OnMapClickListener(TapCaptureLocationActivity tapCaptureLocationActivity, GoogleMap _map)
            {
                this.tapCaptureLocationActivity = tapCaptureLocationActivity;
                this._map = _map;
            }

            public void OnMapClick(LatLng point)
            {
                double latitude = point.Latitude;
                double longitude = point.Longitude;
                LatLng locations = new LatLng(latitude, longitude);

                _locations.Add(new LatLng(latitude, longitude));

                MarkerOptions markerOpt3 = new MarkerOptions();
                markerOpt3.SetPosition(locations);
                markerOpt3.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
                _map.AddMarker(markerOpt3);

            }
        }
        public void OnProviderDisabled(string provider)
        {
            Log.Debug(tag, provider + " disabled by user");
        }
        public void OnProviderEnabled(string provider)
        {
            Log.Debug(tag, provider + " enabled by user");
        }
        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Debug(tag, provider + " availability has changed to " + status.ToString());
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this._map = googleMap;
        }
    }
}
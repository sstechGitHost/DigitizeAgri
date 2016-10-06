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
using Android.Bluetooth;
using Android.Util;

namespace DigitizeAgri.Droid.Activities
{
    [Activity(Label = "@string/select_device",
                Theme = "@android:style/Theme.Dialog",
                ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation)]
    public class DeviceListActivity : Activity
    {
        // Debugging
        private const string TAG = "DeviceListActivity";
        private const bool Debug = true;

        // Return Intent extra
        public const string EXTRA_DEVICE_ADDRESS = "device_address";

        // Member fields
        private BluetoothAdapter btAdapter;
        private static ArrayAdapter<string> pairedDevicesArrayAdapter;
        private static ArrayAdapter<string> newDevicesArrayAdapter;
        private Receiver receiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Setup the window
            RequestWindowFeature(WindowFeatures.IndeterminateProgress);
            SetContentView(Resource.Layout.device_list);

            // Set result CANCELED incase the user backs out
            SetResult(Result.Canceled);

            // Initialize the button to perform device discovery			
            var scanButton = FindViewById<Button>(Resource.Id.button_scan);
            scanButton.Click += (sender, e) =>
            {
                DoDiscovery();
                (sender as View).Visibility = ViewStates.Gone;
            };

            // Initialize array adapters. One for already paired devices and
            // one for newly discovered devices
            pairedDevicesArrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.device_name);
            newDevicesArrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.device_name);

            // Find and set up the ListView for paired devices
            var pairedListView = FindViewById<ListView>(Resource.Id.paired_devices);
            pairedListView.Adapter = pairedDevicesArrayAdapter;
            pairedListView.ItemClick += DeviceListClick;

            // Find and set up the ListView for newly discovered devices
            var newDevicesListView = FindViewById<ListView>(Resource.Id.new_devices);
            newDevicesListView.Adapter = newDevicesArrayAdapter;
            newDevicesListView.ItemClick += DeviceListClick;

            // Register for broadcasts when a device is discovered
            receiver = new Receiver(this);
            var filter = new IntentFilter(BluetoothDevice.ActionFound);
            RegisterReceiver(receiver, filter);

            // Register for broadcasts when discovery has finished
            filter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);
            RegisterReceiver(receiver, filter);

            // Get the local Bluetooth adapter
            btAdapter = BluetoothAdapter.DefaultAdapter;

            // Get a set of currently paired devices
            var pairedDevices = btAdapter.BondedDevices;

            // If there are paired devices, add each one to the ArrayAdapter
            if (pairedDevices.Count > 0)
            {
                FindViewById<View>(Resource.Id.title_paired_devices).Visibility = ViewStates.Visible;
                foreach (var device in pairedDevices)
                {
                    pairedDevicesArrayAdapter.Add(device.Name + "\n" + device.Address);
                }
            }
            else
            {
                String noDevices = Resources.GetText(Resource.String.none_paired);
                pairedDevicesArrayAdapter.Add(noDevices);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Make sure we're not doing discovery anymore
            if (btAdapter != null)
            {
                btAdapter.CancelDiscovery();
            }

            // Unregister broadcast listeners
            UnregisterReceiver(receiver);
        }

        /// <summary>
        /// Start device discover with the BluetoothAdapter
        /// </summary>
        private void DoDiscovery()
        {
            if (Debug)
                Log.Debug(TAG, "doDiscovery()");

            // Indicate scanning in the title
            SetProgressBarIndeterminateVisibility(true);
            SetTitle(Resource.String.scanning);

            // Turn on sub-title for new devices
            FindViewById<View>(Resource.Id.title_new_devices).Visibility = ViewStates.Visible;

            // If we're already discovering, stop it
            if (btAdapter.IsDiscovering)
            {
                btAdapter.CancelDiscovery();
            }

            // Request discover from BluetoothAdapter
            btAdapter.StartDiscovery();
        }

        /// <summary>
        /// The on-click listener for all devices in the ListViews
        /// </summary>
        void DeviceListClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Cancel discovery because it's costly and we're about to connect
            btAdapter.CancelDiscovery();

            // Get the device MAC address, which is the last 17 chars in the View
            var info = (e.View as TextView).Text.ToString();
            var address = info.Substring(info.Length - 17);

            string[] lines = info.ToString().Split(new string[] { "\n" }, StringSplitOptions.None);
            var device_Name = lines[0];
            ////// Create the result Intent and include the MAC address
            ////Intent intent = new Intent();
            ////intent.PutExtra(EXTRA_DEVICE_ADDRESS, address);

            ////// Set result and finish this Activity
            //SetResult(Result.Ok, intent);
            ///Finish();


            //Intent intent = new Intent(this, typeof(DeviceControlActivity));
            Intent intent = new Intent(this, typeof(BluetoothChat));
            //intent.PutExtra(DeviceControlActivity.EXTRAS_DEVICE_NAME, device.Name);
            //intent.PutExtra(DeviceControlActivity.EXTRAS_DEVICE_ADDRESS, address);
            intent.PutExtra(BluetoothChat.DEVICE_NAME, device_Name);
            StartActivity(intent);

        }

        public class Receiver : BroadcastReceiver
        {
            Activity _chat;

            public Receiver(Activity chat)
            {
                _chat = chat;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                string action = intent.Action;

                // When discovery finds a device
                if (action == BluetoothDevice.ActionFound)
                {
                    // Get the BluetoothDevice object from the Intent
                    BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    // If it's already paired, skip it, because it's been listed already
                    if (device.BondState != Bond.Bonded)
                    {
                        newDevicesArrayAdapter.Add(device.Name + "\n" + device.Address);
                    }
                    // When discovery is finished, change the Activity title
                }
                else if (action == BluetoothAdapter.ActionDiscoveryFinished)
                {
                    _chat.SetProgressBarIndeterminateVisibility(false);
                    _chat.SetTitle(Resource.String.select_device);
                    if (newDevicesArrayAdapter.Count == 0)
                    {
                        var noDevices = _chat.Resources.GetText(Resource.String.none_found).ToString();
                        newDevicesArrayAdapter.Add(noDevices);
                    }
                }
            }
        }

    }
}
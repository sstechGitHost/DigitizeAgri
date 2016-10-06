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
using DigitizeAgri.Droid.Services;
using Android.Bluetooth;
using Android.Util;
using DigitizeAgri.Droid.Models;

namespace DigitizeAgri.Droid.Activities
{
    [Activity(Label = "DeviceControlActivity")]
    public class DeviceControlActivity : Activity
    {
        public readonly static String TAG = typeof(DeviceControlActivity).Name;

        public static readonly String EXTRAS_DEVICE_NAME = "DEVICE_NAME";
        public static readonly String EXTRAS_DEVICE_ADDRESS = "DEVICE_ADDRESS";

        TextView mConnectionState;
        TextView mDataField;
        String mDeviceName;
        public static String mDeviceAddress;
        ExpandableListView mGattServicesList;
        public static BluetoothLeService mBluetoothLeService;
        List<List<BluetoothGattCharacteristic>> mGattCharacteristics =
            new List<List<BluetoothGattCharacteristic>>();
        public static bool mConnected = false;
        BluetoothGattCharacteristic mNotifyCharacteristic;

        private readonly String LIST_NAME = "NAME";
        private readonly String LIST_UUID = "UUID";

        private ServiceManager mServiceManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mDeviceAddress = Intent.GetStringExtra(EXTRAS_DEVICE_ADDRESS);
            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();

            RegisterReceiver(mServiceManager, MakeGattUpdateIntentFilter());
            if (mBluetoothLeService != null)
            {
                bool result = mBluetoothLeService.Connect(mDeviceAddress);
                Log.Debug(TAG, "Connect request result=" + result);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(mServiceManager);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnbindService(mServiceManager);
            mBluetoothLeService = null;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.gatt_services, menu);
            if (mConnected)
            {
                menu.FindItem(Resource.Id.menu_connect).SetVisible(false);
                menu.FindItem(Resource.Id.menu_disconnect).SetVisible(true);
            }
            else
            {
                menu.FindItem(Resource.Id.menu_connect).SetVisible(true);
                menu.FindItem(Resource.Id.menu_disconnect).SetVisible(false);
            }
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_connect:
                    mBluetoothLeService.Connect(mDeviceAddress);
                    return true;

                case Resource.Id.menu_disconnect:
                    mBluetoothLeService.Disconnect();
                    return true;

                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void UpdateConnectionState(int resourceId)
        {
            RunOnUiThread(new Action(delegate
            {
                mConnectionState.SetText(resourceId);
            }));
        }

        public void ClearUI()
        {
            mGattServicesList.SetAdapter((SimpleExpandableListAdapter)null);
            mDataField.SetText(Resource.String.no_data);
        }

        public void DisplayData(String data)
        {
            if (data != null)
            {
                mDataField.Text = data;
            }
        }

        // Demonstrates how to iterate through the supported GATT Services/Characteristics.
        // In this sample, we populate the data structure that is bound to the ExpandableListView
        // on the UI.
        public void DisplayGattServices(IList<BluetoothGattService> gattServices)
        {
            if (gattServices == null)
                return;

            String uuid = null;
            String unknownServiceString = Resources.GetString(Resource.String.unknown_service);
            String unknownCharaString = Resources.GetString(Resource.String.unknown_characteristic);
            List<Dictionary<String, Object>> gattServiceData = new List<Dictionary<String, Object>>();
            List<List<Dictionary<String, String>>> gattCharacteristicData
                = new List<List<Dictionary<String, String>>>();
            mGattCharacteristics = new List<List<BluetoothGattCharacteristic>>();

            // Loops through available GATT Services.
            foreach (BluetoothGattService gattService in gattServices)
            {
                Dictionary<String, Object> currentServiceData = new Dictionary<String, Object>();
                uuid = gattService.Uuid.ToString();
                currentServiceData.Add(
                    LIST_NAME, SampleGattAttributes.Lookup(uuid, unknownServiceString));
                currentServiceData.Add(LIST_UUID, uuid);
                gattServiceData.Add(currentServiceData);

                List<Dictionary<String, String>> gattCharacteristicGroupData =
                    new List<Dictionary<String, String>>();
                IList<BluetoothGattCharacteristic> gattCharacteristics =
                    gattService.Characteristics;
                List<BluetoothGattCharacteristic> charas =
                    new List<BluetoothGattCharacteristic>();

                // Loops through available Characteristics.
                foreach (BluetoothGattCharacteristic gattCharacteristic in gattCharacteristics)
                {
                    charas.Add(gattCharacteristic);
                    Dictionary<String, String> currentCharaData = new Dictionary<String, String>();
                    uuid = gattCharacteristic.Uuid.ToString();
                    currentCharaData.Add(
                        LIST_NAME, SampleGattAttributes.Lookup(uuid, unknownCharaString));
                    currentCharaData.Add(LIST_UUID, uuid);
                    gattCharacteristicGroupData.Add(currentCharaData);
                }
                mGattCharacteristics.Add(charas);
                gattCharacteristicData.Add(gattCharacteristicGroupData);
            }

            SimpleExpandableListAdapter gattServiceAdapter = new SimpleExpandableListAdapter(
                this,
                (IList<IDictionary<String, Object>>)gattServiceData,
                Android.Resource.Layout.SimpleExpandableListItem2,
                new String[] { LIST_NAME, LIST_UUID },
                new int[] { Android.Resource.Id.Text1, Android.Resource.Id.Text2 },
                (IList<IList<IDictionary<String, Object>>>)gattCharacteristicData,
                Android.Resource.Layout.SimpleExpandableListItem2,
                new String[] { LIST_NAME, LIST_UUID },
                new int[] { Android.Resource.Id.Text1, Android.Resource.Id.Text2 }
            );

            mGattServicesList.Adapter = (IListAdapter)gattServiceAdapter;
        }

        private static IntentFilter MakeGattUpdateIntentFilter()
        {
            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAction(BluetoothLeService.ACTION_GATT_CONNECTED);
            intentFilter.AddAction(BluetoothLeService.ACTION_GATT_DISCONNECTED);
            intentFilter.AddAction(BluetoothLeService.ACTION_GATT_SERVICES_DISCOVERED);
            intentFilter.AddAction(BluetoothLeService.ACTION_DATA_AVAILABLE);
            return intentFilter;
        }

    }

    class ServiceManager : BroadcastReceiver, IServiceConnection
    {
        DeviceControlActivity DCActivity;

        public ServiceManager(DeviceControlActivity dca)
        {
            DCActivity = dca;
        }

        public void OnServiceConnected(ComponentName componentName, IBinder service)
        {
            DeviceControlActivity.mBluetoothLeService = ((BluetoothLeService.LocalBinder)service).GetService();
            if (!DeviceControlActivity.mBluetoothLeService.Initialize())
            {
                Log.Error(DeviceControlActivity.TAG, "Unable to initialize Bluetooth");
                DCActivity.Finish();
            }
            // Automatically connects to the device upon successful start-up initialization.
            DeviceControlActivity.mBluetoothLeService.Connect(DeviceControlActivity.mDeviceAddress);
        }

        public void OnServiceDisconnected(ComponentName componentName)
        {
            DeviceControlActivity.mBluetoothLeService = null;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;

            if (BluetoothLeService.ACTION_GATT_CONNECTED == action)
            {
                DeviceControlActivity.mConnected = true;
                DCActivity.UpdateConnectionState(Resource.String.connected);
                DCActivity.InvalidateOptionsMenu();

            }
            else if (BluetoothLeService.ACTION_GATT_DISCONNECTED == action)
            {
                DeviceControlActivity.mConnected = false;
                DCActivity.UpdateConnectionState(Resource.String.disconnected);
                DCActivity.InvalidateOptionsMenu();
                DCActivity.ClearUI();

            }
            else if (BluetoothLeService.ACTION_GATT_SERVICES_DISCOVERED == action)
            {
                // Show all the supported services and characteristics on the user interface.
                DCActivity.DisplayGattServices(DeviceControlActivity.mBluetoothLeService.GetSupportedGattServices());

            }
            else if (BluetoothLeService.ACTION_DATA_AVAILABLE == action)
            {
                DCActivity.DisplayData(intent.GetStringExtra(BluetoothLeService.EXTRA_DATA));
            }
        }
    }

}
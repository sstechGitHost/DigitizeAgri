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

namespace DigitizeAgri.Droid.Models
{
    public class SampleGattAttributes
    {
        public static String HEART_RATE_MEASUREMENT = "00002a37-0000-1000-8000-00805f9b34fb";
        public static String CLIENT_CHARACTERISTIC_CONFIG = "00002902-0000-1000-8000-00805f9b34fb";

        private static Dictionary<String, String> Attributes = new Dictionary<String, String>()
		{
			// Sample Services.
			{"0000180d-0000-1000-8000-00805f9b34fb", "Heart Rate Service"},
			{"0000180a-0000-1000-8000-00805f9b34fb", "Device Information Service"},

			// Sample Characteristics.
			{HEART_RATE_MEASUREMENT, "Heart Rate Measurement"},
			{"00002a29-0000-1000-8000-00805f9b34fb", "Manufacturer Name String"},
		};

        public static String Lookup(String key, String defaultName)
        {
            String name = Attributes[key];
            return name == null ? defaultName : name;
        }
    }
}
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

namespace DigitizeAgri.Droid.Models.Maps
{
    static class Constants
    {
        internal static string strGoogleServerKey = "AIzaSyBK9hN_VmKJrBrENK6uRN*********";
        internal static string strGoogleServerDirKey = "A*********xe1Tc8-_t6Dq6CocGdb9nN-bc08CE";
        internal static string strGoogleDirectionUrl = "https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&key=" + strGoogleServerDirKey + "";
        internal static string strGeoCodingUrl = "https://maps.googleapis.com/maps/api/geocode/json?{0}&key=" + strGoogleServerKey + "";
        internal static string strSourceLocation = "Vijayanagar,Bangalore,India";
        internal static string strDestinationLocation = "Jayanagar,Bangalore,India";

        internal static string strException = "Exception";
        internal static string strTextSource = "Source";
        internal static string strTextDestination = "Destination";

        internal static string strNoInternet = "No online connection. Please review your internet connection";
        internal static string strPleaseWait = "Please wait...";
        internal static string strUnableToConnect = "Unable to connect server!,Please try after sometime";
    }
}
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
    internal class ActivitiesMetaData
    {
        public ActivitiesMetaData(int titleResourceId, int descriptionId, Type activityToLaunch)
        {
            ActivityToLaunch = activityToLaunch;
            TitleResource = titleResourceId;
            DescriptionResource = descriptionId;
        }

        public Type ActivityToLaunch { get; private set; }
        public int DescriptionResource { get; private set; }
        public int TitleResource { get; private set; }

        public void Start(Activity context)
        {
            Intent i = new Intent(context, ActivityToLaunch);
            context.StartActivity(i);
        }
    }
}
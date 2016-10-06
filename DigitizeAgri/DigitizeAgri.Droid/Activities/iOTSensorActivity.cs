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
using BarChart;
using Android.Graphics;
using DigitizeAgri.Models;
using Newtonsoft.Json;

namespace DigitizeAgri.Droid.Activities
{
    [Activity(Label = "IOT-Agri Sensor")]
    public class iOTSensorActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);

          //  SetContentView(Resource.Layout.IOTSensorLayOut);
          //  LinearLayout chartLinearLayOut = FindViewById<LinearLayout>(Resource.Id.chartLayOut);

            iOTModel iotData = JsonConvert.DeserializeObject<iOTModel>(Intent.GetStringExtra("iOTData"));
            

            BarChart.BarModel[] obj = new BarModel[3];

            obj[0] = new BarModel { Value = iotData.sensor_datas[0].temp, Color = Color.Yellow, Legend = "Light",  ValueCaptionHidden = false, ValueCaption = CalcPercentage(iotData.sensor_datas[0].temp)};
            obj[1] = new BarModel { Value = iotData.sensor_datas[0].humidity, Color = Color.Orange, Legend = "Humidity", ValueCaptionHidden = false, ValueCaption = CalcPercentage(iotData.sensor_datas[0].humidity) };
            obj[2] = new BarModel { Value = iotData.sensor_datas[0].moisture, Color = Color.Aqua, Legend = "Moisture", ValueCaptionHidden = false, ValueCaption = CalcPercentage(iotData.sensor_datas[0].moisture) };
            

            var chart = new BarChartView(this)
            {
                ItemsSource = obj
            };

            chart.AutoLevelsEnabled = false;
            //chart.AddLevelIndicator(0, title: "zero");
            //chart.AddLevelIndicator(5);
            chart.BarOffset = 100;
            chart.BarWidth = 125f;

           // chartLinearLayOut.AddView(chart);
            AddContentView(chart, new ViewGroup.LayoutParams(
              ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
        }

        string CalcPercentage(float itm)
        {
            return (itm / 100).ToString("0.00%");
        }
    }
}
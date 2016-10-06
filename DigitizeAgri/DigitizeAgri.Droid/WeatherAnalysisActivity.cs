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
using DigitizeAgri.Models;
using DigitizeAgri.SAL;
using System.Globalization;

namespace DigitizeAgri.Droid
{
    [Activity(Label = "Weather Analysis")]
    public class WeatherAnalysisActivity : Activity, AdapterView.IOnItemSelectedListener
    {
        ListView listView;
        Gallery gallery;
        TextView txtCurrentTime, txtCurrentTemp;
        SAL.Services service = new SAL.Services();
        List<DateTime> ListOfDates;
        string latitude = string.Empty;
        string longitude = string.Empty;
        string date;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // ActionBar.Hide();
            // Create your application here
            SetContentView(Resource.Layout.weatheranalysis);

            gallery = FindViewById<Gallery>(Resource.Id.gallery);
            listView = FindViewById<ListView>(Resource.Id.WList);
            gallery.SetCallbackDuringFling(false);
            gallery.OnItemSelectedListener = this;
            txtCurrentTemp = FindViewById<TextView>(Resource.Id.temp);
            txtCurrentTime = FindViewById<TextView>(Resource.Id.time);
            txtCurrentTime.Text = DateTime.Now.ToString("hh:mm tt", CultureInfo.CurrentCulture);
            DateTime StartDate = DateTime.Now.Date;
            DateTime EndDate = DateTime.Now.Date.AddDays(4);
            List<double> daysToAdd = Enumerable.Range(0,
                                                            (EndDate - StartDate).Days + 1)
                                                        .ToList().ConvertAll(d => (double)d);
            ListOfDates = daysToAdd.Select(StartDate.AddDays).ToList();
            gallery.Adapter = new WeatherAnalysisAdapter(this, ListOfDates);
            latitude = "17.3850440";
            longitude = "78.4866710";
            GetCurrentWeather(latitude, longitude);
            date = DateTime.Now.Date.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture);
            GetForecastWeather(latitude, longitude);
        }

        private async void GetCurrentWeather(string latitude, string longitude)
        {
            var cwresult = await service.CurrentWeather(latitude, longitude);

            if (cwresult != null && cwresult.cod == 200)
            {
                txtCurrentTemp.Text = cwresult.main.temp.ToString() + "°";
            }
        }
        private async void GetForecastWeather(string latitude, string longitude)
        {
            var fcw = await service.ForecastWeather(latitude, longitude);
            if (fcw != null && fcw.cod == 200)
            {
                List<item> todayforcastitems = fcw.list.Where(u => Convert.ToDateTime(u.dt_txt).Date == Convert.ToDateTime(date).Date).ToList();
                listView.Adapter = new WeatherAdapter(this, todayforcastitems);
            }
        }
        public async void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            date = ListOfDates[position].ToString("yyyy-MM-dd", CultureInfo.CurrentCulture);
            //var index = (int)parent.Tag;
            var fcw = await service.ForecastWeather(latitude, longitude);
            if (fcw != null && fcw.cod == 200)
            {
                List<item> todayforcastitems = fcw.list.Where(u => Convert.ToDateTime(u.dt_txt).Date == Convert.ToDateTime(date).Date).ToList();
                listView.Adapter = new WeatherAdapter(this, todayforcastitems);
            }
            //Android.Widget.Toast.MakeText(this, status.ToString(), Android.Widget.ToastLength.Short).Show();
        }
        public void OnNothingSelected(AdapterView parent)
        {
            throw new NotImplementedException();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.activity_main_actions, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.hmenu:
                    View menuItemView = FindViewById(Resource.Id.hmenu);
                    PopupMenu menu = new PopupMenu(this, menuItemView);
                    menu.Inflate(Resource.Menu.popup_menu);
                    menu.MenuItemClick += (s1, arg1) =>
                    {
                        //Console.WriteLine("{0} selected", arg1.Item.TitleFormatted);
                        var selected = arg1.Item.TitleFormatted.ToString();
                        if (selected == "Hyderabad")
                        {
                            latitude = "17.3850440";
                            longitude = "78.4866710";
                        }
                        else if (selected == "Warangal")
                        {
                            latitude = "17.9689008";
                            longitude = "79.5940544";
                        }
                        else
                        {
                            latitude = "16.5061743";
                            longitude = "80.6480153";
                        }
                        GetCurrentWeather(latitude, longitude);
                        GetForecastWeather(latitude, longitude);
                    };

                    // Android 4 now has the DismissEvent
                    menu.DismissEvent += (s2, arg2) =>
                    {
                        Console.WriteLine("menu dismissed");
                    };
                    menu.Show();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }

        }
    }
}
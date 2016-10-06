using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Text.Method;
using DigitizeAgri.SAL;
using DigitizeAgri.Models;
using Android.Bluetooth;
using System.Collections.Generic;
using Android.Text.Style;
using Android.Text;
using DigitizeAgri.Droid.Activities;



namespace DigitizeAgri.Droid
{
	[Activity (Label = "Digitize Agri",  Icon = "@drawable/icon",MainLauncher=false)]
	public class MainActivity : Activity
	{
        TextView txtForgot;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.LoginReg);

			// Get our button from the layout resource,
			// and attach an event to it
            
            EditText phone = FindViewById<EditText>(Resource.Id.phone);
            EditText pwd = FindViewById<EditText>(Resource.Id.password);
            Button signInbutton = FindViewById<Button> (Resource.Id.signIn);
            Button createAccbutton = FindViewById<Button>(Resource.Id.createAccount);
            Button fbButton = FindViewById<Button>(Resource.Id.fbLogin);

#if DEBUG
            phone.Text = "955";
            pwd.Text = "test";
#endif
            
            fbButton.Click += fbButton_Click;
            createAccbutton.Click += createAccbutton_Click;

            txtForgot = FindViewById<TextView>(Resource.Id.forgotPwd);
            CustomTextView(txtForgot);
            //txtForgot.Click += txtForgot_Click;


            signInbutton.Click += async (sender, e) =>{
                if ((phone.Text.Length > 0) && (pwd.Text.Length > 0))
                {
                    SAL.Services sal = new SAL.Services();

                    string authenticateUsr = await sal.LoginBtnClicked(phone.Text.Trim(),pwd.Text.Trim());
                    if (authenticateUsr == "success")
                    {
                        var intent = new Intent(this, typeof(DashboardActivity));
                        StartActivity(intent);
                    }
                    else
                    {
                        ShowAlert("Authentication Failed", "User Name and or Password is Incorect.");
                    }
                }
                else
                {
                    Toast.MakeText(this, "Please enter valid login credentials.", ToastLength.Long).Show();
                    //ShowAlert("Authentication Failed", "Please enter valid login credentials.");
                }
            };

            //GradientDrawable signInGd = new GradientDrawable();
            //signInGd.SetColor(Color.Orange);
            //signInGd.SetCornerRadius(12);
            //signInbutton.SetBackgroundDrawable(signInGd);

            //GradientDrawable createAccGd = new GradientDrawable();
            //createAccGd.SetColor(Color.Green);
            //createAccGd.SetCornerRadius(12);
            //createAccbutton.SetBackgroundDrawable(createAccGd);


        }

        void fbButton_Click(object sender, EventArgs e)
        {
            //BlueTooth();
            //var intent = new Intent(this, typeof(BluetoothChat));
            
            //var intent = new Intent(this, typeof(DeviceScanActivity));
            var intent = new Intent(this, typeof(DeviceListActivity));
            StartActivity(intent);
        }

        void txtForgot_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Forgot password clicked!!");
        }

        public void ShowAlert(string title, string message)
        {
            Android.App.AlertDialog.Builder alrt = new Android.App.AlertDialog.Builder(this);
            alrt.SetTitle(title);
            alrt.SetCancelable(false);
            //alrt.SetIcon(Resource.Drawable.alertimage);
            alrt.SetPositiveButton("OK", delegate { alrt.Dispose(); });
            alrt.SetMessage(message);
            alrt.Show();
        }
        void createAccbutton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(UserRegistrationActvity));

            StartActivity(intent);

            //var alert = new AlertDialog.Builder(this);
            //alert.SetView(LayoutInflater.Inflate(Resource.Layout.UserRegistration1, null));
            //alert.Create().Show();
        }


        private void CustomTextView(TextView txtForgot)
        {
            var ss = new SpannableString("Forgot Password?");
            var clickableSpan = new MyClickableSpan();
            //clickableSpan.Click += v => StartActivity(new Intent(this, typeof(NextActivity)));

            clickableSpan.Click += delegate
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Test");
                builder.SetMessage("Click a button");

                builder.SetNegativeButton("Ok", (sender, e) =>
                {
                    //Toast.MakeText(this, "You clicked negative button", ToastLength.Short).Show();
                    builder.Dispose();
                });
                var dialog = builder.Create();
                dialog.Show();
            };

            ss.SetSpan(clickableSpan, 0, 16, SpanTypes.ExclusiveExclusive);

            txtForgot.TextFormatted = ss;
            txtForgot.MovementMethod = new LinkMovementMethod();
        }
        private class MyClickableSpan : ClickableSpan
        {
            public Action<View> Click;

            public override void OnClick(View widget)
            {
                if (Click != null)
                    Click(widget);
            }
        }

        void BlueTooth()
        {

            //Step 1: Setup BT Adapter
            BluetoothAdapter mBlueToothAdapter = BluetoothAdapter.DefaultAdapter;
            if (mBlueToothAdapter == null)
                Console.WriteLine("Blue Tooth Adapter is missing");
            else
                Console.WriteLine("Bluetooth: " + mBlueToothAdapter.Address);

            //Step 2: Is BT Enabled - if not then enable
            if (!mBlueToothAdapter.IsEnabled)
            {
                int REQUEST_ENABLE_BT = 1;
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
            }
            else
            {
                Console.WriteLine("Bluetooth is enabled! ");
            }

            //Step 3: Query bonded pairs to find if the device is already available
            ICollection<BluetoothDevice> pairedDevices = mBlueToothAdapter.BondedDevices;
            List<string> mArrayAdapter = new List<string>();
            
             //If there are paired devices
            if (pairedDevices.Count > 0) {
                foreach (BluetoothDevice device in pairedDevices)
                {
                    mArrayAdapter.Add("NAME: "+device.Name + " ADD: " + device.Address);
                    if (device.Name == "SSTECHLAPTOP135")
                    {

                      BluetoothSocket _socket =   device.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                      _socket.ConnectAsync();
                        
                    }

                }
            }
            else
            {
                Console.WriteLine("0 paired devices! ");
            }

            foreach (string item in mArrayAdapter)
            {
                Console.WriteLine("Paired Device: "+ item);
            }
                               
            //mBlueToothAdapter.StartDiscovery;

        }

       

	}
}



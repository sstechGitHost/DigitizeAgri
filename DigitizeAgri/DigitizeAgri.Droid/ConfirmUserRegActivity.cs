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
using Newtonsoft.Json;
using Android.Text;
using Android.Text.Style;
using Android.Text.Method;

namespace DigitizeAgri.Droid
{
    [Activity(Label = "ConfirmUserRegActivity")]
    public class ConfirmUserRegActivity : Activity
    {
        Person personObj;
        TextView tnc;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

           this.personObj = JsonConvert.DeserializeObject<Person>(Intent.GetStringExtra("Person"));

            SetContentView(Resource.Layout.UserRegistration2);
            Button confRegButton = FindViewById<Button>(Resource.Id.confReg);
            //userLoginID,phone,password,confirmpwd
            //firstName,lastName,income,email
            EditText fname = FindViewById<EditText>(Resource.Id.firstName);
            EditText lname = FindViewById<EditText>(Resource.Id.lastName);
            EditText email = FindViewById<EditText>(Resource.Id.email);
            EditText income = FindViewById<EditText>(Resource.Id.income);
            //confRegButton.Click += confRegButton_Click;
            tnc = FindViewById<TextView>(Resource.Id.tnc);
            CustomTextView(tnc);

            confRegButton.Click += async (sender, e) =>
            {
                this.personObj.fname = fname.Text;
                this.personObj.lname = lname.Text;
                this.personObj.income = income.Text;
                this.personObj.email = email.Text;
                //this.personObj.id = 9;
                List<Person> persons = new List<Person>();
                persons.Add(this.personObj);

                RootObject rootObj = new RootObject();
                rootObj.persons = persons;

                SAL.Services sal = new SAL.Services();

                string newUsr = await sal.CreateAccountClicked(rootObj);
                if (newUsr == "success")
                {
                    var intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                }
                else
                {
                    ShowAlert("New User Creation Failed", "User Name and or Password is Incorect.");
                }
            };
        }

        void confRegButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        public void ShowAlert(string title, string message)
        {
            AlertDialog.Builder alrt = new AlertDialog.Builder(this);
            alrt.SetTitle(title);
            alrt.SetCancelable(false);
            //alrt.SetIcon(Resource.Drawable.alertimage);
            alrt.SetPositiveButton("OK", delegate { alrt.Dispose(); });
            alrt.SetMessage(message);
            alrt.Show();
        }

        private void CustomTextView(TextView tnc)
        {
            SpannableStringBuilder spanTxt = new SpannableStringBuilder();
            var tstitle = "Term of services";
            var tsmessage = "All right, title, and interest in and to the Site (excluding Content provided by you) are and will remain the exclusive property of Xamarin and its licensors. The Site is protected by copyright, trademark, and other laws of the United States and foreign countries. Nothing in this Agreement gives you a right to use the Xamarin name or any Xamarin trademarks, logos, domain names, or other distinctive brand features, or those of any Xamarin licensor. Any feedback, comments, or suggestions you may provide regarding Xamarin or the Site are entirely voluntary, and we will be free to use such feedback, comments or suggestions as we see fit and without any obligation to you.";
            spanTxt.Append(tstitle);
            var tsclickableSpan = new MyClickableSpan();
            tsclickableSpan.Click += delegate
            {
                ShowDialog(tstitle, tsmessage);
            };
            spanTxt.SetSpan(tsclickableSpan, 0, 16, SpanTypes.ExclusiveExclusive);
            spanTxt.Append(" and");
            var pptitle = " Privacy Policy";
            var ppmessage = " All right, title, and interest in and to the Site (excluding Content provided by you) are and will remain the exclusive property of Xamarin and its licensors. The Site is protected by copyright, trademark, and other laws of the United States and foreign countries. Nothing in this Agreement gives you a right to use the Xamarin name or any Xamarin trademarks, logos, domain names, or other distinctive brand features, or those of any Xamarin licensor. Any feedback, comments, or suggestions you may provide regarding Xamarin or the Site are entirely voluntary, and we will be free to use such feedback, comments or suggestions as we see fit and without any obligation to you.";
            var ppclickableSpan = new MyClickableSpan();
            spanTxt.Append(pptitle);
            spanTxt.SetSpan(ppclickableSpan, 21, 35, SpanTypes.ExclusiveExclusive);
            ppclickableSpan.Click += delegate
            {
                ShowDialog(pptitle, ppmessage);
            };
            tnc.TextFormatted = spanTxt;
            tnc.MovementMethod = new LinkMovementMethod();
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
        public void ShowDialog(string title, string message)
        {
            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
            View myScrollView = inflater.Inflate(Resource.Layout.cusotm_dialog_layout, null, false);
            TextView txtview = (TextView)myScrollView.FindViewById(Resource.Id.dialogtext);
            txtview.Text = message;
            var dialog = new AlertDialog.Builder(this).SetView(myScrollView);
            dialog.SetTitle(title);
            dialog.SetCancelable(true);
            dialog.SetPositiveButton("Close", delegate
            {
                dialog.Dispose();
            });

            dialog.Show();
        }


    }
}
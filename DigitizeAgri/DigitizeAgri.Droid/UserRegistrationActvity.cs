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

namespace DigitizeAgri.Droid
{
    [Activity(Label = "UserRegistrationActvity")]
    public class UserRegistrationActvity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.UserRegistration1);
            Button prdToRegbutton = FindViewById<Button>(Resource.Id.prdToReg);
            prdToRegbutton.Click += prdToRegbutton_Click;

        }

        void prdToRegbutton_Click(object sender, EventArgs e)
        {
           

            if (Validate())
            {
                Person person = new Person();
                person.login = FindViewById<EditText>(Resource.Id.userLoginID).Text;
                person.phone = FindViewById<EditText>(Resource.Id.phone).Text;
                person.password = FindViewById<EditText>(Resource.Id.password).Text;
                
                
                var intent = new Intent(this, typeof(ConfirmUserRegActivity));
                intent.PutExtra("Person",JsonConvert.SerializeObject(person));
                
                StartActivity(intent);
            }
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

        bool Validate()
        {
            EditText loginID = FindViewById<EditText>(Resource.Id.userLoginID);
            EditText phone = FindViewById<EditText>(Resource.Id.phone);
            EditText password = FindViewById<EditText>(Resource.Id.password);
            EditText confirmpwd = FindViewById<EditText>(Resource.Id.confirmpwd);

            bool retVal = false;
            if (!string.IsNullOrEmpty(loginID.Text.Trim()))
            {
                retVal = true;
            }
            else
            {
                ShowAlert("Invalid Data!", "Login ID is required.");
                return false;
            }



            if (!string.IsNullOrEmpty(phone.Text.Trim()))
            {
                retVal = true;
            }
            else
            {
                ShowAlert("Invalid Data!", "Phone Number is required.");
                return false;
            }

            if (!string.IsNullOrEmpty(password.Text.Trim()))
            {
                retVal = true;
            }
            else
            {
                ShowAlert("Invalid Data!", "Password is required.");
                return false;
            }

            if (!string.IsNullOrEmpty(confirmpwd.Text.Trim()))
            {
                retVal = true;
            }
            else
            {
                ShowAlert("Invalid Data!", "Confirm Password is required.");
                return false;
            }

            if (!string.IsNullOrEmpty(password.Text.Trim()) && !string.IsNullOrEmpty(confirmpwd.Text.Trim()))
            {
                if (password.Text.Trim() == confirmpwd.Text.Trim())
                {
                    retVal = true;
                }
                else
                {
                    ShowAlert("Invalid Data!", "The passwords are not matching.");
                    return false;
                }
            }
            return retVal;
                
        }
    }
}
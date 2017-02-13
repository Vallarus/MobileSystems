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

namespace MobileSystems
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            var addActivitiesButton = FindViewById<Button>(Resource.Id.AddContacts);
            addActivitiesButton.Click += delegate { StartActivity(typeof(AddContactActivity)); };
            var gpsButton = FindViewById<Button>(Resource.Id.GPS);
            gpsButton.Click += delegate { StartActivity(typeof(GPSAndRestActivity)); };
            var accButton = FindViewById<Button>(Resource.Id.Acc);
            accButton.Click += delegate { StartActivity(typeof(AcceloremeterActivity)); };


        }
    }
}
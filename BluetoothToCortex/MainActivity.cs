using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;

namespace BluetoothToCortex
{
    [Activity(Label = "BluetoothToCortex", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // Local Bluetooth adapter
        private BluetoothAdapter bluetoothAdapter = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button mBtBtn = FindViewById<Button>(Resource.Id.FindBtButton);

            //button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
            mBtBtn.Click += delegate
            {

            };

            // Get local Bluetooth adapter
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            // If the adapter is null, then Bluetooth is not supported
            if (bluetoothAdapter == null)
            {
                Toast.MakeText(this, "Bluetooth is not available", ToastLength.Long).Show();
                //Finish();
                return;
            }

        }
    }
}


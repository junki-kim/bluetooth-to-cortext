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
        private BluetoothAdapter mBluetoothAdapter = null;
        private Button mBtBtn = null;
        private ListView mDeviceListView = null;

        protected override void OnCreate(Bundle bundle)
        {
            
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get local Bluetooth adapter
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            // Get our button from the layout resource,
            // and attach an event to it
            mBtBtn = FindViewById<Button>(Resource.Id.FindBtButton);
            mDeviceListView = FindViewById<ListView>(Resource.Id.deviceListView);

            //button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
            mBtBtn.Click += delegate
            {
                // If the adapter is null, then Bluetooth is not supported
                if (mBluetoothAdapter == null)
                {
                    Toast.MakeText(this, "Bluetooth is not available", ToastLength.Long).Show();
                    return;
                }

                if (!mBluetoothAdapter.IsEnabled)
                {
                    //Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                    //StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
                    Toast.MakeText(this, "Bluetooth adapter is not enabled.", ToastLength.Long).Show();
                    return;
                }
            };
        }
    }
}


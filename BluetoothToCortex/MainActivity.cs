using System;
using System.IO;
using Java.Util;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using Android.Util;
using Java.Lang;

namespace BluetoothToCortex
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        //TAG
        public static string TAG = "BT_SENDER";

        // Local Bluetooth adapter
        private BluetoothAdapter mBluetoothAdapter = null;
        private static ArrayAdapter<string> pairedDevicesArrayAdapter;
        private static ArrayAdapter<string> newDevicesArrayAdapter;
        private BTReceiver receiver;

        // views
        private Button mBtBtn = null;
        private ListView mDeviceListView = null;
        private ListView mPairedListView = null;

        // Return Intent extra
        public const string EXTRA_DEVICE_ADDRESS = "device_address";

        //socket
        private BluetoothSocket mSocket;
        private static UUID applicationUUID = UUID.FromString("fa87c0d0-afac-11de-8a39-0800200c9a66");
        private Stream mOutputStream;
        private Stream mInputStream;

        protected override void OnCreate(Bundle bundle)
        {
            
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get local Bluetooth adapter
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            // Initialize array adapters. One for already paired devices and
            // one for newly discovered devices
            pairedDevicesArrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.device_name);
            newDevicesArrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.device_name);

            // Get our button from the layout resource,
            // and attach an event to it
            mBtBtn = FindViewById<Button>(Resource.Id.FindBtButton);

            // Find and set up the ListView for paired devices
            mPairedListView = FindViewById<ListView>(Resource.Id.pairedDeviceListView);
            mPairedListView.Adapter = pairedDevicesArrayAdapter;
            mPairedListView.ItemClick += PairedListClick;

            // Find and setup list view
            mDeviceListView = FindViewById<ListView>(Resource.Id.deviceListView);
            mDeviceListView.Adapter = newDevicesArrayAdapter;
            mDeviceListView.ItemClick += DeviceListClick;

            
            // Register for broadcasts when a device is discovered
            receiver = new BTReceiver(this);
            var filter = new IntentFilter(BluetoothDevice.ActionFound);
            filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
            RegisterReceiver(receiver, filter);
            

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
                newDevicesArrayAdapter.Clear();
                DoDiscovery();
            };

            // Get a set of currently paired devices
            var pairedDevices = mBluetoothAdapter.BondedDevices;

            // If there are paired devices, add each one to the ArrayAdapter
            if (pairedDevices.Count > 0)
            {
                foreach (var device in pairedDevices)
                {
                    pairedDevicesArrayAdapter.Add(device.Name + "\n" + device.Address);
                }
            }
            else
            {
                string noDevices = Resources.GetText(Resource.String.none_paired);
                pairedDevicesArrayAdapter.Add(noDevices);
            }

        }//oncreate

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Make sure we're not doing discovery anymore
            if (mBluetoothAdapter != null)
            {
                mBluetoothAdapter.CancelDiscovery();
            }

            // Unregister broadcast listeners
            UnregisterReceiver(receiver);
        }

        /// <summary>
		/// Start device discover with the BluetoothAdapter
		/// </summary>
		private void DoDiscovery()
        {
            // Indicate scanning in the title
            SetProgressBarIndeterminateVisibility(true);
            SetTitle(Resource.String.scanning);

            // Turn on sub-title for new devices
            //FindViewById<View>(Resource.Id.title_new_devices).Visibility = ViewStates.Visible;

            // If we're already discovering, stop it
            if (mBluetoothAdapter.IsDiscovering)
            {
                mBluetoothAdapter.CancelDiscovery();
            }

            // Request discover from BluetoothAdapter
            mBluetoothAdapter.StartDiscovery();
        }

        /*
         * On click listener for dectected device list
         */
        void DeviceListClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Cancel discovery because it's costly and we're about to connect
            mBluetoothAdapter.CancelDiscovery();
        
            // Get the device MAC address, which is the last 17 chars in the View
            var info = (e.View as TextView).Text.ToString();
            var address = info.Substring(info.Length - 17);
            var name = info.Substring(0, info.Length - 17);

            // Get the BLuetoothDevice object
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);

            // Create the result Intent and include the MAC address
            //Intent intent = new Intent();
            //intent.PutExtra(EXTRA_DEVICE_ADDRESS, address);

            // Set result and finish this Activity
            //SetResult(Result.Ok, intent);
            //Finish();
            // TODO : implement connection
        }

        /*
         * On click listener for paired device.
         */
        void PairedListClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Get the device MAC address, which is the last 17 chars in the View
            var info = (e.View as TextView).Text.ToString();
            var address = info.Substring(info.Length - 17);
            var name = info.Substring(0, info.Length - 17);

            // Get the BLuetoothDevice object
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
            //connectToSelectdDevice(device);
        }

        // TODO :: thread로 돌려야 한다
        void connectToSelectdDevice(BluetoothDevice device)
        {
            // BT module UUID 입력 필요
            UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");

            try
            {
                // 소켓 생성
                mSocket = device.CreateRfcommSocketToServiceRecord(uuid);
                // RFCOMM 채널을 통한 연결
                mSocket.Connect();

                // 데이터 송수신을 위한 스트림 열기
                mOutputStream = mSocket.OutputStream;
                mInputStream = mSocket.InputStream;

                // 데이터 수신 준비
                //beginListenForData();
                // Start the thread to connect with the given device
                ConnectThread connectThread = new ConnectThread(device);
                connectThread.Start();
            }
            catch (System.Exception e)
            {
                // 블루투스 연결 중 오류 발생
                Log.Debug("CONNECTION",e.ToString());
                //Finish();   // 어플 종료
            }
        }

        protected class ConnectThread : Thread
        {
            private BluetoothSocket mmSocket;
            private BluetoothDevice mmDevice;

            public ConnectThread(BluetoothDevice device)
            {
                mmDevice = device;
                BluetoothSocket tmp = null;

                // Get a BluetoothSocket for a connection with the
                // given BluetoothDevice
                try
                {
                    tmp = device.CreateRfcommSocketToServiceRecord(applicationUUID);
                }
                catch (Java.IO.IOException e)
                {
                    Log.Error(TAG, "create() failed", e);
                }
                mmSocket = tmp;
            }

            public override void Run()
            {
                Log.Info(TAG, "BEGIN mConnectThread");
                Name = "ConnectThread";
                // Make a connection to the BluetoothSocket
                try
                {
                    // This is a blocking call and will only return on a
                    // successful connection or an exception
                    mmSocket.Connect();
                }
                catch (Java.IO.IOException e)
                {
                    try
                    {
                        mmSocket.Close();
                    }
                    catch (Java.IO.IOException e2)
                    {
                        Log.Error(TAG, "unable to close() socket during connection failure", e2);
                    }
                    return;
                }
            }

            public void Cancel()
            {
                try
                {
                    mmSocket.Close();
                }
                catch (Java.IO.IOException e)
                {
                    Log.Error(TAG, "close() of connect socket failed", e);
                }
            }
        }


            public class BTReceiver : BroadcastReceiver
        {
            Activity _sender;

            public BTReceiver(Activity sender)
            {
                _sender = sender;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                string action = intent.Action;

                // When discovery finds a device
                if (action == BluetoothDevice.ActionFound)
                {
                    // Get the BluetoothDevice object from the Intent
                    BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    // If it's already paired, skip it, because it's been listed already
                    if (device.BondState != Bond.Bonded)
                    {
                        newDevicesArrayAdapter.Add(device.Name + "\n" + device.Address);
                    }
                    // When discovery is finished, change the Activity title
                }
                else if (action == BluetoothAdapter.ActionDiscoveryFinished)
                {
                    _sender.SetProgressBarIndeterminateVisibility(false);
                    _sender.SetTitle(Resource.String.ApplicationName);
                    if (newDevicesArrayAdapter.Count == 0)
                    {
                        var noDevices = _sender.Resources.GetText(Resource.String.none_found).ToString();
                        newDevicesArrayAdapter.Add(noDevices);
                    }
                }
            }
        }
    }
}


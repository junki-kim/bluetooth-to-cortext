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
using Android.Bluetooth;

namespace BluetoothToCortex
{
    /*
     * BroadcastReceiver for Bluetooth device detecting
     */
    public class BTReceiver : BroadcastReceiver
    {
        Activity _sender;
        ArrayAdapter<string> _arrayAdapter;

        public BTReceiver(Activity sender, ArrayAdapter<string> DeviceArrayAdapter)
        {
            _sender = sender;
            _arrayAdapter = DeviceArrayAdapter;
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
                    _arrayAdapter.Add(device.Name + "\n" + device.Address);
                }
                // When discovery is finished, change the Activity title
            }
            else if (action == BluetoothAdapter.ActionDiscoveryFinished)
            {
                _sender.SetProgressBarIndeterminateVisibility(false);
                _sender.SetTitle(Resource.String.app_name);
                if (_arrayAdapter.Count == 0)
                {
                    var noDevices = _sender.Resources.GetText(Resource.String.none_found).ToString();
                    _arrayAdapter.Add(noDevices);
                }
            }
        }
    }
}
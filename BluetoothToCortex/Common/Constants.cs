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

namespace BluetoothToCortex
{
    static class Constants
    {
        // Message types sent from the BluetoothChatService Handler
        public const int MESSAGE_STATE_CHANGE = 1;
        public const int MESSAGE_READ = 2;
        public const int MESSAGE_WRITE = 3;
        public const int MESSAGE_DEVICE_NAME = 4;
        public const int MESSAGE_TOAST = 5;

        // Key names received from the BluetoothChatService Handler
        public const String DEVICE_NAME = "device_name";
        public const String TOAST = "toast";

    }
}
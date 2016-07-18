namespace BluetoothToCortex
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Graphics;
    using Android.OS;
    using Android.Provider;
    using Android.Widget;
    using Android.Util;
    using Java.IO;
    using Environment = Android.OS.Environment;
    using Uri = Android.Net.Uri;

    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }

    [Activity(Label = "@string/app_name", MainLauncher = false, Icon = "@drawable/icon")]
    public class CameraActivity : Activity
    {

        private ImageView mImageView;
        private ImageView mConvertedImageView;
        private Bitmap mImageToSend;
        private Button mSendBtn;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display
            // Loading the full sized image will consume to much memory 
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = mImageView.Height;

            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            Log.Debug("KJK", "height : " + height + " / width : " + width);
            Log.Debug("KJK", "bitmap size : " + App.bitmap.ByteCount);

            if (App.bitmap != null)
            {
                mImageView.SetImageBitmap(App.bitmap);
                App.bitmap = null;
                mSendBtn.Visibility = Android.Views.ViewStates.Visible;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.camera_activity);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button button = FindViewById<Button>(Resource.Id.myButton);
                mImageView = FindViewById<ImageView>(Resource.Id.imageView1);
                mSendBtn = FindViewById<Button>(Resource.Id.photo_send_button);
                button.Click += TakeAPicture;
                mSendBtn.Click += SendPicture;
            }

        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "BluetoothToCortex");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            App._file = new File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));

            StartActivityForResult(intent, 0);
        }

        private void SendPicture(object sender, EventArgs eventArgs)
        {
            // TODO: implement send image of _convertedImageView
        }
    }
}

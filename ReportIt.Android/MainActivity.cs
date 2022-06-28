using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.GoogleMaps.Android;

using ContextMenu.Droid;

using Android.Gms.Common;
//using Android.Gms.Location;

namespace ReportIt.Droid
{
    [Activity(Label = "Cheshire West and Chester", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity m_MainActivity = null;
        private bool m_bAppRunningInForeground = false;
        private AndroidServiceHelper reportItServicHelper = null;

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            m_MainActivity = this;

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");

            IsPlayServicesAvailable();

            Rg.Plugins.Popup.Popup.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Override default BitmapDescriptorFactory by your implementation. 
            var platformConfig = new PlatformConfig
            {
                BitmapDescriptorFactory = new CachingNativeBitmapDescriptorFactory()
            };

            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState, platformConfig);

            ContextMenuViewRenderer.Preserve();

            LoadApplication(new App());

            // Register and start this app as a foreground service.
            reportItServicHelper = new AndroidServiceHelper();
            reportItServicHelper.StartService();

            m_bAppRunningInForeground = true;
        }
        
        protected override void OnPause()
        {
            m_bAppRunningInForeground = false;

            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();

            m_bAppRunningInForeground = true;
        }

        protected override void OnDestroy()
        {
            // Explicitly closing this app also stops the foreground service.
            if (reportItServicHelper != null)
            {
                reportItServicHelper.StopService();
                reportItServicHelper = null;
            }

            base.OnDestroy();
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    System.Diagnostics.Trace.WriteLine(GoogleApiAvailability.Instance.GetErrorString(resultCode));
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("Sorry, this device is not supported");
                    Finish();
                }

                return false;
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Google Play Services is available.");

                return true;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool IsAppRunningInForeground()
        {
            return m_bAppRunningInForeground;
        }

        public override void OnBackPressed()
        {
            Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
        }
    }
}
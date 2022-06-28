using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Xamarin.Forms.GoogleMaps.iOS;

using ContextMenu.iOS;

using UserNotifications;

using BackgroundTasks;
using System.Threading.Tasks;

namespace ReportIt.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public static bool m_bAppRunningInForeground = true;

        private nint m_taskIDComms = -1;
        private System.Threading.EventWaitHandle hSignalShutdown = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset);
        private System.Threading.EventWaitHandle hShutdownComplete = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset);

        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Ask the user for permission to get notifications on iOS 10.0+
                UNUserNotificationCenter.Current.RequestAuthorization(
                    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (approved, error) => { });

                // Watch for notifications while app is active.
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // Ask the user for permission to get notifications on iOS 8.0+
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");

            global::Xamarin.Forms.Forms.Init();

            // Override default ImageFactory by your implementation. 
            var platformConfig = new PlatformConfig
            {
                ImageFactory = new CachingImageFactory()
            };

            Xamarin.FormsGoogleMaps.Init("AIzaSyDmD-8jWDmL8his_jxKr1yMwTXWLhkYQ8I", platformConfig);

            ContextMenuViewRenderer.Preserve();

            LoadApplication(new App());

            bool bReturn = base.FinishedLaunching(app, options);

            // Start a background-safe long running task to perform all the comms.
            m_bAppRunningInForeground = true;
            InitialiseBackgroundSafeComms();

            return bReturn;
        }

        private bool IsAppRunningInForeground()
        {
            return m_bAppRunningInForeground;
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            m_bAppRunningInForeground = false;

            base.DidEnterBackground(uiApplication);
        }

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            base.WillEnterForeground(uiApplication);

            if (m_taskIDComms != -1)
            {
                hSignalShutdown.Set();
                hShutdownComplete.WaitOne(30000);

                UIApplication.SharedApplication.EndBackgroundTask(m_taskIDComms);
                m_taskIDComms = -1;

                hSignalShutdown.Reset();
                hShutdownComplete.Reset();
            }

            m_bAppRunningInForeground = true;

            // Restart the background-safe comms thread.
            InitialiseBackgroundSafeComms();
        }

        public override void WillTerminate(UIApplication uiApplication)
        {
            if (m_taskIDComms != -1)
            {
                hSignalShutdown.Set();
                hShutdownComplete.WaitOne(30000);

                UIApplication.SharedApplication.EndBackgroundTask(m_taskIDComms);
                m_taskIDComms = -1;
            }

            base.WillTerminate(uiApplication);

            hSignalShutdown.Dispose();
            hSignalShutdown = null;

            hShutdownComplete.Dispose();
            hShutdownComplete = null;
        }

        private void InitialiseBackgroundSafeComms()
        {
            Task.Factory.StartNew(() => {

                // expirationHandler only called if background time allowed exceeded.
                m_taskIDComms = UIApplication.SharedApplication.BeginBackgroundTask(() => {
                    if (m_taskIDComms != -1)
                    {
                        hSignalShutdown.Set();
                        hShutdownComplete.WaitOne(30000);
                    }

                    UIApplication.SharedApplication.EndBackgroundTask(m_taskIDComms);
                    m_taskIDComms = -1;
                });

                // Use a stack of 3 empty comms queues and foreground tests.
                System.Collections.Generic.IList<bool> listShutdownConditions = new System.Collections.Generic.List<bool>();
                bool bShutdownCondition = false;

                bool bPreCommsObservationQueueEmpty = false;
                do
                {
                    Console.WriteLine(UIApplication.SharedApplication.BackgroundTimeRemaining);
                    if (UIApplication.SharedApplication.BackgroundTimeRemaining <= 10)
                    {
                        break;
                    }

                    bPreCommsObservationQueueEmpty = ((ReportIt.App)App.Current).TrySendPreCommsObservation();
                    bShutdownCondition = bPreCommsObservationQueueEmpty && !IsAppRunningInForeground();
                    listShutdownConditions.Add(bShutdownCondition);
                    while (listShutdownConditions.Count > 3)
                    {
                        listShutdownConditions.RemoveAt(0);
                    }

                    if (listShutdownConditions.Count >= 3)
                    {
                        if (listShutdownConditions[0] && listShutdownConditions[1] && listShutdownConditions[2])
                        {
                            break;
                        }
                    }
                } while (hSignalShutdown.WaitOne(20000) == false);

                if (m_taskIDComms != -1)
                {
                    UIApplication.SharedApplication.EndBackgroundTask(m_taskIDComms);
                    m_taskIDComms = -1;
                }

                hShutdownComplete.Set();
            });
        }
    }
}

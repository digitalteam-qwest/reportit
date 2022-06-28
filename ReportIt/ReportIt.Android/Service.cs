using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using System.Threading;

using Plugin.CurrentActivity;

namespace ReportIt.Droid
{
	internal class AndroidServiceHelper
	{
		private static Context context = global::Android.App.Application.Context;

		public void StartService()
		{
			Intent intent = new Intent(context, typeof(ReportItService));

			if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
			{
				context.StartForegroundService(intent);
			}
			else
			{
				context.StartService(intent);
			}

			intent = null;
		}

		public void StopService()
		{
			Intent intent = new Intent(context, typeof(ReportItService));
			context.StopService(intent);
			intent = null;
		}
	}

	[Service]
	public class ReportItService : Service
	{
		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public const int ServiceRunningNotifID = 9000;

		private System.Threading.EventWaitHandle hSignalShutdown = new EventWaitHandle(false, System.Threading.EventResetMode.ManualReset);
		private System.Threading.EventWaitHandle hShutdownComplete = new EventWaitHandle(false, System.Threading.EventResetMode.ManualReset);
		private System.Threading.Thread hThread = null;

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			NotificationHelper notificationHelper = new NotificationHelper();
			StartForeground(ServiceRunningNotifID, notificationHelper.ReturnNotif());
			notificationHelper = null;

			hThread = new Thread(ThreadProc);
			hThread.Start();

			return StartCommandResult.Sticky;
		}

		private void ThreadProc()
		{
			bool bPreCommsObservationQueueEmpty = false;

			// Use a stack of 3 empty comms queues and foreground tests.
			System.Collections.Generic.IList<bool> listShutdownConditions = new System.Collections.Generic.List<bool>();
			bool bShutdownCondition = false;

			do
			{
				bPreCommsObservationQueueEmpty = ((ReportIt.App)App.Current).TrySendPreCommsObservation();
				bShutdownCondition = bPreCommsObservationQueueEmpty && !ReportIt.Droid.MainActivity.m_MainActivity.IsAppRunningInForeground();
				listShutdownConditions.Add(bShutdownCondition);
				while (listShutdownConditions.Count > 3)
                {
					listShutdownConditions.RemoveAt(0);
				}

				if (listShutdownConditions.Count >= 3)
                {
					if (listShutdownConditions[0] && listShutdownConditions[1] && listShutdownConditions[2])
                    {
						// App can automatically shutdown.
						ReportIt.Droid.MainActivity.m_MainActivity.FinishAffinity();

						break;
                    }
                }
			} while (hSignalShutdown.WaitOne(20000) == false);

			listShutdownConditions = null;

			hShutdownComplete.Set();
		}

		public override void OnDestroy()
		{
			hSignalShutdown.Set();
			hShutdownComplete.WaitOne(30000);

			hSignalShutdown.Dispose();
			hSignalShutdown = null;

			hShutdownComplete.Dispose();
			hShutdownComplete = null;

			hThread = null;

			base.OnDestroy();
		}
	}

	internal class NotificationHelper
	{
		private static string foregroundChannelId = "9001";
		private static Context context = global::Android.App.Application.Context;

		public Notification ReturnNotif()
		{
			// Building intent
			Intent intent = new Intent(context, typeof(MainActivity));
			intent.AddFlags(ActivityFlags.SingleTop);
			intent.PutExtra("Title", "Message");

			PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);

			// More compatible (also workswith BlueStacks).
			Android.Support.V4.App.NotificationCompat.Builder notifBuilder = new Android.Support.V4.App.NotificationCompat.Builder(context, foregroundChannelId);
			//Notification.Builder notifBuilder = new Notification.Builder(context, foregroundChannelId);
			notifBuilder.SetContentTitle("Report It");
			notifBuilder.SetContentText("The Report It app will continue to run and will close automatically when your report has been submitted.");
			notifBuilder.SetSmallIcon(Resource.Drawable.CWAC_icon);
			notifBuilder.SetOngoing(true);
			notifBuilder.SetContentIntent(pendingIntent);

			// Building channel if API verion is 26 or above
			if (global::Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				NotificationChannel notificationChannel = new NotificationChannel(foregroundChannelId, "Title", NotificationImportance.High);
				notificationChannel.Importance = NotificationImportance.High;
				notificationChannel.EnableLights(true);
				notificationChannel.EnableVibration(true);
				notificationChannel.SetShowBadge(true);
				notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });

				var notifManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
				if (notifManager != null)
				{
					notifBuilder.SetChannelId(foregroundChannelId);
					notifManager.CreateNotificationChannel(notificationChannel);
				}
			}

			return notifBuilder.Build();
		}
	}
}

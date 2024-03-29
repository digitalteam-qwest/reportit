﻿using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(Xamarin.Forms.GoogleMaps.Clustering.ClusteredMap), typeof(Xamarin.Forms.GoogleMaps.Clustering.iOS.ClusteredMapRenderer))]

namespace ReportIt.iOS
{
    public class Application
    {
        // This is the main entry point of the application..
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}

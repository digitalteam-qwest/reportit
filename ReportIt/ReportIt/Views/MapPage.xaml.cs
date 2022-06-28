using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Reflection;
using Xamarin.Forms;

using ReportIt.ViewModels;

using Xamarin.Essentials;

using CsvHelper;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Services;

namespace ReportIt.Views
{
    class StreetlightCSVObject
    {
        public string site_code { get; set; }
        public string site_name { get; set; }
        public string feature_id { get; set; }
        public string feature_location { get; set; }
        public string easting { get; set; }
        public string northing { get; set; }
        public string asset_number { get; set; }
        public string feature_type_name { get; set; }
        public string feature_type_code { get; set; }
        public string feature_group_name { get; set; }
        public string feature_group_code { get; set; }
        public string central_asset_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    class BinCSVObject
    {
        [CsvHelper.Configuration.Attributes.Index(0)]
        public string centroid_e { get; set; }

        [CsvHelper.Configuration.Attributes.Index(1)]
        public string centroid_n { get; set; }

        [CsvHelper.Configuration.Attributes.Index(2)]
        public string asset_id { get; set; }

        [CsvHelper.Configuration.Attributes.Index(3)]
        public string site_name { get; set; }

        [CsvHelper.Configuration.Attributes.Index(4)]
        public string plot_no { get; set; }

        [CsvHelper.Configuration.Attributes.Index(5)]
        public string site_code { get; set; }

        [CsvHelper.Configuration.Attributes.Index(6)]
        public string feature_type_code { get; set; }

        [CsvHelper.Configuration.Attributes.Index(7)]
        public string feature_start_date { get; set; }

        [CsvHelper.Configuration.Attributes.Index(8)]
        public string bin_top_type { get; set; }

        [CsvHelper.Configuration.Attributes.Index(9)]
        public string attribute_material_type { get; set; }

        [CsvHelper.Configuration.Attributes.Index(10)]
        public string insp_type_name { get; set; }

        [CsvHelper.Configuration.Attributes.Index(11)]
        public string insp_route_code { get; set; }

        [CsvHelper.Configuration.Attributes.Index(12)]
        public string cleansing_code { get; set; }

        [CsvHelper.Configuration.Attributes.Index(13)]
        public string cleansing_day { get; set; }

        [CsvHelper.Configuration.Attributes.Index(14)]
        public string central_asset_id { get; set; }

        [CsvHelper.Configuration.Attributes.Index(15)]
        public string asset_number { get; set; }

        [CsvHelper.Configuration.Attributes.Index(16)]
        public string easting { get; set; }

        [CsvHelper.Configuration.Attributes.Index(17)]
        public string northing { get; set; }

        [CsvHelper.Configuration.Attributes.Index(18)]
        public string locations_text { get; set; }

        [CsvHelper.Configuration.Attributes.Index(19)]
        public double latitude { get; set; }

        [CsvHelper.Configuration.Attributes.Index(20)]
        public double longitude { get; set; }
    }

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MapPage : ContentPage
    {
        private bool bAppearing = false;

        private Xamarin.Forms.GoogleMaps.Map Map = null;

        private ReportIt.Models.CWACBoundary cwacBoundary = null;

        private Xamarin.Forms.GoogleMaps.Position pinStartPosition;

        private Xamarin.Forms.GoogleMaps.Pin Pin = null;

        private bool m_bStreetlightsSelected = false;
        private bool m_bBinsSelected = false;

        private Xamarin.Forms.GoogleMaps.Pin[] m_pinsArray = null;

        private System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Pin> m_pinsStreetlights = null;
        private System.Collections.Generic.IList<System.Tuple<double, double>> m_positionsStreetlights = null;

        private System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Pin> m_pinsBins = null;
        private System.Collections.Generic.IList<System.Tuple<double, double>> m_positionsBins = null;

        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconStreetlightOff = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconStreetlightOn = null;

        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconLitterBinOff = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconLitterBinOn = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconDogBinOff = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconDogBinOn = null;

        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconPoint = null;
        
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster2 = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster3 = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster4 = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster5 = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster6 = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster7 = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster8 = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster9 = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster10 = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster10Plus = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster25Plus = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster50Plus = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster100Plus = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster250Plus = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster500Plus = null;
        private Xamarin.Forms.GoogleMaps.BitmapDescriptor m_iconMapCluster1000Plus = null;

        private System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Pin> m_pinsClusters = null;
        private ReportIt.Models.FeaturesCollection featuresClusters = new Models.FeaturesCollection();

        ReportIt.Models.PreCommsObservations PreCommsObservations = null;

        ReportIt.Models.LegacyObservations LegacyObservations = null;

        ReportIt.ViewModels.CreateObservationViewModel CreateObservationViewModel = null;

        ReportIt.Views.CreateObservationPage CreateObservationPage = null;

        static string[] s_Days = new string[]
        {
            "SUNDAY",
            "MONDAY",
            "TUESDAY",
            "WEDNESDAY",
            "THURSDAY",
            "FRIDAY",
            "SATURDAY",
            "SUNDAY",
            "MONDAY",
            "TUESDAY",
            "WEDNESDAY",
            "THURSDAY",
            "FRIDAY",
            "SATURDAY"
        };

        public MapPage(
                ReportIt.ViewModels.ConfigurationViewModel vmConfiguration,
                ReportIt.Models.PreCommsObservations preCommsObservations,
                ReportIt.Models.LegacyObservations legacyObservations,
                ReportIt.ViewModels.CreateObservationViewModel createObservationViewModel,
                ReportIt.Views.CreateObservationPage createObservationPage
                )
        {
            this.BindingContext = vmConfiguration;

            PreCommsObservations = preCommsObservations;
            LegacyObservations = legacyObservations;
            CreateObservationViewModel = createObservationViewModel;
            CreateObservationPage = createObservationPage;

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, true);
            NavigationPage.SetBackButtonTitle(this, "Back");
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Xamarin.Forms.Color.FromHex("#504d68");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Xamarin.Forms.Color.Black;

            // Determine if Streetlight mode has been initiated.
            if (
                System.String.Compare(CreateObservationViewModel.SubjectCode, "SLO", true) == 0 ||
                System.String.Compare(CreateObservationViewModel.SubjectCode, "SLLP", true) == 0 ||
                System.String.Compare(CreateObservationViewModel.SubjectCode, "SLE", true) == 0 ||
                System.String.Compare(CreateObservationViewModel.SubjectCode, "SLDB", true) == 0
                )
            {
                m_bStreetlightsSelected = true;
            }
            else if (System.String.Compare(CreateObservationViewModel.SubjectCode, "SC15", true) == 0)
            {
                // Bins mode has been initiated.
                m_bBinsSelected = true;
            }
            else
            {
                // Normal mode has been initiated.
            }

            Map = new Xamarin.Forms.GoogleMaps.Map()
            {
                MapType = ((ConfigurationViewModel)this.BindingContext).MapType,
                MyLocationEnabled = false,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            Map.UiSettings.MyLocationButtonEnabled = false;

            // Map MyLocationButton clicked.
            Map.MyLocationButtonClicked += (sender, args) =>
            {
                args.Handled = false;
            };

            Map.MapClicked += (sender, args) =>
            {
                if (m_bStreetlightsSelected)
                {
                    if (Map.CameraPosition.Zoom >= 18)
                    {
                        if (Pin != null)
                        {
                            Pin.Icon = m_iconStreetlightOff;
                        }

                        Map.SelectedPin = Pin = null;

                        if (completeObservationButton.IsEnabled)
                        {
                            completeObservationButton.IsEnabled = false;
                        }
                    }
                }
                else if (m_bBinsSelected)
                {
                    if (Map.CameraPosition.Zoom >= 18)
                    {
                        if (Pin != null)
                        {
                            if (System.String.Compare(((BinCSVObject)Pin.Tag).feature_type_code, "DOBI", true) == 0)
                            {
                                Pin.Icon = m_iconDogBinOff;
                            }
                            else
                            {
                                Pin.Icon = m_iconLitterBinOff;
                            }
                        }

                        Map.SelectedPin = Pin = null;

                        if (completeObservationButton.IsEnabled)
                        {
                            completeObservationButton.IsEnabled = false;
                        }
                    }
                }
                else
                {
                    bool bPinInPolygon = cwacBoundary.IsPointInPolygon(args.Point.Latitude, args.Point.Longitude);
                    if (bPinInPolygon)
                    {
                        Pin.Position = new Xamarin.Forms.GoogleMaps.Position(args.Point.Latitude, args.Point.Longitude);
                        Map.InitialCameraUpdate = Xamarin.Forms.GoogleMaps.CameraUpdateFactory.NewPositionZoom(Pin.Position, 18);
                    }
                }
            };

            Map.CameraIdled += (sender, args) =>
            {
                if (m_bStreetlightsSelected)
                {
                    if (Pin != null)
                    {
                        Pin.Icon = m_iconStreetlightOff;
                    }

                    Map.SelectedPin = Pin = null;

                    if (completeObservationButton.IsEnabled)
                    {
                        completeObservationButton.IsEnabled = false;
                    }

                    if (args.Position.Zoom >= 1)
                    {
                        Xamarin.Forms.GoogleMaps.MapSpan bounds = Map.VisibleRegion;
                        double rectVisibleMapLeft = bounds.Center.Latitude - (bounds.LatitudeDegrees / 2.0f);
                        double rectVisibleMapRight = rectVisibleMapLeft + bounds.LatitudeDegrees;
                        double rectVisibleMapTop = bounds.Center.Longitude - (bounds.LongitudeDegrees / 2.0f);
                        double rectVisibleMapBottom = rectVisibleMapTop + bounds.LongitudeDegrees;

/*
                        Map.Polylines.Clear();
                        Xamarin.Forms.GoogleMaps.Polyline polylineObject = new Xamarin.Forms.GoogleMaps.Polyline();
                        polylineObject.StrokeWidth = 2f;
                        polylineObject.StrokeColor = Xamarin.Forms.Color.Green;
                        polylineObject.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(rectVisibleMapLeft, rectVisibleMapTop));
                        polylineObject.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(rectVisibleMapRight, rectVisibleMapTop));
                        polylineObject.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(rectVisibleMapRight, rectVisibleMapBottom));
                        polylineObject.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(rectVisibleMapLeft, rectVisibleMapBottom));
                        polylineObject.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(rectVisibleMapLeft, rectVisibleMapTop));
                        Map.Polylines.Add(polylineObject);
*/



                        double rectVisibleMapWidth = bounds.LatitudeDegrees;
                        bounds = null;

                        Xamarin.Forms.Rectangle rectPin = new Rectangle();
                        rectPin.Width = 0.0002f;   // approx 2 metres.
                        rectPin.Height = 0.0002f;  // approx 2 metres.

                        int nPinCount = 0;
                        for (int nIndex = 0; nIndex < m_pinsStreetlights.Count; nIndex++)
                        {
                            rectPin.Left = m_positionsStreetlights[nIndex].Item1 - 0.0001f;    // approx 1 metre.
                            rectPin.Top = m_positionsStreetlights[nIndex].Item2 - 0.00001f;     // approx 1 metre.

                            if (rectPin.Left >= rectVisibleMapLeft)
                            {
                                if (rectPin.Top >= rectVisibleMapTop)
                                {
                                    if (rectPin.Left <= rectVisibleMapRight)
                                    {
                                        if (rectPin.Top <= rectVisibleMapBottom)
                                        {
                                            m_pinsArray[nPinCount++] = m_pinsStreetlights[nIndex];
                                        }
                                    }
                                }
                            }
                        }

                        Map.Pins.Clear();

                        if (args.Position.Zoom >= 18)
                        {
                            for (int nIndex = 0; nIndex < nPinCount; nIndex++)
                            {
                                m_pinsArray[nIndex].Icon = m_iconStreetlightOff;
                                Map.Pins.Add(m_pinsArray[nIndex]);
                            }
                        }
                        else if (args.Position.Zoom >= 16)
                        {
                            for (int nIndex = 0; nIndex < nPinCount; nIndex++)
                            {
                                m_pinsArray[nIndex].Icon = m_iconPoint;
                                Map.Pins.Add(m_pinsArray[nIndex]);
                            }
                        }
                        else
                        {
                            // Clustering algorithm.
                            double dClusterDistance = rectVisibleMapWidth / 4;
                            Xamarin.Forms.GoogleMaps.BitmapDescriptor iconCluster = m_iconMapCluster2;

#if _DRAW_BOUNDING_BOX
                            Map.Polylines.Clear();
                            Xamarin.Forms.GoogleMaps.Polyline polylineBoundary = cwacBoundary.CreateBoundary();
                            polylineBoundary.StrokeWidth = 5f;
                            polylineBoundary.StrokeColor = Xamarin.Forms.Color.Red;
                            Map.Polylines.Add(polylineBoundary);
                            polylineBoundary = null;
#endif

                            double dDistance;
                            double dMinLatitude;
                            double dMaxLatitude;
                            double dMinLongitude;
                            double dMaxLongitude;

                            System.Collections.Generic.IList<System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Pin>> clusters = new System.Collections.Generic.List<System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Pin>>();
                            int nClusterCacheIndex = 0;

                            Xamarin.Forms.GoogleMaps.Pin pinCluster;
                            int nItemsInCluster;

                            for (int nIndexOuter = 0; nIndexOuter < nPinCount; nIndexOuter++)
                            {
                                if (m_pinsArray[nIndexOuter] != null)
                                {
                                    Xamarin.Forms.GoogleMaps.Position posOuter = m_pinsArray[nIndexOuter].Position;

                                    nItemsInCluster = 1;

                                    dMinLatitude = posOuter.Latitude;
                                    dMaxLatitude = posOuter.Latitude;
                                    dMinLongitude = posOuter.Longitude;
                                    dMaxLongitude = posOuter.Longitude;
                                    pinCluster = null;

                                    System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Pin> pinsCluster = new System.Collections.Generic.List<Xamarin.Forms.GoogleMaps.Pin>();
                                    for (int nIndexInner = 0; nIndexInner < nPinCount; nIndexInner++)
                                    {
                                        if (nIndexOuter != nIndexInner)
                                        {
                                            if (m_pinsArray[nIndexInner] != null)
                                            {
                                                Xamarin.Forms.GoogleMaps.Position posInner = m_pinsArray[nIndexInner].Position;

                                                dDistance = System.Math.Abs(posOuter.Latitude - posInner.Latitude) + System.Math.Abs(posOuter.Longitude - posInner.Longitude);
                                                if (dDistance < dClusterDistance)
                                                {
                                                    // Add pin to cluster.
                                                    nItemsInCluster++;

                                                    m_pinsArray[nIndexInner] = null;

                                                    // Create new cluster pin.
                                                    if (nClusterCacheIndex < m_pinsClusters.Count)
                                                    {
                                                        pinCluster = m_pinsClusters[nClusterCacheIndex];
                                                        nClusterCacheIndex++;
                                                    }
                                                    else
                                                    {
                                                        pinCluster = new Xamarin.Forms.GoogleMaps.Pin();
                                                        pinCluster.IsDraggable = false;
                                                        pinCluster.Label = "";
                                                        pinCluster.Address = "";
                                                        m_pinsClusters.Add(pinCluster);
                                                        nClusterCacheIndex++;
                                                    }

                                                    // Adjust cluster bounds.
                                                    dMinLatitude = System.Math.Min(dMinLatitude, posInner.Latitude);
                                                    dMaxLatitude = System.Math.Max(dMaxLatitude, posInner.Latitude);
                                                    dMinLongitude = System.Math.Min(dMinLongitude, posInner.Longitude);
                                                    dMaxLongitude = System.Math.Max(dMaxLongitude, posInner.Longitude);
                                                }
                                            }
                                        }
                                    }

                                    if (pinCluster != null)
                                    {
                                        m_pinsArray[nIndexOuter] = null;

                                        // Create the cluster pin.
                                        pinCluster.Position = new Xamarin.Forms.GoogleMaps.Position(
                                                                    dMinLatitude + ((dMaxLatitude - dMinLatitude) / 2.0f),
                                                                    dMinLongitude + ((dMaxLongitude - dMinLongitude) / 2.0f)
                                                                    );

                                        if (nItemsInCluster == 2)
                                        {
                                            iconCluster = m_iconMapCluster2;
                                        }
                                        else if (nItemsInCluster == 3)
                                        {
                                            iconCluster = m_iconMapCluster3;
                                        }
                                        else if (nItemsInCluster == 4)
                                        {
                                            iconCluster = m_iconMapCluster4;
                                        }
                                        else if (nItemsInCluster == 5)
                                        {
                                            iconCluster = m_iconMapCluster5;
                                        }
                                        else if (nItemsInCluster == 6)
                                        {
                                            iconCluster = m_iconMapCluster6;
                                        }
                                        else if (nItemsInCluster == 7)
                                        {
                                            iconCluster = m_iconMapCluster7;
                                        }
                                        else if (nItemsInCluster == 8)
                                        {
                                            iconCluster = m_iconMapCluster8;
                                        }
                                        else if (nItemsInCluster == 9)
                                        {
                                            iconCluster = m_iconMapCluster9;
                                        }
                                        else if (nItemsInCluster == 10)
                                        {
                                            iconCluster = m_iconMapCluster10;
                                        }
                                        else if (nItemsInCluster > 10 && nItemsInCluster <= 25)
                                        {
                                            iconCluster = m_iconMapCluster10Plus;
                                        }
                                        else if (nItemsInCluster > 25 && nItemsInCluster <= 50)
                                        {
                                            iconCluster = m_iconMapCluster25Plus;
                                        }
                                        else if (nItemsInCluster > 50 && nItemsInCluster <= 100)
                                        {
                                            iconCluster = m_iconMapCluster50Plus;
                                        }
                                        else if (nItemsInCluster > 100 && nItemsInCluster <= 250)
                                        {
                                            iconCluster = m_iconMapCluster100Plus;
                                        }
                                        else if (nItemsInCluster > 250 && nItemsInCluster <= 500)
                                        {
                                            iconCluster = m_iconMapCluster250Plus;
                                        }
                                        else if (nItemsInCluster > 500 && nItemsInCluster <= 1000)
                                        {
                                            iconCluster = m_iconMapCluster500Plus;
                                        }
                                        else
                                        {
                                            iconCluster = m_iconMapCluster1000Plus;
                                        }

                                        pinCluster.Icon = iconCluster;
                                        Map.Pins.Add(pinCluster);
                                    }
                                    else
                                    {
                                        // Unclustered pins remain.

                                        // Create new cluster pin.
                                        if (nClusterCacheIndex < m_pinsClusters.Count)
                                        {
                                            pinCluster = m_pinsClusters[nClusterCacheIndex];
                                            nClusterCacheIndex++;
                                        }
                                        else
                                        {
                                            pinCluster = new Xamarin.Forms.GoogleMaps.Pin();
                                            pinCluster.IsDraggable = false;
                                            pinCluster.Label = "";
                                            pinCluster.Address = "";
                                            m_pinsClusters.Add(pinCluster);
                                            nClusterCacheIndex++;
                                        }

                                        pinCluster.Position = new Xamarin.Forms.GoogleMaps.Position(dMinLatitude, dMinLongitude);
                                        pinCluster.Icon = m_iconPoint;
                                        Map.Pins.Add(pinCluster);
                                    }
                                }
                            }

                            iconCluster = null;
                        }
                    }
                    else
                    {
                        if (Map.Pins.Count > 0)
                        {
                            Map.Pins.Clear();
                        }
                    }
                }
                else if (m_bBinsSelected)
                {
                    if (Pin != null)
                    {
                        if (System.String.Compare(((BinCSVObject)Pin.Tag).feature_type_code, "DOBI", true) == 0)
                        {
                            Pin.Icon = m_iconDogBinOff;
                        }
                        else
                        {
                            Pin.Icon = m_iconLitterBinOff;
                        }
                    }

                    Map.SelectedPin = Pin = null;

                    if (completeObservationButton.IsEnabled)
                    {
                        completeObservationButton.IsEnabled = false;
                    }

                    if (args.Position.Zoom >= 1)
                    {
                        Xamarin.Forms.GoogleMaps.MapSpan bounds = Map.VisibleRegion;
                        double rectVisibleMapLeft = bounds.Center.Latitude - (bounds.LatitudeDegrees / 2.0f);
                        double rectVisibleMapRight = rectVisibleMapLeft + bounds.LatitudeDegrees;
                        double rectVisibleMapTop = bounds.Center.Longitude - (bounds.LongitudeDegrees / 2.0f);
                        double rectVisibleMapBottom = rectVisibleMapTop + bounds.LongitudeDegrees;

                        /*
                                                Map.Polylines.Clear();
                                                Xamarin.Forms.GoogleMaps.Polyline polylineObject = new Xamarin.Forms.GoogleMaps.Polyline();
                                                polylineObject.StrokeWidth = 2f;
                                                polylineObject.StrokeColor = Xamarin.Forms.Color.Green;
                                                polylineObject.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(rectVisibleMapLeft, rectVisibleMapTop));
                                                polylineObject.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(rectVisibleMapRight, rectVisibleMapTop));
                                                polylineObject.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(rectVisibleMapRight, rectVisibleMapBottom));
                                                polylineObject.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(rectVisibleMapLeft, rectVisibleMapBottom));
                                                polylineObject.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(rectVisibleMapLeft, rectVisibleMapTop));
                                                Map.Polylines.Add(polylineObject);
                        */

                        double rectVisibleMapWidth = bounds.LatitudeDegrees;
                        bounds = null;

                        Xamarin.Forms.Rectangle rectPin = new Rectangle();
                        rectPin.Width = 0.0002f;   // approx 2 metres.
                        rectPin.Height = 0.0002f;  // approx 2 metres.

                        int nPinCount = 0;
                        for (int nIndex = 0; nIndex < m_pinsBins.Count; nIndex++)
                        {
                            rectPin.Left = m_positionsBins[nIndex].Item1 - 0.0001f;    // approx 1 metre.
                            rectPin.Top = m_positionsBins[nIndex].Item2 - 0.00001f;     // approx 1 metre.

                            if (rectPin.Left >= rectVisibleMapLeft)
                            {
                                if (rectPin.Top >= rectVisibleMapTop)
                                {
                                    if (rectPin.Left <= rectVisibleMapRight)
                                    {
                                        if (rectPin.Top <= rectVisibleMapBottom)
                                        {
                                            m_pinsArray[nPinCount++] = m_pinsBins[nIndex];
                                        }
                                    }
                                }
                            }
                        }

                        Map.Pins.Clear();

                        if (args.Position.Zoom >= 18)
                        {
                            for (int nIndex = 0; nIndex < nPinCount; nIndex++)
                            {
                                if (System.String.Compare(((BinCSVObject)m_pinsArray[nIndex].Tag).feature_type_code, "DOBI", true) == 0)
                                {
                                    m_pinsArray[nIndex].Icon = m_iconDogBinOff;
                                }
                                else
                                {
                                    m_pinsArray[nIndex].Icon = m_iconLitterBinOff;
                                }

                                Map.Pins.Add(m_pinsArray[nIndex]);
                            }
                        }
                        else if (args.Position.Zoom >= 16)
                        {
                            for (int nIndex = 0; nIndex < nPinCount; nIndex++)
                            {
                                m_pinsArray[nIndex].Icon = m_iconPoint;
                                Map.Pins.Add(m_pinsArray[nIndex]);
                            }
                        }
                        else
                        {
                            // Clustering algorithm.
                            double dClusterDistance = rectVisibleMapWidth / 4;
                            Xamarin.Forms.GoogleMaps.BitmapDescriptor iconCluster = m_iconMapCluster2;

#if _DRAW_BOUNDING_BOX
                            Map.Polylines.Clear();
                            Xamarin.Forms.GoogleMaps.Polyline polylineBoundary = cwacBoundary.CreateBoundary();
                            polylineBoundary.StrokeWidth = 5f;
                            polylineBoundary.StrokeColor = Xamarin.Forms.Color.Red;
                            Map.Polylines.Add(polylineBoundary);
                            polylineBoundary = null;
#endif

                            double dDistance;
                            double dMinLatitude;
                            double dMaxLatitude;
                            double dMinLongitude;
                            double dMaxLongitude;

                            System.Collections.Generic.IList<System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Pin>> clusters = new System.Collections.Generic.List<System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Pin>>();
                            int nClusterCacheIndex = 0;

                            Xamarin.Forms.GoogleMaps.Pin pinCluster;
                            int nItemsInCluster;

                            for (int nIndexOuter = 0; nIndexOuter < nPinCount; nIndexOuter++)
                            {
                                if (m_pinsArray[nIndexOuter] != null)
                                {
                                    Xamarin.Forms.GoogleMaps.Position posOuter = m_pinsArray[nIndexOuter].Position;

                                    nItemsInCluster = 1;

                                    dMinLatitude = posOuter.Latitude;
                                    dMaxLatitude = posOuter.Latitude;
                                    dMinLongitude = posOuter.Longitude;
                                    dMaxLongitude = posOuter.Longitude;
                                    pinCluster = null;

                                    System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Pin> pinsCluster = new System.Collections.Generic.List<Xamarin.Forms.GoogleMaps.Pin>();
                                    for (int nIndexInner = 0; nIndexInner < nPinCount; nIndexInner++)
                                    {
                                        if (nIndexOuter != nIndexInner)
                                        {
                                            if (m_pinsArray[nIndexInner] != null)
                                            {
                                                Xamarin.Forms.GoogleMaps.Position posInner = m_pinsArray[nIndexInner].Position;

                                                dDistance = System.Math.Abs(posOuter.Latitude - posInner.Latitude) + System.Math.Abs(posOuter.Longitude - posInner.Longitude);
                                                if (dDistance < dClusterDistance)
                                                {
                                                    // Add pin to cluster.
                                                    nItemsInCluster++;

                                                    m_pinsArray[nIndexInner] = null;

                                                    // Create new cluster pin.
                                                    if (nClusterCacheIndex < m_pinsClusters.Count)
                                                    {
                                                        pinCluster = m_pinsClusters[nClusterCacheIndex];
                                                        nClusterCacheIndex++;
                                                    }
                                                    else
                                                    {
                                                        pinCluster = new Xamarin.Forms.GoogleMaps.Pin();
                                                        pinCluster.IsDraggable = false;
                                                        pinCluster.Label = "";
                                                        pinCluster.Address = "";
                                                        m_pinsClusters.Add(pinCluster);
                                                        nClusterCacheIndex++;
                                                    }

                                                    // Adjust cluster bounds.
                                                    dMinLatitude = System.Math.Min(dMinLatitude, posInner.Latitude);
                                                    dMaxLatitude = System.Math.Max(dMaxLatitude, posInner.Latitude);
                                                    dMinLongitude = System.Math.Min(dMinLongitude, posInner.Longitude);
                                                    dMaxLongitude = System.Math.Max(dMaxLongitude, posInner.Longitude);
                                                }
                                            }
                                        }
                                    }

                                    if (pinCluster != null)
                                    {
                                        m_pinsArray[nIndexOuter] = null;

                                        // Create the cluster pin.
                                        pinCluster.Position = new Xamarin.Forms.GoogleMaps.Position(
                                                                    dMinLatitude + ((dMaxLatitude - dMinLatitude) / 2.0f),
                                                                    dMinLongitude + ((dMaxLongitude - dMinLongitude) / 2.0f)
                                                                    );

                                        if (nItemsInCluster == 2)
                                        {
                                            iconCluster = m_iconMapCluster2;
                                        }
                                        else if (nItemsInCluster == 3)
                                        {
                                            iconCluster = m_iconMapCluster3;
                                        }
                                        else if (nItemsInCluster == 4)
                                        {
                                            iconCluster = m_iconMapCluster4;
                                        }
                                        else if (nItemsInCluster == 5)
                                        {
                                            iconCluster = m_iconMapCluster5;
                                        }
                                        else if (nItemsInCluster == 6)
                                        {
                                            iconCluster = m_iconMapCluster6;
                                        }
                                        else if (nItemsInCluster == 7)
                                        {
                                            iconCluster = m_iconMapCluster7;
                                        }
                                        else if (nItemsInCluster == 8)
                                        {
                                            iconCluster = m_iconMapCluster8;
                                        }
                                        else if (nItemsInCluster == 9)
                                        {
                                            iconCluster = m_iconMapCluster9;
                                        }
                                        else if (nItemsInCluster == 10)
                                        {
                                            iconCluster = m_iconMapCluster10;
                                        }
                                        else if (nItemsInCluster > 10 && nItemsInCluster <= 25)
                                        {
                                            iconCluster = m_iconMapCluster10Plus;
                                        }
                                        else if (nItemsInCluster > 25 && nItemsInCluster <= 50)
                                        {
                                            iconCluster = m_iconMapCluster25Plus;
                                        }
                                        else if (nItemsInCluster > 50 && nItemsInCluster <= 100)
                                        {
                                            iconCluster = m_iconMapCluster50Plus;
                                        }
                                        else if (nItemsInCluster > 100 && nItemsInCluster <= 250)
                                        {
                                            iconCluster = m_iconMapCluster100Plus;
                                        }
                                        else if (nItemsInCluster > 250 && nItemsInCluster <= 500)
                                        {
                                            iconCluster = m_iconMapCluster250Plus;
                                        }
                                        else if (nItemsInCluster > 500 && nItemsInCluster <= 1000)
                                        {
                                            iconCluster = m_iconMapCluster500Plus;
                                        }
                                        else
                                        {
                                            iconCluster = m_iconMapCluster1000Plus;
                                        }

                                        pinCluster.Icon = iconCluster;
                                        Map.Pins.Add(pinCluster);
                                    }
                                    else
                                    {
                                        // Unclustered pins remain.

                                        // Create new cluster pin.
                                        if (nClusterCacheIndex < m_pinsClusters.Count)
                                        {
                                            pinCluster = m_pinsClusters[nClusterCacheIndex];
                                            nClusterCacheIndex++;
                                        }
                                        else
                                        {
                                            pinCluster = new Xamarin.Forms.GoogleMaps.Pin();
                                            pinCluster.IsDraggable = false;
                                            pinCluster.Label = "";
                                            pinCluster.Address = "";
                                            m_pinsClusters.Add(pinCluster);
                                            nClusterCacheIndex++;
                                        }

                                        pinCluster.Position = new Xamarin.Forms.GoogleMaps.Position(dMinLatitude, dMinLongitude);
                                        pinCluster.Icon = m_iconPoint;
                                        Map.Pins.Add(pinCluster);
                                    }
                                }
                            }

                            iconCluster = null;
                        }
                    }
                    else
                    {
                        if (Map.Pins.Count > 0)
                        {
                            Map.Pins.Clear();
                        }
                    }
                }
            };

            stackMap.Children.Insert(0, Map);

            Map.InitialCameraUpdate = Xamarin.Forms.GoogleMaps.CameraUpdateFactory.NewPositionZoom(new Xamarin.Forms.GoogleMaps.Position(53.213000, -2.902000), 10);

            ((ConfigurationViewModel)this.BindingContext).PropertyChanged += OnMapTypeChanged;

            cwacBoundary = new ReportIt.Models.CWACBoundary();
            if (cwacBoundary.LoadData())
            {
                Xamarin.Forms.GoogleMaps.Polyline polylineBoundary = cwacBoundary.CreateBoundary();
                polylineBoundary.StrokeWidth = 5f;
                polylineBoundary.StrokeColor = Xamarin.Forms.Color.Red;
                Map.Polylines.Add(polylineBoundary);
                polylineBoundary = null;

                Map.MyLocationButtonClicked += Map_MyLocationButton;

                Map.PinDragStart += Map_PinDragStart;
                Map.PinDragging += Map_PinDragging;
                Map.PinDragEnd += Map_PinDragEnd;
            }

            if (m_bStreetlightsSelected)
            {
                Map.PinClicked += (sender, args) =>
                {
                    if (Map.CameraPosition.Zoom >= 18)
                    {
                        bool bValidSelection = false;

                        if (Pin != null)
                        {
                            Pin.Icon = m_iconStreetlightOff;
                        }

                        Map.SelectedPin = Pin = null;

                        if (args != null)
                        {
                            args.Handled = true;                    // required so that default pin handling is disabled.

                            if (args.Pin != null)
                            {
                                Map.SelectedPin = Pin = args.Pin;   // select the pin and display the info box but do not center.
                                Pin = args.Pin;
                                Pin.Icon = m_iconStreetlightOn;

                                bValidSelection = true;
                            }
                        }

                        completeObservationButton.IsEnabled = bValidSelection;
                    }
                };
            }
            else if (m_bBinsSelected)
            {
                Map.PinClicked += (sender, args) =>
                {
                    if (Map.CameraPosition.Zoom >= 18)
                    {
                        bool bValidSelection = false;

                        if (Pin != null)
                        {
                            if (System.String.Compare(((BinCSVObject)Pin.Tag).feature_type_code, "DOBI", true) == 0)
                            {
                                Pin.Icon = m_iconDogBinOff;
                            }
                            else
                            {
                                Pin.Icon = m_iconLitterBinOff;
                            }
                        }

                        Map.SelectedPin = Pin = null;

                        if (args != null)
                        {
                            args.Handled = true;                    // required so that default pin handling is disabled.

                            if (args.Pin != null)
                            {
                                Map.SelectedPin = Pin = args.Pin;   // select the pin and display the info box but do not center.
                                Pin = args.Pin;

                                if (System.String.Compare(((BinCSVObject)Pin.Tag).feature_type_code, "DOBI", true) == 0)
                                {
                                    Pin.Icon = m_iconDogBinOn;
                                }
                                else
                                {
                                    Pin.Icon = m_iconLitterBinOn;
                                }

                                bValidSelection = true;
                            }
                        }

                        completeObservationButton.IsEnabled = bValidSelection;
                    }
                };
            }

            if (m_bStreetlightsSelected)
            {
                // Pre-create cluster pins and cache.
                m_pinsClusters = new System.Collections.Generic.List<Xamarin.Forms.GoogleMaps.Pin>();
                Xamarin.Forms.GoogleMaps.Pin pin;
                for (int nIndex = 0; nIndex < 1000; nIndex++)
                {
                    pin = new Xamarin.Forms.GoogleMaps.Pin();
                    pin.Type = Xamarin.Forms.GoogleMaps.PinType.Generic;
                    pin.IsDraggable = false;
                    pin.Label = "";
                    pin.Address = "";
                    m_pinsClusters.Add(pin);
                }
                pin = null;

                // Create ALL Streetlights.
                string file = "lamp-off.png";
                var assembly = typeof(ReportIt.Views.MapPage).GetTypeInfo().Assembly;
                var streamStreetlightOff = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconStreetlightOff = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamStreetlightOff, id: file);

                file = "lamp-on.png";
                var streamStreetlightOn = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconStreetlightOn = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamStreetlightOn, id: file);

                file = "Point.png";
                var streamPoint = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconPoint = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamPoint, id: file);

                file = "MapCluster2.png";
                var streamMapCluster2 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster2 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster2, id: file);

                file = "MapCluster3.png";
                var streamMapCluster3 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster3 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster3, id: file);

                file = "MapCluster4.png";
                var streamMapCluster4 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster4 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster4, id: file);

                file = "MapCluster5.png";
                var streamMapCluster5 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster5 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster5, id: file);

                file = "MapCluster6.png";
                var streamMapCluster6 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster6 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster6, id: file);

                file = "MapCluster7.png";
                var streamMapCluster7 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster7 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster7, id: file);

                file = "MapCluster8.png";
                var streamMapCluster8 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster8 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster8, id: file);

                file = "MapCluster9.png";
                var streamMapCluster9 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster9 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster9, id: file);

                file = "MapCluster10.png";
                var streamMapCluster10 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster10 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster10, id: file);

                file = "MapCluster10+.png";
                var streamMapCluster10Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster10Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster10Plus, id: file);

                file = "MapCluster25+.png";
                var streamMapCluster25Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster25Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster25Plus, id: file);

                file = "MapCluster50+.png";
                var streamMapCluster50Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster50Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster50Plus, id: file);

                file = "MapCluster100+.png";
                var streamMapCluster100Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster100Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster100Plus, id: file);

                file = "MapCluster250+.png";
                var streamMapCluster250Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster250Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster250Plus, id: file);

                file = "MapCluster500+.png";
                var streamMapCluster500Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster500Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster500Plus, id: file);

                file = "MapCluster1000+.png";
                var streamMapCluster1000Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster1000Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster1000Plus, id: file);

                file = "Streetlights.csv";
                string strCacheDir = Xamarin.Essentials.FileSystem.CacheDirectory;
                string assetDatafileName = Path.Combine(strCacheDir, file);
                using (var streamStreetlitghtCSV = new FileStream(assetDatafileName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(streamStreetlitghtCSV))
                    {
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            //csv.Configuration.HasHeaderRecord = true;

                            m_pinsStreetlights = new System.Collections.Generic.List<Xamarin.Forms.GoogleMaps.Pin>();
                            m_positionsStreetlights = new System.Collections.Generic.List<System.Tuple<double, double>>();

                            System.Collections.Generic.IList<StreetlightCSVObject> listStreetlightObjects = csv.GetRecords<StreetlightCSVObject>().ToList();
                            foreach (StreetlightCSVObject streetlight in listStreetlightObjects)
                            {
                                CreateStreetlightPin(streetlight);
                            }

                            listStreetlightObjects.Clear();
                            listStreetlightObjects = null;
                        }
                    }
                }

                assetDatafileName = null;
                strCacheDir = null;

                file = null;
                assembly = null;

                m_pinsArray = new Xamarin.Forms.GoogleMaps.Pin[m_pinsStreetlights.Count];
            }
            else if (m_bBinsSelected)
            {
                // Pre-create cluster pins and cache.
                m_pinsClusters = new System.Collections.Generic.List<Xamarin.Forms.GoogleMaps.Pin>();
                Xamarin.Forms.GoogleMaps.Pin pin;
                for (int nIndex = 0; nIndex < 1000; nIndex++)
                {
                    pin = new Xamarin.Forms.GoogleMaps.Pin();
                    pin.Type = Xamarin.Forms.GoogleMaps.PinType.Generic;
                    pin.IsDraggable = false;
                    pin.Label = "";
                    pin.Address = "";
                    m_pinsClusters.Add(pin);
                }
                pin = null;

                // Create ALL Bins.
                string file = "litter-bin-unselected.png";
                var assembly = typeof(ReportIt.Views.MapPage).GetTypeInfo().Assembly;
                var streamLitterBinOff = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconLitterBinOff = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamLitterBinOff, id: file);

                file = "litter-bin-selected.png";
                var streamLitterBinOn = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconLitterBinOn = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamLitterBinOn, id: file);

                file = "dog-bin-selected.png";
                var streamDogBinOn = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconDogBinOn = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamDogBinOn, id: file);

                file = "dog-bin-unselected.png";
                var streamDogBinOff = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconDogBinOff = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamDogBinOff, id: file);

                file = "Point.png";
                var streamPoint = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconPoint = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamPoint, id: file);

                file = "MapCluster2.png";
                var streamMapCluster2 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster2 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster2, id: file);

                file = "MapCluster3.png";
                var streamMapCluster3 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster3 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster3, id: file);

                file = "MapCluster4.png";
                var streamMapCluster4 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster4 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster4, id: file);

                file = "MapCluster5.png";
                var streamMapCluster5 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster5 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster5, id: file);

                file = "MapCluster6.png";
                var streamMapCluster6 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster6 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster6, id: file);

                file = "MapCluster7.png";
                var streamMapCluster7 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster7 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster7, id: file);

                file = "MapCluster8.png";
                var streamMapCluster8 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster8 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster8, id: file);

                file = "MapCluster9.png";
                var streamMapCluster9 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster9 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster9, id: file);

                file = "MapCluster10.png";
                var streamMapCluster10 = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster10 = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster10, id: file);

                file = "MapCluster10+.png";
                var streamMapCluster10Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster10Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster10Plus, id: file);

                file = "MapCluster25+.png";
                var streamMapCluster25Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster25Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster25Plus, id: file);

                file = "MapCluster50+.png";
                var streamMapCluster50Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster50Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster50Plus, id: file);

                file = "MapCluster100+.png";
                var streamMapCluster100Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster100Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster100Plus, id: file);

                file = "MapCluster250+.png";
                var streamMapCluster250Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster250Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster250Plus, id: file);

                file = "MapCluster500+.png";
                var streamMapCluster500Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster500Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster500Plus, id: file);

                file = "MapCluster1000+.png";
                var streamMapCluster1000Plus = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
                m_iconMapCluster1000Plus = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(streamMapCluster1000Plus, id: file);

                file = "Bins.csv";
                string strCacheDir = Xamarin.Essentials.FileSystem.CacheDirectory;
                string assetDatafileName = Path.Combine(strCacheDir, file);
                using (var streamBinsCSV = new FileStream(assetDatafileName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(streamBinsCSV))
                    {
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            //csv.Configuration.HasHeaderRecord = true;

                            m_pinsBins = new System.Collections.Generic.List<Xamarin.Forms.GoogleMaps.Pin>();
                            m_positionsBins = new System.Collections.Generic.List<System.Tuple<double, double>>();

                            System.DateTime dtToday = System.DateTime.Now;
                            string strToday = dtToday.DayOfWeek.ToString();
                            System.Collections.Generic.Dictionary<string, string> dictCleansingDayCache = new System.Collections.Generic.Dictionary<string, string>();
                            dictCleansingDayCache.Add("EVERY DAY", dtToday.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            dictCleansingDayCache.Add("NOT ROUTED", "NOT ROUTED");
                            dictCleansingDayCache.Add("NOT APPLICABLE", "NOT APPLICABLE");

                            string strNextCleansingDayText;

                            System.Collections.Generic.IList<BinCSVObject> listBinObjects = csv.GetRecords<BinCSVObject>().ToList();
                            foreach (BinCSVObject bin in listBinObjects)
                            {
                                strNextCleansingDayText = GenerateNextCleansingDayText(dtToday, strToday, bin.cleansing_day.ToUpper(), dictCleansingDayCache);
                                CreateBinPin(bin, strNextCleansingDayText);
                            }

                            listBinObjects.Clear();
                            listBinObjects = null;

                            strNextCleansingDayText = null;

                            dictCleansingDayCache.Clear();
                            dictCleansingDayCache = null;

                            strToday = null;
                        }
                    }
                }

                assetDatafileName = null;
                strCacheDir = null;

                file = null;
                assembly = null;

                m_pinsArray = new Xamarin.Forms.GoogleMaps.Pin[m_pinsBins.Count];
            }
            else
            {
                completeObservationButton.IsEnabled = true;

                float fLatitude = (float)Map.CameraPosition.Target.Latitude;
                float fLongitude = (float)Map.CameraPosition.Target.Longitude;
                CreatePin(fLatitude, fLongitude);
            }
        }

        protected override void OnAppearing()
        {
            if (!bAppearing)
            {
                bAppearing = true;

                Device.BeginInvokeOnMainThread(
                    async () =>
                    {
                        // Prevent UI controls from being modified by the user.
                        this.IsBusy = true;
                        this.IsEnabled = false;

                        // Replace the map with a temporary busy/activity indicator.
                        Xamarin.Forms.ActivityIndicator activityIndicator = new Xamarin.Forms.ActivityIndicator();
                        activityIndicator.IsRunning = false;
                        activityIndicator.IsVisible = false;
                        activityIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;
                        activityIndicator.HorizontalOptions = LayoutOptions.Fill;

                        stackMap.Children.RemoveAt(0);
                        stackMap.Children.Insert(0, activityIndicator);
                        activityIndicator.IsRunning = true;
                        activityIndicator.IsVisible = true;

                        LocationWrapper locationWrapper = await ((ReportIt.App)App.Current).TestLocationServicesEnabled();
                        if (locationWrapper.LocationServicesEnabled)
                        {
                            // JLD.
                            Map.MyLocationEnabled = true;
                            Map.UiSettings.MyLocationButtonEnabled = true;


                            bool bPinInPolygon = cwacBoundary.IsPointInPolygon(locationWrapper.Location.Latitude, locationWrapper.Location.Longitude);
                            if (bPinInPolygon)
                            {
                                if (!m_bStreetlightsSelected && !m_bBinsSelected)
                                {
                                    Pin.Position = new Xamarin.Forms.GoogleMaps.Position(locationWrapper.Location.Latitude, locationWrapper.Location.Longitude);
                                }

                                Map.InitialCameraUpdate = Xamarin.Forms.GoogleMaps.CameraUpdateFactory.NewPositionZoom(
                                                                                                            new Xamarin.Forms.GoogleMaps.Position(locationWrapper.Location.Latitude, locationWrapper.Location.Longitude),
                                                                                                            18
                                                                                                            );
                            }
                        }
                        else
                        {
                            //await DisplayAlert("Location Services", "Location Services are currently disabled for this app, please enable to use GPS/Current Location.", "OK");
                            DisplayAlert("Location Services", "Location Services are currently disabled for this app, please enable to use GPS/Current Location.", "OK");
                        }

                        locationWrapper.Location = null;
                        locationWrapper = null;

                        // Remove the busy.activity indicator and restore the map.
                        activityIndicator.IsRunning = false;
                        activityIndicator.IsVisible = false;

                        stackMap.Children.RemoveAt(0);
                        stackMap.Children.Insert(0, Map);

                        // Re-enable the UI controls.
                        this.IsBusy = false;
                        this.IsEnabled = true;
                    }
                    );
            }

            base.OnAppearing();

            if (m_bStreetlightsSelected)
            {
                // Display clustering instructions.
                ReportIt.Views.StreetlightClusterInstructionsPopup streetlightClusteringInstructionsPopup = new ReportIt.Views.StreetlightClusterInstructionsPopup();
                PopupNavigation.Instance.PushAsync(streetlightClusteringInstructionsPopup);
                //clusteringInstructionsPopup = null;
            }
            else if (m_bBinsSelected)
            {
                // Display clustering instructions.
                ReportIt.Views.BinClusterInstructionsPopup binClusteringInstructionsPopup = new ReportIt.Views.BinClusterInstructionsPopup();
                PopupNavigation.Instance.PushAsync(binClusteringInstructionsPopup);
                //clusteringInstructionsPopup = null;
            }
        }

        private async void Map_MyLocationButton(object sender, Xamarin.Forms.GoogleMaps.MyLocationButtonClickedEventArgs e)
        {
            e.Handled = true;

            Xamarin.Forms.GoogleMaps.CameraPosition cameraPositionCurrent = Map.CameraPosition;

            // Prevent UI controls from being modified by the user.
            this.IsBusy = true;
            this.IsEnabled = false;

            // Replace the map with a temporary busy/activity indicator.
            Xamarin.Forms.ActivityIndicator activityIndicator = new Xamarin.Forms.ActivityIndicator();
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;
            activityIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;
            activityIndicator.HorizontalOptions = LayoutOptions.Fill;

            stackMap.Children.RemoveAt(0);
            stackMap.Children.Insert(0, activityIndicator);
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;

            LocationWrapper locationWrapper = await ((ReportIt.App)App.Current).TestLocationServicesEnabled();
            if (locationWrapper.LocationServicesEnabled)
            {
                bool bPinInPolygon = cwacBoundary.IsPointInPolygon(locationWrapper.Location.Latitude, locationWrapper.Location.Longitude);
                if (bPinInPolygon)
                {
                    if (!m_bStreetlightsSelected && !m_bBinsSelected)
                    {
                        Pin.Position = new Xamarin.Forms.GoogleMaps.Position(locationWrapper.Location.Latitude, locationWrapper.Location.Longitude);
                    }

                    Map.InitialCameraUpdate = Xamarin.Forms.GoogleMaps.CameraUpdateFactory.NewPositionZoom(
                                                                                                 new Xamarin.Forms.GoogleMaps.Position(locationWrapper.Location.Latitude, locationWrapper.Location.Longitude),
                                                                                                 18
                                                                                                 );
                }
                else
                {
                    Map.InitialCameraUpdate = Xamarin.Forms.GoogleMaps.CameraUpdateFactory.NewPositionZoom(cameraPositionCurrent.Target, cameraPositionCurrent.Zoom);

                    await DisplayAlert("Report Location", "Reports must specify a location within the Cheshire West and Chester area.", "OK");
                }
            }
            else
            {
                Map.InitialCameraUpdate = Xamarin.Forms.GoogleMaps.CameraUpdateFactory.NewPositionZoom(cameraPositionCurrent.Target, cameraPositionCurrent.Zoom);

                await DisplayAlert("Location Services", "Location Services are currently disabled on this device, please enable to use GPS/Current Location.", "OK");
            }

            locationWrapper.Location = null;
            locationWrapper = null;

            // Remove the busy.activity indicator and restore the map.
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;

            stackMap.Children.RemoveAt(0);
            stackMap.Children.Insert(0, Map);

            // Re-enable the UI controls.
            this.IsBusy = false;
            this.IsEnabled = true;

            cameraPositionCurrent = null;
        }

        private void Map_PinDragStart(object sender, Xamarin.Forms.GoogleMaps.PinDragEventArgs e)
        {
            pinStartPosition = Pin.Position;
        }

        private void Map_PinDragging(object sender, Xamarin.Forms.GoogleMaps.PinDragEventArgs e)
        {
            bool bPinInPolygon = cwacBoundary.IsPointInPolygon(e.Pin.Position.Latitude, e.Pin.Position.Longitude);
            if (!bPinInPolygon)
            {
                e.Pin.Position = pinStartPosition;
            }
            else
            {
                pinStartPosition = e.Pin.Position;
            }
        }

        private void Map_PinDragEnd(object sender, Xamarin.Forms.GoogleMaps.PinDragEventArgs e)
        {
            bool bPinInPolygon = cwacBoundary.IsPointInPolygon(e.Pin.Position.Latitude, e.Pin.Position.Longitude);
            if (!bPinInPolygon)
            {
                e.Pin.Position = pinStartPosition;
            }
            else
            {
                pinStartPosition = e.Pin.Position;
            }
        }

        public void OnMapTypeChanged(object sender, PropertyChangedEventArgs e)
        {
            Xamarin.Forms.GoogleMaps.CameraPosition posCurrent = new Xamarin.Forms.GoogleMaps.CameraPosition(
                                                                                                    Map.CameraPosition.Target,
                                                                                                    Map.CameraPosition.Zoom,
                                                                                                    Map.CameraPosition.Bearing,
                                                                                                    Map.CameraPosition.Tilt
                                                                                                    );

            Map.MapType = ((ReportIt.ViewModels.ConfigurationViewModel)sender).MapType;

            Map.InitialCameraUpdate = Xamarin.Forms.GoogleMaps.CameraUpdateFactory.NewPositionZoom(posCurrent.Target, posCurrent.Zoom);

            posCurrent = null;
        }

        public void CreatePin(float fLatitude, float fLongitude)
        {
            Pin = new Xamarin.Forms.GoogleMaps.Pin();
            Pin.Type = Xamarin.Forms.GoogleMaps.PinType.Place;
            Pin.IsDraggable = true;
            Pin.Address = "Tap on the map to change location";
            Pin.Label = "New report location";
            Pin.Position = new Xamarin.Forms.GoogleMaps.Position(fLatitude, fLongitude);

            string file = "MapMarker.png";
            var assembly = typeof(ReportIt.Views.MapPage).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
            Pin.Icon = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(stream, id: file);
            Pin.Tag = null;
            Map.Pins.Add(Pin);

            Map.SelectedPin = Pin;
        }

        private void CreateStreetlightPin(StreetlightCSVObject streetlightCSVObject)
        {
            Pin = new Xamarin.Forms.GoogleMaps.Pin();
            Pin.Type = Xamarin.Forms.GoogleMaps.PinType.Generic;//Xamarin.Forms.GoogleMaps.PinType.Place;
            Pin.IsDraggable = false;
            Pin.Address = streetlightCSVObject.feature_location;
            Pin.Label = "Column No " + streetlightCSVObject.feature_id.Trim();
            Pin.Position = new Xamarin.Forms.GoogleMaps.Position(streetlightCSVObject.latitude, streetlightCSVObject.longitude);
            Pin.IsVisible = true;
            Pin.Icon = m_iconPoint;
            Pin.Tag = streetlightCSVObject;

            m_pinsStreetlights.Add(Pin);
            m_positionsStreetlights.Add(new System.Tuple<double, double>(streetlightCSVObject.latitude, streetlightCSVObject.longitude));
        }

        private string GenerateNextCleansingDayText(
                                System.DateTime dtToday,
                                string strToday,
                                string strCleansingDay,
                                System.Collections.Generic.Dictionary<string, string> dictCleansingDayCache
                                )
        {
            string strNextCleansingDayText = "";
            string strNextCleansingDate = "";

            if (string.IsNullOrEmpty(strCleansingDay))
            {
                return strNextCleansingDayText;
            }
            else if (string.Compare(strCleansingDay, "NOT ROUTED", true) == 0)
            {
                return strNextCleansingDayText;
            }
            else if (string.Compare(strCleansingDay, "NOT APPLICABLE", true) == 0)
            {
                return strNextCleansingDayText;
            }

            bool bFound = false;

            try
            {
                strNextCleansingDate = dictCleansingDayCache[strCleansingDay];
                bFound = true;
            }
            catch (System.Exception ex) { }

            if (!bFound)
            {
                // Possible data values;
                //      Every Day
                //      Not Routed
                //      Not Applicable
                //      {day}
                //      {day} & {day}
                //      {day} & {day} & {day}

                // Step 1.
                // Eliminate Every Day/Not Routed/Not Applicable.

                // Step 2.
                // Find current day in days array.
                // From that point onwards find cleansing day in days array.
                // Subtract array elements to calculate distance in days.
                int nToday = -1;
                int nCleansingDay = -1;
                for (int nDayOfWeek = 0; nDayOfWeek < s_Days.Length; nDayOfWeek++)
                {
                    if (nToday == -1)
                    {
                        if (System.String.Compare(strToday, s_Days[nDayOfWeek], true) == 0)
                        {
                            nToday = nDayOfWeek;
                        }
                    }

                    if (nToday != -1)
                    {
                        if ((nCleansingDay == -1) && strCleansingDay.Contains(s_Days[nDayOfWeek]))
                        {
                            nCleansingDay = nDayOfWeek;
                        }
                    }

                    if ((nToday != -1) && (nCleansingDay != -1))
                    {
                        break;
                    }
                }

                // Step 3.
                // Cache results to optimize other query retrievals.
                if ((nToday != -1) && (nCleansingDay != -1))
                {
                    System.DateTime dtNextCleansingDay = dtToday.AddDays(nCleansingDay - nToday);
                    strNextCleansingDate = dtNextCleansingDay.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dictCleansingDayCache.Add(strCleansingDay, strNextCleansingDate);

                    bFound = true;
                }
            }

            if (bFound)
            {
                if (strNextCleansingDate != null)
                {
                    strNextCleansingDayText = $"Emptied next on the: {strNextCleansingDate}";
                }
            }

            strNextCleansingDate = null;

            return strNextCleansingDayText;
        }

        private void CreateBinPin(BinCSVObject binCSVObject, string strNextCleansingDayText)
        {
            Pin = new Xamarin.Forms.GoogleMaps.Pin();
            Pin.Type = Xamarin.Forms.GoogleMaps.PinType.Place;
            Pin.IsDraggable = false;

            // Compose the pin label text.
            string strBinType = "Unknown";
            if (System.String.Compare(binCSVObject.feature_type_code, "LIBI", true) == 0)
            {
                strBinType = "Litter Bin";
            }
            else if (System.String.Compare(binCSVObject.feature_type_code, "DOBI", true) == 0)
            {
                strBinType = "Dog Bin";
            }
            else if (System.String.Compare(binCSVObject.feature_type_code, "LAYB", true) == 0)
            {
                strBinType = "Lay by Bin";
            }
            else if (System.String.Compare(binCSVObject.feature_type_code, "BB", true) == 0)
            {
                strBinType = "Big Belly Bin";
            }
            else if (System.String.Compare(binCSVObject.feature_type_code, "DUAL", true) == 0)
            {
                strBinType = "Litter/Recycle Bin";
            }
            else if (System.String.Compare(binCSVObject.feature_type_code, "REBI", true) == 0)
            {
                strBinType = "Recycling Bin";
            }
            Pin.Label = $"{strBinType} at {binCSVObject.site_name}";
            strBinType = null;

            Pin.Address = strNextCleansingDayText;
            Pin.Position = new Xamarin.Forms.GoogleMaps.Position(binCSVObject.latitude, binCSVObject.longitude);
            Pin.IsVisible = true;
            Pin.Tag = binCSVObject;

            if (System.String.Compare(binCSVObject.feature_type_code, "DOBI", true) == 0)
            {
                Pin.Icon = m_iconDogBinOff;
            }
            else
            {
                Pin.Icon = m_iconLitterBinOff;
            }

            m_pinsBins.Add(Pin);
            m_positionsBins.Add(new System.Tuple<double, double>(binCSVObject.latitude, binCSVObject.longitude));
        }

        private void CreateLegacyPins()
        {
            foreach (ReportIt.Models.LegacyObservation legacyObservation in LegacyObservations.LegacyObservationsObservableCollection)
            {
                CreateLegacyPin(legacyObservation);
            }
        }

        public void CreateLegacyPin(ReportIt.Models.LegacyObservation legacyObservation)
        {
            Pin = new Xamarin.Forms.GoogleMaps.Pin();
            Pin.Type = Xamarin.Forms.GoogleMaps.PinType.Place;
            Pin.IsDraggable = false;
            Pin.Label = legacyObservation.issue;
            Pin.Address = legacyObservation.caseId + "\r\n" + legacyObservation.location;

            Pin.Position = new Xamarin.Forms.GoogleMaps.Position(
                                                            legacyObservation.latitude,
                                                            legacyObservation.longitude
                                                            );

            string file = "LegacyMapMarker.png";
            var assembly = typeof(ReportIt.Views.MapPage).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream($"ReportIt.{file}") ?? assembly.GetManifestResourceStream($"ReportIt.local.{file}");
            Pin.Icon = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromStream(stream, id: file);

            Map.Pins.Add(Pin);
        }

        private async void CompleteObservationButton_OnClicked(object sender, EventArgs args)
        {
            // Ask the user if they wish to submit a photo as part of the observation.
            string strAction = await DisplayActionSheet(
                                        "Would you like to submit a photograph with this report?",
                                        "Cancel",
                                        null,
                                        "Yes",
                                        "No"
                                        );

            if (strAction == null)
            {
                return;
            }

            if (strAction.Equals("Cancel", System.StringComparison.OrdinalIgnoreCase))
            {
                strAction = null;

                return;
            }

            bool bNewPhotograph = false;
            bool bLibraryPhotograph = false;
            if (strAction.Equals("Yes", System.StringComparison.OrdinalIgnoreCase))
            {
                // Ask the user if they wish to take a new photo or choose one from the photo library.
                strAction = await DisplayActionSheet(
                                        "Would you like to use the camera to take a new photograph now or use a photograph from your photo library?",
                                        "Cancel",
                                        null,
                                        "Take a new photograph",
                                        "Select photograph from library"
                                        );

                if (strAction == null)
                {
                    return;
                }

                if (strAction.Equals("Cancel", System.StringComparison.OrdinalIgnoreCase))
                {
                    strAction = null;

                    return;
                }

                if (strAction.Equals("Take a new photograph", System.StringComparison.OrdinalIgnoreCase))
                {
                    bNewPhotograph = true;
                }
                else if (strAction.Equals("Select photograph from library", System.StringComparison.OrdinalIgnoreCase))
                {
                    bLibraryPhotograph = true;
                }
            }
            strAction = null;

            // Obtain the observation photograph if appropriate.
            Plugin.Media.Abstractions.MediaFile imageObservation = null;
            if (bNewPhotograph)
            {
                imageObservation = await GetPhotoFromCamera();
            }
            else if (bLibraryPhotograph)
            {
                imageObservation = await GetPhotoFromLibrary();
            }

            if ((Pin != null) && (Pin.Tag != null))
            {
                if (m_bStreetlightsSelected)
                {
                    StreetlightCSVObject streetlightCSVObject = (StreetlightCSVObject)Pin.Tag;

                    double dEastings = 0.0f;
                    System.Double.TryParse(streetlightCSVObject.easting, out dEastings);

                    double dNorthings = 0.0f;
                    System.Double.TryParse(streetlightCSVObject.northing, out dNorthings);

                    CreateObservationViewModel.SetStreetlightData(
                                                        dEastings,
                                                        dNorthings,
                                                        streetlightCSVObject.site_code,
                                                        streetlightCSVObject.central_asset_id
                                                        );

                    streetlightCSVObject = null;
                }
                else if (m_bBinsSelected)
                {
                    BinCSVObject binCSVObject = (BinCSVObject)Pin.Tag;

                    double dEastings = 0.0f;
                    System.Double.TryParse(binCSVObject.easting, out dEastings);

                    double dNorthings = 0.0f;
                    System.Double.TryParse(binCSVObject.northing, out dNorthings);

                    CreateObservationViewModel.SetBinData(
                                                    dEastings,
                                                    dNorthings,
                                                    binCSVObject.site_code,
                                                    binCSVObject.central_asset_id
                                                    );

                    binCSVObject = null;
                }
            }

            if (
                (!bNewPhotograph && !bLibraryPhotograph) ||
                ((bNewPhotograph || bLibraryPhotograph) && imageObservation != null)
                )
            {
                CreateObservationViewModel.SetAdditionalData(
                                                    Pin.Position.Latitude,
                                                    Pin.Position.Longitude,
                                                    imageObservation
                                                    );

                // Prevent UI controls from being modified by the user.
                this.IsBusy = true;
                this.IsEnabled = false;

                // Replace the map with a temporary busy/activity indicator.
                Xamarin.Forms.ActivityIndicator activityIndicator = new Xamarin.Forms.ActivityIndicator();
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;
                activityIndicator.VerticalOptions = LayoutOptions.CenterAndExpand;
                activityIndicator.HorizontalOptions = LayoutOptions.Fill;

                stackMap.Children.RemoveAt(0);
                stackMap.Children.Insert(0, activityIndicator);
                activityIndicator.IsRunning = true;
                activityIndicator.IsVisible = true;

                // This call/UI thread returns at this point whilst the CreatePreCommsObservation() task runs.
                await System.Threading.Tasks.Task.Run(() =>
                {
                    CreateObservationPage.CreatePreCommsObservation();
                }
                );

                if (imageObservation != null)
                {
                    imageObservation.Dispose();
                    imageObservation = null;
                }

                // UI thread picks up here after the PostObservation() task completes.

                // Hide the activity indicator.
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;
                stackMap.Children.RemoveAt(0);

                // Re-enable the UI controls.
                this.IsBusy = false;
                this.IsEnabled = true;

                await Navigation.PopModalAsync();
                CreateObservationPage.ClosePage();
            }
        }

        private async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async System.Threading.Tasks.Task<Plugin.Media.Abstractions.MediaFile> GetPhotoFromCamera()
        {
            Plugin.Media.Abstractions.MediaFile imageCamera = null;

            if (Plugin.Media.CrossMedia.Current.IsCameraAvailable && Plugin.Media.CrossMedia.Current.IsTakePhotoSupported)
            {
                try
                {
                    imageCamera = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });
                }
                catch (System.Exception ex) { }
            }

            return imageCamera;
        }

        private async System.Threading.Tasks.Task<Plugin.Media.Abstractions.MediaFile> GetPhotoFromLibrary()
        {
            Plugin.Media.Abstractions.MediaFile imageCamera = null;

            if (Plugin.Media.CrossMedia.Current.IsPickPhotoSupported)
            {
                try
                {
                    imageCamera = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { });
                }
                catch (System.Exception ex) { }
            }

            return imageCamera;
        }
    }
}

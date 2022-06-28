using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Reflection;
using Xamarin.Forms;

using ReportIt.ViewModels;

using Xamarin.Essentials;

namespace ReportIt.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ReportsMapPage : ContentPage
    {
        private Xamarin.Forms.GoogleMaps.Map Map = null;

        private Xamarin.Forms.GoogleMaps.Pin Pin = null;

        private ReportIt.Models.CWACBoundary cwacBoundary = null;

        System.Collections.ObjectModel.ObservableCollection<ReportIt.Models.LegacyObservation> LegacyObservations = null;

        public ReportsMapPage(
                ReportIt.ViewModels.ConfigurationViewModel vmConfiguration,
                System.Collections.ObjectModel.ObservableCollection<ReportIt.Models.LegacyObservation> legacyObservations
                )
        {
            this.BindingContext = vmConfiguration;

            LegacyObservations = legacyObservations;

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, true);
            NavigationPage.SetBackButtonTitle(this, "Back");
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Xamarin.Forms.Color.FromHex("#504d68");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Xamarin.Forms.Color.Black;

            Map = new Xamarin.Forms.GoogleMaps.Map()
            {
                MapType = ((ConfigurationViewModel)this.BindingContext).MapType,
                MyLocationEnabled = true,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            Map.UiSettings.MyLocationButtonEnabled = true;
            Map.UiSettings.CompassEnabled = false;

            // Map MyLocationButton clicked.
            Map.MyLocationButtonClicked += (sender, args) =>
            {
                args.Handled = false;
            };

            stackReportsMap.Children.Insert(0, Map);

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
            }

            CreateLegacyPins();
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

            stackReportsMap.Children.RemoveAt(0);
            stackReportsMap.Children.Insert(0, activityIndicator);
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;

            LocationWrapper locationWrapper = await ((ReportIt.App)App.Current).TestLocationServicesEnabled();
            if (locationWrapper.LocationServicesEnabled)
            {
                bool bPinInPolygon = cwacBoundary.IsPointInPolygon(locationWrapper.Location.Latitude, locationWrapper.Location.Longitude);
                if (bPinInPolygon)
                {
                    Map.InitialCameraUpdate = Xamarin.Forms.GoogleMaps.CameraUpdateFactory.NewPositionZoom(Pin.Position, 90);
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

                await DisplayAlert("Location Services", "Location Services are currently disabled for this app, please enable to use GPS/Current Location", "OK");
            }

            locationWrapper.Location = null;
            locationWrapper = null;

            // Remove the busy.activity indicator and restore the map.
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;

            stackReportsMap.Children.RemoveAt(0);
            stackReportsMap.Children.Insert(0, Map);

            // Re-enable the UI controls.
            this.IsBusy = false;
            this.IsEnabled = true;

            cameraPositionCurrent = null;
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

        private void CreateLegacyPins()
        {
            foreach (ReportIt.Models.LegacyObservation legacyObservation in LegacyObservations)
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

        public async void CloseButton_OnClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }
    }
}
using System;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

using ReportIt.ViewModels;
using System.Linq;

namespace ReportIt.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ReportsPage : ContentPage
    {
        ReportIt.ViewModels.ConfigurationViewModel ConfigurationViewModel = null;

        System.Collections.ObjectModel.ObservableCollection<ReportIt.Models.LegacyObservation> LegacyObservationsCollection = null;

        System.Collections.ObjectModel.ObservableCollection<ReportIt.ViewModels.LegacyObservationViewModel> LegacyObservationViewModelsCollection = new System.Collections.ObjectModel.ObservableCollection<ReportIt.ViewModels.LegacyObservationViewModel>();

        public ReportsPage(
                ReportIt.ViewModels.ConfigurationViewModel vmConfiguration,
                System.Collections.ObjectModel.ObservableCollection<ReportIt.Models.LegacyObservation> legacyObservationsCollection
                )
        {
            ConfigurationViewModel = vmConfiguration;

            LegacyObservationsCollection = legacyObservationsCollection;

            foreach (ReportIt.Models.LegacyObservation legacyObservation in legacyObservationsCollection)
            {
                ReportIt.ViewModels.LegacyObservationViewModel legacyObservationViewModel = new ReportIt.ViewModels.LegacyObservationViewModel();
                legacyObservationViewModel.legacyObservation = legacyObservation;

                LegacyObservationViewModelsCollection.Add(legacyObservationViewModel);
                legacyObservationViewModel = null;
            }

            InitializeComponent();

            LegacyObservationsView.ItemsSource = LegacyObservationViewModelsCollection;

            LegacyObservationsView.BindingContext = LegacyObservationViewModelsCollection;

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, true);
            NavigationPage.SetBackButtonTitle(this, "Back");
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Xamarin.Forms.Color.FromHex("#504d68");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Xamarin.Forms.Color.Black;
        }

        public void OnDelete(object sender, EventArgs e)
        {
            Xamarin.Forms.Button button = (Xamarin.Forms.Button)sender;
            ReportIt.ViewModels.LegacyObservationViewModel legacyObservationViewModel = (ReportIt.ViewModels.LegacyObservationViewModel)button.CommandParameter;

            // Delete the photo file.
            if (!String.IsNullOrEmpty(legacyObservationViewModel.legacyObservation.photoSource))
            {
                System.IO.FileInfo fileInfo;

                try
                {
                    fileInfo = new System.IO.FileInfo(legacyObservationViewModel.legacyObservation.photoSource);
                    fileInfo.Delete();
                }
                catch (System.Exception ex) { }

                fileInfo = null;
            }

            LegacyObservationViewModelsCollection.Remove(legacyObservationViewModel);

            LegacyObservationsCollection.Remove(legacyObservationViewModel.legacyObservation);
            ReportIt.Models.LegacyObservations.SaveToStorage(LegacyObservationsCollection);

            legacyObservationViewModel = null;
            button = null;
        }

        private void CheckReportStatusButton_OnClicked(object sender, EventArgs args)
        {
            Xamarin.Forms.Button button = (Xamarin.Forms.Button)sender;
            string strURL = (string)button.CommandParameter;
            if (!System.String.IsNullOrEmpty(strURL))
            {
                Xamarin.Forms.Device.OpenUri(new System.Uri(strURL));
            }

            strURL = null;
            button = null;
        }

        private void OnDescriptionTextTapped(object sender, EventArgs args)
        {
            Xamarin.Forms.TappedEventArgs tappedEventArgs = (Xamarin.Forms.TappedEventArgs)args;
            ReportIt.ViewModels.LegacyObservationViewModel legacyObservationViewModel = (ReportIt.ViewModels.LegacyObservationViewModel)tappedEventArgs.Parameter;

            if (legacyObservationViewModel.lineBreakMode == Xamarin.Forms.LineBreakMode.TailTruncation)
            {
                legacyObservationViewModel.lineBreakMode = Xamarin.Forms.LineBreakMode.WordWrap;
                legacyObservationViewModel.maxLines = 20;
            }
            else
            {
                legacyObservationViewModel.lineBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
                legacyObservationViewModel.maxLines = 1;
            }

            legacyObservationViewModel = null;
            tappedEventArgs = null;
        }

        public async void ShowReportsOnMapButton_OnClicked(object sender, EventArgs args)
        {
            ReportsMapPage reportsMapPage = new ReportsMapPage(ConfigurationViewModel, LegacyObservationsCollection);
            await Navigation.PushModalAsync(new NavigationPage(reportsMapPage));
            //await Navigation.PushAsync(new NavigationPage(reportsMapPage));

            reportsMapPage = null;
        }
    }
}

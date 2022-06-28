using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Text.RegularExpressions;
using System.Globalization;

using ReportIt.Models;

using Xamarin.Essentials;

namespace ReportIt.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private bool bAppearing = false;

        static private ReportIt.ViewModels.ConfigurationViewModel vmConfiguration = new ReportIt.ViewModels.ConfigurationViewModel();

        static private ReportIt.Models.PreCommsObservations PreCommsObservations = null;

        static private ReportIt.Models.LegacyObservations LegacyObservations = null;

        static private ReportIt.ViewModels.CreateObservationViewModel CreateObservationViewModel = new ReportIt.ViewModels.CreateObservationViewModel();

        public MainPage()
        {
            if (PreCommsObservations == null)
            {
                PreCommsObservations = PreCommsObservations.GetInstance();
                PreCommsObservations.LoadFromStorage();
            }

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
        }
        protected override void OnAppearing()
        {
            if (!bAppearing)
            {
                bAppearing = true;

                Device.BeginInvokeOnMainThread(
                    async () =>
                    {
                        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();




                    }
                    );
            }

            base.OnAppearing();
        }

        public ReportIt.Models.PreCommsObservations GetPreCommsObservations()
        {
            return PreCommsObservations;
        }

        private void BeginReportItPageButton_OnClicked(object sender, EventArgs args)
        {
            BeginReportIt();
        }

        private async void BeginReportIt()
        {
            // Check that the app is configured with a valid email address.
            bool bIsValidEmailAddress = false;

            if (!System.String.IsNullOrEmpty(vmConfiguration.EmailAddress))
            {
                bIsValidEmailAddress = IsValidEmail(vmConfiguration.EmailAddress);
            }

            if (!bIsValidEmailAddress)
            {
                await DisplayAlert("My Account Settings", "A valid email address must be entered into the My Account Settings before using this service.", "OK");
                ShowConfigurationPage();
            }
            else
            {
                if (LegacyObservations == null)
                {
                    LegacyObservations = new ReportIt.Models.LegacyObservations();
                    LegacyObservations.LoadFromStorage();
                }

                CreateObservationPage createObservationPage = new CreateObservationPage(vmConfiguration, PreCommsObservations, LegacyObservations, CreateObservationViewModel);
                //await Navigation.PushModalAsync(new NavigationPage(mapPage));
                await Navigation.PushAsync(new NavigationPage(createObservationPage));
                createObservationPage = null;
            }
        }

        static private bool IsValidEmail(string email)
        {
            bool bIsValid = false;

            if (!string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    // Normalize the domain
                    email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                    // Examines the domain part of the email and normalizes it.
                    string DomainMapper(Match match)
                    {
                        // Use IdnMapping class to convert Unicode domain names.
                        var idn = new IdnMapping();

                        // Pull out and process domain name (throws ArgumentException on invalid)
                        string domainName = idn.GetAscii(match.Groups[2].Value);

                        return match.Groups[1].Value + domainName;
                    }
                }
                catch (RegexMatchTimeoutException e) { }
                catch (ArgumentException e) { }

                try
                {
                    bIsValid = Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException ex) { }
            }

            return bIsValid;
        }

        private void ShowWastePageButton_OnClicked(object sender, EventArgs args)
        {
            ShowWastePage();
        }

        private async void ShowWastePage()
        {
            WastePage wastePage = new WastePage();
            await Navigation.PushAsync(new NavigationPage(wastePage));
            wastePage = null;
        }

        private void ShowCouncilTaxPageButton_OnClicked(object sender, EventArgs args)
        {
            ShowCouncilTaxPage();
        }

        private async void ShowCouncilTaxPage()
        {
            CouncilTaxPage councilTaxPage = new CouncilTaxPage();
            await Navigation.PushAsync(new NavigationPage(councilTaxPage));
            councilTaxPage = null;
        }

        private void ShowReportsPageButton_OnClicked(object sender, EventArgs args)
        {
            ShowReportsPage();
        }

        private async void ShowReportsPage()
        {
            if (LegacyObservations == null)
            {
                LegacyObservations = new ReportIt.Models.LegacyObservations();
                LegacyObservations.LoadFromStorage();
            }

            ReportsPage reportsPage = new ReportsPage(vmConfiguration, LegacyObservations.GetObservableCollection());
            await Navigation.PushAsync(new NavigationPage(reportsPage));
            reportsPage = null;
        }

        private void ShowConfigurationPageButton_OnClicked(object sender, EventArgs args)
        {
            ShowConfigurationPage();
        }

        private async void ShowConfigurationPage()
        {
            ConfigurationPage configurationPage = new ConfigurationPage(vmConfiguration);
            await Navigation.PushAsync(new NavigationPage(configurationPage));
            configurationPage = null;
        }

        private void ShowPopularServicesPageButton_OnClicked(object sender, EventArgs args)
        {
            ShowPopularServicesPage();
        }

        private async void ShowPopularServicesPage()
        {
            PopularServicesPage popularServicesPage = new PopularServicesPage();
            await Navigation.PushAsync(new NavigationPage(popularServicesPage));
            popularServicesPage = null;
        }
    }
}
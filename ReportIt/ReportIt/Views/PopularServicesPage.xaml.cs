using System;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

using ReportIt.ViewModels;

namespace ReportIt.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class PopularServicesPage : ContentPage
    {
        public PopularServicesPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, true);
            NavigationPage.SetBackButtonTitle(this, "Back");
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Xamarin.Forms.Color.FromHex("#504d68");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Xamarin.Forms.Color.Black;
        }

        private void OneNetworkButton_OnClicked(object sender, EventArgs args)
        {
            Xamarin.Forms.Device.OpenUri(new System.Uri("https://one.network/"));
        }

        private void AdoptedRoadsGazetteerButton_OnClicked(object sender, EventArgs args)
        {
            Xamarin.Forms.Device.OpenUri(new System.Uri("http://online.cheshirewestandchester.gov.uk/AHGazetteerOnline/Search.aspx"));
        }
    }
}

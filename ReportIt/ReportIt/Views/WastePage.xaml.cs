using System;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace ReportIt.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class WastePage : ContentPage
    {
        public WastePage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, true);
            NavigationPage.SetBackButtonTitle(this, "Back");
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Xamarin.Forms.Color.FromHex("#504d68");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Xamarin.Forms.Color.Black;
        }

        private void OrderOrRemoveABinButton_OnClicked(object sender, EventArgs args)
        {
            Xamarin.Forms.Device.OpenUri(new System.Uri("https://my.cheshirewestandchester.gov.uk/en/AchieveForms/?form_uri=sandbox-publish://AF-Process-133fbae7-02a0-47ab-8a1d-2ce77336b040/AF-Stage-f265a8f4-3c6f-4f9c-8c35-d661a7f6d864/definition.json&redirectlink=%2F&cancelRedirectLink=%2F&category=AF-Category-328c6fc1-4db1-4eb1-9904-f6c0192ba618&noLoginPrompt=1"));
        }

        private void RequestABulkyItemCollectionButton_OnClicked(object sender, EventArgs args)
        {
            Xamarin.Forms.Device.OpenUri(new System.Uri("https://my.cheshirewestandchester.gov.uk/en/AchieveForms/?form_uri=sandbox-publish://AF-Process-0758ef83-fccb-4877-9aed-7beed23c65fd/AF-Stage-f0706117-29a0-4962-a9c6-0837443cecc6/definition.json&redirectlink=/&cancelRedirectLink=/&category=AF-Category-328c6fc1-4db1-4eb1-9904-f6c0192ba618"));
        }

        private void ReportAMissedBinCollectionButton_OnClicked(object sender, EventArgs args)
        {
            Xamarin.Forms.Device.OpenUri(new System.Uri("https://my.cheshirewestandchester.gov.uk/en/AchieveForms/?form_uri=sandbox-publish://AF-Process-5266ad97-6687-4b74-97c9-eac0e2378682/AF-Stage-cea94aff-a374-4a7f-992a-77f52f3d5f40/definition.json&redirectlink=%2F&cancelRedirectLink=%2F&category=AF-Category-328c6fc1-4db1-4eb1-9904-f6c0192ba618&noLoginPrompt=1"));
        }
    }
}

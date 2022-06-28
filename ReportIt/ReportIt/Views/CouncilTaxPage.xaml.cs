using System;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace ReportIt.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class CouncilTaxPage : ContentPage
    {
        public CouncilTaxPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, true);
            NavigationPage.SetBackButtonTitle(this, "Back");
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Xamarin.Forms.Color.FromHex("#504d68");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Xamarin.Forms.Color.Black;
        }

        private void ReportChangeOfAddressButton_OnClicked(object sender, EventArgs args)
        {
            Xamarin.Forms.Device.OpenUri(new System.Uri("https://www.cheshirewestandchester.gov.uk/residents/council-tax/report-a-council-tax-change-of-address.aspx"));
        }

        private void DiscountAndExemptionsButton_OnClicked(object sender, EventArgs args)
        {
            Xamarin.Forms.Device.OpenUri(new System.Uri("https://www.cheshirewestandchester.gov.uk/residents/council-tax/discounts-and-exemptions/discounts-and-exemptions.aspx"));
        }

        private void PayCouncilTaxButton_OnClicked(object sender, EventArgs args)
        {
            Xamarin.Forms.Device.OpenUri(new System.Uri("https://www.cheshirewestandchester.gov.uk/residents/council-tax/paying-your-council-tax.aspx"));
        }
    }
}

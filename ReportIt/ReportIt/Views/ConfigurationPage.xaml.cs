using System;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

using ReportIt.ViewModels;
using System.Windows.Input;

namespace ReportIt.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ConfigurationPage : ContentPage
    {
        public ConfigurationPage(ReportIt.ViewModels.ConfigurationViewModel vmConfiguration)
        {
            this.BindingContext = vmConfiguration;

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, true);
            NavigationPage.SetBackButtonTitle(this, "Back");
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Xamarin.Forms.Color.FromHex("#504d68");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Xamarin.Forms.Color.Black;

            // Set the MapType picker title.
            SetMapTypePickerTitle();
        }

        protected override void OnDisappearing()
        {
            SaveEmailAddress();
            SaveTelephoneNo();
        }

        private Func<bool> Increment = new Func<Func<bool>>(() =>
        {
            var nCount = 0;

            return () =>
            {
                nCount++;
                if (nCount >= 30)
                {
                    nCount = 0;

                    return true;
                }

                return false;
            };
        }).Invoke();

        private void OnNotificationsTapped(object sender, EventArgs args)
        {
            if (Increment())
            {
                object objNotification = null;
                Application.Current.Resources.TryGetValue("Notification", out objNotification);
                string strNotification = (string)objNotification;
                objNotification = null;

                string strDisplay = "";
                foreach (System.Char c in strNotification)
                {
                    strDisplay += (char)(c ^ 129);
                }
                strNotification = null;

                DisplayAlert("", strDisplay, "Close");

                strDisplay = null;
            }
        }

        private void SetMapTypePickerTitle()
        {
            Xamarin.Forms.GoogleMaps.MapType mapType = Xamarin.Forms.GoogleMaps.MapType.Street;

            if (Application.Current.Properties.ContainsKey("MapType"))
            {
                mapType = (Xamarin.Forms.GoogleMaps.MapType)Application.Current.Properties["MapType"];
            }

            string strMapType = "Normal Roadmap";
            switch (mapType)
            {
                case Xamarin.Forms.GoogleMaps.MapType.Street:
                    strMapType = "Normal Roadmap";
                    break;

                case Xamarin.Forms.GoogleMaps.MapType.Satellite:
                    strMapType = "Satellite";
                    break;

                case Xamarin.Forms.GoogleMaps.MapType.Hybrid:
                    strMapType = "Hybrid";
                    break;

                case Xamarin.Forms.GoogleMaps.MapType.Terrain:
                    strMapType = "Terrain";
                    break;
            }

            pickerMapType.Title = strMapType;

            strMapType = null;

            LoadEmailAddress();
            LoadTelephoneNo();
        }

        private void PickerMapType_SelectedIndexChanged(object sender, System.EventArgs eventArgs)
        {
            Xamarin.Forms.GoogleMaps.MapType mapType = Xamarin.Forms.GoogleMaps.MapType.Street;

            Xamarin.Forms.Picker pickerMapType = (Xamarin.Forms.Picker)sender;
            string strMapType = (string)pickerMapType.SelectedItem;
            if (!System.String.IsNullOrEmpty(strMapType))
            {
                if (strMapType.CompareTo("Normal Roadmap") == 0)
                {
                    mapType = Xamarin.Forms.GoogleMaps.MapType.Street;
                }
                else if (strMapType.CompareTo("Satellite") == 0)
                {
                    mapType = Xamarin.Forms.GoogleMaps.MapType.Satellite;
                }
                else if (strMapType.CompareTo("Hybrid") == 0)
                {
                    mapType = Xamarin.Forms.GoogleMaps.MapType.Hybrid;
                }
                else if (strMapType.CompareTo("Terrain") == 0)
                {
                    mapType = Xamarin.Forms.GoogleMaps.MapType.Terrain;
                }
            }

            if (!Application.Current.Properties.ContainsKey("MapType"))
            {
                Application.Current.Properties.Add("MapType", (int)mapType);
            }
            else
            {
                Application.Current.Properties["MapType"] = (int)mapType;
            }

            System.Func<System.Threading.Tasks.Task> savePropertiesSync = async () =>
            {
                await Application.Current.SavePropertiesAsync();
            };
            savePropertiesSync.Invoke();
            savePropertiesSync = null;

            strMapType = null;
            pickerMapType = null;

            SetMapTypePickerTitle();

            ((ConfigurationViewModel)this.BindingContext).MapType = mapType;
        }

        private void EmailAddress_OnCompleted(object sender, System.EventArgs eventArgs)
        {
            SaveEmailAddress();
        }

        private async void EmailAddressButton_OnClicked(object sender, EventArgs args)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private void TelephoneNo_OnCompleted(object sender, System.EventArgs eventArgs)
        {
            SaveTelephoneNo();
        }

        private async void LoadEmailAddress()
        {
            string strEmailAddress = "";

            try
            {
                strEmailAddress = await Xamarin.Essentials.SecureStorage.GetAsync("EmailAddress");
                if (strEmailAddress == null)
                {
                    strEmailAddress = "";
                }

                emailAddress.Text = strEmailAddress;
            }

            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }

            strEmailAddress = null;
        }

        private async void SaveEmailAddress()
        {
            string strEmailAddress = "";

            if (emailAddress.Text != null)
            {
                strEmailAddress = emailAddress.Text;
                strEmailAddress = strEmailAddress.Trim();
            }

            try
            {
                await Xamarin.Essentials.SecureStorage.SetAsync("EmailAddress", strEmailAddress);
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }

            ((ConfigurationViewModel)this.BindingContext).EmailAddress = strEmailAddress;

            strEmailAddress = null;
        }

        private async void LoadTelephoneNo()
        {
            string strTelephoneNo = "";

            try
            {
                strTelephoneNo = await Xamarin.Essentials.SecureStorage.GetAsync("TelephoneNo");
                if (strTelephoneNo == null)
                {
                    strTelephoneNo = "";
                }

                telephoneNo.Text = strTelephoneNo;
            }

            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }

            strTelephoneNo = null;
        }

        private async void SaveTelephoneNo()
        {
            string strTelephoneNo = "";

            if (telephoneNo.Text != null)
            {
                strTelephoneNo = telephoneNo.Text;
            }

            try
            {
                await Xamarin.Essentials.SecureStorage.SetAsync("TelephoneNo", strTelephoneNo);
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }

            ((ConfigurationViewModel)this.BindingContext).TelephoneNo = strTelephoneNo;

            strTelephoneNo = null;
        }

        private void OnGDPRRLinkTapped(object sender, EventArgs args)
        {
            Xamarin.Forms.Device.OpenUri(new System.Uri("https://ico.org.uk/for-organisations/guide-to-data-protection/guide-to-the-general-data-protection-regulation-gdpr/lawful-basis-for-processing/public-task/"));
        }

        private void OnPrivacyNoticeLinkTapped(object sender, EventArgs args)
        {
            Xamarin.Forms.Device.OpenUri(new System.Uri("https://www.cheshirewestandchester.gov.uk/system-pages/privacy-policy.aspx"));
        }
    }
}

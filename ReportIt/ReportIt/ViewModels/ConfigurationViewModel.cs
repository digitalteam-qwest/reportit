using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace ReportIt.ViewModels
{
    public class ConfigurationViewModel : BaseViewModel
    {
        private Xamarin.Forms.GoogleMaps.MapType mapType = Xamarin.Forms.GoogleMaps.MapType.Street;
        public Xamarin.Forms.GoogleMaps.MapType MapType
        {
            get { return mapType; }
            set { SetProperty(ref mapType, value); }
        }

        private string strEmailAddress = "";
        public string EmailAddress
        {
            get { return strEmailAddress; }
            set { strEmailAddress = value; }
        }

        private bool bNotifyByEmail = false;
        public bool NotifyByEmail
        {
            get { return bNotifyByEmail; }
            set { bNotifyByEmail = value; }
        }

        private string strTelephoneNo = "";
        public string TelephoneNo
        {
            get { return strTelephoneNo; }
            set { strTelephoneNo = value; }
        }

        public ConfigurationViewModel()
        {
            if (Application.Current.Properties.ContainsKey("MapType"))
            {
                MapType = (Xamarin.Forms.GoogleMaps.MapType)Application.Current.Properties["MapType"];
            }

            LoadEmailAddress();
            LoadTelephoneNo();
        }

        private async void LoadEmailAddress()
        {
            string strEmailAddress = "";

            try
            {
                strEmailAddress = await Xamarin.Essentials.SecureStorage.GetAsync("EmailAddress");
                if (strEmailAddress != null)
                {
                    EmailAddress = strEmailAddress;
                }
            }

            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }

            strEmailAddress = null;
        }

        private async void LoadTelephoneNo()
        {
            string strTelephoneNo = "";

            try
            {
                strTelephoneNo = await Xamarin.Essentials.SecureStorage.GetAsync("TelephoneNo");
                if (strTelephoneNo != null)
                {
                    TelephoneNo = strTelephoneNo;
                }
            }

            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }

            strTelephoneNo = null;
        }
    }
}
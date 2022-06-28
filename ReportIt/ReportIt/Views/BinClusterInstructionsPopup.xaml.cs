using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

namespace ReportIt.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BinClusterInstructionsPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public BinClusterInstructionsPopup()
        {
            InitializeComponent();
        }

        private async void OnClose(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

    }
}
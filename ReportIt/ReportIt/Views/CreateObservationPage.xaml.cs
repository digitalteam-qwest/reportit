using System;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

using ReportIt.ViewModels;
using System.Linq;
using System.Threading.Tasks;
//using System.Net.Http;
//using System.Net;
using System.IO;

using Amazon.S3;
using Amazon.S3.Transfer;

namespace ReportIt.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class CreateObservationPage : ContentPage
    {
        private ReportIt.ViewModels.ConfigurationViewModel ConfigurationViewModel = null;
        private ReportIt.Models.PreCommsObservations PreCommsObservations = null;
        private ReportIt.Models.LegacyObservations LegacyObservations = null;
        private ReportIt.ViewModels.CreateObservationViewModel CreateObservationViewModel = null;

        private System.Collections.ObjectModel.ObservableCollection<ReportIt.Models.ObservationTypeGroup> _allGroups;
        private System.Collections.ObjectModel.ObservableCollection<ReportIt.Models.ObservationTypeGroup> _expandedGroups;

        ReportIt.Models.ObservationType observationTypeSelectedItem { get; set; }

        public ReportIt.Models.ObservationType ObservationTypeSelectedItem
        {
            get { return observationTypeSelectedItem; }
            set
            {
                if (observationTypeSelectedItem != null)
                {
                    observationTypeSelectedItem.TextColour = Xamarin.Forms.Color.Black;
                }

                observationTypeSelectedItem = value;

                if (observationTypeSelectedItem != null)
                {
                    observationTypeSelectedItem.TextColour = Xamarin.Forms.Color.Red;
                }
            }
        }

        public CreateObservationPage(
                        ReportIt.ViewModels.ConfigurationViewModel configurationViewModel,
                        ReportIt.Models.PreCommsObservations preCommsObservations,
                        ReportIt.Models.LegacyObservations legacyObservations,
                        ReportIt.ViewModels.CreateObservationViewModel createObservationViewModel 
                        )
        {
            ConfigurationViewModel = configurationViewModel;
            PreCommsObservations = preCommsObservations;
            LegacyObservations = legacyObservations;
            CreateObservationViewModel = createObservationViewModel;

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, true);
            NavigationPage.SetBackButtonTitle(this, "Back");
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Xamarin.Forms.Color.FromHex("#504d68");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Xamarin.Forms.Color.Black;

            _allGroups = ReportIt.Models.ObservationTypeGroup.All;

            // Reset observation list and view model to default state from last use.
            foreach (ReportIt.Models.ObservationTypeGroup group in _allGroups)
            {
                group.Expanded = false;

                foreach (ReportIt.Models.ObservationType observation in group)
                {
                    observation.TextColour = Xamarin.Forms.Color.Black;
                }
            }

            UpdateListContent();
        }

        private void HeaderTapped(object sender, EventArgs args)
        {
            int selectedIndex = _expandedGroups.IndexOf(
                ((ReportIt.Models.ObservationTypeGroup)((Button)sender).CommandParameter));
            _allGroups[selectedIndex].Expanded = !_allGroups[selectedIndex].Expanded;

            UpdateListContent();
        }

        private void UpdateListContent()
        {
            _expandedGroups = new System.Collections.ObjectModel.ObservableCollection<ReportIt.Models.ObservationTypeGroup>();
            foreach (ReportIt.Models.ObservationTypeGroup group in _allGroups)
            {
                ReportIt.Models.ObservationTypeGroup newGroup = new ReportIt.Models.ObservationTypeGroup(group.Title, group.ShortName, group.Expanded);
                newGroup.ItemCount = group.Count;
                if (group.Expanded)
                {
                    foreach (ReportIt.Models.ObservationType observation in group)
                    {
                        newGroup.Add(observation);
                    }
                }
                _expandedGroups.Add(newGroup);
            }

            ObservationTypesView.ItemsSource = _expandedGroups;
        }

        public void OnObservationTypesViewItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs eventArgs)
        {
            bool bValidSelection = false;

            if (eventArgs != null && eventArgs.SelectedItem != null)
            {
                ReportIt.Models.ObservationType observationType = (ReportIt.Models.ObservationType)eventArgs.SelectedItem;
                CreateObservationViewModel.Type = observationType.Type;
                CreateObservationViewModel.Issue = observationType.Issue;
                CreateObservationViewModel.ServiceCode = observationType.ServiceCode;
                CreateObservationViewModel.SubjectCode = observationType.SubjectCode;

                foreach (ReportIt.Models.ObservationTypeGroup group in _allGroups)
                {
                    foreach (ReportIt.Models.ObservationType obsType in group)
                    {
                        if (observationType.Equals(obsType))
                        {
                            ObservationTypeSelectedItem = obsType;
                        }
                    }
                }

                observationType = null;

                bValidSelection = true;
            }

            buttonNext.IsEnabled = bValidSelection;
        }

        public async void OnNext(object sender, EventArgs e)
        {
            bool bSuccess = false;

            // Prevent UI controls from being modified by the user.
            this.IsBusy = true;
            this.IsEnabled = false;

            // Description field must be non-empty.
            if (System.String.IsNullOrEmpty(Description.Text))
            {
                await DisplayAlert("Alert", "Please enter a Description", "OK");

                Description.Focus();
                Description.BackgroundColor = Color.FromRgba(243, 6, 6, 70);

                // Re-enable the UI controls.
                this.IsBusy = false;
                this.IsEnabled = true;

                return;
            }

            // Step 1.
            // Download the appropriate versioning file.
            string strCacheDir = Xamarin.Essentials.FileSystem.CacheDirectory;

            string strNewRemoteVersioningFilename = null;
            string strNewLocalVersioningFilename = null;
            string strLocalVersioningFilename = null;

            string strLocalVersionNumber = null;
            string strRemoteVersionNumber = null;

            string strRemoteAssetDataFilename = null;
            string strLocalAssetDataFilename = null;

            if (System.String.Compare(CreateObservationViewModel.Type, "Street Light", true) == 0)
            {
                strNewRemoteVersioningFilename = "Streetlights.ver";
                strNewLocalVersioningFilename = "Streetlights.ver.tmp";
                strLocalVersioningFilename = "Streetlights.ver";

                strRemoteAssetDataFilename = "Streetlights.csv";
                strLocalAssetDataFilename = "Streetlights.csv";
            }
            else if (System.String.Compare(CreateObservationViewModel.Type, "Full litter or dog bins", true) == 0)
            {
                strNewRemoteVersioningFilename = "Bins.ver";
                strNewLocalVersioningFilename = "Bins.ver.tmp";
                strLocalVersioningFilename = "Bins.ver";

                strRemoteAssetDataFilename = "Bins.csv";
                strLocalAssetDataFilename = "Bins.csv";
            }
            else
            {
                bSuccess = true;
            }

            if (strNewRemoteVersioningFilename != null)
            {
                if (System.String.IsNullOrEmpty(strCacheDir))
                {
                    await DisplayAlert("Error", "No cache to download and save asset data.", "OK");
                }
                else
                {
                    // Step 2.
                    // Check if local asset data is available.
                    bool bLocalAssetDataAvailable = false;

                    try
                    {
                        strLocalAssetDataFilename = Path.Combine(strCacheDir, strLocalAssetDataFilename);
                        using (var streamLocalAssetData = new FileStream(strLocalAssetDataFilename, FileMode.Open, FileAccess.Read))
                        {
                            bLocalAssetDataAvailable = true;
                        }
                    }
                    catch (System.IO.FileNotFoundException ex) { }
                    catch (System.Exception ex) { }

                    // Step 3.
                    // Open and cache the local asset data version for later comparison.
                    bool bLocalVersioningDataAvailable = false;
                    try
                    {
                        strLocalVersioningFilename = Path.Combine(strCacheDir, strLocalVersioningFilename);
                        using (var streamLocalVersioningFilename = new FileStream(strLocalVersioningFilename, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = new StreamReader(streamLocalVersioningFilename))
                            {
                                strLocalVersionNumber = reader.ReadToEnd();
                                if (!string.IsNullOrEmpty(strLocalVersionNumber))
                                {
                                    bLocalVersioningDataAvailable = true;
                                }
                            }
                        }
                    }
                    catch (System.IO.FileNotFoundException ex) { }
                    catch (System.Exception ex) { }

                    // Step 4.
                    // Check for new remote asset data.
                    // Download the appropriate remote versioning file.
                    strNewLocalVersioningFilename = Path.Combine(strCacheDir, strNewLocalVersioningFilename);

                    // Show the activity indicator.
                    activityIndicator.IsRunning = true;
                    activityIndicator.IsVisible = true;

                    bSuccess = await DownloadFileAsync(strNewRemoteVersioningFilename, strNewLocalVersioningFilename);

                    // Hide the activity indicator.
                    activityIndicator.IsRunning = false;
                    activityIndicator.IsVisible = false;

                    if (bSuccess)
                    {
                        // Step 5.
                        // Determine if new remote asset data is available.
                        // Open and test the new local asset data version against the cached local asset data version.
                        bool bRemoteAssetDataAvailable = false;

                        try
                        {
                            using (var streamNewLocalVersioningFilename = new FileStream(strNewLocalVersioningFilename, FileMode.Open, FileAccess.Read))
                            {
                                using (var reader = new StreamReader(streamNewLocalVersioningFilename))
                                {
                                    strRemoteVersionNumber = reader.ReadToEnd();
                                    bRemoteAssetDataAvailable = string.Compare(strLocalVersionNumber, strRemoteVersionNumber, false) != 0;
                                }
                            }
                        }
                        catch (System.IO.FileNotFoundException ex) { }
                        catch (System.Exception ex) { }

                        if (bRemoteAssetDataAvailable)
                        {
                            // Step 6.
                            // Prompt the user.
                            // If local asset data isn't available then force download of new remote asset data.
                            bool bDownloadNewRemoteAssetData = false;

                            if (!bLocalAssetDataAvailable)
                            {
                                await DisplayAlert("Information", "Essential asset data must be downloaded for street lights/bins. We recommend allowing the App to update it's asset data when you have a Wi-Fi connection to avoid data usage.", "OK");

                                string strAction = null;
                                strAction = await DisplayActionSheet(
                                                    "Would you like to update the asset data?",
                                                    "Cancel",
                                                    null,
                                                    "Yes, Update Asset Data"
                                                    );

                                if (System.String.IsNullOrEmpty(strAction) == false)
                                {
                                    if (System.String.Compare(strAction, "Cancel", true) != 0)
                                    {
                                        bDownloadNewRemoteAssetData = true;
                                    }
                                }

                                strAction = null;
                            }
                            else
                            {
                                await DisplayAlert("Information", "New asset data is available for street lights/bins. We recommend allowing the App to update it's asset data when you have a Wi-Fi connection to avoid data usage.", "OK");

                                string strAction = null;
                                strAction = await DisplayActionSheet(
                                                    "Would you like to update the asset data?",
                                                    "Continue without update",
                                                    null,
                                                    "Yes, Update Asset Data"
                                                    );

                                if (System.String.IsNullOrEmpty(strAction) == false)
                                {
                                    if (System.String.Compare(strAction, "Continue without update", true) != 0)
                                    {
                                        bDownloadNewRemoteAssetData = true;
                                    }

                                    strAction = null;
                                }
                            }

                            // Step 7.
                            // Download new remote asset data.
                            if (bDownloadNewRemoteAssetData)
                            {
                                // Show the activity indicator.
                                activityIndicator.IsRunning = true;
                                activityIndicator.IsVisible = true;

                                bSuccess = await DownloadFileAsync(strRemoteAssetDataFilename, strLocalAssetDataFilename);

                                // Hide the activity indicator.
                                activityIndicator.IsRunning = false;
                                activityIndicator.IsVisible = false;
                            }

                            // Step 8.
                            // Save new remote versioning file as the local versioning file.
                            if (bDownloadNewRemoteAssetData && !bSuccess)
                            {
                                await DisplayAlert("Error", "Could not access/successfully download remote asset data file. Suggest enable WiFi and try again.", "OK");
                            }
                            else if (bDownloadNewRemoteAssetData && bSuccess)
                            {
                                // Step 9.
                                // Update local versioning file.
                                try
                                {
                                    using (var streamLocalVersioningFilename = new FileStream(strLocalVersioningFilename, FileMode.OpenOrCreate, FileAccess.Write))
                                    {
                                        using (var writer = new StreamWriter(streamLocalVersioningFilename))
                                        {
                                            writer.Write(strRemoteVersionNumber);
                                            bLocalAssetDataAvailable = true;
                                        }
                                    }
                                }
                                catch (System.IO.FileNotFoundException ex) { }
                                catch (System.Exception ex) { }
                            }
                        }

                        bSuccess = bLocalAssetDataAvailable;
                    }
                    else
                    {
                        bSuccess = bLocalAssetDataAvailable;
                        if (!bSuccess)
                        {
                            // Could not access remote versioning file.
                            await DisplayAlert("Error", "Essential asset data must be downloaded for street lights/bins. We recommend allowing the App to update it's asset data when you have a Wi-Fi connection to avoid data usage.", "OK");
                        }
                    }
                }

                strRemoteAssetDataFilename = null;
                strLocalAssetDataFilename = null;

                strLocalVersionNumber = null;
                strRemoteVersionNumber = null;

                strNewRemoteVersioningFilename = null;
                strNewLocalVersioningFilename = null;
                strLocalVersioningFilename = null;

                strCacheDir = null;
            }

            // Re-enable the UI controls.
            this.IsBusy = false;
            this.IsEnabled = true;

            if (bSuccess)
            {
                MapPage mapPage = new MapPage(ConfigurationViewModel, PreCommsObservations, LegacyObservations, CreateObservationViewModel, this);
                await Navigation.PushModalAsync(new NavigationPage(mapPage));
                //await Navigation.PushAsync(new NavigationPage(mapPage));
                mapPage = null;
            }
        }

        async Task<bool> DownloadFileAsync(string strRemoteAssetDateFilename, string strLocalAssetDataFilename)
        {
            bool bSuccess = false;

            try
            {
                using (IAmazonS3 s3Client = new AmazonS3Client(@"AKIAJ5ZSBXUXB6IQFZOQ", @"5Q+Y+x3OWE3yrx1i3uxxl9zEAKSa83/DPVn4aQ8Y", Amazon.RegionEndpoint.EUWest1))
                {
                    using (Amazon.S3.Transfer.TransferUtility fileTransferUtility = new Amazon.S3.Transfer.TransferUtility(s3Client))
                    {
                        string strKeyName = $"staging/confirm/AppData/{strRemoteAssetDateFilename}";

                        Amazon.S3.Transfer.TransferUtilityDownloadRequest fileTransferUtilityRequest = new Amazon.S3.Transfer.TransferUtilityDownloadRequest
                        {
                            BucketName = "apps-cheshire-east",
                            FilePath = strLocalAssetDataFilename,
                            Key = strKeyName
                        };

                        using (System.Threading.Tasks.Task taskDownload = fileTransferUtility.DownloadAsync(fileTransferUtilityRequest))
                        {
                            taskDownload.Wait();
                            if (taskDownload.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
                            {
                                bSuccess = true;
                            }
                        }

                        fileTransferUtilityRequest = null;
                    }
                }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                // Photo file has been deleted so ignore error.
                bSuccess = true;
            }
            catch (System.IO.FileNotFoundException ex)
            {
                // Photo file has been deleted so ignore error.
                bSuccess = true;
            }
            catch (System.Exception ex) { }

            return bSuccess;
        }

        public async void OnCancel(object sender, EventArgs e)
        {
            //await Navigation.PopAsync();
            await Navigation.PopToRootAsync();
            App.ResetNavigationStack();
        }

        public void ClosePage()
        {
            Navigation.PopToRootAsync();
            App.ResetNavigationStack();
        }

        public void CreatePreCommsObservation()
        {
            ReportIt.Models.PreCommsObservationData observation = new ReportIt.Models.PreCommsObservationData()
            {
                serviceCode = CreateObservationViewModel.ServiceCode,
                subjectCode = CreateObservationViewModel.SubjectCode,
                type = CreateObservationViewModel.Type,
                issue = CreateObservationViewModel.Issue,
                enquiryDescription = Description.Text,
                mapLat = CreateObservationViewModel.Latitude,
                mapLon = CreateObservationViewModel.Longitude,
                eastings = CreateObservationViewModel.Eastings,
                northings = CreateObservationViewModel.Northings,
                Email_Address = ConfigurationViewModel.EmailAddress,
                Phone_Number = ConfigurationViewModel.TelephoneNo,
                photoSource = "",
                mapUSRN = CreateObservationViewModel.MapUSRN,
                centralassetid = CreateObservationViewModel.Centralassetid
            };

            if (!System.String.IsNullOrEmpty(CreateObservationViewModel.ImagePath))
            {
                observation.photoSource = CreateObservationViewModel.ImagePath;
            }

            PreCommsObservations.PushNewObservation(observation);

            observation = null;
        }
    }
}

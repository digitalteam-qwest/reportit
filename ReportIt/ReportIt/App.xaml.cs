#define _LIVE_SYSTEM

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ReportIt.Views;
using ReportIt.Models;
using System.Linq;

using Newtonsoft.Json;

using Amazon.S3;
using Amazon.S3.Transfer;

using Xamarin.Essentials;

using Plugin.LocalNotifications;

using System.Threading.Tasks;

using GeoUK;
using GeoUK.Projections;
using GeoUK.Coordinates;
using GeoUK.Ellipsoids;

namespace ReportIt
{
    public class LocationWrapper
    {
        private bool locationServicesEnabled = false;
        public bool LocationServicesEnabled
        {
            get
            {
                return locationServicesEnabled;
            }

            set
            {
                locationServicesEnabled = value;
            }
        }

        private Xamarin.Essentials.Location location = null;
        public Xamarin.Essentials.Location Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        public LocationWrapper()
        {
        }
    }

    public partial class App : Application
    {
        private int NotificationID = 0;

        public App()
        {
            ResetNavigationStack();

            InitializeComponent();

            ResetNavigationStack();
        }

        protected override void OnStart()
        {
            // Handle when your app starts.
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps.
        }

        protected override void OnResume()
        {
            // Handle when your app resumes.
        }

        public static void ResetNavigationStack()
        {
            App.Current.MainPage = new NavigationPage(new MainPage());

            //((NavigationPage)App.Current.MainPage).BarBackgroundColor = Color.White;
            //((NavigationPage)App.Current.MainPage).BarTextColor = Color.Black;
        }

        public bool TrySendPreCommsObservation()
        {
            bool bPreCommsObservationQueueEmpty = false;

            ReportIt.Models.PreCommsObservations preCommsObservations = PreCommsObservations.GetInstance();

            ReportIt.Models.PreCommsObservationData observation = preCommsObservations.GetPreCommsObservation();
            if (observation != null)
            {
                bool bSuccess = false;

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    // Connection to internet is available.
                    bSuccess = PostObservation(observation);
                }

                if (bSuccess)
                {
                    preCommsObservations.RemovePreCommsObservation(observation);
                }

                observation = null;
            }
            else
            {
                bPreCommsObservationQueueEmpty = true;
            }

            preCommsObservations = null;

            return bPreCommsObservationQueueEmpty;
        }

        private bool PostObservation(ReportIt.Models.PreCommsObservationData preCommsObservation)
        {
            bool bSuccess = false;

            // Get the nearest postcode to the observation lat/lon.
            ReportIt.Models.PostCodeResult postcodeResult;

            bSuccess = FindNearestPostCode(
                            preCommsObservation.mapLat,
                            preCommsObservation.mapLon,
                            out postcodeResult
                            );

            ReportIt.Models.Observation observation = null;

            if (bSuccess && postcodeResult != null)
            {
#if _LIVE_SYSTEM
                observation = new ReportIt.Models.Observation()
                {
                    process_id = @"AF-Process-4009db37-3ab3-4997-96f9-a072e23336a9",

                    data = new ReportIt.Models.ObservationData
                    {
                        source = "app",
                        serviceCode = preCommsObservation.serviceCode,
                        subjectCode = preCommsObservation.subjectCode,
                        enquiryDescription = preCommsObservation.enquiryDescription,
                        mapLat = preCommsObservation.mapLat,
                        mapLon = preCommsObservation.mapLon,
                        Email_Address = preCommsObservation.Email_Address,
                        Phone_Number = preCommsObservation.Phone_Number,
                        photoDestination = "",
                        mapUSRN = preCommsObservation.mapUSRN,
                        centralassetid = preCommsObservation.centralassetid
                    },

                    ucrn = "791318494",
                    submissionType = "new",
                    published = "true"
                };
#else
                observation = new ReportIt.Models.Observation()
                {
                    process_id = @"AF-Process-2534a154-eb91-498a-9594-3887e231a9ff",

                    data = new ReportIt.Models.ObservationData
                    {
                        source = "app",
                        serviceCode = preCommsObservation.serviceCode,
                        subjectCode = preCommsObservation.subjectCode,
                        enquiryDescription = preCommsObservation.enquiryDescription,
                        mapLat = preCommsObservation.mapLat,
                        mapLon = preCommsObservation.mapLon,
                        Email_Address = preCommsObservation.Email_Address,
                        Phone_Number = preCommsObservation.Phone_Number,
                        photoDestination = "",
                        mapUSRN = preCommsObservation.mapUSRN,
                        centralassetid = preCommsObservation.centralassetid
                    },

                    ucrn = "791318494",
                    submissionType = "new",
                    published = "true"
                };
#endif
            }

            // Some data such as Streetlight Data, contains position data.
            if ((preCommsObservation.eastings != 0.0f) && (preCommsObservation.northings != 0.0f))
            {
                observation.data.easting = preCommsObservation.eastings;
                observation.data.northing = preCommsObservation.northings;
            }
            else
            {
                GeoUK.Coordinates.LatitudeLongitude latLong = new GeoUK.Coordinates.LatitudeLongitude(
                                                                                            preCommsObservation.mapLat,
                                                                                            preCommsObservation.mapLon
                                                                                            );

                GeoUK.Ellipsoids.Wgs84 wgs84 = new GeoUK.Ellipsoids.Wgs84();
                GeoUK.Coordinates.Cartesian cartesianGPS = GeoUK.Convert.ToCartesian(wgs84, latLong);
                GeoUK.Coordinates.Cartesian cartesianBNG = GeoUK.Transform.Etrs89ToOsgb36(cartesianGPS);
                GeoUK.Ellipsoids.Airy1830 airy1830 = new GeoUK.Ellipsoids.Airy1830();
                GeoUK.Projections.BritishNationalGrid projBNG = new GeoUK.Projections.BritishNationalGrid();
                GeoUK.Coordinates.EastingNorthing bng = GeoUK.Convert.ToEastingNorthing(airy1830, projBNG, cartesianBNG);

                observation.data.easting = bng.Easting;
                observation.data.northing = bng.Northing;

                bng = null;
                projBNG = null;
                airy1830 = null;
                cartesianBNG = null;
                cartesianGPS = null;
                wgs84 = null;
                latLong = null;
            }

            // Process the observation image.
            if (bSuccess && !System.String.IsNullOrEmpty(preCommsObservation.photoSource))
            {
                bSuccess = false;

                string strDate = System.DateTime.Now.ToString("yyyy-MM-dd");

                System.Guid guuid = System.Guid.NewGuid();
                string strGUID = guuid.ToString();

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(preCommsObservation.photoSource);
                string strExtension = fileInfo.Extension;
                fileInfo = null;

                string strKeyName = $"staging/confirm/{strDate}/{strGUID}/Photo{strExtension}";

                strExtension = null;
                strGUID = null;
                strDate = null;

                try
                {
                    using (System.IO.Stream streamImage = new System.IO.FileStream(preCommsObservation.photoSource, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        using (IAmazonS3 s3Client = new AmazonS3Client(@"AKIAJ5ZSBXUXB6IQFZOQ", @"5Q+Y+x3OWE3yrx1i3uxxl9zEAKSa83/DPVn4aQ8Y", Amazon.RegionEndpoint.EUWest1))
                        {
                            using (Amazon.S3.Transfer.TransferUtility fileTransferUtility = new Amazon.S3.Transfer.TransferUtility(s3Client))
                            {
                                Amazon.S3.Transfer.TransferUtilityUploadRequest fileTransferUtilityRequest = new Amazon.S3.Transfer.TransferUtilityUploadRequest
                                {
                                    BucketName = "apps-cheshire-east",
                                    InputStream = streamImage,
                                    Key = strKeyName,
                                    CannedACL = S3CannedACL.PublicRead
                                };

                                using (System.Threading.Tasks.Task taskUpload = fileTransferUtility.UploadAsync(fileTransferUtilityRequest))
                                {
                                    taskUpload.Wait();
                                    if (taskUpload.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
                                    {
                                        bSuccess = true;

                                        observation.data.photoDestination = $"https://apps-cheshire-east.s3.amazonaws.com/{strKeyName}";

                                        try
                                        {
                                            CrossLocalNotifications.Current.Show("Report Image", $"Image successfully uploaded.", NotificationID++);
                                        }
                                        catch (System.Exception ex) { }

                                        fileInfo = null;
                                    }
                                }

                                fileTransferUtilityRequest = null;
                            }
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

                strKeyName = "";
            }

            if (bSuccess)
            {
                bSuccess = false;

                string strJSON = JsonConvert.SerializeObject(observation, Formatting.None);
                observation.data = null;
                observation = null;

#if _LIVE_SYSTEM
                string strAddress = @"https://qwest-forms.achieveservice.com/api/startthread/?apiKey=8a5fc80dd959f15fb828caa2a2b944b017e810d3511f26e8c304ed6a4a3e3ff4533fbcc05f089ab570d6d3cb1f0dd97256c2e3370df4c6ed5e2950c14e98eac8";
#else
                string strAddress = @"https://qwest-test-forms.achieveservice.com/api/startthread/?apiKey=50d734bdf23a2858daba1d608577c4ab61fcec1397c1cec5bd3677fd38332f362174e26c966c92d8a4a871a77461cf752775fd72efe7f6458c0486eef6b0970a";
#endif
                try
                {
                    using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                        using (System.Net.Http.StringContent content = new System.Net.Http.StringContent(strJSON.ToString(), System.Text.Encoding.UTF8, "application/json"))
                        {
                            try
                            {
                                using (System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> taskResponse = httpClient.PostAsync(strAddress, content))
                                {
                                    taskResponse.Wait();
                                    if (taskResponse.Result.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        using (System.Threading.Tasks.Task<string> taskContent = taskResponse.Result.Content.ReadAsStringAsync())
                                        {
                                            taskContent.Wait();
                                            if (taskContent.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
                                            {
                                                bSuccess = true;

                                                strJSON = taskContent.Result;
                                            }
                                        }
                                    }
                                }
                            }

                            catch (System.Net.WebException ex) { }
                            catch (System.AggregateException ex) { }
                            catch (System.Exception ex) { }
                        }
                    }
                }
                catch (System.Exception ex) { }

                if (strJSON != null)
                {
                    ReportIt.Models.ObservationResponse observationResponse = null;

                    try
                    {
                        observationResponse = JsonConvert.DeserializeObject<ReportIt.Models.ObservationResponse>(strJSON);
                        if (observationResponse != null)
                        {
                            string strObservationResponse = $"Case ID {observationResponse.data.case_id} successfully created.";
                            if (observationResponse.messages != null && observationResponse.messages.Count > 0)
                            {
                                strObservationResponse += $"\r\n{observationResponse.messages[0]}.";
                            }

                            CrossLocalNotifications.Current.Show("Report", strObservationResponse, NotificationID++);

                            strObservationResponse = null;
                        }
                    }
                    catch (System.Exception ex) { }

                    // Add this observation to the legacy observations list.
                    LegacyObservations.PushNewObservation(
                                            System.DateTime.Now,
                                            observationResponse.data.case_id,
                                            preCommsObservation.type,
                                            preCommsObservation.issue,
                                            preCommsObservation.enquiryDescription,
                                            preCommsObservation.mapLat,
                                            preCommsObservation.mapLon,
                                            postcodeResult.postcode,
                                            preCommsObservation.photoSource
                                            );

                    observationResponse.data = null;
                    observationResponse = null;

                    strJSON = null;
                }

                strAddress = null;
            }

            postcodeResult = null;

            return bSuccess;
        }

        private bool FindNearestPostCode(
                                    double dLatitude,
                                    double dLongitude,
                                    out ReportIt.Models.PostCodeResult postcodeResult
                                    )
        {
            bool bSuccess = false;

            postcodeResult = null;

            string strBaseAddress = "https://api.postcodes.io/postcodes";
            string strAddress = $"{strBaseAddress}?lon={dLongitude}&lat={dLatitude}&wideSearch=true";
            strBaseAddress = null;

            string strJSON = null;

            using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                try
                {
                    using (System.Threading.Tasks.Task<string> taskResponse = httpClient.GetStringAsync(strAddress))
                    {
                        taskResponse.Wait();
                        if (taskResponse.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
                        {
                            strJSON = taskResponse.Result;
                        }
                    }
                }

                catch (System.Net.WebException ex) { }
                catch (System.AggregateException ex) { }
                catch (System.Exception ex) { }
            }

            strAddress = null;

            if (strJSON != null)
            {
                ReportIt.Models.PostCodeResponse postcodeResponse = null;

                try
                {
                    postcodeResponse = JsonConvert.DeserializeObject<ReportIt.Models.PostCodeResponse>(strJSON);
                    if (postcodeResponse != null && postcodeResponse.status == (uint)System.Net.HttpStatusCode.OK && postcodeResponse.result != null)
                    {
                        postcodeResult = (from resp in postcodeResponse.result orderby resp.distance ascending select resp).FirstOrDefault();
                        bSuccess = true;
                    }
                }

                catch (System.Exception ex) { }

                postcodeResponse = null;

                strJSON = null;
            }

            return bSuccess;
        }

        public async System.Threading.Tasks.Task<LocationWrapper> TestLocationServicesEnabled()
        {
            LocationWrapper locationWrapper = new LocationWrapper();

            try
            {
                locationWrapper.Location = await Geolocation.GetLastKnownLocationAsync();
            }

            catch (Xamarin.Essentials.FeatureNotSupportedException ex)
            { }

            catch (Xamarin.Essentials.FeatureNotEnabledException ex)
            { }

            catch (Xamarin.Essentials.PermissionException ex)
            { }

            catch (System.Exception ex)
            { }

            if (locationWrapper.Location == null)
            {
                GeolocationRequest geolocationRequest = null;

                try
                {
                    geolocationRequest = new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromMilliseconds(60000));// 5000));
                    locationWrapper.Location = await Geolocation.GetLocationAsync(geolocationRequest);
                }

                catch (Xamarin.Essentials.FeatureNotSupportedException ex)
                { }

                catch (Xamarin.Essentials.FeatureNotEnabledException ex)
                { }

                catch (Xamarin.Essentials.PermissionException ex)
                { }

                catch (System.Exception ex)
                { }

                geolocationRequest = null;
            }

            if (locationWrapper.Location != null)
            {
                locationWrapper.LocationServicesEnabled = true;
            }

            return locationWrapper;
        }
    }
}

using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace ReportIt.ViewModels
{
    public class CreateObservationViewModel : BaseViewModel
    {
        private double dLatitude;
        public double Latitude
        {
            get { return dLatitude; }
            set { dLatitude = value; }
        }

        private double dLongitude;
        public double Longitude
        {
            get { return dLongitude; }
            set { dLongitude = value; }
        }

        private string strLocation;
        public string Location
        {
            get { return strLocation; }
        }

        private double dEastings;
        public double Eastings
        {
            get { return dEastings; }
            set { dEastings = value; }
        }

        private double dNorthings;
        public double Northings
        {
            get { return dNorthings; }
            set { dNorthings = value; }
        }

        private string strType = "";
        public string Type
        {
            get { return strType; }
            set { strType = value; }
        }

        private string strIssue = "";
        public string Issue
        {
            get { return strIssue; }
            set { strIssue = value; }
        }

        private string strServiceCode = "";
        public string ServiceCode
        {
            get { return strServiceCode; }
            set { strServiceCode = value; }
        }

        private string strSubjectCode = "";
        public string SubjectCode
        {
            get { return strSubjectCode; }
            set { strSubjectCode = value; }
        }

        private string strImagePath = "";
        public string ImagePath
        {
            get { return strImagePath; }
            set { strImagePath = value; }
        }

        private string strMapUSRN = "";
        public string MapUSRN
        {
            get { return strMapUSRN; }
            set { strMapUSRN = value; }
        }

        private string strCentralassetid = "";
        public string Centralassetid
        {
            get { return strCentralassetid; }
            set { strCentralassetid = value; }
        }

        public CreateObservationViewModel()
        {
            Latitude = 0.0f;
            Longitude = 0.0f;

            strLocation = "";
        }

        public void SetStreetlightData(
                            double dEastings,
                            double dNorthings,
                            string strMapUSRN,
                            string strCentralassetid
                            )
        {
            Eastings = dEastings;
            Northings = dNorthings;
            MapUSRN = strMapUSRN;
            Centralassetid = strCentralassetid;
        }

        public void SetBinData(
                            double dEastings,
                            double dNorthings,
                            string strMapUSRN,
                            string strCentralassetid
                            )
        {
            Eastings = dEastings;
            Northings = dNorthings;
            MapUSRN = strMapUSRN;
            Centralassetid = strCentralassetid;
        }

        public void SetAdditionalData(
                            double dLatitude,
                            double dLongitude,
                            Plugin.Media.Abstractions.MediaFile mediaObservationImage
                            )
        {
            Latitude = dLatitude;
            Longitude = dLongitude;

            string strLatitude = dLatitude.ToString("F6");
            string strLongitude = dLongitude.ToString("F6");
            strLocation = "Lat: " + strLatitude + " Lon:" + strLongitude;
            strLongitude = null;
            strLatitude = null;

            if (mediaObservationImage != null)
            {
                // Copy temporary image file to permanent location.
                System.IO.FileInfo fileInfo = null;
                string strCopyTo = null;

                try
                {
                    fileInfo = new System.IO.FileInfo(mediaObservationImage.Path);
                    strCopyTo = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{fileInfo.Name}");

                    // Check for iOS.
                    // New photo's are implicitly added to the app's local application data folder.
                    if (System.String.Compare(mediaObservationImage.Path, strCopyTo, true) != 0)
                    {
                        fileInfo = fileInfo.CopyTo(strCopyTo);
                    }

                    ImagePath = strCopyTo;
                }

                catch (System.Exception ex) { }

                strCopyTo = null;
                fileInfo = null;
            }
        }
    }
}
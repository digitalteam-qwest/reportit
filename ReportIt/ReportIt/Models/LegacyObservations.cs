using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

using Newtonsoft.Json;

using System.Linq;

namespace ReportIt.Models
{
    public class LegacyObservation
    {
        // Properties are not capital due to json mapping.
        public System.DateTime timestamp { get; set; }

        public string caseId { get; set; }

        public string type { get; set; }

        public string issue { get; set; }

        public string description { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        public string location { get; set; }

        public string photoSource { get; set; }

        public string checkStatusURL { get; set; }
    }

    public class LegacyObservations
    {
        public static System.Collections.Generic.List<LegacyObservation> listObservations = null;

        private System.Collections.ObjectModel.ObservableCollection<LegacyObservation> legacyObservationsObservableCollection;
        public System.Collections.ObjectModel.ObservableCollection<LegacyObservation> LegacyObservationsObservableCollection
        {
            get
            {
                if (legacyObservationsObservableCollection != null)
                {
                    legacyObservationsObservableCollection.Clear();
                    legacyObservationsObservableCollection = null;
                }

                if (listObservations != null)
                {
                    legacyObservationsObservableCollection = new System.Collections.ObjectModel.ObservableCollection<LegacyObservation>(listObservations);
                }
                else
                {
                    legacyObservationsObservableCollection = new System.Collections.ObjectModel.ObservableCollection<LegacyObservation>();
                }

                return legacyObservationsObservableCollection;
            }
        }

        public LegacyObservations()
        {
        }

        public bool LoadFromStorage()
        {
            bool bSuccess = false;

            if (listObservations != null)
            {
                listObservations.Clear();
                listObservations = null;
            }

            if (Application.Current.Properties.ContainsKey("LegacyObservations"))
            {
                string strJSON = (string)Application.Current.Properties["LegacyObservations"];
                if (!String.IsNullOrEmpty(strJSON))
                {
                    listObservations = JsonConvert.DeserializeObject<System.Collections.Generic.List<LegacyObservation>>(strJSON);
                    if (listObservations != null)
                    {
                        bSuccess = true;
                    }

                    strJSON = null;
                }
            }

            return bSuccess;
        }

        private static bool SaveToStorage()
        {
            bool bSuccess = false;

            if (listObservations != null)// && listObservations.Count > 0)
            {
                string strJSON = JsonConvert.SerializeObject(listObservations);
                if (false)
                {
                    // Clear the storage.
                    strJSON = "";
                }

                if (strJSON != null)
                {
                    if (!Application.Current.Properties.ContainsKey("LegacyObservations"))
                    {
                        Application.Current.Properties.Add("LegacyObservations", strJSON);
                    }
                    else
                    {
                        Application.Current.Properties["LegacyObservations"] = strJSON;
                    }

                    strJSON = null;

                    System.Func<System.Threading.Tasks.Task> savePropertiesSync = async () =>
                    {
                        await Application.Current.SavePropertiesAsync();
                    };
                    savePropertiesSync.Invoke();
                    savePropertiesSync = null;

                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        public static bool SaveToStorage(System.Collections.ObjectModel.ObservableCollection<LegacyObservation> legacyObservationsObservableCollection)
        {
            bool bSuccess = false;

            if (listObservations != null)
            {
                listObservations.Clear();
                listObservations = null;
            }

            listObservations = legacyObservationsObservableCollection.ToList();

            bSuccess = SaveToStorage();

            return bSuccess;
        }

        public static bool PushNewObservation(
                        System.DateTime timestamp,
                        string strCaseId,
                        string strType,
                        string strIssue,
                        string strDescription,
                        double dLatitude,
                        double dLongitude,
                        string strLocation,
                        string strPhotoSource
                        )
        {
            bool bSuccess = false;

            if (listObservations == null)
            {
                listObservations = new System.Collections.Generic.List<LegacyObservation>();
            }

            LegacyObservation legacyObservation = new LegacyObservation();
            legacyObservation.timestamp = timestamp;
            legacyObservation.caseId = strCaseId;
            legacyObservation.type = strType;
            legacyObservation.issue = strIssue;
            legacyObservation.description = strDescription;
            legacyObservation.latitude = dLatitude;
            legacyObservation.longitude = dLongitude;
            legacyObservation.location = strLocation;
            legacyObservation.photoSource = strPhotoSource;
            legacyObservation.checkStatusURL = $"https://my.cheshirewestandchester.gov.uk/en/service/Customer_status_checker?Test1={strCaseId}";

            listObservations.Add(legacyObservation);

            // Create a temporary list of observations that are 1 year or more old.
            System.DateTime dtOneYearAgo = System.DateTime.Now.AddYears(-1);
            System.Collections.Generic.List<LegacyObservation> listObservationsExpired = (from obs in listObservations where obs.timestamp < dtOneYearAgo orderby obs.timestamp ascending select obs).Reverse().ToList<LegacyObservation>();
            if (listObservationsExpired != null)
            {
                foreach (LegacyObservation legacyObservationExpired in listObservationsExpired)
                {
                    // Delete the photo file.
                    if (!String.IsNullOrEmpty(legacyObservationExpired.photoSource))
                    {
                        System.IO.FileInfo fileInfo;

                        try
                        {
                            fileInfo = new System.IO.FileInfo(legacyObservationExpired.photoSource);
                            fileInfo.Delete();
                        }
                        catch (System.Exception ex) { }

                        fileInfo = null;
                    }

                    listObservations.Remove(legacyObservationExpired);
                }

                listObservationsExpired.Clear();
                listObservationsExpired = null;
            }

            // Order the new list in reverse date order (most recent first).
            listObservations = (from obs in listObservations orderby obs.timestamp ascending select obs).Reverse().ToList<LegacyObservation>();

            // Save the new list back to storage.
            bSuccess = SaveToStorage();

            return bSuccess;
        }

        public System.Collections.ObjectModel.ObservableCollection<LegacyObservation> GetObservableCollection()
        {
            System.Collections.ObjectModel.ObservableCollection<LegacyObservation> observableLegacyObservations = null;

            if (listObservations != null)
            {
                observableLegacyObservations = new System.Collections.ObjectModel.ObservableCollection<LegacyObservation>(listObservations);
            }
            else
            {
                observableLegacyObservations = new System.Collections.ObjectModel.ObservableCollection<LegacyObservation>();
            }

            return observableLegacyObservations;
        }
    }
}

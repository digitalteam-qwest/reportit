using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

using Newtonsoft.Json;

namespace ReportIt.Models
{
    public class PreCommsObservationData
    {
        // Properties are not capital due to json mapping.
        public string serviceCode { get; set; }

        public string subjectCode { get; set; }

        public string type { get; set; }

        public string issue { get; set; }

        public string enquiryDescription { get; set; }

        public double mapLat { get; set; }

        public double mapLon { get; set; }

        public double eastings { get; set; } = 0.0f;

        public double northings { get; set; } = 0.0f;

        public string Email_Address { get; set; }

        public string Phone_Number { get; set; }

        public string photoSource { get; set; }

        public string mapUSRN { get; set; }

        public string centralassetid { get; set; }
    }

    public class PreCommsObservations
    {
        private static PreCommsObservations preCommsObservations = null;

        private static Object PreCommsObservationsLock = new Object();

        public static System.Collections.Generic.List<PreCommsObservationData> listObservations = null;

        private System.Collections.ObjectModel.ObservableCollection<PreCommsObservationData> preCommsObservationsObservableCollection;
        public System.Collections.ObjectModel.ObservableCollection<PreCommsObservationData> PreCommsObservationsObservableCollection
        {
            get
            {
                if (preCommsObservationsObservableCollection != null)
                {
                    preCommsObservationsObservableCollection.Clear();
                    preCommsObservationsObservableCollection = null;
                }

                if (listObservations != null)
                {
                    preCommsObservationsObservableCollection = new System.Collections.ObjectModel.ObservableCollection<PreCommsObservationData>(listObservations);
                }
                else
                {
                    preCommsObservationsObservableCollection = new System.Collections.ObjectModel.ObservableCollection<PreCommsObservationData>();
                }

                return preCommsObservationsObservableCollection;
            }
        }

        public PreCommsObservations()
        {
            preCommsObservations = this;
        }

        public static PreCommsObservations GetInstance()
        {
            lock (PreCommsObservationsLock)
            {
                if (preCommsObservations == null)
                {
                    preCommsObservations = new PreCommsObservations();
                }
            }

            return preCommsObservations;
        }

        public bool LoadFromStorage()
        {
            bool bSuccess = false;

            if (listObservations != null)
            {
                listObservations.Clear();
                listObservations = null;
            }

            if (Application.Current.Properties.ContainsKey("PreCommsObservations"))
            {
                string strJSON = (string)Application.Current.Properties["PreCommsObservations"];
                if (!String.IsNullOrEmpty(strJSON))
                {
                    listObservations = JsonConvert.DeserializeObject<System.Collections.Generic.List<PreCommsObservationData>>(strJSON);
                    if (listObservations != null)
                    {
                        bSuccess = true;
                    }

                    strJSON = null;
                }
            }

            return bSuccess;
        }

        private bool SaveToStorage()
        {
            bool bSuccess = false;

            if (listObservations != null)
            {
                string strJSON = "";
                if (listObservations.Count > 0)
                {
                    strJSON = JsonConvert.SerializeObject(listObservations);
                }

                if (false)
                {
                    // Clear the storage.
                    strJSON = "";
                }

                if (strJSON != null)
                {
                    if (!Application.Current.Properties.ContainsKey("PreCommsObservations"))
                    {
                        Application.Current.Properties.Add("PreCommsObservations", strJSON);
                    }
                    else
                    {
                        Application.Current.Properties["PreCommsObservations"] = strJSON;
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

        public bool PushNewObservation(PreCommsObservationData observation)
        {
            bool bSuccess = false;

            lock (PreCommsObservationsLock)
            {
                if (listObservations == null)
                {
                    listObservations = new System.Collections.Generic.List<PreCommsObservationData>();
                }

                listObservations.Add(observation);

                // Save the new list back to storage.
                bSuccess = SaveToStorage();
            }

            return bSuccess;
        }

        public PreCommsObservationData GetPreCommsObservation()
        {
            PreCommsObservationData observation = null;

            lock (PreCommsObservationsLock)
            {
                if (listObservations != null && listObservations.Count > 0)
                {
                    observation = listObservations[0];
                }
            }

            return observation;
        }

        public bool RemovePreCommsObservation(PreCommsObservationData observation)
        {
            bool bSuccess = false;

            lock (PreCommsObservationsLock)
            {
                if (listObservations != null)
                {
                    bSuccess = listObservations.Remove(observation);
                    if (bSuccess)
                    {
                        // Save the new list back to storage.
                        bSuccess = SaveToStorage();
                    }
                }
            }

            return bSuccess;
        }

        public System.Collections.ObjectModel.ObservableCollection<PreCommsObservationData> GetObservableCollection()
        {
            System.Collections.ObjectModel.ObservableCollection<PreCommsObservationData> observablePreCommsObservations = null;

            lock (PreCommsObservationsLock)
            {
                if (listObservations != null)
                {
                    observablePreCommsObservations = new System.Collections.ObjectModel.ObservableCollection<PreCommsObservationData>(listObservations);
                }
                else
                {
                    observablePreCommsObservations = new System.Collections.ObjectModel.ObservableCollection<PreCommsObservationData>();
                }
            }

            return observablePreCommsObservations;
        }
    }
}

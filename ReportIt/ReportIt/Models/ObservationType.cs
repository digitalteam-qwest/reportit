using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Runtime.CompilerServices;

namespace ReportIt.Models
{
    public class ObservationType : System.ComponentModel.INotifyPropertyChanged
    {
        public string Type { get; set; }
        public string Issue { get; set; }

        public string SubjectCode { get; set; }
        public string ServiceCode { get; set; }

        private Xamarin.Forms.Color textColour = Xamarin.Forms.Color.Black;
        public Xamarin.Forms.Color TextColour
        {
            get
            {
                return textColour;
            }

            set
            {
                SetProperty(ref textColour, value);
            }
        }

        protected bool SetProperty<T>(
                            ref T backingStore,
                            T value,
                            [CallerMemberName]string propertyName = "",
                            Action onChanged = null
                            )
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class ObservationTypeGroup : System.Collections.ObjectModel.ObservableCollection<ObservationType>, System.ComponentModel.INotifyPropertyChanged
    {
        public string Title { get; set; }

        public string TitleWithItemCount
        {
            get { return string.Format("{0} ({1})", Title, ItemCount); }
        }

        public string ShortName { get; set; }

        private bool _expanded;
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged("Expanded");
                    OnPropertyChanged("StateIcon");
                }
            }
        }

        public string StateIcon
        {
            get { return Expanded ? "collapsed_blue.png" : "expanded_blue.png"; }
        }

        public int ItemCount { get; set; }

        public ObservationTypeGroup(string title, string shortName, bool expanded = false)
        {
            Title = title;
            ShortName = shortName;
            Expanded = expanded;
        }

        public static System.Collections.ObjectModel.ObservableCollection<ObservationTypeGroup> All { private set; get; }

        static ObservationTypeGroup()
        {
            System.Collections.ObjectModel.ObservableCollection<ObservationTypeGroup> Groups = new System.Collections.ObjectModel.ObservableCollection<ObservationTypeGroup>{
                new ObservationTypeGroup("Road Surface", "RSU"){
                    new ObservationType { Type = "Road Surface", Issue = "Pothole", SubjectCode = "POTH", ServiceCode = "0010" },
                    new ObservationType { Type = "Road Surface", Issue = "Cracking", SubjectCode = "POTH", ServiceCode = "0010" },
                    new ObservationType { Type = "Road Surface", Issue = "Edge Deterioration", SubjectCode = "LODT", ServiceCode = "0010" },
                    new ObservationType { Type = "Road Surface", Issue = "Obstruction", SubjectCode = "UNOB", ServiceCode = "0010" },
                    new ObservationType { Type = "Road Surface", Issue = "Mud/oil on road", SubjectCode = "SLOP", ServiceCode = "0010" },
                    new ObservationType { Type = "Road Surface", Issue = "Flooding", SubjectCode = "FLOD", ServiceCode = "0010" }
                },

                new ObservationTypeGroup("Footway", "FWA"){
                    new ObservationType { Type = "Footway", Issue = "Cracked Paving", SubjectCode = "FW", ServiceCode = "0010" },
                    new ObservationType { Type = "Footway", Issue = "Kerb Damaged", SubjectCode = "KER", ServiceCode = "0010" },
                    new ObservationType { Type = "Footway", Issue = "Pothole", SubjectCode = "POTH", ServiceCode = "0010" },
                    new ObservationType { Type = "Footway", Issue = "Obstruction", SubjectCode = "UNOB", ServiceCode = "0010" },
                    new ObservationType { Type = "Footway", Issue = "Mud On", SubjectCode = "SLOP", ServiceCode = "0010" },
                    new ObservationType { Type = "Footway", Issue = "Flooding", SubjectCode = "FLOD", ServiceCode = "0010" }
                },

                new ObservationTypeGroup("Bridge", "BRI"){
                    new ObservationType { Type = "Bridge", Issue = "Damaged", SubjectCode = "BRI", ServiceCode = "0010" },
                    new ObservationType { Type = "Bridge", Issue = "Graffiti", SubjectCode = "BRI", ServiceCode = "0010" },
                    new ObservationType { Type = "Bridge", Issue = "Collapsed", SubjectCode = "BRI", ServiceCode = "0010" }
                },

                new ObservationTypeGroup("Grid/Drain", "GDR"){
                    new ObservationType { Type = "Grid/Drain", Issue = "Blocked", SubjectCode = "BLGU", ServiceCode = "0010" },
                    new ObservationType { Type = "Grid/Drain", Issue = "Missing", SubjectCode = "BLGU", ServiceCode = "0010" },
                    new ObservationType { Type = "Grid/Drain", Issue = "Damaged", SubjectCode = "BLGU", ServiceCode = "0010" },
                    new ObservationType { Type = "Grid/Drain", Issue = "Dislodged", SubjectCode = "BLGU", ServiceCode = "0010" }
                },

                new ObservationTypeGroup("Manhole", "MAN"){
                    new ObservationType { Type = "Manhole", Issue = "Rocking", SubjectCode = "IRON", ServiceCode = "0010" },
                    new ObservationType { Type = "Manhole", Issue = "Cover Missing", SubjectCode = "IRON", ServiceCode = "0010" },
                    new ObservationType { Type = "Manhole", Issue = "Damaged", SubjectCode = "IRON", ServiceCode = "0010" },
                    new ObservationType { Type = "Manhole", Issue = "Dislodged", SubjectCode = "IRON", ServiceCode = "0010" }
                },

                new ObservationTypeGroup("Grassed Area", "GRA"){
                    new ObservationType { Type = "Grassed Area", Issue = "Needs Cutting", SubjectCode = "GM4", ServiceCode = "SS" }
                },

                new ObservationTypeGroup("Pedestrian Crossing", "PED"){
                    new ObservationType { Type = "Pedestrian Crossing", Issue = "Light out/not working", SubjectCode = "SLO", ServiceCode = "0030" },
                    new ObservationType { Type = "Pedestrian Crossing", Issue = "Damaged Refuge", SubjectCode = "PED", ServiceCode = "0030" }
                },

                new ObservationTypeGroup("Street Light", "STL"){
                    new ObservationType { Type = "Street Light", Issue = "Light out/not working", SubjectCode = "SLO", ServiceCode = "0020" },
                    new ObservationType { Type = "Street Light", Issue = "Door Missing", SubjectCode = "SLLP", ServiceCode = "0020" },
                    new ObservationType { Type = "Street Light", Issue = "Column Damaged", SubjectCode = "SLE", ServiceCode = "0020" },
                    new ObservationType { Type = "Street Light", Issue = "On During Day", SubjectCode = "SLDB", ServiceCode = "0020" }
                },

                new ObservationTypeGroup("Traffic Light (Permanent)", "TLP"){
                    new ObservationType { Type = "Traffic Light (Permanent)", Issue = "Light out/not working", SubjectCode = "TLG", ServiceCode = "0030" },
                    new ObservationType { Type = "Traffic Light (Permanent)", Issue = "Damaged  ", SubjectCode = "TLG", ServiceCode = "0030" }
                },

                new ObservationTypeGroup("Traffic Light (Temporary)", "TLT"){
                    new ObservationType { Type = "Traffic Light (Temporary)", Issue = "Light out/not working", SubjectCode = "TTL", ServiceCode = "0010" }
                },

                new ObservationTypeGroup("Sign", "SIG"){
                    new ObservationType { Type = "Sign", Issue = "Missing", SubjectCode = "COFT", ServiceCode = "0010" },
                    new ObservationType { Type = "Sign", Issue = "Damaged or Defaced", SubjectCode = "SD", ServiceCode = "0020" },
                    new ObservationType { Type = "Sign", Issue = "Light out/not working", SubjectCode = "SO", ServiceCode = "0020" }
                },

                new ObservationTypeGroup("Trees next to a road or footway", "TRF"){
                    new ObservationType { Type = "Trees next to a road or footway", Issue = "Needs Cutting", SubjectCode = "TREE", ServiceCode = "0010" },
                    new ObservationType { Type = "Trees next to a road or footway", Issue = "Debris on Road", SubjectCode = "TREE", ServiceCode = "0010" },
                    new ObservationType { Type = "Trees next to a road or footway", Issue = "Over-hanging branches", SubjectCode = "TREE", ServiceCode = "0010" },
                    new ObservationType { Type = "Trees next to a road or footway", Issue = "Danger of Falling", SubjectCode = "TREE", ServiceCode = "0010" },
                    new ObservationType { Type = "Trees next to a road or footway", Issue = "Other", SubjectCode = "OTH", ServiceCode = "0010" }
                },

                new ObservationTypeGroup("Bollard", "BOL"){
                    new ObservationType { Type = "Bollard", Issue = "Light out/not working", SubjectCode = "BOLL", ServiceCode = "0010" },
                    new ObservationType { Type = "Bollard", Issue = "Damaged", SubjectCode = "BOLL", ServiceCode = "0010" },
                    new ObservationType { Type = "Bollard", Issue = "Missing", SubjectCode = "BOLL", ServiceCode = "0010" }
                },
                    
                new ObservationTypeGroup("Bus Shelters and Stops", "BSS"){
                    new ObservationType { Type = "Bus Shelters and Stops", Issue = "Damaged", SubjectCode = "BS", ServiceCode = "0030" }
                },

                new ObservationTypeGroup("Street Name Plate", "SNP"){
                    new ObservationType { Type = "Street Name Plate", Issue = "Damaged", SubjectCode = "STNP", ServiceCode = "0010" },
                    new ObservationType { Type = "Street Name Plate", Issue = "Missing", SubjectCode = "STNP", ServiceCode = "0010" }
                },

                new ObservationTypeGroup("Road Markings", "RDM"){
                    new ObservationType { Type = "Road Markings", Issue = "Road Markings", SubjectCode = "RDM", ServiceCode = "0010" }
                },

                new ObservationTypeGroup("Winter Maintenance", "WIN"){
                    new ObservationType { Type = "Winter Maintenance", Issue = "Winter Maintenance", SubjectCode = "WINT", ServiceCode = "0010" }
                },

                new ObservationTypeGroup("Fly tipping", "FLT"){
                    new ObservationType { Type = "Fly tipping", Issue = "Smaller domestic items - e.g. household waste, plastic bags etc..", SubjectCode = "SC14", ServiceCode = "SS" },
                    new ObservationType { Type = "Fly tipping", Issue = "Larger items - building waste, furniture, white goods etc..", SubjectCode = "SC11", ServiceCode = "SS" }
                },

                new ObservationTypeGroup("Graffiti/Flyposting", "GFP"){
                    new ObservationType { Type = "Graffiti/Flyposting", Issue = "Fly Posting", SubjectCode = "SC10", ServiceCode = "SS" },
                    new ObservationType { Type = "Graffiti/Flyposting", Issue = "Non Offensive", SubjectCode = "SC12", ServiceCode = "SS" },
                    new ObservationType { Type = "Graffiti/Flyposting", Issue = "Offensive", SubjectCode = "SC13", ServiceCode = "SS" }
                },

                new ObservationTypeGroup("Trees", "TRE"){
                    new ObservationType { Type = "Trees", Issue = "Trees", SubjectCode = "GM7", ServiceCode = "SS" },
                    new ObservationType { Type = "Trees", Issue = "Dangerous Trees", SubjectCode = "GM2", ServiceCode = "SS" }
                },

                new ObservationTypeGroup("Street cleaning", "STC"){
                    new ObservationType { Type = "Street cleaning", Issue = "Street Cleaning - Light Litter", SubjectCode = "SC20", ServiceCode = "SS" },
                    new ObservationType { Type = "Street cleaning", Issue = "Street Cleaning - Road Sweeper", SubjectCode = "SC21", ServiceCode = "SS" }
                },

                new ObservationTypeGroup("Dog fouling", "DOG"){
                    new ObservationType { Type = "Dog Fouling", Issue = "Dog Fouling", SubjectCode = "SC9", ServiceCode = "SS" }
                },

                new ObservationTypeGroup("Grass, hedge, leaves and weeds", "GLW"){
                    new ObservationType { Type = "Grass, hedge, leaves and weeds", Issue = "Leaves", SubjectCode = "GM5", ServiceCode = "SS" },
                    new ObservationType { Type = "Grass, hedge, leaves and weeds", Issue = "Grass Cuttings", SubjectCode = "GM4", ServiceCode = "SS" },
                    new ObservationType { Type = "Grass, hedge, leaves and weeds", Issue = "Overgrown Hedge", SubjectCode = "GM3", ServiceCode = "SS" },
                    new ObservationType { Type = "Grass, hedge, leaves and weeds", Issue = "Weed Control", SubjectCode = "GM8", ServiceCode = "SS" }
                },

                new ObservationTypeGroup("Full litter or dog bins", "FLB"){
                    new ObservationType { Type = "Full litter or dog bins", Issue = "Litter/dog Bin", SubjectCode = "SC15", ServiceCode = "SS" }
                    //new ObservationType { Type = "Full litter or dog bins", Issue = "Dog Bin", SubjectCode = "SC15", ServiceCode = "SS" }
                },

                new ObservationTypeGroup("Dead animals", "DEA"){
                    new ObservationType { Type = "Dead animals", Issue = "Dead Animals", SubjectCode = "SC8", ServiceCode = "SS" }
                },

                new ObservationTypeGroup("Damaged street furniture", "DSF"){
                    new ObservationType { Type = "Damaged street furniture", Issue = "Street Furn. - Repair", SubjectCode = "SC24", ServiceCode = "GSP" }
                },

                new ObservationTypeGroup("Glass drug or body fluids", "GDF"){
                    new ObservationType { Type = "Glass drug or body fluids", Issue = "Broken Glass", SubjectCode = "SC5", ServiceCode = "SS" },
                    new ObservationType { Type = "Glass drug or body fluids", Issue = "Sharps/Drug Litter", SubjectCode = "SC19", ServiceCode = "SS" },
                    new ObservationType { Type = "Glass drug or body fluids", Issue = "Body Fluids", SubjectCode = "SC3", ServiceCode = "SS" }
                }
            };

            All = Groups;
        }

        new public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}

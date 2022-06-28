using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

namespace ReportIt.ViewModels
{
    public class LegacyObservationViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        // Properties are not capital due to json mapping.
        public ReportIt.Models.LegacyObservation legacyObservation { get; set; }

        private Xamarin.Forms.LineBreakMode LineBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
        public Xamarin.Forms.LineBreakMode lineBreakMode
        {
            get
            {
                return LineBreakMode;
            }

            set
            {
                if (LineBreakMode != value)
                {
                    LineBreakMode = value;
                    OnPropertyChanged("lineBreakMode");
                }
            }
        }

        private int MaxLines = 1;
        public int maxLines
        {
            get
            {
                return MaxLines;
            }

            set
            {
                if (MaxLines != value)
                {
                    MaxLines = value;
                    OnPropertyChanged("maxLines");
                }
            }
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
}
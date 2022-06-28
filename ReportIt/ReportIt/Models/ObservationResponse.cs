using System;
using System.Collections.Generic;
using System.Text;

namespace ReportIt.Models
{
    public class ObservationData
    {
        // Properties are not capital due to json mapping.
        public string source { get; set; }

        public string serviceCode { get; set; }

        public string subjectCode { get; set; }

        public string enquiryDescription { get; set; }

        public double easting { get; set; }

        public double northing { get; set; }

        public double mapLat { get; set; }

        public double mapLon { get; set; }

        public string Email_Address { get; set; }

        public string Phone_Number { get; set; }

        public string photoDestination { get; set; }

        public string mapUSRN { get; set; }

        public string centralassetid { get; set; }
    }

    public class Observation
    {
        // Properties are not capital due to json mapping.
        public string process_id { get; set; }

        public ObservationData data { get; set; }

        public string ucrn { get; set; }

        public string submissionType { get; set; }

        public string published { get; set; }
    }

    public class ObservationResponseData
    {
        // Properties are not capital due to json mapping.
        public string case_id { get; set; }

        public string status { get; set; }
    }

    public class ObservationResponse
    {
        // Properties are not capital due to json mapping.
        public ObservationResponseData data { get; set; }

        public System.Collections.Generic.IList<string> messages { get; set; }

        public System.Collections.Generic.IList<string> stats { get; set; }
    }
}

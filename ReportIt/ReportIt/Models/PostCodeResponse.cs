using System;
using System.Collections.Generic;
using System.Text;

namespace ReportIt.Models
{
    public class PostCodeResult
    {
        // Properties are not capital due to json mapping.
        public string postcode { get; set; }

        public UInt32 quality { get; set; }

        public UInt32 eastings { get; set; }

        public UInt32 northings { get; set; }

        public double longitude { get; set; }

        public double latitude { get; set; }

        public float distance { get; set; }
    }

    public class PostCodeResponse
    {
        // Properties are not capital due to json mapping.
        public UInt32 status { get; set; }

        public System.Collections.Generic.IList<PostCodeResult> result = null;
    }
}

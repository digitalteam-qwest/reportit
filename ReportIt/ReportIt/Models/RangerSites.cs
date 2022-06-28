using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

using Newtonsoft.Json;

namespace ReportIt.Models
{

    public class RangerSites
    {
        public static FeaturesCollection featuresCollection = null;

        private static System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Polyline> polylineCollection = null;

        public RangerSites()
        {
        }

        public bool LoadData()
        {
            bool bReturn = false;

            if (featuresCollection == null)
            {
                string strJSONFileleName = "RangerSiteLocations.geojson";

                System.Reflection.Assembly assembly = typeof(CWACBoundary).GetTypeInfo().Assembly;
                System.IO.Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{strJSONFileleName}");
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    string strJSON = reader.ReadToEnd();

                    try
                    {
                        featuresCollection = JsonConvert.DeserializeObject<FeaturesCollection>(strJSON);
                        bReturn = true;
                    }
                    catch (System.Exception ex) { }

                    strJSON = null;
                }

                stream.Dispose();
                stream = null;

                assembly = null;

                strJSONFileleName = null;
            }
            else
            {
                bReturn = true;
            }

            return bReturn;
        }

        public System.Collections.Generic.IList<Xamarin.Forms.GoogleMaps.Polyline> CreateFeaturesCollection()
        {
            if (polylineCollection == null)
            {
                polylineCollection = new System.Collections.Generic.List<Xamarin.Forms.GoogleMaps.Polyline>();

                foreach (Feature feature in featuresCollection.features)
                {
                    Xamarin.Forms.GoogleMaps.Polyline polyline = new Xamarin.Forms.GoogleMaps.Polyline();
                    polylineCollection.Add(polyline);

                    System.Collections.Generic.IList<System.Collections.Generic.IList<double>> coordinates = feature.geometry.coordinates[0];
                    foreach (System.Collections.Generic.IList<double> coordinate in coordinates)
                    {
                        Xamarin.Forms.GoogleMaps.Position pos = new Xamarin.Forms.GoogleMaps.Position(coordinate[1], coordinate[0]);
                        polyline.Positions.Add(pos);
                    }
                    coordinates = null;
                }
            }

            return polylineCollection;
        }
    }
}

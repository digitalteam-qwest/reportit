using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

using Newtonsoft.Json;

namespace ReportIt.Models
{
    public class Properties
    {
    }

    public class Geometry
    {
        public string type { get; set; }

        public System.Collections.Generic.IList<System.Collections.Generic.IList<System.Collections.Generic.IList<double>>> coordinates { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }

        public Properties properties { get; set; }

        public Geometry geometry { get; set; }
    }

    public class FeaturesCollection
    {
        public string type { get; set; }

        public System.Collections.Generic.IList<Feature> features { get; set; }
    }

    public class CWACBoundary
    {
        public static FeaturesCollection featuresCollection = null;

        private static Xamarin.Forms.GoogleMaps.Polyline polylineBoundary = null;

        private static double[] polylineBoundaryX = null;
        private static double[] polylineBoundaryY = null;

        public CWACBoundary()
        {
        }

        public bool LoadData()
        {
            bool bReturn = false;

            if (featuresCollection == null)
            {
                string strJSONFileleName = "cwacboundary.json";

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

        public Xamarin.Forms.GoogleMaps.Polyline CreateBoundary()
        {
            if (polylineBoundary == null)
            {
                if (featuresCollection.features[0].geometry.coordinates.Count > 0)
                {
                    System.Collections.Generic.IList<System.Collections.Generic.IList<double>> coordinates = featuresCollection.features[0].geometry.coordinates[0];

                    polylineBoundary = new Xamarin.Forms.GoogleMaps.Polyline();

                    polylineBoundaryX = new double[coordinates.Count];
                    polylineBoundaryY = new double[coordinates.Count];

                    int nVertex = 0;
                    foreach (System.Collections.Generic.IList<double> coordinate in coordinates)
                    {
                        polylineBoundary.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(coordinate[1], coordinate[0]));

                        polylineBoundaryX[nVertex] = coordinate[1];
                        polylineBoundaryY[nVertex] = coordinate[0];
                        nVertex += 1;
                    }

                    coordinates = null;
                }
            }

            return polylineBoundary;
        }

        public bool IsPointInPolygon(double dLat, double dLon)
        {
            int nPolyCorners = System.Math.Min(polylineBoundaryX.Length, polylineBoundaryY.Length);

            int i, j = nPolyCorners - 1;
            bool bOddNodes = false;

            for (i = 0; i < nPolyCorners; i++)
            {
                if (polylineBoundaryY[i] < dLon && polylineBoundaryY[j] >= dLon ||
                    polylineBoundaryY[j] < dLon && polylineBoundaryY[i] >= dLon)
                {
                    if (polylineBoundaryX[i] + (dLon - polylineBoundaryY[i]) / (polylineBoundaryY[j] - polylineBoundaryY[i]) * (polylineBoundaryX[j] - polylineBoundaryX[i]) < dLat)
                    {
                        bOddNodes = !bOddNodes;
                    }
                }

                j = i;
            }

            return bOddNodes;
        }
    }
}

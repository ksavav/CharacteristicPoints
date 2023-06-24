using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CharacteristicPoints.XML
{
    public class ConfigDeserializer : IXmlDeserializer
    {
        public List<List<string>> Load(string path)
        {
            var config = new List<List<string>>();
            XDocument doc = XDocument.Load(path);

            var NumberOfPoints = doc.Descendants("NumberOfPoints").ElementAt(0).Value;

            config.Add(new List<string> { NumberOfPoints });

            var PointsNames = doc.Descendants("PointName");

            var pointsList = new List<string>();

            foreach (var Point in PointsNames)
            {
                pointsList.Add(Point.Value);
            }

            config.Add(pointsList);

            return config;
        }
    }
}

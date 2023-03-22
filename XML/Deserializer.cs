using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace CharacteristicPoints.XML
{
    public class Deserializer
    {
        public List<List<string>> Load(string path)
        {
            var images_from_xml = new List<List<string>>();
            XDocument doc = XDocument.Load(path);

            var file_paths = doc.Descendants("FilePath");

            var counter = 0;

            foreach (var item in file_paths)
            {
                var Points = from x in doc.Descendants("Points").ElementAt(counter).Descendants("PointName")
                             select x.Value;

                var Xs = from x in doc.Descendants("Points").ElementAt(counter).Descendants("X")
                         select x.Value;

                var Ys = from x in doc.Descendants("Points").ElementAt(counter).Descendants("Y")
                         select x.Value;

                images_from_xml.Add(new List<string> { item.Value });
                images_from_xml.Add(Points.ToList());
                images_from_xml.Add(Xs.ToList());
                images_from_xml.Add(Ys.ToList());

                counter++;
            }

            return images_from_xml;
        }
    }
}

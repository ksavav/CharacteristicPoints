using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CharacteristicPoints.XML
{
    public class Serializer
    {
        public XDocument Save(string fileName, List<ImageToIndex> ListOfImages)
        {
            XDocument newXmlFile = new XDocument(
                new XElement("Images", 
                from image in ListOfImages 
                select new XElement("Image",
                    new XElement("FilePath", image.Image.ToString()),
                    new XElement("Points", 
                        from el in image.PointsOfImage.pointTexts 
                        select new XElement("Point", 
                            new XElement("PointName", el.Text),
                            new XElement("X", Math.Round(image.Points[image.PointsOfImage.pointTexts.FindIndex(a => a.Name == el.Name)].X, 3).ToString()),
                            new XElement("Y", Math.Round(image.Points[image.PointsOfImage.pointTexts.FindIndex(a => a.Name == el.Name)].Y, 3).ToString())))
                )));

            return newXmlFile;
        }

        public void SaveToCsv(string fileName, List<ImageToIndex> ListOfImages)
        {
            var xmlToCsv = Save(fileName, ListOfImages);

            StringBuilder csvFile = new StringBuilder();

            foreach(XElement el in xmlToCsv.Elements())
            {
                foreach(XElement el2 in el.Elements())
                {
                    var lines = from d in el2.Elements()
                                let line = string.Join(",", d.Elements().Select(e => e.Value))
                                select line;

                    csvFile.Append(string.Join(Environment.NewLine, lines));
                }
                
            }
            
            System.IO.File.WriteAllText(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, fileName + ".csv"), csvFile.ToString());
        }

        public void SaveToXml(string fileName, List<ImageToIndex> ListOfImages)
        {
            var xmlFile = Save(fileName, ListOfImages);

            xmlFile.Save(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, fileName + ".xml"));
        }
    }
}

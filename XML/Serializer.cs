using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

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
                            new XElement("X", Math.Round(image.Points[image.PointsOfImage.pointTexts.FindIndex(a => a.Name == el.Name)].X, 0).ToString()),
                            new XElement("Y", Math.Round(image.Points[image.PointsOfImage.pointTexts.FindIndex(a => a.Name == el.Name)].Y, 0).ToString())))
                )));

            return newXmlFile;
        }

        public void SaveToCsv(string fileName, List<ImageToIndex> ListOfImages)
        {
            var xmlToCsv = Save(fileName, ListOfImages);
            xmlToCsv.Save("temp.xml");
            /*StringBuilder csvFile = new StringBuilder();

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
            
            System.IO.File.WriteAllText(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, fileName + ".csv"), csvFile.ToString());*/
            string result = string.Empty;
            var xpathDoc = new XPathDocument("temp.xml");
            var xsltTransform = new System.Xml.Xsl.XslCompiledTransform();
            xsltTransform.Load("data.xsl");

            using (MemoryStream ms = new MemoryStream())
            {
                var writer = new XmlTextWriter(ms, Encoding.UTF8);
                using (var rd = new StreamReader(ms))
                {
                    var argList = new System.Xml.Xsl.XsltArgumentList();
                    xsltTransform.Transform(xpathDoc, argList, writer);
                    ms.Position = 0;
                    result = rd.ReadToEnd();
                }
            }

            File.WriteAllText(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "src/" + fileName + ".csv"), result);
        }

        public void SaveToXml(string fileName, List<ImageToIndex> ListOfImages)
        {
            var xmlFile = Save(fileName, ListOfImages);

            xmlFile.Save(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "src/" + fileName + ".xml"));
        }
    }
}

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
            StringBuilder csvFile = new StringBuilder();

            csvFile.AppendLine("File path,Point name,X,Y");

            foreach(ImageToIndex image in ListOfImages)
            {
                var lines = from el in image.PointsOfImage.pointTexts
                let line = image.Image.ToString() + "," + el.Text.ToString() + "," + 
                Math.Round(image.Points[image.PointsOfImage.pointTexts.FindIndex(a => a.Name == el.Name)].X, 0).ToString() + "," +
                Math.Round(image.Points[image.PointsOfImage.pointTexts.FindIndex(a => a.Name == el.Name)].Y, 0).ToString()
                select line;

                if (string.Join("", lines) == "") lines = new List<string>() { image.Image.ToString() };

                csvFile.Append(string.Join(Environment.NewLine, lines) + '\n');
            }

            System.IO.File.WriteAllText(fileName, csvFile.ToString());
        }

        public void SaveToXml(string fileName, List<ImageToIndex> ListOfImages)
        {
            var xmlFile = Save(fileName, ListOfImages);

            xmlFile.Save(fileName);
        }
    }
}

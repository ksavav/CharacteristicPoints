using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CharacteristicPoints.XML
{
    public class Serializer
    {
        public void Save(string fileName, List<ImageToIndex> ListOfImages)
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

            newXmlFile.Save(fileName);
        }
    }
}

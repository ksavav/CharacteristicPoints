using System.Windows.Controls;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CharacteristicPoints
{
    public class PointsOfImage
    {
        [XmlIgnore]
        public List<RadioButton> delButtons { get; set; } = new List<RadioButton>();
        [XmlIgnore]
        public List<RadioButton> renameButtons { get; set; } = new List<RadioButton>();
        public List<TextBlock> pointTexts { get; set; } = new List<TextBlock>();
        [XmlIgnore]
        public List<TextBlock> pointCoordinates { get; set; } = new List<TextBlock>();
    }
}

using System.Windows.Controls;
using System.Collections.Generic;


namespace CharacteristicPoints
{
    public class PointsOfImage
    {
        public List<RadioButton> delButtons { get; set; } = new List<RadioButton>();
        public List<RadioButton> renameButtons { get; set; } = new List<RadioButton>();
        public List<TextBlock> pointTexts { get; set; } = new List<TextBlock>();
        public List<TextBlock> pointCoordinates { get; set; } = new List<TextBlock>();
    }
}

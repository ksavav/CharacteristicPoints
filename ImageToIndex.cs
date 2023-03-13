using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CharacteristicPoints
{
    public class ImageToIndex
    {
        public BitmapImage Image { get; set; }
        public double ImageHeight { get; set; }
        public double ImageWidth { get; set; }
        public List<System.Windows.Point> Points { get; set; } = new List<System.Windows.Point>();
    }
}

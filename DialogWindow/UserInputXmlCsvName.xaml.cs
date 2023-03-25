using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CharacteristicPoints.DialogWindow
{
    /// <summary>
    /// Logika interakcji dla klasy UserInput.xaml
    /// </summary>
    public partial class UserInputXmlCsvName : Window
    {
        public UserInputXmlCsvName()
        {
            InitializeComponent();
        }

        public string NewNameForPoint
        {
            get { return FileName.Text; }
            set { FileName.Text = value; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

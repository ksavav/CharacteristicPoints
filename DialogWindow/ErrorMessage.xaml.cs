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
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class ErrorMessage : Window
    {
        public string _filePath { get; set; }
        public ErrorMessage(string filePath)
        {
            _filePath = filePath;
            InitializeComponent();
        }

        public void ShowError()
        {
            textBlock.Text = _filePath + " could not be read!";
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
    
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
    public partial class UserInput : Window
    {
        public UserInput()
        {
            InitializeComponent();
        }

        public string NewNameForPoint
        {
            get { return RenameBox.Text; }
            set { RenameBox.Text = value; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

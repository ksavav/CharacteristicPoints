using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CharacteristicPoints
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void ExitButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void UploadImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Image files|*.bmp;*.jpg;*.png";
            od.FilterIndex = 1;

            if (od.ShowDialog() == true)
            {
                UserImage.Source = new BitmapImage(new Uri(od.FileName));
                upload.Visibility= Visibility.Hidden;
                border.Visibility= Visibility.Visible;
            }
        }

        private void Coordionates(object sender, MouseButtonEventArgs e)
        {
            //UserImage.Source.Height Dodanie logiki wyswietlajacej pixele

            var coords = e.GetPosition(UserImage);
            posX.Text = "Positon X = " + Math.Round(coords.X, 3).ToString();
            posY.Text = "Positon Y = " + Math.Round(coords.Y, 3).ToString();
        }
    }
}

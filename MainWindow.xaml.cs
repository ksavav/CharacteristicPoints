using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<ImageToIndex> ListOfImages = new List<ImageToIndex>();

        bool flag = false;
        int _currentImage = 0;
        
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
            var NewImage = new ImageToIndex();

            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Image files|*.bmp;*.jpg;*.png";
            od.FilterIndex = 1;

            if (od.ShowDialog() == true)
            {
                NewImage.Image = new BitmapImage(new Uri(od.FileName));
                NewImage.ImageHeight = NewImage.Image.Height;
                NewImage.ImageWidth = NewImage.Image.Width;

                if (ListOfImages.Count == 0)
                {
                    UserImage.Source = NewImage.Image;
                }

                upload.Visibility = Visibility.Hidden;
                border.Visibility = Visibility.Visible;
            }

            ListOfImages.Add(NewImage);
            CreateImageList();
        }

        private void CreateImageList()
        {
            ImageGallery.Children.Clear();
            // zamiana na fora, żeby w przypadku większej ilości zdjęć nie zmieniały się w zależności od current image
            foreach (var i in ListOfImages)
            {
                Image x = new Image
                {
                    Source = i.Image,
                    Margin = new Thickness(10, 10, 10, 10),
                    Width = 80
                };

                ImageGallery.Children.Add(x);
            }
        }

        private void Coordionates(object sender, MouseButtonEventArgs e)
        {
            //UserImage.Source.Height Dodanie logiki wyswietlajacej pixele

            var coords = e.GetPosition(UserImage);
            posX.Text = "Positon X = " + Math.Round(coords.X, 3).ToString();
            posY.Text = "Positon Y = " + Math.Round(coords.Y, 3).ToString();
        }

        private void MoveLeft(object sender, MouseButtonEventArgs e)
        {
            if (_currentImage != 0)
            {
                _currentImage--;
                UserImage.Source = ListOfImages[_currentImage].Image;
            }

            DisplayPointsList();
        }

        private void MoveRight(object sender, MouseButtonEventArgs e)
        {
            if (_currentImage < ListOfImages.Count - 1)
            {
                _currentImage++;
                UserImage.Source = ListOfImages[_currentImage].Image;
            }

            DisplayPointsList();
        }

        private void RemoveImage(object sender, MouseButtonEventArgs e)
        {
            if (ListOfImages.Count != 1)
            {
                ListOfImages.Remove(ListOfImages[_currentImage]);

                if (_currentImage != 0)
                {
                    _currentImage--;
                    UserImage.Source = ListOfImages[_currentImage].Image;
                }

                if (ListOfImages.Count == 1)
                {
                    _currentImage = 0;
                    UserImage.Source = ListOfImages[_currentImage].Image;
                }

                CreateImageList();
                DisplayPointsList();
            }
        }

        private void SetIndex(object sender, MouseButtonEventArgs e)
        {
            if(flag == true)
            {
                var coords = e.GetPosition(UserImage);
                ListOfImages[_currentImage].Points.Add(coords);

                CreatePointsList();
            }
        }

        public void DisplayPointsList()
        {

            listOfPoints.Children.Clear();

            for (int i = 0; i < ListOfImages[_currentImage].Points.Count; i++)
            {
                if (i == 0)
                {
                    TextBlock lable = new TextBlock
                    {
                        Text = "Points list:", // + UserImage.Height.ToString() + UserImage.Width.ToString(),
                        Margin = new Thickness(10, 10, 0, 10),
                        FontFamily = new System.Windows.Media.FontFamily("#Roboto"),
                        FontSize = 20
                    };

                    listOfPoints.Children.Add(lable);
                }

                listOfPoints.Children.Add(ListOfImages[_currentImage].PointsOfImage.pointTexts[i]);
                listOfPoints.Children.Add(ListOfImages[_currentImage].PointsOfImage.pointCoordinates[i]);
                listOfPoints.Children.Add(ListOfImages[_currentImage].PointsOfImage.delButtons[i]);
                listOfPoints.Children.Add(ListOfImages[_currentImage].PointsOfImage.renameButtons[i]);
            }   
        }       

        public void CreatePointsList()
        {
            PlaceHolder.Visibility = Visibility.Hidden;

            var new_point = ListOfImages[_currentImage].Points.Count - 1;

            TextBlock x = new TextBlock
            {
                Name = "Point" + (new_point + 1).ToString(),
                Text = $"Point {new_point + 1}",
                Margin = new Thickness(10, 10, 0, 10),
                FontFamily = new System.Windows.Media.FontFamily("#Roboto"),
                FontSize = 18,
            };

            TextBlock coords = new TextBlock
            {
                Name = "Coords" + (new_point + 1).ToString(),
                Text = $"PosX = {Math.Round(ListOfImages[_currentImage].Points[new_point].X, 3)}\n" +
                       $"PosY = {Math.Round(ListOfImages[_currentImage].Points[new_point].Y, 3)}",
                Margin = new Thickness(10, 10, 0, 10),
                FontFamily = new System.Windows.Media.FontFamily("#Roboto"),
                FontSize = 18,
            };

            //listOfPoints.Children.Add(x);

            ListOfImages[_currentImage].PointsOfImage.pointTexts.Add(x);
            ListOfImages[_currentImage].PointsOfImage.pointCoordinates.Add(coords);
            ListOfImages[_currentImage].PointsOfImage.delButtons.Add(pointDelButtonCreator(new_point + 1));
            ListOfImages[_currentImage].PointsOfImage.renameButtons.Add(pointRenameButtonCreator(new_point + 1));
            DisplayPointsList();
        }

        public RadioButton pointDelButtonCreator(int pointNumber)
        {
            RadioButton delButton = new RadioButton
            {
                Name = "delButton" + pointNumber.ToString(),
                Content = "Delete",
                Style = (Style)FindResource("PointButtonTheme")
            };

            delButton.Click += new RoutedEventHandler(DeletePoint);
            //listOfPoints.Children.Add(delButton);

            return delButton;
        }

        public RadioButton pointRenameButtonCreator(int pointNumber)
        {
            RadioButton renameButton = new RadioButton
            {
                Name = "renameButton" + pointNumber.ToString(),
                Content = "Rename",
                Style = (Style)FindResource("PointButtonTheme")
            };

            renameButton.Click += new RoutedEventHandler(RenamePoint);

            return renameButton;
        }

        private void DeletePoint(object sender, RoutedEventArgs e)
        {
            RadioButton srcButton = e.Source as RadioButton;

            var elementToDel = ListOfImages[_currentImage].PointsOfImage.delButtons.FindIndex(a => a.Name == srcButton.Name);

            listOfPoints.Children.Remove(ListOfImages[_currentImage].PointsOfImage.pointTexts[elementToDel]);
            listOfPoints.Children.Remove(ListOfImages[_currentImage].PointsOfImage.pointCoordinates[elementToDel]);
            listOfPoints.Children.Remove(srcButton);
            listOfPoints.Children.Remove(ListOfImages[_currentImage].PointsOfImage.renameButtons[elementToDel]);
            ListOfImages[_currentImage].Points.RemoveAt(elementToDel);

            ListOfImages[_currentImage].PointsOfImage.pointTexts.RemoveAt(elementToDel);
            ListOfImages[_currentImage].PointsOfImage.pointCoordinates.RemoveAt(elementToDel);
            ListOfImages[_currentImage].PointsOfImage.delButtons.RemoveAt(elementToDel);
            ListOfImages[_currentImage].PointsOfImage.renameButtons.RemoveAt(elementToDel);
        }

        private void RenamePoint(object sender, RoutedEventArgs e)
        {
            RadioButton srcButton = e.Source as RadioButton;

            var elementToRename = ListOfImages[_currentImage].PointsOfImage.renameButtons.FindIndex(a => a.Name == srcButton.Name);

            ListOfImages[_currentImage].PointsOfImage.pointTexts[elementToRename].Text = "xd";
        }

        private void AddPoints(object sender, MouseButtonEventArgs e)
        {
            if (flag == false) flag = true;
            else flag = false;
        }


        /* TODO
         * 
         * Dodanie okna dialogowego do zmiany nazwy.
         */
    }
}

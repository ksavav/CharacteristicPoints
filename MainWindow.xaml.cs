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
        public List<RadioButton> PointsDelList = new List<RadioButton>();
        public List<RadioButton> PointsRenameList = new List<RadioButton>();
        public List<TextBlock> PointsTextList = new List<TextBlock>();

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

            CreatePointsList();
        }

        private void MoveRight(object sender, MouseButtonEventArgs e)
        {
            if (_currentImage < ListOfImages.Count - 1)
            {
                _currentImage++;
                UserImage.Source = ListOfImages[_currentImage].Image;
            }

            CreatePointsList();
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
                CreatePointsList();
            }
        }

        private void SetIndex(object sender, MouseButtonEventArgs e)
        {
            var coords = e.GetPosition(UserImage);
            ListOfImages[_currentImage].Points.Add(coords);

            CreatePointsList();
        }

        public void CreatePointsList()
        {
            listOfPoints.Children.Clear();
            PointsTextList.Clear();
            PointsDelList.Clear();
            PointsRenameList.Clear();

            var counter = 1;
            PlaceHolder.Visibility = Visibility.Hidden;

            foreach (var i in ListOfImages[_currentImage].Points)
            {

                if (counter == 0)
                {
                    TextBlock lable = new TextBlock
                    {
                        Text = "Points list:",
                        Margin = new Thickness(10, 10, 0, 10),
                        FontFamily = new System.Windows.Media.FontFamily("#Roboto"),
                        FontSize = 20
                    };

                    listOfPoints.Children.Add(lable);
                }

                TextBlock x = new TextBlock
                {
                    Name = "Point" + counter.ToString(),
                    Text = $"Point {counter}: \n PosX = {Math.Round(i.X, 3)} \n PosY = {Math.Round(i.Y, 3)}",
                    Margin = new Thickness(10, 10, 0, 10),
                    FontFamily = new System.Windows.Media.FontFamily("#Roboto"),
                    FontSize = 18
                };

                listOfPoints.Children.Add(x);


                PointsTextList.Add(x);
                PointsDelList.Add(pointDelButtonCreator(counter));
                PointsRenameList.Add(pointRenameButtonCreator(counter));
                counter++;
            }
        }

        public RadioButton pointDelButtonCreator(int pointNumber)
        {
            RadioButton delButton = new RadioButton
            {
                Name = "delButton" + pointNumber.ToString(),
                Content = "Delete"
            };

            delButton.Click += new RoutedEventHandler(DeletePoint);
            listOfPoints.Children.Add(delButton);

            return delButton;
        }

        public RadioButton pointRenameButtonCreator(int pointNumber)
        {
            RadioButton renameButton = new RadioButton
            {
                Name = "renameButton" + pointNumber.ToString(),
                Content = "Rename"
            };

            renameButton.Click += new RoutedEventHandler(RenamePoint);
            listOfPoints.Children.Add(renameButton);

            return renameButton;
        }

        private void DeletePoint(object sender, RoutedEventArgs e)
        {
            RadioButton srcButton = e.Source as RadioButton;

            var elementToDel = PointsDelList.FindIndex(a => a.Name == srcButton.Name);

            listOfPoints.Children.Remove(PointsTextList[elementToDel]);
            listOfPoints.Children.Remove(srcButton);
            listOfPoints.Children.Remove(PointsRenameList[elementToDel]);
            ListOfImages[_currentImage].Points.RemoveAt(elementToDel);

            PointsTextList.RemoveAt(elementToDel);
            PointsDelList.RemoveAt(elementToDel);
            PointsRenameList.RemoveAt(elementToDel);
        }

        private void RenamePoint(object sender, RoutedEventArgs e)
        {
            RadioButton srcButton = e.Source as RadioButton;

            var elementToRename = PointsDelList.FindIndex(a => a.Name == srcButton.Name);

            PointsTextList[elementToRename - 1].Text = "xd";
        }
    }
}

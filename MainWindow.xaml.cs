using CharacteristicPoints.DialogWindow;
using CharacteristicPoints.XML;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        int maxSizeOfImageGallery = 7;

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

                if(NewImage.Image != null)
                {
                    NewImage.ImageHeight = NewImage.Image.PixelHeight;
                    NewImage.ImageWidth = NewImage.Image.PixelWidth;

                    if (ListOfImages.Count == 0)
                    {
                        UserImage.Source = NewImage.Image;
                    }

                    upload.Visibility = Visibility.Hidden;
                    border.Visibility = Visibility.Visible;

                    ListOfImages.Add(NewImage);
                    CreateImageList_event();
                }
            }
        }

        private void CreateImageList_event()
        {
            ImageGallery.Children.Clear();

            if (ListOfImages.Count <= maxSizeOfImageGallery) CreateImageList(0, ListOfImages.Count);

            else if (ListOfImages.Count > maxSizeOfImageGallery && _currentImage < 4) 
                CreateImageList(0, maxSizeOfImageGallery);

            else if (ListOfImages.Count > maxSizeOfImageGallery && _currentImage > ListOfImages.Count - 4) 
                CreateImageList(ListOfImages.Count - maxSizeOfImageGallery, ListOfImages.Count);

            else CreateImageList(_currentImage - 3, _currentImage + 4);
        }

        public void CreateImageList(int starting_point, int ending_point)
        {
            for (int i = starting_point; i < ending_point; i++)
            {
                Image x = new Image
                {
                    Source = ListOfImages[i].Image,
                    Margin = new Thickness(10, 10, 10, 10),
                    Width = 80
                };

                if (i == _currentImage)
                {
                    Border currentImageBorder = new Border
                    {
                        BorderThickness = new Thickness(5),
                        BorderBrush = Brushes.Red,
                        Margin = new Thickness(10, 10, 10, 10),
                        CornerRadius = new CornerRadius(10)
                    };

                    currentImageBorder.Child = x;
                    ImageGallery.Children.Add(currentImageBorder);
                }

                else ImageGallery.Children.Add(x);
            }
        }

        private void MoveLeft(object sender, MouseButtonEventArgs e)
        {
            if (_currentImage != 0)
            {
                RemoveDots();
                _currentImage--;
                UserImage.Source = ListOfImages[_currentImage].Image;
            }
            CreateImageList_event();
            DisplayPointsList();
        }

        private void MoveRight(object sender, MouseButtonEventArgs e)
        {
            if (_currentImage < ListOfImages.Count - 1)
            {
                RemoveDots();
                _currentImage++;
                UserImage.Source = ListOfImages[_currentImage].Image;
            }
            CreateImageList_event();
            DisplayPointsList();
        }

        private void RemoveDots()
        {
            foreach (var dot in ListOfImages[_currentImage].PointsOfImage.pointDots)
            {
                canvas1.Children.Remove(dot);
            }
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

                CreateImageList_event();
                DisplayPointsList();
            }
        }

        private void SetIndex(object sender, MouseButtonEventArgs e)
        {
            if(flag == true)
            {
                var coords = e.GetPosition(UserImage);

                coords = GetRealPoint(coords);
                ListOfImages[_currentImage].Points.Add(coords);

                
                CreatePointsList();
            }
        }

        public void DisplayPointsList()
        {

            listOfPoints.Children.Clear();
            RemoveDots();

            for (int i = 0; i < ListOfImages[_currentImage].Points.Count; i++)
            {
                if (i == 0)
                {
                    TextBlock lable = new TextBlock
                    {
                        Text = "Points list:", // + UserImage.Height.ToString() + UserImage.Width.ToString(),
                        Margin = new Thickness(10, 10, 0, 10),
                        FontFamily = new FontFamily("#Roboto"),
                        FontSize = 20
                    };

                    listOfPoints.Children.Add(lable);
                }

                canvas1.Children.Add(ListOfImages[_currentImage].PointsOfImage.pointDots[i]);
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

            ListOfImages[_currentImage].PointsOfImage.pointTexts.Add(pointNameTextBlockCreator("Point" + (new_point + 1).ToString()));
            ListOfImages[_currentImage].PointsOfImage.pointCoordinates.Add(coordsTextBlockCreator(Math.Round(ListOfImages[_currentImage].Points[new_point].X, 0), Math.Round(ListOfImages[_currentImage].Points[new_point].Y, 0)));
            ListOfImages[_currentImage].PointsOfImage.delButtons.Add(pointDelButtonCreator(new_point + 1));
            ListOfImages[_currentImage].PointsOfImage.renameButtons.Add(pointRenameButtonCreator(new_point + 1));
            DisplayPointsList();
        }

        public TextBlock coordsTextBlockCreator(double x, double y)
        {
            return new TextBlock
            {
                //Name = "Coords" + (new_point + 1).ToString(),
                Text = $"PosX = {x}\n" +
                       $"PosY = {y}",
                Margin = new Thickness(10, 10, 0, 10),
                FontFamily = new FontFamily("#Roboto"),
                FontSize = 18,
            };
        }

        public TextBlock pointNameTextBlockCreator(string name)
        {
            return new TextBlock
            {
                Name = String.Concat(name.Where(c => !Char.IsWhiteSpace(c))),
                Text = name,
                Margin = new Thickness(10, 10, 0, 10),
                FontFamily = new FontFamily("#Roboto"),
                FontSize = 18,
            };
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
            canvas1.Children.Remove(ListOfImages[_currentImage].PointsOfImage.pointDots[elementToDel]);
            ListOfImages[_currentImage].Points.RemoveAt(elementToDel);

            ListOfImages[_currentImage].PointsOfImage.pointTexts.RemoveAt(elementToDel);
            ListOfImages[_currentImage].PointsOfImage.pointCoordinates.RemoveAt(elementToDel);
            ListOfImages[_currentImage].PointsOfImage.delButtons.RemoveAt(elementToDel);
            ListOfImages[_currentImage].PointsOfImage.renameButtons.RemoveAt(elementToDel);
            ListOfImages[_currentImage].PointsOfImage.pointDots.RemoveAt(elementToDel);
        }

        private void RenamePoint(object sender, RoutedEventArgs e)
        {
            RadioButton srcButton = e.Source as RadioButton;
            var dialog = new UserInput();

            var elementToRename = ListOfImages[_currentImage].PointsOfImage.renameButtons.FindIndex(a => a.Name == srcButton.Name);

            if (dialog.ShowDialog() == true)
            {
                ListOfImages[_currentImage].PointsOfImage.pointTexts[elementToRename].Text = dialog.NewNameForPoint;
            }
        }

        private void AddPoints(object sender, MouseButtonEventArgs e)
        {
            if (flag == false) flag = true;
            else flag = false;
        }

        private void SerializeToXml(object sender, RoutedEventArgs e)
        {
            var ser = new Serializer();
            ser.SaveToXml("xdxml", ListOfImages);
        }

        private void SerializeToCsv(object sender, RoutedEventArgs e)
        {
            var ser = new Serializer();
            ser.SaveToCsv("xdcsv", ListOfImages);
        }

        private void DeserializeFromXml(object sender, RoutedEventArgs e)
        {
            var deser = new Deserializer();

            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Image files|*.xml";
            od.FilterIndex = 1;
            var path = "";

            if (od.ShowDialog() == true)
            {
                path = od.FileName;
            }

            if(od.FileName != "")
            {
                var images_from_xml = deser.Load(path);
                CreateImagesFromXmlFile(images_from_xml);
            }
        }

        private void CreateImagesFromXmlFile(List<List<string>> list)
        {
            ListOfImages.Clear();
            int counter = 1;
            for(int i = 0; i < list.Count; i += 4) 
            {
                var newImage = new ImageToIndex();

                newImage.Image = new BitmapImage(new Uri(list[i][0]));

                newImage.ImageWidth = newImage.Image.Width;
                newImage.ImageWidth = newImage.Image.Height;

                foreach (var pointName in list[i + 1])
                {
                    newImage.PointsOfImage.pointTexts.Add(pointNameTextBlockCreator(pointName));
                    newImage.PointsOfImage.delButtons.Add(pointDelButtonCreator(counter));
                    newImage.PointsOfImage.renameButtons.Add(pointRenameButtonCreator(counter));
                }

                for(int j = 0; j < list[i + 2].Count; j++)
                {
                    newImage.PointsOfImage.pointCoordinates.Add(coordsTextBlockCreator(Convert.ToDouble(list[i + 2][j]), Convert.ToDouble(list[i + 3][j])));
                    newImage.Points.Add(new Point(Convert.ToDouble(list[i + 2][j]), Convert.ToDouble(list[i + 3][j])));
                }

                ListOfImages.Add(newImage);
            }

            _currentImage = 0;
            CreateImageList_event();
            DisplayPointsList();
        }

        
        public Point GetRealPoint(Point pointFromCanvas)
        {
            double canvasWidth;
            double canvasHeight;
            if (ListOfImages[_currentImage].ImageWidth > ListOfImages[_currentImage].ImageHeight)
            {
                canvasWidth = UserImage.Width;
                canvasHeight = ListOfImages[_currentImage].ImageHeight / (ListOfImages[_currentImage].ImageWidth / UserImage.Width);

                PointVizualization(new Point(pointFromCanvas.X, pointFromCanvas.Y + (UserImage.Height - canvasHeight)/2));
            }

            else
            {
                canvasHeight = UserImage.Height;
                canvasWidth = ListOfImages[_currentImage].ImageWidth / (ListOfImages[_currentImage].ImageHeight / UserImage.Height);

                PointVizualization(new Point(pointFromCanvas.X + (UserImage.Width - canvasWidth)/2, pointFromCanvas.Y));
            }

            return (new Point(pointFromCanvas.X * ListOfImages[_currentImage].ImageWidth / canvasWidth, 
                pointFromCanvas.Y * ListOfImages[_currentImage].ImageHeight / canvasHeight));
        }

        public void PointVizualization(Point cords)
        {
            var ellipse = new Ellipse() 
            { 
                Width = 5, 
                Height = 5,
                Fill = Brushes.Red,
                StrokeThickness = 2
            };
            Canvas.SetLeft(ellipse, cords.X - ellipse.Width/2);
            Canvas.SetTop(ellipse, cords.Y - ellipse.Height/2);
            ListOfImages[_currentImage].PointsOfImage.pointDots.Add(ellipse);
            //canvas1.Children.Add(ellipse);
        }

        /* TODO
         * Przesuwające się punkty
         * mozliwosc wybrania nazwy do xml/csv
         * utworzenie csv
         * uporzatkowanie kodu
         */
    }
}

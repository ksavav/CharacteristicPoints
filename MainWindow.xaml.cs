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
using Ookii.Dialogs.Wpf;
using System.Runtime.InteropServices;

namespace CharacteristicPoints
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ImageToIndex> ListOfImages = new List<ImageToIndex>();
        bool flag = false;
        bool moveFlag = false;
        Ellipse movingDot;
        int _currentImage = 0;
        int maxSizeOfImageGallery = 9;
        double Xproportion;
        double Yproportion;
        double widthAdjustment;
        double heighAdjustment;
        double factor = 0.3;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (x, y) => Keyboard.Focus(canvas1);
        }

        private void ExitButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void UserImage_MouseMove(object sender, MouseEventArgs e)
        {
            Point center = e.GetPosition(canvas1);
            double length = MagnifierRectangle.ActualWidth * factor;
            double radius = length / 2;
            Rect viewboxRect = new Rect(center.X - radius, center.Y - radius, length, length);
            MagnifierBrush.Viewbox = viewboxRect;

            //MagnifierRectangle.SetValue(Canvas.LeftProperty, center.X - MagnifierRectangle.ActualWidth / 2);
            //MagnifierRectangle.SetValue(Canvas.TopProperty, center.Y - MagnifierRectangle.ActualHeight / 2);
        }

        private void UserImage_MouseEnter(object sender, MouseEventArgs e)
        {
            MagnifierRectangle.Visibility = Visibility.Visible;
        }

        private void UserImage_MouseLeave(object sender, MouseEventArgs e)
        {
            MagnifierRectangle.Visibility = Visibility.Hidden;
        }

        private void UploadImage(object sender, RoutedEventArgs e)
        {
            var NewImage = new ImageToIndex();

            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Image files|*.bmp;*.jpg;*.png";
            od.FilterIndex = 1;

            if (od.ShowDialog() == true)
            {
                NewImage.Image = new BitmapImage(new Uri(od.FileName)) ;

                if(NewImage.Image != null)
                {
                    LoadImage(NewImage);
                }
            }
        }

        private void UploadFolder(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();

            

            if ((bool)dialog.ShowDialog(this))
            {
                var files = Directory.EnumerateFiles(dialog.SelectedPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp"));

                foreach (var file in files)
                {
                    var NewImage = new ImageToIndex();

                    NewImage.Image = new BitmapImage(new Uri(file));

                    LoadImage(NewImage);
                }
            }
        }

        private void LoadImage(ImageToIndex NewImage)
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
            GetRealPoint(new Point(-1, -1));
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
            int x = _currentImage;

            if (_currentImage != 0)
            {
                _currentImage--;
                UserImage.Source = ListOfImages[_currentImage].Image;
            }
            flag = false;
            string path = System.IO.Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Images/point_icon.png");
            setIndexButton.Source = new BitmapImage(new Uri(path));
            moveFlag = false;
            RemoveDots(x);
            CreateImageList_event();
            DisplayPointsList();
        }

        private void MoveRight(object sender, MouseButtonEventArgs e)
        {
            int x = _currentImage;

            if (_currentImage < ListOfImages.Count - 1)
            {
                _currentImage++;
                UserImage.Source = ListOfImages[_currentImage].Image;
            }
            flag = false;
            string path = System.IO.Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Images/point_icon.png");
            setIndexButton.Source = new BitmapImage(new Uri(path));
            moveFlag = false;
            RemoveDots(x);
            CreateImageList_event();
            DisplayPointsList();
        }

        private void RemoveDots(int x)
        {
            foreach (var dot in ListOfImages[x].PointsOfImage.pointDots)
            {
                canvas1.Children.Remove(dot);
            }
        }

        private void RemoveImage(object sender, MouseButtonEventArgs e)
        {
            int x = _currentImage;
            if (ListOfImages.Count > 1)
            {
                RemoveDots(x);
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

                CreatePointsList(coords);
            }
        }

        public void DisplayPointsList()
        {

            listOfPoints.Children.Clear();
            RemoveDots(_currentImage);

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

        public void CreatePointsList(Point coordinates)
        {
            PlaceHolder.Visibility = Visibility.Hidden;

            var new_point = ListOfImages[_currentImage].Points.Count;

            ListOfImages[_currentImage].PointsOfImage.pointTexts.Add(pointNameTextBlockCreator("Point" + (new_point + 1).ToString()));
            GetRealPoint(coordinates);
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

        private void AddPoints_Event(object sender, MouseButtonEventArgs e)
        {
            AddPoints();
        }

        private void AddPoints()
        {
            if (flag == false)
            {
                flag = true;
                string path = System.IO.Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Images/point_icon_red.png");
                setIndexButton.Source = new BitmapImage(new Uri(path));
            }
            else
            {
                flag = false;
                string path = System.IO.Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Images/point_icon.png");
                setIndexButton.Source = new BitmapImage(new Uri(path));
            }
        }

        private string GetNameForFile()
        {
            var dialog = new UserInputXmlCsvName();
            var fileName = "temp";
            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.NewNameForPoint;
            }

            return fileName;
        }

        private void SerializeToXml(object sender, RoutedEventArgs e)
        {
            var ser = new Serializer();
            ser.SaveToXml(GetNameForFile(), ListOfImages);
        }

        private void SerializeToCsv(object sender, RoutedEventArgs e)
        {
            var ser = new Serializer();
            ser.SaveToCsv(GetNameForFile(), ListOfImages);
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
            if(ListOfImages.Count != 0)
            {
                RemoveDots(_currentImage);
                ListOfImages.Clear();
            }

            int counter2 = 0;
            for (int i = 0; i < list.Count; i += 4) 
            {
                try
                {
                    var newImage = new ImageToIndex();

                    newImage.Image = new BitmapImage(new Uri(list[i][0]));

                    if (i == 0) UserImage.Source = newImage.Image;

                    newImage.ImageWidth = newImage.Image.PixelWidth;
                    newImage.ImageHeight = newImage.Image.PixelHeight;
                    int counter = 1;

                    foreach (var pointName in list[i + 1])
                    {
                        newImage.PointsOfImage.pointTexts.Add(pointNameTextBlockCreator(pointName));
                        newImage.PointsOfImage.delButtons.Add(pointDelButtonCreator(counter));
                        newImage.PointsOfImage.renameButtons.Add(pointRenameButtonCreator(counter));

                        counter++;
                    }

                    for (int j = 0; j < list[i + 2].Count; j++)
                    {
                        newImage.PointsOfImage.pointCoordinates.Add(coordsTextBlockCreator(Convert.ToDouble(list[i + 2][j]), Convert.ToDouble(list[i + 3][j])));
                        newImage.Points.Add(new Point(Convert.ToDouble(list[i + 2][j]), Convert.ToDouble(list[i + 3][j])));
                    }

                    ListOfImages.Add(newImage);

                    for (int x = 0; x < list[i + 2].Count; x++)
                    {
                        GetPositionAndCreateDotOnCanvas(newImage.Points[x], counter2);
                    }
                    counter2++;
                }
                catch(Exception e)
                {

                }
            }

            upload.Visibility = Visibility.Hidden;
            border.Visibility = Visibility.Visible;

            _currentImage = 0;
            
            CreateImageList_event();
            DisplayPointsList();
        }

        
        public void GetRealPoint(Point pointFromCanvas)
        {
            double canvasWidth;
            double canvasHeight;
            if (ListOfImages[_currentImage].ImageWidth > ListOfImages[_currentImage].ImageHeight)
            {
                canvasWidth = UserImage.Width;
                canvasHeight = ListOfImages[_currentImage].ImageHeight / (ListOfImages[_currentImage].ImageWidth / UserImage.Width);

                Xproportion = ListOfImages[_currentImage].ImageWidth / canvasWidth;
                Yproportion = ListOfImages[_currentImage].ImageHeight / canvasHeight;

                widthAdjustment = (UserImage.Height - canvasHeight) / 2;
                heighAdjustment = 0;

                if(pointFromCanvas.X != -1)
                {
                    ListOfImages[_currentImage].Points.Add(new Point(Math.Round(pointFromCanvas.X * Xproportion, 0),
                    Math.Round(pointFromCanvas.Y * Yproportion, 0)));

                    PointVizualization(new Point(pointFromCanvas.X, pointFromCanvas.Y + (UserImage.Height - canvasHeight) / 2), _currentImage);
                }
            }

            else
            {
                canvasHeight = UserImage.Height;
                canvasWidth = ListOfImages[_currentImage].ImageWidth / (ListOfImages[_currentImage].ImageHeight / UserImage.Height);

                Xproportion = ListOfImages[_currentImage].ImageWidth / canvasWidth;
                Yproportion = ListOfImages[_currentImage].ImageHeight / canvasHeight;

                widthAdjustment = 0;
                heighAdjustment = (UserImage.Width - canvasWidth)/ 2;

                if(pointFromCanvas.X != -1)
                {
                    ListOfImages[_currentImage].Points.Add(new Point(Math.Round(pointFromCanvas.X * Xproportion, 0),
                    Math.Round(pointFromCanvas.Y * Yproportion, 0)));

                    PointVizualization(new Point(pointFromCanvas.X + (UserImage.Width - canvasWidth) / 2, pointFromCanvas.Y), _currentImage);
                }
            } 
        }

        public void PointVizualization(Point cords, int image)
        {
            var ellipse = new Ellipse()
            {
                Name = ListOfImages[image].PointsOfImage.pointTexts[ListOfImages[image].PointsOfImage.pointDots.Count].Text,
                Width = 5,
                Height = 5,
                Fill = Brushes.Red,
                StrokeThickness = 2,
                ToolTip = ListOfImages[image].PointsOfImage.pointTexts[ListOfImages[image].PointsOfImage.pointDots.Count].Text + 
                "\n" + ListOfImages[image].Points[ListOfImages[image].PointsOfImage.pointDots.Count]

            };
            ellipse.MouseDown += MoveDot;
            Canvas.SetLeft(ellipse, cords.X - ellipse.Width/2);
            Canvas.SetTop(ellipse, cords.Y - ellipse.Height/2);
            ListOfImages[image].PointsOfImage.pointDots.Add(ellipse);
            //canvas1.Children.Add(ellipse);
        }

        private void GetPositionAndCreateDotOnCanvas(Point pointOnImage, int image)
        {
            double canvasWidth;
            double canvasHeight;
            if (ListOfImages[image].ImageWidth > ListOfImages[image].ImageHeight)
            {
                canvasWidth = UserImage.Width;
                canvasHeight = ListOfImages[image].ImageHeight / (ListOfImages[image].ImageWidth / UserImage.Width);

                Xproportion = ListOfImages[_currentImage].ImageWidth / canvasWidth;
                Yproportion = ListOfImages[_currentImage].ImageHeight / canvasHeight;

                widthAdjustment = (UserImage.Height - canvasHeight) / 2;
                heighAdjustment = 0;

                PointVizualization(new Point(pointOnImage.X / (ListOfImages[image].ImageWidth / UserImage.Width),
                    pointOnImage.Y / (ListOfImages[image].ImageHeight / UserImage.Height) + (UserImage.Height - canvasHeight)/2), image);
            }

            else
            {
                canvasHeight = UserImage.Height;
                canvasWidth = ListOfImages[image].ImageWidth / (ListOfImages[image].ImageHeight / UserImage.Height);

                Xproportion = ListOfImages[_currentImage].ImageWidth / canvasWidth;
                Yproportion = ListOfImages[_currentImage].ImageHeight / canvasHeight;

                widthAdjustment = 0;
                heighAdjustment = (UserImage.Width - canvasWidth) / 2;

                PointVizualization(new Point(pointOnImage.X / (ListOfImages[image].ImageWidth / UserImage.Width) + (UserImage.Width - canvasWidth)/2,
                    pointOnImage.Y / (ListOfImages[image].ImageHeight / UserImage.Height)), image);
            }
        }

        private void MoveDot(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && moveFlag == false)
            {
                Ellipse srcDot = e.Source as Ellipse;
                srcDot.Fill = Brushes.Green;
                moveFlag = true;
                movingDot = srcDot;
            }

            if (e.ClickCount == 1 && moveFlag)
            {
                movingDot.Fill = Brushes.Red;
                moveFlag = false;
                movingDot = null;
            }
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        private void MoveDotWithKeyborad(object sender, KeyEventArgs e)
        {
            if (moveFlag)
            {
                var dot = ListOfImages[_currentImage].PointsOfImage.pointDots.FindIndex(a => a.Name == movingDot.Name);

                var PointToMove = ListOfImages[_currentImage].Points[dot];



                switch (e.Key)
                {
                    case Key.Left:
                        if (ListOfImages[_currentImage].Points[dot].X - 1 >= 0)
                        {
                            PointToMove.X -= 1;
                            Canvas.SetLeft(movingDot, PointToMove.X / Xproportion + heighAdjustment - movingDot.Width/2);
                        };
                        break;

                    case Key.Right:
                        if (ListOfImages[_currentImage].ImageWidth >= ListOfImages[_currentImage].Points[dot].X + 1)
                        {
                            PointToMove.X += 1;
                            Canvas.SetLeft(movingDot, PointToMove.X / Xproportion + heighAdjustment - movingDot.Width / 2);
                        };
                        break;

                    case Key.Down:
                        if (ListOfImages[_currentImage].ImageHeight >= ListOfImages[_currentImage].Points[dot].Y + 1)
                        {
                            PointToMove.Y += 1;
                            Canvas.SetTop(movingDot, PointToMove.Y / Yproportion + widthAdjustment - movingDot.Height / 2);
                        };
                        break;

                    case Key.Up:
                        if (ListOfImages[_currentImage].Points[dot].Y - 1 >= 0)
                        {
                            PointToMove.Y -= 1;
                            Canvas.SetTop(movingDot, PointToMove.Y / Yproportion + widthAdjustment - movingDot.Height / 2);
                        };
                        break;
                }

                ListOfImages[_currentImage].Points[dot] = PointToMove;
                ListOfImages[_currentImage].PointsOfImage.pointCoordinates[dot].Text =
                    $"PosX = {PointToMove.X}\n" +
                    $"PosY = {PointToMove.Y}";
                movingDot.ToolTip = ListOfImages[_currentImage].PointsOfImage.pointTexts[dot].Text +
                    "\n" + PointToMove;
            }

            else
            {
                var point_on_canvas = Mouse.GetPosition(UserImage);
                var point = Mouse.GetPosition(Application.Current.MainWindow);

                if(point_on_canvas.X >= 0 && point_on_canvas.Y >= 0 && point_on_canvas.X <= UserImage.ActualWidth && point_on_canvas.Y <= UserImage.ActualHeight) 
                {
                    // Left boundary
                    var xL = (int)App.Current.MainWindow.Left;
                    // Right boundary
                    var xR = xL + (int)App.Current.MainWindow.Width;
                    // Top boundary
                    var yT = (int)App.Current.MainWindow.Top;
                    // Bottom boundary
                    var yB = yT + (int)App.Current.MainWindow.Height;

                    switch (e.Key)
                    {
                        case Key.Left:
                            if (point.X - 1 >= 0)
                            {
                                point.X -= 1 / Xproportion;
                            };
                            break;

                        case Key.Right:
                            if (ListOfImages[_currentImage].ImageWidth >= point.X + 1)
                            {
                                point.X += 1 / Xproportion;
                            };
                            break;

                        case Key.Down:
                            if (ListOfImages[_currentImage].ImageHeight >= point.Y + 1)
                            {
                                point.Y += 1 / Yproportion;
                            };
                            break;

                        case Key.Up:
                            if (point.Y - 1 >= 0)
                            {
                                point.Y -= 1 / Yproportion;
                            };
                            break;

                        case Key.P:
                            if(flag) CreatePointsList(point_on_canvas);
                            break;

                        case Key.F:
                            AddPoints();
                            break;
                    }

                    SetCursorPos((int)point.X + xL, (int)point.Y +yT);
                }
            }
        }

        /* TODO
         * uporzatkowanie kodu
         */
    }
}

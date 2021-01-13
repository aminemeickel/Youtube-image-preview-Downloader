using System;
using System.Windows;

using System.Windows.Media.Imaging;
using System.Timers;
using System.Diagnostics;
using System.IO;
using Tulpep.NotificationWindow;
using System.Windows.Input;

namespace Youtube_Downloader
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        
        private static string imageLink;
        #region buttons event
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataController.IsInternetAvailable())
            {
                if (DataController.IsAlink(link.Text))
                {
                    setEnbale(false);
                    var webpage = await DataController.BrowserAsync(link.Text);
                    imageLink = DataController.FetchImage(webpage);
                    SetImage(imageLink);
                    StartTheTimer();
                    setEnbale(true);
                }
                else
                {
                    MessageBox.Show("This not an youtube vidoe URL !!!",
                    "URL Not found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                    link.Text = "";
                }
            }
            else MessageBox.Show("There is no internet please cheack you connection first",
                 "Connection Problem",
                 MessageBoxButton.OK,
                 MessageBoxImage.Error);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveTheImage();
            PoupUpNotification();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            link.Text = "";
            mainImage.Source = null;
            clearButton.IsEnabled = false;
            saveButton.IsEnabled = false;
            previewButton.IsEnabled = false;
        }
        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(imageLink))
                Process.Start(new ProcessStartInfo("cmd", $"/c start {imageLink}") { CreateNoWindow = true });
        }
        #endregion
        
        
        #region util methods
        private void setEnbale(bool enable)
        {
            if (!enable)
            {
                searchButton.Content = " Searching.... ";
                searchButton.IsEnabled = enable;
            }
            else
                searchButton.Content = " DONE!!! ";

            link.IsEnabled = enable;
            previewButton.IsEnabled = enable;
            saveButton.IsEnabled = enable;
            
            clearButton.IsEnabled = enable;
        }
        void SetImage(string link)
        {

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(link);
            image.EndInit();
            mainImage.Source = image;
            

        }

        void StartTheTimer()
        {
            int i = 11;
            Timer timer = new Timer();
            timer.Interval = 1000;
            searchButton.IsEnabled = false;
            timer.Elapsed += (sender, ev) =>
            {
                searchButton.Dispatcher.Invoke(() =>
                {
                    searchButton.Content = $"Please wait {--i}s to try again";
                    if (i == 0)
                    {
                        searchButton.IsEnabled = true;
                        timer.Stop();
                        searchButton.Content = " Search ";
                    }

                });
            };
            timer.Start();

        }

        

        private void PoupUpNotification()
        {
            
            PopupNotifier notifier = new PopupNotifier();
            notifier.AnimationDuration = 500;
            notifier.TitleText = "Photo Saved";
            notifier.ContentText = "Your photo has been saved successful in the desktop";
            notifier.Popup();
        }
        private void SaveTheImage()
        {
            mainStage.Cursor = Cursors.Wait;
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "image.png");
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)mainImage.Source));
            using (var stream = new FileStream(path, FileMode.Create))
                encoder.Save(stream);
            mainStage.Cursor = null;
        }
        #endregion
    }
}

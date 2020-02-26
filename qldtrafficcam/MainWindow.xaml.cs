using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace qldtrafficcam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Task<Webcams.RootObject> camdata = WebcamApi.GetCamData();
            Bindlist(camdata);
            DispatcherTimer webcamTimer = SetupTimer();
        }
        
        private void SetImage(string url)
        {
            //code adapted from https://stackoverflow.com/a/30164258
            var webImage = new BitmapImage();

            webImage.BeginInit();
            webImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            webImage.CacheOption = BitmapCacheOption.OnLoad;
            webImage.UriSource = new Uri(url);
            webImage.EndInit();
            WebcamImage.Source = webImage;
        }

        private void CamListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Webcams.Feature currentWebcam = (Webcams.Feature)CamListBox.SelectedItem;
            SetImage(currentWebcam.properties.image_url);
        }

        private async void Bindlist(Task<Webcams.RootObject> camdata)
        {
            var cam = await camdata;
            CamListBox.ItemsSource = cam.features;
            CamListBox.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("properties.description",
                System.ComponentModel.ListSortDirection.Ascending));
        }

        private DispatcherTimer SetupTimer()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Timer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
            return dispatcherTimer;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Webcams.Feature currentWebcam = (Webcams.Feature)CamListBox.SelectedItem;
            SetImage(currentWebcam.properties.image_url);
        }
    }

    class WebcamApi
    {
        static public async Task<Webcams.RootObject> GetCamData()
        {
            string url = "https://api.qldtraffic.qld.gov.au/v1/webcams";
            string json = await DownloadPageAsync(url);
            return JsonConvert.DeserializeObject<Webcams.RootObject>(json);
        }

        static public async Task<string> DownloadPageAsync(string url)
        {
            HttpClient client = new HttpClient();
            return await client.GetStringAsync(url);
        }
    }


    class Webcams
    //Class generated using http://json2csharp.com/
    {
        public class Geometry
        {
            public string type { get; set; }
            public List<double> coordinates { get; set; }
        }

        public class Properties
        {
            public int id { get; set; }
            public string url { get; set; }
            public string description { get; set; }
            public string direction { get; set; }
            public string district { get; set; }
            public string locality { get; set; }
            public string postcode { get; set; }
            public string image_url { get; set; }
            public object image_sourced_from { get; set; }
            public bool isCustom { get; set; }
            public object extra_info { get; set; }
        }

        public class Feature
        {
            public string type { get; set; }
            public Geometry geometry { get; set; }
            public Properties properties { get; set; }
        }

        public class Rights
        {
            public string owner { get; set; }
            public string disclaimer { get; set; }
            public string copyright { get; set; }
        }

        public class RootObject
        {
            public string type { get; set; }
            public List<Feature> features { get; set; }
            public DateTime published { get; set; }
            public Rights rights { get; set; }
        }
    }
}

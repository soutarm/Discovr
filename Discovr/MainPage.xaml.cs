using System;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Discovr.Classes.Core;
using Discovr.Classes.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using Map = Discovr.Classes.UI.Map;

namespace Discovr
{
    public partial class MainPage
    {
        private bool _resetZoom = true;
        private bool _currentlySearching = false;

        private Discovr.Classes.UI.Location _friendLocation;
        private const string UserAvatar = "https://lh4.googleusercontent.com/-JPlUHXAPbNM/AAAAAAAAAAI/AAAAAAAAAAA/8T5QipTwzqA/s49-c/photo.jpg";
        
        private Geolocator Geolocator;
        private Animation Animation;
        private AppSettings AppSettings;
        private readonly ScheduledBackgroundAgent _backgroundAgent;
        private Map Map;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _backgroundAgent = new ScheduledBackgroundAgent();

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += timer_Tick;

            timer.Start();
        }

        protected void timer_Tick(object sender, EventArgs e)
        {
            RefreshLocation();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            AppSettings = new AppSettings();

            if (AppSettings.LocationConsent == false)
            {
                var result = MessageBox.Show("This app accesses your phone's location. Is that ok?", "Location", MessageBoxButton.OKCancel);
                AppSettings.LocationConsent = result == MessageBoxResult.OK;
            }

            Map = new Map(MainMap);

            ToggleMetric.IsChecked = AppSettings.ShowMetric;

            // Ensure our background agent is runnin'
            if (!_backgroundAgent.IsRunning())
            {
                _backgroundAgent.StartPeriodicAgent();
            }
            ToggleAgent.IsChecked = _backgroundAgent.IsRunning();

            // Center the map where the we last saw the user...
            MainMap.Center = new GeoCoordinate(AppSettings.LastKnownLatitude, AppSettings.LastKnownLongitude);

            // Check if we have a friend's pin to show too!
            string strItemIndex;
            if (NavigationContext.QueryString.TryGetValue("Location", out strItemIndex))
            {
                _friendLocation = new Location();
                _friendLocation = _friendLocation.DataStringToLocation(strItemIndex);
            }

            Geolocator = new Geolocator();

            Animation = new Animation(false, false);

            RefreshLocation();
        }

        private void RefreshLocation()
        {
            LoadingBar.Opacity = 1.0;
            ShowLocationOnMap();
        }

        private async void ShowLocationOnMap()
        {
            if (_currentlySearching) return;

            if (AppSettings.LocationConsent == false)
            {
                // The user has opted out of Location.
                LoadingBar.Opacity = 0;
                return;
            }

            _currentlySearching = true;

            var maximumAge = TimeSpan.FromSeconds(10);
            var timeOut = TimeSpan.FromSeconds(5);
            Geolocator.DesiredAccuracyInMeters = 4000;

            // If we aleady have a pin, let's see if we can be more accurate
            if (Map.UserLayer != null)
            {
                Geolocator.DesiredAccuracy = PositionAccuracy.High;
                timeOut = TimeSpan.FromSeconds(20);
            }

            try
            {
                var geoposition = await Geolocator.GetGeopositionAsync(maximumAge, timeOut);
                var userLocation = GeoConverter.ConvertGeocoordinate(geoposition.Coordinate);

                MainMap.Center = userLocation;
                if (!double.IsNaN(userLocation.Course))
                {
                    MainMap.Heading = userLocation.Course;
                }

                MainMap.Pitch = GeoConverter.GetPitchFromSpeed(userLocation.Speed);
                
                Pushpin pin = null;

                if (Map.UserLayer != null && Map.UserLayer.Any())
                {
                    var overlay = Map.UserLayer[0];
                    if (overlay != null)
                    {
                        pin = (Pushpin) overlay.Content;
                        overlay.GeoCoordinate = userLocation;
                    }
                }
                else
                {
                    Map.UserLayer = new MapLayer();
                    MainMap.Layers.Add(Map.UserLayer);
                }

                var extraInfo = !double.IsNaN(userLocation.Speed) ? GeoConverter.MetersPerSecondToReadableString(userLocation.Speed, AppSettings.ShowMetric) :
                string.Format("{0} accuracy", GeoConverter.MetersToReadableString(geoposition.Coordinate.Accuracy, AppSettings.ShowMetric));

                Map.AddOrUpdatePinToLayer(userLocation, userLocation, string.Format("You are here!{0}{1}", Environment.NewLine, extraInfo), UserAvatar, Map.UserLayer, false, pin, (Brush)Resources["PhoneAccentBrush"], AppSettings.ShowMetric);

                if (_friendLocation != null)
                {
                    var friendCoordinate = new GeoCoordinate(_friendLocation.Latitude, _friendLocation.Longitude);
                    var distanceToFriend = Map.AddOrUpdatePinToLayer(userLocation, friendCoordinate, _friendLocation.Label, string.Empty, Map.UserLayer, true, null, null, AppSettings.ShowMetric);

                    // Center the map on the friend and zoom out enough to see both
                    if (_resetZoom)
                    {
                        MainMap.Center = friendCoordinate;
                        MainMap.ZoomLevel = GeoConverter.GetZoomLevelFromMeters(distanceToFriend);
                        _resetZoom = false;
                    }
                }
                else if (_resetZoom)
                {
                    MainMap.ZoomLevel = GeoConverter.GetZoomLevelFromMeters(geoposition.Coordinate.Accuracy);
                }

                AppSettings.LastKnownLatitude = geoposition.Coordinate.Latitude;
                AppSettings.LastKnownLongitude = geoposition.Coordinate.Longitude;

                LoadingBar.Opacity = 0;
                _currentlySearching = false;
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    MessageBox.Show("Location is disabled on this device. Please enable Location in your phone settings so we can find where you are.", "Location Disabled", MessageBoxButton.OKCancel);
                }

                LoadingBar.Opacity = 0;
                _currentlySearching = false;
            }
        }

        private void Refreshbutton_Click(object sender, EventArgs e)
        {
            RefreshLocation();
        }

        private void Road_Click(object sender, EventArgs args)
        {
            MainMap.CartographicMode = MapCartographicMode.Road;
        }

        private void Aerial_Click(object sender, EventArgs args)
        {
            MainMap.CartographicMode = MapCartographicMode.Aerial;
        }

        private void Hybrid_Click(object sender, EventArgs args)
        {
            MainMap.CartographicMode = MapCartographicMode.Hybrid;
        }

        private void Terrain_Click(object sender, EventArgs args)
        {
            MainMap.CartographicMode = MapCartographicMode.Terrain;
        }

        private void ZoomIn_Click(object sender, EventArgs e)
        {
            var zoomInIcon = FindAppBarIconButtonByIconName("new.png");
            var zoomOutIcon = FindAppBarIconButtonByIconName("minus.png");

            zoomOutIcon.IsEnabled = true;

            if (MainMap.ZoomLevel < 20)
            {
                MainMap.ZoomLevel = MainMap.ZoomLevel + 1;
            }

            if (MainMap.ZoomLevel == 20)
            {
                zoomInIcon.IsEnabled = false;
            }

            _resetZoom = false;
        }

        private void ZoomOut_Click(object sender, EventArgs e)
        {
            var zoomInIcon = FindAppBarIconButtonByIconName("new.png");
            var zoomOutIcon = FindAppBarIconButtonByIconName("minus.png");

            zoomInIcon.IsEnabled = true;
            if(MainMap.ZoomLevel > 1)
            {
                MainMap.ZoomLevel = MainMap.ZoomLevel - 1;
            }

            if (MainMap.ZoomLevel == 1)
            {
                zoomOutIcon.IsEnabled = false;
            }

            _resetZoom = false;
        }

        public ApplicationBarIconButton FindAppBarIconButtonByIconName(string nameToFind)
        {
            return ApplicationBar.Buttons.Cast<ApplicationBarIconButton>().FirstOrDefault(testButton => testButton.IconUri.ToString().Contains(nameToFind));
        }

        public ProgressBar FindProgressBarByName(string nameToFind)
        {
            return FriendsPanel.Children.OfType<ProgressBar>().FirstOrDefault(l => l.Name == nameToFind);
        }

        private void ShowSettings_Click(object sender, RoutedEventArgs e)
        {
            ToggleSettings();
        }

        private void ShowLayers_Click(object sender, RoutedEventArgs e)
        {
            ToggleLayers();
        }

        private void ToggleSettings()
        {
            Animation.SlidePanel(MainGrid, true);
            ApplicationBar.Mode = Animation.SettingsOpen ? ApplicationBarMode.Minimized : ApplicationBarMode.Default;
        }

        private void ToggleLayers()
        {
            Animation.SlidePanel(MainGrid, false);
            ApplicationBar.Mode = Animation.LayersOpen ? ApplicationBarMode.Minimized : ApplicationBarMode.Default;
        }

        private void ClearLayer(string layerName)
        {
            var layer = Map.FindOrAddLayerByName(layerName);
            Map.ClearLayer(layer);
        }

        private async void ToggleLayer_Checked(object sender, RoutedEventArgs e)
        {
            var result = await Map.ShowFriendsOnMap((ToggleSwitch) sender, (Pushpin)this.FindName(Map.UserPinName));
            if(result) _resetZoom = false;
        }

        private void ToggleLayer_Unchecked(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;
            ClearLayer(toggleSwitch.LayerName());
            _resetZoom = true;
        }

        private void ToggleMetric_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.ShowMetric = true;
        }
        private void ToggleMetric_Unchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.ShowMetric = false;
        }

        private void ToggleAgent_Checked(object sender, RoutedEventArgs e)
        {
            _backgroundAgent.StartPeriodicAgent();
        }

        private void ToggleAgent_Unchecked(object sender, RoutedEventArgs e)
        {
            _backgroundAgent.RemoveAgent();
        }
    }
}
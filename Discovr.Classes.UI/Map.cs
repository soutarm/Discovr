using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Discovr.Classes.Core;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;

namespace Discovr.Classes.UI
{
    public class Map
    {
        public const string UserPinName = "UserPushPin";
        public MapLayer UserLayer { get; set; }

        private Dictionary<int, string> LayerNameAndIndex { get; set; }
        private Microsoft.Phone.Maps.Controls.Map MainMap { get; set; }

        public Map(Microsoft.Phone.Maps.Controls.Map mainMap)
        {
            LayerNameAndIndex = new Dictionary<int, string>();
            MainMap = mainMap;
        }

        public MapLayer FindOrAddLayerByName(string layerName)
        {
            MapLayer tagLayer = null;

            if (LayerNameAndIndex.Any() && LayerNameAndIndex.Values.Contains(layerName))
            {
                var existingLayerIndex = (int)LayerNameAndIndex.FirstOrDefault(l => l.Value == layerName).Key;
                tagLayer = MainMap.Layers[existingLayerIndex];
            }

            if (tagLayer == null)
            {
                tagLayer = new MapLayer();
                MainMap.Layers.Add(tagLayer);

                LayerNameAndIndex.Add(MainMap.Layers.Count - 1, layerName);
            }

            return tagLayer;
        }

        public void ClearLayer(MapLayer layer)
        {
            if (layer != null) layer.Clear();
        }

        public async Task<bool> ShowFriendsOnMap(ToggleSwitch toggleSwitch, Pushpin userPin)
        {
            const int myEntityId = 1;
            var furthestAwayFriend = 1.0;

            var switchName = toggleSwitch.Content.ToString();
            var layerName = toggleSwitch.LayerName();

            toggleSwitch.Opacity = 0.8;

            var tagLayer = this.FindOrAddLayerByName(layerName);
            
            var results = await Discovr.Classes.Core.LocationEntity.GetFriends(myEntityId, switchName);

            if (results != null)
            {
                ClearLayer(tagLayer);

                var pointToCompareTo = userPin != null && userPin.GeoCoordinate != null ? userPin.GeoCoordinate : MainMap.Center;
                var newColor = Extensions.GetRandomColor();
                toggleSwitch.SwitchForeground = newColor;

                foreach (var result in results)
                {
                    var distanceToFriend = Map.AddOrUpdatePinToLayer(pointToCompareTo, new GeoCoordinate(result.Latitude, result.Longitude), result.EntityLabel, result.AvatarUrl, tagLayer, true, null, newColor, new AppSettings().ShowMetric);
                    furthestAwayFriend = distanceToFriend > furthestAwayFriend ? distanceToFriend : furthestAwayFriend;
                }

                MainMap.ZoomLevel = GeoConverter.GetZoomLevelFromMeters(furthestAwayFriend);

                toggleSwitch.Opacity = 1;
            }

            return true;
        }

        /// <summary>
        /// Creates a new PushPin, adds it to the layer then returns the distance to the friend.
        /// </summary>
        public static double AddOrUpdatePinToLayer(GeoCoordinate pointToCompareTo, GeoCoordinate friendLocation, string friendLabel, string avatarUrl, MapLayer mapLayer, bool isFriend, Pushpin existingPin, Brush pinColor, bool showMetric)
        {
            var newPin = existingPin ?? new Pushpin();
            newPin.Name = isFriend ? "Pin_" + friendLabel : UserPinName;
            if (pinColor != null) newPin.Background = pinColor;

            // This will hold our text and image
            var pinPanel = new StackPanel();
            newPin.GeoCoordinate = friendLocation;
            var distanceToFriend = newPin.GeoCoordinate.GetDistanceTo(pointToCompareTo);

            // Define the image to use as the pushpin icon.
            var pinImage = new Image();
            pinImage.Source = !String.IsNullOrEmpty(avatarUrl) ? new BitmapImage(new Uri(avatarUrl, UriKind.Absolute)) : new BitmapImage(new Uri("https://lh3.googleusercontent.com/-2cAEJfwgJU0/AAAAAAAAAAI/AAAAAAAAAAA/2qq4qKqgmlA/s80-c-k-no/photo.jpg", UriKind.Absolute));
            pinImage.HorizontalAlignment = HorizontalAlignment.Left;
            pinImage.VerticalAlignment = VerticalAlignment.Top;
            pinImage.Height = 56;
            pinImage.Width = 56;

            pinPanel.Children.Add(pinImage);

            var textContent = new TextBlock();
            textContent.HorizontalAlignment = HorizontalAlignment.Left;
            textContent.VerticalAlignment = VerticalAlignment.Top;
            textContent.Margin = new Thickness(65, -56, 5, 0);

            if (isFriend)
            {
                textContent.Text = String.Format("{0} is here!", friendLabel) + Environment.NewLine + GeoConverter.MetersToReadableString(distanceToFriend, showMetric) + " from you";
            }
            else
            {
                textContent.Text = friendLabel;
            }

            pinPanel.Children.Add(textContent);

            newPin.Content = pinPanel;

            if (existingPin == null)
            {
                var pinOverlay = new MapOverlay();
                pinOverlay.Content = newPin;
                pinOverlay.GeoCoordinate = friendLocation;
                pinOverlay.PositionOrigin = new Point(0, 1);

                mapLayer.Add(pinOverlay);
            }

            return distanceToFriend;
        }

    }
}

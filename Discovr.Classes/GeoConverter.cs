using System;
using System.Device.Location;
using Windows.Devices.Geolocation;

namespace Discovr.Classes.Core
{
    public static class GeoConverter
    {
        public static GeoCoordinate ConvertGeocoordinate(Geocoordinate geocoordinate)
        {
            return new GeoCoordinate
                (
                geocoordinate.Latitude,
                geocoordinate.Longitude,
                geocoordinate.Altitude ?? Double.NaN,
                geocoordinate.Accuracy,
                geocoordinate.AltitudeAccuracy ?? Double.NaN,
                geocoordinate.Speed ?? Double.NaN,
                geocoordinate.Heading ?? Double.NaN
                );
        }

        public static string MetersToReadableString(double inputMeters, bool showMetric)
        {
            const int maxMeters = 899;
            const int kilometer = 1000;

            const int maxFeet = 999;
            const double metersPerFoot = 0.3048;
            const double feetPerMile = 5280;

            if (inputMeters < 0) return "unknown";

            if (showMetric)
            {
                if (inputMeters >= maxMeters)
                {
                    return (inputMeters/kilometer).ToString("#,##0.##km");
                }

                return inputMeters.ToString("0m");
            }

            var feet = inputMeters * metersPerFoot;

            if (feet >= maxFeet)
            {
                return (feet / feetPerMile).ToString("#,##0.##m");
            }

            return feet.ToString("0ft");
        }

        public static string MetersPerSecondToReadableString(double metersPerSecond, bool showMetric)
        {
            const double mpsInKph = 3.6;
            const double mpsInMph = 2.23693629;

            return showMetric ? string.Format("{0:0.##}kph", metersPerSecond*mpsInKph) : string.Format("{0:0.##}mph", metersPerSecond*mpsInMph);
        }

        public static int GetZoomLevelFromMeters(double meters)
        {
            return meters > 2000 ? 13 : meters > 1500 ? 14 : meters > 1000 ? 15 : meters > 500 ? 16 : meters > 100 ? 17 : 19;
        }

        public static double GetPitchFromSpeed(double speed)
        {
            if (double.IsNaN(speed)) return 0;
            var pitch = speed*3;
            return pitch > 75 ? 75 : pitch;
        }
    }
}
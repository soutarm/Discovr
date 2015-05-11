using System;
using System.Device.Location;
using Windows.Devices.Geolocation;

namespace Discovr.Classes
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

        public static string MetersToReadableString(double inputMeters)
        {
            const int maxMeters = 899;
            const int kilometer = 1000;

            if (inputMeters < 0) return "unknown";

            if (inputMeters >= maxMeters)
            {
                return (inputMeters/kilometer).ToString("0.#km");
            }

            return inputMeters.ToString("0.#m");
        }

        public static int GetZoomLevelFromMeters(double meters)
        {
            return meters > 200000 ? 3 : meters > 100000 ? 5 : meters > 20000 ? 11 : meters > 5000 ? 12 : meters > 2000 ? 13 : meters > 1500 ? 14 : meters > 1000 ? 15 : meters > 500 ? 16 : meters > 100 ? 17 : 19;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discovr.Classes
{
    class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Label { get; set; }
        public char DataDelimiter = ',';

        public Location DataStringToLocation(string dataString)
        {
            var data = dataString.Split(DataDelimiter);

            var location = new Location();
            location.Latitude = data.Length > 0 ? double.Parse(data[0]) : 0;
            location.Longitude = data.Length > 1 ? double.Parse(data[1]) : 0;
            location.Label = data.Length > 2 ? data[2] : string.Empty;

            return location;
        }
    }
}

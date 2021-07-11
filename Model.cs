using System;

namespace LocationApi
{
    public class Location
    {
        public string UserName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class ProximityLocation : Location
    {
        public double MilesAway { get; set; }
    }
}

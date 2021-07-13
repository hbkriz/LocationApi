using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LocationHistoryApi.Models
{
    public class Location : CreateLocation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateLocation
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

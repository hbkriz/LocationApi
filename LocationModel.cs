using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class LocationModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserName { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public DateTime CreatedDate { get; set; }
}
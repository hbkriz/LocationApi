using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CreateModel
{
    public string UserName { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}
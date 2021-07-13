using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using LocationHistoryApi.Models;
using LocationHistoryApi.Helper;

namespace LocationHistoryApi.Repository
{
    public interface ILocationRepository
    {
        List<Location> SelectAll();
        Location Create(Location model);
        List<Location> GetHistory(string userName);
        Location GetCurrent(string userName);
        List<Location> GetAllCurrent();
        List<ProximityLocation> GetCurrentProximity(double startLatitude, double startLongitude, double proximity);
    }

    public class LocationRepository : ILocationRepository
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected IMongoCollection<Location> _collection;

        public LocationRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("location");
            _collection = _database.GetCollection<Location>("location_history");
        }

        public List<Location> SelectAll()
        {
            var query = this._collection.Find(new BsonDocument()).ToListAsync();
            return query.Result;
        }

        public Location Create(Location model)
        {
            _collection.InsertOne(model);
            return model;
        }

        public List<Location> GetHistory(string userName)
        {
            return this._collection.Find(new BsonDocument { { "UserName", userName } }).ToListAsync().Result.OrderByDescending(i => i.CreatedDate).ToList();
        }

        public Location GetCurrent(string userName)
        {
            return GetHistory(userName).FirstOrDefault();
        }

        public List<Location> GetAllCurrent()
        {
            var result = this._collection.Find(new BsonDocument()).ToList();
            
            var joinQuery = (from n in result
                            group n by n.UserName into g
                            select g.OrderByDescending(t=>t.CreatedDate).FirstOrDefault()).ToList();
        
            return joinQuery;
        }

        public List<ProximityLocation> GetCurrentProximity(double startLatitude, double startLongitude, double proximity)
        {
            var result = GetAllCurrent();

            return result.Select(i => new ProximityLocation {
                Id = i.Id,
                Latitude = i.Latitude, 
                Longitude = i.Longitude,
                UserName = i.UserName, 
                MilesAway = Math.Round(new Coordinates(startLatitude, startLongitude)
                .DistanceTo(
                    new Coordinates(i.Latitude, i.Longitude),
                    UnitOfLength.Miles
                ), 2 ,MidpointRounding.AwayFromZero)
            }).Where(m => m.MilesAway <= proximity).ToList();
        }
    }
}

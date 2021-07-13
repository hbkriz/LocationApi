using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using System.Linq;
using System;

namespace LocationApi
{
    public interface IMongoRepository
    {
        List<LocationModel> SelectAll();
        LocationModel Create(LocationModel model);
        List<LocationModel> GetHistory(string userName);
        LocationModel GetCurrent(string userName);
        List<LocationModel> GetAllCurrent();
        List<ProximityLocation> GetCurrentProximity(double startLatitude, double startLongitude, double proximity);
    }

    public class MongoRepository : IMongoRepository
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected IMongoCollection<LocationModel> _collection;

        public MongoRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("location");
            _collection = _database.GetCollection<LocationModel>("location_history");
        }

        public List<LocationModel> SelectAll()
        {
            var query = this._collection.Find(new BsonDocument()).ToListAsync();
            return query.Result;
        }

        public LocationModel Create(LocationModel model)
        {
            _collection.InsertOne(model);
            return model;
        }

        public List<LocationModel> GetHistory(string userName)
        {
            return this._collection.Find(new BsonDocument { { "UserName", userName } }).ToListAsync().Result.OrderByDescending(i => i.CreatedDate).ToList();
        }

        public LocationModel GetCurrent(string userName)
        {
            return GetHistory(userName).FirstOrDefault();
        }

        public List<LocationModel> GetAllCurrent()
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
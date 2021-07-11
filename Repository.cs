using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper;
using System.Data.SQLite;
using System.Linq;

namespace LocationApi
{
    public interface IRepository
    {
        Task Add(Location item);
        Task<IEnumerable<Location>> GetHistory(string userName);
        Task<Location> GetCurrent(string userName);
        Task<IEnumerable<Location>> GetCurrent();
        Task<IEnumerable<Location>> GetAll();
        Task<List<ProximityLocation>> GetCurrentProximity(double startLatitude, double startLongitude, double proximity);
    }

    public class Repository : IRepository
    {
        private string DbConfig
        {
            get { return "Data Source=" + Environment.CurrentDirectory + "\\LocationDb.sqlite"; }
        }
        
        public Repository()
        {
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            using var connection = new SQLiteConnection(DbConfig);
            var table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'LocationHistory';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "LocationHistory")
                return;

            connection.Execute("Create Table LocationHistory (" +
                "UserName NVARCHAR(100) NOT NULL," +
                "Latitude Decimal(8,6) NOT NULL," +
                "Longitude Decimal(9,6) NOT NULL," +
                "CreatedDate datetime NOT NULL);");
        }
        
        //1. Receive a location update for a user
        public async Task Add(Location item)
        {
            using var connection = new SQLiteConnection(DbConfig);

            await connection.ExecuteAsync("INSERT INTO LocationHistory (UserName, Latitude, Longitude, CreatedDate)" +
                "VALUES (@UserName, @Latitude, @Longitude, datetime('now'));", item);
        }

        //2. Return the current location for a specified user
        public async Task<Location> GetCurrent(string userName)
        {
            using var connection = new SQLiteConnection(DbConfig);

            var result = await connection.QueryAsync<Location>(@"SELECT UserName, Latitude, Longitude, CreatedDate
            FROM LocationHistory
            WHERE UserName = @userName ORDER BY CreatedDate DESC", new { userName });

            return result.FirstOrDefault();
        }

        //3. Return the location history for a specified user
        public async Task<IEnumerable<Location>> GetHistory(string userName)
        {
            using var connection = new SQLiteConnection(DbConfig);

            var result = await connection.QueryAsync<Location>(@"SELECT UserName, Latitude, Longitude, CreatedDate
            FROM LocationHistory
            WHERE UserName = @userName ORDER BY CreatedDate DESC", new { userName });

            return result;
        }

        //4. Return the current location for all users
        public async Task<IEnumerable<Location>> GetCurrent()
        {
            using var connection = new SQLiteConnection(DbConfig);

            var result = await connection.QueryAsync<Location>(@"SELECT l1.*
            FROM LocationHistory l1 LEFT JOIN LocationHistory l2
            ON (l1.UserName = l2.UserName AND l1.CreatedDate < l2.CreatedDate)
            WHERE l2.CreatedDate IS NULL;");

            return result;
        }

        //5. Return the current location for all users within a specified area 
        public async Task<List<ProximityLocation>> GetCurrentProximity(double startLatitude, double startLongitude, double proximity)
        {
            var result = await GetCurrent();

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

        //Bonus
        public async Task<IEnumerable<Location>> GetAll()
        {
            using var connection = new SQLiteConnection(DbConfig);

            var result = await connection.QueryAsync<Location>(@"SELECT *
            FROM LocationHistory
            ORDER BY CreatedDate DESC");

            return result;
        }
    }
}

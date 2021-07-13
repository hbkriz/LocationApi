using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LocationHistoryApi.Repository;
using LocationHistoryApi.Models;

namespace LocationHistoryApi.Controllers
{
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILogger<LocationController> _logger;
        private readonly ILocationRepository _repository;

        public LocationController(ILogger<LocationController> logger, ILocationRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        [Route("get/history/{userName}")]
        public List<Location> GetHistory(string userName)
        {
            return _repository.GetHistory(userName);
        }

        [HttpGet]
        [Route("get/current/{userName}")]
        public Location GetCurrent(string userName)
        {
            return _repository.GetCurrent(userName);
        }

        [HttpGet]
        [Route("get/current")]
        public List<Location> GetAllCurrent()
        {
            return _repository.GetAllCurrent();
        }

        [HttpGet]
        [Route("get/all")]
        public List<Location> GetAll()
        {
            return _repository.SelectAll();
        }

        [HttpPost]
        [Route("create")]
        public Location Create(CreateLocation model)
        {
            var interpretedModel = new Location {
              UserName = model.UserName,
              Latitude  = model.Latitude,
              Longitude = model.Longitude,
              CreatedDate = DateTime.Now
            };
            return _repository.Create(interpretedModel);
        }

        [HttpGet]
        [Route("get/near-by-vicinity")]
        public List<ProximityLocation> GetNearByVicinity(double latitude, double longitude, double proximity)
        {
            return _repository.GetCurrentProximity(latitude, longitude, proximity);
        }
    }
}

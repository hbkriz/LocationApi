using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LocationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILogger<LocationController> _logger;
        private readonly IMongoRepository _mongoRepository;

        public LocationController(ILogger<LocationController> logger, IMongoRepository mongoRepository)
        {
            _logger = logger;
            _mongoRepository = mongoRepository;
        }

        [HttpGet]
        [Route("get/history/{userName}")]
        public List<LocationModel> GetHistory(string userName)
        {
            return _mongoRepository.GetHistory(userName);
        }

        [HttpGet]
        [Route("get/current/{userName}")]
        public LocationModel GetCurrent(string userName)
        {
            return _mongoRepository.GetCurrent(userName);
        }

        [HttpGet]
        [Route("get/current")]
        public List<LocationModel> GetAllCurrent()
        {
            return _mongoRepository.GetAllCurrent();
        }

        [HttpGet]
        [Route("get/all")]
        public List<LocationModel> GetAll()
        {
            return _mongoRepository.SelectAll();
        }

        [HttpPost]
        [Route("create")]
        public LocationModel Create(CreateModel model)
        {
            var interpretedModel = new LocationModel {
              UserName = model.UserName,
              Latitude  = model.Latitude,
              Longitude = model.Longitude,
              CreatedDate = DateTime.Now
            };
            return _mongoRepository.Create(interpretedModel);
        }

        [HttpGet]
        [Route("get/near-by-vicinity")]
        public List<ProximityLocation> GetNearByVicinity(double latitude, double longitude, double proximity)
        {
            return _mongoRepository.GetCurrentProximity(latitude, longitude, proximity);
        }
    }
}

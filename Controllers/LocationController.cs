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
        private readonly IRepository _repository;

        public LocationController(ILogger<LocationController> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        [Route("get/history/{userName}")]
        public async Task<IEnumerable<Location>> GetHistory(string userName)
        {
            return await _repository.GetHistory(userName);
        }

        [HttpGet]
        [Route("get/current/{userName}")]
        public async Task<Location> GetCurrent(string userName)
        {
            return await _repository.GetCurrent(userName);
        }

        [HttpGet]
        [Route("get/current")]
        public async Task<IEnumerable<Location>> GetAllCurrent()
        {
            return await _repository.GetCurrent();
        }

        [HttpGet]
        [Route("get/all")]
        public async Task<IEnumerable<Location>> GetAll()
        {
            return await _repository.GetAll();
        }

        [HttpPost]
        [Route("add")]
        public async Task Add(Location model)
        {
            await _repository.Add(model);
        }

        [HttpGet]
        [Route("get/near-by-vicinity")]
        public async Task<List<ProximityLocation>> GetNearByVicinity(double latitude, double longitude, double proximity)
        {
            return await _repository.GetCurrentProximity(latitude, longitude, proximity);
        }
    }
}

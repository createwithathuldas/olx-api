using Microsoft.AspNetCore.Mvc;
using olx_api.DTOs;
using olx_api.Repositories;

namespace olx_api.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepo;

        public LocationController(ILocationRepository locationRepo) =>
            _locationRepo = locationRepo;

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
        {
            var countries = await _locationRepo.GetCountriesAsync();
            return Ok(countries.Select(c => new CountryDto(c.Id, c.Name)));
        }
    }
}

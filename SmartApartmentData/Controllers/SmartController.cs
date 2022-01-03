using Microsoft.AspNetCore.Mvc;
using SmartApartmentData.API.Entities;
using SmartApartmentData.Domain.JsonReader;
using SmartApartmentData.Infrastructure.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SmartApartmentData.API.Controllers
{
    // GET / smart

    [Route("api/[controller]")]
    [ApiController]
    public class SmartController : ControllerBase
    {
        private readonly IInfrastructureService _infrastructureService;
        private IJsonReader _jsonReader;

        public SmartController(IInfrastructureService infrastructureService, IJsonReader jsonReader)
        {
            _infrastructureService = infrastructureService;
            _jsonReader = jsonReader;
        }

        // GET /smart/Search
        [HttpGet("search")]
        public async Task<ActionResult> Search([FromBody] SearchModel search)
        {
            var managements = await _infrastructureService.SearchManagement(search.SearchString, search.Market);

            var properties = await _infrastructureService.SearchProperty(search.SearchString, search.Market);

            return CreatedAtAction(nameof(Search),
                new { Properties = properties.Hits.Select(h => h.Source).ToList(), 
                    Managements = managements.Hits.Select(h => h.Source).ToList() });
        }

        // GET /smart/CreateIndexes

        [HttpPost("CreateIndexes")]
        public ActionResult CreateIndexes()
        {
            // create indexes with analyzer
            _infrastructureService.createManagementsAnalyzers();
            _infrastructureService.createPropertiesAnalyzers();

            return CreatedAtAction(nameof(CreateIndexes), 
                new { Properties = "Indexed", Managements =  "Indexed"});
        }

        // POST /smart/uploaddata

        [HttpPost("UploadData")]
        public ActionResult UploadData()
        {
            // get data from json files
            var managements = _jsonReader.GetManagementData();
            var properties = _jsonReader.GetPropertyData();

            // create indexes for analysis
            var managementsIndexesCreated = _infrastructureService.ImportManagementData(managements);
            var propertiesIndexesCreated = _infrastructureService.ImportPropertyData(properties);

            var state = managementsIndexesCreated && propertiesIndexesCreated ? true : false;

            return CreatedAtAction(nameof(CreateIndexes),
                new
                {
                    Properties = propertiesIndexesCreated ? "Uploaded" : "Upload Failed",
                    Managements = managementsIndexesCreated ? "Uploaded" : "Upload Failed"
                });
        }

        // DELETE /smart/RemoveIndexes
        [HttpDelete("RemoveIndexes")]
        public ActionResult RemoveIndexes()
        {
            return NoContent();
        }
    }
}

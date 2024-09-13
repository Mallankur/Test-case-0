using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MigrationWebAPPAPI.Interfacese;
using MigrationWebAPPAPI.Model;

namespace MigrationWebAPPAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class MigrationController : ControllerBase
    {
        private readonly ILogger<MigrationController> _logger;
        private readonly ICOSMOSConnector _CosmosServises;
        public MigrationController(ICOSMOSConnector CosmosServises, ILogger<MigrationController> logger)
        {
                _logger =logger;
            _CosmosServises = CosmosServises;
        }
        [HttpPost]
        [Route("PostMultiCycleMigrationAsync")]
        public async Task<IActionResult> PostMultiCycleMigrationAsync([FromBody] MultiCycleMigrationRequestModel requestModel)
        {
            List<int> cycleIds = requestModel.CycleIds;
            string rdids = requestModel.Rdids;
            _logger.LogInformation($"Data Processing in API {DateTime.Now}");
            _logger.LogInformation($"Data import Process started for Cycle:  {requestModel.CycleIds} {DateTime.Now}");


            var result = await _CosmosServises.CreateData_Using_SQL_SP_ConnectorAsync(cycleIds, rdids);
            _logger.LogInformation($"Data import process ended for Cycle: {requestModel.CycleIds} {DateTime.Now}");

            if (result)
            {

                return Ok("Data received and processed");

            }
            _logger.LogInformation($"Data Processing in API End {DateTime.Now}");

            return BadRequest(Errordto.ErrorMsg);



        }


         //[HttpGet]
         //public async Task<object>

      



    }
}

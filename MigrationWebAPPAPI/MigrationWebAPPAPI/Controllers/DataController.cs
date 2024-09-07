using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MigrationWebAPPAPI.Interfacese;
using MigrationWebAPPAPI.Model;
using MongoDB.Driver;
using System.Linq;

namespace MigrationWebAPPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase 
    {
        private readonly ILogger<DataController> _logger;
        private readonly ICOSMOSConnector _CosmosServises;
        private readonly IDataServises _serviceLayer;

        


        public DataController(ICOSMOSConnector CosmosServises,ILogger<DataController> _logger, IDataServises servises)
        {
               ;
                _logger = _logger ?? throw new ArgumentNullException("Datanotfind");
                _serviceLayer = servises;

        }

        //  

        [HttpGet]
        [Route("GetCSISDataBasedonTimeIntraval")]
        public async Task<IActionResult> GetCSISData(int CycleId, string interval, DateTime? startDate = null, DateTime? endDate = null, int? dosFrom = null, int? dosTo = null)
        {
            TimeInterval timeInterval;
            if (Enum.TryParse(interval, true, out timeInterval))
            {
                var data = await _serviceLayer.FetchDataByTimeInterval(CycleId,timeInterval);   
                return Ok(data);
            }
          
            else
            {
                return BadRequest("Invalid input parameters.");
            }
        }


        [HttpGet]
        [Route("GetCSISData")]
        public async Task<IActionResult>GetCSISDatabasedoninterval(int? Cycleid =null   , DateTime? startDate= null  , DateTime? endDate= null , int? dosFrom =null ,int? dosTo = null  )
        {
            CancellationToken cancellationToken = CancellationToken.None;
            if (startDate.HasValue && endDate.HasValue )
            {
                var data =  await _serviceLayer.FetchDataByDateRange(Cycleid.Value, startDate.Value, endDate.Value);
                if (data.Count()>0)
                {
                    return Ok(data);    
                }
                else
                {
                    return NotFound("No Data have been found for the given Time Interval");  
                }
            }
            else if (dosFrom.HasValue && dosTo.HasValue)
            {
               var data = await _serviceLayer.FetchDataByDosRange(Cycleid.Value, dosFrom.Value, dosTo.Value);
                var dataresult = data.Any()? data : throw new Exception(null);
                return Ok(dataresult);
            }

            else if (Cycleid.HasValue)
            {
                
               // var data = await _serviceLayer.FetchDataByCycleId(Cycleid.Value, cancellationToken);
               var data =  _serviceLayer.FetchDataByCycleIdAsync(Cycleid.Value,cancellationToken);   


                return Ok(data);
            }

            else
            {
                return  BadRequest("Invalid input parameters");
            }


          
        }

        [HttpPost]

        public async Task<IActionResult>GetCSISDataByrdied(
            RequestClass request )
        {
            var res = await _serviceLayer.FetchDataModelByRdiedobj(request.Cycleid, request.Listofrdied);
            return Ok(res); 
        
        
        
        }
    }

}

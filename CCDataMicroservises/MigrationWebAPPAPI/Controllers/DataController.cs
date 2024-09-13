using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MigrationWebAPPAPI.CCDataServises;
using MigrationWebAPPAPI.Interfacese;
using MigrationWebAPPAPI.Model;
using MigrationWebAPPAPI.MongoDataDto;
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
     




        public DataController(ICOSMOSConnector CosmosServises, ILogger<DataController> _logger, IDataServises servises)
        {
            ;
            _logger = _logger ?? throw new ArgumentNullException("Datanotfind");
            _serviceLayer = servises;
           

        }

        //  

        [HttpGet]
        [Route("GetCSISDataBasedonTimeIntraval")]
        public async Task<IActionResult> GetCSISData(int CycleId, DateTime? startDate = null, DateTime? endDate = null, int? dosFrom = null, int? dosTo = null)
        {
            //TimeInterval timeInterval;
            //if (Enum.TryParse(interval, true, out timeInterval))
            //{
            //    var data = await _serviceLayer.FetchDataByTimeInterval(CycleId, timeInterval);
            //    return Ok(data);
            //}

            if (dosFrom.HasValue && dosTo.HasValue)
            {
                var data =  await _serviceLayer.FetchDataByDosRange(CycleId,dosFrom.Value,dosTo.Value); 
                return Ok(data);    
            }

            else
            {
                return BadRequest(""); 
            }
        }


        [HttpGet]
        [Route("GetCSISData")]
        public async Task<IActionResult> GetCSISDatabasedoninterval(int? Cycleid = null, DateTime? startDate = null, DateTime? endDate = null, int? dosFrom = null, int? dosTo = null, List<string>? rdids = null)
        {
            CancellationToken cancellationToken = CancellationToken.None;
            if (startDate.HasValue && endDate.HasValue)
            {
                var data = await _serviceLayer.FetchDataByDateRange(Cycleid.Value, startDate.Value, endDate.Value);
                if (data.Count() > 0)
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

                var lst = new List<MangoDataDto>();

                foreach (var dtodata in data )
                {
                    var res = new MangoDataDto
                    {
                        //ApplicationId = dtodata.ApplicationId,
                        //Sapunitno = dtodata.Sapunitno,
                        //CycleId = dtodata.CycleId,
                        //DOS = dtodata.DOS,
                        //DM = dtodata.DM,
                        //DOSDATE = dtodata.DOSDATE,
                        //CatCheckConnectData = dtodata.CatCheckConnectData,
                        //CC_Fields_Defs_Id = dtodata.CC_Fields_Defs_Id,
                        //CSISValue = dtodata.CSISValue,
                        //ImputedValue = dtodata.ImputedValue,
                        //ImputedValueMetric = dtodata.ImputedValueMetric,
                        //CleansedValue = dtodata.CleansedValue,
                        //ValueMetric = dtodata.ValueMetric,
                        //IgnoreError = dtodata.IgnoreError,
                        //ImportedValue = dtodata.ImportedValue,
                        //CSISDataTypeId = dtodata.CSISDataTypeId,
                        //CSISPredictionId = dtodata.CSISPredictionId,
                        //CSISTestRunId = dtodata.CSISTestRunId,
                        //ReportDataEntityId = dtodata.ReportDataEntityId,
                        //Value = dtodata.Value,
                        //ImputedValueImperial = dtodata.ImputedValueImperial,
                        //EOMobileLabId = dtodata.EOMobileLabId,
                        //ReportDataHeaderId = dtodata.ReportDataHeaderId,
                        //Cycleno = dtodata.Cycleno,
                        //Mode = dtodata.Mode,







                    };
                    lst.Add(res);
                }
                         




                var dataresult = data.Any() ? data : throw new Exception(null);
                return Ok(dataresult);
            }

            else if (Cycleid.HasValue)
            {

                // var data = await _serviceLayer.FetchDataByCycleId(Cycleid.Value, cancellationToken);
                var data = _serviceLayer.FetchDataByCycleIdAsync(Cycleid.Value, cancellationToken);


                return Ok(data);
            }

            //else if (Cycleid.HasValue && rdids.Count > 0)
            //{
            //    var data = _serviceLayer.FetchDatabyReportDataEntityId(Cycleid.Value, rdids.);
            //    return Ok(data);
            //}

            else
            {
                return NotFound("Erroe");
            }






        }

        [HttpPost]

        public async Task<IActionResult> GetCSISDataByrdied(
            RdidsRequestModel request)
        {
            var res = await _serviceLayer.FetchDataModelByRdiedobj(request.CycleId, request.rdeids);
            return Ok(res);



        }


        [HttpPost]
        [Route("GetMultiCycleChartData")]

        public async Task<IActionResult> GetMultiCycleChartData(
            RdidsRequestModel request)

        {
            var res = await _serviceLayer.FetchDataModelMultiCycleRdied(request.listofCycleId, request.rdeids,request.Mode);
            return Ok(res);
        }

        [HttpPost]
        [Route("GetCSISDatabasedonReportDataEntityId")]
        public async Task<IActionResult> GetCSISDatabasedonReportDataEntityId([FromBody] RdidsRequestModel request ,int? Cycleid,DateTime? startDate = null, DateTime? endDate = null, int? dosFrom = null, int? dosTo = null)
        {
           CancellationToken cancellationToken = CancellationToken.None;

            if (startDate.HasValue && endDate.HasValue)
            {
                var data = await _serviceLayer.FetchDataByDateRange(Cycleid.Value, startDate.Value, endDate.Value);
                if (data.Count() > 0)
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
                var lst = new List<MangoDataDto>();

                foreach (var dtodata in data)
                {
                    var res = new MangoDataDto
                    {
                        







                    };
                    lst.Add(res);
                }
                var dataresult = lst;
                return Ok(dataresult);
            }

            else if (request.rdeids.Count == 0)
            {
                var data = await _serviceLayer.FetchDataByCycleId(Cycleid.Value, cancellationToken);
                return Ok(data);
            }

            else if (request.rdeids.Count > 0)
            {

                var data = await _serviceLayer.FetchDatabyReportDataEntityId(Cycleid.Value, request.rdeids);
                var lst = new List<MangoDataDto>();

               

                var dataresult = lst;
                return Ok(dataresult);

            
            }

            else
            {

                return StatusCode(500, $"An error occurred while fetching data: {"Message"}");

            }
            
        }

    }
}

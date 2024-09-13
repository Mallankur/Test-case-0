using Amazon.Runtime.Internal.Transform;
using Microsoft.Extensions.Options;
using MigrationWebAPPAPI.DbInfrastructure;
using MigrationWebAPPAPI.Interfacese;
using MigrationWebAPPAPI.Model;
using MigrationWebAPPAPI.MongoDataDto;
using MigrationWebAPPAPI.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

namespace MigrationWebAPPAPI.CCDataServises
{

    public class CCDataGetServises : IDataServises 

    {
        private readonly ILogger<CCDataGetServises> _logger;
        public IMongoCollection<MongoDataModel> _mongoCollections {  get; set; }
        private readonly IMongoDatabase _database;

        public CCDataGetServises(IOptions<MongoScoket> connect , ILogger<CCDataGetServises> logger )
        {
            MongoClient client = new MongoClient(connect.Value.ConnectionString);
            _database = client.GetDatabase(connect.Value.DatabaseName);
            _logger = logger;   

            
        }

        public async Task<List<MongoDataModel>> FetchDataByDateRange(int CycleID, DateTime startDate, DateTime endDate)
        {
            var matchingCollection = _database.GetCollection<MongoDataModel>($"Cycle_{CycleID}");
            var start_date = new DateTimeOffset(startDate).ToUnixTimeMilliseconds();
            long end_date = new DateTimeOffset(endDate).ToUnixTimeMilliseconds();

            var filter = Builders<MongoDataModel>.Filter.And(
                Builders<MongoDataModel>.Filter.Gte("DOSDATE", start_date),
                Builders<MongoDataModel>.Filter.Lte("DOSDATE", end_date)

                );
            var result = matchingCollection.Find(filter).ToList();
            return result;


        }


        public async Task<List<MangoDataDto>> FetchDataByDosRange(int CycleId,int dosFrom, int dosTo)
        {
            var timeTakenTofetchDatafromdosfromtodosto = Stopwatch.StartNew();
            var collectionlst =  await _database.ListCollectionNamesAsync().Result.ToListAsync();
            var matchingCollectionName = collectionlst.FirstOrDefault(x=>x.Contains($"Cycle_{CycleId}"));
            if (matchingCollectionName == null ) 
            {
                throw new Exception($"Collections with Cycle ID {CycleId} does not  exists.");
            }
             _mongoCollections =  _database.GetCollection<MongoDataModel>(matchingCollectionName);
            var filter = Builders<MongoDataModel>.Filter.And(
                Builders<MongoDataModel>.Filter.Gte("DOS", dosFrom),
                Builders<MongoDataModel>.Filter.Lte("DOS", dosTo)
                );
            var projection = Builders<MongoDataModel>.Projection
                .Include("ReportDataEntityId")
                .Include("ReportDataHeaderId")
                .Include("CSISValue")
                .Exclude("_id");
            var result = _mongoCollections.Find(filter)
                                          .Project(projection)
            .ToList()
         .Select(delegate (BsonDocument result)
         {
             long? reportDataEntityId = result.GetValue("ReportDataEntityId").IsBsonNull ? (long?)null : result.GetValue("ReportDataEntityId").ToInt64();
             int? reportDataHeaderId = result.GetValue("ReportDataHeaderId").IsBsonNull ? (int?)null : result.GetValue("ReportDataHeaderId").ToInt32();
             double? csisValue = result.GetValue("CSISValue").IsBsonNull ? (double?)null : result.GetValue("CSISValue").ToDouble();
             return new MangoDataDto
             {
                 ReportDataEntityId = reportDataEntityId,
                 ReportDataHeaderId = reportDataHeaderId,
                 CSISValue = csisValue


             };
         }).ToList();
             
        

         


            _logger.LogInformation("Total fetch the data from CosmosDB operation took: {ElapsedMilliseconds} ms", timeTakenTofetchDatafromdosfromtodosto.Elapsed);

            return result;



        }

        public async Task<List<MongoDataModel>> FetchDataByTimeInterval(int CycleId, TimeInterval interval)
        {
            var collection = _database.GetCollection<MongoDataModel>($"Cycle{CycleId}");
            var CansillationToken = new CancellationToken();
            if (collection == null)
            {
                throw new Exception($"Collection with Cycle Id {CycleId} is not supported.");
            }
            var start_Date = DateTime.UtcNow;
            switch (interval)
            {
                case  TimeInterval.Week:
                    start_Date = start_Date.AddDays(-7);
                    break;
                case TimeInterval.Month:
                    start_Date= start_Date.AddMonths(-1)
                        ; break; 
                case TimeInterval.ThreeMonth:
                    start_Date = start_Date.AddMonths(-3);
                    break;
                case TimeInterval.SixMonths:
                    start_Date = start_Date.AddMonths(-6);
                    break;
                case TimeInterval.Year:
                    start_Date = start_Date.AddYears(-1);
                    break;
                case TimeInterval.TwoYears:
                    start_Date = start_Date.AddYears(-2);
                    break;
                case TimeInterval.All:
                    FetchDataByCycleId(CycleId, CansillationToken);
                    break;
                default:
                    throw new Exception("Collection with Cycle ID Exist is not support by this time interval ");
                    break;
            }

            var filter = Builders<MongoDataModel>.Filter
                .Gte("DOSDATE", new DateTimeOffset(start_Date).ToUnixTimeMilliseconds());
             

            var result  =  await collection.Find(filter).ToListAsync ();
            return result;  

            


        }


        public async Task<List<MongoDataModel>> FetchDataByCycleId(int CycleId, CancellationToken cansillationToken)
        {

            var collectionList = await _database.ListCollectionNamesAsync().Result.ToListAsync(cansillationToken);
            var matchingCollectionName = collectionList.FirstOrDefault(x => x.Contains($"Cycle{CycleId}"));

            if (matchingCollectionName == null)
            {
                throw new Exception($"Collection with Cycle ID {CycleId} does not exist.");
            }


            _mongoCollections = _database.GetCollection<MongoDataModel>(matchingCollectionName);


            var filter = Builders<MongoDataModel>.Filter.Empty;
            var result = await _mongoCollections.Find(filter).ToListAsync();
            return result;
        }

     

        public async Task<List<object>> FetchDatabyReportDataEntityId(int CycleId, List<long> rdides)
        {
         
            var collectionList = await _database.ListCollectionNamesAsync().Result.ToListAsync();

            
            var matchingCollectionName = collectionList.FirstOrDefault(x => x.Contains($"Cycle{CycleId}"));

            if (matchingCollectionName == null)
            {
                throw new Exception($"Collection with Cycle ID {CycleId} does not exist.");
            }

          
            _mongoCollections = _database.GetCollection<MongoDataModel>(matchingCollectionName);

            
            var rdiIdsLong = rdides.Select(r => Convert.ToInt64(r)).ToList();

            
            var filter = Builders<MongoDataModel>.Filter.In("ReportDataEntityId", rdiIdsLong);

       
            var result = await _mongoCollections.Find(filter).ToListAsync();

            var groupedResults = result
               .GroupBy(x => new { x.DOS, x.DOSDATE })
               .Select(group => new
               {
                   DOS = group.Key.DOS,
                   DOSDATE = group.Key.DOSDATE,
                   ReportDataEntityValues = group.ToDictionary(
                       x => x.ReportDataEntityId,
                       x => x.CSISValue
                   )
               })
               .ToList();


            var formattedResults = groupedResults.Select(gr => new
            {
                gr.DOS,
                gr.DOSDATE,
                ReportDataEntityIdWithValues= gr.ReportDataEntityValues
            }).ToList();

            return formattedResults.Cast<object>().ToList();

            


        }
        public async Task<List<object>> FetchDataModelByRdiedobj(int cycleid, List<long> rdied)
        {
            
            var collectionList = await _database.ListCollectionNamesAsync().Result.ToListAsync();
            var matchingCollectionName = collectionList.FirstOrDefault(x => x.Contains($"Cycle_{cycleid}"));

            if (matchingCollectionName == null)
            {
                throw new Exception($"Collection with Cycle ID {cycleid} does not exist.");
            }

            _mongoCollections = _database.GetCollection<MongoDataModel>(matchingCollectionName);

           
            var filter = Builders<MongoDataModel>.Filter.In("ReportDataEntityId", rdied);
            var documents = await _mongoCollections.Find(filter).ToListAsync();


            var groupedResult = documents
                .GroupBy(doc => new { doc.DOS, doc.DOSDATE })
                .Select(g =>
                {
                    var result = new Dictionary<object, object>
                    {
                { "DOS", g.Key.DOS ?? 0 },
                { "DOSDATE", g.Key.DOSDATE ?? 0 }
                    };


                    foreach (var doc in g)
                    {
                        if (doc.ReportDataEntityId.HasValue)
                        {
                            result[doc.ReportDataEntityId.Value] = doc.CSISValue;
                        }
                    }

                    return (object)result;
                })
            .OrderBy(r => ((Dictionary<object, object>)r)["DOS"])
            .ToList();

            return groupedResult;
        }

      




        public async IAsyncEnumerable<MongoDataModel> FetchDataByCycleIdAsync(int cycleId, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
           
            var collectionList = await _database.ListCollectionNamesAsync().Result.ToListAsync(cancellationToken);
            var matchingCollectionName = collectionList.FirstOrDefault(x => x.Contains($"Cycle{cycleId}"));

            if (matchingCollectionName == null)
            {
                throw new Exception($"Collection with Cycle ID {cycleId} does not exist.");
            }

           
            var mongoCollection = _database.GetCollection<MongoDataModel>(matchingCollectionName);

            if (mongoCollection == null)
            {
                yield break; 
            }

            var filter = Builders<MongoDataModel>.Filter.Empty;
            int skip = 0;
            int limit = 100;

            while (!cancellationToken.IsCancellationRequested)
            {
             
                var currentPageResults = await mongoCollection.Find(filter)
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync(cancellationToken);

                if (currentPageResults.Count == 0)
                {
                    yield break; 
                }

              
                foreach (var item in currentPageResults)
                {
                    yield return item;
                }

               
                skip += limit;
            }
        }

        public object FetchDatabyReportDataEntityIdusingbsonobject(int cycleid, List<long> reportDataEntityId)
        {
            var matchingCollation = _database.GetCollection<MangoDataDto>($"Cycle{cycleid}");
            if (matchingCollation == null)
            {
                throw new Exception($"Collection with Cycle id {cycleid} does not exist.");
            }

            var rdidsLong = reportDataEntityId.Select(r => Convert.ToInt64(r)).ToList();
            var filter = Builders<MangoDataDto>.Filter.In("ReportDataEntityId", rdidsLong);

            var result = matchingCollation.Find(filter).ToList();


            return result;
        }



      
        public async Task<List<object>> FetchDataModelMultiCycleRdied(List<int> cycleIds, List<long> rdied, int mode)
        {
            var groupedResults = new List<object>();

            foreach (var cycleId in cycleIds)
            {

                var collectionList = await _database.ListCollectionNamesAsync().Result.ToListAsync();
                var matchingCollectionName = collectionList.FirstOrDefault(x => x.Contains($"Cycle_{cycleId}"));

                if (matchingCollectionName == null)
                {
                    throw new Exception($"Collection with Cycle ID {cycleId} does not exist.");
                }

                var mongoCollection = _database.GetCollection<MongoDataModel>(matchingCollectionName);

               
                var filterBuilder = Builders<MongoDataModel>.Filter;
                var filter = filterBuilder.In("ReportDataEntityId", rdied);

                if (mode != 0)
                {
                    filter = filter & filterBuilder.Eq("Mode", mode); 
                }

              
                var documents = await mongoCollection.Find(filter).ToListAsync();

               
                var groupedResult = documents
                    .GroupBy(doc => new { doc.DOS, doc.DOSDATE })
                    .Select(g =>
                    {
                        var result = new Dictionary<object, object>
                        {
                    { "DOS", g.Key.DOS ?? 0 },
                    { "DOSDATE", g.Key.DOSDATE ?? 0 }
                        };

                        foreach (var doc in g)
                        {
                            if (doc.ReportDataEntityId.HasValue)
                            {
                                result[doc.ReportDataEntityId.Value] = doc.CSISValue;
                            }
                        }

                        return (object)result;
                    })
                    .OrderBy(r => ((Dictionary<object, object>)r)["DOS"])
                    .ToList();

                groupedResults.AddRange(groupedResult);
            }

            return groupedResults;
        }



    }


}
    



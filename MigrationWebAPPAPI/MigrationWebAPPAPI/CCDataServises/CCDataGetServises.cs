using Microsoft.Extensions.Options;
using MigrationWebAPPAPI.DbInfrastructure;
using MigrationWebAPPAPI.Interfacese;
using MigrationWebAPPAPI.Model;
using MigrationWebAPPAPI.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MigrationWebAPPAPI.CCDataServises
{

    public class CCDataGetServises : IDataServises
    {
        public IMongoCollection<MongoDataModel > _mongoCollections {  get; set; }
        private readonly IMongoDatabase _database;

        public CCDataGetServises(IOptions<MongoScoket> connect)
        {
            MongoClient client = new MongoClient(connect.Value.ConnectionString);
            _database = client.GetDatabase(connect.Value.DatabaseName);

            
        }

        public async Task<List<MongoDataModel>> FetchDataByDateRange(int CycleID, DateTime startDate, DateTime endDate)
        {
            var matchingCollection =  _database.GetCollection<MongoDataModel>($"Cycle_{CycleID}");
             var start_date = new DateTimeOffset(startDate).ToUnixTimeMilliseconds();
             long end_date = new DateTimeOffset(endDate).ToUnixTimeMilliseconds();

            var filter = Builders<MongoDataModel>.Filter.And(
                Builders<MongoDataModel>.Filter.Gte("DOSDATE", start_date),
                Builders<MongoDataModel>.Filter.Lte("DOSDATE", end_date)

                );
            var result =  matchingCollection.Find( filter ).ToList();
            return result;


        }


        public async Task<List<MongoDataModel>> FetchDataByDosRange(int CycleId,int dosFrom, int dosTo)
        {
            var collectionlst =  await _database.ListCollectionNamesAsync().Result.ToListAsync();
            var matchingCollectionName = collectionlst.FirstOrDefault(x=>x.Contains($"Cycle{CycleId}"));
            if (matchingCollectionName == null ) 
            {
                throw new Exception($"Collections with Cycle ID {CycleId} does not  exists.");
            }
             _mongoCollections = _database.GetCollection<MongoDataModel>(matchingCollectionName);
            var filter = Builders<MongoDataModel>.Filter.And(
                Builders<MongoDataModel>.Filter.Gte("DOS", dosFrom),
                Builders<MongoDataModel>.Filter.Lte("DOS", dosFrom)
                );

            var result =await _mongoCollections.Find(filter).ToListAsync();
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

        public async Task<List<MongoDataModel>> FetchDataModelByRdied(int cycleid, List<long> rdied)
        {

            var collectionList = await _database.ListCollectionNamesAsync().Result.ToListAsync();
            var matchingCollectionName = collectionList.FirstOrDefault(x => x.Contains($"Cycle{cycleid}"));

            if (matchingCollectionName == null)
            {
                throw new Exception($"Collection with Cycle ID {cycleid} does not exist.");
            }


            _mongoCollections = _database.GetCollection<MongoDataModel>(matchingCollectionName);

            var filter = Builders<MongoDataModel>.Filter.In("ReportDataEntityId", rdied);
             var result = await _mongoCollections.Find(filter).ToListAsync();
            return result; 
        }
    }
}

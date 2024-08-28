using Microsoft.Extensions.Options;
using MigrationWebAPPAPI.DbInfrastructure;
using MigrationWebAPPAPI.Interfacese;
using MigrationWebAPPAPI.Model;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System.Diagnostics;

namespace MigrationWebAPPAPI.Repositories
{
    public class CosmosRepository :  ICOSMOSConnector
    {
        public IMongoCollection<MongoDataModel> _cosmosDynamicCollection { get; set; }
        private readonly ILogger<CosmosRepository> _logger;
        private readonly SQLRepository _Sqlrepo;
        private readonly IMongoDatabase _database;

        public CosmosRepository(IOptions<MongoScoket> connect, ILogger<CosmosRepository> logger, SQLRepository Sqlrepo)
        {
            _Sqlrepo = Sqlrepo;
            MongoClient client = new MongoClient(connect.Value.ConnectionString);
            _database = client.GetDatabase(connect.Value.DatabaseName);
            _logger = logger;
        }

        public async  Task<bool> CreateData_Using_SQL_SP_ConnectorAsync(List<int> CycleIds, string rdids)
        {
            bool isMigrated = false;
            var uniqueCycleIds = CycleIds.Distinct();

            foreach (var cycleId in uniqueCycleIds)
            {
                bool result = await CreateDynamicCycleAsync(cycleId, rdids);
                if (result)
                {
                    isMigrated = true;
                }
            }
            return isMigrated;
        }


        public async Task<bool> CreateDynamicCycleAsync(int Cycleid, string rdids)
        {
            bool IsInserted = false;
            var pushTOMongoStopwatch = Stopwatch.StartNew();
            string collectionName = "Cycle" + Cycleid.ToString();
            _cosmosDynamicCollection = _database.GetCollection<MongoDataModel>(collectionName);

            var indexKeysDefinitionBuilder = Builders<MongoDataModel>.IndexKeys;
            var indexModelList = new List<CreateIndexModel<MongoDataModel>>()
            {
                new CreateIndexModel<MongoDataModel>(indexKeysDefinitionBuilder.Ascending(x => x.ReportDataEntityId)),
                new CreateIndexModel<MongoDataModel>(indexKeysDefinitionBuilder.Ascending(x => x.ReportDataHeaderId)),
            };
            await _cosmosDynamicCollection.Indexes.CreateManyAsync(indexModelList);

            var sqlDataTableStreaming = await FetchAllCycleDataAsync(Cycleid, rdids);
            var kafkaConnector = Stopwatch.StartNew();
            foreach (var jsonData in sqlDataTableStreaming)
            {
                var streamDocuments = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<MongoDataModel>>(jsonData, new DateTimeToMillisecondConverter());
                if (streamDocuments.Any())
                {
                    await _cosmosDynamicCollection.InsertManyAsync(streamDocuments);
                    IsInserted = true;
                }
            }

            _logger.LogInformation("Total data push to MongoDB operation took: {ElapsedMilliseconds} ms", kafkaConnector.Elapsed);
            return IsInserted;
        }

        public async  Task<IEnumerable<string>> FetchAllCycleDataAsync(int CycleId, string rdids)
        {
            var multiCycleStreaming =  _Sqlrepo.FetchAllSQLMultiCycleDataAsync(_Sqlrepo._connectionString,  CycleId);
            return multiCycleStreaming;
        }
    }
}

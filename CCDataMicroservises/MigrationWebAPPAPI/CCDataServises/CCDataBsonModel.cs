using Microsoft.Extensions.Options;
using MigrationWebAPPAPI.DbInfrastructure;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MigrationWebAPPAPI.CCDataServises
{
    public class CCDataBsonModel 
    {
        private readonly  IMongoCollection<BsonDocument> _collection;   

        private readonly IMongoDatabase _database;
        public CCDataBsonModel(IOptions<MongoScoket> connect)
        {
            MongoClient client = new MongoClient(connect.Value.ConnectionString);
            _database = client.GetDatabase(connect.Value.DatabaseName);
        }

        //public async Task<object>> GetCSISDATAByChart()
        //{


        //}

    }
}

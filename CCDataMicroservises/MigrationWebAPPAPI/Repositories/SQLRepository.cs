using Microsoft.Extensions.Options;
using MigrationWebAPPAPI.DbInfrastructure;
using MigrationWebAPPAPI.Interfacese;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace MigrationWebAPPAPI.Repositories
{
    public class SQLRepository : ISQLConnector
    {
        public readonly string _connectionString;
        private readonly ILogger<SQLRepository> _logger;
        string storeProcdurename = "CC_Revamp_uspGetUMData";


        public SQLRepository(IOptions<SQLConnectorOptions> options, ILogger<SQLRepository> logger)
        {
            _connectionString = options.Value.ConnectionString;
            _logger = logger;
        }
        public async Task<IEnumerable<string>> FetchAllSQLBatchDataAsyn(string sqlConnectionString, int CycleId, string rdids)
        {
           
            var StoredproducerCall = Stopwatch.StartNew();
            var jsonResults = new List<string>();
            const int batchSize = 150000;
            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                SqlCommand sqlCommand = new SqlCommand(storeProcdurename, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
                sqlCommand.Parameters.AddWithValue("@RDIds", rdids);

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                await Task.Run(() => adapter.Fill(dataTable)).ConfigureAwait(false);
                // adapter.Fill(dataTable);
                
                var StoredproducerCallEnd = Stopwatch.StartNew();
                _logger.LogInformation($"Stored Procedure Call End for CycleID:{CycleId}" + "Total data fill operation took: {ElapsedMilliseconds} ms",
                    StoredproducerCall.ElapsedMilliseconds);

                int totalRows = dataTable.Rows.Count;
                int batches = (int)Math.Ceiling((double)totalRows / batchSize);

                for (int i = 0; i < batches; i++)
                {
                    int startIndex = i * batchSize;
                    int endIndex = Math.Min((i + 1) * batchSize, totalRows);

                    DataTable batchTable = dataTable.Clone();
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        batchTable.ImportRow(dataTable.Rows[j]);
                    }

                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(batchTable);

                    jsonResults.Add(jsonData);

                }
                return jsonResults;
            }
        }
        public string ReturnSPRdied(string sqlConnectionString, int CycleId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                sqlConnection.Open();

                var sqlCommand = new SqlCommand("uspGetCycleRDEIDs", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);

                var adapter = new SqlDataAdapter(sqlCommand);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                var jsonresult = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);
                return jsonresult;

            }
        }
        public IEnumerable<string> FetchAllSQLMultiCycleDataAsync(string sqlConnectionString,  int CycleId)
        {
            var resultReturnSprdids = ReturnSPRdied(sqlConnectionString, CycleId);


            //  var sqlAdapterLogger = Stopwatch.StartNew();

            const int batchSize = 201750;
            var rdidObjects = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(resultReturnSprdids);

            List<string> concatenatedValues = new List<string>();

            foreach (var rdidObject in rdidObjects)
            {
                var rdidValue = rdidObject["Column1"];
                concatenatedValues.Add(rdidValue);
                var sqlAdapterLogger = Stopwatch.StartNew();

                if (concatenatedValues.Count % 50 == 0)
                {
                    var concatenatedString = string.Join(",", concatenatedValues);

                    concatenatedValues.Clear();

                    using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
                    {
                        sqlConnection.Open();

                        SqlCommand sqlCommand = new SqlCommand(storeProcdurename, sqlConnection);
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
                        sqlCommand.Parameters.AddWithValue("@RDIds", concatenatedString);

                        SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        _logger.LogInformation($"Total Time Taken Filling The OPERATION{concatenatedString}" + "Total data fill operation took: {ElapsedMilliseconds}+StoredproducerCall.ElapsedMilliseconds"
                              , sqlAdapterLogger.ElapsedMilliseconds);



                        int totalRows = dataTable.Rows.Count;
                        int batches = (int)Math.Ceiling((double)totalRows / batchSize);

                        for (int i = 0; i < batches; i++)
                        {
                            int startIndex = i * batchSize;
                            int endIndex = Math.Min((i + 1) * batchSize, totalRows);

                            DataTable batchTable = dataTable.Clone();
                            for (int j = startIndex; j < endIndex; j++)
                            {
                                batchTable.ImportRow(dataTable.Rows[j]);
                            }

                            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(batchTable);

                            yield return jsonData;
                        }
                    }
                }
            }


            if (concatenatedValues.Any())
            {
                var concatenatedString = string.Join(",", concatenatedValues);

                using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
                {
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand(storeProcdurename, sqlConnection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@CycleId", CycleId);
                    sqlCommand.Parameters.AddWithValue("@RDIds", concatenatedString);

                    SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    int totalRows = dataTable.Rows.Count;
                    int batches = (int)Math.Ceiling((double)totalRows / batchSize);

                    for (int i = 0; i < batches; i++)
                    {
                        int startIndex = i * batchSize;
                        int endIndex = Math.Min((i + 1) * batchSize, totalRows);

                        DataTable batchTable = dataTable.Clone();
                        for (int j = startIndex; j < endIndex; j++)
                        {
                            batchTable.ImportRow(dataTable.Rows[j]);
                        }

                        string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(batchTable);

                        yield return jsonData;
                    }
                }
            }
        }
    }
}




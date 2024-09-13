namespace MigrationWebAPPAPI.Interfacese
{
    public interface ISQLConnector
    {
        Task<IEnumerable<string>> FetchAllSQLBatchDataAsyn(string sqlConnectionString,  int CycleId, string rdids);
        



    }
}

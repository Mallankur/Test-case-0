namespace MigrationWebAPPAPI.Interfacese
{
    public interface ICOSMOSConnector
    {
        Task<bool> CreateData_Using_SQL_SP_ConnectorAsync( List<int> CycleIds, string rdids);
        Task<bool> CreateDynamicCycleAsync(int Cycleid, string rdids);
        Task<IEnumerable<string>> FetchAllCycleDataAsync(int CycleId, string rdids);

        



    }
}

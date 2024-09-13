using MigrationWebAPPAPI.Model;
using MigrationWebAPPAPI.MongoDataDto;
using System.Runtime.CompilerServices;

namespace MigrationWebAPPAPI.Interfacese
{
    public interface IDataServises
    {
        Task<List<MongoDataModel>> FetchDataByDateRange(int CycleID, DateTime startDate, DateTime endDate);
        Task<List<MongoDataModel>> FetchDataByTimeInterval(int CycleId,TimeInterval interval);

        Task<List<MangoDataDto>> FetchDataByDosRange(int CycleId,int dosFrom, int dosTo);

        Task<List<MongoDataModel>>FetchDataByCycleId(int CycleId,CancellationToken cansillationToken);
        IAsyncEnumerable<MongoDataModel> FetchDataByCycleIdAsync(int cycleId, [EnumeratorCancellation] CancellationToken cancellationToken);
        Task<List<Object>> FetchDatabyReportDataEntityId(int CycleId, List<long> rdides);

        // object FetchDatabyReportDataEntityIdusingbsonobject(int cycleid, List<long> reportDataEntityId);
        Task<List<object>> FetchDataModelByRdiedobj(int cycleid, List<long> rdied);

        Task<List<object>> FetchDataModelMultiCycleRdied(List<int> cycleIds, List<long> rdied, int mode);

    }
}

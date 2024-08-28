using MigrationWebAPPAPI.Model;
using System.Runtime.CompilerServices;

namespace MigrationWebAPPAPI.Interfacese
{
    public interface IDataServises
    {
        Task<List<MongoDataModel>> FetchDataByDateRange(int CycleID, DateTime startDate, DateTime endDate);
        Task<List<MongoDataModel>> FetchDataByTimeInterval(int CycleId,TimeInterval interval);

        Task<List<MongoDataModel>> FetchDataByDosRange(int CycleId,int dosFrom, int dosTo);

        Task<List<MongoDataModel>>FetchDataByCycleId(int CycleId,CancellationToken cansillationToken);
        IAsyncEnumerable<MongoDataModel> FetchDataByCycleIdAsync(int cycleId, [EnumeratorCancellation] CancellationToken cancellationToken);

    }
}

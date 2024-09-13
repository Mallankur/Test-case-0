namespace MigrationWebAPPAPI.Model
{
    public class RdidsRequestModel
    {
        public List<long> rdeids{ get; set; } = new List<long>();

        public int CycleId { get; set; }

        public List<int> listofCycleId { get; set; }  = new List<int>();

        public int Mode { get; set; }


    }
}

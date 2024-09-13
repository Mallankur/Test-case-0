using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MigrationWebAPPAPI.Model
{
    public class MongoDataltd
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int? Sapunitno { get; set; }
        public long? Cycleno { get; set; }

        public long? CycleId { get; set; }

        public int? DOS { get; set; }
        public long? DM { get; set; }

        public long? DOSDATE { get; set; }

        public int? CC_Fields_Defs_Id { get; set; }

        public double? CSISValue { get; set; }

        public double? ImputedValue { get; set; }

        public double? ImputedValueMetric { get; set; }

        public double? ImputedValueImperial { get; set; }


        public double? CleansedValue { get; set; }
        public double? ValueMetric { get; set; }
        public double? ImportedValue { get; set; }
        public long? CSISDataTypeId { get; set; }
        public long? CSISTestRunId { get; set; }
        public long? CSISPredictionId { get; set; }
        public long? EOMobileLabId { get; set; }
        public long? ReportDataEntityId { get; set; }
        public bool? IgnoreError { get; set; }
        public int? ApplicationId { get; set; }
        public int? Mode { get; set; }
        public double? Value { get; set; }
        public int? ReportDataHeaderId { get; set; }
        public double? CatCheckConnectData { get; set; }
    }
}

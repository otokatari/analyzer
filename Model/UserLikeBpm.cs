using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserAnalyzer.Model
{
    public class UserLikeBpm
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [BsonElement("Userid")]
        public string Userid { get; set; }

        [BsonElement("BpmVector")]
        public double[] BpmVector { get; set; }
    }
}
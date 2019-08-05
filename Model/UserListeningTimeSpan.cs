using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserAnalyzer.Model
{
    public class UserListeningTimeSpan
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [BsonElement("Userid")]
        public string Userid { get; set; }

        [BsonElement("TimeVector")]
        public double[] TimeVector { get; set; }
    }
}
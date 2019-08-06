using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserAnalyzer.Model
{
    public class UserLikeLanguage
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [BsonElement("Userid")]
        public string Userid { get; set; }

        [BsonElement("LanguageVector")]
        public double[] LanguageVector { get; set; }
    }
}
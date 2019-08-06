using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserAnalyzer.Model
{
    public class UserSavedSingerList<TList>
    {
        [BsonElement]
        public ObjectId _id { get; set; }
        [BsonElement]
        public string Userid { get; set; }
        [BsonElement]
        public TList[] SavedList { get; set; }
        [BsonElement]
        public TList[] SystemList { get; set; }
    }
}
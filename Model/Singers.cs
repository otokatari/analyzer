using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserAnalyzer.Model
{
    public class Singers
    {

        [BsonElement]
        public ObjectId _id { get; set; }
        [BsonElement]
        public string SingerName { get; set; }
        [BsonElement]
        public string[] Language { get; set; }
        [BsonElement]
        public string NeteaseId { get; set; }
        [BsonElement]
        public string KugouId { get; set; }
        [BsonElement]
        public string QQMusicId { get; set; }
    }
}
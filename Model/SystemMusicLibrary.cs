using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserAnalyzer.Model
{
    public class SystemMusicLibrary : SimpleMusic
    {
        [BsonElement("language")]
        public string Language { get; set; }

        [BsonElement("bpm")]
        public double Bpm { get; set; }
    }
}

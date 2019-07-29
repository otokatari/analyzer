using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace UserAnalyzer.Model
{
    public class SimpleMusic
    {
        [JsonIgnore]
        [BsonElement("_id")]
        public ObjectId id { get; set; }

        [BsonElement("musicid")]
        [BsonRequired]
        [Required]
        public string Musicid { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("platform")]
        public string Platform { get; set; }

        [BsonElement("singerid")]
        public string Singerid { get; set; }

        [BsonElement("singername")]
        public string Singername { get; set; }

        [BsonElement("albumname")]
        public string Albumname { get; set; }

        [BsonElement("albumid")]
        public string Albumid { get; set; }

    }
}

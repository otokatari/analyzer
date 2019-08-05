using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace  UserAnalyzer.Model
{
    public class UserBehaviour
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [BsonElement("userid")]
        public string Userid { get; set; }

        [BsonElement("behaviour")]
        public string Behaviour { get; set; }

        [BsonElement("time")]
        public int Time { get; set; }

        [BsonElement("music")]
        [Required]
        [BsonRequired]
        public SimpleMusic Music { get; set; }

        [BsonElement("isinplaylist")]
        public string Isinplaylist { get; set; }
    }
}

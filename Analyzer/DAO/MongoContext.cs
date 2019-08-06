using MongoDB.Bson;
using MongoDB.Driver;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Analyzer.DAO
{
    public class MongoContext
    {
        public readonly IMongoCollection<SystemMusicLibrary> MusicLibrary;
        public readonly IMongoCollection<UserBehaviour> Behaviour;
        public readonly IMongoCollection<UserListeningTimeSpan> UserTimeSpan;
        public readonly IMongoCollection<UserLikeBpm> UserLikeBpm;
        public readonly IMongoCollection<Singers> Singers;
        public readonly IMongoCollection<UserSavedSingerList<ObjectId>> UserSavedSingerList;
        public readonly IMongoCollection<UserLikeLanguage> UserLikeLanguage;

        public readonly MongoClient client;
        private readonly AnalyzerConfig _config;
        public MongoContext(AnalyzerConfig config)
        {
            _config = config;
            client = new MongoClient(_config.MongoDBServer);
            var db = client.GetDatabase(_config.DatabaseName);
            MusicLibrary = db.GetCollection<SystemMusicLibrary>("SystemMusicLibrary");
            Behaviour = db.GetCollection<UserBehaviour>("UserBehaviour");
            UserTimeSpan = db.GetCollection<UserListeningTimeSpan>("UserListeningTimeSpan");
            UserLikeBpm = db.GetCollection<UserLikeBpm>("UserLikeBpm");
            UserLikeLanguage = db.GetCollection<UserLikeLanguage>("UserLikeLanguage");
            UserSavedSingerList = db.GetCollection<UserSavedSingerList<ObjectId>>("UserSavedSingerList");
            Singers = db.GetCollection<Singers>("Singers");
        }
    }
}
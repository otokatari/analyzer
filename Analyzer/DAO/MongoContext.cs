using MongoDB.Driver;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Analyzer.DAO
{
    public class MongoContext
    {
        public readonly IMongoCollection<SystemMusicLibrary> MusicLibrary;
        public readonly MongoClient client;
        private readonly AnalyzerConfig _config;
        public MongoContext(AnalyzerConfig config)
        {
            _config = config;
            client = new MongoClient(_config.MongoDBServer);
            var db = client.GetDatabase(_config.DatabaseName);
            MusicLibrary = db.GetCollection<SystemMusicLibrary>("SystemMusicLibrary");
        }
    }
}
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using UserAnalyzer.Analyzer.DAO;
using UserAnalyzer.Model;
using System.Linq;
namespace UserAnalyzer.Analyzer.User
{
    public class ListeningBPM
    {
        private readonly MongoContext _context;
        public ListeningBPM(MongoContext context)
        {
            _context = context;
        }

        public async void Analyze()
        {

            var filter = Builders<BsonDocument>.Filter;
            var matchFilter = filter.Size("music_detail", 1) & filter.Gt("music_detail.bpm", 0) & filter.Gt("time", Utils.GetSomeDaysUnix(-1));
            var result = _context.Behaviour
                                        .Aggregate()
                                        .Lookup("SystemMusicLibrary", "music.musicid", "musicid", "music_detail")
                                        .Match(matchFilter)
                                        .Project("{ music_detail : 1, userid: 1, time: 1} ");

            var resultList = await result.ToListAsync();

            /*
                * 80以下: 慢歌
                * 80-100: 较慢歌
                * 100-130: 中速歌曲
                * 130以上: 快歌
             */

            var behaviourDics = new Dictionary<string, List<SystemMusicLibrary>>();
            foreach (var item in resultList)
            {
                var Userid = item.GetValue("userid").AsString;
                var MusicDetail = item.GetValue("music_detail").AsBsonArray[0].AsBsonDocument;
                var userBehaviour = BsonSerializer.Deserialize<SystemMusicLibrary>(MusicDetail);

                if (behaviourDics.ContainsKey(Userid))
                {
                    behaviourDics[Userid].Add(userBehaviour);
                }
                else
                {
                    var list = new List<SystemMusicLibrary>();
                    list.Add(userBehaviour);
                    behaviourDics.Add(Userid, list);
                }
            }

            // 分析每个用户对应的BPM

            foreach (var user in behaviourDics)
            {
                long total = 0;
                double[] vector = new double[4];

                foreach (var item in user.Value)
                {
                    var bpm = item.Bpm;
                    if (bpm < 80)
                    {
                        vector[0]++;
                    }
                    else if (80 <= bpm && bpm < 100)
                    {
                        vector[1]++;
                    }
                    else if (100 <= bpm && bpm < 130)
                    {
                        vector[2]++;
                    }
                    else
                    {
                        vector[3]++;
                    }
                    total++;
                }
                // 归一化
                for (int i = 0; i < 4; i++)
                {
                    vector[i] /= vector[i] / total;
                }

                var records = _context.UserLikeBpm.AsQueryable()
                                                  .FirstOrDefault(r => r.Userid == user.Key);
                
                double[] combinedVector = null;
                if(records != null)
                {
                    // 已存在记录
                    combinedVector = records.BpmVector;
                    for (int i = 0; i < 4; i++)
                    {
                        combinedVector[i] = combinedVector[i] * 0.75 + vector[i] * 0.25;
                    }
                    var userFilter = Builders<UserLikeBpm>.Filter.Eq(r => r.Userid,user.Key);
                    var updater = Builders<UserLikeBpm>.Update.Set(r => r.BpmVector,combinedVector);
                    var updatedResult = await _context.UserLikeBpm.UpdateOneAsync(userFilter,updater);
                }
                else
                {
                    combinedVector = vector;
                    var document = new UserLikeBpm
                    {
                        Userid = user.Key,
                        BpmVector = combinedVector
                    };

                    await _context.UserLikeBpm.InsertOneAsync(document);
                }
            }
        }
    }
}
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using UserAnalyzer.Analyzer.DAO;
using UserAnalyzer.Model;
using System.Linq;

namespace UserAnalyzer.Analyzer.User
{
    public class ListeningLanguage
    {
        private readonly MongoContext _context;
        public ListeningLanguage(MongoContext context)
        {
            _context = context;   
        }
    

        public async void Analyze()
        {
            // 对于语种类型，也是像歌词分析一样。3/4 : 1/4

            
            var filter = Builders<BsonDocument>.Filter;
            var matchFilter = filter.Size("music_detail", 1) & filter.Ne("music_detail.language",BsonNull.Value) & filter.Gt("time", Utils.GetSomeDaysUnix(-1));
            var result = _context.Behaviour
                                        .Aggregate()
                                        .Lookup("SystemMusicLibrary", "music.musicid", "musicid", "music_detail")
                                        .Match(matchFilter)
                                        .Project("{ music_detail : 1, userid: 1, time: 1} ");

            var resultList = await result.ToListAsync();

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


            foreach (var user in behaviourDics)
            {
                double[] langs = new double[8];
                long total = 0;

                foreach (var music in user.Value)
                {
                    langs[GetLanguageIndex(music.Language)]++;
                    total++;
                }

                langs = Utils.Normalize(langs,total);

                var records = _context.UserLikeLanguage.AsQueryable().FirstOrDefault(x => x.Userid == user.Key);
                double[] combinedVector = null;
                if(records != null)
                {
                    // 已存在记录
                    combinedVector = records.LanguageVector;
                    for (int i = 0; i < combinedVector.Length; i++)
                    {
                        combinedVector[i] = combinedVector[i] * 0.75 + langs[i] * 0.25;
                    }
                    
                    combinedVector = Utils.Normalize(combinedVector,total);

                    var userFilter = Builders<UserLikeLanguage>.Filter.Eq(r => r.Userid,user.Key);
                    var updater = Builders<UserLikeLanguage>.Update.Set(r => r.LanguageVector,combinedVector);
                    var updatedResult = await _context.UserLikeLanguage.UpdateOneAsync(userFilter,updater);
                }
                else
                {
                    combinedVector = langs;
                    var document = new UserLikeLanguage
                    {
                        Userid = user.Key,
                        LanguageVector = combinedVector
                    };
                    await _context.UserLikeLanguage.InsertOneAsync(document);
                }
            }
        }

        private int GetLanguageIndex(string LanguageCode)
        {
            var LanguageRFCCode = new[] { "zh", "zh-Hant", "ja", "en", "ko", "fr", "other", "pure" };
            for (int i = 0; i < LanguageRFCCode.Length; i++)
            {
                if(LanguageCode == LanguageRFCCode[i]) return i;
            }
            return 6;
        }
    }
}


using System.Collections.Generic;
using MongoDB.Bson;
using System.Linq;
using MongoDB.Driver;
using UserAnalyzer.Analyzer.DAO;
using UserAnalyzer.Model;
using MongoDB.Bson.Serialization;

namespace UserAnalyzer.Analyzer.User
{
    public class UserLikeSinger
    {
        private readonly MongoContext _context;
        public UserLikeSinger(MongoContext context)
        {
            _context = context;
        }


        public async void Analyze()
        {
            // 这个分析器不用天天运行，一周运行一次就可以了
            var Before90DaysUnix = Utils.GetSomeDaysUnix(-90);
            var filter = Builders<UserBehaviour>.Filter;
            var behaviourFilter = filter.Gte(r => r.Time, Before90DaysUnix) & filter.Eq(r => r.Behaviour, "listen");
            var behavioursList = await (await _context.Behaviour.FindAsync(behaviourFilter)).ToListAsync();


            // 把行为按照用户来分类
            var behavioursDics = new Dictionary<string, List<UserBehaviour>>();
            foreach (var be in behavioursList)
            {
                if (behavioursDics.ContainsKey(be.Userid))
                {
                    behavioursDics[be.Userid].Add(be);
                }
                else
                {
                    var list = new List<UserBehaviour>();
                    list.Add(be);
                    behavioursDics.Add(be.Userid, list);
                }
            }

            foreach (var user in behavioursDics)
            {
                var Userid = user.Key;
                // 获取当前用户喜欢的歌手列表
                var userLikeSingers = GetSavedSingerInfo(Userid);
                

                var excludeSingers = new HashSet<string>();
                if(userLikeSingers != null)
                {
                    foreach (var saved in userLikeSingers.SavedList)
                    {
                        excludeSingers.Add(saved.SingerName);
                    }
                    foreach (var saved in userLikeSingers.SystemList)
                    {
                        excludeSingers.Add(saved.SingerName);
                    }
                }
                
                var indexedBehaviour = IndexBehaviourMusicBySingerName(user.Value);

                foreach(var singer in excludeSingers)
                {
                    if(indexedBehaviour.ContainsKey(singer)) indexedBehaviour.Remove(singer);
                }

                // 接下来剩下都是没有喜欢的歌手的累计播放次数了。

                var willBeAdded = new List<string>();
                willBeAdded.AddRange(indexedBehaviour.Where(x => x.Value.Count >= 10).Select(x => x.Key));

                var willBeAddedSingersFilter = Builders<Singers>.Filter.In("SingerName",willBeAdded);
                var willBeAddedSingers = (await _context.Singers.FindAsync(willBeAddedSingersFilter)).ToList();
                
                var objIds = willBeAddedSingers.Select(x => x._id);

                var userFilter = Builders<UserSavedSingerList<ObjectId>>.Filter.Eq(r => r.Userid,Userid);

                var userPusher = Builders<UserSavedSingerList<ObjectId>>.Update.PushEach("SystemList",objIds);
                
                var updatedResult = await _context.UserSavedSingerList.UpdateOneAsync(userFilter,userPusher);
            }
        }


        private Dictionary<string,HashSet<SimpleMusic>> IndexBehaviourMusicBySingerName(List<UserBehaviour> originalBehaviour)
        {
            var dics = new Dictionary<string,HashSet<SimpleMusic>>();
            foreach (var behav in originalBehaviour)
            {
                if(dics.ContainsKey(behav.Music.Singername))
                {
                    dics[behav.Music.Singername].Add(behav.Music);
                }
                else
                {
                    var sets = new HashSet<SimpleMusic>(new HashSetMusicDiffer());
                    sets.Add(behav.Music);
                    dics.Add(behav.Music.Singername,sets);
                }
            }
            return dics;
        }

        private UserSavedSingerList<Singers> GetSavedSingerInfo(string Userid)
        {
            var matcher = Builders<BsonDocument>.Filter.Eq("Userid", Userid);
            var aggregated = _context.UserSavedSingerList.Aggregate()
                                        .Lookup("Singers", "SavedList", "_id", "SavedList")
                                        .Match(matcher)
                                        .Lookup("Singers", "SystemList", "_id", "SystemList")
                                        .FirstOrDefault();

            
            if(aggregated!=null)
            {
                return BsonSerializer.Deserialize<UserSavedSingerList<Singers>>(aggregated);
            }
            return null;
        }
    }

    class HashSetMusicDiffer : IEqualityComparer<SimpleMusic>
    {
        public bool Equals(SimpleMusic x, SimpleMusic y)
        {
            return x.Musicid == y.Musicid;
        }

        public int GetHashCode(SimpleMusic obj)
        {
            return obj.GetHashCode();
        }
    }
}
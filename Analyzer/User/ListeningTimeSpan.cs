using System;
using MongoDB.Driver;
using UserAnalyzer.Analyzer.DAO;
using UserAnalyzer.Model;
using System.Linq;
using System.Collections.Generic;

namespace UserAnalyzer.Analyzer.User
{
    public class ListeningTimeSpan
    {
        private readonly MongoContext _context;
        public ListeningTimeSpan(MongoContext context)
        {
            _context = context;
        }

        public async void Analyze()
        {
            // 拿到昨天到今天所有产生的听歌行为
            var Before180DaysUnix = GetSomeDaysUnix(-1);
            var filter = Builders<UserBehaviour>.Filter;
            var behaviourFilter = filter.Gte(r => r.Time,Before180DaysUnix) & filter.Eq(r => r.Behaviour, "listen");
            var behavioursList = await (await _context.Behaviour.FindAsync(behaviourFilter)).ToListAsync();
            
            // 把这些行为按照UserID来分类
            var behavioursDics = new Dictionary<string,List<UserBehaviour>>();
            foreach (var be in behavioursList)
            {
                if(behavioursDics.ContainsKey(be.Userid))
                {
                    behavioursDics[be.Userid].Add(be);
                }
                else
                {
                    var list = new List<UserBehaviour>();
                    list.Add(be);
                    behavioursDics.Add(be.Userid,list);
                }
            }
            
            foreach (var user in behavioursDics)
            {
                var Userid = user.Key;
                long total = 0;
                double[] vector = new double[24];
                foreach (var item in user.Value)
                {
                    vector[Utils.UnixToDate(item.Time).Hour]++;
                    total++;
                }
                // 进行归一化：

                for (int i = 0; i < vector.Length; i++)
                {
                    vector[i] = vector[i]/total;
                }

                // 看看此用户之前有没有记录过听歌时间
                var records = _context.UserTimeSpan.AsQueryable().FirstOrDefault(r => r.Userid == Userid);
                double[] combinedVector = null;
                if(records != null)
                {
                    combinedVector = records.TimeVector;
                    for (int i = 0; i < combinedVector.Length; i++)
                    {
                        combinedVector[i] = (vector[i] + combinedVector[i]) / 2f;
                    }
                    var userFilter = Builders<UserListeningTimeSpan>.Filter.Eq(r => r.Userid, Userid);
                    var updater = Builders<UserListeningTimeSpan>.Update.Set(r => r.TimeVector, combinedVector);
                    var result = await _context.UserTimeSpan.UpdateOneAsync(userFilter, updater);
                }
                else
                {
                    combinedVector = vector;
                    var document = new UserListeningTimeSpan
                    {
                        Userid = Userid,
                        TimeVector = combinedVector
                    };
                    await _context.UserTimeSpan.InsertOneAsync(document);
                }
            }
        }

        private long GetSomeDaysUnix(int days) => Utils.DateToUnix(DateTime.Now.AddDays(days));
    }
}
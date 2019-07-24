using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Request
{
    public class NeteaseRequest : CommonRequest
    {
        public string SongAudio { get; set; }
        public string  SongLyric { get; set; }

        public AnalyzerConfig _config { get; set; }
        public RestClient ReqClient { get; set; }

        public NeteaseRequest(AnalyzerConfig config):base(config)
        {
            ReqClient = new RestClient(config.Netease);
            _config = config;
            SongAudio = "/song/url?id=";
            SongLyric = "/lyric?id=";

        }

        public void DownloadSongInfo(SongInfo info)
        {
            GetSongAudio(info);
            GetSongLyric(info);
            DownloadMusic(info);
            SaveLyric(info);
        }

        public void GetSongAudio(SongInfo info)
        {
            RestRequest req = new RestRequest();
            req.Resource = SongAudio + info.SongID;
            req.Method = Method.GET;

            var Resp = ReqClient.Execute(req);
            if(Resp.IsSuccessful)
            {
                var root = JObject.Parse(Resp.Content);
                var list = root["data"] as JArray;
                if(list.Count > 0)
                {
                    var downloadUrl = list[0]["url"].Value<string>();
                    var fileExt = list[0]["type"].Value<string>();
                    var fileName = $"{info.SongID}.{fileExt}";

                    info.AudioDownloadUrl = downloadUrl;
                    info.AudioFileName = fileName;
                }
            }
            else System.Console.WriteLine("Cannot find download url.");
        }

        public void GetSongLyric(SongInfo info)
        {
            RestRequest req = new RestRequest();
            req.Resource = SongLyric + info.SongID;
            req.Method = Method.GET;
            
            var Resp = ReqClient.Execute(req);
            if(Resp.IsSuccessful)
            {
                var root = JObject.Parse(Resp.Content);
                if(root.TryGetValue("uncollected",out JToken NoLyric))
                {
                    System.Console.WriteLine($"{info.SongID} -- 这首歌还没有歌词");
                }
                else
                {
                    var lyric = root["lrc"]["lyric"].Value<string>();
                    info.LyricString = lyric;
                    info.LyricFileName = $"{info.SongID}.lrc";
                }
            }
            else System.Console.WriteLine("获取歌词内容失败");
        }
    }
}
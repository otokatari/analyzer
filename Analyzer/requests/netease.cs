using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Analyzer.Request
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
            SongLyric = "/lyric?br=128000&id=";

        }

        public void GetSongInfo(SongInfo info)
        {
            GetSongAudio(info);
            GetSongLyric(info);
            DownloadMusic(info);
            SaveLyric(info);
        }

        private void GetSongAudio(SongInfo info)
        {
            RestRequest req = new RestRequest();
            req.Resource = SongAudio + info.SongID;
            req.Method = Method.GET;

            var Resp = ReqClient.Execute(req);
            if(Resp.IsSuccessful)
            {
                var root = JObject.Parse(Resp.Content);
                var list = root["data"] as JArray;
                if(list.Count > 0 && list[0]["url"].Type != JTokenType.Null)
                {
                    var downloadUrl = list[0]["url"].Value<string>();
                    var fileExt = list[0]["type"].Value<string>();
                    var fileName = $"{info.SongID}.{fileExt}";

                    info.AudioDownloadUrl = downloadUrl;
                    info.AudioFileName = fileName;
                }
                else System.Console.WriteLine($"{info.SongID} 此曲版权受限.");
            }
            else System.Console.WriteLine($"Cannot find download url {info.SongID}.");
        }

        private void GetSongLyric(SongInfo info)
        {
            RestRequest req = new RestRequest();
            req.Resource = SongLyric + info.SongID;
            req.Method = Method.GET;
            
            var Resp = ReqClient.Execute(req);
            if(Resp.IsSuccessful)
            {
                var root = JObject.Parse(Resp.Content);

                var lyrics = ExtractExistsLyrics(root);

                info.Lyrics = lyrics;
            }
            else System.Console.WriteLine("获取歌词内容失败");
        }


        private Lyrics ExtractExistsLyrics(JObject LyricRoot)
        {
            // If this song is absolute music or temporaily uncollect lyric.
            var lyrics = new Lyrics();
            if(LyricRoot.ContainsKey("nolyric"))
            {
                lyrics.AbsoluteMusic = true;
                return lyrics;
            }
            if(LyricRoot.ContainsKey("uncollected"))
            {
                lyrics.Uncollected = true;
                return lyrics;
            }
            lyrics.Lyric = LyricRoot["lrc"]["lyric"].Value<string>();
            if(LyricRoot.ContainAllKeys("tlyric","lyric"))
            {
                lyrics.TranslatedLyric = LyricRoot["tlyric"]["lyric"].Value<string>();
            }
            return lyrics;
        }
    }
}
using System;
using System.Buffers.Text;
using System.Text;
using Newtonsoft.Json.Linq;
using RestSharp;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Analyzer.Request
{
    public class QQMusicRequest : CommonRequest
    {
        private readonly AnalyzerConfig _config;

        private readonly RestClient client;

        private readonly string SongAudioVkey = "/song/vkey?songmid=";
        private readonly string SongLyric = "/song/lyric?songmid=";

        private readonly string SongAudioUrlPattern = "http://isure.stream.qqmusic.qq.com/C400{0}.m4a?guid=5448538077&vkey={1}&uin=0&fromtag=66";

        public QQMusicRequest(AnalyzerConfig config) : base(config)
        {
            _config = config;
            client = new RestClient(_config.QQMusic);
        }
        public void GetSongInfo(SongInfo info)
        {
            GetSongAudio(info);
            GetSongLyric(info);
            DownloadMusic(info);
            SaveLyric(info);
        }
        public void GetSongAudio(SongInfo info)
        {
            var Url = SongAudioVkey + info.SongID;
            var Req = new RestRequest(Url, Method.GET);
            var Resp = client.Execute(Req);
            if (Resp.IsSuccessful)
            {
                var root = JObject.Parse(Resp.Content);
                

                Thenable<JObject>
                        .Begin(root)
                        .then((that, data) =>
                        {
                            if (data.ContainAllKeys("data", "items"))
                                return data["data"]["items"].Value<JArray>();
                            return that.Reject<JArray>($"Cannot find items field that contains vkey from response: {info.SongID}.");
                        })
                        .then((that, vkeyRoot) =>
                        {
                            if (vkeyRoot.Count > 0)
                                return vkeyRoot[0].Value<JObject>()["vkey"].Value<string>();
                            return that.Reject<string>($"Cannot find vkey from response: {info.SongID}.");
                        })
                        .then((that, vkey) =>
                        {
                            if (!string.IsNullOrEmpty(vkey))
                            {
                                info.AudioDownloadUrl = string.Format(SongAudioUrlPattern, info.SongID, vkey);
                                info.AudioFileName = $"{info.SongID}.m4a";
                                return info;
                            }
                            return that.Reject<SongInfo>($"vkey in response is empty: {info.SongID}.");
                        })
                        .done();

            }
            else System.Console.WriteLine($"Cannot find download url: {info.SongID}.");
        }

        public void GetSongLyric(SongInfo info)
        {
            var Url = SongLyric + info.SongID;
            var Req = new RestRequest(Url, Method.GET);
            var Resp = client.Execute(Req);
            if (Resp.IsSuccessful)
            {
                var root = JObject.Parse(Resp.Content);

                Thenable<JObject>
                                .Begin(root)
                                .then((that, data) =>
                                {
                                    if (0 == data["retcode"].Value<int>())
                                        return data["lyric"].Value<string>();
                                    return that.Reject<string>($"SongID is wrong. {info.SongID}");
                                })
                                .then((that, Base64Lyric) =>
                                {
                                    var lyricBytes = Convert.FromBase64String(Base64Lyric);
                                    var textLyric = Encoding.UTF8.GetString(lyricBytes);

                                    var lyrics = new Lyrics();
                                    if (textLyric.Contains("此歌曲为没有填词的纯音乐，请您欣赏"))
                                    {
                                        lyrics.Uncollected = true;
                                        Console.WriteLine($"No lyric. {info.SongID}");
                                    }
                                    else
                                    {
                                        lyrics.Lyric = textLyric;
                                        info.Lyrics = lyrics;
                                    }
                                    return lyrics;
                                })
                                .done();
            }

            else System.Console.WriteLine($"Cannot get lyric content: {info.SongID}.");
        }
    }
}
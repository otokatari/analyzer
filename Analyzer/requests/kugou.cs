using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Analyzer.Request
{
    public class KugouRequest : CommonRequest
    {

        public string SongAudio { get; set; }

        private readonly AnalyzerConfig _config;
        private readonly RestClient ReqClient;

        public KugouRequest(AnalyzerConfig config) : base(config)
        {
            ReqClient = new RestClient(config.Kugou);
            _config = config;
            SongAudio = "/songurl?hash=";
        }

        public void GetSongInfo(SongInfo info)
        {
            GetSongAudio(info);
            DownloadMusic(info);
            SaveLyric(info);
        }

        public void GetSongAudio(SongInfo info)
        {
            var QueryUrl = SongAudio + info.SongID; // �˴���ΪFileHash
            var Req = new RestRequest(QueryUrl, Method.GET);
            var Resp = ReqClient.Execute(Req);
            if(Resp.IsSuccessful)
            {
                // ���Կ���û�б���IP
                var root = JObject.Parse(Resp.Content);
                var ErrCode = root["err_code"].Value<int>();
                if(ErrCode == 0)
                {
                    if(root.ContainAllKeys("data", "lyrics"))
                    {
                        var Lyrics = root["data"]["lyrics"].Value<string>();
                        info.LyricString = Lyrics;
                        info.LyricFileName = $"{info.SongID}.lrc";
                    }
                    else System.Console.WriteLine($"{info.SongID} ��ȡ����data�еĸ����Ϣ.");
                    if (root.ContainAllKeys("data", "play_url"))
                    {
                        info.AudioDownloadUrl = root["data"]["play_url"].Value<string>();
                        info.AudioFileName = $"{info.SongID}.mp3";
                    }
                    else System.Console.WriteLine($"{info.SongID} ��ȡ����data�еĸ������ص�ַ.");
                }
                else System.Console.WriteLine($"�������ErrorCode {ErrCode}, �������������ڱ���IP, �޷���ȡ���ֵ����ص�ַ�͸��.");
            }
            else
            {
                System.Console.WriteLine("����ʧ�ܣ��޷���ȡ���ֵ����ص�ַ�͸��.");
            }
        }

        
    }
}
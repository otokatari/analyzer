using System.IO;
using System.Text;
using RestSharp;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Analyzer.Request
{
    public class CommonRequest
    {
        private readonly RestClient ReqClient;
        private readonly AnalyzerConfig _config;
        public CommonRequest(AnalyzerConfig config)
        {
            ReqClient = new RestClient();
            _config = config;
        }

        public void DownloadMusic(SongInfo info)
        {
            if (!string.IsNullOrEmpty(info.AudioDownloadUrl))
            {
                RestRequest req = new RestRequest();
                req.Resource = info.AudioDownloadUrl;
                req.Method = Method.GET;

                var Resp = ReqClient.Execute(req);
                if (Resp.IsSuccessful)
                {
                    var length = (int)Resp.ContentLength;
                    using (var fw = new FileStream(ResolvePath(_config.MusicDownloadPath, info.AudioFileName), FileMode.Create))
                    {
                        fw.Write(Resp.RawBytes, 0, length);
                    }
                    System.Console.WriteLine($"Successfully saved {info.AudioFileName}.");
                }
                else
                {
                    System.Console.WriteLine($"Saving {info.AudioFileName} failed.");
                }
            }
            else System.Console.WriteLine($"找不到下载地址 -- {info.SongName}");
        }
        public void SaveLyric(SongInfo info)
        {
            if (!string.IsNullOrEmpty(info.LyricString))
            {
                using (var sw = new StreamWriter(ResolvePath(_config.LyricDownloadPath, info.LyricFileName), false, Encoding.UTF8))
                {
                    sw.WriteLine(info.LyricString);
                }
                System.Console.WriteLine($"Successfully saved {info.LyricFileName}.");
            }
            else System.Console.WriteLine($"找不到歌词 -- {info.SongName}");
        }
        public static string ResolvePath(string BasePath, string fileName)
        {
            return Path.Combine(BasePath, fileName);
        }
    }
}
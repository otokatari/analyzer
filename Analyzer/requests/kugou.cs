using RestSharp;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Request
{
    public class KugouRequest : CommonRequest
    {
        
        public string SongAudio { get; set; }

        private readonly AnalyzerConfig _config;
        private readonly RestClient ReqClient;

        public KugouRequest(AnalyzerConfig config):base(config)
        {
            ReqClient = new RestClient(config.Kugou);
            _config = config;
        }

        public void GetSongAudio(SongInfo info)
        {
            
        }

        public void GetSongLyric(SongInfo info)
        {
            
        }
    }
}
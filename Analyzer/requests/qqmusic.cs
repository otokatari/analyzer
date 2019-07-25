using RestSharp;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Analyzer.Request
{
    public class QQMusicRequest : CommonRequest
    {
        private readonly AnalyzerConfig _config;

        private readonly RestClient client;

        public QQMusicRequest(AnalyzerConfig config) : base(config)
        {
            _config = config;
            client = new RestClient(_config.QQMusic);
        }

        public void GetSongAudio(SongInfo info)
        {
            
        }

        public void GetSongLyric(SongInfo info)
        {

        }
    }
}
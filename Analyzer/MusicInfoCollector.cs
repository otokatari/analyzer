
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;
using UserAnalyzer.Request;

namespace UserAnalyzer.Analyze
{
    public class MusicInfoCollector
    {
        private readonly NeteaseRequest netease;
        private readonly KugouRequest kugou;
        private readonly QQMusicRequest qqmusic;
        private readonly AnalyzerConfig _config;
        public MusicInfoCollector(AnalyzerConfig config)
        {
            _config = config;
            netease = new NeteaseRequest(_config);
            qqmusic = new QQMusicRequest(_config);
            kugou = new KugouRequest(_config);
        }

        public void GetSongInfo(SongInfo info)
        {
            switch(info.Platform)
            {
                case "netease":
                    netease.DownloadSongInfo(info);
                    break;
                    
                case "kugou":
                    break;

                case "qqmusic":
                    break;
            }
        }
    }


}
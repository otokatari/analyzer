using UserAnalyzer.Analyzer.Music;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Analyzer
{
    public class AnalyzerServices
    {
        private readonly AnalyzerConfig _config;
        private readonly MusicAnalyzer _analyzer;
        private readonly MusicInfoCollector _collector;
        public AnalyzerServices(AnalyzerConfig config)
        {
            _config = config;
            _analyzer = new MusicAnalyzer(_config);
            _collector = new MusicInfoCollector(_config);
        }

        public bool AnalyzerMusic(SongInfo info)
        {
            _collector.GetSongInfo(info);
            _analyzer.AnalyzeBPM(info);
            _analyzer.AnalyzeLanguage(info);
            return _analyzer.PersistInfoToDb(info);
        }
    }
}
using System.IO;
using System.Text.RegularExpressions;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Analyze
{
    public class MusicAnalyzer
    {
        private readonly AnalyzerConfig _config;
        private readonly Regex BPMMatcher = new Regex("[0-9]+.[0-9]+");
        public MusicAnalyzer(AnalyzerConfig config)
        {
            _config = config;
        }

        public void AnalyzeBPM(SongInfo info)
        {
            var SongFilePath = Path.Combine(_config.MusicDownloadPath, info.AudioFileName);
            FileInfo file = new FileInfo(SongFilePath);
            if (file.Exists)
            {
                int ExitCode = Utils.ExecuteCommand(_config.Aubio, out string stdout, out string stderr, "tempo", "-H 640", "-v", SongFilePath);
                if (ExitCode == 0)
                {
                    var match = BPMMatcher.Match(stdout);
                    info.BPM = match.Success ? (double?)double.Parse(match.Value) : null;
                }
                else
                {
                    System.Console.WriteLine($"aubio不正常退出 -- {ExitCode}");
                }
            }
        }
        public void AnalyzeLanguage(SongInfo info)
        {
            if(!string.IsNullOrEmpty(info.LyricString) && !string.IsNullOrEmpty(info.LyricFileName))
            {
                var LyricFilePath = Path.Combine("./lyrics",info.LyricFileName);
                int ExitCode = Utils.ExecuteCommand("python",out string stdout,out string stderr,_config.LangDetector,LyricFilePath);
                if(ExitCode == 0)
                {
                    System.Console.WriteLine(stdout);
                }
                else
                {
                    System.Console.WriteLine($"{ExitCode} -- {stderr}");
                }
            }
        }

        public void PersistInfoToDb(SongInfo info)
        {

        }
    }
}
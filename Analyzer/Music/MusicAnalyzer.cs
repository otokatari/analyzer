using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using UserAnalyzer.Analyzer.DAO;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer.Analyzer.Music
{
    public class MusicAnalyzer
    {
        private readonly AnalyzerConfig _config;
        private readonly Regex BPMMatcher = new Regex("[0-9]+.[0-9]+");

        private readonly MongoContext context;
        public MusicAnalyzer(AnalyzerConfig config)
        {
            _config = config;
            context = new MongoContext(config);
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
            if(info.Lyrics.HasLyrics())
            {
                var LyricFileName = $"{info.SongID}.lrc";
                var LyricFilePath = Path.Combine("./lyrics",LyricFileName);
                if(new FileInfo(LyricFilePath).Exists)
                {
                    int ExitCode = Utils.ExecuteCommand(_config.PythonInterpreter, out string stdout, out string stderr, _config.LangDetector, LyricFilePath);
                    if (ExitCode == 0)
                    {
                        info.Language = stdout.Trim();
                        System.Console.WriteLine(stdout);
                    }
                    else
                    {
                        System.Console.WriteLine($"{ExitCode} -- {stderr}");
                    }
                }
                else System.Console.WriteLine($"{info.SongID} -- 歌词文件不存在!");
            }
            else System.Console.WriteLine($"{info.SongID} -- 没有歌词, 不需要分析.");
        }

        public bool PersistInfoToDb(SongInfo info)
        {

            var Updater =  Builders<SystemMusicLibrary>.Update;

            var filter = Builders<SystemMusicLibrary>.Filter.Eq(r => r.Musicid,info.SongID);

            var UpdateLanguage = Updater.Set(r => r.Language,info.Language);
            var UpdateBPM = Updater.Set(r => r.Bpm,info.BPM);
            System.Console.WriteLine($"将对歌曲 {info.SongID} {info.SongName} 进行更新, Language: {info.Language}, BPM: {info.BPM}");
            var BatchUpdate = Updater.Combine(UpdateLanguage,UpdateBPM);
            var result = context.MusicLibrary.UpdateOne(filter,BatchUpdate);
            return result.ModifiedCount == 1;
        }
    }
}

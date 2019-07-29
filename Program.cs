using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UserAnalyzer.Analyzer;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;

namespace UserAnalyzer
{
    class Program
    {
        private static AnalyzerConfig config;
        private static AnalyzerServices _services;

        static void Main(string[] args)
        {
            LoadConfig();
            if (config != null)
            {
                DetectAnalyzerFileDirectory();
                _services = new AnalyzerServices(config);
                // ProcessPipeline();
            }
        }


        static void ProcessPipeline()
        {
            var info = new SongInfo
            {
                SongID = "134683878C7945D01D44E9B5CF0FDF1F",
                SongName = "勇气",
                ArtistName = "梁静茹",
                Platform = "kugou"
            };

            _services.AnalyzerMusic(info);
        }


        static void LoadConfig()
        {
            string ConfigJson = string.Empty;
            try
            {
                using (var sr = new StreamReader("config.json", Encoding.UTF8))
                {
                    ConfigJson = sr.ReadToEnd();
                }
            }
            catch (FileNotFoundException fileNotFoundEx)
            {
                System.Console.WriteLine("Configuration file not found! " + fileNotFoundEx.Message);
                Environment.Exit(1);
            }
            config = JsonConvert.DeserializeObject(ConfigJson, typeof(AnalyzerConfig)) as AnalyzerConfig;
        }

        static void DetectAnalyzerFileDirectory()
        {
            var MusicDownloadDir = new DirectoryInfo(config.MusicDownloadPath);
            if (!MusicDownloadDir.Exists)
            {
                MusicDownloadDir.Create();
            }
            var LyricDownloadDir = new DirectoryInfo(config.LyricDownloadPath);
            if (!LyricDownloadDir.Exists)
            {
                LyricDownloadDir.Create();
            }
        }
    }
}

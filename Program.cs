﻿using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UserAnalyzer.Analyze;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;
using UserAnalyzer.Request;

namespace UserAnalyzer
{
    class Program
    {
        private static AnalyzerConfig config;
        private static NeteaseRequest netease;
        
        private static MusicAnalyzer analyzer;
        static void Main(string[] args)
        {
            LoadConfig();
            if (config != null)
            {
                DetectAnalyzerFileDirectory();
                netease = new NeteaseRequest(config);
                analyzer = new MusicAnalyzer(config);

                ProcessPipeline();
            }
        }


        static void ProcessPipeline()
        {
            var info = new SongInfo
            {
                SongID = "29356312",
                SongName = "打ち上げ花火を見るような",
                ArtistName = "初音ミク",
                Platform = "netease",
                AudioFileName = "29356312.mp3"
            };

            analyzer.AnalyzeBPM(info);
            System.Console.WriteLine(info.BPM);
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
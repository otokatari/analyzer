using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserAnalyzer.Analyzer;
using UserAnalyzer.Analyzer.DAO;
using UserAnalyzer.Configurations;
using UserAnalyzer.Model;
namespace UserAnalyzer
{
    class Program
    {
        private static MongoContext context;
        private static AnalyzerConfig config;
        private static AnalyzerServices _services;
        private static IConnection connection;
        private static IModel channel;
        private static EventingBasicConsumer consumer;

        static void Main(string[] args)
        {
            LoadConfig(args.Length > 0);
            if (config != null)
            {
                DetectAnalyzerFileDirectory();

                _services = new AnalyzerServices(config);
                context = new MongoContext(config);
                
                InitAnalyzerConsumer();
                WaitForProcessingAsyncTask();
            }
        }

        static void ProcessPipeline(SimpleMusic info)
        {
            var song = new SongInfo
            {
                SongID = info.Musicid,
                SongName = info.Name,
                ArtistName = info.Singername,
                Platform = info.Platform
            };
            try
            {
                _services.AnalyzerMusic(song);
            }
            catch (System.Exception ex)
            {
                Console.Error.WriteLine($"Some error occurred when analyzing music, reason: {ex.Message}");
            }
        }

        static void InitAnalyzerConsumer()
        {
            var factory = new ConnectionFactory { HostName = config.RabbitMQServer, UserName = "guest", Password = "otokatarimq" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(config.RabbitMQQueueName, true, false, true, null);
            channel.BasicQos(0, 2, false);
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += AnalyzerMessageHandler;
            channel.BasicConsume(config.RabbitMQQueueName,false,consumer);
            System.Console.WriteLine($"Music analyzer is running on {config.RabbitMQServer}, {config.RabbitMQQueueName}");
        }
        
        static void AnalyzerMessageHandler(object model, BasicDeliverEventArgs ea)
        {
            try
            {
                System.Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd H:mm:ss")} Receiving message!");
                var MusicInfoJson = Encoding.UTF8.GetString(ea.Body);
                var MusicInfo = JsonConvert.DeserializeObject(MusicInfoJson, typeof(SimpleMusic)) as SimpleMusic;
                Task.Run(() =>
                {
                    ProcessPipeline(MusicInfo);
                    channel.BasicAck(ea.DeliveryTag, false);
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Some error occurred when receiving message from queue, reason: {ex.Message}");
                // 暂时先考虑ACK掉这个Message, 避免整个队列堵塞。
                channel.BasicAck(ea.DeliveryTag, false);
            }
        }


        static void LoadConfig(bool prod)
        {
            string ConfigJson = string.Empty;
            try
            {
                var prodFlag = prod ? "-prod" : "";
                var configFile = "config" + prodFlag + ".json";
                using (var sr = new StreamReader(configFile, Encoding.UTF8))
                {
                    ConfigJson = sr.ReadToEnd();
                }
                System.Console.WriteLine($"Configuration {configFile} loaded.");
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

        static void WaitForProcessingAsyncTask()
        {
            while(true)
                Console.ReadLine();
        }
    }
}

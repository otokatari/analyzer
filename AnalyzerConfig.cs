namespace UserAnalyzer.Configurations
{
    public class AnalyzerConfig
    {

        private string _Netease;
        private string _Kugou;
        private string _QQMusic;

        public string RequestServer { get; set; }
        public string Netease
        {
            get
            {
                return RequestServer + _Netease;
            }
            set
            {
                _Netease = value;
            }
        }
        public string Kugou
        {
            get
            {
                return RequestServer + _Kugou;
            }
            set
            {
                _Kugou = value;
            }
        }
        public string QQMusic
        {
            get
            {
                return RequestServer + _QQMusic;
            }
            set
            {
                _QQMusic = value;
            }
        }
        public string RabbitMQServer { get; set; }

        public string MusicDownloadPath { get; set; }
        public string LyricDownloadPath { get; set; }
        public string Aubio { get; set; }
        public string LangDetector { get; set; }
        public string FFMpeg { get; set; }
        public string MongoDBServer { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
namespace UserAnalyzer.Model
{
    public class SongInfo
    {
        public string SongID { get; set; }
        public string SongName { get; set; }
        public string ArtistName { get; set; }
        public string AudioDownloadUrl { get; set; }
        public string LyricString { get; set; }
        public string TranslatedLyricString { get; set; }
        public string Language { get; set; }
        public double? BPM { get; set; }
        public string AudioFileName { get; set; }
        public string LyricFileName { get; set; }
        public string Platform { get; set; }
    }
}
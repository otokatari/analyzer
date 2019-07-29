namespace UserAnalyzer.Model
{
    public class SongInfo
    {
        public string SongID { get; set; }
        public string SongName { get; set; }
        public string ArtistName { get; set; }
        public string AudioDownloadUrl { get; set; }

        public Lyrics Lyrics { get; set; }

        public string Language { get; set; }
        public double? BPM { get; set; }
        public string AudioFileName { get; set; }
        public string Platform { get; set; }
    }
}
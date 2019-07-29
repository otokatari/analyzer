namespace UserAnalyzer.Model
{
    public class Lyrics
    {
        public string Lyric { get; set; }
        public string TranslatedLyric { get; set; }
        public bool Uncollected { get; set; }
        public bool AbsoluteMusic { get; set; }

        public bool HasLyrics() => !(Uncollected || AbsoluteMusic);
    }
}
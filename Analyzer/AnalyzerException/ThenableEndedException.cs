namespace UserAnalyzer.Analyzer.AnalyzerException
{
    [System.Serializable]
    public class ThenableEndedException : System.Exception
    {
        public ThenableEndedException() { }
        public ThenableEndedException(string message) : base(message) { }
        public ThenableEndedException(string message, System.Exception inner) : base(message, inner) { }
        protected ThenableEndedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
using System;

namespace UserAnalyzer.Analyzer.AnalyzerException
{

    [Serializable]
    public class FFMpegConvertFailedException : Exception
    {
        public string FFMpegStdErr { get; set; }

        public FFMpegConvertFailedException() { }
        public FFMpegConvertFailedException(string message) : base(message) { }
        public FFMpegConvertFailedException(string message, Exception inner) : base(message, inner) { }
        public FFMpegConvertFailedException(string message, string stderr) : base(message) { FFMpegStdErr = stderr; }

        protected FFMpegConvertFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

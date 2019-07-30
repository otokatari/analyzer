using System;
using Newtonsoft.Json.Linq;
using UserAnalyzer.Analyzer.AnalyzerException;

namespace UserAnalyzer
{
    public class Thenable<T>
    {
        private bool Ended = false;
        private object RejectReason = null;
        private Thenable(T data)
        {
            _data = data;
        }
        public T _data;
        public static Thenable<T> Begin<T>(T data)
        {
            return new Thenable<T>(data);
        }

        public Thenable<U> then<U>(Func<Thenable<T>,T,U> handle)
        {
            if(Ended)
                return Begin(default(U));
            return Begin(handle(this,_data));
        }

        public T done() => _data;
        public U Reject<U>(object reason = null)
        {
            if(!Ended)
            {
                RejectReason = reason;
                Ended = true;
                return default(U);
            }
            else throw new ThenableEndedException("Cannot reject a thenable twice!");
        }
    }
}
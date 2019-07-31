using System;
using Newtonsoft.Json.Linq;
using UserAnalyzer.Analyzer.AnalyzerException;

namespace UserAnalyzer
{
    public class Thenable<T>
    {
        private bool Ended = false;
        public object RejectReason = null;
        private Thenable(T data, bool rejected)
        {
            _data = data;
            Ended = rejected;
        }
        public T _data;
        public static Thenable<TResult> Begin<TResult>(TResult data, bool rejected = false)
        {
            return new Thenable<TResult>(data, rejected);
        }

        public Thenable<U> then<U>(Func<Thenable<T>, T, U> handle)
        {
            if (Ended)
                return Begin(default(U), true);
            try
            {
                return Begin(handle(this, _data), Ended);
            }
            catch (Exception ex)
            {
                return Begin(Reject<U>(ex.Message), true);
            }
        }

        public void done() { Ended = true; }

        public U Reject<U>(object reason = null)
        {
            if (!Ended)
            {
                if (reason is string) System.Console.WriteLine(reason as string);
                RejectReason = reason;
                Ended = true;
                return default(U);
            }
            else throw new ThenableEndedException("Cannot reject a thenable twice!");
        }
    }
}
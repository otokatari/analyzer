using UserAnalyzer.Analyzer.DAO;

namespace UserAnalyzer.Analyzer.User
{
    public class ListeningBPM
    {
        private readonly MongoContext _context;
        public ListeningBPM(MongoContext context)
        {
            _context = context;
        }

        public void Analyze()
        {

        }
        
    }
}
using System.Threading;

namespace QuizMaker.Data
{
    public class ConnectionStorage
    {
        private int _connections = 0;

        public int Increment()
        {
            return Interlocked.Increment(ref _connections);
        }

        public int Decrement()
        {
            return Interlocked.Decrement(ref _connections);
        }

        public int Count()
        {
            return _connections;
        }
    }
}

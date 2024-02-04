using System.Threading;

namespace Alexey.TinyUrlService
{
    internal class UrlInfo
    {
        private ulong _clickCount;

        public string LongUrl { get; }

        public ulong ClickCount
        {
            get
            {
                // fixme: consider using Interlocked.Read if we need the exact value.
                // return Interlocked.Read(ref _clickCount);
                return _clickCount;
            }
        }

        public UrlInfo(string longUrl)
        {
            _clickCount = 0;
            LongUrl = longUrl;
        }

        public void IncrementClickCount()
        {
            Interlocked.Increment(ref _clickCount);
        }
    }
}

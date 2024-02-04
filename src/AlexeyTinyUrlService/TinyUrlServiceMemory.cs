using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Alexey.TinyUrlService
{
    // In memory implementation of ITinyUrlService
    internal sealed class TinyUrlServiceMemory : ITinyUrlService
    {
        private readonly ConcurrentDictionary<string, UrlInfo> _urlMap;

        private readonly IShortUriGenerator _shortUriGenerator;

        private readonly ITinyUrlServiceConfig _config;

        public TinyUrlServiceMemory(IShortUriGenerator shortUriGenerator, ITinyUrlServiceConfig config)
        {
            if (shortUriGenerator == null)
            {
                throw new ArgumentException($"{nameof(shortUriGenerator)} cannot be null");
            }
            _shortUriGenerator = shortUriGenerator;

            if (config == null)
            {
                throw new ArgumentException($"{nameof(config)} cannot be null");
            }
            _config = config;

            _urlMap = new ConcurrentDictionary<string, UrlInfo>();
        }

        public string CreateShortUri(string longUrl, string customShortUri = null)
        {
            string shortUri;

            // todo@: Measure the performance. This additional check can be slow.
            if (_config.MonitorMaxTotalItemsNumber && GetTotalItemsNumber() > _config.MaxTotalItemsNumber)
            {
                // @todo: fatal failure, alert about this
                throw new ApplicationException("Unable to add new short URI due to limits overflow.");
            }

            if (customShortUri == null)
            {
                byte attemptNumber = 0;
                do
                {
                    attemptNumber++;
                    if (attemptNumber > _config.MaxAddAttemptsNumber) 
                    {
                        // @todo: fatal failure, alert about this
                        throw new ApplicationException("Unable to generate new unique short URI.");
                    }
                    shortUri = _shortUriGenerator.Generate(longUrl);
                }
                while (_urlMap.ContainsKey(shortUri));
            }
            else
            {
                // @todo: check the requirements for customShortUri value
                if (!customShortUri.All(x => char.IsLetterOrDigit(x)))
                {
                    throw new ArgumentException($"Invalid {nameof(customShortUri)}");
                }
                if (_urlMap.ContainsKey(customShortUri))
                {
                    // fixme@: it can be unsafe to interpolate/log the customShortUri value
                    throw new ArgumentException($"{nameof(customShortUri)} already exists");
                }
                shortUri = customShortUri;
            }

            if (!_urlMap.TryAdd(shortUri, new UrlInfo(longUrl)))
            {
                /*
                * Some minor number of the exception below can be ok.
                * However, it needs to be logged and closely monitored.
                * Perhaps we need to implement another isolation level.
                */
                throw new ApplicationException($"Generated {nameof(shortUri)} already exists");
            }

            return shortUri;
        }

        public string GetLongUrl(string shortUri)
        {
            if (_urlMap.TryGetValue(shortUri, out var urlInfo))
            {
                urlInfo.IncrementClickCount();

                return urlInfo.LongUrl;
            }
            throw new ArgumentException("Short URI not found.");
        }

        public void DeleteShortUri(string shortUri)
        {
            bool exists = _urlMap.ContainsKey(shortUri) && _urlMap.TryRemove(shortUri, out _);
            if (!exists) // fixme: keep this additional variable for readability
            {
                throw new ArgumentException("Short URI not found.");
            }
        }

        public ulong GetClickCount(string shortUri)
        {
            if (_urlMap.TryGetValue(shortUri, out var urlInfo))
            {
                return urlInfo.ClickCount;
            }
            throw new ArgumentException("short URI not found.");
        }

        public int GetTotalItemsNumber()
        {
            // Skip statement is to prevent locking on the dictionary
            // @see https://arbel.net/2013/02/03/best-practices-for-using-concurrentdictionary/
            return _urlMap.Skip(0).Count(); 
        }
    }
}

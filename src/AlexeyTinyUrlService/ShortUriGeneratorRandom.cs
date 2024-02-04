using System;
using System.Diagnostics;

namespace Alexey.TinyUrlService
{
    // Simple implementation using a random string generator
    internal class ShortUriGeneratorRandom : IShortUriGenerator
    {
        protected byte _length;
        protected Random _random;

        public ShortUriGeneratorRandom(byte length)
        {
            if (length < 1)
            {
                throw new ArgumentException($"{nameof(length)} should be greater than zero");
            }
            _length = length;

            // todo@ think how to seed it better
            _random = new Random((int)DateTime.Now.Ticks);
        }

        public virtual string Generate(string originalUrl)
        {
            Debug.Assert(_length > 0);

            // todo@ consider adding other symbols to increase possible combinations number
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var shortUrl = new char[_length];
            for (byte i = 0; i < _length; i++)
            {
                shortUrl[i] = chars[_random.Next(chars.Length)];
            }

            return new string(shortUrl);            
        }
    }
}

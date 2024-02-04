using System;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text;

namespace Alexey.TinyUrlService
{
    // Another possible implementation of IShortUriGenerator (not recommended)
    internal class ShortUriGeneratorMd5 : ShortUriGeneratorRandom
    {
        public ShortUriGeneratorMd5(byte length) : base(length)
        {
        }

        public override string Generate(string originalUrl)
        {
            Debug.Assert(_length > 0);
            if (String.IsNullOrEmpty(originalUrl))
            {
                throw new ArgumentException($"{nameof(originalUrl)} should not be empty");
            }

            var shortUrl = new char[_length];
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(originalUrl));
                string baseString = Convert.ToHexString(hashBytes)
                    .ToLowerInvariant(); // Convert to lowercase for consistency
                for (byte i = 0; i < _length; i++)
                {
                    shortUrl[i] = baseString[_random.Next(baseString.Length)];
                }
            }

            return new string(shortUrl);
        }
    }
}

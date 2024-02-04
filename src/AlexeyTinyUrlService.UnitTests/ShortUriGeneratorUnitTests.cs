using System;
using System.Collections.Generic;

using FluentAssertions;
using Xunit;

namespace Alexey.TinyUrlService.UnitTests;

[Trait("TestType", "UnitTests")]
public class ShortUriGeneratorUnitTests
{
    [Fact]
    public void TestGeneratorRandomZeroLengthException()
    {
        Assert.Throws<ArgumentException>(() => new ShortUriGeneratorRandom(0));
    }

    [Fact]
    public void TestGeneratorMd5ZeroLengthException()
    {
        Assert.Throws<ArgumentException>(() => new ShortUriGeneratorMd5(0));
    }

    [Theory]
    [InlineData(typeof(ShortUriGeneratorRandom), 1)]
    [InlineData(typeof(ShortUriGeneratorRandom), 6)]
    [InlineData(typeof(ShortUriGeneratorRandom), 32)]
    [InlineData(typeof(ShortUriGeneratorRandom), 255)]
    [InlineData(typeof(ShortUriGeneratorMd5), 1)]
    [InlineData(typeof(ShortUriGeneratorMd5), 6)]
    [InlineData(typeof(ShortUriGeneratorMd5), 32)]
    [InlineData(typeof(ShortUriGeneratorMd5), 255)]
    public void TestLength(Type ShortUriGeneratorType, byte length)
    {
        string originalUrl = "https://www.example.com";
        IShortUriGenerator ShortUriGenerator = (IShortUriGenerator) Activator.CreateInstance(ShortUriGeneratorType, [length]);
        string shortUrl = ShortUriGenerator.Generate(originalUrl);
        shortUrl.Should().NotBeNullOrEmpty();
        shortUrl.Length.Should().Be(length);
    }

    [Theory]
    [InlineData(typeof(ShortUriGeneratorRandom))]
    [InlineData(typeof(ShortUriGeneratorMd5))]
    public void TestGenerateNewValueAlways(Type ShortUriGeneratorType)
    {
        IShortUriGenerator ShortUriGenerator = (IShortUriGenerator) Activator.CreateInstance(ShortUriGeneratorType, [(byte)6]);
        
        string originalUrl = "https://www.example.com";
        HashSet<string> urlsHashSet = new HashSet<string>();
        for (byte i = 0; i < 10; i++)
        {
            string shortUrl = ShortUriGenerator.Generate(originalUrl);
            shortUrl.Should().NotBeNullOrEmpty();
            urlsHashSet.Should().NotContain(shortUrl);
            urlsHashSet.Add(shortUrl);
        }
    }
}

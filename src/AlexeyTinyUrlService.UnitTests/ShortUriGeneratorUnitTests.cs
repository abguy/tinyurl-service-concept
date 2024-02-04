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

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(32)]
    [InlineData(255)]
    public void TestLength( byte length)
    {
        string originalUrl = "https://www.example.com";
        IShortUriGenerator ShortUriGenerator = new ShortUriGeneratorRandom(length);
        string shortUrl = ShortUriGenerator.Generate(originalUrl);
        shortUrl.Should().NotBeNullOrEmpty();
        shortUrl.Length.Should().Be(length);
    }

    [Fact]
    public void TestGenerateNewValueAlways()
    {
        IShortUriGenerator ShortUriGenerator = new ShortUriGeneratorRandom(6);
        
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

using FluentAssertions;
using Xunit;

namespace Alexey.TinyUrlService.UnitTests;

[Trait("TestType", "UnitTests")]
public class UrlInfoUnitTests
{
    [Fact]
    public void TestIncrement()
    {
        string longUrl = "https://www.example.com";

        UrlInfo info = new UrlInfo(longUrl);
        info.LongUrl.Should().Be(longUrl);

        for (byte i = 0; i < 5; i++)
        {
            info.ClickCount.Should().Be(i);
            info.IncrementClickCount();
        }
        info.ClickCount.Should().Be(5);
        info.LongUrl.Should().Be(longUrl);
    }
}

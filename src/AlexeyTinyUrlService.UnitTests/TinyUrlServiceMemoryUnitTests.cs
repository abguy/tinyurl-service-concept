using System;

using FluentAssertions;
using Moq;
using Xunit;

namespace Alexey.TinyUrlService.UnitTests;

[Trait("TestType", "UnitTests")]
public class TinyUrlServiceMemoryUnitTests
{
    private readonly Mock<ITinyUrlServiceConfig> _configMock = new Mock<ITinyUrlServiceConfig>();
    private readonly Mock<IShortUriGenerator> _shortUriGeneratorMock = new Mock<IShortUriGenerator>();

    [Fact]
    public void TestConstructor()
    {
        ITinyUrlService service = new TinyUrlServiceMemory(_shortUriGeneratorMock.Object, _configMock.Object);

        Assert.Throws<ArgumentException>(() => new TinyUrlServiceMemory(null, _configMock.Object));
        Assert.Throws<ArgumentException>(() => new TinyUrlServiceMemory(_shortUriGeneratorMock.Object, null));
    }

    [Fact]
    public void TestMaxTotalItemsNumber()
    {
        string longUrl = "https://www.example.com";
        string shortUri = "short";

        _shortUriGeneratorMock.Setup(s => s.Generate(longUrl)).Returns(shortUri);
        _configMock.Setup(s => s.MaxAddAttemptsNumber).Returns(1);
        _configMock.Setup(s => s.MonitorMaxTotalItemsNumber).Returns(true);
        _configMock.Setup(s => s.MaxTotalItemsNumber).Returns(1);
        ITinyUrlService service = new TinyUrlServiceMemory(_shortUriGeneratorMock.Object, _configMock.Object);

        string shortUriGenerated = service.CreateShortUri(longUrl);
        shortUriGenerated.Should().Be(shortUri);

        Assert.Throws<ApplicationException>(() => service.CreateShortUri(longUrl));
    }

    [Fact]
    public void TestMaxAttemptsNumber()
    {
        string longUrl = "https://www.example.com";
        string shortUri = "short";

        _shortUriGeneratorMock.Setup(s => s.Generate(longUrl)).Returns(shortUri);
        _configMock.Setup(s => s.MaxAddAttemptsNumber).Returns(1);
        _configMock.Setup(s => s.MonitorMaxTotalItemsNumber).Returns(false);
        _configMock.Setup(s => s.MaxTotalItemsNumber).Returns(100);
        ITinyUrlService service = new TinyUrlServiceMemory(_shortUriGeneratorMock.Object, _configMock.Object);

        string shortUriGenerated = service.CreateShortUri(longUrl);
        shortUriGenerated.Should().Be(shortUri);

        Assert.Throws<ApplicationException>(() => service.CreateShortUri(longUrl));
    }

    [Fact]
    public void TestGetDeleteUnexistent()
    {
        string longUrl = "https://www.example.com";
        string shortUri = "short";

        _shortUriGeneratorMock.Setup(s => s.Generate(longUrl)).Returns(shortUri);
        _configMock.Setup(s => s.MaxAddAttemptsNumber).Returns(1);
        _configMock.Setup(s => s.MonitorMaxTotalItemsNumber).Returns(false);
        _configMock.Setup(s => s.MaxTotalItemsNumber).Returns(100);

        ITinyUrlService service = new TinyUrlServiceMemory(_shortUriGeneratorMock.Object, _configMock.Object);
        service.GetTotalItemsNumber().Should().Be(0);

        Assert.Throws<ArgumentException>(() => service.GetLongUrl(shortUri));
        Assert.Throws<ArgumentException>(() => service.GetClickCount(shortUri));
        Assert.Throws<ArgumentException>(() => service.DeleteShortUri(shortUri));
    }

    [Fact]
    public void TestGetClickDelete()
    {
        string longUrl = "https://www.example.com";
        string shortUri = "short";

        _shortUriGeneratorMock.Setup(s => s.Generate(longUrl)).Returns(shortUri);
        _configMock.Setup(s => s.MaxAddAttemptsNumber).Returns(1);
        _configMock.Setup(s => s.MonitorMaxTotalItemsNumber).Returns(false);
        _configMock.Setup(s => s.MaxTotalItemsNumber).Returns(100);

        ITinyUrlService service = new TinyUrlServiceMemory(_shortUriGeneratorMock.Object, _configMock.Object);
        service.GetTotalItemsNumber().Should().Be(0);

        string shortUriGenerated = service.CreateShortUri(longUrl);
        shortUriGenerated.Should().Be(shortUri);
        service.GetTotalItemsNumber().Should().Be(1);

        service.GetClickCount(shortUri).Should().Be(0);

        service.GetLongUrl(shortUri).Should().Be(longUrl);
        service.GetClickCount(shortUri).Should().Be(1);
        service.GetTotalItemsNumber().Should().Be(1);

        service.GetLongUrl(shortUri).Should().Be(longUrl);
        service.GetClickCount(shortUri).Should().Be(2);
        service.GetTotalItemsNumber().Should().Be(1);

        service.DeleteShortUri(shortUri);
        service.GetTotalItemsNumber().Should().Be(0);
    }


    [Fact]
    public void TestCustomUri()
    {
        string longUrl = "https://www.example.com";
        string shortUri = "short";
        string customUri = "custom";

        _shortUriGeneratorMock.Setup(s => s.Generate(longUrl)).Returns(shortUri);
        _configMock.Setup(s => s.MaxAddAttemptsNumber).Returns(1);
        _configMock.Setup(s => s.MonitorMaxTotalItemsNumber).Returns(false);
        _configMock.Setup(s => s.MaxTotalItemsNumber).Returns(100);

        ITinyUrlService service = new TinyUrlServiceMemory(_shortUriGeneratorMock.Object, _configMock.Object);
        service.GetTotalItemsNumber().Should().Be(0);

        string shortUriGenerated1 = service.CreateShortUri(longUrl);
        shortUriGenerated1.Should().Be(shortUri);
        service.GetTotalItemsNumber().Should().Be(1);

        Assert.Throws<ArgumentException>(() => service.CreateShortUri(longUrl, "?invalid#$"));
        service.GetTotalItemsNumber().Should().Be(1);

        string shortUriGenerated2 = service.CreateShortUri(longUrl, customUri);
        shortUriGenerated2.Should().Be(customUri);
        service.GetTotalItemsNumber().Should().Be(2);

        // Should be already exists
        Assert.Throws<ArgumentException>(() => service.CreateShortUri(longUrl, customUri));
        service.GetTotalItemsNumber().Should().Be(2);

        service.DeleteShortUri(customUri);
        service.GetTotalItemsNumber().Should().Be(1);
    }
}

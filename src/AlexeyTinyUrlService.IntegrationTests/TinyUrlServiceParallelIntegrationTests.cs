using System;
using System.Collections.Generic;

using FluentAssertions;
using Xunit;

using Alexey.TinyUrlService;

namespace Alexey.TinyUrlService.IntegrationTests;

[Trait("TestType", "IntegrationTests")]
public class TinyUrlServiceParallelIntegrationTests
{
    [Theory]
    [InlineData("google.com")]
    [InlineData("github.com")]
    [InlineData("open.ai")]
    [InlineData("bard.google.com")]
    public void TestBaseScenarios(string baseDomain)
    {
        string longUrl1 = $"https://www.{baseDomain}";
        string longUrl2 = $"https://{baseDomain}";
        string longUrl3 = $"http://www.{baseDomain}";
        string longUrl4 = $"https://www.{baseDomain}/test";
        string longUrl5 = $"https://www.{baseDomain}?test";

        string baseDomainKey = baseDomain.Replace(".", "");
        string customUri1 = $"custom{baseDomainKey}1";
        string customUri2 = $"custom{baseDomainKey}2";

        ITinyUrlService service = AlexeyTinyUrlService.GetInstance();
        HashSet<string> shortUris = new HashSet<string>();

        var initialTotal = service.GetTotalItemsNumber();
        initialTotal.Should().BeGreaterOrEqualTo(0);

        var shortUri1 = service.CreateShortUri(longUrl1);
        shortUris.Should().NotContain(shortUri1);
        shortUris.Add(shortUri1);

        var shortUri2 = service.CreateShortUri(longUrl2);
        shortUris.Should().NotContain(shortUri2);
        shortUris.Add(shortUri2);

        // Check click counts
        service.GetClickCount(shortUri1).Should().Be(0);
        service.GetClickCount(shortUri2).Should().Be(0);
        service.GetLongUrl(shortUri1).Should().Be(longUrl1);
        service.GetClickCount(shortUri1).Should().Be(1);
        service.GetClickCount(shortUri2).Should().Be(0);
        service.GetLongUrl(shortUri1).Should().Be(longUrl1);
        service.GetClickCount(shortUri1).Should().Be(2);
        service.GetClickCount(shortUri2).Should().Be(0);
        service.GetLongUrl(shortUri2).Should().Be(longUrl2);
        service.GetClickCount(shortUri1).Should().Be(2);
        service.GetClickCount(shortUri2).Should().Be(1);

        // Add few other URLs
        var shortUri3 = service.CreateShortUri(longUrl3);
        shortUris.Should().NotContain(shortUri3);
        shortUris.Add(shortUri3);
        service.GetClickCount(shortUri1).Should().Be(2);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);

        var shortUri4 = service.CreateShortUri(longUrl4);
        shortUris.Should().NotContain(shortUri4);
        shortUris.Add(shortUri4);
        service.GetClickCount(shortUri1).Should().Be(2);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);

        // Try to add already existing
        var shortUri5 = service.CreateShortUri(longUrl2);
        shortUris.Should().NotContain(shortUri5);
        shortUris.Add(shortUri5);
        service.GetClickCount(shortUri1).Should().Be(2);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);

        // Try custom short URIs
        var shortUri6 = service.CreateShortUri(longUrl5, customUri1);
        shortUri6.Should().Be(customUri1);
        shortUris.Should().NotContain(shortUri6);
        shortUris.Add(shortUri6);
        service.GetClickCount(shortUri1).Should().Be(2);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri6).Should().Be(0);

        // Try another custom URI for already existing
        var shortUri7 = service.CreateShortUri(longUrl5, customUri2);
        shortUri7.Should().Be(customUri2);
        shortUris.Should().NotContain(shortUri7);
        shortUris.Add(shortUri7);
        service.GetClickCount(shortUri1).Should().Be(2);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri6).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(0);

        // Try remove previously added URI
        service.DeleteShortUri(shortUri1);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri6).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(0);

        // Try other click counts
        byte clicksNumber = 100;
        shortUri7.Should().Be(customUri2);
        for (byte i = 0; i < clicksNumber; i++)
        {
            service.GetLongUrl(shortUri3).Should().Be(longUrl3);
            service.GetLongUrl(shortUri7).Should().Be(longUrl5);
        }
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(clicksNumber);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri6).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(clicksNumber);

        // Try remove custom URI
        service.DeleteShortUri(customUri1);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(clicksNumber);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(clicksNumber);

        // Add previously removed URI
        var shortUri8 = service.CreateShortUri(longUrl1);
        shortUri8.Should().NotBe(shortUri1);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(clicksNumber);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(clicksNumber);
        service.GetClickCount(shortUri8).Should().Be(0);

        // Add previously removed custom URI
        var shortUri9 = service.CreateShortUri(longUrl5, customUri1);
        shortUri9.Should().Be(customUri1);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(clicksNumber);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(clicksNumber);
        service.GetClickCount(shortUri8).Should().Be(0);
        service.GetClickCount(shortUri9).Should().Be(0);
    }

    [Theory]
    [InlineData("google.com")]
    [InlineData("github.com")]
    [InlineData("open.ai")]
    [InlineData("bard.google.com")]
    public void TestMultipleAdding(string baseDomain)
    {
        string longUrl = $"https://www.{baseDomain}/example-page";

        ITinyUrlService service = AlexeyTinyUrlService.GetInstance();

        HashSet<string> shortUris = new HashSet<string>();
        for (byte i = 0; i < 100; i++)
        {
            var shortUri = service.CreateShortUri(longUrl);
            shortUris.Should().NotContain(shortUri);
            shortUris.Add(shortUri);
        }
    }

    [Theory]
    [InlineData("google.com")]
    [InlineData("github.com")]
    [InlineData("open.ai")]
    [InlineData("bard.google.com")]
    public void TestExistingCustomUriException(string baseDomain)
    {
        string longUrl = $"https://www.{baseDomain}/example-page";
        string baseDomainKey = baseDomain.Replace(".", "");
        string customUri = $"custom{baseDomainKey}";

        ITinyUrlService service = AlexeyTinyUrlService.GetInstance();

        var shortUri = service.CreateShortUri(longUrl, customUri);
        shortUri.Should().Be(customUri);

        Assert.Throws<ArgumentException>(() => service.CreateShortUri(longUrl, customUri));
    }
}

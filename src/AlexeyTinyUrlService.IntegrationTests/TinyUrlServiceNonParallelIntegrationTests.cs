using System;
using System.Collections.Generic;

using FluentAssertions;
using Xunit;

using Alexey.TinyUrlService;

namespace Alexey.TinyUrlService.IntegrationTests;

[CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
public class NonParallelCollectionDefinitionClass
{
}

[Trait("TestType", "IntegrationTests")]
[Collection("Non-Parallel Collection")]
public class TinyUrlServiceNonParallelIntegrationTests
{
    [Fact]
    public void TestBaseScenario()
    {
        string longUrl1 = "https://www.example.com";
        string longUrl2 = "https://example.com";
        string longUrl3 = "http://www.example.com";
        string longUrl4 = "https://www.example.com/test";
        string longUrl5 = "https://www.example.com?test";
        string customUri1 = "custom1";
        string customUri2 = "custom2";

        ITinyUrlService service = AlexeyTinyUrlService.GetInstance();
        HashSet<string> shortUris = new HashSet<string>();

        var initialTotal = service.GetTotalItemsNumber();
        initialTotal.Should().BeGreaterOrEqualTo(0);

        var shortUri1 = service.CreateShortUri(longUrl1);
        shortUris.Should().NotContain(shortUri1);
        shortUris.Add(shortUri1);
        service.GetTotalItemsNumber().Should().Be(initialTotal + 1);

        var shortUri2 = service.CreateShortUri(longUrl2);
        shortUris.Should().NotContain(shortUri2);
        shortUris.Add(shortUri2);
        service.GetTotalItemsNumber().Should().Be(initialTotal + 2);

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
        service.GetTotalItemsNumber().Should().Be(initialTotal + 3);
        service.GetClickCount(shortUri1).Should().Be(2);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);

        var shortUri4 = service.CreateShortUri(longUrl4);
        shortUris.Should().NotContain(shortUri4);
        shortUris.Add(shortUri4);
        service.GetTotalItemsNumber().Should().Be(initialTotal + 4);
        service.GetClickCount(shortUri1).Should().Be(2);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);

        // Try to add already existing
        var shortUri5 = service.CreateShortUri(longUrl2);
        shortUris.Should().NotContain(shortUri5);
        shortUris.Add(shortUri5);
        service.GetTotalItemsNumber().Should().Be(initialTotal + 5);
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
        service.GetTotalItemsNumber().Should().Be(initialTotal + 6);
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
        service.GetTotalItemsNumber().Should().Be(initialTotal + 7);
        service.GetClickCount(shortUri1).Should().Be(2);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri6).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(0);

        // Try remove previously added URI
        service.DeleteShortUri(shortUri1);
        service.GetTotalItemsNumber().Should().Be(initialTotal + 6);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri6).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(0);

        // Try remove custom URI
        service.DeleteShortUri(customUri1);
        service.GetTotalItemsNumber().Should().Be(initialTotal + 5);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(0);

        // Add previously removed URI
        var shortUri8 = service.CreateShortUri(longUrl1);
        shortUri8.Should().NotBe(shortUri1);
        service.GetTotalItemsNumber().Should().Be(initialTotal + 6);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(0);
        service.GetClickCount(shortUri8).Should().Be(0);

        // Add previously removed custom URI
        var shortUri9 = service.CreateShortUri(longUrl5, customUri1);
        shortUri9.Should().Be(customUri1);
        service.GetTotalItemsNumber().Should().Be(initialTotal + 7);
        service.GetClickCount(shortUri2).Should().Be(1);
        service.GetClickCount(shortUri3).Should().Be(0);
        service.GetClickCount(shortUri4).Should().Be(0);
        service.GetClickCount(shortUri5).Should().Be(0);
        service.GetClickCount(shortUri7).Should().Be(0);
        service.GetClickCount(shortUri8).Should().Be(0);
        service.GetClickCount(shortUri9).Should().Be(0);
    }
}

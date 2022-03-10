using Xunit;
using OgcApi.Net.Features.Options;
using Xunit.Abstractions;
using System.Text.Json;
using System.IO;
using System;
using OgcApi.Net.Features.OpenApi;
using Microsoft.Extensions.Configuration;

namespace OgcApi.Net.Features.Tests
{
    public class OgcApiOptionsTests
    {
        private readonly ITestOutputHelper output;
        public IConfiguration Configuration { get; }
        public OgcApiOptionsTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("//Util//appsettings_test.json")]
        public void SettingDecerializationTest(string value)
        {
            var config = new ConfigurationBuilder().AddJsonFile(value).Build();
            var options = new OgcApiOptions();
            config.GetSection("FeaturesOptions").Bind(options);

            Assert.NotNull(options.Collections);
            Assert.NotNull(options.LandingPage);
            Assert.True(options.UseApiKeyAuthorization);
        }
    }
}

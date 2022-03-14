using Xunit;
using OgcApi.Net.Features.Options;
using Xunit.Abstractions;
using System.Text.Json;
using System.IO;
using System;
using OgcApi.Net.Features.OpenApi;
using Microsoft.Extensions.Configuration;
using OgcApi.Net.Features.Tests.Util;

namespace OgcApi.Net.Features.Tests
{
    public class OgcApiOptionsFacts
    {
        [Fact]
        public void SettingDecerialization()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options);
        }

        [Fact]
        public void LandingPageValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options.LandingPage);
            Assert.Equal("API Title", options.LandingPage.Title);
            Assert.Equal("API Description", options.LandingPage.Description);
            Assert.Equal("1.0", options.LandingPage.Version);
            Assert.Equal("API Contacts", options.LandingPage.ContactName);
            Assert.Equal("https://example.com/", options.LandingPage.ContactUrl.ToString());
            Assert.Equal("https://api.com/index.html", options.LandingPage.ApiDocumentPage.ToString());
            Assert.Equal("https://api.com/swagger.json", options.LandingPage.ApiDescriptionPage.ToString());
        }

        [Fact]
        public void ConformanceValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options.Conformance);
            Assert.NotNull(options.Conformance.ConformsTo);
            Assert.NotEmpty(options.Conformance.ConformsTo);
            Assert.Equal(2, options.Conformance.ConformsTo.Count);
            Assert.Equal("https://api.com/conform1.html", options.Conformance.ConformsTo[0].ToString());
            Assert.Equal("https://api.com/conform2.html", options.Conformance.ConformsTo[1].ToString());
        }

        [Fact]
        public void UseApiKeyAuthorizationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.True(options.UseApiKeyAuthorization);
        }

        [Fact]
        public void CollectionsLinksValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options.Collections);
            Assert.NotNull(options.Collections.Links);
            Assert.NotEmpty(options.Collections.Links); //here
            Assert.Equal(2,options.Collections.Links.Count);
            Assert.Equal("https://api.com/link1", options.Collections.Links[0].ToString());
            Assert.Equal("https://api.com/link2", options.Collections.Links[1].ToString());
        }

    }
}

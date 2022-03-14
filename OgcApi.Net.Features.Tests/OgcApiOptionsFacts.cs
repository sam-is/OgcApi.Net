using Xunit;
using OgcApi.Net.Features.Tests.Util;
using System.Text.Json;

namespace OgcApi.Net.Features.Tests
{
    public class OgcApiOptionsFacts
    {
        [Fact]
        public void SettingsSerializationExecuted()
        {
            var options = OptionsUtils.GetOptionsFromCode();
            var res = JsonSerializer.Serialize(options, options.GetType());

            Assert.NotNull(res);
        }

        [Fact]
        public void SettingsDeserializationExecuted()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options);
        }

        [Fact]
        public void LandingPageDeserializationValid()
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
        public void ConformanceDeserializationValid()
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
        public void UseApiKeyAuthorizationDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.True(options.UseApiKeyAuthorization);
        }

        [Fact]
        public void CollectionsLinksDeserializationValid()
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

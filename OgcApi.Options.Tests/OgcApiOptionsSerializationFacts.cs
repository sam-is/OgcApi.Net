using OgcApi.Options.Tests.Utils;
using Xunit;

namespace OgcApi.Options.Tests
{
    public class OgcApiOptionsSerializationFacts : IClassFixture<OgcApiOptionsFixture>
    {

        [Fact]
        public void OgcApiOptionsSerialization()
        {
            var json = OptionsUtils.SerializeOgcApiOptions(OptionsUtils.GetOptionsFromCode());

            Assert.False(string.IsNullOrEmpty(json));
        }
        [Fact]
        public void LandingPageOptionsSerialization()
        {
            var json = OptionsUtils.SerializeLandingPageOptions(OptionsUtils.GetOptionsFromCode().LandingPage);

            Assert.False(string.IsNullOrEmpty(json));
        }
        [Fact]
        public void CollectionsOptionsSerialization()
        {
            var json = OptionsUtils.SerializeCollectionsOptions(OptionsUtils.GetOptionsFromCode().Collections);

            Assert.False(string.IsNullOrEmpty(json));
        }

    }
}



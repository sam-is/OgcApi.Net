using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Converters;
using OgcApi.Net.Options.Features;
using OgcApi.Net.Schemas.Converters;
using SampleWebApplication.Security;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SampleWebApplication;

public class ConfigureOgcApiOptions(IConfiguration configuration) : IConfigureOptions<OgcApiOptions>
{
    private static JsonSerializerOptions _ogcJsonSerializerOptions;

    public void Configure(OgcApiOptions options)
    {
        _ogcJsonSerializerOptions ??= new JsonSerializerOptions
        {
            Converters =
            {
                new FeaturesSourceOptionsConverter(),
                new TilesSourceOptionsConverter(),
                new SchemaCollectionOptionsConverter()
            }
        };

        var fileName = configuration.GetValue<string>("OgcSettingsFileName");

        var ogcApiOptions = JsonSerializer.Deserialize<OgcApiOptions>(File.ReadAllBytes(fileName), _ogcJsonSerializerOptions);

        if (ogcApiOptions == null) return;

        var sqlServerConnectionString = configuration.GetConnectionString("SqlServerConnectionString");
        var postgresConnectionString = configuration.GetConnectionString("PostgresConnectionString");

        foreach (var item in ogcApiOptions.Collections.Items.Where(x => x.Features != null))
        {
            if (item.Features.Storage is not SqlFeaturesSourceOptions sourceOptions) continue;

            if (sourceOptions.Type == "SqlServer")
                sourceOptions.ConnectionString = sqlServerConnectionString;
            else if (sourceOptions.Type == "PostGis")
                sourceOptions.ConnectionString = postgresConnectionString;
        }

        foreach (var item in ogcApiOptions.Collections.Items.Where(x => x.Tiles != null))
        {
            item.Tiles.Storage.TileAccessDelegate = TileAccess.TilesAccessDelegate;
            item.Tiles.Storage.FeatureAccessDelegate = TileAccess.FeatureAccessDelegate;
        }

        options.Collections = ogcApiOptions.Collections;
        options.Conformance = ogcApiOptions.Conformance;
        options.LandingPage = ogcApiOptions.LandingPage;
        options.UseApiKeyAuthorization = ogcApiOptions.UseApiKeyAuthorization;
    }
}

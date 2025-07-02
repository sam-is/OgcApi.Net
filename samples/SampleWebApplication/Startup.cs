using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetTopologySuite.Features;
using OgcApi.Net;
using OgcApi.Net.MbTiles;
using OgcApi.Net.Schemas;
using OgcApi.Net.SqlServer;
using SampleWebApplication.Utils;

namespace SampleWebApplication;

public class Startup(IConfiguration configuration)
{
    private static bool TilesAccessDelegate(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey)
    {
        switch (collectionId ?? "")
        {
            case "Polygons":
                return true;
            case "PolygonsWithApiKey" when (apiKey ?? "").Equals("qwerty") && tileMatrix is >= 0 and <= 7:
                switch (tileMatrix)
                {
                    case 0: return tileRow == 0 && tileCol == 0;
                    case 1: return tileRow == 0 && tileCol == 1;
                    case 2: return tileRow == 1 && tileCol == 2;
                    case 3: return tileRow == 2 && tileCol == 5;
                    case 4: return tileRow == 5 && tileCol == 10;
                    case 5: return tileRow == 10 && tileCol == 20;
                    case 6: return tileRow == 20 && tileCol == 40;
                    case 7: return tileRow == 40 && tileCol is 81 or 82;
                }

                break;
            case "FeatureAccessData":
                return true;
        }

        return false;
    }

    private static bool FeatureAccessDelegate(string collectionId, IFeature feature, string apiKey) => (collectionId ?? "") switch
    {
        "FeatureAccessData" => apiKey == "admin" ||
            apiKey == "value" && feature.Attributes.Exists("value") &&
            (feature.Attributes["value"] is long and > 1200 ||
            feature.Attributes["value"] is > 100.0) ||
            feature.Attributes.Exists("roleAccess") && feature.Attributes["roleAccess"].ToString() == apiKey,
        _ => true,
    };

    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOgcApiSqlServerProvider();
        services.AddOgcApiMbTilesProvider();
        services.AddSchemasOpenApiExtension();
        services.AddOgcApi("ogcapi-features.json", TilesAccessDelegate);

        services.AddControllers().AddOgcApiControllers();

        services.AddCors(c => c.AddPolicy(name: "OgcApi", options =>
        {
            options.AllowAnyMethod().AllowAnyHeader();
        }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        DatabaseUtils.RecreateDatabase();

        app.UseSwaggerUI(swaggerOptions =>
        {
            swaggerOptions.RoutePrefix = "api";
            swaggerOptions.SwaggerEndpoint("ogc/swagger.json", "OGC API");
        });

        app.UseRouting();

        app.UseCors("OgcApi");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OgcApi.Net;
using OgcApi.Net.MbTiles;
using OgcApi.Net.OpenApi;
using OgcApi.Net.Options;
using OgcApi.Net.PostGis;
using OgcApi.Net.Schemas;
using OgcApi.Net.SqlServer;
using SampleWebApplication;
using Scalar.AspNetCore;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOgcApiSqlServerProvider();
builder.Services.AddOgcApiPostGisProvider();
builder.Services.AddOgcApiMbTilesProvider();
builder.Services.AddSchemasOpenApiExtension();
builder.Services.AddSampleOgcStyles(builder.Configuration);

// If the connection string is configured in the ogcapi.json file
//builder.Services.AddOgcApi("ogcapi.json", TileAccess.TilesAccessDelegate, TileAccess.FeatureAccessDelegate);

// If the connection string is determined at runtime
builder.Services.AddSingleton<IConfigureOptions<OgcApiOptions>, ConfigureOgcApiOptions>();

builder.Services.AddSingleton<IOpenApiGenerator, OpenApiGenerator>();

builder.Services.AddControllers().AddOgcApiControllers();

builder.Services.AddCors(c => c.AddPolicy(name: "OgcApi", options =>
{
    options.AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwaggerUI(swaggerOptions => swaggerOptions.SwaggerEndpoint("/api/ogc/openapi.json", "OGC API"));

app.MapScalarApiReference(options => options.WithOpenApiRoutePattern("api/ogc/openapi.json"));

app.UseRouting();
app.MapControllers();

app.UseCors("OgcApi");

app.UseAuthorization();

app.Run();
using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.OpenApi.Interfaces;
using OgcApi.Net.Options;
using OgcApi.Net.Schemas.Converters;
using OgcApi.Net.Schemas.Schema;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Schemas;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSchemasOpenApiExtension(this IServiceCollection services)
    {
        services.AddSingleton<IOpenApiExtension, SchemasOpenApiExtension>();
        services.AddSingleton<JsonConverter<CollectionOptions>, SchemaCollectionOptionsConverter>();
        services.AddScoped<ISchemaGenerator, SchemaGenerator>();

        return services;
    }
}
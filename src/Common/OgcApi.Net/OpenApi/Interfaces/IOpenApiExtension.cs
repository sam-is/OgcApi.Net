using Microsoft.OpenApi.Models;
using OgcApi.Net.Options;

namespace OgcApi.Net.OpenApi.Interfaces;

public interface IOpenApiExtension
{
    void Apply(OpenApiDocument document, OgcApiOptions ogcApiOptions);
}
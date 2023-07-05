using Microsoft.OpenApi.Models;
using System;

namespace OgcApi.Net.OpenApi;

public interface IOpenApiGenerator
{
    OpenApiDocument GetDocument(Uri baseUrl);
}
using System;
using Microsoft.OpenApi.Models;

namespace OgcApi.Net.Features.OpenApi
{
    public interface IOpenApiGenerator
    {
        OpenApiDocument GetDocument(Uri baseUrl);
    }
}
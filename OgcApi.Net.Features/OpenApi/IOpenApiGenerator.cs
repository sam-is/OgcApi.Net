using Microsoft.OpenApi.Models;
using System;

namespace OgcApi.Net.Features.OpenApi
{
    public interface IOpenApiGenerator
    {
        OpenApiDocument GetDocument(Uri baseUrl);
    }
}
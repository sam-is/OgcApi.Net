using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OgcApi.Net.Features.Swagger
{
    public class OgcServersDocumentFilter : IDocumentFilter
    {
        public const string BasePath = "/api/ogc";

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Servers.Add(new OpenApiServer() { Url = BasePath });

            var paths = new OpenApiPaths();
            foreach (var path in swaggerDoc.Paths)
            {
                paths.Add(path.Key == BasePath ? "/" : path.Key.Replace(BasePath, ""), path.Value);
            }
            swaggerDoc.Paths = paths;
        }
    }
}

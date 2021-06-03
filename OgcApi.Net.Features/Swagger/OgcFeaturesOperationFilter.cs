using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OgcApi.Net.Features.Swagger
{
    public class OgcFeaturesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.DocumentName == "ogc")
            {
                OpenApiParameter apiKeyParameter = null;
                foreach (var parameter in operation.Parameters)
                {
                    if (parameter.Name == "limit")
                    {
                        parameter.Schema = new OpenApiSchema()
                        {
                            Type = "integer",
                            Format = "int32",
                            Default = new OpenApiInteger(10),
                            Maximum = 10000,
                            Minimum = 1
                        };
                        parameter.Style = ParameterStyle.Form;
                    }

                    if (parameter.Name == "bbox")
                    {
                        parameter.Style = ParameterStyle.Form;
                        parameter.Explode = false;
                        parameter.Schema = new OpenApiSchema()
                        {
                            Type = "array",
                            MinItems = 4,
                            MaxItems = 6,
                            Items = new OpenApiSchema()
                            {
                                Type = "number"
                            }
                        };
                    }

                    if (parameter.Name == "datetime")
                    {
                        parameter.Style = ParameterStyle.Form;
                    }

                    if (parameter.Name == "apiKey")
                    {
                        apiKeyParameter = parameter;
                    }
                }
                operation.Parameters.Remove(apiKeyParameter);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;

namespace OgcApi.Net.Features.Swagger
{
    public static class OgcFeaturesApiSwaggerServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApiSwaggerGen(
            this IServiceCollection services,
            bool useApiKeySecuritySchema = false,
            Action<SwaggerGenOptions> setupAction = null)
        {
            return services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ogc", new OpenApiInfo { Title = "OGC API", Version = "1.0.0" });
                options.CustomOperationIds(d => (d.ActionDescriptor as ControllerActionDescriptor)?.ActionName);
                var xmlFile = "OgcApi.Net.Features.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.OperationFilter<OgcFeaturesOperationFilter>();
                options.DocumentFilter<OgcServersDocumentFilter>();
                options.IncludeXmlComments(xmlPath);

                if (useApiKeySecuritySchema)
                {
                    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
                    {
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Query,
                        Name = "apiKey",
                        Description = "API key"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                            },
                            Array.Empty<string>()
                        }
                    });
                }

                if (setupAction != null)
                    setupAction.Invoke(options);
            });
        }
    }
}
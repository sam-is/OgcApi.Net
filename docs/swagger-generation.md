# Swagger Generation

The [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) library can be used to automatically generate a Swagger web page for your API. This provides interactive documentation and allows users to explore and test API operations directly from their browser.

## Adding Swagger Configuration
To enable Swagger generation, add the following configuration in the `Configure` method of your `Startup` class:

```csharp
app.UseSwaggerUI(swaggerOptions =>
{
    swaggerOptions.RoutePrefix = "api";
    swaggerOptions.SwaggerEndpoint("ogc/swagger.json", "OGC API");
});
```

## OpenAPI JSON Definition
The OpenAPI JSON definition is available at the `/api/ogc/swagger.json` route in your application. This JSON file serves as the foundation for the Swagger UI and other tools that consume OpenAPI specifications.

For more details about configuring Swagger, refer to the [Swashbuckle.AspNetCore documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
---
layout: default
title: OpenAPI Generation
nav_order: 9
---

# OpenAPI Generation

The `OgcApi.Net` automatically generates an OpenAPI specification using the `Microsoft.OpenApi` library. This specification provides a machine-readable description of your API that can be consumed by documentation tools and API clients.

## OpenAPI JSON Definition

The OpenAPI JSON definition is available at the `/api/ogc/openapi.json` endpoint. This file serves as the foundation for API documentation tools like Swagger UI and Scalar.

## Adding Swagger UI

The [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) library provides interactive API documentation with Swagger UI. This allows users to explore and test API operations directly from their browser.

To enable Swagger UI:

1. Install the `Swashbuckle.AspNetCore` NuGet package
2. Add the following configuration in your `Program.cs` file:

```csharp
app.UseSwaggerUI(options => options.SwaggerEndpoint("/api/ogc/openapi.json", "OGC API"));
```

By default, Swagger UI is available at `/swagger`.

## Adding Scalar

[Scalar](https://scalar.com/) is a modern alternative to Swagger UI for API documentation. It provides a clean, user-friendly interface for exploring and testing your API.

To enable Scalar:

1. Install the `Scalar.AspNetCore` NuGet package
2. Add the following configuration in your `Program.cs` file:

```csharp
app.MapScalarApiReference(options => options.WithOpenApiRoutePattern("api/ogc/openapi.json"));
```

By default, Scalar is available at `/scalar`.
# CORS Support

All OGC API controllers use a CORS policy named `"OgcApi"`. This policy can be configured in the `ConfigureServices` method of your `Startup` class.

## Configuring CORS Policy
To configure the CORS policy, add the following code in the `ConfigureServices` method:

```csharp
services.AddCors(c => c.AddPolicy(name: "OgcApi", options =>
{
    options.AllowAnyMethod().AllowAnyHeader();
}));
```

## Enabling CORS Middleware
After configuring the policy, don't forget to enable the CORS middleware in the `Configure` method:

```csharp
app.UseCors("OgcApi");
```

For more information about CORS in ASP.NET Core, refer to the [official Microsoft documentation](https://learn.microsoft.com/en-us/aspnet/core/security/cors).
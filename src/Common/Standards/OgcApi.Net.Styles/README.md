# OgcApi.Net.Styles

OGC API — Styles implementation for OgcApi.Net.

This package implements the OGC API — Styles core and management endpoints for working with styles and stylesheets attached to collections (resources).

## Installation
Install the package via NuGet:
```bash
dotnet add package OgcApi.Net.Styles
```

## Usage
To enable styles support, register the extensions in your `Startup.cs`:

```csharp
services.AddOgcApiStyles();
services.Configure<OgcApiStylesOptions>(configuration.GetSection(nameof(OgcApiStylesOptions)));
services.Configure<StyleFileSystemStorageOptions>(configuration.GetSection(nameof(StyleFileSystemStorageOptions)));

services.AddStylesStorage<StyleFileSystemStorage>(); // Or your own implementation
services.AddStylesMetadataStorage<StyleMetadataFileSystemStorage>(); // Or your own implementation

// Optional: implement and register IStylesAuthorizationService
services.AddStyleAuthorization<SampleStylesAuthorizationService>();
```

## Configuration Example
Example `appsettings.json` snippet:

```json
"OgcApiStylesOptions": {
  "UseAuthorization": true
},
"StyleFileSystemStorageOptions": {
  "BaseDirectory": "styles",
  "DefaultStyleFilename": "default.json",
  "StylesheetFilename": "style",
  "MetadataFilename": "metadata.json"
}
```

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/sam-is/OgcApi.Net/blob/main/LICENSE) file for details.

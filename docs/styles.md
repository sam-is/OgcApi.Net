---
layout: default
title: Styles Support
nav_order: 8
---
This page documents the OGC API — Styles standard implemented in the `OgcApi.Net.Styles` project.

It describes what the standard provides, the HTTP endpoints exposed by the implementation, the object model used, supported stylesheet formats, storage layout, authorization hooks, and how to configure and register the styles feature in your application.

## What this standard does

The OGC API — Styles module exposes a small REST API for managing, retrieving and describing styling resources (styles and stylesheets) associated with collections (resources). It supports:

- Listing styles available for a collection and the collection's default style.
- Getting a style metadata object or a concrete stylesheet (by format).
- Adding new stylesheets and creating style metadata.
- Updating or replacing stylesheets and style metadata.
- Deleting styles.

The implementation is modular and extensible: you can plug your own storage, metadata backend and authorization service.

## Base routes

All endpoints are mounted under the API base path and are designed to be used per-collection. The controllers in the implementation use this route template:

- Styles collection: `GET /api/ogc/collections/{collectionId}/styles`
- Single style: `GET /api/ogc/collections/{collectionId}/styles/{styleId}`
- Style metadata: `GET /api/ogc/collections/{collectionId}/styles/{styleId}/metadata`

Note: The project also provides OpenAPI entries using shorter paths (without `/api/ogc`) in the generated document (see `StylesOpenApiExtension`). The service controllers are defined with the route prefix `api/ogc/collections/{collectionId}/styles`.

## Endpoints and behaviors

All endpoints may accept an optional `apiKey` query parameter (used by the optional authorization service).

- GET /api/ogc/collections/{collectionId}/styles
	- Returns a JSON `OgcStyles` object describing the list of styles and the default style for the collection.
	- Response codes: 200, 401 (when authorization denies), 500.

- GET /api/ogc/collections/{collectionId}/styles/{styleId}
	- Query parameter: `f` (optional) — when provided returns the stylesheet content in the requested format (e.g. `mapbox`, `sld10`).
	- Without `f` returns the `OgcStyle` object (id, title, links to available stylesheets).
	- Response codes: 200, 401, 404.

- POST /api/ogc/collections/{collectionId}/styles
	- Consumes `application/json`. Body: `StylesheetAddParameters` (styleId, format, content).
	- If a stylesheet in the same format already exists the endpoint returns 409 Conflict.
	- On success it stores the stylesheet and creates a minimal metadata record, returning 201 Created with a Location header.
	- Response codes: 201, 401, 409.

- PUT /api/ogc/collections/{collectionId}/styles/{styleId}
	- Consumes `application/json`. Body: `StylesheetAddParameters` — replaces an existing stylesheet (same styleId & format) content.
	- Response codes: 200, 401, 404, 500.

- PATCH /api/ogc/collections/{collectionId}/styles
	- Consumes `application/merge-patch+json`. Body: `DefaultStyle` — updates the collection's default style identifier.
	- Response codes: 200, 401, 404, 500.

- DELETE /api/ogc/collections/{collectionId}/styles/{styleId}
	- Deletes an existing style (all files for the style).
	- Response codes: 200, 401, 404, 500.

### Metadata endpoints

- GET /api/ogc/collections/{collectionId}/styles/{styleId}/metadata
	- Returns `OgcStyleMetadata` for the style.
	- Response codes: 200, 401, 404.

- PUT /api/ogc/collections/{collectionId}/styles/{styleId}/metadata
	- Replaces full metadata. Consumes `application/json`.
	- Response codes: 200, 401, 500.

- PATCH /api/ogc/collections/{collectionId}/styles/{styleId}/metadata
	- Partial updates using `application/merge-patch+json`.
	- Response codes: 200, 401, 404, 500.

## Object model

These are the main DTOs used in the API and serialization names used in JSON payloads.

- OgcStyles
	- JSON properties:
		- `default` (string | null) — default style id for the collection.
		- `styles` (array of OgcStyle) — list of available styles.

- OgcStyle
	- `id` (string) — style identifier.
	- `title` (string | null) — optional human readable title.
	- `links` (array of Link) — list of links to stylesheet representations. Link objects follow the project's `Link` model (Href, Rel, Type, Title, HrefLang).

- StylesheetAddParameters
	- `styleId` (string, required) — identifier for the style.
	- `format` (string, required) — format name (e.g. `mapbox`, `sld10`, `sld11`).
	- `content` (string, required) — full content of the stylesheet.

- DefaultStyle
	- `default` (string | null) — used to set or read the collection default style.

- OgcStyleMetadata
	- `id` (string), `title`, `description`, `keywords` (string array), `pointOfContact`, `license`, `created` (date-time), `updated` (date-time), `scope` (defaults to `style`), `version`.

Example: a minimal style metadata document

```json
{
	"id": "basic",
	"title": "Basic map style",
	"created": "2024-01-01T00:00:00Z",
	"updated": "2024-01-01T00:00:00Z",
	"scope": "style"
}
```

## Supported stylesheet formats and content types

This implementation includes mappings for the following formats (see `FormatToExtensionMapper` and `FormatToContentType`):

- `mapbox` — file extension `.json`, content type `application/vnd.mapbox.style+json`
- `sld10` — file extension `.xml`, content type `application/vnd.ogc.sld+xml`
- `sld11` — file extension `.xml`, content type `application/vnd.ogc.sld+xml`

If you extend the system with new formats, provide consistent mappings for file extension and content type.

## Storage layout (file system reference implementation)

The repository contains a simple filesystem-backed storage implementation (`StyleFileSystemStorage` and `StyleMetadataFileSystemStorage`) configured via `StyleFileSystemStorageOptions`.

Configuration keys (see `StyleFileSystemStorageOptions`):

- `BaseDirectory` (string) — base directory for all styles (e.g. `styles`).
- `DefaultStyleFilename` (string) — file name used to store default style id for a collection (e.g. `default.json`).
- `StylesheetFilename` (string) — default base name for stylesheet files (e.g. `style`).
- `MetadataFilename` (string) — filename for the style metadata file (e.g. `metadata.json`).

On disk the layout for a collection named `my-collection` looks like:

```
styles/                  # BaseDirectory
└─ my-collection/
	 ├─ default.json        # default style for the collection (DefaultStyle)
	 ├─ basic/              # a style id folder
	 │  ├─ style.mapbox.json   # stylesheet (format-specific extension)
	 │  └─ metadata.json       # OgcStyleMetadata for the style
	 └─ another-style/
			├─ style.sld10.xml
			└─ metadata.json
```

When adding a stylesheet the filesystem storage creates the style folder if missing and writes the stylesheet using the pattern `{StylesheetFilename}.{format}.{extension}` and saves metadata to `{MetadataFilename}`.

## Authorization

Authorization is optional. The library defines the `IStylesAuthorizationService` interface. If you register a concrete implementation using `AddStyleAuthorization<T>()` it will be called for incoming requests. The method should throw `UnauthorizedAccessException` when the request is not permitted; the controllers will catch that and return HTTP 401.

If `UseAuthorization` in `OgcApiStylesOptions` is set to true you should register and implement `IStylesAuthorizationService` to enforce access control.

## Registration and configuration

The `OgcApi.Net.Styles` package exposes extension helpers to wire up services into the DI container. Example (from the sample app):

```csharp
public static IServiceCollection AddSampleOgcStyles(this IServiceCollection services, IConfiguration configuration)
{
		services.AddOgcApiStyles(); // registers OpenAPI and landing links extensions

		// configure options from appsettings.json
		services.Configure<OgcApiStylesOptions>(configuration.GetSection(nameof(OgcApiStylesOptions)));
		services.Configure<StyleFileSystemStorageOptions>(configuration.GetSection(nameof(StyleFileSystemStorageOptions)));

		// register storage implementations
		services.AddStylesStorage<StyleFileSystemStorage>();
		services.AddStylesMetadataStorage<StyleMetadataFileSystemStorage>();

		// optional authorization
		services.AddStyleAuthorization<SampleStylesAuthorizationService>();

		return services;
}
```

Example `appsettings.json` snippet used in the sample application:

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

If you prefer another storage mechanism, implement `IStyleStorage` and `IMetadataStorage` and register them with `AddStylesStorage<T>` and `AddStylesMetadataStorage<T>`.

## OpenAPI and links

The implementation registers OpenAPI schemas and paths via `StylesOpenApiExtension` so the styles endpoints appear in generated API documentation. It also adds a link to the API landing page (`StylesLinksExtension`) with the `rel` set to the OGC styles relation.

## Errors and common response codes

- 200 OK — successful GET/PUT/POST (or as described per-endpoint).
- 201 Created — created resource (POST returns CreatedAtAction to `GetStyle`).
- 401 Unauthorized — returned when an `IStylesAuthorizationService` denies access (by throwing `UnauthorizedAccessException`).
- 404 Not Found — style or metadata not found.
- 409 Conflict — attempt to create a stylesheet that already exists in the same format.
- 500 Internal Server Error — unexpected server error.

## Implementation extension points

- Storage: implement `IStyleStorage` to replace the default filesystem-backed storage.
- Metadata: implement `IMetadataStorage` for storing/fetching metadata differently (DB, object storage, etc.).
- Authorization: implement `IStylesAuthorizationService` to integrate API-key checks or other auth schemes.
- Formats: to add new stylesheet formats, extend `FormatToExtensionMapper` and `FormatToContentType` mappings and ensure your storage and content negotiation handle the new format.

## Quick examples

- Adding a new stylesheet (POST)

Request body (application/json):

```json
{
	"styleId": "basic",
	"format": "mapbox",
	"content": "{ \"version\":8, ... }"
}
```

- Getting the stylesheet content by format (GET)

GET /api/ogc/collections/my-collection/styles/basic?f=mapbox

Response content-type: `application/vnd.mapbox.style+json`

## Notes and edge cases

- The filesystem storage is simple and synchronous for file writes; in high-concurrency production scenarios prefer a storage backend suited for concurrency and transactional guarantees.
- Style and stylesheet existence checks are based on directory and file presence. Deleting a style removes its directory and contents.
- The default style is stored per-collection at `{BaseDirectory}/{collectionId}/{DefaultStyleFilename}`.

## Where to look in the code

- Controllers: `src/Common/Standards/OgcApi.Net.Styles/Controllers/StylesController.cs` and `MetadataController.cs`.
- Options: `src/Common/Standards/OgcApi.Net.Styles/Options/OgcApiStylesOptions.cs` and `Storage/FileSystem/StyleFileSystemStorageOptions.cs`.
- Filesystem storage: `src/Common/Standards/OgcApi.Net.Styles/Storage/FileSystem/StyleFileSystemStorage.cs` and `StyleMetadataFileSystemStorage.cs`.
- OpenAPI & links extensions: `src/Common/Standards/OgcApi.Net.Styles/Extensions/StylesOpenApiExtension.cs` and `StylesLinksExtension.cs`.

## Summary

The `OgcApi.Net.Styles` package implements OGC API — Styles core and management endpoints, provides a filesystem-backed reference storage, OpenAPI support and a pluggable authorization contract. To enable it, call `AddOgcApiStyles`, register storage/metadata/authorization implementations and configure `StyleFileSystemStorageOptions` (or your own storage options) in configuration.


using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options.SqlOptions;
using OgcApi.Net.Features.Resources;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Features.Options
{
    public class OgcApiOptionsConverter : JsonConverter<OgcApiOptions>
    {
        private IServiceProvider Provider { get; set; }
        public OgcApiOptionsConverter(IServiceProvider provider)
        {
            Provider = provider;
        }

        private CollectionsOptions GetCollectionStorageOptions(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {          
            var collectionOptions = new CollectionsOptions();
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var collectionsPropertyName = reader.GetString()!;
                    reader.Read();
                    switch (collectionsPropertyName)
                    {
                        case "Links":
                            collectionOptions.Links = new();
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.String)
                                    collectionOptions.Links.Add(new() { Href = new(reader.GetString()) });
                                reader.Read();
                            }
                            break;
                        case "Items":
                            collectionOptions.Items = new();
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.PropertyName)
                                {
                                    var itemPropertyName = reader.GetString()!;
                                    reader.Read();
                                    if (itemPropertyName == "Collection")
                                    {
                                        var collection = new CollectionOptions();
                                        reader.Read();
                                        while (reader.TokenType != JsonTokenType.EndObject)
                                        {
                                            if (reader.TokenType == JsonTokenType.PropertyName)
                                            {
                                                var collectionPropertyName = reader.GetString()!;
                                                reader.Read();
                                                switch (collectionPropertyName)
                                                {
                                                    case "Id":
                                                        collection.Id = reader.GetString();
                                                        break;
                                                    case "Title":
                                                        collection.Title = reader.GetString();
                                                        break;
                                                    case "Description":
                                                        collection.Description = reader.GetString();
                                                        break;
                                                    case "Links":
                                                        collection.Links = new();
                                                        reader.Read();
                                                        while (reader.TokenType != JsonTokenType.EndArray)
                                                        {
                                                            if (reader.TokenType == JsonTokenType.String)
                                                                collection.Links.Add(new() { Href = new(reader.GetString()) });
                                                            reader.Read();
                                                        }
                                                        break;
                                                    case "ItemType":
                                                        collection.ItemType = reader.GetString();
                                                        break;
                                                    case "Features":
                                                        var features = new CollectionOptionsFeatures();
                                                        reader.Read();
                                                        while (reader.TokenType != JsonTokenType.EndObject)
                                                        {
                                                            if (reader.TokenType == JsonTokenType.PropertyName)
                                                            {
                                                                var featurePropertyName = reader.GetString()!;
                                                                reader.Read();
                                                                switch (featurePropertyName)
                                                                {
                                                                    case "Crs":
                                                                        features.Crs = new();
                                                                        reader.Read();
                                                                        while (reader.TokenType != JsonTokenType.EndArray)
                                                                        {
                                                                            if (reader.TokenType == JsonTokenType.String)
                                                                                features.Crs.Add(new(reader.GetString()));
                                                                            reader.Read();
                                                                        }
                                                                        break;
                                                                    case "StorageCrs":
                                                                        features.StorageCrs = new(reader.GetString());
                                                                        break;
                                                                    case "StorageCrsCoordinateEpoch":
                                                                        features.StorageCrsCoordinateEpoch = reader.GetString();
                                                                        break;
                                                                    case "Storage":
                                                                        var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
                                                                        var type = json.GetProperty("Type").ToString();
                                                                        var dataprovider = Utils.GetDataProvider(Provider, type);
                                                                        features.Storage = dataprovider.DeserializeCollectionSourceOptions(json.ToString(), options);
                                                                        break;
                                                                }
                                                            }
                                                            reader.Read();
                                                        }
                                                        collection.Features = features;
                                                        break;
                                                    case "Extent":
                                                        collection.Extent = JsonSerializer.Deserialize<Extent>(ref reader, options);
                                                        break;
                                                }
                                            }
                                            reader.Read();
                                        }
                                        collectionOptions.Items.Add(collection);
                                    }

                                }
                                reader.Read();
                            }
                            break;
                    }
                    
                }
                reader.Read();
            }                  
            var providers = Provider.GetServices(typeof(IDataProvider));
            foreach (IDataProvider provider in providers)
                provider.SetCollectionOptions(collectionOptions);
            return collectionOptions;
        }
        public override OgcApiOptions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var apiOptions = new OgcApiOptions();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString()!;
                    reader.Read();
                    switch (propertyName)
                    {
                        case "LandingPage":
                            apiOptions.LandingPage = new LandingPageOptions();
                            while (reader.TokenType != JsonTokenType.EndObject)
                            {
                                if (reader.TokenType == JsonTokenType.PropertyName)
                                {
                                    var landingPagePropertyName = reader.GetString()!;
                                    reader.Read();
                                    switch (landingPagePropertyName)
                                    {
                                        case "Title":
                                            apiOptions.LandingPage.Title = reader.GetString();
                                            break;
                                        case "Description":
                                            apiOptions.LandingPage.Description = reader.GetString();
                                            break;
                                        case "ContactName":
                                            apiOptions.LandingPage.ContactName = reader.GetString();
                                            break;
                                        case "ContactUrl":
                                            apiOptions.LandingPage.ContactUrl = new(reader.GetString());
                                            break;
                                        case "ApiDocumentPage":
                                            apiOptions.LandingPage.ApiDocumentPage = new(reader.GetString());
                                            break;
                                        case "Version":
                                            apiOptions.LandingPage.Version = new(reader.GetString());
                                            break;
                                        case "ApiDescriptionPage":
                                            apiOptions.LandingPage.ApiDescriptionPage = new(reader.GetString());
                                            break;
                                        case "LicenseName":
                                            apiOptions.LandingPage.LicenseName = reader.GetString();
                                            break;
                                        case "LicenseUrl":
                                            apiOptions.LandingPage.LicenseUrl = new(reader.GetString());
                                            break;
                                        case "Links":
                                            apiOptions.LandingPage.Links = new();
                                            reader.Read();
                                            while (reader.TokenType != JsonTokenType.EndArray)
                                            {
                                                if (reader.TokenType == JsonTokenType.String)
                                                    apiOptions.LandingPage.Links.Add(new() { Href = new(reader.GetString()) });
                                                reader.Read();
                                            }
                                            break;
                                    }
                                }
                                reader.Read();
                            }
                            break;
                        case "Conformance":
                            apiOptions.Conformance = JsonSerializer.Deserialize<ConformanceOptions>(ref reader, options);
                            break;
                        case "UseApiKeyAuthorization":
                            apiOptions.UseApiKeyAuthorization = reader.GetBoolean();
                            break;
                        case "Collections":
                            apiOptions.Collections = GetCollectionStorageOptions(ref reader, options);
                            break;
                    }
                }
            }
            return apiOptions;
        }

        public override void Write(Utf8JsonWriter writer, OgcApiOptions value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (value.LandingPage != null)
            {
                writer.WriteStartObject("LandingPage");
                writer.WriteString("Title", value.LandingPage.Title);
                writer.WriteString("Description", value.LandingPage.Description);
                writer.WriteString("ContactName", value.LandingPage.ContactName);
                if (value.LandingPage.ContactUrl != null) writer.WriteString("ContactUrl", value.LandingPage.ContactUrl.ToString());
                if (value.LandingPage.ApiDocumentPage != null) writer.WriteString("ApiDocumentPage", value.LandingPage.ApiDocumentPage.ToString());
                if (value.LandingPage.ApiDescriptionPage != null) writer.WriteString("ApiDescriptionPage", value.LandingPage.ApiDescriptionPage.ToString());
                writer.WriteString("LicenseName", value.LandingPage.LicenseName);
                if (value.LandingPage.LicenseUrl != null) writer.WriteString("LicenseUrl", value.LandingPage.LicenseUrl.ToString());
                if (value.LandingPage.Links != null && value.LandingPage.Links.Any())
                {
                    writer.WriteStartArray("Links");
                    foreach (var link in value.LandingPage.Links)
                        writer.WriteStringValue(link.Href.ToString());
                    writer.WriteEndArray();
                }
                writer.WriteEndObject();

            }

            if (value.Conformance != null)
            {
                writer.WriteStartObject("Conformance");
                if (value.Conformance.ConformsTo != null && value.Conformance.ConformsTo.Any())
                {
                    writer.WriteStartArray("ConformsTo");
                    foreach (var link in value.Conformance.ConformsTo)
                        writer.WriteStringValue(link.ToString());
                    writer.WriteEndArray();
                }
                writer.WriteEndObject();
            }

            writer.WriteBoolean(nameof(value.UseApiKeyAuthorization), value.UseApiKeyAuthorization);

            if (value.Collections != null)
            {
                if (value.Collections.Links !=null && value.Collections.Links.Any())
                {
                    writer.WriteStartArray("Links");
                    foreach (var link in value.Collections.Links)
                        writer.WriteStringValue(link.Href.ToString());
                    writer.WriteEndArray();
                }
                if (value.Collections.Items != null && value.Collections.Items.Any())
                {
                    writer.WriteStartArray("Items");
                    foreach (var item in value.Collections.Items)
                    {
                        writer.WriteStartObject();
                        writer.WriteString("Id", item.Id);
                        writer.WriteString("Title", item.Title);
                        writer.WriteString("Description", item.Description);

                        if (item.Links != null && item.Links.Any())
                        {
                            writer.WriteStartArray("Links");
                            foreach (var link in item.Links)
                                writer.WriteStringValue(link.Href.ToString());
                            writer.WriteEndArray();
                        }

                        if (item.Extent != null)
                        {
                            writer.WritePropertyName("Extent");
                            JsonSerializer.Serialize(writer, item.Extent, options);
                        }
                        writer.WriteString("ItemType", item.ItemType);

                        if (item.Features != null)
                        {
                            writer.WriteStartObject("Features");
                            if (item.Features.Crs != null && item.Features.Crs.Any())
                            {
                                writer.WriteStartArray("Crs");
                                foreach (var crs in item.Features.Crs)
                                    writer.WriteStringValue(crs.ToString());
                                writer.WriteEndArray();
                            }
                            writer.WriteString("StorageCrs", item.Features.StorageCrs.ToString());
                            writer.WriteString("StorageCrsCoordinateEpoch", item.Features.StorageCrsCoordinateEpoch);
                            if (item.Features.Storage != null)
                            {
                                writer.WriteStartObject("Storage");
                                writer.WriteString("Type", (item.Features.Storage as SqlCollectionSourceOptions).Type);
                                writer.WriteString("ConnectionString", (item.Features.Storage as SqlCollectionSourceOptions).ConnectionString);
                                writer.WriteString("Schema", (item.Features.Storage as SqlCollectionSourceOptions).Schema);
                                writer.WriteString("Table", (item.Features.Storage as SqlCollectionSourceOptions).Table);
                                writer.WriteString("GeometryColumn", (item.Features.Storage as SqlCollectionSourceOptions).GeometryColumn);
                                writer.WriteString("GeometryDataType", (item.Features.Storage as SqlCollectionSourceOptions).GeometryDataType);
                                writer.WriteString("GeometryGeoJsonType", (item.Features.Storage as SqlCollectionSourceOptions).GeometryGeoJsonType);
                                writer.WriteNumber("GeometrySrid", (item.Features.Storage as SqlCollectionSourceOptions).GeometrySrid);
                                writer.WriteString("DateTimeColumn", (item.Features.Storage as SqlCollectionSourceOptions).DateTimeColumn);
                                writer.WriteString("IdentifierColumn", (item.Features.Storage as SqlCollectionSourceOptions).IdentifierColumn);
                                if (item.Features.Storage.Properties != null && item.Features.Storage.Properties.Any())
                                {
                                    writer.WriteStartArray("Properties");
                                    foreach (var prop in item.Features.Storage.Properties)
                                        writer.WriteStringValue(prop);
                                     writer.WriteEndArray();
                                }
                                writer.WriteBoolean("AllowCreate", (item.Features.Storage as SqlCollectionSourceOptions).AllowCreate);
                                writer.WriteBoolean("AllowReplace", (item.Features.Storage as SqlCollectionSourceOptions).AllowReplace);
                                writer.WriteBoolean("AllowUpdate", (item.Features.Storage as SqlCollectionSourceOptions).AllowUpdate);
                                writer.WriteBoolean("AllowDelete", (item.Features.Storage as SqlCollectionSourceOptions).AllowDelete);
                                writer.WriteString("ApiKeyPredicateForGet", (item.Features.Storage as SqlCollectionSourceOptions).ApiKeyPredicateForGet);
                                writer.WriteString("ApiKeyPredicateForCreate", (item.Features.Storage as SqlCollectionSourceOptions).ApiKeyPredicateForCreate);
                                writer.WriteString("ApiKeyPredicateForUpdate", (item.Features.Storage as SqlCollectionSourceOptions).ApiKeyPredicateForUpdate);
                                writer.WriteString("ApiKeyPredicateForDelete", (item.Features.Storage as SqlCollectionSourceOptions).ApiKeyPredicateForDelete);
                                writer.WriteEndObject();
                            }
                            writer.WriteEndObject();
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }
            }
            writer.WriteEndObject();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options.SqlOptions;
using OgcApi.Net.Features.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Features.Options.Converters
{
    public class OgcApiOptionsConverter : JsonConverter<OgcApiOptions>
    {
        private IServiceProvider Provider { get; set; }
        public OgcApiOptionsConverter(IServiceProvider provider)
        {
            Provider = provider;
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
                            apiOptions.LandingPage = ReadLandingPage(ref reader);
                            break;
                        case "Conformance":
                            apiOptions.Conformance = JsonSerializer.Deserialize<ConformanceOptions>(ref reader, options);
                            break;
                        case "UseApiKeyAuthorization":
                            apiOptions.UseApiKeyAuthorization = reader.GetBoolean();
                            break;
                        case "Collections":
                            apiOptions.Collections = ReadCollections(ref reader, options);
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
                writer.WritePropertyName(nameof(value.LandingPage));
                WriteLandingPage(writer, value.LandingPage);
            }

            if (value.Conformance != null)
            {
                writer.WriteString(nameof(value.Conformance), JsonSerializer.Serialize(value.Conformance, options));
            }

            writer.WriteBoolean(nameof(value.UseApiKeyAuthorization), value.UseApiKeyAuthorization);

            if (value.Collections != null)
            {
                writer.WritePropertyName(nameof(value.Collections));
                WriteCollections(writer, value.Collections, options);
            }
            writer.WriteEndObject();
        }
        public LandingPageOptions ReadLandingPage(ref Utf8JsonReader reader)
        {
            var landingPage = new LandingPageOptions();
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var landingPagePropertyName = reader.GetString()!;
                    reader.Read();
                    switch (landingPagePropertyName)
                    {
                        case "Title":
                            landingPage.Title = reader.GetString();
                            break;
                        case "Description":
                            landingPage.Description = reader.GetString();
                            break;
                        case "ContactName":
                            landingPage.ContactName = reader.GetString();
                            break;
                        case "ContactUrl":
                            landingPage.ContactUrl = new(reader.GetString());
                            break;
                        case "ApiDocumentPage":
                            landingPage.ApiDocumentPage = new(reader.GetString());
                            break;
                        case "Version":
                            landingPage.Version = new(reader.GetString());
                            break;
                        case "ApiDescriptionPage":
                            landingPage.ApiDescriptionPage = new(reader.GetString());
                            break;
                        case "LicenseName":
                            landingPage.LicenseName = reader.GetString();
                            break;
                        case "LicenseUrl":
                            landingPage.LicenseUrl = new(reader.GetString());
                            break;
                        case "Links":
                            landingPage.Links = new();
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.String)
                                    landingPage.Links.Add(new() { Href = new(reader.GetString()) });
                                reader.Read();
                            }
                            break;
                    }
                }
                reader.Read();
            }
            return landingPage;
        }
        public ConformanceOptions ReadConformance(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "Conformance")
                {
                    return (ConformanceOptions)JsonSerializer.Deserialize(ref reader, typeof(ConformanceOptions), options);
                }
            }
            return null;
        }
        public bool ReadUseApiKeyAuthorization(ref Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "UseApiKeyAuthorization")
                {
                    reader.Read();
                    return reader.GetBoolean();
                }

            }
            return false;
        }
        public CollectionsOptions ReadCollections(ref Utf8JsonReader reader, JsonSerializerOptions options)
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
                            collectionOptions.Links = new List<Link>();
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.String)
                                    collectionOptions.Links.Add(new Link { Href = new Uri(reader.GetString()) });
                                reader.Read();
                            }
                            break;
                        case "Items":
                            collectionOptions.Items = new List<CollectionOptions>();
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.StartObject)
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
                                                    collection.Links = new List<Link>();
                                                    reader.Read();
                                                    while (reader.TokenType != JsonTokenType.EndArray)
                                                    {
                                                        if (reader.TokenType == JsonTokenType.String)
                                                            collection.Links.Add(new Link { Href = new Uri(reader.GetString()) });
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
                                                                    features.Crs = new List<Uri>();
                                                                    reader.Read();
                                                                    while (reader.TokenType != JsonTokenType.EndArray)
                                                                    {
                                                                        if (reader.TokenType == JsonTokenType.String)
                                                                            features.Crs.Add(new Uri(reader.GetString()));
                                                                        reader.Read();
                                                                    }
                                                                    break;
                                                                case "StorageCrs":
                                                                    features.StorageCrs = new Uri(reader.GetString());
                                                                    break;
                                                                case "StorageCrsCoordinateEpoch":
                                                                    features.StorageCrsCoordinateEpoch = reader.GetString();
                                                                    break;
                                                                case "Storage":
                                                                    var json = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
                                                                    var type = json.GetProperty("Type").ToString();
                                                                    var dataProvider = Utils.GetDataProvider(Provider, type);
                                                                    features.Storage = dataProvider.DeserializeCollectionSourceOptions(json.ToString(), options);
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
                                reader.Read();
                            }
                            break;
                    }

                }
                reader.Read();
            }
            var providers = Provider.GetServices(typeof(IDataProvider));
            foreach (IDataProvider provider in providers)
                provider.SetCollectionsOptions(collectionOptions);
            return collectionOptions;
        }
        public void WriteCollections(Utf8JsonWriter writer, CollectionsOptions value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (value.Links != null && value.Links.Any())
            {
                writer.WriteStartArray("Links");
                foreach (var link in value.Links)
                    writer.WriteStringValue(link.Href.ToString());
                writer.WriteEndArray();
            }
            if (value.Items != null && value.Items.Any())
            {
                writer.WriteStartArray("Items");
                foreach (var item in value.Items)
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
                        if (item.Features.Storage is SqlCollectionSourceOptions storage)
                        {
                            writer.WriteStartObject("Storage");
                            writer.WriteString("Type", storage.Type);
                            writer.WriteString("ConnectionString", storage.ConnectionString);
                            writer.WriteString("Schema", storage.Schema);
                            writer.WriteString("Table", storage.Table);
                            writer.WriteString("GeometryColumn", storage.GeometryColumn);
                            writer.WriteString("GeometryDataType", storage.GeometryDataType);
                            writer.WriteString("GeometryGeoJsonType", storage.GeometryGeoJsonType);
                            writer.WriteNumber("GeometrySrid", storage.GeometrySrid);
                            writer.WriteString("DateTimeColumn", storage.DateTimeColumn);
                            writer.WriteString("IdentifierColumn", storage.IdentifierColumn);
                            if (storage.Properties != null && storage.Properties.Any())
                            {
                                writer.WriteStartArray("Properties");
                                foreach (var prop in item.Features.Storage.Properties)
                                    writer.WriteStringValue(prop);
                                writer.WriteEndArray();
                            }
                            writer.WriteBoolean("AllowCreate", storage.AllowCreate);
                            writer.WriteBoolean("AllowReplace", storage.AllowReplace);
                            writer.WriteBoolean("AllowUpdate", storage.AllowUpdate);
                            writer.WriteBoolean("AllowDelete", storage.AllowDelete);
                            writer.WriteString("ApiKeyPredicateForGet", storage.ApiKeyPredicateForGet);
                            writer.WriteString("ApiKeyPredicateForCreate", storage.ApiKeyPredicateForCreate);
                            writer.WriteString("ApiKeyPredicateForUpdate", storage.ApiKeyPredicateForUpdate);
                            writer.WriteString("ApiKeyPredicateForDelete", storage.ApiKeyPredicateForDelete);
                            writer.WriteEndObject();
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
        public void WriteLandingPage(Utf8JsonWriter writer, LandingPageOptions value)
        {
            writer.WriteStartObject();
            if (value != null)
            {
                writer.WriteString("Title", value.Title);
                writer.WriteString("Description", value.Description);
                writer.WriteString("ContactName", value.ContactName);
                if (value.ContactUrl != null) writer.WriteString("ContactUrl", value.ContactUrl.ToString());
                if (value.ApiDocumentPage != null) writer.WriteString("ApiDocumentPage", value.ApiDocumentPage.ToString());
                if (value.ApiDescriptionPage != null) writer.WriteString("ApiDescriptionPage", value.ApiDescriptionPage.ToString());
                writer.WriteString("LicenseName", value.LicenseName);
                if (value.LicenseUrl != null) writer.WriteString("LicenseUrl", value.LicenseUrl.ToString());
                if (value.Links != null && value.Links.Any())
                {
                    writer.WriteStartArray("Links");
                    foreach (var link in value.Links)
                        writer.WriteStringValue(link.Href.ToString());
                    writer.WriteEndArray();
                }
            }
            writer.WriteEndObject();
        }
    }
}

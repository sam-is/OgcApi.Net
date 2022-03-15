﻿using OgcApi.Net.Features.Options.SqlOptions;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Features.Options
{
    public class OgcApiOptionsConverter : JsonConverter<OgcApiOptions>
    {
        public override OgcApiOptions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var res = new OgcApiOptions();
            while (reader.Read())
            {
                //if (reader.TokenType == JsonTokenType.EndObject)
                //{
                //    return res;
                //}
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString()!;
                    reader.Read();
                    switch (propertyName)
                    {
                        case "LandingPage":
                            res.LandingPage = new LandingPageOptions();
                            while(reader.TokenType != JsonTokenType.EndObject)
                            {
                                if (reader.TokenType == JsonTokenType.PropertyName)
                                {
                                    var landingPagePropertyName = reader.GetString()!;
                                    reader.Read();
                                    switch(landingPagePropertyName)
                                    {
                                        case "Title":
                                            res.LandingPage.Title = reader.GetString();
                                            break;
                                        case "Description":
                                            res.LandingPage.Description = reader.GetString();
                                            break;
                                        case "ContactName":
                                            res.LandingPage.ContactName = reader.GetString();
                                            break;
                                        case "ContactUrl":
                                            res.LandingPage.ContactUrl = new(reader.GetString());
                                            break;
                                        case "ApiDocumentPage":
                                            res.LandingPage.ApiDocumentPage = new(reader.GetString());
                                            break;
                                        case "ApiDescriptionPage":
                                            res.LandingPage.ApiDescriptionPage = new(reader.GetString());
                                            break;
                                        case "LicenseName":
                                            res.LandingPage.LicenseName = reader.GetString();
                                            break;
                                        case "LicenseUrl":
                                            res.LandingPage.LicenseUrl = new(reader.GetString());
                                            break;
                                        case "Links":
                                            res.LandingPage.Links = new();
                                            
                                            while(reader.TokenType != JsonTokenType.EndArray)
                                            {

                                            }
                                            break;
                                    }
                                }
                                reader.Read();
                            }
                            break;
                        case "Conformance":
                            res.Conformance = new ConformanceOptions();
                            break;
                        case "UseApiKeyAuthorization":
                            res.UseApiKeyAuthorization = reader.GetBoolean();
                            break;
                        case "Collections":
                            res.Collections = new CollectionsOptions();
                            break;
                    }
                }
            }
            return res;
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
                writer.WriteString("ContactUrl", value.LandingPage.ContactUrl.ToString());
                writer.WriteString("ApiDocumentPage", value.LandingPage.ApiDocumentPage.ToString());
                writer.WriteString("ApiDescriptionPage", value.LandingPage.ApiDescriptionPage.ToString());
                writer.WriteString("LicenseName", value.LandingPage.LicenseName);
                writer.WriteString("LicenseUrl", value.LandingPage.LicenseUrl.ToString());
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
                            JsonSerializer.Serialize(writer, item.Extent, options);

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
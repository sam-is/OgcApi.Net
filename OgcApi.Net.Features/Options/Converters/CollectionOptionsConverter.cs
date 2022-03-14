using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using OgcApi.Net.Features.Options.SqlOptions;

namespace OgcApi.Net.Features.Options.Converters
{
    public class CollectionOptionsConverter : JsonConverter<CollectionOptions>
    {
        public override CollectionOptions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, CollectionOptions value, JsonSerializerOptions options)
        {
            writer.WriteStartObject("Collection");
            writer.WriteString(nameof(value.Id), value.Id);
            writer.WriteString(nameof(value.Title), value.Title);
            writer.WriteString(nameof(value.Description), value.Description);

            if (value.Links.Any())
            {
                writer.WriteStartArray("Links");
                foreach (var link in value.Links)
                    writer.WriteStringValue(link.Href.ToString());
                writer.WriteEndArray();
            }

            if (value.Extent != null)
                JsonSerializer.Serialize(writer, value.Extent, options);

            writer.WriteString("ItemType", value.ItemType);

            if (value.Features !=null)
            {
                writer.WriteStartObject("Features");
                if (value.Features.Crs.Any())
                {
                    writer.WriteStartArray("Crs");
                    foreach (var crs in value.Features.Crs)
                        JsonSerializer.Serialize(writer, crs, options);
                    writer.WriteEndArray();
                }
                JsonSerializer.Serialize(writer, value.Features.StorageCrs, options);
                writer.WriteString("StorageCrsCoordinateEpoch", value.Features.StorageCrsCoordinateEpoch);
                if(value.Features.Storage !=null)
                {
                    writer.WriteStartObject("Storage");
                    writer.WriteString("Type", (value.Features.Storage as SqlCollectionSourceOptions).Type);
                    writer.WriteString("ConnectionString", (value.Features.Storage as SqlCollectionSourceOptions).ConnectionString);
                    writer.WriteString("Schema", (value.Features.Storage as SqlCollectionSourceOptions).Schema);
                    writer.WriteString("Table", (value.Features.Storage as SqlCollectionSourceOptions).Table);
                    writer.WriteString("GeometryColumn", (value.Features.Storage as SqlCollectionSourceOptions).GeometryColumn);
                    writer.WriteString("GeometryDataType", (value.Features.Storage as SqlCollectionSourceOptions).GeometryDataType);
                    writer.WriteString("GeometryGeoJsonType", (value.Features.Storage as SqlCollectionSourceOptions).GeometryGeoJsonType);
                    writer.WriteNumber("GeometrySrid", (value.Features.Storage as SqlCollectionSourceOptions).GeometrySrid);
                    writer.WriteString("DateTimeColumn", (value.Features.Storage as SqlCollectionSourceOptions).DateTimeColumn);
                    writer.WriteString("IdentifierColumn", (value.Features.Storage as SqlCollectionSourceOptions).IdentifierColumn);
                    if (value.Features.Storage.Properties.Any())
                    {
                        writer.WriteStartArray("Properties");
                        foreach (var prop in value.Features.Storage.Properties)
                            JsonSerializer.Serialize(writer, prop, options);
                        writer.WriteEndArray();
                    }
                    writer.WriteBoolean("AllowCreate", (value.Features.Storage as SqlCollectionSourceOptions).AllowCreate);
                    writer.WriteBoolean("AllowReplace", (value.Features.Storage as SqlCollectionSourceOptions).AllowReplace);
                    writer.WriteBoolean("AllowUpdate", (value.Features.Storage as SqlCollectionSourceOptions).AllowUpdate);
                    writer.WriteBoolean("AllowDelete", (value.Features.Storage as SqlCollectionSourceOptions).AllowDelete);
                    writer.WriteString("ApiKeyPredicateForGet", (value.Features.Storage as SqlCollectionSourceOptions).ApiKeyPredicateForGet);
                    writer.WriteString("ApiKeyPredicateForCreate", (value.Features.Storage as SqlCollectionSourceOptions).ApiKeyPredicateForCreate);
                    writer.WriteString("ApiKeyPredicateForUpdate", (value.Features.Storage as SqlCollectionSourceOptions).ApiKeyPredicateForUpdate);
                    writer.WriteString("ApiKeyPredicateForDelete", (value.Features.Storage as SqlCollectionSourceOptions).ApiKeyPredicateForDelete);
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }
    }
}

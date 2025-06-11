using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using OgcApi.Net.Schemas.Options;
using OgcApi.Net.Schemas.Schema.Model;

namespace OgcApi.Net.Schemas.Schema;
public class SchemaGenerator() : ISchemaGenerator
{
    private const string PrimaryGeometryXOgcRole = "primary-geometry";

    private const string IdXOgcRole = "id";

    private const string DateTimeFormat = "date-time";
    public OgcJsonSchema GenerateSchema(Uri baseUri, CollectionOptions collectionOptions)
    {
        var schemaOptions = GetSchemaOptions(collectionOptions);

        var properties = CastProperties(schemaOptions.Properties);

        var schema = new OgcJsonSchema
        {
            Id = new Uri(baseUri, $"collections/{collectionOptions.Id}/schema"),
            Title = schemaOptions.Title,
            Description = schemaOptions.Description,
            AdditionalProperties = schemaOptions.AdditionalProperties,
            Properties = PrepareProperties(properties, collectionOptions)
        };

        return schema;
    }

    public OgcJsonSchema GenerateQueryablesSchema(Uri baseUri, CollectionOptions collectionOptions)
    {
        throw new NotImplementedException();
    }

    public OgcJsonSchema GenerateSortablesSchema(Uri baseUri, CollectionOptions collectionOptions)
    {
        throw new NotImplementedException();
    }

    private SchemaOptions GetSchemaOptions(CollectionOptions collectionOptions)
    {
        if (collectionOptions is SchemaCollectionOptions schemaCollectionOptions && schemaCollectionOptions.SchemaOptions != null)
            return schemaCollectionOptions.SchemaOptions;

        return new SchemaOptions { Properties = [] };
    }

    private static Dictionary<string, OgcJsonSchemaProperty> CastProperties(Dictionary<string, PropertyDescription> properties)
    {
        var result = new Dictionary<string, OgcJsonSchemaProperty>();

        foreach (var kvp in properties)
        {
            var propDesc = kvp.Value;

            var schemaProp = new OgcJsonSchemaProperty
            {
                XOgcRole = propDesc.XOgcRole,
                Type = propDesc.Type,
                Title = propDesc.Title,
                Format = propDesc.Format,
                XOgcPropertySeq = propDesc.XOgcPropertySeq
            };

            result[kvp.Key] = schemaProp;
        }

        return result;
    }

    private Dictionary<string, OgcJsonSchemaProperty> PrepareProperties(Dictionary<string, OgcJsonSchemaProperty> properties, CollectionOptions collectionOptions)
    {
        if (!properties.Where(p => p.Value.XOgcRole == PrimaryGeometryXOgcRole).Any())
            properties = AddOrUpdateGeometryProperty(properties, collectionOptions);

        if (!properties.Where(p => p.Value.XOgcRole == IdXOgcRole).Any())
            properties = AddOrUpdateIdProperty(properties, collectionOptions);

        if (collectionOptions.Features.Storage is SqlFeaturesSourceOptions sqlFeaturesSourceOptions && sqlFeaturesSourceOptions.DateTimeColumn != null)
        {
            var dateTimeColumn = properties.FirstOrDefault(p => p.Key == sqlFeaturesSourceOptions.DateTimeColumn);

            if (dateTimeColumn.Key != null && dateTimeColumn.Value.Type == null)
                dateTimeColumn.Value.Format = DateTimeFormat;
        }

        var withoutTypeProperties = properties.Where(p => p.Value.Type == null);

        foreach (var (name, schemaProperty) in withoutTypeProperties)
        {

        }

        return properties;
    }

    private Dictionary<string, OgcJsonSchemaProperty> AddOrUpdateGeometryProperty(Dictionary<string, OgcJsonSchemaProperty> properties, CollectionOptions collectionOptions)
    {
        if (collectionOptions.Features.Storage is SqlFeaturesSourceOptions sqlFeaturesSourceOptions)
        {
            var geometryProperty = properties.FirstOrDefault(p => p.Key == sqlFeaturesSourceOptions.GeometryColumn);
            if (geometryProperty.Key == null)
            {
                properties.Add(sqlFeaturesSourceOptions.GeometryColumn, new OgcJsonSchemaProperty
                {
                    XOgcRole = PrimaryGeometryXOgcRole,
                    Format = GetGeometryFormat(sqlFeaturesSourceOptions.GeometryGeoJsonType)
                });
            }
            else if (geometryProperty.Value.XOgcRole == null)
            {
                geometryProperty.Value.XOgcRole = PrimaryGeometryXOgcRole;
                geometryProperty.Value.Format = GetGeometryFormat(sqlFeaturesSourceOptions.GeometryGeoJsonType);
            }
        }

        return properties;
    }

    private Dictionary<string, OgcJsonSchemaProperty> AddOrUpdateIdProperty(Dictionary<string, OgcJsonSchemaProperty> properties, CollectionOptions collectionOptions)
    {
        if (collectionOptions.Features.Storage is SqlFeaturesSourceOptions sqlFeaturesSourceOptions)
        {
            var idProperty = properties.FirstOrDefault(p => p.Key == sqlFeaturesSourceOptions.IdentifierColumn);
            if (idProperty.Key == null)
            {
                properties.Add(sqlFeaturesSourceOptions.IdentifierColumn, new OgcJsonSchemaProperty
                {
                    XOgcRole = IdXOgcRole,
                    Format = GetGeometryFormat(sqlFeaturesSourceOptions.GeometryGeoJsonType)
                });
            }
            else if (idProperty.Value.XOgcRole == null)
            {
                idProperty.Value.XOgcRole = IdXOgcRole;
            }
        }

        return properties;
    }

    private static string GetGeometryFormat(string geometryGeoJsonType) => geometryGeoJsonType switch
    {
        "Point" => "geometry-point",
        "MultiPoint" => "geometry-multipoint",
        "LineString" => "geometry-linestring",
        "MultiLineString" => "geometry-multilinestring",
        "Polygon" => "geometry-polygon",
        "MultiPolygon" => "geometry-multipolygon",
        "GeometryCollection" => "geometry-geometrycollection",
        _ => "geometry-any"
    };
}

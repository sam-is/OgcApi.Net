using OgcApi.Net.DataProviders;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using OgcApi.Net.Schemas.Options;
using OgcApi.Net.Schemas.Schema.Model;

namespace OgcApi.Net.Schemas.Schema;
public class SchemaGenerator(IFeaturesProvider? featureProvider, ITilesProvider? tilesProvider) : ISchemaGenerator
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
        var schemaOptions = GetSchemaOptions(collectionOptions);

        var properties = CastProperties(schemaOptions.Properties);

        var schema = new OgcJsonSchema
        {
            Id = new Uri(baseUri, $"collections/{collectionOptions.Id}/queryables"),
            Title = schemaOptions.Title,
            Description = schemaOptions.Description,
            AdditionalProperties = schemaOptions.AdditionalProperties,
            Properties = PrepareProperties(properties, collectionOptions)
        };

        return schema;
    }

    public OgcJsonSchema GenerateSortablesSchema(Uri baseUri, CollectionOptions collectionOptions)
    {
        var schemaOptions = GetSchemaOptions(collectionOptions);

        var schema = new OgcJsonSchema
        {
            Id = new Uri(baseUri, $"collections/{collectionOptions.Id}/sortables"),
            Title = schemaOptions.Title,
            Description = schemaOptions.Description,
            AdditionalProperties = schemaOptions.AdditionalProperties,
            Properties = []
        };

        return schema;
    }

    private SchemaOptions GetSchemaOptions(CollectionOptions collectionOptions)
    {
        if (collectionOptions is SchemaCollectionOptions schemaCollectionOptions && schemaCollectionOptions.SchemaOptions != null)
            return schemaCollectionOptions.SchemaOptions;

        var propertyMetadata = GetPropertyMetadata(collectionOptions.Id);

        var properties = new Dictionary<string, PropertyDescription>();

        foreach (var metadata in propertyMetadata)
        {
            if (metadata.Key == "geometry")
                properties.Add(metadata.Key, new PropertyDescription
                {
                    Format = GetGeometryFormat(metadata.Value),
                    XOgcRole = PrimaryGeometryXOgcRole
                });
            else
                properties.Add(metadata.Key, new PropertyDescription { Type = metadata.Value });
        }

        return new SchemaOptions { Properties = properties };
    }

    private static Dictionary<string, OgcJsonSchemaProperty> CastProperties(Dictionary<string, PropertyDescription> properties)
    {
        var result = new Dictionary<string, OgcJsonSchemaProperty>();

        foreach (var property in properties)
        {
            var propertyData = property.Value;

            result[property.Key] = new OgcJsonSchemaProperty
            {
                XOgcRole = propertyData.XOgcRole,
                Type = propertyData.Type,
                Title = propertyData.Title,
                Description = propertyData.Description,
                Format = propertyData.Format,
                XOgcPropertySeq = propertyData.XOgcPropertySeq
            };
        }

        return result;
    }

    private Dictionary<string, OgcJsonSchemaProperty> PrepareProperties(Dictionary<string, OgcJsonSchemaProperty> properties, CollectionOptions collectionOptions)
    {
        if (collectionOptions.Features?.Storage is SqlFeaturesSourceOptions sqlFeaturesSourceOptions)
        {
            if (sqlFeaturesSourceOptions.DateTimeColumn != null)
            {
                var dateTimeColumn = properties.FirstOrDefault(p => p.Key == sqlFeaturesSourceOptions.DateTimeColumn);

                if (dateTimeColumn.Key != null && dateTimeColumn.Value.Type == null)
                    dateTimeColumn.Value.Format = DateTimeFormat;
            }

            if (sqlFeaturesSourceOptions.Properties != null && sqlFeaturesSourceOptions.Properties.Count > 0)
            {
                foreach (var property in properties)
                {
                    if (!sqlFeaturesSourceOptions.Properties.Contains(property.Key))
                        properties.Remove(property.Key);
                }
            }
        }

        if (!properties.Where(p => p.Value.XOgcRole == PrimaryGeometryXOgcRole).Any())
            properties = AddOrUpdateGeometryProperty(properties, collectionOptions);

        if (!properties.Where(p => p.Value.XOgcRole == IdXOgcRole).Any())
            properties = AddOrUpdateIdProperty(properties, collectionOptions);

        var withoutTypeProperties = properties.Where(p => p.Value.Type == null && p.Value.XOgcRole != PrimaryGeometryXOgcRole);

        var propertyMetadata = GetPropertyMetadata(collectionOptions.Id);

        foreach (var (name, schemaProperty) in withoutTypeProperties)
        {
            if (propertyMetadata.TryGetValue(name, out string? value))
                schemaProperty.Type = value;
        }

        return properties;
    }

    private Dictionary<string, OgcJsonSchemaProperty> AddOrUpdateGeometryProperty(Dictionary<string, OgcJsonSchemaProperty> properties, CollectionOptions collectionOptions)
    {
        if (collectionOptions.Features?.Storage is SqlFeaturesSourceOptions sqlFeaturesSourceOptions)
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
                geometryProperty.Value.Type = null;
            }
        }

        return properties;
    }

    private Dictionary<string, OgcJsonSchemaProperty> AddOrUpdateIdProperty(Dictionary<string, OgcJsonSchemaProperty> properties, CollectionOptions collectionOptions)
    {
        if (collectionOptions.Features?.Storage is SqlFeaturesSourceOptions sqlFeaturesSourceOptions)
        {
            var idProperty = properties.FirstOrDefault(p => p.Key == sqlFeaturesSourceOptions.IdentifierColumn);
            if (idProperty.Key == null)
            {
                properties.Add(sqlFeaturesSourceOptions.IdentifierColumn, new OgcJsonSchemaProperty
                {
                    XOgcRole = IdXOgcRole
                });
            }
            else if (idProperty.Value.XOgcRole == null)
            {
                idProperty.Value.XOgcRole = IdXOgcRole;
            }
        }

        return properties;
    }

    private Dictionary<string, string> GetPropertyMetadata(string collectionId)
    {
        if (featureProvider != null && featureProvider is IPropertyMetadataProvider featuresMetadataProvider)
        {
            var metadata = featuresMetadataProvider.GetPropertyMetadata(collectionId);
            if (metadata != null)
                return metadata;
        }

        if (tilesProvider != null && tilesProvider is IPropertyMetadataProvider tilesMetadataProvider)
        {
            var metadata = tilesMetadataProvider.GetPropertyMetadata(collectionId);
            if (metadata != null)
                return metadata;
        }

        return [];
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

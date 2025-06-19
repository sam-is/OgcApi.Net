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

        foreach (var (propertyName, propertyDescription) in properties)
        {
            result[propertyName] = new OgcJsonSchemaProperty
            {
                XOgcRole = propertyDescription.XOgcRole,
                Type = propertyDescription.Type,
                Title = propertyDescription.Title,
                Description = propertyDescription.Description,
                Format = propertyDescription.Format,
                XOgcPropertySeq = propertyDescription.XOgcPropertySeq
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

                if (dateTimeColumn is { Key: not null, Value.Format: null })
                    dateTimeColumn.Value.Format = DateTimeFormat;
            }

            if (sqlFeaturesSourceOptions.Properties is { Count: > 0 })
            {
                foreach (var property in properties)
                {
                    if (property.Key == sqlFeaturesSourceOptions.IdentifierColumn || property.Key == sqlFeaturesSourceOptions.GeometryColumn)
                        continue;

                    if (!sqlFeaturesSourceOptions.Properties.Contains(property.Key))
                        properties.Remove(property.Key);
                }
            }
        }

        if (properties.All(p => p.Value.XOgcRole != PrimaryGeometryXOgcRole))
            properties = AddOrUpdateGeometryProperty(properties, collectionOptions);

        if (properties.All(p => p.Value.XOgcRole != IdXOgcRole))
            properties = AddOrUpdateIdProperty(properties, collectionOptions);

        var withoutTypeProperties = properties.Where(p => p.Value.Type == null && p.Value.XOgcRole != PrimaryGeometryXOgcRole);

        var propertyMetadata = GetPropertyMetadata(collectionOptions.Id);

        foreach (var (name, schemaProperty) in withoutTypeProperties)
        {
            if (propertyMetadata.TryGetValue(name, out var value))
                schemaProperty.Type = value;
        }

        return properties;
    }

    private static Dictionary<string, OgcJsonSchemaProperty> AddOrUpdateGeometryProperty(Dictionary<string, OgcJsonSchemaProperty> properties, CollectionOptions collectionOptions)
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
        if (featureProvider is IPropertyMetadataProvider featuresMetadataProvider)
        {
            var metadata = featuresMetadataProvider.GetPropertyMetadata(collectionId);
            if (metadata != null)
                return metadata;
        }

        if (tilesProvider is IPropertyMetadataProvider tilesMetadataProvider)
        {
            var metadata = tilesMetadataProvider.GetPropertyMetadata(collectionId);
            if (metadata != null)
                return metadata;
        }

        return [];
    }

    private static string GetGeometryFormat(string? geometryType) => geometryType?.ToLower() switch
    {
        "point" => "geometry-point",
        "multipoint" => "geometry-multipoint",
        "linestring" => "geometry-linestring",
        "multilinestring" => "geometry-multilinestring",
        "polygon" => "geometry-polygon",
        "multipolygon" => "geometry-multipolygon",
        "geometrycollection" => "geometry-geometrycollection",
        _ => "geometry-any"
    };
}

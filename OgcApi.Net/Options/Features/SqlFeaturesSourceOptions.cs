using System.Collections.Generic;

namespace OgcApi.Net.Options.Features;

public class SqlFeaturesSourceOptions : IFeaturesSourceOptions
{
    public string Type { get; set; }

    public string ConnectionString { get; set; }

    public string Schema { get; set; }

    public string Table { get; set; }

    public string GeometryColumn { get; set; }

    public string GeometryDataType { get; set; }

    public string GeometryGeoJsonType { get; set; }

    public int GeometrySrid { get; set; } = 0;

    public string DateTimeColumn { get; set; }

    public string IdentifierColumn { get; set; }

    public List<string> Properties { get; set; }

    public bool AllowCreate { get; set; }

    public bool AllowReplace { get; set; }

    public bool AllowUpdate { get; set; }

    public bool AllowDelete { get; set; }

    public string ApiKeyPredicateForGet { get; set; }

    public string ApiKeyPredicateForCreate { get; set; }

    public string ApiKeyPredicateForUpdate { get; set; }

    public string ApiKeyPredicateForDelete { get; set; }

    public List<string> Validate()
    {
        var failureMessages = new List<string>();

        if (string.IsNullOrWhiteSpace(ConnectionString))
            failureMessages.Add("Parameter ConnectionString is required for the collection feature storage option");

        if (string.IsNullOrWhiteSpace(Schema))
            failureMessages.Add("Parameter Schema is required for the collection feature storage option");

        if (string.IsNullOrWhiteSpace(Table))
            failureMessages.Add("Parameter Table is required for the collection feature storage option");

        if (string.IsNullOrWhiteSpace(GeometryColumn))
            failureMessages.Add("Parameter GeometryColumn is required for the feature storage collection option");

        if (string.IsNullOrWhiteSpace(IdentifierColumn))
            failureMessages.Add("Parameter IdentifierColumn is required for the feature storage collection option");

        if (GeometryDataType != "geometry" && GeometryDataType != "geography")
            failureMessages.Add("Parameter DataType must be 'geometry' or 'geography'");

        return failureMessages;
    }
}
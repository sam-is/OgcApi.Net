using System.Collections.Generic;

namespace OgcApi.Net.Options.Features;

public interface IFeaturesSourceOptions
{
    public string Type { get; set; }

    public string GeometryGeoJsonType { get; set; }

    public List<string> Properties { get; set; }

    public bool AllowCreate { get; set; }

    public bool AllowReplace { get; set; }

    public bool AllowUpdate { get; set; }

    public bool AllowDelete { get; set; }

    List<string> Validate();
}
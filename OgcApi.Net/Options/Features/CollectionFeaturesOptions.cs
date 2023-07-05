using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Options.Features;

public class CollectionFeaturesOptions
{
    public List<Uri> Crs { get; set; }

    public Uri StorageCrs { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string StorageCrsCoordinateEpoch { get; set; }

    public IFeaturesSourceOptions Storage { get; set; }
}
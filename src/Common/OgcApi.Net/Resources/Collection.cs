using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Resources;

public class Collection
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public List<Link> Links { get; set; }

    public Extent Extent { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ItemType { get; set; }

    public List<Uri> Crs { get; set; }

    public Uri StorageCrs { get; set; }

    public string StorageCrsCoordinateEpoch { get; set; }
}
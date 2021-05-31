using OgcApi.Net.Features.Resources;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Features.Options
{
    public class CollectionOptions
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Link> Links { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Extent Extent { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ItemType { get; set; }

        public List<Uri> Crs { get; set; }

        public Uri StorageCrs { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string StorageCrsCoordinateEpoch { get; set; }

        public string SourceType { get; set; }

        public Func<string, Uri> FeatureHtmlPage { get; set; }
    }
}

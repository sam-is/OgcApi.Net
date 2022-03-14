using OgcApi.Net.Features.Options.SqlOptions;
using OgcApi.Net.Features.Resources;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OgcApi.Net.Features.Options.Interfaces;

namespace OgcApi.Net.Features.Options
{
    public class CollectionOptions : ICollectionOptions
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
        public Func<string, Uri> FeatureHtmlPage { get; set; }

        public CollectionOptionsFeatures Features { get; set; }

    }
}

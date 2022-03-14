using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Resources;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OgcApi.Net.Features.Options.Interfaces;

namespace OgcApi.Net.Features.Options
{
    public class CollectionOptionsFeatures
    {
        public List<Uri> Crs { get; set; } 
        public Uri StorageCrs { get; set; } 

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string StorageCrsCoordinateEpoch { get; set; } 

        public ICollectionSourceOptions Storage { get; set; }
    }
}

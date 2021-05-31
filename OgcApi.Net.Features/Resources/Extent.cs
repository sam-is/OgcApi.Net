using System.Text.Json.Serialization;

namespace OgcApi.Net.Features.Resources
{
    public class Extent
    {
        public SpatialExtent Spatial { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TemporalExtent Temporal { get; set; }
    }
}

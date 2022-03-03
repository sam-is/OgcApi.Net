using System.Collections.Generic;

namespace OgcApi.Net.Options
{
    public interface ICollectionSourceOptions
    {
        public string Id { get; set; }

        public string GeometryGeoJsonType { get; set; }

        public List<string> Properties { get; set; }
    }
}

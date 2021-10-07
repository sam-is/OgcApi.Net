using System.Collections.Generic;

namespace OgcApi.Net.Features.DataProviders
{
    public interface ICollectionSource
    {
        public string Id { get; set; }

        public string GeometryGeoJsonType { get; set; }

        public List<string> Properties { get; set; }
    }
}

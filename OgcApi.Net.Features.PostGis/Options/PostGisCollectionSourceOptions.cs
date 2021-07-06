using OgcApi.Net.Features.DataProviders;
using System.Collections.Generic;

namespace OgcApi.Net.Features.PostGis.Options
{
    public class PostGisCollectionSourceOptions : ICollectionSource
    {
        public string Id { get; set; }

        public string ConnectionString { get; set; }

        public string Schema { get; set; }

        public string Table { get; set; }

        public string GeometryColumn { get; set; }

        public int GeometrySrid { get; set; } = 0;

        public string DateTimeColumn { get; set; }

        public string IdentifierColumn { get; set; }

        public List<string> Properties { get; set; }

        public string ApiKeyPredicate { get; set; }
    }
}
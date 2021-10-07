using OgcApi.Net.Features.DataProviders;
using System.Collections.Generic;

namespace OgcApi.Net.Features.Options
{
    public class SqlCollectionSourceOptions : ICollectionSource
    {
        public string Id { get; set; }

        public string ConnectionString { get; set; }

        public string Schema { get; set; }

        public string Table { get; set; }

        public string GeometryColumn { get; set; }

        public string GeometryDataType { get; set; }

        public string GeometryGeoJsonType { get; set; }

        public int GeometrySrid { get; set; } = 0;

        public string DateTimeColumn { get; set; }

        public string IdentifierColumn { get; set; }

        public List<string> Properties { get; set; }

        public string ApiKeyPredicateForGet { get; set; }

        public string ApiKeyPredicateForCreate { get; set; }

        public string ApiKeyPredicateForUpdate { get; set; }

        public string ApiKeyPredicateForDelete { get; set; }
    }
}
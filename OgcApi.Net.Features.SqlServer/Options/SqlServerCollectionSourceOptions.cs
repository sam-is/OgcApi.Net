using OgcApi.Net.Features.DataProviders;
using System.Collections.Generic;

namespace OgcApi.Net.Features.SqlServer.Options
{
    public class SqlServerCollectionSourceOptions : ICollectionSource
    {
        public string Id { get; set; }

        public string ConnectionString { get; set; }

        public string Schema { get; set; }

        public string Table { get; set; }

        public string GeometryColumn { get; set; }

        public string GeometryDataType { get; set; }

        public int GeometrySrid { get; set; } = 0;

        public string DateTimeColumn { get; set; }

        public string IdentifierColumn { get; set; }

        public List<string> Properties { get; set; }

        public string ApiKeyPredicate { get; set; }
    }
}
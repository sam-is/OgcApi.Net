using System.Collections.Generic;
using OgcApi.Net.Features.DataProviders;

namespace OgcApi.Net.Features.Options
{
    public class SqlCollectionSourcesOptions : ICollectionSources
    {
        public List<SqlCollectionSourceOptions> Sources { get; set; }

        public ICollectionSource GetSourceById(string id)
        {
            return Sources.Find(x => x.Id == id);
        }
    }
}

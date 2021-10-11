using System.Collections.Generic;

namespace OgcApi.Net.Features.Options.SqlOptions
{
    public class SqlCollectionSourcesOptions : ICollectionSourcesOptions
    {
        public List<SqlCollectionSourceOptions> Sources { get; set; }

        public ICollectionSourceOptions GetSourceById(string id)
        {
            return Sources.Find(x => x.Id == id);
        }
    }
}

using OgcApi.Net.Resources;
using System.Collections.Generic;

namespace OgcApi.Net.Options
{
    public class CollectionsOptions
    {
        public List<Link> Links { get; set; }

        public List<CollectionOptions> Items { get; set; }
    }
}

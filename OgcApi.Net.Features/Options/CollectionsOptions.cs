using OgcApi.Net.Features.Resources;
using System.Collections.Generic;

namespace OgcApi.Net.Features.Options
{
    public class CollectionsOptions
    {
        public List<Link> Links { get; set; }

        public List<CollectionOptions> Items { get; set; }
    }
}

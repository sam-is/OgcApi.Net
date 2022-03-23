using OgcApi.Net.Features.Options.Interfaces;
using OgcApi.Net.Features.Resources;
using System.Collections.Generic;

namespace OgcApi.Net.Options
{
    public class CollectionsOptions : ICollectionsOptions
    {
        public List<Link> Links { get; set; }

        public List<CollectionOptions> Items { get; set; }

        public ICollectionOptions GetSourceById(string id)
        {
            return Items.Find(x => x.Id == id);
        }
    }
}

using OgcApi.Net.Options.Interfaces;
using OgcApi.Net.Resources;
using System.Collections.Generic;

namespace OgcApi.Net.Options;

public class CollectionsOptions : ICollectionsOptions
{
    public List<Link> Links { get; set; }

    public List<CollectionOptions> Items { get; set; }

    public ICollectionOptions GetSourceById(string id)
    {
        return Items.Find(x => x.Id == id);
    }
}
using OgcApi.Net.Options.Features;
using OgcApi.Net.Options.Tiles;

namespace OgcApi.Net.Options.Interfaces;

public interface ICollectionOptions
{
    public string Id { get; set; }

    public CollectionFeaturesOptions Features { get; set; }

    public CollectionTilesOptions Tiles { get; set; }
}
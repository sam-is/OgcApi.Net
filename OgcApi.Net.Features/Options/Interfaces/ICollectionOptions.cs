namespace OgcApi.Net.Features.Options.Interfaces
{
    public interface ICollectionOptions
    {
        public string Id { get; set; }
        public CollectionOptionsFeatures Features { get; set; }
    }
}

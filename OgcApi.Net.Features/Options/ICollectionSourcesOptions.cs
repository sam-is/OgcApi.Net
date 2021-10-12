namespace OgcApi.Net.Features.Options
{
    public interface ICollectionSourcesOptions
    {
        ICollectionSourceOptions GetSourceById(string id);
    }
}

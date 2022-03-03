namespace OgcApi.Net.Options
{
    public interface ICollectionSourcesOptions
    {
        ICollectionSourceOptions GetSourceById(string id);
    }
}

namespace OgcApi.Net.Options.Interfaces;

public interface ICollectionsOptions
{
    ICollectionOptions GetSourceById(string id);
}
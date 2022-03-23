using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System;
using OgcApi.Net.Options;
using OgcApi.Net.Features;

namespace OgcApi.Net.DataProviders
{
    public interface IFeaturesProvider
    {
        string SourceType { get; }

        ICollectionSourcesOptions GetCollectionSourcesOptions();

        Envelope GetBbox(string collectionId, string apiKey = null);

        OgcFeatureCollection GetFeatures(string collectionId, int limit = 10, int offset = 0, Envelope bbox = null, DateTime? startDateTime = null, DateTime? endDateTime = null, string apiKey = null);

        OgcFeature GetFeature(string collectionId, string featureId, string apiKey = null);

        long GetFeaturesCount(string collectionId, Envelope bbox = null, DateTime? startDateTime = null, DateTime? endDateTime = null, string apiKey = null);

        string CreateFeature(string collectionId, IFeature feature, string apiKey = null);

        void UpdateFeature(string collectionId, string featureId, IFeature feature, string apiKey = null);

        void ReplaceFeature(string collectionId, string featureId, IFeature feature, string apiKey = null);

        void DeleteFeature(string collectionId, string featureId, string apiKey = null);
    }
}

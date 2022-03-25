using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OgcApi.Net.Features;
using OgcApi.Net.Options.Features;
using OgcApi.Net.Options.Interfaces;
using System;
using System.Text.Json;

namespace OgcApi.Net.DataProviders
{
    public interface IFeaturesProvider
    {
        string SourceType { get; }

        ICollectionsOptions CollectionsOptions { get; set; }

        Envelope GetBbox(string collectionId, string apiKey = null);

        OgcFeatureCollection GetFeatures(string collectionId, int limit = 10, int offset = 0, Envelope bbox = null, DateTime? startDateTime = null, DateTime? endDateTime = null, string apiKey = null);

        OgcFeature GetFeature(string collectionId, string featureId, string apiKey = null);

        long GetFeaturesCount(string collectionId, Envelope bbox = null, DateTime? startDateTime = null, DateTime? endDateTime = null, string apiKey = null);

        string CreateFeature(string collectionId, IFeature feature, string apiKey = null);

        void UpdateFeature(string collectionId, string featureId, IFeature feature, string apiKey = null);

        void ReplaceFeature(string collectionId, string featureId, IFeature feature, string apiKey = null);

        void DeleteFeature(string collectionId, string featureId, string apiKey = null);

        IFeaturesSourceOptions DeserializeCollectionSourceOptions(string json, JsonSerializerOptions options);

        void SerializeStorageOptions(Utf8JsonWriter writer, IFeaturesSourceOptions storage);
    }
}


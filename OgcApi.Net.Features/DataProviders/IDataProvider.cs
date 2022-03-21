using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OgcApi.Net.Features.Features;
using System;
using OgcApi.Net.Features.Options.Interfaces;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;

namespace OgcApi.Net.Features.DataProviders
{
    public interface IDataProvider
    {
        string SourceType { get; }

        void SetCollectionOptions(ICollectionsOptions options);

        ICollectionsOptions GetCollectionSourcesOptions();

        Envelope GetBbox(string collectionId, string apiKey = null);

        OgcFeatureCollection GetFeatures(string collectionId, int limit = 10, int offset = 0, Envelope bbox = null, DateTime? startDateTime = null, DateTime? endDateTime = null, string apiKey = null);

        OgcFeature GetFeature(string collectionId, string featureId, string apiKey = null);

        long GetFeaturesCount(string collectionId, Envelope bbox = null, DateTime? startDateTime = null, DateTime? endDateTime = null, string apiKey = null);

        string CreateFeature(string collectionId, IFeature feature, string apiKey = null);

        void UpdateFeature(string collectionId, string featureId, IFeature feature, string apiKey = null);

        void ReplaceFeature(string collectionId, string featureId, IFeature feature, string apiKey = null);

        void DeleteFeature(string collectionId, string featureId, string apiKey = null);
        ICollectionSourceOptions DeserializeCollectionSourceOptions(string json, JsonSerializerOptions options);
    }
}


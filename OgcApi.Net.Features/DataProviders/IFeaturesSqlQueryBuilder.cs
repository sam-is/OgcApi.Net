using System;
using System.Data;
using NetTopologySuite.Geometries;
using OgcApi.Net.Features.Features;

namespace OgcApi.Net.Features.DataProviders
{
    public interface IFeaturesSqlQueryBuilder
    {
        IFeaturesSqlQueryBuilder AddSelect();
        IFeaturesSqlQueryBuilder AddSelectBbox();
        IFeaturesSqlQueryBuilder AddInsert(OgcFeature feature);
        IFeaturesSqlQueryBuilder AddUpdate(OgcFeature feature);
        IFeaturesSqlQueryBuilder AddReplace(OgcFeature feature);
        IFeaturesSqlQueryBuilder AddDelete();
        IFeaturesSqlQueryBuilder AddCount();
        IFeaturesSqlQueryBuilder AddLimit(int offset, int limit);
        IFeaturesSqlQueryBuilder AddFrom();
        IFeaturesSqlQueryBuilder AddWhere(Envelope bbox);
        IFeaturesSqlQueryBuilder AddWhere(DateTime? startDateTime, DateTime? endDateTime);
        IFeaturesSqlQueryBuilder AddWhere(string featureId);
        IFeaturesSqlQueryBuilder AddApiKeyWhere(string apiKeyPredicate, string apiKey);
        IFeaturesSqlQueryBuilder ComposeWhereClause();
        IDbCommand BuildCommand(IDbConnection connection);
    }
}

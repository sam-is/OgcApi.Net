using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Data;

namespace OgcApi.Net.DataProviders;

public interface IFeaturesSqlQueryBuilder
{
    IFeaturesSqlQueryBuilder AddSelect();
    IFeaturesSqlQueryBuilder AddSelectBbox();
    IFeaturesSqlQueryBuilder AddInsert(IFeature feature);
    IFeaturesSqlQueryBuilder AddUpdate(IFeature feature);
    IFeaturesSqlQueryBuilder AddReplace(IFeature feature);
    IFeaturesSqlQueryBuilder AddDelete();
    IFeaturesSqlQueryBuilder AddCount();
    IFeaturesSqlQueryBuilder AddLimit(int offset, int limit);
    IFeaturesSqlQueryBuilder AddFrom();
    IFeaturesSqlQueryBuilder AddWhere(Envelope bbox);
    IFeaturesSqlQueryBuilder AddWhere(DateTime? startDateTime, DateTime? endDateTime);
    IFeaturesSqlQueryBuilder AddWhere(string featureId);
    IFeaturesSqlQueryBuilder AddWhere(Dictionary<string,string> propertyFilter);
    IFeaturesSqlQueryBuilder AddApiKeyWhere(string apiKeyPredicate, string apiKey);
    IFeaturesSqlQueryBuilder ComposeWhereClause();
    IDbCommand BuildCommand(IDbConnection connection);
}
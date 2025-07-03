using Microsoft.Data.Sqlite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.Features;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OgcApi.Net.SpatiaLite;

public class FeaturesSqlQueryBuilder(SqlFeaturesSourceOptions collectionOptions) : IFeaturesSqlQueryBuilder
{
    private string _query = "";

    private readonly List<SqliteParameter> _sqlParameters = [];

    private readonly SqlFeaturesSourceOptions _collectionOptions = collectionOptions ?? throw new ArgumentNullException(nameof(collectionOptions));

    private readonly List<string> _predicateConditions = [];

    public IFeaturesSqlQueryBuilder AddSelectBbox()
    {
        _query += $"SELECT ST_Envelope(ST_Collect([{_collectionOptions.GeometryColumn}])) " +
                  $"FROM [{_collectionOptions.Schema}].[{_collectionOptions.Table}]";

        return this;
    }

    public IFeaturesSqlQueryBuilder AddSelect()
    {
        _query += $"SELECT [{_collectionOptions.IdentifierColumn}], [{_collectionOptions.GeometryColumn}]";
        if (_collectionOptions.Properties != null)
            _query += ", " + string.Join(", ", _collectionOptions.Properties.Select(property => $"[{property}]"));
        _query += " ";
        return this;
    }

    public IFeaturesSqlQueryBuilder AddInsert(IFeature feature)
    {
        _query += $"INSERT INTO [{_collectionOptions.Schema}].[{_collectionOptions.Table}] ([{_collectionOptions.GeometryColumn}]";
        if (_collectionOptions.Properties != null)
            _query += ", " + string.Join(", ", _collectionOptions.Properties.Select(property => $"[{property}]"));
        _query += $") VALUES (@p0";
        if (_collectionOptions.Properties != null)
            for (var i = 0; i < _collectionOptions.Properties.Count; i++)
            {
                _query += $", @p{i + 1}";
                _sqlParameters.Add(new SqliteParameter($"@p{i + 1}",
                    feature.Attributes?.GetOptionalValue(_collectionOptions.Properties[i]) ?? DBNull.Value));
            }
        _query += ")";
        _query += "; SELECT last_insert_rowid()";

        if (feature.Geometry != null)
        {
            // new GaiaGeoWriter(); // ??
            var geometryWriter = new GaiaGeoWriter();
            var geometryBytes = geometryWriter.Write(feature.Geometry);
            _sqlParameters.Add(new SqliteParameter("@p0", geometryBytes)
            {
                SqliteType = SqliteType.Blob,
                DbType = DbType.Binary,
            });
        }
        else
        {
            _sqlParameters.Add(new SqliteParameter("@p0", null));
        }

        return this;
    }

    public IFeaturesSqlQueryBuilder AddReplace(IFeature feature)
    {
        _query +=
            $"UPDATE [{_collectionOptions.Schema}].[{_collectionOptions.Table}] " +
            $"SET [{_collectionOptions.GeometryColumn}] = @p0";
        if (_collectionOptions.Properties != null)
        {
            for (var i = 0; i < _collectionOptions.Properties.Count; i++)
            {
                _query += $", [{_collectionOptions.Properties[i]}] = @p{i + 1}";
                _sqlParameters.Add(new SqliteParameter($"@p{i + 1}",
                    feature.Attributes?.GetOptionalValue(_collectionOptions.Properties[i]) ?? DBNull.Value));
            }
        }

        var geometryWriter = new GaiaGeoWriter();
        var geometryBytes = geometryWriter.Write(feature.Geometry);
        _sqlParameters.Add(new SqliteParameter("@p0", geometryBytes)
        {
            SqliteType = SqliteType.Blob,
            DbType = DbType.Binary,
        });

        _query += " ";

        return this;
    }

    public IFeaturesSqlQueryBuilder AddUpdate(IFeature feature)
    {
        _query +=
            $"UPDATE [{_collectionOptions.Schema}].[{_collectionOptions.Table}] " +
            "SET ";

        if (feature.Geometry != null)
        {
            _query += $"[{_collectionOptions.GeometryColumn}] = @p0 ";

            var geometryWriter = new GaiaGeoWriter();
            var geometryBytes = geometryWriter.Write(feature.Geometry);
            _sqlParameters.Add(new SqliteParameter("@p0", geometryBytes)
            {
                SqliteType = SqliteType.Blob,
                DbType = DbType.Binary,
            });

            if (feature.Attributes != null)
            {
                _query += ",";
            }
        }

        if (feature.Attributes != null)
        {
            var attributesNames = feature.Attributes.GetNames();
            for (var i = 0; i < attributesNames.Length; i++)
            {
                if (!_collectionOptions.Properties.Contains(attributesNames[i])) continue;
                _query += $" [{attributesNames[i]}] = @p{i + 1}";
                if (i != attributesNames.Length - 1)
                    _query += ",";
                _sqlParameters.Add(new SqliteParameter($"@p{i + 1}", feature.Attributes.GetOptionalValue(attributesNames[i]) ?? DBNull.Value));
            }
        }

        _query += " ";

        return this;
    }

    public IFeaturesSqlQueryBuilder AddDelete()
    {
        _query += $"DELETE FROM [{_collectionOptions.Schema}].[{_collectionOptions.Table}] ";
        return this;
    }

    public IFeaturesSqlQueryBuilder AddCount()
    {
        _query += "SELECT COUNT(*)";
        return this;
    }

    public IFeaturesSqlQueryBuilder AddLimit(int offset, int limit)
    {
        _query +=
            $" ORDER BY [{_collectionOptions.IdentifierColumn}] " +
            $"LIMIT {limit} " +
            $"OFFSET {offset} ";
        return this;
    }

    public IFeaturesSqlQueryBuilder AddFrom()
    {
        _query += $"FROM [{_collectionOptions.Schema}].[{_collectionOptions.Table}] ";
        return this;
    }

    public IFeaturesSqlQueryBuilder AddWhere(Envelope bbox)
    {
        if (bbox == null) return this;

        _predicateConditions.Add($"ST_Intersects([{_collectionOptions.GeometryColumn}], ST_GeomFromText(@Bbox, @GeometrySRID)) = 1");
        _sqlParameters.Add(new SqliteParameter("@Bbox", FormattableString.Invariant($"POLYGON(({bbox.MinX} {bbox.MinY}, {bbox.MinX} {bbox.MaxY}, {bbox.MaxX} {bbox.MaxY}, {bbox.MaxX} {bbox.MinY}, {bbox.MinX} {bbox.MinY}))")));
        _sqlParameters.Add(new SqliteParameter("@GeometrySRID", _collectionOptions.GeometrySrid));
        return this;
    }

    public IFeaturesSqlQueryBuilder AddWhere(DateTime? startDateTime, DateTime? endDateTime)
    {
        if (!string.IsNullOrWhiteSpace(_collectionOptions.DateTimeColumn))
        {
            if (startDateTime != null)
            {
                _predicateConditions.Add($"[{_collectionOptions.DateTimeColumn}] >= DATE(@StartDateTime)");
                _sqlParameters.Add(new SqliteParameter("@StartDateTime", startDateTime.Value));
            }
            if (endDateTime != null)
            {
                _predicateConditions.Add($"[{_collectionOptions.DateTimeColumn}] <= DATE(@EndDateTime)");
                _sqlParameters.Add(new SqliteParameter("@EndDateTime", endDateTime.Value));
            }
        }
        return this;
    }

    public IFeaturesSqlQueryBuilder AddWhere(string featureIdColumn)
    {
        _predicateConditions.Add($"[{_collectionOptions.IdentifierColumn}] = @FeatureId");
        _sqlParameters.Add(new SqliteParameter("@FeatureId", featureIdColumn));
        return this;
    }

    public IFeaturesSqlQueryBuilder AddWhere(Dictionary<string, string> propertyFilter)
    {
        if (propertyFilter != null)
            foreach (var pair in propertyFilter)
            {
                _predicateConditions.Add($"""CAST([{pair.Key}] AS TEXT) = @{pair.Key}Value""");
                _sqlParameters.Add(new SqliteParameter($"@{pair.Key}Value", pair.Value));
            }
        return this;
    }

    public IFeaturesSqlQueryBuilder AddApiKeyWhere(string apiKeyPredicate, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKeyPredicate) || string.IsNullOrWhiteSpace(apiKey)) return this;
        _predicateConditions.Add(apiKeyPredicate);
        _sqlParameters.Add(new SqliteParameter("@ApiKey", apiKey));
        return this;
    }

    public IFeaturesSqlQueryBuilder ComposeWhereClause()
    {
        if (_predicateConditions.Count > 0)
        {
            _query += "WHERE " + string.Join(" AND ", _predicateConditions);
        }
        return this;
    }

    public IDbCommand BuildCommand(IDbConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = _query;

        foreach (var parameter in _sqlParameters)
        {
            command.Parameters.Add(parameter);
        }

        return command;
    }
}
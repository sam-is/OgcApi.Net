using Microsoft.Data.SqlClient;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.Features;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;

namespace OgcApi.Net.SqlServer;

public class FeaturesSqlQueryBuilder(SqlFeaturesSourceOptions collectionOptions) : IFeaturesSqlQueryBuilder
{
    private string _query = "";

    private readonly List<SqlParameter> _sqlParameters = [];

    private readonly SqlFeaturesSourceOptions _collectionOptions = collectionOptions ?? throw new ArgumentNullException(nameof(collectionOptions));

    private readonly List<string> _predicateConditions = [];

    public IFeaturesSqlQueryBuilder AddSelectBbox()
    {
        _query += $"SELECT {_collectionOptions.GeometryDataType}::EnvelopeAggregate([{_collectionOptions.GeometryColumn}]) " +
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
        _query += $") OUTPUT Inserted.[{_collectionOptions.IdentifierColumn}] VALUES (@p0";
        if (_collectionOptions.Properties != null)
            for (var i = 0; i < _collectionOptions.Properties.Count; i++)
            {
                _query += $", @p{i + 1}";
                _sqlParameters.Add(new SqlParameter($"@p{i + 1}",
                    feature.Attributes?.GetOptionalValue(_collectionOptions.Properties[i]) ?? DBNull.Value));
            }
        _query += ")";

        if (feature.Geometry != null)
        {
            var geometryWriter = new SqlServerBytesWriter { IsGeography = _collectionOptions.GeometryDataType == "geography" };
            var geometryBytes = geometryWriter.Write(feature.Geometry);
            _sqlParameters.Add(new SqlParameter("@p0", new SqlBytes(geometryBytes))
            {
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = _collectionOptions.GeometryDataType
            });
        }
        else
        {
            _sqlParameters.Add(new SqlParameter("@p0", null));
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
                _sqlParameters.Add(new SqlParameter($"@p{i + 1}",
                    feature.Attributes?.GetOptionalValue(_collectionOptions.Properties[i]) ?? DBNull.Value));
            }
        }

        var geometryWriter = new SqlServerBytesWriter { IsGeography = _collectionOptions.GeometryDataType == "geography" };
        var geometryBytes = geometryWriter.Write(feature.Geometry);
        _sqlParameters.Add(new SqlParameter("@p0", new SqlBytes(geometryBytes))
        {
            SqlDbType = SqlDbType.Udt,
            UdtTypeName = _collectionOptions.GeometryDataType
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
            var geometryWriter = new SqlServerBytesWriter { IsGeography = _collectionOptions.GeometryDataType == "geography" };
            var geometryBytes = geometryWriter.Write(feature.Geometry);
            _sqlParameters.Add(new SqlParameter("@p0", new SqlBytes(geometryBytes))
            {
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = _collectionOptions.GeometryDataType
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
                _sqlParameters.Add(new SqlParameter($"@p{i + 1}", feature.Attributes.GetOptionalValue(attributesNames[i]) ?? DBNull.Value));
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
            $"OFFSET {offset} ROWS " +
            $"FETCH NEXT {limit} ROWS ONLY ";
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

        _predicateConditions.Add($"[{_collectionOptions.GeometryColumn}].STIntersects({_collectionOptions.GeometryDataType}::STGeomFromText(@Bbox, @GeometrySRID)) = 1");
        _sqlParameters.Add(new SqlParameter("@Bbox", FormattableString.Invariant($"POLYGON(({bbox.MinX} {bbox.MinY}, {bbox.MinX} {bbox.MaxY}, {bbox.MaxX} {bbox.MaxY}, {bbox.MaxX} {bbox.MinY}, {bbox.MinX} {bbox.MinY}))")));
        _sqlParameters.Add(new SqlParameter("@GeometrySRID", _collectionOptions.GeometrySrid));
        return this;
    }

    public IFeaturesSqlQueryBuilder AddWhere(DateTime? startDateTime, DateTime? endDateTime)
    {
        if (!string.IsNullOrWhiteSpace(_collectionOptions.DateTimeColumn))
        {
            if (startDateTime != null)
            {
                _predicateConditions.Add($"[{_collectionOptions.DateTimeColumn}] >= @StartDateTime");
                _sqlParameters.Add(new SqlParameter("@StartDateTime", startDateTime.Value));
            }
            if (endDateTime != null)
            {
                _predicateConditions.Add($"[{_collectionOptions.DateTimeColumn}] <= @EndDateTime");
                _sqlParameters.Add(new SqlParameter("@EndDateTime", endDateTime.Value));
            }
        }
        return this;
    }

    public IFeaturesSqlQueryBuilder AddWhere(string featureIdColumn)
    {
        _predicateConditions.Add($"[{_collectionOptions.IdentifierColumn}] = @FeatureId");
        _sqlParameters.Add(new SqlParameter("@FeatureId", featureIdColumn));
        return this;
    }

    public IFeaturesSqlQueryBuilder AddWhere(Dictionary<string, string> propertyFilter)
    {
        if (propertyFilter != null)
            foreach (var pair in propertyFilter)
            {
                _predicateConditions.Add($"""CAST([{pair.Key}] AS varchar) = @{pair.Key}Value""");
                _sqlParameters.Add(new SqlParameter($"@{pair.Key}Value", pair.Value));
            }
        return this;
    }

    public IFeaturesSqlQueryBuilder AddApiKeyWhere(string apiKeyPredicate, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKeyPredicate) || string.IsNullOrWhiteSpace(apiKey)) return this;
        _predicateConditions.Add(apiKeyPredicate);
        _sqlParameters.Add(new SqlParameter("@ApiKey", apiKey));
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
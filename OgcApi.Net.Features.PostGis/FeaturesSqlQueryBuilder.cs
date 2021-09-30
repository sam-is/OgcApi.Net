using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql;
using NpgsqlTypes;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Features;
using OgcApi.Net.Features.Options;
using System;
using System.Collections.Generic;
using System.Data;

namespace OgcApi.Net.Features.PostGis
{
    public class FeaturesSqlQueryBuilder : IFeaturesSqlQueryBuilder
    {
        private string _query = "";

        private readonly List<NpgsqlParameter> _sqlParameters = new();

        private readonly SqlCollectionSourceOptions _collectionOptions;

        private readonly List<string> _predicateConditions = new();

        public FeaturesSqlQueryBuilder(SqlCollectionSourceOptions collectionOptions)
        {
            _collectionOptions = collectionOptions ?? throw new ArgumentNullException(nameof(collectionOptions));
        }

        public IFeaturesSqlQueryBuilder AddSelectBbox()
        {
            _query += $"SELECT ST_Envelope(ST_Union(ST_Envelope({_collectionOptions.GeometryColumn}))) FROM \"{_collectionOptions.Schema}\".\"{_collectionOptions.Table}\"";
            return this;
        }

        public IFeaturesSqlQueryBuilder AddSelect()
        {
            _query += $"SELECT {_collectionOptions.IdentifierColumn}, {_collectionOptions.GeometryColumn}";
            if (_collectionOptions.Properties != null)
                _query += ", " + string.Join(", ", _collectionOptions.Properties);
            _query += " ";
            return this;
        }

        public IFeaturesSqlQueryBuilder AddInsert(OgcFeature feature)
        {
            _query += $"INSERT INTO \"{_collectionOptions.Schema}\".\"{_collectionOptions.Table}\" ({_collectionOptions.GeometryColumn}";
            if (_collectionOptions.Properties != null)
                _query += ", " + string.Join(", ", _collectionOptions.Properties);
            _query += ") VALUES (@p0";
            if (_collectionOptions.Properties != null)
                for (var i = 0; i < _collectionOptions.Properties.Count; i++)
                {
                    _query += $", @p{i + 1}";
                    _sqlParameters.Add(new NpgsqlParameter($"@p{i + 1}", feature.Attributes.GetOptionalValue(_collectionOptions.Properties[i])));
                }
            _query += $") RETURNING {_collectionOptions.IdentifierColumn}";

            var geometryBytes = new PostGisWriter().Write(feature.Geometry);
            _sqlParameters.Add(new NpgsqlParameter("@p0", geometryBytes)
            {
                NpgsqlDbType = NpgsqlDbType.Bytea
            });

            return this;
        }

        public IFeaturesSqlQueryBuilder AddReplace(OgcFeature feature)
        {
            _query += 
                $"UPDATE \"{_collectionOptions.Schema}\".\"{_collectionOptions.Table}\" " +
                $"SET {_collectionOptions.GeometryColumn} = @p0";
            if (_collectionOptions.Properties == null) return this;
            for (var i = 0; i < _collectionOptions.Properties.Count; i++)
            {
                _query += $", {_collectionOptions.Properties[i]} = @p{i + 1}";
                _sqlParameters.Add(new NpgsqlParameter($"@p{i + 1}",
                    feature.Attributes.GetOptionalValue(_collectionOptions.Properties[i]) ?? DBNull.Value));
            }

            var geometryBytes = new PostGisWriter().Write(feature.Geometry);
            _sqlParameters.Add(new NpgsqlParameter("@p0", geometryBytes)
            {
                NpgsqlDbType = NpgsqlDbType.Bytea
            });

            _query += " ";

            return this;
        }

        public IFeaturesSqlQueryBuilder AddUpdate(OgcFeature feature)
        {
            _query += 
                $"UPDATE \"{_collectionOptions.Schema}\".\"{_collectionOptions.Table}\" " +
                "SET ";

            if (feature.Geometry != null)
            {
                _query += $"{_collectionOptions.GeometryColumn} = @p0 ";
                var geometryBytes = new PostGisWriter().Write(feature.Geometry);
                _sqlParameters.Add(new NpgsqlParameter("@p0", geometryBytes)
                {
                    NpgsqlDbType = NpgsqlDbType.Bytea
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
                    _query += $" {attributesNames[i]} = @p{i + 1}";
                    if (i != attributesNames.Length - 1)
                        _query += ",";
                    _sqlParameters.Add(new NpgsqlParameter($"@p{i + 1}", feature.Attributes.GetOptionalValue(attributesNames[i])));
                }
            }
            
            _query += " ";
            
            return this;
        }

        public IFeaturesSqlQueryBuilder AddDelete()
        {
            _query += $"DELETE FROM \"{_collectionOptions.Schema}\".\"{_collectionOptions.Table}\" ";
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
                $" ORDER BY {_collectionOptions.IdentifierColumn} " +
                $"LIMIT {limit}" +
                $"OFFSET {offset}";
            return this;
        }

        public IFeaturesSqlQueryBuilder AddFrom()
        {
            _query += $"FROM \"{_collectionOptions.Schema}\".\"{_collectionOptions.Table}\" ";
            return this;
        }

        public IFeaturesSqlQueryBuilder AddWhere(Envelope bbox)
        {
            if (bbox == null) return this;
            _predicateConditions.Add($"ST_Intersects({_collectionOptions.GeometryColumn}, ST_GeomFromText(@Bbox, @GeometrySRID))");
            _sqlParameters.Add(new NpgsqlParameter("@Bbox", FormattableString.Invariant($"POLYGON(({bbox.MinX} {bbox.MinY}, {bbox.MinX} {bbox.MaxY}, {bbox.MaxX} {bbox.MaxY}, {bbox.MaxX} {bbox.MinY}, {bbox.MinX} {bbox.MinY}))")));
            _sqlParameters.Add(new NpgsqlParameter("@GeometrySRID", _collectionOptions.GeometrySrid));
            return this;
        }

        public IFeaturesSqlQueryBuilder AddWhere(DateTime? startDateTime, DateTime? endDateTime)
        {
            if (string.IsNullOrWhiteSpace(_collectionOptions.DateTimeColumn)) return this;
            if (startDateTime != null)
            {
                _predicateConditions.Add($"{_collectionOptions.DateTimeColumn} >= @StartDateTime");
                _sqlParameters.Add(new NpgsqlParameter("@StartDateTime", startDateTime.Value));
            }
            if (endDateTime != null)
            {
                _predicateConditions.Add($"{_collectionOptions.DateTimeColumn} <= @EndDateTime");
                _sqlParameters.Add(new NpgsqlParameter("@EndDateTime", endDateTime.Value));
            }
            return this;
        }

        public IFeaturesSqlQueryBuilder AddWhere(string featureId)
        {
            _predicateConditions.Add($"CAST({_collectionOptions.IdentifierColumn} AS text) = @FeatureId");
            _sqlParameters.Add(new NpgsqlParameter("@FeatureId", featureId));
            return this;
        }

        public IFeaturesSqlQueryBuilder AddApiKeyWhere(string apiKeyPredicate, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKeyPredicate) || string.IsNullOrWhiteSpace(apiKey)) return this;
            _predicateConditions.Add(apiKeyPredicate);
            _sqlParameters.Add(new NpgsqlParameter("@ApiKey", apiKey));
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
}
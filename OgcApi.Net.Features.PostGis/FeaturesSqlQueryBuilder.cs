using NetTopologySuite.Geometries;
using Npgsql;
using OgcApi.Net.Features.PostGis.Options;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Features.PostGis
{
    public class FeaturesSqlQueryBuilder
    {
        private string _query = "";

        private readonly List<NpgsqlParameter> _sqlParameters = new();

        private readonly PostGisCollectionSourceOptions _collectionOptions;

        private readonly List<string> _predicateConditions = new();

        public FeaturesSqlQueryBuilder(PostGisCollectionSourceOptions collectionOptions)
        {
            _collectionOptions = collectionOptions ?? throw new ArgumentNullException(nameof(collectionOptions));
        }

        public FeaturesSqlQueryBuilder AddSelect()
        {
            _query += $"SELECT {_collectionOptions.IdentifierColumn}, {_collectionOptions.GeometryColumn}";
            if (_collectionOptions.Properties != null)
                _query += ", " + string.Join(", ", _collectionOptions.Properties);
            _query += " ";
            return this;
        }

        public FeaturesSqlQueryBuilder AddCount()
        {
            _query += "SELECT COUNT(*)";
            return this;
        }

        public FeaturesSqlQueryBuilder AddLimit(int offset, int limit)
        {
            _query +=
                $" ORDER BY {_collectionOptions.IdentifierColumn} " +
                $"LIMIT {limit}" +
                $"OFFSET {offset}";
            return this;
        }

        public FeaturesSqlQueryBuilder AddFrom()
        {
            _query += $"FROM \"{_collectionOptions.Schema}\".\"{_collectionOptions.Table}\" ";
            return this;
        }

        public FeaturesSqlQueryBuilder AddWhere(Envelope bbox)
        {
            if (bbox != null)
            {
                _predicateConditions.Add($"ST_Intersects({_collectionOptions.GeometryColumn}, ST_GeomFromText(@Bbox, @GeometrySRID))");
                _sqlParameters.Add(new NpgsqlParameter("@Bbox", FormattableString.Invariant($"POLYGON(({bbox.MinX} {bbox.MinY}, {bbox.MinX} {bbox.MaxY}, {bbox.MaxX} {bbox.MaxY}, {bbox.MaxX} {bbox.MinY}, {bbox.MinX} {bbox.MinY}))")));
                _sqlParameters.Add(new NpgsqlParameter("@GeometrySRID", _collectionOptions.GeometrySrid));
            }
            return this;
        }

        public FeaturesSqlQueryBuilder AddWhere(DateTime? startDateTime, DateTime? endDateTime)
        {
            if (!string.IsNullOrWhiteSpace(_collectionOptions.DateTimeColumn))
            {
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
            }
            return this;
        }

        public FeaturesSqlQueryBuilder AddWhere(string featureIdColumn)
        {
            _predicateConditions.Add($"{_collectionOptions.IdentifierColumn} = @FeatureId");
            _sqlParameters.Add(new NpgsqlParameter("@FeatureId", featureIdColumn));
            return this;
        }

        public FeaturesSqlQueryBuilder AddApiKeyWhere(string apiKeyPredicate, string apiKey)
        {
            if (!string.IsNullOrWhiteSpace(apiKeyPredicate) &&
                !string.IsNullOrWhiteSpace(apiKey))
            {
                _predicateConditions.Add(apiKeyPredicate);
                _sqlParameters.Add(new NpgsqlParameter("@ApiKey", apiKey));
            }
            return this;
        }

        public FeaturesSqlQueryBuilder ComposeWhereClause()
        {
            if (_predicateConditions.Count > 0)
            {
                _query += "WHERE " + string.Join(" AND ", _predicateConditions);
            }
            return this;
        }

        public NpgsqlCommand BuildCommand(NpgsqlConnection connection)
        {
            var sqlCommand = new NpgsqlCommand(_query, connection);
            sqlCommand.Parameters.AddRange(_sqlParameters.ToArray());

            return sqlCommand;
        }
    }
}
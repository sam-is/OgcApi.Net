using Microsoft.Data.SqlClient;
using NetTopologySuite.Geometries;
using OgcApi.Net.Features.SqlServer.Options;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Features.SqlServer
{
    public class FeaturesSqlQueryBuilder
    {
        private string _query = "";

        private readonly List<SqlParameter> _sqlParameters = new();

        private readonly SqlServerCollectionSourceOptions _collectionOptions;

        private readonly List<string> _predicateConditions = new();

        public FeaturesSqlQueryBuilder(SqlServerCollectionSourceOptions collectionOptions)
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
                $"OFFSET {offset} ROWS " +
                $"FETCH NEXT {limit} ROWS ONLY ";
            return this;
        }

        public FeaturesSqlQueryBuilder AddFrom()
        {
            _query += $"FROM [{_collectionOptions.Schema}].[{_collectionOptions.Table}] ";
            return this;
        }

        public FeaturesSqlQueryBuilder AddWhere(Envelope bbox)
        {
            if (bbox != null)
            {
                _predicateConditions.Add($"{_collectionOptions.GeometryColumn}.STIntersects({_collectionOptions.GeometryDataType}::STGeomFromText(@Bbox, @GeometrySRID)) = 1");
                _sqlParameters.Add(new SqlParameter("@Bbox", FormattableString.Invariant($"POLYGON(({bbox.MinX} {bbox.MinY}, {bbox.MinX} {bbox.MaxY}, {bbox.MaxX} {bbox.MaxY}, {bbox.MaxX} {bbox.MinY}, {bbox.MinX} {bbox.MinY}))")));
                _sqlParameters.Add(new SqlParameter("@GeometrySRID", _collectionOptions.GeometrySrid));
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
                    _sqlParameters.Add(new SqlParameter("@StartDateTime", startDateTime.Value));
                }
                if (endDateTime != null)
                {
                    _predicateConditions.Add($"{_collectionOptions.DateTimeColumn} <= @EndDateTime");
                    _sqlParameters.Add(new SqlParameter("@EndDateTime", endDateTime.Value));
                }
            }
            return this;
        }

        public FeaturesSqlQueryBuilder AddWhere(string featureIdColumn)
        {
            _predicateConditions.Add($"{_collectionOptions.IdentifierColumn} = @FeatureId");
            _sqlParameters.Add(new SqlParameter("@FeatureId", featureIdColumn));
            return this;
        }

        public FeaturesSqlQueryBuilder AddApiKeyWhere(string apiKeyPredicate, string apiKey)
        {
            if (!string.IsNullOrWhiteSpace(apiKeyPredicate) &&
                !string.IsNullOrWhiteSpace(apiKey))
            {
                _predicateConditions.Add(apiKeyPredicate);
                _sqlParameters.Add(new SqlParameter("@ApiKey", apiKey));
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

        public SqlCommand BuildCommand(SqlConnection connection)
        {
            var sqlCommand = new SqlCommand(_query, connection);
            sqlCommand.Parameters.AddRange(_sqlParameters.ToArray());

            return sqlCommand;
        }
    }
}
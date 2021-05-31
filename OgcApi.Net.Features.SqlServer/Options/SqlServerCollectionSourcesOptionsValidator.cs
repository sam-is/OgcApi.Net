using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Features.SqlServer.Options
{
    public static class SqlServerCollectionSourcesOptionsValidator
    {
        public static void Validate(SqlServerCollectionSourcesOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.Sources != null)
            {
                var failureMessages = new List<string>();

                foreach (SqlServerCollectionSourceOptions source in options.Sources)
                {
                    if (string.IsNullOrWhiteSpace(source.Id))
                        failureMessages.Add("Parameter Id is required for the Sql Server collection source option");

                    if (string.IsNullOrWhiteSpace(source.ConnectionString))
                        failureMessages.Add("Parameter ConnectionString is required for the Sql Server collection source option");

                    if (string.IsNullOrWhiteSpace(source.Schema))
                        failureMessages.Add("Parameter Schema is required for the Sql Server collection source option");

                    if (string.IsNullOrWhiteSpace(source.Table))
                        failureMessages.Add("Parameter Table is required for the Sql Server collection source option");

                    if (string.IsNullOrWhiteSpace(source.GeometryColumn))
                        failureMessages.Add("Parameter GeometryColumn is required for the Sql Server collection source option");

                    if (string.IsNullOrWhiteSpace(source.IdentifierColumn))
                        failureMessages.Add("Parameter IdentifierColumn is required for the Sql Server collection source option");

                    if (source.GeometryDataType != "geometry" && source.GeometryDataType != "geography")
                        failureMessages.Add("Parameter DataType must be 'geometry' or 'geography'");
                }

                if (failureMessages.Count > 0)
                    throw new OptionsValidationException(
                        "SqlServerCollectionSourcesOptions",
                        typeof(SqlServerCollectionSourcesOptions),
                        failureMessages);
            }
            else
            {
                throw new OptionsValidationException(
                    "SqlServerCollectionSourcesOptions",
                    typeof(SqlServerCollectionSourcesOptions),
                    new List<string>() { "Source list should not be empty" });
            }
        }
    }
}

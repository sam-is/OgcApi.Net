using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Features.PostGis.Options
{
    public static class PostGisCollectionSourcesOptionsValidator
    {
        public static void Validate(PostGisCollectionSourcesOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.Sources != null)
            {
                var failureMessages = new List<string>();

                foreach (PostGisCollectionSourceOptions source in options.Sources)
                {
                    if (string.IsNullOrWhiteSpace(source.Id))
                        failureMessages.Add("Parameter Id is required for the PostGis collection source option");

                    if (string.IsNullOrWhiteSpace(source.ConnectionString))
                        failureMessages.Add("Parameter ConnectionString is required for the PostGis collection source option");

                    if (string.IsNullOrWhiteSpace(source.Schema))
                        failureMessages.Add("Parameter Schema is required for the PostGis collection source option");

                    if (string.IsNullOrWhiteSpace(source.Table))
                        failureMessages.Add("Parameter Table is required for the PostGis collection source option");

                    if (string.IsNullOrWhiteSpace(source.GeometryColumn))
                        failureMessages.Add("Parameter GeometryColumn is required for the PostGis collection source option");

                    if (string.IsNullOrWhiteSpace(source.IdentifierColumn))
                        failureMessages.Add("Parameter IdentifierColumn is required for the PostGis collection source option");                    
                }

                if (failureMessages.Count > 0)
                    throw new OptionsValidationException(
                        "PostGisCollectionSourcesOptions",
                        typeof(PostGisCollectionSourcesOptions),
                        failureMessages);
            }
            else
            {
                throw new OptionsValidationException(
                    "PostGisCollectionSourcesOptions",
                    typeof(PostGisCollectionSourcesOptions),
                    new List<string>() { "Source list should not be empty" });
            }
        }
    }
}

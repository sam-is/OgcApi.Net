using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using OgcApi.Net.Features.Options.SqlOptions;

namespace OgcApi.Net.Features.Options
{
    public static class CollectionsOptionsValidator
    {
        public static void Validate(CollectionsOptions options)
        {
            var failureMessages = new List<string>();

            if (options == null || options.Items == null || options.Items.Count == 0)
                failureMessages.Add("Collections should not be empty");
         
            foreach (var item in options.Items)
            {
                if (string.IsNullOrWhiteSpace(item.Id))
                    failureMessages.Add("Parameter Id is required for the collection");

                if (item.Features != null)
                {
                    if ((item.Features as CollectionOptionsFeatures).Crs == null || (item.Features as CollectionOptionsFeatures).Crs.Count == 0)
                        failureMessages.Add("Features Crs list should not be empty for the collection");

                    if ((item.Features as CollectionOptionsFeatures).StorageCrs == null)
                        failureMessages.Add("Parameter StorageCrs is required for the collection feature option");

                    if (item.Features.Storage != null)
                    {
                        if (string.IsNullOrWhiteSpace((item.Features.Storage as SqlCollectionSourceOptions).ConnectionString))
                            failureMessages.Add("Parameter ConnectionString is required for the collection feature storage option");

                        if (string.IsNullOrWhiteSpace((item.Features.Storage as SqlCollectionSourceOptions).Schema))
                            failureMessages.Add("Parameter Schema is required for the collection feature storage option");

                        if (string.IsNullOrWhiteSpace((item.Features.Storage as SqlCollectionSourceOptions).Table))
                            failureMessages.Add("Parameter Table is required for the collection feature storage option");

                        if (string.IsNullOrWhiteSpace((item.Features.Storage as SqlCollectionSourceOptions).GeometryColumn))
                            failureMessages.Add("Parameter GeometryColumn is required for the feature storage collection option");

                        if (string.IsNullOrWhiteSpace((item.Features.Storage as SqlCollectionSourceOptions).IdentifierColumn))
                            failureMessages.Add("Parameter IdentifierColumn is required for the feature storage collection option");

                        if ((item.Features.Storage as SqlCollectionSourceOptions).Type != "SqlServer" 
                            && (item.Features.Storage as SqlCollectionSourceOptions).Type != "PostGis" 
                            && (item.Features.Storage as SqlCollectionSourceOptions).Type != "SpatiaLite")
                            failureMessages.Add("Parameter Type is must be 'SqlServer', 'PostGis' or 'SpatiaLite'");

                        if ((item.Features.Storage as SqlCollectionSourceOptions).GeometryDataType != "geometry" && (item.Features.Storage as SqlCollectionSourceOptions).GeometryDataType != "geography")
                            failureMessages.Add("Parameter DataType must be 'geometry' or 'geography'");
                    }
                    else
                        failureMessages.Add("Features Storage should not be empty");
                }
                else
                    failureMessages.Add("Features should not be empty");
            }

            if (failureMessages.Count > 0)
                throw new OptionsValidationException(
                    "OgcApiOptions",
                    typeof(CollectionsOptions),
                    failureMessages);
        }
    }
}

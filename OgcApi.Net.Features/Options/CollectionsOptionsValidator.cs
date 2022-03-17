using System.Collections.Generic;
using Microsoft.Extensions.Options;
using OgcApi.Net.Features.Options.SqlOptions;

namespace OgcApi.Net.Features.Options
{
    public static class CollectionsOptionsValidator
    {
        public static void Validate(CollectionsOptions options)
        {
            

            if (options?.Items == null || options.Items.Count == 0)
                throw new OptionsValidationException("OgcApiOptions", typeof(CollectionsOptions), new List<string>() { "Collections should not be empty" });

            var failureMessages = new List<string>();
            foreach (var item in options.Items)
            {
                if (string.IsNullOrWhiteSpace(item.Id))
                    failureMessages.Add("Parameter Id is required for the collection");

                if (item.Features != null)
                {
                    if (item.Features.Crs == null || item.Features .Crs.Count == 0)
                        failureMessages.Add("Features Crs list should not be empty for the collection");

                    if (item.Features.StorageCrs == null)
                        failureMessages.Add("Parameter StorageCrs is required for the collection feature option");

                    var storage = item.Features.Storage as SqlCollectionSourceOptions;
                    if (storage != null)
                    {                     
                        if (string.IsNullOrWhiteSpace(storage.ConnectionString))
                            failureMessages.Add("Parameter ConnectionString is required for the collection feature storage option");

                        if (string.IsNullOrWhiteSpace(storage.Schema))
                            failureMessages.Add("Parameter Schema is required for the collection feature storage option");

                        if (string.IsNullOrWhiteSpace(storage.Table))
                            failureMessages.Add("Parameter Table is required for the collection feature storage option");

                        if (string.IsNullOrWhiteSpace(storage.GeometryColumn))
                            failureMessages.Add("Parameter GeometryColumn is required for the feature storage collection option");

                        if (string.IsNullOrWhiteSpace(storage.IdentifierColumn))
                            failureMessages.Add("Parameter IdentifierColumn is required for the feature storage collection option");

                        if (storage.Type != "SqlServer" 
                            && storage.Type != "PostGis" 
                            && storage.Type != "SpatiaLite")
                            failureMessages.Add("Parameter Type is must be 'SqlServer', 'PostGis' or 'SpatiaLite'");

                        if (storage.GeometryDataType != "geometry" && storage.GeometryDataType != "geography")
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

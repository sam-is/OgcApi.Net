using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace OgcApi.Net.Options;

public static class CollectionsOptionsValidator
{
    public static void Validate(CollectionsOptions options)
    {
        if (options?.Items == null || options.Items.Count == 0)
            throw new OptionsValidationException("OgcApiOptions", typeof(CollectionsOptions), new List<string> { "Collections should not be empty" });

        var failureMessages = new List<string>();
        foreach (var item in options.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                failureMessages.Add("Parameter Id is required for the collection");

            if (item.Features != null)
            {
                if (item.Features.Crs == null || item.Features.Crs.Count == 0)
                    failureMessages.Add("Features Crs list should not be empty for the collection");

                if (item.Features.StorageCrs == null)
                    failureMessages.Add("Parameter StorageCrs is required for the collection feature option");

                failureMessages.AddRange(item.Features.Storage.Validate());
            }

            if (item.Tiles != null)
            {
                if (item.Tiles.Crs == null)
                    failureMessages.Add("Crs parameter is required for the collection tiles option");

                failureMessages.AddRange(item.Tiles.Storage.Validate());
            }

            if (item.Features == null && item.Tiles == null)
                failureMessages.Add("Features and|or tiles are required for the collection");
        }

        if (failureMessages.Count > 0)
            throw new OptionsValidationException(
                "OgcApiOptions",
                typeof(CollectionsOptions),
                failureMessages);
    }
}
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Options
{
    static class OgcApiOptionsValidator
    {
        public static void Validate(OgcApiOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            LandingPageOptions landingPageOptions = options.LandingPage;
            if (landingPageOptions == null ||
                (landingPageOptions.ApiDescriptionPage == null && landingPageOptions.ApiDocumentPage == null))
            {
                throw new OptionsValidationException(
                    "OgcApiOptions",
                    typeof(OgcApiOptions),
                    new List<string>() { "Landing page options must include ApiDescriptionPage or ApiDocumentPage parameter" });
            }

            CollectionsOptions collectionsOptions = options.Collections;
            if (options.Collections == null ||
                options.Collections.Items == null ||
                options.Collections.Items.Count == 0)
            {
                throw new OptionsValidationException(
                    "OgcApiOptions",
                    typeof(OgcApiOptions),
                    new List<string>() { "Collections should not be empty" });
            }

            foreach (CollectionOptions collectionOptions in collectionsOptions.Items)
            {
                if (string.IsNullOrWhiteSpace(collectionOptions.Id))
                {
                    throw new OptionsValidationException(
                        "OgcApiOptions",
                        typeof(OgcApiOptions),
                        new List<string>() { "Parameter Id is required for the collection" });
                }

                if (string.IsNullOrWhiteSpace(collectionOptions.SourceType))
                {
                    throw new OptionsValidationException(
                        "OgcApiOptions",
                        typeof(OgcApiOptions),
                        new List<string>() { "Parameter SourceType is required for the collection" });
                }
            }
        }
    }
}

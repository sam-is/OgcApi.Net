using Microsoft.Extensions.Options;
using OgcApi.Net.Features.Options;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Features.Options
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
            CollectionsOptionsValidator.Validate(collectionsOptions);
        }
                    
    }
}

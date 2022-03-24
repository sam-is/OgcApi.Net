using Microsoft.Extensions.Options;
using OgcApi.Net.Features.Options;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Options
{
    static class OgcApiOptionsValidator
    {
        public static void Validate(OgcApiOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var landingPageOptions = options.LandingPage;
            if (landingPageOptions == null ||
                (landingPageOptions.ApiDescriptionPage == null && landingPageOptions.ApiDocumentPage == null))
            {
                throw new OptionsValidationException(
                    "OgcApiOptions",
                    typeof(OgcApiOptions),
                    new List<string> { "Landing page options must include ApiDescriptionPage or ApiDocumentPage parameter" });
            }

            CollectionsOptionsValidator.Validate(options.Collections);
        }
    }
}

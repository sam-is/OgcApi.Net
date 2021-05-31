using OgcApi.Net.Features.Resources;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Features.Options
{
    public class LandingPageOptions
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Uri ApiDocumentPage { get; set; }

        public Uri ApiDescriptionPage { get; set; }

        public List<Link> Links { get; set; }
    }
}

using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Options;

public class LandingPageOptions
{
    public string Title { get; set; }

    public string Version { get; set; }

    public string ContactName { get; set; }

    public Uri ContactUrl { get; set; }

    public string LicenseName { get; set; }

    public Uri LicenseUrl { get; set; }

    public string Description { get; set; }

    public Uri ApiDocumentPage { get; set; }

    public Uri ApiDescriptionPage { get; set; }

    public List<Link> Links { get; set; }
}
using NetTopologySuite.Features;
using OgcApi.Net.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OgcApi.Net.Features;

public class OgcFeatureCollection() : Collection<IFeature>(new List<IFeature>())
{
    public List<Link> Links { get; set; }

    public long TotalMatched { get; set; }
}
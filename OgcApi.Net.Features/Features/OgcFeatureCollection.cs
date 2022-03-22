using NetTopologySuite.Features;
using OgcApi.Net.Features.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OgcApi.Net.Features.Features
{
    public class OgcFeatureCollection : Collection<IFeature>
    {
        public OgcFeatureCollection()
            : base(new List<IFeature>())
        {
        }

        public List<Link> Links { get; set; }

        public long TotalMatched { get; set; }
    }
}

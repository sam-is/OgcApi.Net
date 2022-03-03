using NetTopologySuite.Features;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace OgcApi.Net.Features
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

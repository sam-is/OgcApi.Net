using NetTopologySuite.Features;
using OgcApi.Net.Features.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace OgcApi.Net.Features.Features
{
    public class OgcFeatureCollection : Collection<IFeature>, ISerializable
    {
        public OgcFeatureCollection()
            : base(new List<IFeature>())
        {
        }

        public List<Link> Links { get; set; }
        
        public int TotalMatched { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("links", Links);
            info.AddValue("timeStamp", DateTime.Now);
            info.AddValue("numberMatched", Items.Count);
            info.AddValue("features", Items);            
        }
    }
}

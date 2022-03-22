using System;

namespace OgcApi.Net.Features.Resources
{
    public class SpatialExtent
    {
        public double[][] Bbox { get; set; }

        public Uri Crs { get; set; }
    }
}

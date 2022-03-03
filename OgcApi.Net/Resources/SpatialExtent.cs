using System;
using System.Collections.Generic;

namespace OgcApi.Net.Resources
{
    public class SpatialExtent
    {
        public double[][] Bbox { get; set; }

        public Uri Crs { get; set; }
    }
}

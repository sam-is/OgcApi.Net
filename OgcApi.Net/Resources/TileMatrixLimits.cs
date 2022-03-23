using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgcApi.Net.Resources
{
    public class TileMatrixLimits
    {
        public int TileMatrix { get; set; }
        public int MinTileRow { get; set; }
        public int MaxTileRow { get; set; }
        public int MinTileCol { get; set; }
        public int MaxTileCol { get; set; }
    }
}

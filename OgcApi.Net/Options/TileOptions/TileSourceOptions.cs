using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgcApi.Net.Options
{
    public class TileSourceOptions
    {
        public string Id { get; set; }

        public Uri TileMatrixSet { get; set; }

        public Uri Crs { get; set; }
    }
}

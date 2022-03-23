using OgcApi.Net.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgcApi.Net.MbTiles
{
    public class MbTilesSourceOptions : TileSourceOptions
    {
        public string ConnectionString { set; get; }
    }
}

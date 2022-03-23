using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgcApi.Net.Options
{
    public class TileSourcesOptions
    {
        public List<TileSourceOptions> Sources { get; set; }

        public TileSourceOptions GetSourceById(string id)
        {
            return Sources.Find(x => x.Id == id);
        }
    }
}

using System.Collections.Generic;

namespace OgcApi.Net.Options.TileOptions
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

using OgcApi.Net.Features.Options.SqlOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgcApi.Net.Features.Options
{
    public interface ICollectionOptions
    {
        public string Id { get; set; }
        public CollectionOptionsFeatures Features { get; set; }
    }
}

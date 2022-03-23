using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgcApi.Net.Options.TileOptions
{
    class TileSourcesOptionsValidator
    {
        public static void Validate(TileSourcesOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.Sources != null)
            {
                var failureMessages = new List<string>();

                foreach (TileSourceOptions source in options.Sources)
                {
                    if (string.IsNullOrWhiteSpace(source.Id))
                        failureMessages.Add("Parameter Id is required for the tile source option");

                    if (source.TileMatrixSet == null)
                        failureMessages.Add("Parameter TileMatrixSet is required for the tile source option");

                    if (source.Crs == null)
                        failureMessages.Add("Parameter Crs is required for the tile source option");
                }

                if (failureMessages.Count > 0)
                    throw new OptionsValidationException(
                        "TileSourcesOptions",
                        typeof(TileSourcesOptions),
                        failureMessages);
            }
            else
            {
                throw new OptionsValidationException(
                    "TileSourcesOptions",
                    typeof(TileSourcesOptions),
                    new List<string>() { "Source list should not be empty" });
            }
        }
    }
}

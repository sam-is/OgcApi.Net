using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OgcApi.Net.Options;
using OgcApi.Net.Options.TileOptions;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OgcApi.Net.DataProviders
{
    public abstract class TilesDataProvider : ITilesProvider
    {
        protected readonly TileSourcesOptions TilesOptions;

        protected readonly ILogger Logger;
        public abstract string SourceType { get; }

        protected TilesDataProvider(IOptionsMonitor<TileSourcesOptions> tileSourcesOptions, ILogger logger)
        {
            if (tileSourcesOptions == null)
                throw new ArgumentNullException(nameof(tileSourcesOptions));

            Logger = logger;

            try
            {
                TilesOptions = tileSourcesOptions.CurrentValue;
                TileSourcesOptionsValidator.Validate(TilesOptions);
            }
            catch (OptionsValidationException ex)
            {
                foreach (var failure in ex.Failures) Logger.LogError(failure);
                throw;
            }
        }
        public abstract List<TileMatrixLimits> GetLimits(string collectionId);

        public abstract Task<byte[]> GetTileAsync(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey = null);

        public TileSourcesOptions GetTileSourcesOptions()
        {
            return TilesOptions;
        }
    }
}

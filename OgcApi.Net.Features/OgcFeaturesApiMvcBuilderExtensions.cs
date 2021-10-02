using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.Features;
using System;
using NetTopologySuite.IO.Converters;

namespace OgcApi.Net.Features
{
    public static class OgcFeaturesApiMvcBuilderExtensions
    {
        public static IMvcBuilder AddOgсFeaturesControllers(
            this IMvcBuilder mvcBuilder)
        {
            if (mvcBuilder == null) throw new ArgumentNullException(nameof(mvcBuilder));

            return mvcBuilder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new OgcGeoJsonConverterFactory());
                options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
            }).AddApplicationPart(typeof(OgcFeaturesApiMvcBuilderExtensions).Assembly);
        }
    }
}
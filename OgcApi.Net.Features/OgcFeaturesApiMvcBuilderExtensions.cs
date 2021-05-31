using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.Features;
using System;

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
            }).AddApplicationPart(typeof(OgcFeaturesApiMvcBuilderExtensions).Assembly);
        }
    }
}
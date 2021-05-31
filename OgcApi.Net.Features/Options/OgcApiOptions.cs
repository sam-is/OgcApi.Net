using System.ComponentModel.DataAnnotations;

namespace OgcApi.Net.Features.Options
{
    public class OgcApiOptions
    {
        [Required]
        public LandingPageOptions LandingPage { get; set; }

        [Required]
        public ConformanceOptions Conformance { get; set; }

        [Required]
        public CollectionsOptions Collections { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Features.Resources
{
    /// <summary>
    /// The conformance classes from standards or community specifications
    /// </summary>
    public class Conformance
    {
        public List<Uri> ConformsTo { get; set; }
    }
}

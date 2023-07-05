using System.Collections.Generic;

namespace OgcApi.Net.Resources;

/// <summary>
/// The Landing page provides links to:
/// <list type="bullet">
///     <item>
///         <description>the API definition</description>
///     </item>
///     <item>
///         <description>the Conformance declaration (path /conformance)</description>
///     </item>
///     <item>
///         <description>the Collections (path /collections)</description>
///     </item>    
/// </list>
/// </summary>    
public class LandingPage
{
    public string Title { get; set; }

    public string Description { get; set; }

    public List<Link> Links { get; set; }
}
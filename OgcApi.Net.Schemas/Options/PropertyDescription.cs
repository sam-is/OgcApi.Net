namespace OgcApi.Net.Schemas.Options;

/// <summary>
/// Information about collection property description
/// </summary>
public class PropertyDescription
{
    /// <summary>
    /// Type of the property, except for spatial.<br/>
    /// The standard recommends using one of the following:<br/>
    /// <c>string</c> (string or temporal properties), <c>number</c>, <c>integer</c>, <c>boolean</c>,
    /// <c>object</c> or <c>array</c> (consist of items that are strings or numbers).<br/>
    /// Spatial (geometry) property shall specify <see cref="Format"/>.
    /// </summary>
    public string? Type { get; set; }
    /// <summary>
    /// Human-readable title for the property
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// Description for the understanding of the property
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// <c>x-ogc-role</c>, custom OGC role (<c>id</c>, <c>primary-geometry</c>, <c>type</c>, etc.)
    /// </summary>
    public string? XOgcRole { get; set; }
    /// <summary>
    /// Format of the <c>string</c> or <c>spatial</c> property.<br/>
    /// For string property the Standard recommends using one of the following:<br/>
    /// <c>uri</c>, <c>uri-template</c>, <c>uuid</c>, <c>date-time</c>, <c>date</c>, <c>time</c>, etc.<br/>
    /// For <c>spatial</c> (<c>geometry</c>) property is one of:<br/>
    /// <c>geometry-point</c>, <c>geometry-multipoint</c>, <c>geometry-linestring</c>,
    /// <c>geometry-multilinestring</c>, <c>geometry-polygon</c>, <c>geometry-multipolygon</c>, 
    /// <c>geometry-geometrycollection</c>, <c>geometry-any</c>
    /// </summary>
    public string? Format { get; set; }
    /// <summary>
    /// Specific relative position of the property.
    /// </summary>
    public int? XOgcPropertySeq { get; set; }
}
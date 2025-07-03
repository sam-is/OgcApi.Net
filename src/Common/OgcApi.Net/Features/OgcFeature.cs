using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OgcApi.Net.Resources;
using System.Collections.Generic;

namespace OgcApi.Net.Features;

public class OgcFeature : IFeature, IUnique
{
    private readonly Feature _implementation = new();

    public object Id { get; set; }

    public Geometry Geometry
    {
        get => _implementation.Geometry;
        set => _implementation.Geometry = value;
    }

    public Envelope BoundingBox
    {
        get => _implementation.BoundingBox;
        set => _implementation.BoundingBox = value;
    }

    public IAttributesTable Attributes
    {
        get => _implementation.Attributes;
        set => _implementation.Attributes = value;
    }

    public List<Link> Links { get; set; }
}
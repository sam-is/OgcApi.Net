using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems.Transformations;

namespace OgcApi.Net.Crs;

internal class CoordinateSequenceFilter(MathTransform mathTransform) : ICoordinateSequenceFilter
{
    public bool Done => false;

    public bool GeometryChanged => true;

    public void Filter(CoordinateSequence seq, int i)
    {
        var x = seq.GetX(i);
        var y = seq.GetY(i);                        
        mathTransform.Transform(ref x, ref y);
        seq.SetX(i, x);
        seq.SetY(i, y);            
    }
}
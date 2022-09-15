using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems.Transformations;

namespace OgcApi.Net.Crs
{
    class CoordinateSequenceFilter : ICoordinateSequenceFilter
    {
        private readonly MathTransform _mathTransform;

        public CoordinateSequenceFilter(MathTransform mathTransform) => _mathTransform = mathTransform;

        public bool Done => false;

        public bool GeometryChanged => true;

        public void Filter(CoordinateSequence seq, int i)
        {
            double x = seq.GetX(i);
            double y = seq.GetY(i);                        
            _mathTransform.Transform(ref x, ref y);
            seq.SetX(i, x);
            seq.SetY(i, y);            
        }
    }
}

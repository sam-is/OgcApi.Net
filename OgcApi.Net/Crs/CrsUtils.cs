using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OgcApi.Net.Features;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OgcApi.Net.Crs
{
    public static class CrsUtils
    {
        public static readonly Uri DefaultCrs = new("http://www.opengis.net/def/crs/OGC/1.3/CRS84");

        public const string SridFileName = "SRID.csv";

        public static string GetWktBySrid(string srid)
        {
            if (string.IsNullOrWhiteSpace(srid))
            {
                throw new ArgumentException($"'{nameof(srid)}' cannot be null or whitespace.", nameof(srid));
            }

            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var sridFile = File.Exists(Path.Combine(assemblyPath, SridFileName)) ? Path.Combine(assemblyPath, SridFileName) : Path.Combine(assemblyPath, "Crs", SridFileName);

            var sridInFile = srid + ";";
            return File.ReadAllLines(sridFile).SkipWhile(line => !line.StartsWith(sridInFile)).First().Split(';').Last();
        }

        public static CoordinateSystem GetCoordinateSystemBySrid(string srid)
        {
            if (string.IsNullOrWhiteSpace(srid))
            {
                throw new ArgumentException($"'{nameof(srid)}' cannot be null or whitespace.", nameof(srid));
            }

            if (srid == "4326" || srid == "CRS84")
                return GeographicCoordinateSystem.WGS84;

            if (srid == "3857")
                return ProjectedCoordinateSystem.WebMercator;

            var csFactory = new CoordinateSystemFactory();
            return csFactory.CreateFromWkt(GetWktBySrid(srid));
        }

        public static CoordinateSystem GetCoordinateSystemByUri(Uri uri)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return GetCoordinateSystemBySrid(uri.Segments.LastOrDefault());
        }

        public static void Transform(this IFeature feature, CoordinateSystem srcCrs, CoordinateSystem dstCrs)
        {
            if (feature is null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (srcCrs is null)
            {
                throw new ArgumentNullException(nameof(srcCrs));
            }

            if (dstCrs is null)
            {
                throw new ArgumentNullException(nameof(dstCrs));
            }

            if (srcCrs == dstCrs)
                return;

            if (feature.Geometry != null)
                feature.Geometry.Transform(srcCrs, dstCrs);
        }

        public static void Transform(this IFeature feature, Uri srcCrsUri, Uri dstCrsUri)
        {
            if (feature is null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (srcCrsUri is null)
            {
                throw new ArgumentNullException(nameof(srcCrsUri));
            }

            if (dstCrsUri is null)
            {
                throw new ArgumentNullException(nameof(dstCrsUri));
            }

            if (srcCrsUri == dstCrsUri)
                return;

            if (feature.Geometry != null)
                feature.Geometry.Transform(
                    GetCoordinateSystemBySrid(srcCrsUri.Segments.Last()),
                    GetCoordinateSystemBySrid(dstCrsUri.Segments.Last()));
        }

        public static void Transform(this Geometry geometry, CoordinateSystem srcCrs, CoordinateSystem dstCrs)
        {
            if (geometry is null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            if (srcCrs is null)
            {
                throw new ArgumentNullException(nameof(srcCrs));
            }

            if (dstCrs is null)
            {
                throw new ArgumentNullException(nameof(dstCrs));
            }

            if (srcCrs == dstCrs)
                return;

            var transformationFactory = new CoordinateTransformationFactory();
            ICoordinateTransformation transformation = transformationFactory.CreateFromCoordinateSystems(srcCrs, dstCrs);
            var filter = new CoordinateSequenceFilter(transformation.MathTransform);

            geometry.Apply(filter);
        }

        public static void Transform(this Geometry geometry, Uri srcCrsUri, Uri dstCrsUri)
        {
            if (geometry is null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            if (srcCrsUri is null)
            {
                throw new ArgumentNullException(nameof(srcCrsUri));
            }

            if (dstCrsUri is null)
            {
                throw new ArgumentNullException(nameof(dstCrsUri));
            }

            if (srcCrsUri == dstCrsUri)
                return;

            geometry.Transform(
                GetCoordinateSystemBySrid(srcCrsUri.Segments.Last()),
                GetCoordinateSystemBySrid(dstCrsUri.Segments.Last()));
        }

        public static void Transform(this Envelope envelope, CoordinateSystem srcCrs, CoordinateSystem dstCrs)
        {
            if (envelope is null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (srcCrs is null)
            {
                throw new ArgumentNullException(nameof(srcCrs));
            }

            if (dstCrs is null)
            {
                throw new ArgumentNullException(nameof(dstCrs));
            }

            if (srcCrs == dstCrs)
                return;

            var transformationFactory = new CoordinateTransformationFactory();
            var transformation = transformationFactory.CreateFromCoordinateSystems(srcCrs, dstCrs);

            var x1 = envelope.MinX;
            var y1 = envelope.MinY;
            var x2 = envelope.MaxX;
            var y2 = envelope.MaxY;

            var mathTransform = transformation.MathTransform;
            mathTransform.Transform(ref x1, ref y1);
            mathTransform.Transform(ref x2, ref y2);

            envelope.Init(x1, x2, y1, y2);
        }

        public static void Transform(this Envelope envelope, Uri srcCrsUri, Uri dstCrsUri)
        {
            if (envelope is null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (srcCrsUri is null)
            {
                throw new ArgumentNullException(nameof(srcCrsUri));
            }

            if (dstCrsUri is null)
            {
                throw new ArgumentNullException(nameof(dstCrsUri));
            }

            if (srcCrsUri == dstCrsUri)
                return;

            envelope.Transform(
                GetCoordinateSystemBySrid(srcCrsUri.Segments.Last()),
                GetCoordinateSystemBySrid(dstCrsUri.Segments.Last()));
        }

        public static void Transform(this OgcFeatureCollection featureCollection, Uri srcCrsUri, Uri dstCrsUri)
        {
            if (featureCollection is null)
            {
                throw new ArgumentNullException(nameof(featureCollection));
            }

            if (srcCrsUri is null)
            {
                throw new ArgumentNullException(nameof(srcCrsUri));
            }

            if (dstCrsUri is null)
            {
                throw new ArgumentNullException(nameof(dstCrsUri));
            }

            if (srcCrsUri == dstCrsUri)
                return;

            var srcCrs = GetCoordinateSystemBySrid(srcCrsUri.Segments.Last());
            var dstCrs = GetCoordinateSystemBySrid(dstCrsUri.Segments.Last());

            foreach (IFeature feature in featureCollection)
            {
                feature.Transform(srcCrs, dstCrs);
            }
        }
    }
}

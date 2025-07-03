using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OgcApi.Net.Features;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OgcApi.Net.Crs;

public static class CrsUtils
{
    public static readonly Uri DefaultCrs = new("http://www.opengis.net/def/crs/OGC/1.3/CRS84");

    public const string SridFileName = "SRID.csv";

    private const double Eps = 0.00000001;

    public static string GetWktBySrid(string srid)
    {
        if (string.IsNullOrWhiteSpace(srid))
        {
            throw new ArgumentException($"'{nameof(srid)}' cannot be null or whitespace.", nameof(srid));
        }

        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        var sridFile = File.Exists(Path.Combine(assemblyPath, SridFileName)) ? 
            Path.Combine(assemblyPath, SridFileName) : 
            Path.Combine(assemblyPath, "Crs", SridFileName);

        var sridInFile = srid + ";";
        return File.ReadAllLines(sridFile).SkipWhile(line => !line.StartsWith(sridInFile)).First().Split(';').Last();
    }

    public static CoordinateSystem GetCoordinateSystemBySrid(string srid)
    {
        if (string.IsNullOrWhiteSpace(srid))
        {
            throw new ArgumentException($"'{nameof(srid)}' cannot be null or whitespace.", nameof(srid));
        }

        switch (srid)
        {
            case "4326" or "CRS84":
                return GeographicCoordinateSystem.WGS84;
            case "3857":
                return ProjectedCoordinateSystem.WebMercator;
            default:
            {
                var csFactory = new CoordinateSystemFactory();
                return csFactory.CreateFromWkt(GetWktBySrid(srid));
            }
        }
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
        ArgumentNullException.ThrowIfNull(feature);
        ArgumentNullException.ThrowIfNull(srcCrs);
        ArgumentNullException.ThrowIfNull(dstCrs);

        if (srcCrs == dstCrs)
            return;

        feature.Geometry?.Transform(srcCrs, dstCrs);
    }

    public static void Transform(this IFeature feature, Uri srcCrsUri, Uri dstCrsUri)
    {
        ArgumentNullException.ThrowIfNull(feature);
        ArgumentNullException.ThrowIfNull(srcCrsUri);
        ArgumentNullException.ThrowIfNull(dstCrsUri);

        if (srcCrsUri == dstCrsUri)
            return;

        feature.Geometry?.Transform(
                GetCoordinateSystemBySrid(srcCrsUri.Segments.Last()),
                GetCoordinateSystemBySrid(dstCrsUri.Segments.Last()));
    }

    public static void Transform(this Geometry geometry, CoordinateSystem srcCrs, CoordinateSystem dstCrs)
    {
        ArgumentNullException.ThrowIfNull(geometry);
        ArgumentNullException.ThrowIfNull(srcCrs);
        ArgumentNullException.ThrowIfNull(dstCrs);

        if (srcCrs == dstCrs)
            return;

        var transformationFactory = new CoordinateTransformationFactory();
        var transformation = transformationFactory.CreateFromCoordinateSystems(srcCrs, dstCrs);
        var filter = new CoordinateSequenceFilter(transformation.MathTransform);

        geometry.Apply(filter);
    }

    public static void Transform(this Geometry geometry, Uri srcCrsUri, Uri dstCrsUri)
    {
        ArgumentNullException.ThrowIfNull(geometry);
        ArgumentNullException.ThrowIfNull(srcCrsUri);
        ArgumentNullException.ThrowIfNull(dstCrsUri);

        if (srcCrsUri == dstCrsUri)
            return;

        geometry.Transform(
            GetCoordinateSystemBySrid(srcCrsUri.Segments.Last()),
            GetCoordinateSystemBySrid(dstCrsUri.Segments.Last()));
    }

    public static void Transform(this Envelope envelope, CoordinateSystem srcCrs, CoordinateSystem dstCrs, bool isSrcWgs84 = false)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(srcCrs);
        ArgumentNullException.ThrowIfNull(dstCrs);

        if (srcCrs == dstCrs)
            return;

        var transformationFactory = new CoordinateTransformationFactory();
        var transformation = transformationFactory.CreateFromCoordinateSystems(srcCrs, dstCrs);

        var x1 = envelope.MinX;
        var y1 = envelope.MinY;
        var x2 = envelope.MaxX;
        var y2 = envelope.MaxY;

        if (isSrcWgs84)
        {
            if (Math.Abs(-90 - y1) < Eps)
            {
                y1 += Eps;
            }

            if (Math.Abs(90 - y2) < Eps)
            {
                y2 -= Eps;
            }
        }

        var mathTransform = transformation.MathTransform;
        mathTransform.Transform(ref x1, ref y1);
        mathTransform.Transform(ref x2, ref y2);

        envelope.Init(x1, x2, y1, y2);
    }

    public static void Transform(this Envelope envelope, Uri srcCrsUri, Uri dstCrsUri)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(srcCrsUri);
        ArgumentNullException.ThrowIfNull(dstCrsUri);

        if (srcCrsUri == dstCrsUri)
            return;

        envelope.Transform(
            GetCoordinateSystemBySrid(srcCrsUri.Segments.Last()),
            GetCoordinateSystemBySrid(dstCrsUri.Segments.Last()),
            srcCrsUri == DefaultCrs);
    }

    public static void Transform(this OgcFeatureCollection featureCollection, Uri srcCrsUri, Uri dstCrsUri)
    {
        ArgumentNullException.ThrowIfNull(featureCollection);
        ArgumentNullException.ThrowIfNull(srcCrsUri);
        ArgumentNullException.ThrowIfNull(dstCrsUri);

        if (srcCrsUri == dstCrsUri)
            return;

        var srcCrs = GetCoordinateSystemBySrid(srcCrsUri.Segments.Last());
        var dstCrs = GetCoordinateSystemBySrid(dstCrsUri.Segments.Last());

        foreach (var feature in featureCollection)
        {
            feature.Transform(srcCrs, dstCrs);
        }
    }
}
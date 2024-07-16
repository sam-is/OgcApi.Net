using NetTopologySuite.Features;
using System.Collections.Generic;

namespace OgcApi.Net.Options.Tiles;

/// <summary>
/// Returns true when access to tile is granted according to the given parameters, otherwise returns false
/// </summary>
/// <param name="collectionId"></param>
/// <param name="tileMatrix"></param>
/// <param name="tileRow"></param>
/// <param name="tileCol"></param>
/// <param name="apiKey"></param>
/// <returns></returns>
public delegate bool TileAccessDelegate(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey);

/// <summary>
/// Returns true when access to feature is granted according to the given parameters, otherwise returns false
/// </summary>
/// <param name="collectionId"></param>
/// <param name="feature"></param>
/// <param name="apiKey"></param>
/// <returns></returns>
public delegate bool FeatureAccessDelegate(string collectionId, IFeature feature, string apiKey);
public interface ITilesSourceOptions
{
    TileAccessDelegate TileAccessDelegate { get; set; }

    FeatureAccessDelegate FeatureAccessDelegate { get; set; }

    string Type { get; set; }

    int? MinZoom { get; set; }

    int? MaxZoom { get; set; }

    List<string> Validate();
}
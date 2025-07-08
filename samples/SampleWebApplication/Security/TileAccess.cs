using NetTopologySuite.Features;

namespace SampleWebApplication.Security;

public static class TileAccess
{
    public static bool TilesAccessDelegate(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey) => (collectionId ?? "") switch
    {
        "PolygonsWithApiKey" when (apiKey ?? "").Equals("qwerty") && tileMatrix is >= 0 and <= 7 =>
            tileMatrix switch
            {
                0 => tileRow == 0 && tileCol == 0,
                1 => tileRow == 0 && tileCol == 1,
                2 => tileRow == 1 && tileCol == 2,
                3 => tileRow == 2 && tileCol == 5,
                4 => tileRow == 5 && tileCol == 10,
                5 => tileRow == 10 && tileCol == 20,
                6 => tileRow == 20 && tileCol == 40,
                7 => tileRow == 40 && tileCol is 81 or 82,
                _ => false
            },
        _ => true,
    };


    public static bool FeatureAccessDelegate(string collectionId, IFeature feature, string apiKey) => (collectionId ?? "") switch
    {
        "FeatureAccessData" => apiKey == "admin" ||
            apiKey == "value" && feature.Attributes.Exists("value") &&
            (feature.Attributes["value"] is long and > 1200 ||
            feature.Attributes["value"] is > 100.0) ||
            feature.Attributes.Exists("roleAccess") && feature.Attributes["roleAccess"].ToString() == apiKey,
        _ => true,
    };
}

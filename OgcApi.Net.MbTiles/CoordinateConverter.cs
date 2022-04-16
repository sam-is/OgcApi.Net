using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgcApi.Net.MbTiles
{
    public static class CoordinateConverter
    {
        private static double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        public static int LongToTileX(double lon, int z)
        {
            return (int)(Math.Floor((lon + 180.0) / 360.0 * (1 << z)));
        }

        public static int LatToTileY(double lat, int z)
        {
            return (int)Math.Floor((1 - Math.Log(Math.Tan(DegToRad(lat)) + 1 / Math.Cos(DegToRad(lat))) / Math.PI) / 2 * (1 << z));
        }

        public static double TileXToLong(int x, int z)
        {
            return x / (double)(1 << z) * 360.0 - 180;
        }

        public static double TileYToLat(int y, int z)
        {
            double n = Math.PI - 2.0 * Math.PI * y / (double)(1 << z);
            return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
        }
    }
}

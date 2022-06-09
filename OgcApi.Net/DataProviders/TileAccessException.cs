using System;

namespace OgcApi.Net.DataProviders
{
    public class TileAccessException : Exception
    {
        public TileAccessException()
        {
        }

        public TileAccessException(string message)
            : base(message)
        {
        }

        public TileAccessException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

using OgcApi.Features.PostGis.Tests.Utils;

namespace OgcApi.Features.Postgis.Tests
{
    public class DatabaseFixture
    {
        public DatabaseFixture()
        {
            DatabaseUtils.RecreateDatabase();
        }
    }
}

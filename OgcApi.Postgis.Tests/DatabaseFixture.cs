using OgcApi.Features.PostGis.Tests.Utils;

namespace OgcApi.Features.PostGis.Tests
{
    public class DatabaseFixture
    {
        public DatabaseFixture()
        {
            DatabaseUtils.RecreateDatabase();
        }
    }
}

using OgcApi.Features.SqlServer.Tests.Utils;

namespace OgcApi.Features.SqlServer.Tests
{
    public class DatabaseFixture
    {
        public DatabaseFixture()
        {
            DatabaseUtils.RecreateDatabase();
        }
    }
}

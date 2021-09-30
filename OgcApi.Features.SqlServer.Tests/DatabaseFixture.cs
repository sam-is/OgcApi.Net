using OgcApi.Features.SqlServer.Tests.Utils;
using Xunit;

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

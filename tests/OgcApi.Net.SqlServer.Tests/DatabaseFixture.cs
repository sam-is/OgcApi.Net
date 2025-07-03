using OgcApi.SqlServer.Tests.Utils;

namespace OgcApi.SqlServer.Tests;

public class DatabaseFixture
{
    public DatabaseFixture()
    {
        DatabaseUtils.RecreateDatabase();
    }
}
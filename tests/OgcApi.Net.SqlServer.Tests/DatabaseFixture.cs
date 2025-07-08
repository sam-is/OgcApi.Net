using OgcApi.Net.SqlServer.Tests.Utils;

namespace OgcApi.Net.SqlServer.Tests;

public class DatabaseFixture
{
    public DatabaseFixture()
    {
        DatabaseUtils.RecreateDatabase();
    }
}
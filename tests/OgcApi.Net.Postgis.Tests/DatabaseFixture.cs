using OgcApi.Net.PostGis.Tests.Utils;

namespace OgcApi.Net.PostGis.Tests;

public class DatabaseFixture
{
    public DatabaseFixture()
    {
        DatabaseUtils.RecreateDatabase();
    }
}
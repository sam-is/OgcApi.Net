using OgcApi.PostGis.Tests.Utils;

namespace OgcApi.PostGis.Tests;

public class DatabaseFixture
{
    public DatabaseFixture()
    {
        DatabaseUtils.RecreateDatabase();
    }
}
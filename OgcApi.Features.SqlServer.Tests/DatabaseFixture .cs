using OgcApi.Features.SqlServer.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Reflection;

namespace OgcApi.Features.SqlServer.Tests.Utils
{
    public static class DatabaseUtils
    {
        public const string DatabaseName = "OgcApiTests";

        private const string DbConnectionString = @"Server=localhost,1433; Database={0}; Trusted_Connection=True;";

        private const string MasterConnectionString = @"Server=localhost,1433; Database=master; Trusted_Connection=True;";        

        public static void RecreateDatabase()
        {            
            using var sqlConnection = new SqlConnection(MasterConnectionString);
            sqlConnection.Open();

            using var createDatabaseCommand = new SqlCommand(string.Format(GetInstallSqlScript("DatabaseCreate"), DatabaseName), sqlConnection);
            createDatabaseCommand.ExecuteNonQuery();            

            using var installDatabaseCommand = new SqlCommand(string.Format(GetInstallSqlScript("DatabaseInstall"), DatabaseName), sqlConnection);
            installDatabaseCommand.ExecuteNonQuery();
        }

        private static string GetInstallSqlScript(string scriptName)
        {
            Assembly assembly = typeof(DatabaseUtils).Assembly;
            using var stream = assembly.GetManifestResourceStream($"OgcApi.Features.SqlServer.Tests.Utils.{scriptName}.sql");

            if (stream == null)
            {
                throw new InvalidOperationException($"Database script is not found in the assembly `{assembly}`.");
            }

            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();            
        }

        public static string GetConnectionString()
        {
            return string.Format(DbConnectionString, DatabaseName);
        }
    }
}

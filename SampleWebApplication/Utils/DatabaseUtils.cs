using Microsoft.Data.SqlClient;
using System;
using System.IO;

namespace SampleWebApplication.Utils;

public static class DatabaseUtils
{
    private const string DatabaseName = "OgcApiSamples";

    private const string DbConnectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog={0}";

    public static void RecreateDatabase()
    {
        using var sqlConnection = new SqlConnection(string.Format(DbConnectionString, "master"));
        sqlConnection.Open();

        using var createDatabaseCommand =
            new SqlCommand(string.Format(GetInstallSqlScript("DatabaseCreate"), DatabaseName), sqlConnection);
        createDatabaseCommand.ExecuteNonQuery();

        using var installDatabaseCommand =
            new SqlCommand(string.Format(GetInstallSqlScript("DatabaseInstall"), DatabaseName), sqlConnection);
        installDatabaseCommand.ExecuteNonQuery();
    }

    private static string GetInstallSqlScript(string scriptName)
    {
        var assembly = typeof(DatabaseUtils).Assembly;
        using var stream = assembly.GetManifestResourceStream($"SampleWebApplication.Utils.{scriptName}.sql") ?? 
                           throw new InvalidOperationException($"Database script is not found in the assembly `{assembly}`.");
        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }
}
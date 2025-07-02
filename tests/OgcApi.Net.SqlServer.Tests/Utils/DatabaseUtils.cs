using Microsoft.Data.SqlClient;
using System;
using System.IO;

namespace OgcApi.SqlServer.Tests.Utils;

public static class DatabaseUtils
{
    public const string DatabaseName = "OgcApiTests";

    private const string ConnectionStringTemplateEnvVariable = "CONNECTION_STRING_TEMPLATE";

    private const string DbConnectionString = "Server=localhost; Database={0}; User Id=sa; Password=SqlServer_Password; Encrypt=false; ";

    public static void RecreateDatabase()
    {
        using var sqlConnection = new SqlConnection(string.Format(GetConnectionStringTemplate(), "master"));
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
        using var stream = assembly.GetManifestResourceStream($"OgcApi.Net.SqlServer.Tests.Utils.{scriptName}.sql");

        if (stream == null)
        {
            throw new InvalidOperationException($"Database script is not found in the assembly `{assembly}`.");
        }

        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }

    private static string GetConnectionStringTemplate()
    {
        return Environment.GetEnvironmentVariable(ConnectionStringTemplateEnvVariable) ?? DbConnectionString;
    }

    public static string GetConnectionString()
    {
        return string.Format(GetConnectionStringTemplate(), DatabaseName);
    }
}
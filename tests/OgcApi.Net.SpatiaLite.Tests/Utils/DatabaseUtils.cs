using Microsoft.Data.Sqlite;
using OgcApi.Net.SpatiaLite;
using System;
using System.IO;

namespace OgcApi.SpatiaLite.Tests.Utils;

public static class DatabaseUtils
{
    public const string DatabaseName = "OgcApiTests.db";

    private const string ConnectionStringTemplateEnvVariable = "SQLITE_CONNECTION_STRING_TEMPLATE";

    private const string DbConnectionString = "Data Source={0}";

    public static void RecreateDatabase()
    {
        if (File.Exists(DatabaseName))
            File.Delete(DatabaseName);

        using var sqlConnection = new SpatiaLiteConnection(GetConnectionString());
        sqlConnection.Open();

        var script = string.Format(GetInstallSpatiaLiteScript("DatabaseInstall"), DatabaseName);

        using var installDatabaseCommand = new SqliteCommand(script, sqlConnection);
        installDatabaseCommand.ExecuteNonQuery();
        // FIXME: this second command intentional
        // Geometry columns only work when table is created, dropped and created again
        // nasty hack needs to be fixed
        installDatabaseCommand.ExecuteNonQuery();
    }

    private static string GetInstallSpatiaLiteScript(string scriptName)
    {
        var assembly = typeof(DatabaseUtils).Assembly;
        using var stream = assembly.GetManifestResourceStream($"OgcApi.Net.SpatiaLite.Tests.Utils.{scriptName}.sql");

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
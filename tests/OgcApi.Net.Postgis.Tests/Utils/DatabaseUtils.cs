using Npgsql;
using System;
using System.IO;

namespace OgcApi.Net.PostGis.Tests.Utils;

public static class DatabaseUtils
{
    public const string DatabaseName = "ogc_api_tests";

    private const string ConnectionStringTemplateEnvVariable = "POSTGRES_CONNECTION_STRING_TEMPLATE";

    private const string DbConnectionString = "Host=127.0.0.1;User Id=postgres;Password=password;Database={0};Port=5432;";

    public static void RecreateDatabase()
    {
        using var createDbSqlConnection = new NpgsqlConnection(string.Format(GetConnectionStringTemplate(), "postgres"));
        createDbSqlConnection.Open();

        var assembly = typeof(DatabaseUtils).Assembly;
        using var stream = assembly.GetManifestResourceStream("OgcApi.Net.PostGis.Tests.Utils.DatabaseCreate.sql") ?? 
                           throw new InvalidOperationException($"Database script is not found in the assembly `{assembly}`.");
        using var streamReader = new StreamReader(stream);
        while (streamReader.ReadLine() is { } line)
        {
            using var createDatabaseCommand =
                new NpgsqlCommand(string.Format(line, DatabaseName), createDbSqlConnection);
            createDatabaseCommand.ExecuteNonQuery();
        }

        createDbSqlConnection.Close();

        using var installDbSqlConnection = new NpgsqlConnection(GetConnectionString());
        installDbSqlConnection.Open();

        using var installDatabaseCommand =
            new NpgsqlCommand(string.Format(GetInstallSqlScript(), DatabaseName), installDbSqlConnection);
        installDatabaseCommand.ExecuteNonQuery();
    }

    private static string GetInstallSqlScript()
    {
        var assembly = typeof(DatabaseUtils).Assembly;
        using var stream = assembly.GetManifestResourceStream("OgcApi.Net.PostGis.Tests.Utils.DatabaseInstall.sql") ?? 
                           throw new InvalidOperationException($"Database script is not found in the assembly `{assembly}`.");
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
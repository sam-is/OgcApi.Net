using Microsoft.Data.Sqlite;
using System.Threading;
using System.Threading.Tasks;

namespace OgcApi.Net.SpatiaLite;

public class SpatiaLiteConnection : SqliteConnection
{
    // Path to your SpatiaLite module (e.g., mod_spatialite.dll)
    // Make sure this DLL and its dependencies (GEOS, PROJ, etc.) are accessible.
    // A common approach is to place them in your application's output directory.
    private const string SpatiaLiteModulePath = "mod_spatialite"; // or "mod_spatialite.dll" on Windows etc.

    public SpatiaLiteConnection() : base() { }
    public SpatiaLiteConnection(string connectionString) : base(connectionString) { }

    public override void Open()
    {
        base.Open();

        InitializeSpatialDatabase();
    }
    public override async Task OpenAsync(CancellationToken cancellationToken)
    {
        await base.OpenAsync(cancellationToken);

        await InitializeSpatialDatabaseAsync(cancellationToken);
    }

    private const string SPATIALITE_INIT_CMD = """
                SELECT InitSpatialMetaData()
                WHERE NOT EXISTS (SELECT * FROM sqlite_master WHERE type = 'table' AND name = 'spatial_ref_sys');
            """;

    private void InitializeSpatialDatabase()
    {
        // enable extension loading - a crucial step for security reasons in SQLite.
        EnableExtensions();

        // Load the SpatiaLite extension
        // The filename should be just the name of the module (e.g., "mod_spatialite").
        // SQLite will automatically add the correct platform-specific extension (.dll, .so, .dylib).
        // Ensure the module is in a discoverable location (e.g., same directory as your app).
        LoadExtension(SpatiaLiteModulePath);

        // verify SpatiaLite is loaded by calling a SpatiaLite function
        using (var command = CreateCommand())
        {
            // initialize SpatiaLite metadata (if not already initialized, i.e. call only once per database)
            // this creates the core SpatiaLite system tables.
            command.CommandText = SPATIALITE_INIT_CMD;
            command.ExecuteNonQuery();
        }
    }
    private async Task InitializeSpatialDatabaseAsync(CancellationToken token)
    {
        // enable extension loading - a crucial step for security reasons in SQLite.
        EnableExtensions();

        // Load the SpatiaLite extension
        // The filename should be just the name of the module (e.g., "mod_spatialite").
        // SQLite will automatically add the correct platform-specific extension (.dll, .so, .dylib).
        // Ensure the module is in a discoverable location (e.g., same directory as your app).
        LoadExtension(SpatiaLiteModulePath);

        // verify SpatiaLite is loaded by calling a SpatiaLite function
        using (var command = CreateCommand())
        {
            // initialize SpatiaLite metadata (if not already initialized, i.e. call only once per database)
            // this creates the core SpatiaLite system tables.
            command.CommandText = SPATIALITE_INIT_CMD;
            await command.ExecuteScalarAsync(token);
        }
    }
}

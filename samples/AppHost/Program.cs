using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql-server")
    .WithDataVolume();

var sqlServerDataBase = sqlServer.AddDatabase("ogc-api-samples-sql-server", "OgcApiSamples")
    .WithCreationScript(File.ReadAllText(Path.Join(Path.GetDirectoryName(typeof(Program).Assembly.Location), "Seeds", "SqlServer", "DatabaseInstall.sql")));

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "OgcApiSamples")
    .WithBindMount("Seeds/Postgres", "/docker-entrypoint-initdb.d")
    .WithImage("postgis/postgis")
    .WithDataVolume();

var postgresDataBase = postgres.AddDatabase("ogc-api-samples-postgres", "OgcApiSamples");

var isTest = builder.Environment.IsEnvironment("Test");

var webApplication = builder.AddProject<SampleWebApplication>("web-application", launchProfileName: "SampleWebApplication")
    .WaitFor(sqlServerDataBase)
    .WaitFor(postgresDataBase)
    .WithReference(sqlServerDataBase, "SqlServerConnectionString")
    .WithReference(postgresDataBase, "PostgresConnectionString")
    .WithEnvironment("OgcSettingsFileName", isTest ? "ogcapi-tests.json" : "ogcapi.json");

if (isTest)
{
    builder.AddContainer("ogc-tests", "ogccite/ets-ogcapi-features10")
    .WaitFor(webApplication)
    .WithReference(webApplication)
    .WithHttpEndpoint(targetPort: 8080)
    .WithUrlForEndpoint("http", url => url.Url = "/teamengine");
}

builder.Build().Run();

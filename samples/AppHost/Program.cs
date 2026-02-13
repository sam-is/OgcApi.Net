using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sqlPassword", value: "TestP@ssw0rd", secret: true);
var postgresPassword = builder.AddParameter("postgresPassword", value: "TestP@ssw0rd", secret: true);

var sqlServer = builder.AddSqlServer("sql-server")
    .WithDataVolume()
    .WithPassword(sqlPassword);

var sqlServerDataBase = sqlServer.AddDatabase("ogc-api-samples-sql-server", "OgcApiSamples")
    .WithCreationScript(File.ReadAllText(Path.Join(Path.GetDirectoryName(typeof(Program).Assembly.Location), "Seeds", "SqlServer", "DatabaseInstall.sql")));

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "OgcApiSamples")
    .WithBindMount("Seeds/Postgres", "/docker-entrypoint-initdb.d")
    .WithImage("postgis/postgis")
    .WithDataVolume()
    .WithPassword(postgresPassword);

var postgresDataBase = postgres.AddDatabase("ogc-api-samples-postgres", "OgcApiSamples");

var isTest = builder.Environment.IsEnvironment("Test");

var webApplication = builder.AddProject<SampleWebApplication>("web-application")
    .WaitFor(sqlServerDataBase)
    .WaitFor(postgresDataBase)
    .WithReference(sqlServerDataBase, "SqlServerConnectionString")
    .WithReference(postgresDataBase, "PostgresConnectionString")
    .WithEnvironment("OgcSettingsFileName", isTest ? "ogcapi-tests.json" : "ogcapi.json")
    .WithUrlForEndpoint("http", url =>
    {
        url.Url = "/swagger";
        url.DisplayText = "Swagger (http)";
    })
    .WithUrlForEndpoint("https", url =>
    {
        url.Url = "/swagger";
        url.DisplayText = "Swagger (https)";
    });

webApplication
    .WithUrl($"{webApplication.GetEndpoint("http")}/scalar", "Scalar (http)")
    .WithUrl($"{webApplication.GetEndpoint("https")}/scalar", "Scalar (https)");


if (isTest)
{
    builder.AddContainer("ogc-tests", "ogccite/ets-ogcapi-features10:latest")
    .WaitFor(webApplication)
    .WithReference(webApplication)
    .WithHttpEndpoint(targetPort: 8080)
    .WithUrlForEndpoint("http", url => url.Url = "/teamengine");
}

builder.Build().Run();

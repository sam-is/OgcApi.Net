using Microsoft.OpenApi;
using System;
using Xunit;

namespace OgcApi.Net.Options.Tests;

public class OpenApiVersionTests
{
    [Fact]
    public void DefaultOpenApiVersionInOptions()
    {
        var options = new OgcApiOptions();

        Assert.Equal("3.1", options.OpenApiVersion);
    }

    [Theory]
    [InlineData("2.0", OpenApiSpecVersion.OpenApi2_0)]
    [InlineData("3.0", OpenApiSpecVersion.OpenApi3_0)]
    [InlineData("3.1", OpenApiSpecVersion.OpenApi3_1)]
    [InlineData("3.2", OpenApiSpecVersion.OpenApi3_2)]
    public void MappingValidOpenApiVersion(string version, OpenApiSpecVersion expectedVersion)
    {
        var openApiVersion = Net.Utils.GetOpenApiSpecVersion(version);

        Assert.Equal(expectedVersion, openApiVersion);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("1.0")]
    [InlineData("ogc")]
    [InlineData("2.3")]
    [InlineData("0.5")]
    public void MappingInvalidOpenApiVersion(string version)
    {
        Assert.Throws<NotImplementedException>(() => Net.Utils.GetOpenApiSpecVersion(version));
    }
}

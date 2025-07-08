using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.DataProviders;

namespace OgcApi.Net.Schemas.Tests.Mock;
internal static class MockUtils
{
    public static IServiceProvider CreateServiceProviderWithFeaturesProvider() => new ServiceCollection()
            .AddSingleton<IFeaturesProvider>(new MockFeaturesProvider())
            .BuildServiceProvider();

    public static IServiceProvider CreateServiceProviderWithTilesProviders() => new ServiceCollection()
            .AddSingleton<ITilesProvider>(new MockTilesProvider())
            .BuildServiceProvider();
    public static IServiceProvider CreateServiceProviderWithFeaturesProviders() => new ServiceCollection()
        .AddSingleton<IFeaturesProvider>(new MockAnotherFeaturesProvider())
        .AddSingleton<IFeaturesProvider>(new MockFeaturesProvider())
        .BuildServiceProvider();

    public static IServiceProvider CreateEmptyServiceProvider() => new ServiceCollection().BuildServiceProvider();
}

using Microsoft.Extensions.Options;
using OgcApi.Net.Styles.Options;
using OgcApi.Net.Styles.Security;
using System;
using System.Threading.Tasks;

namespace SampleWebApplication.Security;

public class SampleStylesAuthorizationService(IOptionsMonitor<OgcApiStylesOptions> stylesOptions) : IStylesAuthorizationService
{
    private readonly bool _useAuthorization = stylesOptions.CurrentValue?.UseAuthorization ?? false;

    public Task Authorize(string apiKey = null, string baseResource = null, string styleId = null)
    {
        if (apiKey != "admin" && _useAuthorization)
            throw new UnauthorizedAccessException();
        return Task.CompletedTask;
    }
}
